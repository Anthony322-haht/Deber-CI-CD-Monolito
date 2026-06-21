using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExcelDataReader;
using Capa_Datos;
using Capa_Datos.Modelos;
using Capa_Negocio;

namespace Monolito_4am_DB.Mantenimiento
{
    public partial class importar_excel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Importar Excel";
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            pnlMensaje.Visible = false;
            pnlResultados.Visible = false;
            pnlPreview.Visible = false;
            divLog.InnerHtml = "";

            if (!fileUploadExcel.HasFile)
            {
                MostrarMensaje("Debe seleccionar un archivo.", false);
                return;
            }

            try
            {
                string ext = Path.GetExtension(fileUploadExcel.FileName).ToLower();
                if (ext != ".xlsx" && ext != ".xls" && ext != ".csv")
                {
                    MostrarMensaje("El formato del archivo no es válido.", false);
                    return;
                }

                string pathTemp = Server.MapPath("~/Uploads/Import/");
                if (!Directory.Exists(pathTemp))
                    Directory.CreateDirectory(pathTemp);

                string filePath = Path.Combine(pathTemp, Guid.NewGuid().ToString() + ext);
                fileUploadExcel.SaveAs(filePath);

                DataTable dtExcel = LeerExcelCSV(filePath, ext);

                if (dtExcel == null || dtExcel.Rows.Count == 0)
                {
                    MostrarMensaje("El archivo está vacío o no se pudo leer.", false);
                    return;
                }

                PrevisualizarDatos(dtExcel);

                // Borrar archivo temporal
                if (File.Exists(filePath))
                    File.Delete(filePath);

                // Cerrar loading SweetAlert y mostrar log
                ScriptManager.RegisterStartupScript(this, GetType(), "close_swal", "Swal.close();", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Ocurrió un error inesperado: " + ex.Message, false);
                ScriptManager.RegisterStartupScript(this, GetType(), "close_swal_err", "Swal.close();", true);
            }
        }

        private DataTable LeerExcelCSV(string filePath, string ext)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader reader = null;
                    if (ext == ".xls")
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (ext == ".xlsx")
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else if (ext == ".csv")
                    {
                        var conf = new ExcelReaderConfiguration()
                        {
                            FallbackEncoding = System.Text.Encoding.GetEncoding(1252),
                            AutodetectSeparators = new char[] { ',', ';', '\t', '|' }
                        };
                        reader = ExcelReaderFactory.CreateCsvReader(stream, conf);
                    }

                    if (reader != null)
                    {
                        // TIP: Usamos AsDataSet para convertir el Excel a una tabla virtual (DataTable) en memoria.
                        // La configuracion 'UseHeaderRow' asegura que la primera fila se use como nombre de columnas.
                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };

                        var dataSet = reader.AsDataSet(conf);
                        if (dataSet.Tables.Count > 0)
                        {
                            dt = dataSet.Tables[0];
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }

        private void PrevisualizarDatos(DataTable dt)
        {
            var productosDB = CN_tbl_producto.traertodosproductos()
                .GroupBy(p => p.pro_nombre.Trim().ToLower())
                .ToDictionary(g => g.Key, g => g.First());

            List<ProductoExcelPreview> previewList = new List<ProductoExcelPreview>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                int numeroFila = i + 2;
                
                string colNombre = ObtenerValorColumna(dt, row, "Nombre", "Pro_Nombre", "Producto");
                if (string.IsNullOrWhiteSpace(colNombre)) continue;

                string colCantidad = ObtenerValorColumna(dt, row, "Cantidad", "Pro_Cantidad");
                string colPrecio = ObtenerValorColumna(dt, row, "Precio", "Pro_Precio");
                string colCategoria = ObtenerValorColumna(dt, row, "Categoria", "Cat_Nombre", "Categoría");
                string colProveedor = ObtenerValorColumna(dt, row, "Proveedor", "Prov_Nombre", "Prov");
                string colImagenes = ObtenerValorColumna(dt, row, "Imagenes", "Fotos", "Imagen", "Image");

                int cantidad = 0;
                int.TryParse(colCantidad, out cantidad);
                
                decimal precio = 0;
                if (!string.IsNullOrWhiteSpace(colPrecio))
                {
                    colPrecio = colPrecio.Replace(",", ".");
                    decimal.TryParse(colPrecio, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out precio);
                }

                string accion = productosDB.ContainsKey(colNombre.Trim().ToLower()) ? "Actualizar" : "Nuevo";

                List<string> listaImagenes = new List<string>();
                if (!string.IsNullOrWhiteSpace(colImagenes))
                {
                    var fotos = colImagenes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var f in fotos)
                    {
                        string fotoName = f.Trim();
                        // Agregar ruta virtual de Uploads
                        listaImagenes.Add("/Uploads/Productos/" + fotoName);
                    }
                }
                if(listaImagenes.Count == 0)
                {
                    listaImagenes.Add("data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%22300%22%20height%3D%22300%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20viewBox%3D%220%200%20300%20300%22%20preserveAspectRatio%3D%22none%22%3E%3Cdefs%3E%3Cstyle%20type%3D%22text%2Fcss%22%3E%23holder_1%20text%20%7B%20fill%3A%2394a3b8%3Bfont-weight%3Abold%3Bfont-family%3AArial%2C%20Helvetica%2C%20Open%20Sans%2C%20sans-serif%2C%20monospace%3Bfont-size%3A18pt%20%7D%20%3C%2Fstyle%3E%3C%2Fdefs%3E%3Cg%20id%3D%22holder_1%22%3E%3Crect%20width%3D%22300%22%20height%3D%22300%22%20fill%3D%22%23f1f5f9%22%3E%3C%2Frect%3E%3Cg%3E%3Ctext%20x%3D%2295%22%20y%3D%22158%22%3ESin%20Imagen%3C%2Ftext%3E%3C%2Fg%3E%3C%2Fg%3E%3C%2Fsvg%3E");
                }

                previewList.Add(new ProductoExcelPreview
                {
                    Fila = numeroFila,
                    Nombre = colNombre.Trim(),
                    Cantidad = cantidad,
                    Precio = precio,
                    Categoria = colCategoria?.Trim(),
                    Proveedor = colProveedor?.Trim(),
                    Imagenes = listaImagenes,
                    ImagenesRaw = colImagenes,
                    Accion = accion
                });
            }

            // TIP: Guardamos los datos procesados temporalmente en la Session.
            // Esto permite que el usuario los vea en la tabla de previsualización
            // y decida si confirmarlos (guardarlos en BD) o cancelarlos, sin haber tocado la base de datos aun.
            Session["PreviewData"] = previewList;
            
            // OPTIMIZACION VISUAL: Si hay 10,000 filas, dibujar 10,000 <tr> congelaría Chrome.
            // Solo dibujamos los primeros 100 para la previsualizacion.
            if (previewList.Count > 100)
            {
                rptPreview.DataSource = previewList.Take(100).ToList();
                // Podrias agregar un label que diga "Mostrando 100 de X registros..."
            }
            else
            {
                rptPreview.DataSource = previewList;
            }
            
            rptPreview.DataBind();
            
            pnlPreview.Visible = true;
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            var previewList = Session["PreviewData"] as List<ProductoExcelPreview>;
            if (previewList == null || previewList.Count == 0)
            {
                MostrarMensaje("No hay datos para confirmar.", false);
                return;
            }

            pnlPreview.Visible = false;
            pnlResultados.Visible = true;
            divLog.InnerHtml = "";

            ProcesarDatosPrevisualizados(previewList);
            Session.Remove("PreviewData");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("PreviewData");
            pnlPreview.Visible = false;
            pnlResultados.Visible = false;
            pnlMensaje.Visible = false;
        }

        private void ProcesarDatosPrevisualizados(List<ProductoExcelPreview> lista)
        {
            int insertados = 0;
            int actualizados = 0;
            int errores = 0;
            int provCreados = 0;
            int catCreadas = 0;

            divLog.InnerHtml += $"<div class='log-info'>[{DateTime.Now:HH:mm:ss}] Iniciando procesamiento de {lista.Count} filas...</div>";

            var proveedoresDB = CN_tbl_proveedor.traerproveedores()
                .GroupBy(p => p.prov_nombre.Trim().ToLower())
                .ToDictionary(g => g.Key, g => g.First().prov_id);

            var categoriasDB = CN_tbl_categoria.traercategorias()
                .GroupBy(c => c.cat_nombre.Trim().ToLower())
                .ToDictionary(g => g.Key, g => g.First().cat_id);

            var productosDB = CN_tbl_producto.traertodosproductos()
                .GroupBy(p => p.pro_nombre.Trim().ToLower())
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var item in lista)
            {
                try
                {
                    // 1. Obtener o Crear Categoría
                    string idCategoria = "";
                    if (!string.IsNullOrWhiteSpace(item.Categoria))
                    {
                        string catKey = item.Categoria.Trim().ToLower();
                        if (categoriasDB.ContainsKey(catKey))
                        {
                            idCategoria = categoriasDB[catKey];
                        }
                        else
                        {
                            tbl_categoria nuevaCat = new tbl_categoria { cat_nombre = item.Categoria.Trim() };
                            CN_tbl_categoria.save(nuevaCat);
                            idCategoria = nuevaCat.cat_id;
                            categoriasDB.Add(catKey, idCategoria);
                            catCreadas++;
                            divLog.InnerHtml += $"<div class='log-info'>[{DateTime.Now:HH:mm:ss}] Fila {item.Fila}: Categoría creada -> '{nuevaCat.cat_nombre}'</div>";
                        }
                    }

                    // 2. Obtener o Crear Proveedor
                    string idProveedor = null;
                    if (!string.IsNullOrWhiteSpace(item.Proveedor))
                    {
                        string provKey = item.Proveedor.Trim().ToLower();
                        if (proveedoresDB.ContainsKey(provKey))
                        {
                            idProveedor = proveedoresDB[provKey];
                        }
                        else
                        {
                            tbl_proveedor nuevoProv = new tbl_proveedor { prov_nombre = item.Proveedor.Trim() };
                            CN_tbl_proveedor.save(nuevoProv);
                            idProveedor = nuevoProv.prov_id;
                            proveedoresDB.Add(provKey, idProveedor);
                            provCreados++;
                            divLog.InnerHtml += $"<div class='log-info'>[{DateTime.Now:HH:mm:ss}] Fila {item.Fila}: Proveedor creado -> '{nuevoProv.prov_nombre}'</div>";
                        }
                    }

                    // 3. Insertar o Actualizar Producto
                    tbl_producto prodParaImagenes = null;
                    string proKey = item.Nombre.Trim().ToLower();
                    if (productosDB.ContainsKey(proKey))
                    {
                        var prodExistente = productosDB[proKey];
                        prodExistente.pro_cantidad = item.Cantidad;
                        prodExistente.pro_precio = item.Precio;
                        
                        if (!string.IsNullOrEmpty(idCategoria)) prodExistente.cat_id = idCategoria;
                        if (!string.IsNullOrEmpty(idProveedor)) prodExistente.prov_id = idProveedor;

                        CN_tbl_producto.modify(prodExistente);
                        actualizados++;
                        divLog.InnerHtml += $"<div class='log-success'>[{DateTime.Now:HH:mm:ss}] Fila {item.Fila}: Producto Actualizado -> '{prodExistente.pro_nombre}'</div>";
                        prodParaImagenes = prodExistente;
                    }
                    else
                    {
                        tbl_producto nuevoProd = new tbl_producto
                        {
                            pro_nombre = item.Nombre.Trim(),
                            pro_cantidad = item.Cantidad,
                            pro_precio = item.Precio,
                            cat_id = string.IsNullOrEmpty(idCategoria) ? null : idCategoria,
                            prov_id = string.IsNullOrEmpty(idProveedor) ? null : idProveedor
                        };

                        CN_tbl_producto.save(nuevoProd);
                        insertados++;
                        divLog.InnerHtml += $"<div class='log-success'>[{DateTime.Now:HH:mm:ss}] Fila {item.Fila}: Producto Insertado -> '{nuevoProd.pro_nombre}'</div>";
                        productosDB.Add(proKey, nuevoProd);
                        prodParaImagenes = nuevoProd;
                    }

                    // 4. Procesar Imágenes (Solo si se proporcionó algo en la columna)
                    if (prodParaImagenes != null && !string.IsNullOrWhiteSpace(item.ImagenesRaw))
                    {
                        var fotosArray = item.ImagenesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        var imagenesActuales = CN_tbl_imagen_producto.traerimagenxproducto(prodParaImagenes.pro_id);

                        foreach (string f in fotosArray)
                        {
                            string nomFoto = f.Trim();
                            if (!string.IsNullOrEmpty(nomFoto))
                            {
                                string rutaEsperada = "/Uploads/Productos/" + nomFoto;
                                bool yaExiste = imagenesActuales.Any(img => img.imgp_ruta.Equals(rutaEsperada, StringComparison.OrdinalIgnoreCase));
                                
                                if (!yaExiste)
                                {
                                    tbl_imagen_producto imgProd = new tbl_imagen_producto();
                                    imgProd.pro_id = prodParaImagenes.pro_id;
                                    imgProd.imgp_nombre = nomFoto;
                                    imgProd.imgp_ruta = rutaEsperada;
                                    imgProd.imgp_orden = 0;
                                    CN_tbl_imagen_producto.save(imgProd);
                                    
                                    divLog.InnerHtml += $"<div class='log-info'>[{DateTime.Now:HH:mm:ss}] Fila {item.Fila}: Imagen enlazada -> '{nomFoto}'</div>";
                                }
                            }
                        }
                    }
                }
                catch (Exception exRow)
                {
                    divLog.InnerHtml += $"<div class='log-error'>[{DateTime.Now:HH:mm:ss}] Fila {item.Fila}: ERROR -> {exRow.Message}</div>";
                    errores++;
                }
            }

            divLog.InnerHtml += $"<hr style='border-color: #e2e8f0;' />";
            divLog.InnerHtml += $"<div class='log-info'><strong>Resumen:</strong></div>";
            divLog.InnerHtml += $"<div class='log-success'>Nuevos: {insertados} | Actualizados: {actualizados} | Categorias nuevas: {catCreadas} | Prov. nuevos: {provCreados}</div>";
            if (errores > 0)
                divLog.InnerHtml += $"<div class='log-error'>Errores: {errores}</div>";

            divLog.InnerHtml += $"<div class='mt-4 text-center'><a href='listar_tbl_producto.aspx' class='btn btn-primary btn-lg shadow-sm'><i class='fa-solid fa-box mr-2'></i>Ir a Gestionar Productos</a></div>";

            string tituloMsg = errores == 0 ? "¡Exportación Exitosa!" : "Importación con Advertencias";
            string iconoMsg = errores == 0 ? "success" : "warning";
            string script = $"Swal.fire('{tituloMsg}', 'El proceso finalizó. Nuevos: {insertados}, Actualizados: {actualizados}, Errores: {errores}', '{iconoMsg}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "swal_import_success", script, true);
        }

        // Metodo auxiliar para leer columnas flexiblemente
        private string ObtenerValorColumna(DataTable dt, DataRow row, params string[] posiblesNombres)
        {
            foreach (var nombre in posiblesNombres)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        return row[col] != DBNull.Value ? row[col].ToString() : "";
                    }
                }
            }
            // Si no encontró por nombre, intenta por prefijo
            foreach (var nombre in posiblesNombres)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.StartsWith(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        return row[col] != DBNull.Value ? row[col].ToString() : "";
                    }
                }
            }
            return "";
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = (esExito ? "<i class='fa-solid fa-circle-check mr-2'></i>" : "<i class='fa-solid fa-circle-xmark mr-2'></i>") + mensaje;
            lblMensaje.CssClass = esExito
                ? "d-block alert-msg alert alert-success"
                : "d-block alert-msg alert alert-danger";
        }
    }

    [Serializable]
    public class ProductoExcelPreview
    {
        public int Fila { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public string Proveedor { get; set; }
        public string Accion { get; set; }
        public List<string> Imagenes { get; set; }
        public string ImagenesRaw { get; set; }
    }
}
