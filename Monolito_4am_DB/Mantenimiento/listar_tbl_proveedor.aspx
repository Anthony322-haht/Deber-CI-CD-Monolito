<%@ Page Title="Proveedores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="listar_tbl_proveedor.aspx.cs" Inherits="Monolito_4am_DB.Mantenimiento.listar_tbl_proveedor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-custom { border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.03); border: none; background: #fff; }
        .filter-box { background-color: #f8fafc; border-radius: 12px; padding: 20px; border: 1px solid #f1f5f9; }
        .table-modern th { border-top: none !important; border-bottom: 2px solid #f1f5f9; color: #475569; font-weight: 700; padding: 15px; }
        .table-modern td { vertical-align: middle; color: #64748b; padding: 15px; border-bottom: 1px solid #f8fafc; font-weight: 500; }
        .btn-action { background: transparent; border: none; font-weight: 600; font-size: 0.85rem; cursor: pointer; }
        .btn-edit { color: #f97316; } .btn-edit:hover { color: #ea580c; text-decoration: none; }
        .btn-deactivate { color: #f59e0b; } .btn-deactivate:hover { color: #d97706; text-decoration: none; }
        .btn-activate { color: #059669; } .btn-activate:hover { color: #047857; text-decoration: none; }
        .btn-delete { color: #ef4444; } .btn-delete:hover { color: #dc2626; text-decoration: none; }
        .form-control-pill { border-radius: 50rem; border: 1px solid #cbd5e1; }
        .form-control-pill:focus { border-color: #2563eb; box-shadow: 0 0 0 0.2rem rgba(37,99,235,0.1); }
        .alert-msg { border-radius: 8px; font-weight: 600; padding: 12px 18px; }
        .badge-activo { background-color: #dcfce7; color: #166534; padding: 5px 12px; border-radius: 50rem; font-weight: 600; font-size: 0.8rem; }
        .badge-inactivo { background-color: #fee2e2; color: #991b1b; padding: 5px 12px; border-radius: 50rem; font-weight: 600; font-size: 0.8rem; }
        /* Paginaci&oacute;n Num&eacute;rica Premium */
        .gridview-pager { background-color: #fff !important; border-top: none !important; }
        .gridview-pager td { border: none !important; padding: 16px 0 0 !important; }
        .gridview-pager table { margin: 0 auto; border-collapse: separate; border-spacing: 4px 0; }
        .gridview-pager table td { padding: 0 !important; border: none !important; }
        .gridview-pager table td a,
        .gridview-pager table td span {
            display: inline-block; min-width: 36px; padding: 7px 12px;
            border: 1px solid #dee2e6; border-radius: 6px;
            color: #2563eb; text-decoration: none; font-weight: 600;
            font-size: 0.88rem; text-align: center; transition: all 0.2s ease;
            background-color: #fff;
        }
        .gridview-pager table td a:hover { background-color: #eef2ff; border-color: #2563eb; color: #1d4ed8; }
        .gridview-pager table td span {
            background-color: #2563eb; color: #fff; border-color: #2563eb; cursor: default;
        }
    </style>

    <div class="card card-custom">
        <div class="card-body p-4">
            
            <!-- T&iacute;tulo y Bot&oacute;n Superior -->
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h4 class="m-0 font-weight-bold text-dark" style="font-size: 1.5rem;">
                    <i class="fa-solid fa-truck-field text-primary mr-2"></i> Gesti&oacute;n de Proveedores
                </h4>
                <a href="nuevo_tbl_proveedor.aspx" class="btn text-white px-4 font-weight-bold shadow-sm" style="background-color: #2563eb; border-radius: 50rem;">
                    + Nuevo Proveedor
                </a>
            </div>

            <!-- Caja de Filtros (FUERA del UpdatePanel para mantener el foco del cursor) -->
            <div class="filter-box mb-4">
                <div class="row align-items-end">
                    <div class="col-md-3">
                        <label class="font-weight-bold text-dark mb-2">Filtrar por Estado:</label>
                        <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-control form-control-pill" style="border-color: #2563eb;">
                            <asp:ListItem Text="Todos" Value="" />
                            <asp:ListItem Text="Activo" Value="A" />
                            <asp:ListItem Text="Inactivo" Value="I" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-5">
                        <label class="font-weight-bold text-dark mb-2">Nombre del Proveedor:</label>
                        <asp:TextBox ID="txtBuscarNombre" runat="server" CssClass="form-control form-control-pill" Placeholder="Escribe para buscar..." autocomplete="off"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <asp:LinkButton ID="btnBuscar" runat="server" CssClass="btn btn-light w-100 form-control-pill font-weight-bold text-secondary shadow-sm" OnClick="btnBuscar_Click">
                            <i class="fa-solid fa-magnifying-glass text-primary mr-1"></i> Buscar
                        </asp:LinkButton>
                    </div>
                    <div class="col-md-2">
                        <asp:LinkButton ID="btnLimpiar" runat="server" CssClass="btn btn-outline-secondary w-100 form-control-pill font-weight-bold shadow-sm" OnClick="btnLimpiar_Click">
                            <i class="fa-solid fa-arrows-rotate mr-1"></i> Limpiar
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <!-- UpdatePanel: Solo refresca la grilla y mensajes sin recargar toda la p&aacute;gina -->
            <asp:UpdatePanel ID="updProveedores" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- Mensaje de resultado -->
                    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="mb-4">
                        <asp:Label ID="lblMensaje" runat="server" CssClass="d-block alert-msg"></asp:Label>
                    </asp:Panel>

                    <!-- Tabla GridView -->
                    <div class="table-responsive">
                        <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-hover table-modern" GridLines="None"
                            DataKeyNames="prov_id" OnRowCommand="gvProveedores_RowCommand"
                            AllowPaging="True" PageSize="10" OnPageIndexChanging="gvProveedores_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="prov_id" HeaderText="ID" />
                                <asp:BoundField DataField="prov_nombre" HeaderText="Nombre" />
                                
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='<%# Eval("prov_estado").ToString() == "A" ? "badge-activo" : "badge-inactivo" %>'>
                                            <%# Eval("prov_estado").ToString() == "A" ? "Activo" : "Inactivo" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("prov_id") %>' CssClass="btn-action btn-edit mr-2" ToolTip="Editar proveedor">
                                            <i class="fa-solid fa-pencil mr-1"></i> Editar
                                        </asp:LinkButton>
                                        
                                        <asp:LinkButton ID="btnDesactivar" runat="server" CommandName="Desactivar" CommandArgument='<%# Eval("prov_id") %>' 
                                            CssClass="btn-action btn-deactivate btn-swal-confirm mr-2" ToolTip="Desactivar proveedor"
                                            Visible='<%# Eval("prov_estado").ToString() == "A" %>'
                                            data-swal-title="&iquest;Desactivar proveedor?" data-swal-text="Los productos asociados no se ver&aacute;n afectados directamente.">
                                            <i class="fa-solid fa-ban mr-1"></i> Desactivar
                                        </asp:LinkButton>
                                        
                                        <asp:LinkButton ID="btnActivar" runat="server" CommandName="Activar" CommandArgument='<%# Eval("prov_id") %>' 
                                            CssClass="btn-action btn-activate btn-swal-confirm mr-2" ToolTip="Activar proveedor"
                                            Visible='<%# Eval("prov_estado").ToString() == "I" %>'
                                            data-swal-title="&iquest;Activar proveedor?" data-swal-text="El proveedor volver&aacute; a estar operativo.">
                                            <i class="fa-solid fa-circle-check mr-1"></i> Activar
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("prov_id") %>' 
                                            CssClass="btn-action btn-delete btn-swal-confirm" ToolTip="Eliminar permanentemente"
                                            data-swal-title="&iquest;Eliminar permanentemente?" data-swal-text="&iexcl;Esta acci&oacute;n eliminar&aacute; el proveedor de la base de datos!" data-swal-type="warning">
                                            <i class="fa-solid fa-trash-can mr-1"></i> Eliminar
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center py-5">
                                    <i class="fa-solid fa-truck-ramp-box fa-3x text-muted mb-3"></i>
                                    <h5 class="text-muted font-weight-bold">No se encontraron proveedores</h5>
                                </div>
                            </EmptyDataTemplate>
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="&laquo;" LastPageText="&raquo;" PageButtonCount="6" />
                            <PagerStyle CssClass="gridview-pager" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnBuscar" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnLimpiar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>

    <!-- Script de Filtrado en Tiempo Real (Debounce 400ms) + SweetAlert2 con Delegaci&oacute;n -->
    <script>
        (function () {
            var timer = null;

            function setupLiveFilter() {
                var txt = document.getElementById('<%= txtBuscarNombre.ClientID %>');
                var ddl = document.getElementById('<%= ddlFiltroEstado.ClientID %>');
                var btn = document.getElementById('<%= btnBuscar.ClientID %>');

                if (txt && btn) {
                    txt.removeEventListener('input', txt._liveHandler);
                    txt._liveHandler = function () {
                        clearTimeout(timer);
                        timer = setTimeout(function () { btn.click(); }, 400);
                    };
                    txt.addEventListener('input', txt._liveHandler);
                }

                if (ddl && btn) {
                    ddl.removeEventListener('change', ddl._liveHandler);
                    ddl._liveHandler = function () {
                        clearTimeout(timer);
                        btn.click();
                    };
                    ddl.addEventListener('change', ddl._liveHandler);
                }
            }

            document.addEventListener('DOMContentLoaded', function () {
                setupLiveFilter();
                if (typeof Sys !== 'undefined' && Sys.WebForms) {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(setupLiveFilter);
                }
            });

            // SweetAlert2 con delegaci\u00f3n de eventos
            document.body.addEventListener('click', function (e) {
                var btn = e.target.closest('.btn-swal-confirm');
                if (!btn) return;

                e.preventDefault();
                var targetUrl = btn.getAttribute('href');
                var title = btn.getAttribute('data-swal-title') || '\u00bfEst\u00e1s seguro?';
                var text = btn.getAttribute('data-swal-text') || '';
                var type = btn.getAttribute('data-swal-type') || 'question';

                Swal.fire({
                    title: title,
                    text: text,
                    icon: type,
                    showCancelButton: true,
                    confirmButtonColor: '#2563eb',
                    cancelButtonColor: '#ef4444',
                    confirmButtonText: 'S\u00ed, continuar',
                    cancelButtonText: 'Cancelar'
                }).then(function (result) {
                    if (result.isConfirmed) {
                        if (targetUrl && targetUrl.indexOf('javascript:') === 0) {
                            eval(targetUrl.replace('javascript:', ''));
                        } else if (targetUrl) {
                            window.location.href = targetUrl;
                        }
                    }
                });
            });
        })();
    </script>
</asp:Content>
