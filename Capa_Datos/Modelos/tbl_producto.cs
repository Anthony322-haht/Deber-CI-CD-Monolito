using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.Modelos
{
    public class tbl_producto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string pro_id { get; set; }
        public string pro_nombre { get; set; }
        public int? pro_cantidad { get; set; }
        public decimal? pro_precio { get; set; }
        public string pro_estado { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string prov_id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string cat_id { get; set; }
    }
}
