using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.Modelos
{
    public class tbl_categoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string cat_id { get; set; }
        public string cat_nombre { get; set; }
        public string cat_estado { get; set; }
    }
}
