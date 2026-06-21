using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Datos.Modelos;
using Capa_Negocio;

namespace Monolito_4am_DB.Mantenimiento
{
    public partial class listar_tbl_categoria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Gesti\u00f3n de Categor\u00edas";
            if (!IsPostBack)
            {
                CargarGrid();
            }
        }

        private void CargarGrid()
        {
            try
            {
                gvCategorias.DataSource = CN_tbl_categoria.traertodascategorias();
                gvCategorias.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar categorías: " + ex.Message, "error");
            }
        }

        protected void gvCategorias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                Response.Redirect("nuevo_tbl_categoria.aspx?id=" + id);
            }
            else if (e.CommandName == "Desactivar")
            {
                try
                {
                    var categoria = CN_tbl_categoria.traercategoriaxidSinEstado(id);
                    if (categoria != null)
                    {
                        CN_tbl_categoria.elimiLog(categoria);
                        MostrarMensaje("Categoría desactivada correctamente.", "success");
                        CargarGrid();
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al desactivar: " + ex.Message, "error");
                }
            }
            else if (e.CommandName == "Activar")
            {
                try
                {
                    var categoria = CN_tbl_categoria.traercategoriaxidSinEstado(id);
                    if (categoria != null)
                    {
                        CN_tbl_categoria.activar(categoria);
                        MostrarMensaje("Categoría activada correctamente.", "success");
                        CargarGrid();
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al activar: " + ex.Message, "error");
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    var categoria = CN_tbl_categoria.traercategoriaxidSinEstado(id);
                    if (categoria != null)
                    {
                        CN_tbl_categoria.delete(categoria);
                        MostrarMensaje("Categoría eliminada permanentemente.", "success");
                        CargarGrid();
                    }
                }
                catch (Exception)
                {
                    MostrarMensaje("No se puede eliminar porque existen productos asignados a esta categoría.", "error");
                }
            }
        }

        protected void gvCategorias_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategorias.PageIndex = e.NewPageIndex;
            CargarGrid();
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            // Muestra mensaje usando SweetAlert2
            string icon = tipo == "success" ? "success" : "error";
            string titulo = tipo == "success" ? "\u00a1\u00c9xito!" : "\u00a1Error!";
            string script = $"Swal.fire('{titulo}', '{mensaje}', '{icon}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "swal_msg", script, true);
        }
    }
}
