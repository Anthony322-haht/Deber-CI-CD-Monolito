<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarClave.aspx.cs" Inherits="Monolito_4am_DB.Seguridad.RecuperarClave" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <title>Recuperar Contrase&ntilde;a</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@400;600;700&display=swap" rel="stylesheet" />
    <style>
        body { margin: 0; padding: 0; display: flex; align-items: center; justify-content: center; min-height: 100vh; font-family: 'Sora', sans-serif; background-color: #0d1117; color: white; }
        .card { background: #161b22; padding: 40px; border-radius: 12px; box-shadow: 0 8px 24px rgba(0,0,0,0.5); width: 100%; max-width: 400px; text-align: center; border: 1px solid #30363d; }
        .logo { width: 80px; margin-bottom: 20px; }
        h2 { margin: 0 0 10px 0; color: #58a6ff; }
        p { color: #8b949e; font-size: 0.9rem; margin-bottom: 30px; line-height: 1.5; }
        .input-group { margin-bottom: 20px; text-align: left; }
        label { display: block; margin-bottom: 8px; font-size: 0.85rem; color: #c9d1d9; }
        .form-input { width: 100%; box-sizing: border-box; padding: 12px; border-radius: 6px; border: 1px solid #30363d; background: #0d1117; color: white; font-family: inherit; }
        .form-input:focus { outline: none; border-color: #58a6ff; box-shadow: 0 0 0 3px rgba(88, 166, 255, 0.3); }
        .btn-recover { width: 100%; padding: 12px; background: #238636; color: white; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; transition: background 0.2s; font-family: inherit; font-size: 1rem; }
        .btn-recover:hover { background: #2ea043; }
        .back-link { display: block; margin-top: 20px; color: #58a6ff; text-decoration: none; font-size: 0.85rem; }
        .back-link:hover { text-decoration: underline; }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="card">
            <asp:Panel ID="pnlPaso1" runat="server">
                <h2>Recuperar Contrase&ntilde;a</h2>
                <p>Ingresa tu c&eacute;dula o correo electr&oacute;nico y te enviaremos un c&oacute;digo por WhatsApp.</p>

                <div class="input-group">
                    <label>C&eacute;dula o Correo Electr&oacute;nico</label>
                    <asp:TextBox ID="txtIdentificador" runat="server" CssClass="form-input" placeholder="ej. 1700000000 o correo@ejemplo.com"></asp:TextBox>
                </div>

                <asp:Button ID="btnRecuperar" runat="server" Text="Enviar C&oacute;digo" CssClass="btn-recover" OnClick="btnRecuperar_Click" OnClientClick="if(document.getElementById('txtIdentificador').value.trim() === '') { Swal.fire('Error', 'Ingresa tu correo o c&eacute;dula', 'error'); return false; } this.value='Enviando...'; this.style.opacity='0.7'; return true;" />

                <a href="Login.aspx" class="back-link">Volver al Inicio de Sesi&oacute;n</a>
            </asp:Panel>

            <asp:Panel ID="pnlPaso2" runat="server" Visible="false">
                <h2>C&oacute;digo de WhatsApp</h2>
                <p>Hemos enviado un c&oacute;digo de 6 d&iacute;gitos a tu WhatsApp. Ingr&eacute;salo a continuaci&oacute;n para continuar.</p>

                <div class="input-group">
                    <label>C&oacute;digo de 6 D&iacute;gitos</label>
                    <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-input" placeholder="Ej. 123456" MaxLength="6"></asp:TextBox>
                </div>

                <asp:Button ID="btnValidar" runat="server" Text="Verificar C&oacute;digo" CssClass="btn-recover" OnClick="btnValidar_Click" OnClientClick="if(document.getElementById('txtCodigo').value.trim() === '') { Swal.fire('Error', 'Ingresa el c&oacute;digo', 'error'); return false; } this.value='Verificando...'; this.style.opacity='0.7'; return true;" />

                <a href="RecuperarClave.aspx" class="back-link">Solicitar otro c&oacute;digo</a>
            </asp:Panel>
            
            <asp:HiddenField ID="hfCedula" runat="server" />
            <asp:HiddenField ID="hfToken" runat="server" />
        </div>
    </form>
</body>
</html>
