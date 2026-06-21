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
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidarSesionYRoles();
            }
        }

        private void ValidarSesionYRoles()
        {
            // Validar que exista la sesión
            if (Session["usuario"] == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx");
                return;
            }

            // Obtener el usuario en sesión
            var usuario = Session["usuario"] as Capa_Datos.Modelos.tbl_usuario;

            if (usuario != null)
            {
                // Si tusu_id es 1 (Administrador), mostrar menú admin
                if (usuario.tusu_id == "1")
                {
                    phAdmin.Visible = true;
                    phUsuario.Visible = false;
                }
                // Si tusu_id es 2 (Usuario), mostrar menú usuario
                else if (usuario.tusu_id == "2")
                {
                    phAdmin.Visible = false;
                    phUsuario.Visible = true;
                }

                // Cargar datos del perfil (Foto y Nombre)
                CargarDatosPerfil(usuario);
            }
        }

        private void CargarDatosPerfil(tbl_usuario usuario)
        {
            try
            {
                // Establecer nombre
                lnkNombreUsuario.Text = usuario.usu_nombres + " " + usuario.usu_apellidos;

                // Buscar imagen de perfil
                var imagen = CN_tbl_imagen_usuario.ObtenerImagenPerfil(usuario.usu_id);

                if (imagen != null && imagen.img_datos != null)
                {
                    string base64String = Convert.ToBase64String(imagen.img_datos.ToArray());
                    imgPerfilSidebar.ImageUrl = "data:" + imagen.img_tipo + ";base64," + base64String;
                }
                else
                {
                    // Imagen por defecto con iniciales si no tiene foto
                    string iniciales = usuario.usu_nombres.Substring(0, 1).ToUpper();
                    imgPerfilSidebar.ImageUrl = "https://ui-avatars.com/api/?name=" + iniciales + "&background=random&color=fff";
                }
            }
            catch (Exception)
            {
                // Fallback en caso de error
                imgPerfilSidebar.ImageUrl = "https://ui-avatars.com/api/?name=U&background=random";
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Limpiar la sesión
            Session.Clear();
            Session.Abandon();

            // Eliminar la cookie de recordar contraseña si se desea, o simplemente redirigir
            Response.Redirect("~/Seguridad/Login.aspx");
        }
    }
}
