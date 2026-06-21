<%@ Page Title="Usuario Dashboard" Language="C#" MasterPageFile="~/Mantenimiento/Principal.Master" AutoEventWireup="true" CodeBehind="UsuarioDashboard.aspx.cs" Inherits="Monolito_4am.Mantenimiento.UsuarioDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Usuario Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-12">
                <div class="card card-success">
                    <div class="card-header">
                        <h3 class="card-title">Panel de Usuario</h3>
                    </div>
                    <div class="card-body">
                        <p>Bienvenido a tu panel de usuario.</p>
                        <!-- Aquí va el contenido del dashboard de usuario -->
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
