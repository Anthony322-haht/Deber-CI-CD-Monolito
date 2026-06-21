using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Capa_Datos.Modelos
{
    public class tbl_usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string usu_id { get; set; }
        public string usu_cedula { get; set; }
        public string usu_nombres { get; set; }
        public string usu_apellidos { get; set; }
        public string usu_direccion { get; set; }
        public string usu_celular { get; set; }
        public string usu_correo { get; set; }
        public string usu_contraseña { get; set; }
        public DateTime? usu_fecha_creacion { get; set; }
        public DateTime? usu_fecha_cumple { get; set; }
        public string usu_nick { get; set; }
        public int? usu_intentos { get; set; }
        public string usu_codigo_OTP { get; set; }
        public string usu_estado { get; set; }
        
        public string tusu_id { get; set; }
        
        public DateTime? usu_fecha_ultimo_intento { get; set; }
        public string usu_token_recuperacion { get; set; }
        public DateTime? usu_fecha_expiracion_token { get; set; }
    }
}
