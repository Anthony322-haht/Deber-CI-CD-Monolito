using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_producto
    {
        private static MongoDBContext _context = new MongoDBContext();

        public static List<tbl_producto> traerproductos()
        {
            return _context.Productos.Find(p => p.pro_estado == "A").ToList();
        }

        public static List<tbl_producto> traertodosproductos()
        {
            return _context.Productos.Find(_ => true).ToList();
        }

        public static tbl_producto traerproductoxidSinEstado(string id)
        {
            return _context.Productos.Find(p => p.pro_id == id).FirstOrDefault();
        }
        public static tbl_producto traerproductoxid(string id)
        {
            return _context.Productos.Find(p => p.pro_id == id && p.pro_estado == "A").FirstOrDefault();
        }

        public static bool verificarproductounico(string nombre)
        {
            return _context.Productos.Find(p => p.pro_nombre == nombre && p.pro_estado == "A").Any();
        }

        public static List<tbl_producto> buscarproductospornombre(string nombre)
        {
            // Búsqueda simple case-sensitive (para hacerla case-insensitive se podría usar un regex en Mongo)
            // Para mantener compatibilidad básica usamos LINQ en memoria para la lista reducida,
            // o mejor aún usamos Contains en DB. MongoDB Driver traduce Contains a regex básico.
            return _context.Productos.Find(p => p.pro_nombre.Contains(nombre) && p.pro_estado == "A").ToList();
        }

        public static void save(tbl_producto producto)
        {
            try
            {
                producto.pro_estado = "A";
                _context.Productos.InsertOne(producto);
            }
            catch (Exception) { throw; }
        }

        public static void modify(tbl_producto producto)
        {
            try
            {
                var filter = Builders<tbl_producto>.Filter.Eq(p => p.pro_id, producto.pro_id);
                _context.Productos.ReplaceOne(filter, producto);
            }
            catch (Exception) { throw; }
        }

        public static void delete(tbl_producto producto)
        {
            try
            {
                if (producto != null && !string.IsNullOrEmpty(producto.pro_id))
                {
                    _context.Productos.DeleteOne(p => p.pro_id == producto.pro_id);
                }
            }
            catch (Exception) { throw; }
        }

        public static void elimiLog(tbl_producto producto)
        {
            try
            {
                producto.pro_estado = "I";
                modify(producto);
            }
            catch (Exception) { throw; }
        }

        public static void activar(tbl_producto producto)
        {
            try
            {
                producto.pro_estado = "A";
                modify(producto);
            }
            catch (Exception) { throw; }
        }
    }
}
