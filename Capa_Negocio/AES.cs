using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Capa_Negocio
{
    public static class AES
    {
        // La clave debe ser exactamente de 16 (128 bits), 24 o 32 caracteres (256 bits)
        private static readonly string Clave = "MonolitoKey12345";
        // Vector de inicializacion (IV) fijo de 16 bytes (128 bits) para simplicidad y consistencia
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("monolito_4am_iv_");

        /// <summary>
        /// Encripta un texto plano usando AES
        /// </summary>
        public static string Cifrar(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return string.Empty;

            byte[] arrayClave = Encoding.UTF8.GetBytes(Clave);
            byte[] arreglo = Encoding.UTF8.GetBytes(textoPlano);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = arrayClave;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = aes.CreateEncryptor();
                byte[] resultado = cTransform.TransformFinalBlock(arreglo, 0, arreglo.Length);
                
                // Retorna en Base64 para que sea seguro pasarlo por URLs
                // Reemplazamos + y / por caracteres URL-safe
                string base64 = Convert.ToBase64String(resultado);
                return base64.Replace("+", "-").Replace("/", "_");
            }
        }

        /// <summary>
        /// Desencripta un texto cifrado usando AES
        /// </summary>
        public static string Descifrar(string textoCifradoUrlSafe)
        {
            if (string.IsNullOrEmpty(textoCifradoUrlSafe)) return string.Empty;

            try
            {
                byte[] arrayClave = Encoding.UTF8.GetBytes(Clave);
                // Revertimos los caracteres URL-safe al Base64 original
                string base64 = textoCifradoUrlSafe.Replace("-", "+").Replace("_", "/");
                
                // Agregar padding = si es necesario
                int mod4 = base64.Length % 4;
                if (mod4 > 0) base64 += new string('=', 4 - mod4);

                byte[] arreglo = Convert.FromBase64String(base64);

                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.Key = arrayClave;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform cTransform = aes.CreateDecryptor();
                    byte[] resultado = cTransform.TransformFinalBlock(arreglo, 0, arreglo.Length);

                    return Encoding.UTF8.GetString(resultado);
                }
            }
            catch
            {
                // Si la cadena fue manipulada o no es valida, retornamos nulo
                return null;
            }
        }
    }
}
