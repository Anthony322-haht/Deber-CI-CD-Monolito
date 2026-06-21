using System;
using System.Net;
using System.Net.Mail;

namespace Capa_Negocio
{
    public static class Mail
    {
        // Credenciales proporcionadas
        private static readonly string CorreoRemitente = "veraanthony127@gmail.com";
        private static readonly string ClaveApp = "qejp czrt eyuc xbig"; // Clave de aplicación de Gmail

        /// <summary>
        /// Envía un correo electrónico con el enlace de recuperación de contraseña.
        /// </summary>
        public static bool EnviarCorreoRecuperacion(string correoDestino, string nombres, string enlaceRecuperacion)
        {
            try
            {
                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(CorreoRemitente, "Soporte Monolito 4AM");
                mensaje.To.Add(correoDestino);
                mensaje.Subject = "Recuperación de Contraseña - Monolito 4AM";
                mensaje.IsBodyHtml = true;

                // Plantilla HTML del correo
                string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px;'>
                    <h2 style='color: #2563eb; text-align: center;'>Recuperación de Contraseña</h2>
                    <p>Hola <strong>{nombres}</strong>,</p>
                    <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en Monolito 4AM.</p>
                    <p>Si fuiste tú, haz clic en el siguiente botón para crear una nueva contraseña. Este enlace expira en 15 minutos.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{enlaceRecuperacion}' style='background-color: #2563eb; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;'>Restablecer Contraseña</a>
                    </div>
                    <p style='font-size: 12px; color: #666;'>Si no solicitaste este cambio, simplemente ignora este correo. Tu cuenta sigue estando segura.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                    <p style='font-size: 11px; color: #999; text-align: center;'>Si el botón no funciona, copia y pega este enlace en tu navegador:<br>{enlaceRecuperacion}</p>
                </div>";

                mensaje.Body = body;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(CorreoRemitente, ClaveApp);
                    smtp.EnableSsl = true; // Gmail requiere SSL
                    smtp.Send(mensaje);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Aquí podrías loggear el error
                Console.WriteLine("Error al enviar correo: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Envía un correo electrónico con el código OTP y el QR encriptado para el login 2FA.
        /// </summary>
        public static bool EnviarCorreoOTP(string correoDestino, string nombres, string otpPlano, string otpEncriptado)
        {
            try
            {
                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(CorreoRemitente, "Seguridad Monolito 4AM");
                mensaje.To.Add(correoDestino);
                mensaje.Subject = "Código de Seguridad 2FA - Monolito 4AM";
                mensaje.IsBodyHtml = true;

                // Generar URL del QR usando una API pública (QR Server)
                string urlQR = "https://api.qrserver.com/v1/create-qr-code/?size=250x250&data=" + Uri.EscapeDataString(otpEncriptado);

                // Plantilla HTML del correo
                string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; text-align: center;'>
                    <h2 style='color: #2563eb;'>Doble Factor de Autenticación</h2>
                    <p>Hola <strong>{nombres}</strong>,</p>
                    <p>Has intentado iniciar sesión en tu cuenta. Para continuar, por favor ingresa el siguiente código de seguridad o escanea el código QR con la cámara desde la pantalla de login.</p>
                    
                    <div style='background: #f3f4f6; border-radius: 8px; padding: 15px; margin: 20px 0; font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #1f2937;'>
                        {otpPlano}
                    </div>

                    <p>O escanea este código QR desde tu pantalla:</p>
                    <img src='{urlQR}' alt='Código QR 2FA' style='border-radius: 8px; border: 2px solid #e0e0e0; width: 200px; height: 200px;' />

                    <p style='font-size: 12px; color: #666; margin-top: 30px;'>Este código expirará en unos minutos por tu seguridad. Si no intentaste iniciar sesión, ignora este correo y te recomendamos cambiar tu contraseña.</p>
                </div>";

                mensaje.Body = body;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(CorreoRemitente, ClaveApp);
                    smtp.EnableSsl = true; // Gmail requiere SSL
                    smtp.Send(mensaje);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo OTP: " + ex.Message);
                return false;
            }
        }
    }
}
