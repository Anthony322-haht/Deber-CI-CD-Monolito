using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Datos.Modelos;
using Capa_Negocio;

namespace Monolito_4am_DB.Mantenimiento
{
    public partial class nuevo_tbl_categoria : System.Web.UI.Page
    {
        private string idCategoria = "";

        //carga la pagina
        protected void Page_Load(object sender, EventArgs e)

        {
            Page.Title = "Nueva Categor\u00eda";
            if (Request.QueryString["id"] != null)
            {   //texto a n entero
                idCategoria = Request.QueryString["id"];
            }

            if (!IsPostBack)//P .pagi
            {
                if (!string.IsNullOrEmpty(idCategoria))//si es nuevo o es edicion
                {
                    lblTitulo.Text = "Editar Categor\u00eda";
                    CargarDatos();
                }
            }
        }
        //M
        private void CargarDatos()
        {   // sal Capa de Negocio busca BD Cat ID
            var categoria = CN_tbl_categoria.traercategoriaxidSinEstado(idCategoria);
            if (categoria != null)//si dev
            {
                txtNombre.Text = categoria.cat_nombre;
            }
        }
        //Ev
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();// usuario esc

            if (string.IsNullOrEmpty(nombre))//va
            {
                MostrarMensaje("El nombre no puede estar vacío.", "error");
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(idCategoria))
                {
                    // Si el id es 0, significa que la URL vino limpia, asi que es un INSERT.
                    // Validación de duplicados al crear
                    if (CN_tbl_categoria.verificarcategoriaunica(nombre))
                    {
                        MostrarMensaje("Ya existe una categoría activa con este nombre.", "warning");
                        return;
                    }

                    tbl_categoria nueva = new tbl_categoria();
                    nueva.cat_nombre = nombre;
                    CN_tbl_categoria.save(nueva); // aca salto a la Capa de Negocio con F11
                    
                    // inyecta JS al servidor. .then()redirige cuando dan OK.
                    ScriptManager.RegisterStartupScript(this, GetType(), "swal_redirect", 
                        "Swal.fire('¡Guardado!', 'La categoría ha sido creada.', 'success').then((result) => { window.location.href = 'listar_tbl_categoria.aspx'; });", true);
                }
                else
                {
                    // si el id NO es 0, es un UPDATE. Primero traigo el original de la BD.
                    var categoria = CN_tbl_categoria.traercategoriaxidSinEstado(idCategoria);
                    if (categoria != null)
                    {
                        categoria.cat_nombre = nombre;
                        CN_tbl_categoria.modify(categoria); //LINQ ya sabe que cambie el nombre, solo hace SubmitChanges.
                        
                        ScriptManager.RegisterStartupScript(this, GetType(), "swal_redirect", 
                            "Swal.fire('¡Actualizado!', 'La categoría ha sido modificada.', 'success').then((result) => { window.location.href = 'listar_tbl_categoria.aspx'; });", true);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar: " + ex.Message, "error");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string icon = tipo;
            string titulo = tipo == "success" ? "\u00a1\u00c9xito!" : (tipo == "warning" ? "Atenci\u00f3n" : "\u00a1Error!");
            string script = $"Swal.fire('{titulo}', '{mensaje}', '{icon}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "swal_msg", script, true);
        }
    }
}
