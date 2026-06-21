using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Capa_Datos.Modelos
{
    public class tbl_imagen_usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string img_id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string usu_id { get; set; }
        
        public byte[] img_datos { get; set; }
        public string img_nombre { get; set; }
        public string img_tipo { get; set; }
        public int? img_tamaño { get; set; }
        public bool? img_es_perfil { get; set; }
        public DateTime? img_fecha { get; set; }
        public string img_estado { get; set; }
    }
}
