using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_proveedor
    {
        private static MongoDBContext _context = new MongoDBContext();

        public static List<tbl_proveedor> traerproveedores()
        {
            return _context.Proveedores.Find(p => p.prov_estado == "A").ToList();
        }

        public static List<tbl_proveedor> traertodosproveedores()
        {
            return _context.Proveedores.Find(_ => true).ToList();
        }

        public static tbl_proveedor traerproveedorxidSinEstado(string id)
        {
            return _context.Proveedores.Find(p => p.prov_id == id).FirstOrDefault();
        }

        public static bool verificarproveedorunico(string nombre)
        {
            return _context.Proveedores.Find(p => p.prov_nombre == nombre.Trim() && p.prov_estado == "A").Any();
        }

        public static void save(tbl_proveedor proveedor)
        {
            try
            {
                proveedor.prov_estado = "A";
                string nombreLimpio = proveedor.prov_nombre.Trim();

                // Verificar si existe en el historial de eliminados
                var eliminado = _context.ProveedoresEliminados.Find(p => p.prov_nombre == nombreLimpio).FirstOrDefault();

                if (eliminado != null)
                {
                    // En MongoDB no necesitamos IDENTITY_INSERT, simplemente asignamos el mismo ID que tenía
                    proveedor.prov_id = eliminado.prov_id;
                    _context.Proveedores.InsertOne(proveedor);
                    
                    // Borrar de la papelera
                    _context.ProveedoresEliminados.DeleteOne(p => p.prov_id == eliminado.prov_id);
                }
                else
                {
                    // Insertar normalmente (Mongo auto-genera el _id tipo ObjectId)
                    _context.Proveedores.InsertOne(proveedor);
                }
            }
            catch (Exception) { throw; }
        }

        public static void modify(tbl_proveedor proveedor)
        {
            try
            {
                var filter = Builders<tbl_proveedor>.Filter.Eq(p => p.prov_id, proveedor.prov_id);
                _context.Proveedores.ReplaceOne(filter, proveedor);
            }
            catch (Exception) { throw; }
        }

        public static void delete(tbl_proveedor proveedor)
        {
            try
            {
                if (proveedor != null && !string.IsNullOrEmpty(proveedor.prov_id))
                {
                    // === SIMULACIÓN DE TRIGGER DE SQL ===
                    // Guardamos una copia en ProveedoresEliminados antes de borrarlo definitivamente
                    var backup = new tbl_proveedor_eliminado
                    {
                        prov_id = proveedor.prov_id,
                        prov_nombre = proveedor.prov_nombre
                    };
                    
                    // Insertamos si no existía ya en la papelera
                    var existe = _context.ProveedoresEliminados.Find(p => p.prov_id == backup.prov_id).Any();
                    if (!existe)
                    {
                        _context.ProveedoresEliminados.InsertOne(backup);
                    }

                    // Ahora sí, eliminamos
                    _context.Proveedores.DeleteOne(p => p.prov_id == proveedor.prov_id);
                }
            }
            catch (Exception) { throw; }
        }

        public static void elimiLog(tbl_proveedor proveedor)
        {
            try
            {
                if (proveedor != null)
                {
                    proveedor.prov_estado = "I";
                    modify(proveedor);
                }
            }
            catch (Exception) { throw; }
        }

        public static void activar(tbl_proveedor proveedor)
        {
            try
            {
                if (proveedor != null)
                {
                    proveedor.prov_estado = "A";
                    modify(proveedor);
                }
            }
            catch (Exception) { throw; }
        }
    }
}
