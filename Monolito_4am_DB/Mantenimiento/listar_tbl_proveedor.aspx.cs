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
    public partial class listar_tbl_proveedor : System.Web.UI.Page
    {
        // C página en el servidor
        protected void Page_Load(object sender, EventArgs e)//text titulo
        {
            Page.Title = "Gesti\u00f3n de Proveedores";

            // primera carga la página
            if (!IsPostBack)
            {
                CargarGrid();//Inv M
            }
        }

        // Met
        private void CargarGrid()
        {   //alt visu
            try
            {
                // Salta a la C_Negocio y lis  prove 
                var proveedores = CN_tbl_proveedor.traertodosproveedores();

                // estado  
                string estado = ddlFiltroEstado.SelectedValue;//fil
                if (!string.IsNullOrEmpty(estado))
                {
                    proveedores = proveedores.Where(p => p.prov_estado == estado).ToList();
                }   //fil lis 

                // F coicidencia ing M y m
                string nombre = txtBuscarNombre.Text.Trim();
                if (!string.IsNullOrEmpty(nombre))
                {
                    proveedores = proveedores.Where(p => p.prov_nombre.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }  

                // Ordenación predeterminada descendente por ID (últimos agregados primero)
                proveedores = proveedores.OrderByDescending(p => p.prov_id).ToList();

                // Enlaza la lista procesada al control GridView de la pantalla
                gvProveedores.DataSource = proveedores;
                gvProveedores.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar proveedores: " + ex.Message, "error");
            }
        }

        // evento para el botón Buscar
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvProveedores.PageIndex = 0; // reinicia a la primera página antes de buscar
            CargarGrid();
        }

        // evento para limpiar filtros y restablecer la tabla
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlFiltroEstado.SelectedIndex = 0;
            txtBuscarNombre.Text = string.Empty;
            gvProveedores.PageIndex = 0; // R:P
            CargarGrid();
        }

        // controla Paginación
        protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex; //nuevo índice p
            CargarGrid();
        }

        // Evento
        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Si el comando pertenece a la paginación nativa del GridView, ignora el resto del código
            if (e.CommandName == "Page") return;

            // Captura el ID del proveedor de la fila seleccionada
            string id = e.CommandArgument.ToString();

            // Opción: Editar registro
            if (e.CommandName == "Editar")
            {
                // Redirige al formulario enviando el ID por la URL (QueryString)
                Response.Redirect("nuevo_tbl_proveedor.aspx?id=" + id);
            }
            // Opción: Desactivar (Eliminado Lógico)
            else if (e.CommandName == "Desactivar")
            {
                try
                {
                    var proveedor = CN_tbl_proveedor.traerproveedorxidSinEstado(id);
                    if (proveedor != null)
                    {
                        CN_tbl_proveedor.elimiLog(proveedor); // cambia el estado a 'I' en la Capa de Negocio
                        MostrarMensaje("Proveedor desactivado correctamente.", "success");
                        CargarGrid(); // R:tab
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al desactivar: " + ex.Message, "error");
                }
            }
            // Activar registro
            else if (e.CommandName == "Activar")
            {
                try
                {
                    var proveedor = CN_tbl_proveedor.traerproveedorxidSinEstado(id);
                    if (proveedor != null)
                    {
                        CN_tbl_proveedor.activar(proveedor); // Cambia el estado a 'A' en la Capa de Negocio
                        MostrarMensaje("Proveedor activado correctamente.", "success");
                        CargarGrid();
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al activar: " + ex.Message, "error");
                }
            }
            // Eliminar (Eliminado Físico)
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    var proveedor = CN_tbl_proveedor.traerproveedorxidSinEstado(id);
                    if (proveedor != null)
                    {
                        CN_tbl_proveedor.delete(proveedor); // Hace el DELETE definitivo de la base de datos
                        MostrarMensaje("Proveedor eliminado permanentemente.", "success");
                        CargarGrid();
                    }
                }
                catch (Exception ex)
                {
                    // Atrapa errores complejos de SQL (como restricciones de llave foránea) y limpia el texto para JS
                    MostrarMensaje("Error real del servidor: " + ex.Message.Replace("'", "\"").Replace("\r", "").Replace("\n", " "), "error");
                }
            }
        }

        // Método Sweet alert  
        private void MostrarMensaje(string mensaje, string tipo)
        {
            string icon = tipo == "success" ? "success" : "error";
            string titulo = tipo == "success" ? "\u00a1\u00c9xito!" : "\u00a1Error!";
            string script = $"Swal.fire('{titulo}', '{mensaje}', '{icon}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "swal_msg", script, true);
        }
    }
}
