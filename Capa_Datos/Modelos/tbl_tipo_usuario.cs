using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Capa_Datos.Modelos
{
    public class tbl_tipo_usuario
    {
        [BsonId]
        public string tusu_id { get; set; }
        public string tusu_nombre { get; set; }
        public string tusu_estado { get; set; }
    }
}
