<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Monolito_4am_DB.Seguridad.Login" %>

<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
  <title>Iniciar Sesión | Premium Login</title>
  
  <!-- CSS Externos: Bootstrap y Fuentes -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
  <link href="https://fonts.googleapis.com/css2?family=Sora:wght@300;400;500;600;700&display=swap" rel="stylesheet"/>
  
  <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
  <script src="https://unpkg.com/html5-qrcode" type="text/javascript"></script>

  <link rel="stylesheet" href="styles.css" />
  
  <style>
    /* ----- ESTILOS DE LA NUEVA BARRA DE CARGA ----- */
    #progressContainer { 
        display: none; 
        margin-bottom: 25px; 
        opacity: 0; 
        transition: opacity 0.3s ease; 
    }
    
    .progress { 
        height: 8px; 
        background-color: rgba(255, 255, 255, 0.05); 
        border-radius: 20px; 
        overflow: hidden;
        border: 1px solid rgba(255,255,255,0.1);
        box-shadow: inset 0 1px 3px rgba(0,0,0,0.3);
    }
    
    .progress-bar-custom { 
        height: 100%;
        width: 0%; 
        background: linear-gradient(90deg, var(--blue-glow), var(--accent-cyan), var(--blue-glow)); 
        background-size: 200% 200%;
        border-radius: 20px;
        box-shadow: 0 0 10px rgba(0, 242, 254, 0.5);
        transition: width 2s ease-in-out; 
        animation: gradientMove 2s ease infinite;
    }
    
    @keyframes gradientMove {
        0% { background-position: 0% 50%; }
        50% { background-position: 100% 50%; }
        100% { background-position: 0% 50%; }
    }

    .progress-text { 
        text-align: center; 
        font-size: 0.8rem; 
        color: var(--accent-cyan); 
        margin-top: 10px; 
        font-weight: 600; 
        letter-spacing: 0.5px;
        animation: pulseText 1.5s infinite;
    }
    
    @keyframes pulseText {
        0%, 100% { opacity: 0.5; }
        50% { opacity: 1; }
    }

    /* ----- ESTILOS DE SWEETALERT ----- */
    .swal2-popup {
        background: #0f1c3f !important;
        color: white !important;
        border-radius: 20px !important;
        border: 1px solid rgba(255, 255, 255, 0.1);
    }
    .swal2-title { color: white !important; }
    .swal2-html-container { color: rgba(255,255,255,0.8) !important; }
  </style>
</head>
<body>
    <form id="form1" runat="server">
      <div class="bg-panel"></div>
      <div class="blob blob-1"></div>
      <div class="blob blob-2"></div>

      <div class="card">
        <div class="card-header">
            <div class="main-logo-container">
                <div class="modern-logo">
                    <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <path d="M12 4L3 18H21L12 4Z" fill="none" stroke="white" stroke-width="2" stroke-linejoin="round"/>
                        <path d="M9 14L12 9L15 14" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>
                </div>
            </div>
            <h1>Bienvenido</h1>
            <p class="subtitle">Ingresa tus credenciales para continuar</p>
        </div>

        <asp:Panel ID="pnlLogin" runat="server">
        <div class="field">
          <label for="txt_ced">IDENTIFICACI&Oacute;N</label>
          <div class="input-wrap">
            <asp:TextBox ID="txt_ced" runat="server" CssClass="form-input" placeholder="Ingresa tu usuario"></asp:TextBox>
          </div>
        </div>

        <div class="field">
          <label for="txt_pass">CONTRASE&Ntilde;A</label>
          <div class="input-wrap">
            <asp:TextBox ID="txt_pass" TextMode="Password" runat="server" CssClass="form-input" placeholder="Ingresa tu contrase&ntilde;a"></asp:TextBox>
            <button class="eye-btn" onclick="togglePassword()" aria-label="Mostrar contraseña" type="button">
              <svg id="eyeIcon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" width="20" height="20">
                <path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z"/>
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"/>
              </svg>
            </button>
          </div>
          <asp:LinkButton ID="lnk_olvido" runat="server" CssClass="forgot" OnClick="lnk_olvido_Click">&iquest;Olvidaste tu contrase&ntilde;a?</asp:LinkButton>
        </div>

        <div class="form-options">
            <label class="checkbox-wrapper">
                <asp:CheckBox ID="CheckBox1" runat="server" CssClass="checkbox-input" />
                <span class="checkbox-label">RECORDARME</span>
            </label>
        </div>

        <div id="progressContainer">
            <div class="progress">
              <div id="progressBarInner" class="progress-bar-custom"></div>
            </div>
            <div class="progress-text">Autenticando credenciales...</div>
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Acceder" CssClass="btn-signin" 
            OnClick="btnLogin_Click" OnClientClick="return validarFormulario();" />

        <div class="divider">o accede con</div>

        <div class="social-row">
          <button class="btn-social" type="button" title="Google">
            <svg viewBox="0 0 24 24" width="24" height="24">
                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l3.66-2.84z" fill="#FBBC05"/>
                <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
            </svg>
          </button>
          <button class="btn-social" type="button" title="GitHub">
            <svg viewBox="0 0 24 24" width="24" height="24" fill="white">
                <path d="M12 2C6.48 2 2 6.48 2 12c0 4.42 2.87 8.17 6.84 9.49.5.09.68-.22.68-.48 0-.24-.01-.87-.01-1.71-2.78.6-3.37-1.34-3.37-1.34-.45-1.16-1.11-1.47-1.11-1.47-.91-.62.07-.61.07-.61 1 .07 1.53 1.03 1.53 1.03.89 1.52 2.34 1.08 2.91.83.09-.65.35-1.08.63-1.33-2.22-.25-4.56-1.11-4.56-4.95 0-1.09.39-1.99 1.03-2.69-.1-.25-.45-1.27.1-2.65 0 0 .84-.27 2.75 1.02A9.56 9.56 0 0 1 12 6.8c.85.004 1.71.115 2.51.337 1.91-1.29 2.75-1.02 2.75-1.02.55 1.38.2 2.4.1 2.65.64.7 1.03 1.6 1.03 2.69 0 3.85-2.34 4.7-4.57 4.95.36.31.68.92.68 1.85 0 1.34-.01 2.41-.01 2.74 0 .27.18.58.69.48A10.001 10.001 0 0 0 22 12c0-5.52-4.48-10-10-10z"/>
            </svg>
          </button>
        </div>
        </asp:Panel>

        <!-- PANEL DE DOBLE FACTOR 2FA -->
        <asp:Panel ID="pnl2FA" runat="server" Visible="false">
            <h3 style="color: var(--accent-cyan); text-align: center; margin-bottom: 20px;">Seguridad 2FA</h3>
            <p style="text-align: center; font-size: 0.9rem; color: #a1a1aa; margin-bottom: 20px;">
                Ingresa el c&oacute;digo enviado a tu correo o escanea el QR.
            </p>

            <div class="field">
                <label>C&Oacute;DIGO DE 6 D&Iacute;GITOS</label>
                <div class="input-wrap">
                    <asp:TextBox ID="txtCodigo2FA" runat="server" CssClass="form-input" placeholder="Ej. 123456" MaxLength="6"></asp:TextBox>
                </div>
            </div>

            <div style="text-align: center; margin-bottom: 20px;">
                <button type="button" class="btn-recover" id="btnStartScan" style="background: #3b82f6; width: 100%; margin-bottom: 10px;">
                    &#128247; Escanear QR con C&aacute;mara
                </button>
                <button type="button" class="btn-recover" id="btnStopScan" style="background: #ef4444; width: 100%; display: none;">
                    &#128683; Detener C&aacute;mara
                </button>
                <div id="qr-reader" style="width:100%; max-width: 400px; margin: 10px auto; border-radius: 8px; overflow: hidden; display: none;"></div>
            </div>

            <asp:HiddenField ID="hfQRResult" runat="server" />
            <asp:HiddenField ID="hfCedula2FA" runat="server" />

            <asp:Button ID="btnValidar2FA" runat="server" Text="Validar C&oacute;digo" CssClass="btn-signin" 
                OnClick="btnValidar2FA_Click" OnClientClick="this.value='Validando...'; this.style.opacity='0.7'; return true;" />
                
            <div style="text-align: center; margin-top: 15px;">
                <a href="Login.aspx" class="forgot" style="color: #ef4444;">Cancelar y volver al Login</a>
            </div>
        </asp:Panel>
        
        <p class="register-line">
          &iquest;Nuevo por aqu&iacute;?
          <asp:LinkButton ID="lnk_registrar" runat="server" CssClass="register-link" OnClick="lnk_registrar_Click">Crea una cuenta</asp:LinkButton>
        </p>
      </div>
    </form>

    <script type="text/javascript">
        // @ts-nocheck
        let formSubmitted = false;

        function validarFormulario() {
            if (formSubmitted) {
                Swal.fire({
                    icon: 'info',
                    title: 'Procesando...',
                    text: 'Tu solicitud ya está en curso. Por favor, espera un momento.', 
                    confirmButtonColor: '#2563eb'
                });
                return false;
            }

            var txtUser = document.getElementById('<%= txt_ced.ClientID %>');
            var txtPass = document.getElementById('<%= txt_pass.ClientID %>');

            var userValue = txtUser ? txtUser.value.trim() : '';
            var passValue = txtPass ? txtPass.value.trim() : '';

            if (userValue === '') {
                Swal.fire({
                    icon: 'warning',
                    title: 'Campo Requerido',
                    text: 'Por favor, ingrese su identificación.', 
                    confirmButtonColor: '#2563eb'
                }).then(function() { if (txtUser) txtUser.focus(); });
                return false; 
            }

            if (passValue === '') {
                Swal.fire({
                    icon: 'warning',
                    title: 'Campo Requerido',
                    text: 'La contraseña es obligatoria.', 
                    confirmButtonColor: '#2563eb'
                }).then(function() { if (txtPass) txtPass.focus(); });
                return false; 
            }

            formSubmitted = true;
            
            var btn = document.getElementById('<%= btnLogin.ClientID %>');
            if (btn) {
                btn.value = 'Procesando...'; 
                btn.style.opacity = '0.7';
                btn.style.cursor = 'wait'; 
            }
            
            var pc = document.getElementById('progressContainer');
            var innerBar = document.getElementById('progressBarInner');
            
            if (pc && innerBar) {
                pc.style.display = 'block';
                setTimeout(function() {
                    pc.style.opacity = '1';
                    innerBar.style.width = '100%'; 
                }, 50);
            }

            setTimeout(function() {
                <%= ClientScript.GetPostBackEventReference(btnLogin, "") %>;
            }, 2000);

            return false; 
        }

        function togglePassword() {
            var input = document.getElementById('<%= txt_pass.ClientID %>');
            var icon = document.getElementById('eyeIcon');
            
            if (input && icon) {
                var isPass = input.type === 'password';
                input.type = isPass ? 'text' : 'password';
                
                icon.innerHTML = isPass 
                    ? '<path stroke-linecap="round" stroke-linejoin="round" d="M3.98 8.223A10.477 10.477 0 001.934 12C3.226 16.338 7.244 19.5 12 19.5c.993 0 1.953-.138 2.863-.395M6.228 6.228A10.45 10.45 0 0112 4.5c4.756 0 8.773 3.162 10.065 7.498a10.523 10.523 0 01-4.293 5.774M6.228 6.228L3 3m3.228 3.228l3.65 3.65m7.894 7.894L21 21m-3.228-3.228l-3.65-3.65m0 0a3 3 0 10-4.243-4.243m4.242 4.242L9.88 9.88" />'
                    : '<path stroke-linecap="round" stroke-linejoin="round" d="M2.036 12.322a1.012 1.012 0 010-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z"/><path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0Z"/>';
            }
        }

        // ====== LOGICA DE SCANNER QR (2FA) ======
        document.addEventListener("DOMContentLoaded", function () {
            let html5QrcodeScanner;
            const btnStart = document.getElementById("btnStartScan");
            const btnStop = document.getElementById("btnStopScan");
            const readerDiv = document.getElementById("qr-reader");
            const hfResult = document.getElementById('<%= hfQRResult.ClientID %>');
            const btnValidar2FA = document.getElementById('<%= btnValidar2FA.ClientID %>');

            if (btnStart) {
                btnStart.addEventListener("click", function () {
                    readerDiv.style.display = "block";
                    btnStart.style.display = "none";
                    btnStop.style.display = "inline-block";

                    // Inicializar el escaner (solicitara permisos de camara)
                    html5QrcodeScanner = new Html5Qrcode("qr-reader");
                    
                    const config = { fps: 10, qrbox: { width: 250, height: 250 } };
                    
                    html5QrcodeScanner.start(
                        { facingMode: "environment" }, 
                        config,
                        (decodedText, decodedResult) => {
                            // Cuando lee un codigo QR con exito
                            console.log(`Codigo escaneado: ${decodedText}`);
                            
                            // Guardar el codigo encriptado en el campo oculto
                            if(hfResult) hfResult.value = decodedText;
                            
                            // Detener camara
                            html5QrcodeScanner.stop().then(() => {
                                readerDiv.style.display = "none";
                                btnStop.style.display = "none";
                                btnStart.style.display = "inline-block";
                                btnStart.innerHTML = "&#9989; QR Escaneado Correctamente";
                                btnStart.style.background = "#22c55e";
                                
                                // Auto-enviar el formulario
                                if(btnValidar2FA) btnValidar2FA.click();
                            });
                        },
                        (errorMessage) => {
                            // Ignorar errores de "no se encontro codigo" (pasan 10 veces por segundo)
                        }
                    ).catch((err) => {
                        console.error(`Error al iniciar camara: ${err}`);
                        Swal.fire('Error de C\u00e1mara', 'No pudimos acceder a tu c\u00e1mara. Aseg\u00farate de darle permisos.', 'error');
                        readerDiv.style.display = "none";
                        btnStart.style.display = "inline-block";
                        btnStop.style.display = "none";
                    });
                });
            }

            if (btnStop) {
                btnStop.addEventListener("click", function () {
                    if (html5QrcodeScanner) {
                        html5QrcodeScanner.stop().then(() => {
                            readerDiv.style.display = "none";
                            btnStop.style.display = "none";
                            btnStart.style.display = "inline-block";
                        });
                    }
                });
            }
        });
    </script>
</body>
</html>