using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.Modelos
{
    public class tbl_proveedor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string prov_id { get; set; }
        public string prov_nombre { get; set; }
        public string prov_estado { get; set; }
    }
}
