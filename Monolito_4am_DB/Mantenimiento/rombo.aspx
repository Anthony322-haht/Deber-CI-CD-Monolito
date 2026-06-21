<%@ Page Title="Generador de Rombo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="rombo.aspx.cs" Inherits="Monolito_4am_DB.Mantenimiento.rombo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-custom { border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.03); border: none; background: #fff; }
        .form-control-custom { border-radius: 8px; padding: 12px 15px; border: 1px solid #cbd5e1; }
        .form-control-custom:focus { border-color: #2563eb; box-shadow: 0 0 0 0.2rem rgba(37,99,235,0.1); }
        .btn-custom { border-radius: 8px; padding: 12px 24px; font-weight: 600; }
        /* Estilos para que el rombo se vea exacto como en consola */
        .ascii-art { 
            font-family: 'Courier New', Courier, monospace; 
            background: #1e1e1e; 
            color: #d4d4d4; 
            padding: 20px; 
            border-radius: 8px; 
            overflow-x: auto; 
            text-align: center; 
            line-height: 1.2; 
            font-size: 16px;
            font-weight: bold;
            margin: 0 auto;
            display: inline-block;
        }
        .ascii-container {
            text-align: center;
        }
    </style>

    <div class="container mt-4 mb-5">
        <div class="card card-custom">
            <div class="card-body p-4 p-md-5">
                <h4 class="font-weight-bold text-dark mb-4 text-center">
                    <i class="fa-solid fa-shapes text-info mr-2"></i> Generador de Rombo (Reto del Aula)
                </h4>

                <div class="row justify-content-center">
                    <div class="col-md-6">
                        <div class="form-group mb-4">
                            <label class="font-weight-bold text-secondary mb-2">Ingrese el tamaño del rombo <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtTamano" runat="server" CssClass="form-control form-control-custom" TextMode="Number" Placeholder="Ej: 5"></asp:TextBox>
                        </div>
                        
                        <div class="text-center mb-4">
                            <asp:Button ID="btnGenerar" runat="server" Text="Generar Rombo" CssClass="btn btn-info btn-custom text-white" OnClick="btnGenerar_Click" />
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                    <hr />
                    <h5 class="text-center mb-3 text-secondary font-weight-bold">Resultado de la Matriz:</h5>
                    <div class="ascii-container">
                        <pre class="ascii-art"><asp:Literal ID="litRombo" runat="server"></asp:Literal></pre>
                    </div>
                </asp:Panel>

            </div>
        </div>
    </div>
</asp:Content>
