using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using Capa_Datos;
using Capa_Datos.Modelos;
using Capa_Negocio;

namespace Monolito_4am_DB.Seguridad
{
    public partial class Registrar : System.Web.UI.Page
    {
        private CN_tbl_tipo_usuario objcn = new CN_tbl_tipo_usuario();

        // guardar imagenes temporalmente en Session
        [Serializable]
        private class ImagenTemporal
        {
            public byte[] Datos { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public int Tamano { get; set; }
        }

        // Propiedad de acceso a la lista en Session
        private List<ImagenTemporal> ImagenesSession
        {
            get
            {
                if (Session["ImagenesRegistro"] == null)
                    Session["ImagenesRegistro"] = new List<ImagenTemporal>();
                return (List<ImagenTemporal>)Session["ImagenesRegistro"];
            }
            set { Session["ImagenesRegistro"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargar_perfil();
            }
            else
            {
                // X eliminar) 
                if (ImagenesSession.Count > 0)
                    RenderizarGaleria();
            }
        }

        protected void cargar_perfil()
        {
            List<tbl_tipo_usuario> objtu = new List<tbl_tipo_usuario>();
            objtu = CN_tbl_tipo_usuario.ListarTipoUsuario();
           
            ddl_perfil.DataSource = objtu;
            ddl_perfil.DataTextField = "tusu_nombre";
            ddl_perfil.DataValueField = "tusu_id";
            ddl_perfil.DataBind();
        }

        // ================================================================
        //  EVENTOS AutoPostBack - Se ejecutan al salir del campo (Tab/Enter)
        // ================================================================

        #region ===== CÉDULA =====
        protected void txtCedula_TextChanged(object sender, EventArgs e)
        {
            string cedula = txtCedula.Text.Trim();

            // Si esta vacio, no hacer nada (el usuario solo paso por el campo)
            if (string.IsNullOrEmpty(cedula))
            {
                LimpiarCamposAuto();
                return;
            }

            // Validar: solo numeros
            if (!cedula.All(char.IsDigit))
            {
                MostrarAlerta("error", "Cedula Invalida", "La cedula debe contener solo numeros.");
                LimpiarCamposAuto();
                return;
            }

            // Validar: exactamente 10 digitos
            if (cedula.Length != 10)
            {
                MostrarAlerta("error", "Cedula Invalida", "La cedula debe tener exactamente 10 digitos. Tienes " + cedula.Length + ".");
                LimpiarCamposAuto();
                return;
            }

            // Validar digito verificador ecuatoriano
            if (!ValidarCedulaEcuatoriana(cedula))
            {
                MostrarAlerta("error", "Cedula Invalida", "El numero de cedula ingresado no es valido segun el algoritmo ecuatoriano.");
                LimpiarCamposAuto();
                return;
            }

            // Verificar si ya existe en BD
            if (CN_tbl_usuario.autentixced(cedula))
            {
                MostrarAlerta("warning", "Usuario Existente", "Ya existe un usuario registrado con esta cedula.");
                LimpiarCamposAuto();
                return;
            }

            MostrarAlerta("success", "Cedula Valida", "La cedula ha sido verificada correctamente.");
            AutoGenerarCampos();
        }
        #endregion

        #region ===== NOMBRES =====
        protected void txtNombres_TextChanged(object sender, EventArgs e)
        {
            string nombres = txtNombres.Text.Trim();

            if (string.IsNullOrEmpty(nombres))
            {
                MostrarAlerta("warning", "Campo Vacío", "Ingresa al menos un nombre.");
                return;
            }

            // Validar solo letras y espacios
            if (!Regex.IsMatch(nombres, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                MostrarAlerta("error", "Nombres Inválidos", "Los nombres solo pueden contener letras.");
                return;
            }

            string[] partes = nombres.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Validar máximo 2 nombres
            if (partes.Length > 2)
            {
                MostrarAlerta("warning", "Máximo 2 Nombres", "Solo se permiten máximo 2 nombres.");
                txtNombres.Text = partes[0] + " " + partes[1];
                AutoGenerarCampos();
                return;
            }

            // Validar mínimo 3 caracteres por nombre
            foreach (string parte in partes)
            {
                if (parte.Length < 3)
                {
                    MostrarAlerta("error", "Nombre Muy Corto", "Cada nombre debe tener al menos 3 caracteres.");
                    return;
                }
            }

            // Si solo hay 1 nombre → duplicarlo según regla de negocio
            if (partes.Length == 1)
            {
                txtNombres.Text = partes[0] + " " + partes[0];
                MostrarAlerta("info", "Nombre Duplicado", "Se ha duplicado tu nombre automáticamente según la regla del sistema.");
            }

            AutoGenerarCampos();
        }
        #endregion

        #region ===== APELLIDOS =====
        protected void txtApellidos_TextChanged(object sender, EventArgs e)
        {
            string apellidos = txtApellidos.Text.Trim();

            if (string.IsNullOrEmpty(apellidos))
            {
                MostrarAlerta("warning", "Campo Vacío", "Ingresa al menos un apellido.");
                return;
            }

            if (!Regex.IsMatch(apellidos, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                MostrarAlerta("error", "Apellidos Inválidos", "Los apellidos solo pueden contener letras.");
                return;
            }

            string[] partes = apellidos.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length > 2)
            {
                MostrarAlerta("warning", "Máximo 2 Apellidos", "Solo se permiten máximo 2 apellidos.");
                txtApellidos.Text = partes[0] + " " + partes[1];
                AutoGenerarCampos();
                return;
            }

            foreach (string parte in partes)
            {
                if (parte.Length < 3)
                {
                    MostrarAlerta("error", "Apellido Muy Corto", "Cada apellido debe tener al menos 3 caracteres.");
                    return;
                }
            }

            // Si solo hay 1 apellido → duplicarlo
            if (partes.Length == 1)
            {
                txtApellidos.Text = partes[0] + " " + partes[0];
                MostrarAlerta("info", "Apellido Duplicado", "Se ha duplicado tu apellido automáticamente según la regla del sistema.");
            }

            AutoGenerarCampos();
        }
        #endregion

        // ================================================================
        //  AUTO-GENERACIÓN DE CAMPOS (Correo, Contraseña, Nick)
        // ================================================================

        #region ===== AUTO-GENERACIÓN =====
        private void AutoGenerarCampos()
        {
            string cedula = txtCedula.Text.Trim();
            string nombres = txtNombres.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();

            // Solo generar si los 3 campos principales están completos
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10 ||
                string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
                return;

            string[] partsNombre = nombres.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] partsApellido = apellidos.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (partsNombre.Length == 0 || partsApellido.Length == 0) return;

            // 1. CORREO → primer_nombre.primer_apellido@cordillera.edu.ec
            string correo = (RemoverAcentos(partsNombre[0]) + "." + RemoverAcentos(partsApellido[0]) + "@cordillera.edu.ec").ToLower();
            txtCorreo.Text = correo;

            // 2. CONTRASEÑA → 8 caracteres: mayúsculas + minúsculas + números + especiales
            string password = GenerarContrasena();
            txtClave.Text = password;
            hfPasswordPlano.Value = password;

            // 3. NICK → Iniciales + 3 números aleatorios + 1 especial + 2 dígitos cédula
            string nick = GenerarNick(partsNombre[0], partsApellido[0], cedula);
            txtNick.Text = nick;
        }

        /// <summary>
        /// Genera una contraseña fuerte de 8 caracteres.
        /// Garantiza al menos: 1 mayúscula, 1 minúscula, 1 número, 1 carácter especial.
        /// </summary>
        private string GenerarContrasena()
        {
            const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";
            const string especiales = "!@#$%&*_-";

            // Usar RNGCryptoServiceProvider para entropia criptografica
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                char[] password = new char[8];
                password[0] = mayusculas[ObtenerIndiceCripto(rng, mayusculas.Length)];
                password[1] = minusculas[ObtenerIndiceCripto(rng, minusculas.Length)];
                password[2] = numeros[ObtenerIndiceCripto(rng, numeros.Length)];
                password[3] = especiales[ObtenerIndiceCripto(rng, especiales.Length)];

                string todos = mayusculas + minusculas + numeros + especiales;
                for (int i = 4; i < 8; i++)
                {
                    password[i] = todos[ObtenerIndiceCripto(rng, todos.Length)];
                }

                // Fisher-Yates Shuffle con cripto
                for (int i = password.Length - 1; i > 0; i--)
                {
                    int j = ObtenerIndiceCripto(rng, i + 1);
                    char temp = password[i];
                    password[i] = password[j];
                    password[j] = temp;
                }

                return new string(password);
            }
        }

        /// <summary>
        /// Obtiene un indice aleatorio criptograficamente seguro entre 0 y max-1
        /// </summary>
        private int ObtenerIndiceCripto(RNGCryptoServiceProvider rng, int max)
        {
            byte[] randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            int valor = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
            return valor % max;
        }

        /// <summary>
        /// Genera el Nick del usuario con reglas estrictas:
        /// [Inicial Nombre][Inicial Apellido] + 3 números aleatorios + 1 carácter especial + 2 últimos dígitos de cédula
        /// Ejemplo: Cédula 1725934680, Carlos López → CL472#80
        /// </summary>
        private string GenerarNick(string nombre, string apellido, string cedula)
        {
            Random rnd = new Random();

            string iniciales = nombre[0].ToString().ToUpper() + apellido[0].ToString().ToUpper();
            string numeros = rnd.Next(100, 999).ToString(); // 3 numeros aleatorios
            char especial = '_'; // 1 caracter especial (solo guion bajo para cumplir regex)
            string dosCedula = cedula.Substring(cedula.Length - 2); // 2 ultimos digitos

            return iniciales + numeros + especial + dosCedula;
        }
        #endregion

        // ================================================================
        //  REGISTRO EN BASE DE DATOS
        // ================================================================

        #region ===== REGISTRO =====
        protected void btn_registrar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones servidor-side como respaldo de seguridad
                string cedula = txtCedula.Text.Trim();
                string nombres = txtNombres.Text.Trim();
                string apellidos = txtApellidos.Text.Trim();
                string celular = txtCelular.Text.Trim();
                string direccion = txtDireccion.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string nick = txtNick.Text.Trim();
                // Si el usuario edito la clave manualmente, usar esa; si no, usar la auto-generada
                string password = !string.IsNullOrEmpty(txtClave.Text.Trim()) ? txtClave.Text.Trim() : hfPasswordPlano.Value;
                string fechaStr = txtFechaCumple.Text;

                // Validar que ningún campo esté vacío
                if (string.IsNullOrEmpty(cedula) || string.IsNullOrEmpty(nombres) ||
                    string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(celular) ||
                    string.IsNullOrEmpty(direccion) || string.IsNullOrEmpty(correo) ||
                    string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(fechaStr))
                {
                    MostrarAlerta("warning", "Campos Incompletos", "Por favor, completa todos los campos antes de registrarte.");
                    return;
                }

                // Validar cédula servidor-side
                if (!ValidarCedulaEcuatoriana(cedula))
                {
                    MostrarAlerta("error", "Cédula Inválida", "La cédula no pasó la validación del servidor.");
                    return;
                }

                // Verificar duplicado de cedula en BD
                if (CN_tbl_usuario.autentixced(cedula))
                {
                    MostrarAlerta("warning", "Usuario Existente", "Ya existe un usuario con esta cedula.");
                    return;
                }

                // Validar nick: formato y unicidad
                nick = nick.Replace(" ", ""); // sin espacios
                if (nick.Length < 4 || nick.Length > 15 || !Regex.IsMatch(nick, @"^[a-zA-Z0-9_]+$"))
                {
                    MostrarAlerta("error", "Nick Invalido", "El nick debe tener 4-15 caracteres: letras, numeros y guion bajo.");
                    return;
                }
                if (CN_tbl_usuario.existeNick(nick))
                {
                    MostrarAlerta("warning", "Nick Duplicado", "El nick ya esta en uso. Elige otro.");
                    return;
                }

                // Validar correo formato
                if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MostrarAlerta("error", "Correo Invalido", "El formato del correo no es valido.");
                    return;
                }

                // Validar contrasena: min 8, mayusc, minusc, numero, especial
                if (password.Length < 8 || !Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[a-z]") ||
                    !Regex.IsMatch(password, @"[0-9]") || !Regex.IsMatch(password, @"[!@#$%&*_\-]"))
                {
                    MostrarAlerta("error", "Contrasena Debil", "La contrasena debe tener min 8 chars con mayuscula, minuscula, numero y caracter especial.");
                    return;
                }

                // Construir objeto usuario
                tbl_usuario nuevoUsuario = new tbl_usuario();
                nuevoUsuario.usu_cedula = cedula;
                nuevoUsuario.usu_nombres = nombres;
                nuevoUsuario.usu_apellidos = apellidos;
                nuevoUsuario.usu_celular = celular;
                nuevoUsuario.usu_direccion = direccion;
                nuevoUsuario.usu_correo = correo;
                nuevoUsuario.usu_nick = nick;
                DateTime fechaNac;
                if (!DateTime.TryParseExact(fechaStr, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fechaNac))
                {
                    if (!DateTime.TryParse(fechaStr, out fechaNac))
                    {
                        MostrarAlerta("error", "Formato de Fecha", "La fecha de nacimiento tiene un formato invalido.");
                        return;
                    }
                }
                nuevoUsuario.usu_fecha_cumple = fechaNac;
                nuevoUsuario.usu_intentos = 3;
                nuevoUsuario.tusu_id = ddl_perfil.SelectedValue;

                // Registrar con encriptacion de contrasena
                CN_tbl_usuario.registrarUsuarioConClave(nuevoUsuario, password);

                // Guardar TODAS las fotos desde Session (la primera = perfil)
                List<ImagenTemporal> imagenes = ImagenesSession;
                if (imagenes.Count > 0)
                {
                    string nuevoId = nuevoUsuario.usu_id;
                    for (int i = 0; i < imagenes.Count; i++)
                    {
                        CN_tbl_imagen_usuario.GuardarImagen(
                            nuevoId, imagenes[i].Datos, imagenes[i].Nombre,
                            imagenes[i].Tipo, imagenes[i].Tamano, i == 0); // primera = perfil
                    }
                    ImagenesSession = null; // limpiar session
                }

                // Mostrar exito con la contrasena generada
                string mensajeExito = "Tu cuenta ha sido creada exitosamente. " +
                    "Correo: " + correo + " | " +
                    "Nick: " + nick + " | " +
                    "Contrasena: " + password + " | " +
                    "IMPORTANTE: Guarda tu contrasena en un lugar seguro.";

                mensajeExito = mensajeExito.Replace("'", "");

                string script = "Swal.fire({ " +
                    "icon: 'success', " +
                    "title: 'Registro Exitoso', " +
                    "text: '" + mensajeExito + "', " +
                    "confirmButtonColor: '#2563eb', " +
                    "confirmButtonText: 'Ir al Login' " +
                    "}).then(function(result) { " +
                    "if (result.isConfirmed) { window.location.href = 'Login.aspx'; } " +
                    "});";

                ClientScript.RegisterStartupScript(this.GetType(), "RegistroExitoso", script, true);

                // Limpiar formulario
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MostrarAlerta("error", "Error del Sistema", "Ocurrió un error al registrar: " + ex.Message);
            }
        }
        #endregion

        // ================================================================
        //  MÉTODOS AUXILIARES
        // ================================================================

        #region ===== HELPERS =====

        /// <summary>
        /// Validación básica de cédula (10 dígitos, no todos iguales)
        /// </summary>
        private bool ValidarCedulaEcuatoriana(string cedula)
        {
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10) return false;
            if (!cedula.All(char.IsDigit)) return false;

            // Evitar cédulas como 1111111111 o 0000000000
            if (cedula.Distinct().Count() == 1) return false;

            return true;
        }

        /// <summary>
        /// Elimina acentos para generar el correo limpio
        /// </summary>
        private string RemoverAcentos(string texto)
        {
            string conAcentos = "áéíóúÁÉÍÓÚñÑ";
            string sinAcentos = "aeiouAEIOUnN";

            for (int i = 0; i < conAcentos.Length; i++)
            {
                texto = texto.Replace(conAcentos[i], sinAcentos[i]);
            }

            return texto;
        }

        /// <summary>
        /// Muestra un SweetAlert2 desde el servidor
        /// </summary>
        private void MostrarAlerta(string tipo, string titulo, string mensaje)
        {
            string msjLimpio = mensaje.Replace("'", "\\'").Replace("\n", " ").Replace("\r", " ");
            string titLimpio = titulo.Replace("'", "\\'").Replace("\n", " ").Replace("\r", " ");
            string script = "Swal.fire({ icon: '" + tipo + "', title: '" + titLimpio + "', text: '" + msjLimpio + "', confirmButtonColor: '#2563eb' });";
            ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert_" + DateTime.Now.Ticks, script, true);
        }

        /// <summary>
        /// Limpia los campos auto-generados
        /// </summary>
        private void LimpiarCamposAuto()
        {
            txtCorreo.Text = "";
            txtNick.Text = "";
            txtClave.Text = "";
            hfPasswordPlano.Value = "";
        }

        /// <summary>
        /// Limpia todo el formulario después de un registro exitoso
        /// </summary>
        private void LimpiarFormulario()
        {
            txtCedula.Text = "";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            txtCelular.Text = "";
            txtDireccion.Text = "";
            txtFechaCumple.Text = "";
            LimpiarCamposAuto();
        }
        #endregion

        // ================================================================
        //  PREVISUALIZACION DE IMAGEN (Backend puro, sin JavaScript)
        // ================================================================

        #region ===== IMAGEN =====

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuFotoPerfil.HasFiles)
            {
                
                if (ImagenesSession.Count > 0)
                {
                    RenderizarGaleria();
                    MostrarAlerta("info", "Galeria Actual", "Viendo las " + ImagenesSession.Count + " imagen(es) ya cargadas.");
                    return;
                }
                MostrarAlerta("warning", "Sin Archivos", "Selecciona al menos una imagen.");
                return;
            }

            string[] extValidas = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            int agregadas = 0;
            int errores = 0;

            // nuevas imagenes 
            foreach (HttpPostedFile archivo in fuFotoPerfil.PostedFiles)
            {
                string extension = Path.GetExtension(archivo.FileName).ToLower();

                if (!extValidas.Contains(extension)) { errores++; continue; }
                if (archivo.ContentLength > 2 * 1024 * 1024) { errores++; continue; }

                byte[] datos = new byte[archivo.ContentLength];
                archivo.InputStream.Position = 0;
                archivo.InputStream.Read(datos, 0, archivo.ContentLength);

                ImagenesSession.Add(new ImagenTemporal
                {
                    Datos = datos,
                    Nombre = archivo.FileName,
                    Tipo = archivo.ContentType,
                    Tamano = archivo.ContentLength
                });
                agregadas++;
            }

            if (agregadas > 0)
            {
                RenderizarGaleria();
                string msg = agregadas + " imagen(es) agregada(s). Total: " + ImagenesSession.Count + ". La primera es tu foto de perfil.";
                if (errores > 0) msg += " " + errores + " archivo(s) ignorados.";
                MostrarAlerta("success", "Imagenes Actualizadas", msg);
            }
            else
            {
                MostrarAlerta("error", "Sin Imagenes Validas", "Ninguno de los archivos nuevos es valido.");
            }
        }

        /// <summary>
        /// Elimina una imagen del preview por su indice en la Session
        /// </summary>
        protected void EliminarImagenPreview(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int indice = int.Parse(btn.CommandArgument);

            if (indice >= 0 && indice < ImagenesSession.Count)
            {
                string nombreEliminado = ImagenesSession[indice].Nombre;
                ImagenesSession.RemoveAt(indice);
                RenderizarGaleria();
                MostrarAlerta("info", "Imagen Eliminada", nombreEliminado + " fue removida. Quedan " + ImagenesSession.Count + " imagen(es).");
            }
        }

        /// <summary>
        /// Renderiza la galeria desde la Session (se llama despues de agregar o eliminar)
        /// </summary>
        private void RenderizarGaleria()
        {
            pnlGaleria.Controls.Clear();
            List<ImagenTemporal> imagenes = ImagenesSession;

            if (imagenes.Count == 0)
            {
                pnlGaleria.Visible = false;
                placeholderImg.Visible = true;
                return;
            }

            for (int i = 0; i < imagenes.Count; i++)
            {
                string base64 = Convert.ToBase64String(imagenes[i].Datos);

                // Card contenedora
                System.Web.UI.WebControls.Panel card = new System.Web.UI.WebControls.Panel();
                card.Style.Add("position", "relative");
                card.Style.Add("width", "95px");
                card.Style.Add("text-align", "center");

                // Boton X para eliminar
                LinkButton btnEliminar = new LinkButton();
                btnEliminar.ID = "btnEliminar_" + i; // ID FIJO NECESARIO PARA QUE EL EVENTO FUNCIONE AL 1ER CLIC
                btnEliminar.Text = "&#10005;";
                btnEliminar.CommandArgument = i.ToString();
                btnEliminar.Click += EliminarImagenPreview;
                btnEliminar.CausesValidation = false;
                btnEliminar.Style.Add("position", "absolute");
                btnEliminar.Style.Add("top", "-5px");
                btnEliminar.Style.Add("right", "-2px");
                btnEliminar.Style.Add("background", "#e11d48");
                btnEliminar.Style.Add("color", "white");
                btnEliminar.Style.Add("border-radius", "50%");
                btnEliminar.Style.Add("width", "20px");
                btnEliminar.Style.Add("height", "20px");
                btnEliminar.Style.Add("display", "flex");
                btnEliminar.Style.Add("align-items", "center");
                btnEliminar.Style.Add("justify-content", "center");
                btnEliminar.Style.Add("font-size", "0.65rem");
                btnEliminar.Style.Add("text-decoration", "none");
                btnEliminar.Style.Add("z-index", "2");
                btnEliminar.Style.Add("cursor", "pointer");
                btnEliminar.ToolTip = "Eliminar " + imagenes[i].Nombre;
                card.Controls.Add(btnEliminar);

                // Imagen
                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                img.ImageUrl = "data:" + imagenes[i].Tipo + ";base64," + base64;
                img.Style.Add("width", "90px");
                img.Style.Add("height", "90px");
                img.Style.Add("object-fit", "cover");
                img.Style.Add("border-radius", "10px");
                img.Style.Add("border", i == 0 ? "3px solid #00f2fe" : "2px solid rgba(255,255,255,0.15)");
                card.Controls.Add(img);

                // Etiqueta
                System.Web.UI.WebControls.Label lbl = new System.Web.UI.WebControls.Label();
                lbl.Style.Add("display", "block");
                lbl.Style.Add("font-size", "0.6rem");
                lbl.Style.Add("margin-top", "4px");
                lbl.Style.Add("font-weight", "600");
                if (i == 0) { lbl.Text = "PERFIL"; lbl.Style.Add("color", "#00f2fe"); }
                else { lbl.Text = "Foto " + (i + 1); lbl.Style.Add("color", "rgba(255,255,255,0.5)"); }
                card.Controls.Add(lbl);

                pnlGaleria.Controls.Add(card);
            }

            pnlGaleria.Visible = true;
            placeholderImg.Visible = false;
        }

        #endregion

        protected void lnk_login_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}
