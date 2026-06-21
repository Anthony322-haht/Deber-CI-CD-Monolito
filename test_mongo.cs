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
            Console.WriteLine("Total users: " + docs.Count);
            foreach (var doc in docs) {
                Console.WriteLine(doc.ToJson());
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
