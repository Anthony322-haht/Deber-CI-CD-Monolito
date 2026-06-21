using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.Modelos
{
    public class tbl_imagen_producto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string imgp_id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string pro_id { get; set; }
        
        public string imgp_ruta { get; set; }
        public string imgp_nombre { get; set; }
        public int? imgp_orden { get; set; }
        public string imgp_estado { get; set; }
    }
}
