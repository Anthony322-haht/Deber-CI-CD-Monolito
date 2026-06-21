using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_imagen_producto
    {
        private static MongoDBContext _context = new MongoDBContext();

        public static List<tbl_imagen_producto> traerimagenxproducto(string pro_id)
        {
            return _context.ImagenesProducto.Find(img => img.pro_id == pro_id && img.imgp_estado == "A")
                     .SortBy(img => img.imgp_orden)
                     .ToList();
        }

        public static List<tbl_imagen_producto> traertodaslasimagenes()
        {
            return _context.ImagenesProducto.Find(img => img.imgp_estado == "A").ToList();
        }

        public static tbl_imagen_producto traerimagenxid(string id)
        {
            return _context.ImagenesProducto.Find(img => img.imgp_id == id).FirstOrDefault();
        }

        public static void save(tbl_imagen_producto img)
        {
            try
            {
                img.imgp_estado = "A";
                _context.ImagenesProducto.InsertOne(img);
            }
            catch (Exception) { throw; }
        }

        public static void delete(tbl_imagen_producto img)
        {
            try
            {
                if (img != null && !string.IsNullOrEmpty(img.imgp_id))
                {
                    _context.ImagenesProducto.DeleteOne(i => i.imgp_id == img.imgp_id);
                }
            }
            catch (Exception) { throw; }
        }
    }
}
