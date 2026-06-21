<%@ Page Title="Gesti&oacute;n de Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DesbloqueoUsuarios.aspx.cs" Inherits="Monolito_4am_DB.Administracion.DesbloqueoUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Panel de Administrador: Gesti&oacute;n de Usuarios</h2>
        <p>Aqu&iacute; puedes ver todos los usuarios registrados y desbloquear aquellos que superaron el l&iacute;mite de intentos.</p>
        
        <asp:Label ID="lblMensaje" runat="server" CssClass="text-success fw-bold d-block mb-3"></asp:Label>

        <div class="table-responsive">
            <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" 
                CssClass="table table-bordered table-striped table-hover"
                OnRowCommand="gvUsuarios_RowCommand">
                <Columns>
                    <asp:BoundField DataField="usu_id" HeaderText="ID" />
                    <asp:BoundField DataField="usu_nombres" HeaderText="Nombres" />
                    <asp:BoundField DataField="usu_apellidos" HeaderText="Apellidos" />
                    <asp:BoundField DataField="usu_cedula" HeaderText="C&eacute;dula" />
                    <asp:BoundField DataField="usu_correo" HeaderText="Correo" />
                    <asp:BoundField DataField="usu_intentos" HeaderText="Intentos Restantes" />
                    
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <%# Eval("usu_intentos") != null && Convert.ToInt32(Eval("usu_intentos")) <= 0 ? "<span class='badge bg-danger'>Bloqueado</span>" : "<span class='badge bg-success'>Activo</span>" %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnDesbloquear" runat="server" 
                                CommandName="Desbloquear" 
                                CommandArgument='<%# Eval("usu_id") %>' 
                                Text="Desbloquear" 
                                CssClass="btn btn-warning btn-sm"
                                Visible='<%# Eval("usu_intentos") != null && Convert.ToInt32(Eval("usu_intentos")) <= 0 %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
