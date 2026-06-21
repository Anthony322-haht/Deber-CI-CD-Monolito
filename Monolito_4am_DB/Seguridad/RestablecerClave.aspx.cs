using System;
using Capa_Negocio;

namespace Monolito_4am_DB.Seguridad
{
    public partial class RestablecerClave : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string token = Request.QueryString["token"];
                
                // Si no hay token en la URL, mostramos el panel de error directamente
                if (string.IsNullOrEmpty(token))
                {
                    pnlFormulario.Style["display"] = "none";
                    pnlError.Style["display"] = "block";
                }
                else
                {
                    pnlFormulario.Style["display"] = "block";
                    pnlError.Style["display"] = "none";
                }
            }
        }

        protected void btnRestablecer_Click(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];
            string nuevaClave = txtClave1.Text.Trim();

            if (string.IsNullOrEmpty(token))
            {
                MostrarAlertaYRedirigir("error", "Error", "Token no proporcionado.", "RecuperarClave.aspx");
                return;
            }

            try
            {
                bool exito = CN_tbl_usuario.RestablecerClaveConToken(token, nuevaClave);

                if (exito)
                {
                    MostrarAlertaYRedirigir("success", "Contraseña Actualizada", "Tu contraseña ha sido cambiada exitosamente. Ahora puedes iniciar sesión.", "Login.aspx");
                }
                else
                {
                    // Token invalido o expirado
                    pnlFormulario.Style["display"] = "none";
                    pnlError.Style["display"] = "block";
                }
            }
            catch (Exception ex)
            {
                string script = $"Swal.fire({{ icon: 'error', title: 'Error del Sistema', text: 'Ocurrió un error: {ex.Message}', confirmButtonColor: '#e11d48' }});";
                ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", script, true);
            }
        }

        private void MostrarAlertaYRedirigir(string tipo, string titulo, string mensaje, string urlDestino)
        {
            string script = $"Swal.fire({{ " +
                $"icon: '{tipo}', " +
                $"title: '{titulo}', " +
                $"text: '{mensaje}', " +
                $"confirmButtonColor: '#2563eb' " +
                $"}}).then(function() {{ window.location.href = '{urlDestino}'; }});";
                
            ClientScript.RegisterStartupScript(this.GetType(), "SweetAlertRedirect", script, true);
        }
    }
}
