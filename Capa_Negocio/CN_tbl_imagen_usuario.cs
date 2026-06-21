using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_imagen_usuario
    {
        private static MongoDBContext _context = new MongoDBContext();

        public static void GuardarImagen(string usuId, byte[] datos, string nombre, string tipo, int tamano, bool esPerfil)
        {
            if (esPerfil)
            {
                var anteriores = _context.ImagenesUsuario.Find(i => i.usu_id == usuId && i.img_es_perfil == true && i.img_estado == "A").ToList();
                foreach (var img in anteriores)
                {
                    img.img_es_perfil = false;
                    var filter = Builders<tbl_imagen_usuario>.Filter.Eq(i => i.img_id, img.img_id);
                    _context.ImagenesUsuario.ReplaceOne(filter, img);
                }
            }

            tbl_imagen_usuario nueva = new tbl_imagen_usuario();
            nueva.usu_id = usuId;
            nueva.img_datos = datos; // Mongo guarda byte[] directamente
            nueva.img_nombre = nombre;
            nueva.img_tipo = tipo;
            nueva.img_tamaño = tamano;
            nueva.img_es_perfil = esPerfil;
            nueva.img_fecha = DateTime.Now;
            nueva.img_estado = "A";

            _context.ImagenesUsuario.InsertOne(nueva);
        }

        public static tbl_imagen_usuario ObtenerImagenPerfil(string usuId)
        {
            return _context.ImagenesUsuario.Find(i => i.usu_id == usuId && i.img_es_perfil == true && i.img_estado == "A").FirstOrDefault();
        }

        public static List<tbl_imagen_usuario> ListarImagenes(string usuId)
        {
            return _context.ImagenesUsuario.Find(i => i.usu_id == usuId && i.img_estado == "A")
                .SortByDescending(i => i.img_fecha)
                .ToList();
        }

        public static void EliminarImagen(string imgId)
        {
            var img = _context.ImagenesUsuario.Find(i => i.img_id == imgId).FirstOrDefault();
            if (img != null)
            {
                img.img_estado = "I";
                var filter = Builders<tbl_imagen_usuario>.Filter.Eq(i => i.img_id, img.img_id);
                _context.ImagenesUsuario.ReplaceOne(filter, img);
            }
        }
    }
}
