using System;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

public class Program {
    public static void Main() {
        try {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("Monolito_4am_DB");
            var coll = db.GetCollection<BsonDocument>("tbl_usuario");
            
            var docs = coll.Find(new BsonDocument()).ToList();
            Console.WriteLine("Usuarios encontrados: " + docs.Count);
            
            foreach (var doc in docs) {
                var update = Builders<BsonDocument>.Update
                    .Set("usu_estado", "A")
                    .Set("tusu_id", "2"); // Force user profile
                coll.UpdateOne(new BsonDocument("_id", doc["_id"]), update);
                Console.WriteLine("Usuario actualizado a string 'A': " + doc["usu_cedula"]);
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
