using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_categoria
    {
        private static MongoDBContext _context = new MongoDBContext();

        // Traer solo categorias activas
        public static List<tbl_categoria> traercategorias()
        {
            return _context.Categorias.Find(c => c.cat_estado == "A").ToList();
        }

        // Traer todas las categorias
        public static List<tbl_categoria> traertodascategorias()
        {
            return _context.Categorias.Find(_ => true).ToList();
        }

        public static tbl_categoria traercategoriaxidSinEstado(string id)
        {
            return _context.Categorias.Find(c => c.cat_id == id).FirstOrDefault();
        }

        public static bool verificarcategoriaunica(string nombre)
        {
            return _context.Categorias.Find(c => c.cat_nombre == nombre && c.cat_estado == "A").Any();
        }

        public static void save(tbl_categoria categoria)
        {
            try
            {
                categoria.cat_estado = "A";
                _context.Categorias.InsertOne(categoria);
            }
            catch (Exception) { throw; }
        }

        public static void modify(tbl_categoria categoria)
        {
            try
            {
                var filter = Builders<tbl_categoria>.Filter.Eq(c => c.cat_id, categoria.cat_id);
                _context.Categorias.ReplaceOne(filter, categoria);
            }
            catch (Exception) { throw; }
        }

        public static void delete(tbl_categoria categoria)
        {
            try
            {
                if (categoria != null && !string.IsNullOrEmpty(categoria.cat_id))
                {
                    _context.Categorias.DeleteOne(c => c.cat_id == categoria.cat_id);
                }
            }
            catch (Exception) { throw; }
        }

        public static void elimiLog(tbl_categoria categoria)
        {
            try
            {
                categoria.cat_estado = "I";
                modify(categoria); // Reutilizamos modify
            }
            catch (Exception) { throw; }
        }

        public static void activar(tbl_categoria categoria)
        {
            try
            {
                categoria.cat_estado = "A";
                modify(categoria);
            }
            catch (Exception) { throw; }
        }
    }
}
