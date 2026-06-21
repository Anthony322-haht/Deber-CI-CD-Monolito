using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Datos.Modelos;
using Capa_Negocio;

namespace Monolito_4am_DB.Seguridad
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar cookie "Recordarme"
                HttpCookie cookie = Request.Cookies["RecordarUsuario"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    txt_ced.Text = cookie.Value;
                    CheckBox1.Checked = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string cedula = txt_ced.Text.Trim();
            string password = txt_pass.Text.Trim();

            // 1. Validaciones por si JavaScript falla
            if (string.IsNullOrEmpty(cedula) || string.IsNullOrEmpty(password))
            {
                return; // Cortamos el código, JS ya mostró el SweetAlert
            }

            // 2. PRIMERO buscamos si el usuario existe (Solo con la cédula)
            tbl_usuario usinfo = CN_tbl_usuario.traerced(cedula);

            if (usinfo != null)
            {
                // --> EL USUARIO SÍ EXISTE <--

                // Sacamos los intentos que le quedan (si está nulo en la BD, le asignamos 3 por defecto)
                int intentosRestantes = usinfo.usu_intentos ?? 3;

                // Verificamos si ya está bloqueado antes de comprobar la contraseña
                if (intentosRestantes <= 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Cuenta Bloqueada', 'Has superado el límite de intentos. Contacta al administrador.', 'error');", true);
                    return; // Detenemos el proceso
                }

                // 3. Verificamos la contraseña
                bool existecc = CN_tbl_usuario.autentixcc(cedula, password);

                if (existecc)
                {
                    // --> LOGIN EXITOSO <--

                    // Como entro correctamente, le devolvemos sus intentos a 3
                    usinfo.usu_intentos = 3;
                    CN_tbl_usuario.modificarIntentos(usinfo);

                    // Cookie "Recordarme"
                    if (CheckBox1.Checked)
                    {
                        HttpCookie cookie = new HttpCookie("RecordarUsuario", cedula);
                        cookie.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        // Eliminar cookie si no quiere recordar
                        HttpCookie cookie = new HttpCookie("RecordarUsuario");
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cookie);
                    }

                    // Verificamos si es una clave temporal (estado T)
                    if (usinfo.usu_estado == "T")
                    {
                        // Necesitamos forzar el cambio de clave. 
                        string tokenCifrado = CN_tbl_usuario.GenerarTokenRecuperacion(usinfo.usu_correo);
                        if (!string.IsNullOrEmpty(tokenCifrado))
                        {
                            Response.Redirect("RestablecerClave.aspx?token=" + Server.UrlEncode(tokenCifrado));
                            return;
                        }
                    }

                    //  2FA (OTP/QR)           
                    // Guardar  sesion
                    // punto de quiembre
                    Session["Pending2FA_Cedula"] = cedula;
                    hfCedula2FA.Value = cedula;

                    //  Gg OTP d.v r.r e.c
                    // empaquetado C tublas muchas var
                    var otpData = CN_tbl_usuario.GenerarYGuardarOTP(cedula);
                    
                    if (otpData != null)
                    {
                        // c bo y 4 par 
                        bool enviado = Mail.EnviarCorreoOTP(usinfo.usu_correo, usinfo.usu_nombres, otpData.Item1, otpData.Item2);//Tupla emp

                        // Mostrar panel 2FA
                        pnlLogin.Visible = false;
                        pnl2FA.Visible = true;
                        
                        if (enviado)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Seguridad 2FA', 'Hemos enviado un código a tu correo electrónico.', 'info');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Advertencia', 'Tuvimos un problema enviando el correo, pero puedes intentar generar otro código o contactar soporte.', 'warning');", true);
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Error', 'No se pudo generar el código 2FA.', 'error');", true);
                    }
                }
                else
                {
                    // --> CONTRASEÑA INCORRECTA <--

                    // Le restamos 1 intento
                    intentosRestantes = intentosRestantes - 1;
                    usinfo.usu_intentos = intentosRestantes;

                    // Si al restar llegó a cero, lo bloqueamos (Opcional: cambiar estado a inactivo si tu BD lo maneja)
                    // if (intentosRestantes == 0) usinfo.usu_estado = "B";

                    // Guardamos el nuevo numero de intentos y la fecha del intento en la BD
                    usinfo.usu_fecha_ultimo_intento = DateTime.Now;
                    CN_tbl_usuario.modificarIntentos(usinfo);

                    if (intentosRestantes > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", $"Swal.fire('Contraseña Incorrecta', 'Te quedan {intentosRestantes} intentos.', 'warning');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Bloqueado', 'Has agotado tus intentos. La cuenta ha sido bloqueada.', 'error');", true);
                    }
                }
            }
            else
            {
                // --> EL USUARIO NO EXISTE EN LA BASE DE DATOS <--
                ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Usuario No Encontrado', 'Verifica tu número de identificación.', 'warning');", true);
            }
        }

        protected void lnk_registrar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registra.aspx");
        }

        protected void lnk_olvido_Click(object sender, EventArgs e)
        {
            Response.Redirect("RecuperarClave.aspx");
        }
        protected void btnValidar2FA_Click(object sender, EventArgs e)
        {
            string cedula = Session["Pending2FA_Cedula"] as string;
            if (string.IsNullOrEmpty(cedula))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Sesión Expirada', 'Por favor vuelve a iniciar sesión.', 'error').then(function(){ window.location='Login.aspx'; });", true);
                return;
            }

            // Verificamos si el usuario escaneo un QR o si escribio el codigo manualmente
            string codigoQR = hfQRResult.Value.Trim();
            string codigoManual = txtCodigo2FA.Text.Trim();

            bool esQR = !string.IsNullOrEmpty(codigoQR);
            string codigoAValidar = esQR ? codigoQR : codigoManual;

            if (string.IsNullOrEmpty(codigoAValidar))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Campo Vacío', 'Por favor ingresa el código de 6 dígitos o escanea el QR.', 'warning');", true);
                return;
            }

            // Validar en la Capa de Negocio
            bool esValido = CN_tbl_usuario.ValidarOTP(cedula, codigoAValidar, esQR);

            if (esValido)
            {
                // Login Completo! Limpiar sesion temporal y crear sesion de usuario
                Session.Remove("Pending2FA_Cedula");
                
                var usinfo = CN_tbl_usuario.traerced(cedula);
                Session["usuario"] = usinfo;
                
                // Redirigir al Dashboard
                Response.Redirect("../Default.aspx", false);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Código Incorrecto', 'El código ingresado o escaneado no es válido.', 'error');", true);
            }
        }
    }
}
