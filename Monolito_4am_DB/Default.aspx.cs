using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Negocio;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Monolito_4am_DB
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Solo cargar datos si el usuario está autenticado
                if (Session["usuario"] != null)
                {
                    try
                    {
                        var usuarios = CN_tbl_usuario.ObtenerTodosLosUsuarios();
                        lblCantidadUsuarios.Text = usuarios.Count.ToString();
                        
                        CargarCatalogoProductos();
                    }
                    catch
                    {
                        lblCantidadUsuarios.Text = "Error";
                    }
                }
            }
        }

        private void CargarCatalogoProductos()
        {
            var productos = CN_tbl_producto.traerproductos();
            var todasLasImagenes = CN_tbl_imagen_producto.traertodaslasimagenes().GroupBy(i => i.pro_id).ToDictionary(g => g.Key, g => g.OrderBy(i => i.imgp_orden).ThenBy(i => i.imgp_id).Select(i => i.imgp_ruta).ToList());
            var categoriasDict = CN_tbl_categoria.traercategorias().ToDictionary(c => c.cat_id, c => c.cat_nombre);

            // Base64 Placeholder para cuando no hay imagen
            string defaultImage = "data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%22300%22%20height%3D%22300%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20viewBox%3D%220%200%20300%20300%22%20preserveAspectRatio%3D%22none%22%3E%3Cdefs%3E%3Cstyle%20type%3D%22text%2Fcss%22%3E%23holder_1%20text%20%7B%20fill%3A%2394a3b8%3Bfont-weight%3Abold%3Bfont-family%3AArial%2C%20Helvetica%2C%20Open%20Sans%2C%20sans-serif%2C%20monospace%3Bfont-size%3A18pt%20%7D%20%3C%2Fstyle%3E%3C%2Fdefs%3E%3Cg%20id%3D%22holder_1%22%3E%3Crect%20width%3D%22300%22%20height%3D%22300%22%20fill%3D%22%23f1f5f9%22%3E%3C%2Frect%3E%3Cg%3E%3Ctext%20x%3D%2295%22%20y%3D%22158%22%3ESin%20Imagen%3C%2Ftext%3E%3C%2Fg%3E%3C%2Fg%3E%3C%2Fsvg%3E";

            var data = productos.Select(p => {
                
                List<string> imagenesValidas = new List<string>();
                if (p.pro_id != null && todasLasImagenes.ContainsKey(p.pro_id) && todasLasImagenes[p.pro_id].Count > 0)
                {
                    foreach(var imgUrl in todasLasImagenes[p.pro_id])
                    {
                        string fullPath = Server.MapPath("~" + imgUrl);
                        if (System.IO.File.Exists(fullPath))
                        {
                            imagenesValidas.Add(imgUrl);
                        }
                    }
                }

                if (imagenesValidas.Count == 0)
                {
                    imagenesValidas.Add(defaultImage);
                }

                return new
                {
                    p.pro_id,
                    p.pro_nombre,
                    pro_precio = p.pro_precio.HasValue ? p.pro_precio.Value.ToString("C2", new System.Globalization.CultureInfo("en-US")) : "$0.00",
                    cat_nombre = p.cat_id != null && categoriasDict.ContainsKey(p.cat_id) ? categoriasDict[p.cat_id] : "General",
                    imagenes = imagenesValidas
                };
            }).ToList();

            rptProductos.DataSource = data;
            rptProductos.DataBind();
        }

        protected void rptProductos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Enlazar las imágenes al repeater anidado
                Repeater rptImagenes = (Repeater)e.Item.FindControl("rptImagenes");
                var imagenes = DataBinder.Eval(e.Item.DataItem, "imagenes") as List<string>;
                
                if (rptImagenes != null && imagenes != null)
                {
                    rptImagenes.DataSource = imagenes;
                    rptImagenes.DataBind();
                }
            }
        }
    }
}
