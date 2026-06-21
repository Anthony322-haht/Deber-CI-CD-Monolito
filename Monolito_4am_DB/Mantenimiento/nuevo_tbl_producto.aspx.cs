using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Capa_Datos;
using Capa_Datos.Modelos;
using Capa_Negocio;

namespace Monolito_4am_DB.Mantenimiento
{
    public partial class nuevo_tbl_producto : System.Web.UI.Page
    {
        // Propiedad para manejar la lista temporal de imágenes en ViewState
        private List<ImagenTemp> ListaImagenesTemp
        {
            get
            {
                if (ViewState["ListaImagenesTemp"] == null)
                    ViewState["ListaImagenesTemp"] = new List<ImagenTemp>();
                return (List<ImagenTemp>)ViewState["ListaImagenesTemp"];
            }
            set { ViewState["ListaImagenesTemp"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarProveedores();
                CargarCategorias();

                // Si viene un ID por QueryString, cargar datos para editar
                if (Request.QueryString["id"] != null)
                {
                    string id = Request.QueryString["id"];
                    CargarProducto(id);
                }
            }
        }

        private void CargarProveedores()
        {
            var proveedores = CN_tbl_proveedor.traerproveedores();

            ddlProveedor.DataSource = proveedores;
            ddlProveedor.DataTextField = "prov_nombre";
            ddlProveedor.DataValueField = "prov_id";
            ddlProveedor.DataBind();

            ddlProveedor.Items.Insert(0, new ListItem("-- Selecciona un proveedor --", ""));
        }

        private void CargarCategorias()
        {
            var categorias = CN_tbl_categoria.traercategorias();

            ddlCategoria.DataSource = categorias;
            ddlCategoria.DataTextField = "cat_nombre";
            ddlCategoria.DataValueField = "cat_id";
            ddlCategoria.DataBind();

            ddlCategoria.Items.Insert(0, new ListItem("-- Selecciona una categoría --", ""));
        }

        private void CargarProducto(string id)
        {
            // Usar metodo sin filtro de estado para poder editar productos inactivos tambien
            var producto = CN_tbl_producto.traerproductoxidSinEstado(id);
            if (producto != null)
            {
                txtNombre.Text = producto.pro_nombre;
                txtCantidad.Text = producto.pro_cantidad.HasValue ? producto.pro_cantidad.Value.ToString() : "0";
                // Usar InvariantCulture para que el separador decimal sea punto (.) y no coma (,)
                txtPrecio.Text = producto.pro_precio.HasValue
                    ? producto.pro_precio.Value.ToString("F2", CultureInfo.InvariantCulture)
                    : "0.00";
                ddlProveedor.SelectedValue = !string.IsNullOrEmpty(producto.prov_id) ? producto.prov_id : "";
                ddlCategoria.SelectedValue = !string.IsNullOrEmpty(producto.cat_id) ? producto.cat_id : "";
                
                RefrescarImagenesActuales(id);
            }
        }

        private void RefrescarImagenesActuales(string pro_id)
        {
            var imagenes = CN_tbl_imagen_producto.traerimagenxproducto(pro_id);
            pnlImagenesActuales.Visible = imagenes.Count > 0;
            rptImagenesActuales.DataSource = imagenes;
            rptImagenesActuales.DataBind();
        }

        private void LimpiarErrores()
        {
            lblErrorNombre.Visible = false;
            lblErrorCantidad.Visible = false;
            lblErrorPrecio.Visible = false;
            lblErrorProveedor.Visible = false;
            lblErrorCategoria.Visible = false;
            pnlMensaje.Visible = false;

            txtNombre.CssClass = "form-control form-control-soft";
            txtCantidad.CssClass = "form-control form-control-soft";
            txtPrecio.CssClass = "form-control form-control-soft";
            ddlProveedor.CssClass = "form-control form-control-soft";
            ddlCategoria.CssClass = "form-control form-control-soft";
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = (esExito ? "<i class='fa-solid fa-circle-check mr-2'></i>" : "<i class='fa-solid fa-circle-xmark mr-2'></i>") + mensaje;
            lblMensaje.CssClass = esExito
                ? "d-block alert-msg alert alert-success"
                : "d-block alert-msg alert alert-danger";
        }

        private bool ValidarFormulario()
        {
            bool esValido = true;

            // --- Validar Nombre ---
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblErrorNombre.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> El nombre del producto es obligatorio.";
                lblErrorNombre.Visible = true;
                txtNombre.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else if (txtNombre.Text.Trim().Length < 2)
            {
                lblErrorNombre.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> El nombre debe tener al menos 2 caracteres.";
                lblErrorNombre.Visible = true;
                txtNombre.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else
            {
                // Verificar duplicado al crear
                if (Request.QueryString["id"] == null)
                {
                    if (CN_tbl_producto.verificarproductounico(txtNombre.Text.Trim()))
                    {
                        lblErrorNombre.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> Ya existe un producto con este nombre.";
                        lblErrorNombre.Visible = true;
                        txtNombre.CssClass = "form-control form-control-soft input-error";
                        esValido = false;
                    }
                }
                else
                {
                    // Verificar duplicado al editar (excluyendo el actual)
                    string idActual = Request.QueryString["id"];
                    var productoExistente = CN_tbl_producto.traertodosproductos()
                        .FirstOrDefault(p => p.pro_nombre.Equals(txtNombre.Text.Trim(), StringComparison.OrdinalIgnoreCase) && p.pro_id != idActual);

                    if (productoExistente != null)
                    {
                        lblErrorNombre.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> Ya existe otro producto con este nombre.";
                        lblErrorNombre.Visible = true;
                        txtNombre.CssClass = "form-control form-control-soft input-error";
                        esValido = false;
                    }
                }
            }

            // --- Validar Cantidad ---
            int cantidad;
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                lblErrorCantidad.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> La cantidad es obligatoria.";
                lblErrorCantidad.Visible = true;
                txtCantidad.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else if (!int.TryParse(txtCantidad.Text, out cantidad))
            {
                lblErrorCantidad.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> La cantidad debe ser un numero entero valido.";
                lblErrorCantidad.Visible = true;
                txtCantidad.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else if (cantidad < 0)
            {
                lblErrorCantidad.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> La cantidad no puede ser negativa.";
                lblErrorCantidad.Visible = true;
                txtCantidad.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }

            // --- Validar Precio ---
            decimal precio;
            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                lblErrorPrecio.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> El precio es obligatorio.";
                lblErrorPrecio.Visible = true;
                txtPrecio.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out precio))
            {
                lblErrorPrecio.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> El precio debe ser un numero valido.";
                lblErrorPrecio.Visible = true;
                txtPrecio.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else if (precio < 0)
            {
                lblErrorPrecio.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> El precio no puede ser negativo.";
                lblErrorPrecio.Visible = true;
                txtPrecio.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }
            else if (precio == 0)
            {
                lblErrorPrecio.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> El precio debe ser mayor a cero.";
                lblErrorPrecio.Visible = true;
                txtPrecio.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }

            // --- Validar Proveedor ---
            if (string.IsNullOrEmpty(ddlProveedor.SelectedValue))
            {
                lblErrorProveedor.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> Debe seleccionar un proveedor.";
                lblErrorProveedor.Visible = true;
                ddlProveedor.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }

            // --- Validar Categoría ---
            if (string.IsNullOrEmpty(ddlCategoria.SelectedValue))
            {
                lblErrorCategoria.Text = "<i class='fa-solid fa-triangle-exclamation mr-1'></i> Debe seleccionar una categoría.";
                lblErrorCategoria.Visible = true;
                ddlCategoria.CssClass = "form-control form-control-soft input-error";
                esValido = false;
            }

            return esValido;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LimpiarErrores();

            if (!ValidarFormulario())
                return;

            try
            {
                // Parsear precio con InvariantCulture para que funcione con punto decimal
                decimal precio = decimal.Parse(txtPrecio.Text, NumberStyles.Any, CultureInfo.InvariantCulture);

                if (Request.QueryString["id"] != null)
                {
                    // EDITAR producto existente
                    string id = Request.QueryString["id"];
                    var producto = CN_tbl_producto.traerproductoxidSinEstado(id);
                    if (producto != null)
                    {
                        producto.pro_nombre = txtNombre.Text.Trim();
                        producto.pro_cantidad = Convert.ToInt32(txtCantidad.Text);
                        producto.pro_precio = precio;
                        producto.prov_id = ddlProveedor.SelectedValue;
                        producto.cat_id = ddlCategoria.SelectedValue;

                        CN_tbl_producto.modify(producto);
                        GuardarImagenesFinales(producto.pro_id);
                        MostrarMensaje("Producto actualizado correctamente.", true);
                    }
                    else
                    {
                        MostrarMensaje("No se encontro el producto a editar.", false);
                    }
                }
                else
                {
                    // NUEVO producto
                    tbl_producto producto = new tbl_producto();
                    producto.pro_nombre = txtNombre.Text.Trim();
                    producto.pro_cantidad = Convert.ToInt32(txtCantidad.Text);
                    producto.pro_precio = precio;
                    producto.prov_id = ddlProveedor.SelectedValue;
                    producto.cat_id = ddlCategoria.SelectedValue;

                    CN_tbl_producto.save(producto);
                    GuardarImagenesFinales(producto.pro_id);
                    MostrarMensaje("Producto registrado correctamente.", true);

                    // Limpiar formulario
                    txtNombre.Text = "";
                    txtCantidad.Text = "0";
                    txtPrecio.Text = "0.00";
                    ddlProveedor.SelectedIndex = 0;
                    ddlCategoria.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar el producto: " + ex.Message, false);
            }
        }

        // --- MÉTODOS DE IMÁGENES ---

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            LimpiarErrores();
            if (fileUploadImagenes.HasFiles)
            {
                string pathTemp = Server.MapPath("~/Uploads/Temp/");
                if (!Directory.Exists(pathTemp))
                    Directory.CreateDirectory(pathTemp);

                var lista = ListaImagenesTemp;

                foreach (HttpPostedFile uploadedFile in fileUploadImagenes.PostedFiles)
                {
                    string ext = Path.GetExtension(uploadedFile.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                    {
                        // TIP: Usamos Guid.NewGuid() para generar un nombre de archivo único aleatorio.
                        // Así evitamos que si suben dos fotos llamadas "zapato.jpg", una sobrescriba a la otra.
                        string fileName = Guid.NewGuid().ToString() + ext;
                        string rutaFisica = Path.Combine(pathTemp, fileName);
                        uploadedFile.SaveAs(rutaFisica);

                        lista.Add(new ImagenTemp
                        {
                            RutaFisica = rutaFisica,
                            RutaVirtual = "/Uploads/Temp/" + fileName,
                            Nombre = uploadedFile.FileName
                        });
                    }
                }
                ListaImagenesTemp = lista;
                RefrescarGaleria();
                MostrarMensaje("Imagen(es) agregada(s) a la previsualización correctamente.", true);
            }
        }

        protected void rptImagenesTemp_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                string rutaFisica = e.CommandArgument.ToString();
                var lista = ListaImagenesTemp;
                
                var itemToRemove = lista.FirstOrDefault(x => x.RutaFisica == rutaFisica);
                if (itemToRemove != null)
                {
                    lista.Remove(itemToRemove);
                    if (File.Exists(rutaFisica))
                    {
                        File.Delete(rutaFisica);
                    }
                }

                ListaImagenesTemp = lista;
                RefrescarGaleria();
            }
        }

        private void RefrescarGaleria()
        {
            var lista = ListaImagenesTemp;
            pnlGaleria.Visible = lista.Count > 0;
            rptImagenesTemp.DataSource = lista;
            rptImagenesTemp.DataBind();
        }

        protected void rptImagenesActuales_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EliminarActual")
            {
                string imgp_id = e.CommandArgument.ToString();
                var img = CN_tbl_imagen_producto.traerimagenxid(imgp_id);
                if (img != null)
                {
                    // Obtener la ruta física para borrar el archivo del servidor
                    string rutaFisica = Server.MapPath("~" + img.imgp_ruta);
                    
                    // Eliminar de base de datos
                    CN_tbl_imagen_producto.delete(img);
                    
                    // Eliminar archivo físico
                    if (File.Exists(rutaFisica))
                    {
                        try { File.Delete(rutaFisica); } catch { /* Ignore file lock issues */ }
                    }

                    MostrarMensaje("Imagen eliminada correctamente.", true);
                    RefrescarImagenesActuales(img.pro_id);
                }
            }
        }

        private void GuardarImagenesFinales(string pro_id)
        {
            var listaTemp = ListaImagenesTemp;
            if (listaTemp.Count > 0)
            {
                string pathFinal = Server.MapPath("~/Uploads/Productos/");
                if (!Directory.Exists(pathFinal))
                    Directory.CreateDirectory(pathFinal);

                foreach (var imgTemp in listaTemp)
                {
                    // Move file from Temp to Productos folder
                    string fileName = Path.GetFileName(imgTemp.RutaFisica);
                    string finalRutaFisica = Path.Combine(pathFinal, fileName);
                    
                    if (File.Exists(imgTemp.RutaFisica))
                    {
                        File.Move(imgTemp.RutaFisica, finalRutaFisica);
                        
                        // Guardar en base de datos
                        tbl_imagen_producto nuevaImg = new tbl_imagen_producto();
                        nuevaImg.pro_id = pro_id;
                        nuevaImg.imgp_ruta = "/Uploads/Productos/" + fileName;
                        nuevaImg.imgp_nombre = imgTemp.Nombre;
                        nuevaImg.imgp_orden = 0; // Opcional
                        
                        CN_tbl_imagen_producto.save(nuevaImg);
                    }
                }

                // Limpiar lista temporal
                ListaImagenesTemp = new List<ImagenTemp>();
                RefrescarGaleria();
                
                // Actualizar la grilla de imágenes actuales (si estamos editando)
                if (Request.QueryString["id"] != null)
                {
                    RefrescarImagenesActuales(pro_id);
                }
                else
                {
                    pnlImagenesActuales.Visible = false;
                }
            }
        }
    }

    [Serializable]
    public class ImagenTemp
    {
        public string RutaFisica { get; set; }
        public string RutaVirtual { get; set; }
        public string Nombre { get; set; }
    }
}
