using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_usuario
    {
        private static MongoDBContext _context = new MongoDBContext();

        public static List<tbl_usuario> ListarUsuario()
        {
            return _context.Usuarios.Find(u => u.usu_estado == "A").ToList();
        }

        public static bool autentixced(string cedula)
        {
            return _context.Usuarios.Find(u => u.usu_cedula == cedula && u.usu_estado == "A").Any();
        }

        public static bool autentixcc(string cedula, string password)
        {
            var users = _context.Usuarios.Find(u => u.usu_cedula == cedula && u.usu_estado == "A").ToList();
            return users.Any(u => AES.Descifrar(u.usu_contraseña) == password);
        }

        public static tbl_usuario traerUsuario(string cedula, string password)
        {
            var users = _context.Usuarios.Find(u => u.usu_cedula == cedula && u.usu_estado == "A").ToList();
            return users.FirstOrDefault(u => AES.Descifrar(u.usu_contraseña) == password);
        }

        public static tbl_usuario traerced(string cedula)
        {
            return _context.Usuarios.Find(u => u.usu_cedula == cedula && (u.usu_estado == "A" || u.usu_estado == "T")).FirstOrDefault();
        }

        public static void registrarUsuario(tbl_usuario usuario)
        {
            usuario.usu_fecha_creacion = DateTime.Now;
            usuario.usu_estado = "A";
            _context.Usuarios.InsertOne(usuario);
        }

        public static void registrarUsuarioConClave(tbl_usuario usuario, string claveTextoPlano)
        {
            usuario.usu_fecha_creacion = DateTime.Now;
            usuario.usu_estado = "A";
            usuario.usu_intentos = 3;
            usuario.usu_contraseña = AES.Cifrar(claveTextoPlano);
            _context.Usuarios.InsertOne(usuario);
        }

        public static void modificarIntentos(tbl_usuario usuario)
        {
            var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
            _context.Usuarios.ReplaceOne(filter, usuario);
        }

        public static bool existeNick(string nick)
        {
            return _context.Usuarios.Find(u => u.usu_nick == nick && (u.usu_estado == "A" || u.usu_estado == "T")).Any();
        }

        public static string GenerarTokenRecuperacion(string identificador)
        {
            var usuario = _context.Usuarios.Find(u => 
                (u.usu_correo == identificador || u.usu_cedula == identificador) 
                && (u.usu_estado == "A" || u.usu_estado == "T")).FirstOrDefault();

            if (usuario == null) return null;

            string tokenPlano = Guid.NewGuid().ToString("N");
            string tokenCifrado = AES.Cifrar(tokenPlano);

            usuario.usu_token_recuperacion = tokenPlano;
            usuario.usu_fecha_expiracion_token = DateTime.Now.AddMinutes(15);
            
            var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
            _context.Usuarios.ReplaceOne(filter, usuario);

            return tokenCifrado;
        }

        public static bool RestablecerClaveConToken(string tokenCifrado, string nuevaClave)
        {
            string tokenPlano = AES.Descifrar(tokenCifrado);
            if (string.IsNullOrEmpty(tokenPlano)) return false;

            var usuario = _context.Usuarios.Find(u => 
                u.usu_token_recuperacion == tokenPlano 
                && u.usu_fecha_expiracion_token >= DateTime.Now).FirstOrDefault();

            if (usuario == null) return false;

            usuario.usu_contraseña = AES.Cifrar(nuevaClave);
            usuario.usu_token_recuperacion = null;
            usuario.usu_fecha_expiracion_token = null;
            usuario.usu_intentos = 3; 
            
            if (usuario.usu_estado == "T") usuario.usu_estado = "A";

            var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
            _context.Usuarios.ReplaceOne(filter, usuario);
            
            return true;
        }

        public static tbl_usuario ObtenerUsuarioPorIdentificador(string identificador)
        {
            return _context.Usuarios.Find(u =>
                (u.usu_correo == identificador || u.usu_cedula == identificador)
                && (u.usu_estado == "A" || u.usu_estado == "T")).FirstOrDefault();
        }

        public static void GuardarClaveTemporal(tbl_usuario usuario, string claveTemporal)
        {
            usuario.usu_contraseña = AES.Cifrar(claveTemporal);
            usuario.usu_estado = "T";
            usuario.usu_intentos = 3;
            
            var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
            _context.Usuarios.ReplaceOne(filter, usuario);
        }

        public static bool ValidarClaveTemporalWhatsApp(string cedula, string claveIngresada)
        {
            var usuario = _context.Usuarios.Find(u => u.usu_cedula == cedula && u.usu_estado == "T").FirstOrDefault();
            if (usuario == null) return false;

            string claveBD = AES.Descifrar(usuario.usu_contraseña);
            return claveBD == claveIngresada;
        }

        public static Tuple<string, string> GenerarYGuardarOTP(string cedula)
        {
            var usuario = _context.Usuarios.Find(u => u.usu_cedula == cedula).FirstOrDefault();
            if (usuario == null) return null;

            Random rnd = new Random();
            string otpPlano = rnd.Next(100000, 999999).ToString();
            string otpEncriptado = AES.Cifrar(otpPlano);

            usuario.usu_codigo_OTP = otpEncriptado;
            var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
            _context.Usuarios.ReplaceOne(filter, usuario);

            return new Tuple<string, string>(otpPlano, otpEncriptado);
        }

        public static bool ValidarOTP(string cedula, string codigoIngresado, bool esQR)
        {
            var usuario = _context.Usuarios.Find(u => u.usu_cedula == cedula).FirstOrDefault();
            if (usuario == null || string.IsNullOrEmpty(usuario.usu_codigo_OTP)) return false;

            string codigoAComparar = esQR ? codigoIngresado : AES.Cifrar(codigoIngresado);

            if (usuario.usu_codigo_OTP == codigoAComparar)
            {
                usuario.usu_codigo_OTP = null;
                var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
                _context.Usuarios.ReplaceOne(filter, usuario);
                return true;
            }

            return false;
        }

        public static List<tbl_usuario> ObtenerTodosLosUsuarios()
        {
            return _context.Usuarios.Find(_ => true).ToList();
        }

        public static bool DesbloquearUsuario(string usu_id)
        {
            var usuario = _context.Usuarios.Find(u => u.usu_id == usu_id).FirstOrDefault();
            if (usuario != null)
            {
                usuario.usu_intentos = 3;
                if (usuario.usu_estado == "B" || usuario.usu_estado == "I") 
                {
                    usuario.usu_estado = "A";
                }
                var filter = Builders<tbl_usuario>.Filter.Eq(u => u.usu_id, usuario.usu_id);
                _context.Usuarios.ReplaceOne(filter, usuario);
                return true;
            }
            return false;
        }
    }
}
