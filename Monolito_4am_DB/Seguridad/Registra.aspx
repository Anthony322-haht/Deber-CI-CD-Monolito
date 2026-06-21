<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registra.aspx.cs" Inherits="Monolito_4am_DB.Seguridad.Registrar" %>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
  <title>Crear Cuenta | Premium Registration</title>
  
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
  <link href="https://fonts.googleapis.com/css2?family=Sora:wght@300;400;500;600;700&display=swap" rel="stylesheet"/>
  <link rel="stylesheet" href="Registrar.css" />
  
  <style>
      input[type="date"]::-webkit-calendar-picker-indicator { cursor: pointer; opacity: 0.6; filter: invert(0.8); }
      select.form-input {
          appearance: none; background-image: url("data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%22292.4%22%20height%3D%22292.4%22%3E%3Cpath%20fill%3D%22%2394a3b8%22%20d%3D%22M287%2069.4a17.6%2017.6%200%200%200-13-5.4H18.4c-5%200-9.3%201.8-12.9%205.4A17.6%2017.6%200%200%200%200%2082.2c0%205%201.8%209.3%205.4%2012.9l128%20127.9c3.6%203.6%207.8%205.4%2012.8%205.4s9.2-1.8%2012.8-5.4L287%2095c3.5-3.5%205.4-7.8%205.4-12.8%200-5-1.9-9.2-5.5-12.8z%22%2F%3E%3C%2Fsvg%3E");
          background-repeat: no-repeat; background-position: right 14px top 50%; background-size: 12px auto;
      }
      /* Campos auto-generados con estilo especial */
      .auto-generated { background: rgba(0, 242, 254, 0.08) !important; border: 2px solid rgba(0, 242, 254, 0.25) !important; color: #ffffff !important; }
      .auto-badge { display: inline-block; font-size: 0.6rem; background: rgba(0,242,254,0.2); color: var(--accent-cyan); padding: 2px 8px; border-radius: 20px; margin-left: 8px; vertical-align: middle; letter-spacing: 0.5px; }
      /* SweetAlert Oscuro */
      .swal2-popup { background: #0f1c3f !important; color: white !important; border-radius: 20px !important; border: 1px solid rgba(255, 255, 255, 0.1); }
      .swal2-title { color: white !important; }
      .swal2-html-container { color: rgba(255,255,255,0.8) !important; }
  </style>

  <!-- SweetAlert2 (Must be in head so RegisterStartupScript can use it before </form>) -->
  <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</head>
<body>
    <form id="form1" runat="server">
      <div class="blob blob-1"></div>
      <div class="blob blob-2"></div>

      <!-- HiddenField para almacenar la contraseña en texto plano -->
      <asp:HiddenField ID="hfPasswordPlano" runat="server" />

      <div class="split-card">
          
        <!-- PANEL IZQUIERDO: Branding y Beneficios -->
        <div class="card-left">
            <div class="modern-logo">
                <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path d="M15 7C15 8.65685 13.6569 10 12 10C10.3431 10 9 8.65685 9 7C9 5.34315 10.3431 4 12 4C13.6569 4 15 5.34315 15 7Z" fill="white"/>
                    <path d="M5.5 21C5.5 17.4101 8.41015 14.5 12 14.5C15.5899 14.5 18.5 17.4101 18.5 21" stroke="white" stroke-width="2" stroke-linecap="round"/>
                </svg>
            </div>
            <h2>&Uacute;nete a nosotros</h2>
            <p class="subtitle">Accede a una experiencia completa con seguridad garantizada.</p>
            
            <ul class="benefit-list">
                <li><svg viewBox="0 0 24 24"><path d="M20 6L9 17l-5-5"/></svg> Registro simple y seguro</li>
                <li><svg viewBox="0 0 24 24"><path d="M20 6L9 17l-5-5"/></svg> Protecci&oacute;n de datos avanzada</li>
                <li><svg viewBox="0 0 24 24"><path d="M20 6L9 17l-5-5"/></svg> Acceso inmediato a tu cuenta</li>
                <li><svg viewBox="0 0 24 24"><path d="M20 6L9 17l-5-5"/></svg> Contrase&ntilde;a y Nick autogenerados</li>
            </ul>
        </div>

        <!-- PANEL DERECHO: Formulario Grid -->
        <div class="card-right">
            
            <div class="form-grid">

                <!-- Fila 1: Cédula (AutoPostBack) y Celular -->
                <div class="field">
                    <label>C&eacute;dula <span style="color:#f87171;font-size:0.65rem;">(INGRESA PRIMERO)</span></label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c1.306 0 2.417.835 2.83 2M9 14a3.001 3.001 0 00-2.83 2M15 11h3m-3 4h2"></path></svg>
                        <asp:TextBox ID="txtCedula" runat="server" CssClass="form-input" placeholder="10 d&iacute;gitos" MaxLength="10"
                            AutoPostBack="true" OnTextChanged="txtCedula_TextChanged"></asp:TextBox>
                    </div>
                </div>
                <div class="field">
                    <label>Celular</label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path></svg>
                        <asp:TextBox ID="txtCelular" runat="server" CssClass="form-input" placeholder="09XXXXXXXX" MaxLength="10"></asp:TextBox>
                    </div>
                </div>

                <!-- Fila 2: Nombres y Apellidos (AutoPostBack) -->
                <div class="field">
                    <label>Nombres</label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path></svg>
                        <asp:TextBox ID="txtNombres" runat="server" CssClass="form-input" placeholder="M&aacute;ximo 2 nombres"
                            AutoPostBack="true" OnTextChanged="txtNombres_TextChanged"></asp:TextBox>
                    </div>
                </div>
                <div class="field">
                    <label>Apellidos</label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path></svg>
                        <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-input" placeholder="M&aacute;ximo 2 apellidos"
                            AutoPostBack="true" OnTextChanged="txtApellidos_TextChanged"></asp:TextBox>
                    </div>
                </div>

                <!-- Fila 3: Correo y Nick (AUTO-GENERADOS, editables) -->
                <div class="field">
                    <label>Correo <span class="auto-badge">&#9889; AUTO (editable)</span></label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-input auto-generated" placeholder="Se genera o ingresa manualmente"></asp:TextBox>
                    </div>
                </div>
                <div class="field">
                    <label>Nick / Usuario <span class="auto-badge">&#9889; AUTO (editable)</span></label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.121 17.804A13.937 13.937 0 0112 16c2.5 0 4.847.655 6.879 1.804M15 10a3 3 0 11-6 0 3 3 0 016 0zm6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                        <asp:TextBox ID="txtNick" runat="server" CssClass="form-input auto-generated" placeholder="4-15 chars: letras, n&uacute;meros, _" MaxLength="15"></asp:TextBox>
                    </div>
                </div>

                <!-- Fila 4: Contrase&ntilde;a generada (full-width, editable) -->
                <div class="field full-width">
                    <label>Contrase&ntilde;a <span class="auto-badge">&#9889; AUTO (editable)</span></label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
                        <asp:TextBox ID="txtClave" runat="server" CssClass="form-input auto-generated" placeholder="M&iacute;n 8 chars: may&uacute;sc, min&uacute;sc, n&uacute;m, especial" style="font-family: monospace; font-size: 1.1rem; letter-spacing: 2px; font-weight: 700;"></asp:TextBox>
                    </div>
                </div>

                <!-- Fila 5: Fecha de Nacimiento y Perfil -->
                <div class="field">
                    <label>Fecha de Nacimiento</label>
                    <div class="input-wrap">
                        <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                        <asp:TextBox ID="txtFechaCumple" TextMode="Date" runat="server" CssClass="form-input"></asp:TextBox>
                    </div>
                </div>
                <div class="field">
                  <label>Perfil</label>
                  <div class="input-wrap">
                    <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path></svg>
                    <asp:DropDownList ID="ddl_perfil" runat="server" CssClass="form-input"></asp:DropDownList>
                  </div>
                </div>

                <!-- Fila 6: Dirección (full-width) -->
                <div class="field full-width">
                  <label>Direcci&oacute;n</label>
                  <div class="input-wrap">
                    <svg class="input-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
                    <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-input" placeholder="Ingresa tu direcci&oacute;n"></asp:TextBox>
                  </div>
                </div>

            </div> <!-- Fin de form-grid -->

            <!-- Fila 7: Fotos del usuario (Backend puro, multiples imagenes) -->
            <div style="margin-bottom: 20px;">
                <label>Fotos del Usuario <span style="font-size:0.65rem; color:rgba(255,255,255,0.5);">(JPG, PNG, GIF, BMP, WEBP &mdash; m&aacute;x 2MB c/u)</span></label>
                <p style="font-size: 0.7rem; color: #00f2fe; margin: 4px 0 10px 0; font-weight: 500;">&#9733; La primera imagen seleccionada ser&aacute; tu FOTO DE PERFIL</p>
                <div style="display: flex; gap: 15px; align-items: flex-start;">
                    <div style="flex: 1;">
                        <asp:FileUpload ID="fuFotoPerfil" runat="server" CssClass="form-input" AllowMultiple="true"
                            accept="image/jpeg,image/png,image/gif,image/bmp,image/webp"
                            style="padding: 10px; background: rgba(255,255,255,0.08); border: 2px dashed rgba(0,242,254,0.3); border-radius: 10px; color: white; cursor: pointer;" 
                            onchange="return validarArchivos(this);" />
                        <asp:Button ID="btnPrevisualizar" runat="server" Text="&#128065; Previsualizar Todo" CssClass="btn-preview"
                            OnClick="btnPrevisualizar_Click" CausesValidation="false"
                            style="margin-top: 10px; padding: 8px 20px; background: linear-gradient(90deg, #6a11cb, #2575fc); color: white; border: none; border-radius: 8px; cursor: pointer; font-family: 'Sora', sans-serif; font-size: 0.8rem; font-weight: 600;" />
                    </div>
                </div>
                <!-- Galeria de preview (se llena por backend via PostBack) -->
                <asp:Panel ID="pnlGaleria" runat="server" Visible="false"
                    style="display: flex; flex-wrap: wrap; gap: 10px; margin-top: 12px; padding: 12px; background: rgba(0,0,0,0.2); border-radius: 12px; border: 1px solid rgba(0,242,254,0.15);">
                </asp:Panel>
                <span id="placeholderImg" runat="server" style="color: rgba(255,255,255,0.25); font-size: 0.7rem; display: block; margin-top: 8px;">Selecciona archivos y presiona Previsualizar para verlos aqu&iacute;</span>
            </div>

            <!-- Acepto los términos -->
            <label class="terms-wrapper">
                <asp:CheckBox ID="chkTerminos" runat="server" />
                <span>Acepto los t&eacute;rminos y condiciones</span>
            </label>

            <!-- Barra de progreso -->
            <div id="progressContainer">
                <div class="progress">
                  <div id="progressBarInner" class="progress-bar-custom"></div>
                </div>
                <div class="progress-text">Creando perfil de usuario...</div>
            </div>

            <!-- Botón de Registro -->
            <asp:Button ID="btnRegistrar" runat="server" Text="Registrarse" CssClass="btn-register" 
                OnClick="btn_registrar_Click" OnClientClick="return validarRegistro();" />

            <p class="login-link">
              &iquest;Ya tienes una cuenta? <a href="Login.aspx">Inicia Sesi&oacute;n aqu&iacute;</a>
            </p>

        </div> <!-- Fin de card-right -->
      </div>
    </form>

    <script type="text/javascript">
        // @ts-nocheck
        let formSubmitted = false;

        function validarRegistro() {
            if (formSubmitted) {
                Swal.fire({ icon: 'info', title: 'Procesando...', text: 'Estamos creando tu cuenta. Por favor, espera.', confirmButtonColor: '#2563eb' });
                return false;
            }

            var cedula   = document.getElementById('<%= txtCedula.ClientID %>').value.trim();
            var nombres  = document.getElementById('<%= txtNombres.ClientID %>').value.trim();
            var apellidos= document.getElementById('<%= txtApellidos.ClientID %>').value.trim();
            var celular  = document.getElementById('<%= txtCelular.ClientID %>').value.trim();
            var fecha    = document.getElementById('<%= txtFechaCumple.ClientID %>').value;
            var direccion= document.getElementById('<%= txtDireccion.ClientID %>').value.trim();
            var correo   = document.getElementById('<%= txtCorreo.ClientID %>').value.trim();
            var nick     = document.getElementById('<%= txtNick.ClientID %>').value.trim();
            var clave    = document.getElementById('<%= txtClave.ClientID %>').value.trim();
            var terminos = document.getElementById('<%= chkTerminos.ClientID %>');

            // === 1. CÉDULA: solo números, exactamente 10 ===
            if (!/^\d{10}$/.test(cedula)) {
                Swal.fire({ icon: 'error', title: 'C\u00e9dula Inv\u00e1lida', text: 'La c\u00e9dula debe tener exactamente 10 d\u00edgitos num\u00e9ricos.', confirmButtonColor: '#e11d48' });
                return false;
            }

            // === 2. NOMBRES: solo letras, máx 2 palabras, mín 3 chars por palabra ===
            var partesNom = nombres.split(/\s+/).filter(function(p){ return p.length > 0; });
            if (partesNom.length === 0 || partesNom.length > 2) {
                Swal.fire({ icon: 'error', title: 'Nombres Inv\u00e1lidos', text: 'Ingresa 1 o 2 nombres (solo letras).', confirmButtonColor: '#e11d48' });
                return false;
            }
            for (var i = 0; i < partesNom.length; i++) {
                if (!/^[a-zA-Z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00f1\u00d1]+$/.test(partesNom[i])) {
                    Swal.fire({ icon: 'error', title: 'Nombres Inv\u00e1lidos', text: 'Los nombres solo pueden contener letras (sin n\u00fameros ni s\u00edmbolos).', confirmButtonColor: '#e11d48' });
                    return false;
                }
                if (partesNom[i].length < 3) {
                    Swal.fire({ icon: 'error', title: 'Nombre Muy Corto', text: 'Cada nombre debe tener al menos 3 caracteres.', confirmButtonColor: '#e11d48' });
                    return false;
                }
            }

            // === 3. APELLIDOS: solo letras, máx 2 palabras, mín 3 chars ===
            var partesApe = apellidos.split(/\s+/).filter(function(p){ return p.length > 0; });
            if (partesApe.length === 0 || partesApe.length > 2) {
                Swal.fire({ icon: 'error', title: 'Apellidos Inv\u00e1lidos', text: 'Ingresa 1 o 2 apellidos (solo letras).', confirmButtonColor: '#e11d48' });
                return false;
            }
            for (var j = 0; j < partesApe.length; j++) {
                if (!/^[a-zA-Z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00f1\u00d1]+$/.test(partesApe[j])) {
                    Swal.fire({ icon: 'error', title: 'Apellidos Inv\u00e1lidos', text: 'Los apellidos solo pueden contener letras.', confirmButtonColor: '#e11d48' });
                    return false;
                }
                if (partesApe[j].length < 3) {
                    Swal.fire({ icon: 'error', title: 'Apellido Muy Corto', text: 'Cada apellido debe tener al menos 3 caracteres.', confirmButtonColor: '#e11d48' });
                    return false;
                }
            }

            // === 4. CELULAR: solo números, 10 dígitos, empieza con 09 ===
            if (!/^09\d{8}$/.test(celular)) {
                Swal.fire({ icon: 'error', title: 'Celular Inv\u00e1lido', text: 'El celular debe tener 10 d\u00edgitos y comenzar con 09.', confirmButtonColor: '#e11d48' });
                return false;
            }

            // === 5. FECHA: no vacía, no futura, edad mínima 18, máxima 100 ===
            if (!fecha) {
                Swal.fire({ icon: 'warning', title: 'Fecha Requerida', text: 'Selecciona tu fecha de nacimiento.', confirmButtonColor: '#2563eb' });
                return false;
            }
            var fechaNac = new Date(fecha + 'T00:00:00');
            var hoy = new Date();
            if (fechaNac > hoy) {
                Swal.fire({ icon: 'error', title: 'Fecha Inv\u00e1lida', text: 'La fecha de nacimiento no puede ser futura.', confirmButtonColor: '#e11d48' });
                return false;
            }
            var edad = hoy.getFullYear() - fechaNac.getFullYear();
            var m = hoy.getMonth() - fechaNac.getMonth();
            if (m < 0 || (m === 0 && hoy.getDate() < fechaNac.getDate())) edad--;
            if (edad < 18) {
                Swal.fire({ icon: 'error', title: 'Edad Insuficiente', text: 'Debes tener al menos 18 a\u00f1os para registrarte.', confirmButtonColor: '#e11d48' });
                return false;
            }
            if (edad > 100) {
                Swal.fire({ icon: 'error', title: 'Fecha Inv\u00e1lida', text: 'La edad no puede superar los 100 a\u00f1os. Verifica tu fecha de nacimiento.', confirmButtonColor: '#e11d48' });
                return false;
            }

            // === 6. DIRECCIÓN: mínimo 5 caracteres ===
            if (direccion.length < 5) {
                Swal.fire({ icon: 'warning', title: 'Direcci\u00f3n Requerida', text: 'La direcci\u00f3n debe tener al menos 5 caracteres.', confirmButtonColor: '#2563eb' });
                return false;
            }

            // === 7. CORREO: formato RFC 5322 ===
            if (!correo) {
                Swal.fire({ icon: 'warning', title: 'Correo Requerido', text: 'El correo electr\u00f3nico es obligatorio.', confirmButtonColor: '#2563eb' });
                return false;
            }
            var regexCorreo = /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;
            if (!regexCorreo.test(correo)) {
                Swal.fire({ icon: 'error', title: 'Correo Inv\u00e1lido', text: 'El formato del correo electr\u00f3nico no es v\u00e1lido.', confirmButtonColor: '#e11d48' });
                return false;
            }

            // === 8. NICK: 4-15 chars, solo letras/números/_, sin espacios ===
            if (!nick) {
                Swal.fire({ icon: 'warning', title: 'Nick Requerido', text: 'El nick / usuario es obligatorio.', confirmButtonColor: '#2563eb' });
                return false;
            }
            var nickLimpio = nick.replace(/\s/g, '');
            if (nickLimpio !== nick) {
                document.getElementById('<%= txtNick.ClientID %>').value = nickLimpio;
                nick = nickLimpio;
            }
            if (nick.length < 4 || nick.length > 15) {
                Swal.fire({ icon: 'error', title: 'Nick Inv\u00e1lido', text: 'El nick debe tener entre 4 y 15 caracteres.', confirmButtonColor: '#e11d48' });
                return false;
            }
            if (!/^[a-zA-Z0-9_]+$/.test(nick)) {
                Swal.fire({ icon: 'error', title: 'Nick Inv\u00e1lido', text: 'El nick solo puede contener letras, n\u00fameros y guiones bajos (_).', confirmButtonColor: '#e11d48' });
                return false;
            }

            // === 9. CONTRASEÑA: mín 8 chars, 1 mayúsc, 1 minúsc, 1 número, 1 especial ===
            if (!clave) {
                Swal.fire({ icon: 'warning', title: 'Contrase\u00f1a Requerida', text: 'La contrase\u00f1a es obligatoria.', confirmButtonColor: '#2563eb' });
                return false;
            }
            if (clave.length < 8) {
                Swal.fire({ icon: 'error', title: 'Contrase\u00f1a D\u00e9bil', text: 'La contrase\u00f1a debe tener al menos 8 caracteres.', confirmButtonColor: '#e11d48' });
                return false;
            }
            if (!/[A-Z]/.test(clave)) {
                Swal.fire({ icon: 'error', title: 'Contrase\u00f1a D\u00e9bil', text: 'Debe incluir al menos una letra may\u00fascula.', confirmButtonColor: '#e11d48' });
                return false;
            }
            if (!/[a-z]/.test(clave)) {
                Swal.fire({ icon: 'error', title: 'Contrase\u00f1a D\u00e9bil', text: 'Debe incluir al menos una letra min\u00fascula.', confirmButtonColor: '#e11d48' });
                return false;
            }
            if (!/[0-9]/.test(clave)) {
                Swal.fire({ icon: 'error', title: 'Contrase\u00f1a D\u00e9bil', text: 'Debe incluir al menos un n\u00famero.', confirmButtonColor: '#e11d48' });
                return false;
            }
            if (!/[!@#$%&*_\-]/.test(clave)) {
                Swal.fire({ icon: 'error', title: 'Contrase\u00f1a D\u00e9bil', text: 'Debe incluir al menos un car\u00e1cter especial (!@#$%&*_-).', confirmButtonColor: '#e11d48' });
                return false;
            }

            // === 10. TÉRMINOS ===
            if (!terminos.checked) {
                Swal.fire({ icon: 'warning', title: 'T\u00e9rminos y Condiciones', text: 'Debes aceptar los t\u00e9rminos y condiciones para registrarte.', confirmButtonColor: '#2563eb' });
                return false;
            }

            // --- ANIMACIÓN ---
            formSubmitted = true;
            var btn = document.getElementById('<%= btnRegistrar.ClientID %>');
            if (btn) { btn.value = 'Procesando...'; btn.style.opacity = '0.7'; btn.style.cursor = 'wait'; }
            var pc = document.getElementById('progressContainer');
            var innerBar = document.getElementById('progressBarInner');
            if (pc && innerBar) {
                pc.style.display = 'block';
                setTimeout(function() { pc.style.opacity = '1'; innerBar.style.width = '100%'; }, 50);
            }
            return true;
        }

        // Validacion JS para multiples archivos de imagen
        function validarArchivos(input) {
            if (!input.files || input.files.length === 0) return true;

            var extensionesValidas = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp'];
            var errores = [];

            for (var i = 0; i < input.files.length; i++) {
                var file = input.files[i];
                var nombre = file.name.toLowerCase();
                var extension = nombre.substring(nombre.lastIndexOf('.'));

                if (extensionesValidas.indexOf(extension) === -1) {
                    errores.push(file.name + ' - formato no valido');
                    continue;
                }

                if (file.size > 2 * 1024 * 1024) {
                    errores.push(file.name + ' - supera 2MB');
                }
            }

            if (errores.length > 0) {
                Swal.fire({ icon: 'error', title: 'Archivos con Problemas', html: errores.join('<br>'), confirmButtonColor: '#e11d48' });
                input.value = '';
                return false;
            }

            Swal.fire({ icon: 'info', title: input.files.length + ' imagen(es) seleccionada(s)', text: 'La primera imagen sera tu foto de perfil. Presiona Previsualizar para verlas.', confirmButtonColor: '#2563eb', timer: 3000, showConfirmButton: false });
            return true;
        }
    </script>
</body>
</html>