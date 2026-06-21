using MongoDB.Driver;
using System.Configuration;

namespace Capa_Datos
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext()
        {
            var client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]);
            _database = client.GetDatabase(ConfigurationManager.AppSettings["MongoDatabase"]);
        }

        public IMongoCollection<Modelos.tbl_categoria> Categorias => _database.GetCollection<Modelos.tbl_categoria>("tbl_categoria");
        public IMongoCollection<Modelos.tbl_proveedor> Proveedores => _database.GetCollection<Modelos.tbl_proveedor>("tbl_proveedor");
        public IMongoCollection<Modelos.tbl_producto> Productos => _database.GetCollection<Modelos.tbl_producto>("tbl_producto");
        public IMongoCollection<Modelos.tbl_usuario> Usuarios => _database.GetCollection<Modelos.tbl_usuario>("tbl_usuario");
        public IMongoCollection<Modelos.tbl_tipo_usuario> TiposUsuario => _database.GetCollection<Modelos.tbl_tipo_usuario>("tbl_tipo_usuario");
        public IMongoCollection<Modelos.tbl_imagen_producto> ImagenesProducto => _database.GetCollection<Modelos.tbl_imagen_producto>("tbl_imagen_producto");
        public IMongoCollection<Modelos.tbl_imagen_usuario> ImagenesUsuario => _database.GetCollection<Modelos.tbl_imagen_usuario>("tbl_imagen_usuario");
        public IMongoCollection<Modelos.tbl_proveedor_eliminado> ProveedoresEliminados => _database.GetCollection<Modelos.tbl_proveedor_eliminado>("tbl_proveedor_eliminado");
    }
}
