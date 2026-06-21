<%@ Page Title="Nueva Categoria" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="nuevo_tbl_categoria.aspx.cs" Inherits="Monolito_4am_DB.Mantenimiento.nuevo_tbl_categoria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-custom { border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.03); border: none; background: #fff; }
        .form-control-custom { border-radius: 8px; padding: 12px 15px; border: 1px solid #cbd5e1; }
        .form-control-custom:focus { border-color: #2563eb; box-shadow: 0 0 0 0.2rem rgba(37,99,235,0.1); }
        .btn-custom { border-radius: 8px; padding: 12px 24px; font-weight: 600; }
    </style>

    <div class="container mt-4 mb-5" style="max-width: 600px;">
        <div class="card card-custom">
            <div class="card-body p-4 p-md-5">
                
                <h4 class="font-weight-bold text-dark mb-4 text-center">
                    <i class="fa-solid fa-tag text-info mr-2"></i> 
                    <asp:Label ID="lblTitulo" runat="server" Text="Nueva Categoría"></asp:Label>
                </h4>

                <div class="form-group mb-4">
                    <label class="font-weight-bold text-secondary mb-2">Nombre de la Categoría <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control form-control-custom" Placeholder="Ej: Electrónica"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre" 
                        ErrorMessage="El nombre es obligatorio" CssClass="text-danger small mt-1 d-block" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="d-flex justify-content-between mt-5">
                    <a href="listar_tbl_categoria.aspx" class="btn btn-light btn-custom text-secondary border">
                        <i class="fa-solid fa-arrow-left mr-1"></i> Cancelar
                    </a>
                    <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-custom shadow-sm" OnClick="btnGuardar_Click" OnClientClick="return validarFormulario();">
                        <i class="fa-solid fa-save mr-1"></i> Guardar Categoría
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script>
        function validarFormulario() {
            var nombre = document.getElementById('<%= txtNombre.ClientID %>').value.trim();
            if (nombre === "") {
                Swal.fire({
                    icon: 'warning',
                    title: 'Faltan datos',
                    text: 'El nombre de la categoría es obligatorio.'
                });
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
