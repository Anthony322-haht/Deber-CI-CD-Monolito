using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio
{
    public static class WhatsApp
    {
        // Credenciales de Twilio
        private static readonly string AccountSid = "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        private static readonly string AuthToken = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        private static readonly string NumeroTwilio = "whatsapp:+14155238886"; // Sandbox de Twilio

        /// <summary>
        /// Envia una clave temporal por WhatsApp al numero del usuario.
        /// El numero debe estar en formato internacional sin +, ej: 593987654321
        /// </summary>
        public static bool EnviarClaveTemporal(string numeroCelular, string nombreUsuario, string claveTemporal)
        {
            try
            {
                // Formatear numero para WhatsApp (agregar codigo de pais Ecuador si empieza con 09)
                string numeroFormateado = FormatearNumero(numeroCelular);

                string mensaje = $"🔐 *Monolito 4AM - Clave Temporal*\n\n" +
                    $"Hola *{nombreUsuario}*,\n\n" +
                    $"Tu clave temporal es:\n" +
                    $"👉 *{claveTemporal}*\n\n" +
                    $"⚠️ *IMPORTANTE:* Esta clave es temporal. Al iniciar sesion, el sistema te pedira crear una nueva contrasena segura.\n\n" +
                    $"Si no solicitaste este cambio, ignora este mensaje.\n\n" +
                    $"_Monolito 4AM Security_";

                // Enviar usando la API REST de Twilio
                string url = $"https://api.twilio.com/2010-04-01/Accounts/{AccountSid}/Messages.json";

                using (var client = new WebClient())
                {
                    // Autenticacion basica con Account SID y Auth Token
                    string credenciales = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}"));
                    client.Headers[HttpRequestHeader.Authorization] = "Basic " + credenciales;
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    string postData = $"From={Uri.EscapeDataString(NumeroTwilio)}" +
                        $"&To={Uri.EscapeDataString("whatsapp:" + numeroFormateado)}" +
                        $"&Body={Uri.EscapeDataString(mensaje)}";

                    string respuesta = client.UploadString(url, "POST", postData);

                    // Si no lanzo excepcion, fue exitoso
                    return true;
                }
            }
            catch (WebException ex)
            {
                // Loggear el error para debugging
                if (ex.Response != null)
                {
                    using (var reader = new System.IO.StreamReader(ex.Response.GetResponseStream()))
                    {
                        string errorBody = reader.ReadToEnd();
                        System.Diagnostics.Debug.WriteLine("Error Twilio: " + errorBody);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error WhatsApp: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Convierte un numero ecuatoriano (09XXXXXXXX) al formato internacional (+593XXXXXXXXX)
        /// </summary>
        private static string FormatearNumero(string numero)
        {
            numero = numero.Trim().Replace(" ", "").Replace("-", "");

            // Si empieza con 09, reemplazar con +593 9
            if (numero.StartsWith("09") && numero.Length == 10)
            {
                return "+593" + numero.Substring(1); // quita el 0, queda 9XXXXXXXX
            }

            // Si ya tiene codigo de pais
            if (numero.StartsWith("+"))
            {
                return numero;
            }

            // Si empieza con 593
            if (numero.StartsWith("593"))
            {
                return "+" + numero;
            }

            return "+" + numero;
        }

        /// <summary>
        /// Genera una clave temporal numerica de 6 digitos
        /// </summary>
        public static string GenerarClaveTemporal()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                int valor = Math.Abs(BitConverter.ToInt32(bytes, 0));
                int codigo = (valor % 900000) + 100000; // Siempre 6 digitos (100000-999999)
                return codigo.ToString();
            }
        }
    }
}
