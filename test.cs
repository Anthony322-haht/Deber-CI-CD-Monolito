using System;
using System.Linq;
using Capa_Datos;
using Capa_Negocio;

class Program {
    static void Main() {
        try {
            var prov = CN_tbl_proveedor.traerproveedores().FirstOrDefault(p => p.prov_nombre == "Adidas");
            if (prov != null) {
                Console.WriteLine("Deleting Adidas...");
                CN_tbl_proveedor.delete(prov);
                Console.WriteLine("Success!");
            } else {
                Console.WriteLine("Adidas not found.");
            }
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
            if (ex.InnerException != null) Console.WriteLine("INNER: " + ex.InnerException.Message);
        }
    }
}
