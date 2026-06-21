<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RestablecerClave.aspx.cs" Inherits="Monolito_4am_DB.Seguridad.RestablecerClave" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8">
    <title>Restablecer Contrase&ntilde;a</title>
    <link href="https://fonts.googleapis.com/css2?family=Sora:wght@400;600;700&display=swap" rel="stylesheet" />
    <style>
        body { margin: 0; padding: 0; display: flex; align-items: center; justify-content: center; min-height: 100vh; font-family: 'Sora', sans-serif; background-color: #0d1117; color: white; }
        .card { background: #161b22; padding: 40px; border-radius: 12px; box-shadow: 0 8px 24px rgba(0,0,0,0.5); width: 100%; max-width: 400px; text-align: center; border: 1px solid #30363d; }
        h2 { margin: 0 0 10px 0; color: #58a6ff; }
        p { color: #8b949e; font-size: 0.9rem; margin-bottom: 30px; line-height: 1.5; }
        .input-group { margin-bottom: 20px; text-align: left; }
        label { display: block; margin-bottom: 8px; font-size: 0.85rem; color: #c9d1d9; }
        .form-input { width: 100%; box-sizing: border-box; padding: 12px; border-radius: 6px; border: 1px solid #30363d; background: #0d1117; color: white; font-family: inherit; }
        .form-input:focus { outline: none; border-color: #58a6ff; box-shadow: 0 0 0 3px rgba(88, 166, 255, 0.3); }
        .btn-recover { width: 100%; padding: 12px; background: #238636; color: white; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; transition: background 0.2s; font-family: inherit; font-size: 1rem; margin-top: 10px; }
        .btn-recover:hover { background: #2ea043; }
        .error-panel { display: none; background: rgba(225, 29, 72, 0.1); border: 1px solid #e11d48; color: #ff7b72; padding: 20px; border-radius: 8px; margin-bottom: 20px; }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="card" id="pnlFormulario" runat="server">
            <h2>Crear Nueva Contrase&ntilde;a</h2>
            <p>Ingresa tu nueva contrase&ntilde;a. Aseg&uacute;rate de que sea segura.</p>

            <div class="input-group">
                <label>Nueva Contrase&ntilde;a</label>
                <asp:TextBox ID="txtClave1" runat="server" CssClass="form-input" TextMode="Password" placeholder="M&iacute;nimo 8 caracteres, 1 may&uacute;s, 1 n&uacute;mero, 1 especial"></asp:TextBox>
            </div>
            
            <div class="input-group">
                <label>Confirmar Contrase&ntilde;a</label>
                <asp:TextBox ID="txtClave2" runat="server" CssClass="form-input" TextMode="Password" placeholder="Repite la contrase&ntilde;a"></asp:TextBox>
            </div>

            <asp:Button ID="btnRestablecer" runat="server" Text="Actualizar Contrase&ntilde;a" CssClass="btn-recover" OnClick="btnRestablecer_Click" OnClientClick="return validarClaves();" />
        </div>

        <div class="card error-panel" id="pnlError" runat="server">
            <h2>Enlace Inv&aacute;lido</h2>
            <p>El enlace de recuperaci&oacute;n es inv&aacute;lido o ha expirado. Por favor solicita uno nuevo.</p>
            <a href="RecuperarClave.aspx" class="btn-recover" style="display: inline-block; text-decoration: none; box-sizing: border-box;">Solicitar nuevo enlace</a>
        </div>
    </form>

    <script>
        function validarClaves() {
            var c1 = document.getElementById('<%= txtClave1.ClientID %>').value;
            var c2 = document.getElementById('<%= txtClave2.ClientID %>').value;

            if (c1 === '' || c2 === '') {
                Swal.fire('Error', 'Completa ambos campos', 'error'); return false;
            }
            if (c1 !== c2) {
                Swal.fire('Error', 'Las contraseñas no coinciden', 'error'); return false;
            }
            if (c1.length < 8) {
                Swal.fire('Error', 'Debe tener al menos 8 caracteres', 'error'); return false;
            }
            if (!/[A-Z]/.test(c1) || !/[a-z]/.test(c1) || !/[0-9]/.test(c1) || !/[!@#$%&*_\-]/.test(c1)) {
                Swal.fire('Contraseña Débil', 'Debe incluir al menos una mayúscula, una minúscula, un número y un carácter especial (!@#$%&*_-).', 'error'); return false;
            }

            var btn = document.getElementById('<%= btnRestablecer.ClientID %>');
            btn.value = 'Actualizando...';
            btn.style.opacity = '0.7';
            return true;
        }
    </script>
</body>
</html>
