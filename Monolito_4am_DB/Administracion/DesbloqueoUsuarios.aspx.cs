using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Negocio;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Monolito_4am_DB.Administracion
{
    public partial class DesbloqueoUsuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Seguridad: Validar que sea Administrador
                var usuario = Session["usuario"] as tbl_usuario;
                if (usuario == null || usuario.tusu_id != "1")
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                CargarUsuarios();
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                var usuarios = CN_tbl_usuario.ObtenerTodosLosUsuarios();
                gvUsuarios.DataSource = usuarios;
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar usuarios: " + ex.Message;
                lblMensaje.CssClass = "text-danger d-block mb-3";
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Desbloquear")
            {
                string usu_id = e.CommandArgument.ToString();
                
                bool exito = CN_tbl_usuario.DesbloquearUsuario(usu_id);
                
                if (exito)
                {
                    lblMensaje.Text = "Usuario desbloqueado exitosamente.";
                    lblMensaje.CssClass = "text-success fw-bold d-block mb-3";
                    CargarUsuarios(); // Recargar grilla
                }
                else
                {
                    lblMensaje.Text = "Hubo un error al intentar desbloquear al usuario.";
                    lblMensaje.CssClass = "text-danger d-block mb-3";
                }
            }
        }
    }
}
