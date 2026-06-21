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
    public partial class nuevo_tbl_proveedor : System.Web.UI.Page
    {
        private string idProveedor = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Nuevo Proveedor";
            if (Request.QueryString["id"] != null)
            {
                idProveedor = Request.QueryString["id"];
            }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(idProveedor))
                {
                    lblTitulo.Text = "Editar Proveedor";
                    CargarDatos();
                }
            }
        }

        private void CargarDatos()
        {
            var proveedor = CN_tbl_proveedor.traerproveedorxidSinEstado(idProveedor);
            if (proveedor != null)
            {
                txtNombre.Text = proveedor.prov_nombre;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarMensaje("El nombre no puede estar vacío.", "error");
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(idProveedor))
                {
                    // Validación de duplicados al crear
                    if (CN_tbl_proveedor.verificarproveedorunico(nombre))
                    {
                        MostrarMensaje("Ya existe un proveedor activo con este nombre.", "warning");
                        return;
                    }

                    tbl_proveedor nuevo = new tbl_proveedor();
                    nuevo.prov_nombre = nombre;
                    nuevo.prov_estado = "A";
                    CN_tbl_proveedor.save(nuevo);
                    
                    ScriptManager.RegisterStartupScript(this, GetType(), "swal_redirect", 
                        "Swal.fire('¡Guardado!', 'El proveedor ha sido creado.', 'success').then((result) => { window.location.href = 'listar_tbl_proveedor.aspx'; });", true);
                }
                else
                {
                    var proveedor = CN_tbl_proveedor.traerproveedorxidSinEstado(idProveedor);
                    if (proveedor != null)
                    {
                        // Validación de duplicados al editar (si el nombre cambió)
                        if (!proveedor.prov_nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                        {
                            if (CN_tbl_proveedor.verificarproveedorunico(nombre))
                            {
                                MostrarMensaje("Ya existe otro proveedor activo con este nombre.", "warning");
                                return;
                            }
                        }

                        proveedor.prov_nombre = nombre;
                        CN_tbl_proveedor.modify(proveedor);
                        
                        ScriptManager.RegisterStartupScript(this, GetType(), "swal_redirect", 
                            "Swal.fire('¡Actualizado!', 'El proveedor ha sido modificado.', 'success').then((result) => { window.location.href = 'listar_tbl_proveedor.aspx'; });", true);
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
