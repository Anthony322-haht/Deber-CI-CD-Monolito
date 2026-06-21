using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Capa_Negocio
{
    public class CN_tbl_tipo_usuario
    {
        private static MongoDBContext _context = new MongoDBContext();

        public static List<tbl_tipo_usuario> ListarTipoUsuario()
        {
            var roles = _context.TiposUsuario.Find(tu => tu.tusu_estado == "A").ToList();
            if (roles.Count == 0)
            {
                // Auto-seed for fresh local MongoDB instances
                var admin = new tbl_tipo_usuario { tusu_id = "1", tusu_nombre = "Administrador", tusu_estado = "A" };
                var user = new tbl_tipo_usuario { tusu_id = "2", tusu_nombre = "Usuario", tusu_estado = "A" };
                _context.TiposUsuario.InsertMany(new[] { admin, user });
                roles.Add(admin);
                roles.Add(user);
            }
            return roles;
        }
    }
}
