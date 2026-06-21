using System;
using Capa_Negocio;

namespace Monolito_4am_DB.Seguridad
{
    public partial class RecuperarClave : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRecuperar_Click(object sender, EventArgs e)
        {
            string identificador = txtIdentificador.Text.Trim();
            if (string.IsNullOrEmpty(identificador))
            {
                MostrarAlerta("error", "Campo Vacio", "Por favor ingresa tu correo o cedula.");
                return;
            }

            try
            {
                // 1. Buscar usuario
                var usuario = CN_tbl_usuario.ObtenerUsuarioPorIdentificador(identificador);

                if (usuario == null)
                {
                    // Mensaje generico por seguridad
                    MostrarAlerta("info", "Solicitud Recibida", "Si el correo o cedula existe en nuestro sistema, recibiras un enlace de recuperacion.");
                    txtIdentificador.Text = "";
                    return;
                }

                // 2. Generar clave temporal de 6 digitos
                string claveTemporal = WhatsApp.GenerarClaveTemporal();

                // 3. Guardar la clave temporal en la BD (encriptada) y marcar estado como 'T' (Temporal)
                CN_tbl_usuario.GuardarClaveTemporal(usuario, claveTemporal);

                // 4. Intentar enviar por WhatsApp
                bool enviadoWhatsApp = false;
                if (!string.IsNullOrEmpty(usuario.usu_celular))
                {
                    enviadoWhatsApp = WhatsApp.EnviarClaveTemporal(
                        usuario.usu_celular,
                        usuario.usu_nombres,
                        claveTemporal
                    );
                }

                // 5. Tambien generar token y guardarlo en el HiddenField
                string tokenCifrado = CN_tbl_usuario.GenerarTokenRecuperacion(usuario.usu_cedula);
                
                if (enviadoWhatsApp && !string.IsNullOrEmpty(tokenCifrado))
                {
                    // Guardar datos en campos ocultos para el Paso 2
                    hfCedula.Value = usuario.usu_cedula;
                    hfToken.Value = tokenCifrado;

                    // Cambiar visibilidad de paneles
                    pnlPaso1.Visible = false;
                    pnlPaso2.Visible = true;

                    MostrarAlerta("success", "Enviado por WhatsApp",
                        "Tu codigo de 6 digitos fue enviado a tu WhatsApp. Ingresalo abajo.");
                }
                else
                {
                    MostrarAlerta("error", "Error de Envio",
                        "No pudimos enviar el codigo a tu WhatsApp en este momento. Verifica tu numero e intenta mas tarde.");
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("error", "Error del Sistema", "Ocurrio un error: " + ex.Message.Replace("'", ""));
            }
        }

        protected void btnValidar_Click(object sender, EventArgs e)
        {
            string codigoIngresado = txtCodigo.Text.Trim();
            string cedula = hfCedula.Value;
            string token = hfToken.Value;

            if (string.IsNullOrEmpty(codigoIngresado) || string.IsNullOrEmpty(cedula))
            {
                MostrarAlerta("error", "Datos Faltantes", "Por favor ingresa el codigo.");
                return;
            }

            try
            {
                // Delegamos la validacion a la Capa de Negocio para evitar referencias directas a Capa_Datos en la UI
                bool codigoCorrecto = CN_tbl_usuario.ValidarClaveTemporalWhatsApp(cedula, codigoIngresado);

                if (codigoCorrecto)
                {
                    // Codigo correcto, redirigir a RestablecerClave con el token
                    Response.Redirect("RestablecerClave.aspx?token=" + Server.UrlEncode(token), false);
                }
                else
                {
                    MostrarAlerta("error", "Codigo Incorrecto", "El codigo ingresado no es valido o la sesion expiro. Revisa tu WhatsApp e intenta de nuevo.");
                }
            }
            catch (Exception ex)
            {
                MostrarAlerta("error", "Error del Sistema", "Ocurrio un error al validar: " + ex.Message.Replace("'", ""));
            }
        }

        private void MostrarAlerta(string tipo, string titulo, string mensaje)
        {
            string script = "Swal.fire({ icon: '" + tipo + "', title: '" + titulo + "', text: '" + mensaje + "', confirmButtonColor: '#2563eb' });";
            ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", script, true);
        }
    }
}
