using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Negocio;

namespace Monolito_4am_DB.Mantenimiento
{
    public partial class listar_tbl_producto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Gestión de Productos";
            if (!IsPostBack)
            {
                ViewState["SortExpression"] = "pro_id";
                ViewState["SortDirection"] = "DESC";
                CargarGrid();
            }
        }

        private string GridSortDirection
        {
            get { return ViewState["SortDirection"] as string ?? "DESC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private string GridSortExpression
        {
            get { return ViewState["SortExpression"] as string ?? "pro_id"; }
            set { ViewState["SortExpression"] = value; }
        }

        private void CargarGrid(string searchTerms = "")
        {
            try
            {
                var productos = CN_tbl_producto.traertodosproductos();
                var provDict = CN_tbl_proveedor.traertodosproveedores().ToDictionary(p => p.prov_id, p => p.prov_nombre);

                // Filtrado en tiempo real
                if (!string.IsNullOrEmpty(searchTerms))
                {
                    string filtro = ddlFiltro.SelectedValue;
                    if (string.IsNullOrEmpty(filtro))
                    {
                        // Búsqueda inteligente en múltiples campos a la vez
                        productos = productos.Where(p => 
                            (p.pro_nombre != null && p.pro_nombre.IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (p.prov_id != null && provDict.ContainsKey(p.prov_id) && provDict[p.prov_id].IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
                    }
                    else if (filtro == "Nombre")
                    {
                        productos = productos.Where(p => p.pro_nombre != null && p.pro_nombre.IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    }
                    else if (filtro == "Proveedor")
                    {
                        productos = productos.Where(p => p.prov_id != null && provDict.ContainsKey(p.prov_id) && provDict[p.prov_id].IndexOf(searchTerms, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    }
                    else if (filtro == "Estado")
                    {
                        if (searchTerms.IndexOf("activo", StringComparison.OrdinalIgnoreCase) >= 0 && searchTerms.IndexOf("inactivo", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            productos = productos.Where(p => p.pro_estado == "A").ToList();
                        }
                        else if (searchTerms.IndexOf("inactivo", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            productos = productos.Where(p => p.pro_estado == "I").ToList();
                        }
                    }
                }

                // Ordenación predeterminada descendente por ID (últimos agregados primero)
                productos = productos.OrderByDescending(p => p.pro_id).ToList();

                var imagenesPorProducto = CN_tbl_imagen_producto.traertodaslasimagenes()
                    .GroupBy(img => img.pro_id)
                    .ToDictionary(g => g.Key, g => g.OrderBy(i => i.imgp_orden).ThenBy(i => i.imgp_id).First().imgp_ruta);

                var data = productos.Select(p => new
                {
                    p.pro_id,
                    p.pro_nombre,
                    p.pro_cantidad,
                    p.pro_precio,
                    prov_nombre = p.prov_id != null && provDict.ContainsKey(p.prov_id) ? provDict[p.prov_id] : "Sin proveedor",
                    pro_estado = !string.IsNullOrEmpty(p.pro_estado) ? p.pro_estado : "I",
                    primera_imagen = p.pro_id != null && imagenesPorProducto.ContainsKey(p.pro_id) ? imagenesPorProducto[p.pro_id] : "/Uploads/Productos/default.png"
                }).ToList();

                // Lógica de Ordenamiento Dinámico
                string sortExp = GridSortExpression;
                string sortDir = GridSortDirection;

                if (sortDir == "ASC")
                {
                    if (sortExp == "pro_id") data = data.OrderBy(x => x.pro_id).ToList();
                    else if (sortExp == "pro_nombre") data = data.OrderBy(x => x.pro_nombre).ToList();
                    else if (sortExp == "pro_cantidad") data = data.OrderBy(x => x.pro_cantidad).ToList();
                    else if (sortExp == "pro_precio") data = data.OrderBy(x => x.pro_precio).ToList();
                    else if (sortExp == "prov_nombre") data = data.OrderBy(x => x.prov_nombre).ToList();
                    else if (sortExp == "pro_estado") data = data.OrderBy(x => x.pro_estado).ToList();
                }
                else
                {
                    if (sortExp == "pro_id") data = data.OrderByDescending(x => x.pro_id).ToList();
                    else if (sortExp == "pro_nombre") data = data.OrderByDescending(x => x.pro_nombre).ToList();
                    else if (sortExp == "pro_cantidad") data = data.OrderByDescending(x => x.pro_cantidad).ToList();
                    else if (sortExp == "pro_precio") data = data.OrderByDescending(x => x.pro_precio).ToList();
                    else if (sortExp == "prov_nombre") data = data.OrderByDescending(x => x.prov_nombre).ToList();
                    else if (sortExp == "pro_estado") data = data.OrderByDescending(x => x.pro_estado).ToList();
                }

                gvProductos.DataSource = data;
                gvProductos.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar productos: " + ex.Message, false);
            }
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = (esExito ? "<i class='fa-solid fa-circle-check mr-2'></i>" : "<i class='fa-solid fa-circle-xmark mr-2'></i>") + mensaje;
            lblMensaje.CssClass = esExito
                ? "d-block alert-msg alert alert-success"
                : "d-block alert-msg alert alert-danger";
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            pnlMensaje.Visible = false;
            gvProductos.PageIndex = 0; // Reiniciar página al buscar
            CargarGrid(txtBuscar.Text.Trim());
        }

        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // TIP: Este evento se dispara cuando el usuario hace clic en los numeros de paginacion abajo de la grilla.
            // Le decimos al GridView a qué pagina ir (NewPageIndex) y volvemos a consultar la base de datos
            // pasando el texto de busqueda para que no se pierda el filtro en la nueva pagina.
            gvProductos.PageIndex = e.NewPageIndex;
            CargarGrid(txtBuscar.Text.Trim());
        }

        protected void gvProductos_Sorting(object sender, GridViewSortEventArgs e)
        {
            string currentSortExpression = GridSortExpression;
            string currentSortDirection = GridSortDirection;

            // Si es la misma columna, invierte la dirección. Si es nueva, por defecto ASC.
            if (currentSortExpression == e.SortExpression)
            {
                GridSortDirection = (currentSortDirection == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                GridSortDirection = "ASC";
                GridSortExpression = e.SortExpression;
            }

            gvProductos.PageIndex = 0;
            CargarGrid(txtBuscar.Text.Trim());
        }

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Page") return; // Ignorar comandos de paginación nativos

            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                Response.Redirect("nuevo_tbl_producto.aspx?id=" + id);
            }
            else if (e.CommandName == "Desactivar")
            {
                try
                {
                    var producto = CN_tbl_producto.traerproductoxidSinEstado(id);

                    if (producto != null)
                    {
                        CN_tbl_producto.elimiLog(producto); // Cambia estado a 'I'
                        MostrarMensaje("El producto fue desactivado correctamente.", true);
                        CargarGrid(txtBuscar.Text.Trim());
                    }
                    else
                    {
                        MostrarMensaje("No se encontró el producto.", false);
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error: No se pudo desactivar el producto. " + ex.Message, false);
                }
            }
            else if (e.CommandName == "Activar")
            {
                try
                {
                    var producto = CN_tbl_producto.traerproductoxidSinEstado(id);

                    if (producto != null)
                    {
                        CN_tbl_producto.activar(producto); // Cambia estado a 'A'
                        MostrarMensaje("El producto fue activado correctamente.", true);
                        CargarGrid(txtBuscar.Text.Trim());
                    }
                    else
                    {
                        MostrarMensaje("No se encontró el producto.", false);
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error: No se pudo activar el producto. " + ex.Message, false);
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    var producto = CN_tbl_producto.traerproductoxidSinEstado(id);

                    if (producto != null)
                    {
                        CN_tbl_producto.delete(producto); // Eliminado FÍSICO
                        MostrarMensaje("El producto fue eliminado permanentemente de la base de datos.", true);
                        CargarGrid(txtBuscar.Text.Trim());
                    }
                    else
                    {
                        MostrarMensaje("No se encontró el producto a eliminar.", false);
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error: No se pudo eliminar este producto. " + ex.Message, false);
                }
            }
        }
    }
}
