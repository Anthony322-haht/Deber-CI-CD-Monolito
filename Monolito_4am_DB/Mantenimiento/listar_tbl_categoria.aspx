<%@ Page Title="Categorias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="listar_tbl_categoria.aspx.cs" Inherits="Monolito_4am_DB.Mantenimiento.listar_tbl_categoria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-custom { border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.03); border: none; background: #fff; }
        .table-modern th { border-top: none !important; border-bottom: 2px solid #f1f5f9; color: #475569; font-weight: 700; padding: 15px; }
        .table-modern td { vertical-align: middle; color: #64748b; padding: 15px; border-bottom: 1px solid #f8fafc; font-weight: 500; }
        .btn-action { background: transparent; border: none; font-weight: 600; font-size: 0.85rem; cursor: pointer; }
        .btn-edit { color: #f97316; } .btn-edit:hover { color: #ea580c; text-decoration: none; }
        .btn-deactivate { color: #f59e0b; } .btn-deactivate:hover { color: #d97706; text-decoration: none; }
        .btn-activate { color: #059669; } .btn-activate:hover { color: #047857; text-decoration: none; }
        .btn-delete { color: #ef4444; } .btn-delete:hover { color: #dc2626; text-decoration: none; }
        .badge-activo { background-color: #dcfce7; color: #166534; padding: 5px 12px; border-radius: 50rem; font-weight: 600; font-size: 0.8rem; }
        .badge-inactivo { background-color: #fee2e2; color: #991b1b; padding: 5px 12px; border-radius: 50rem; font-weight: 600; font-size: 0.8rem; }
        /* Paginacion Numerica Premium */
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
            
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h4 class="m-0 font-weight-bold text-dark" style="font-size: 1.5rem;">
                    <i class="fa-solid fa-tags text-info mr-2"></i> Gesti&oacute;n de Categor&iacute;as
                </h4>
                <a href="nuevo_tbl_categoria.aspx" class="btn text-white px-4 font-weight-bold shadow-sm" style="background-color: #2563eb; border-radius: 50rem;">
                    + Nueva Categor&iacute;a
                </a>
            </div>

            <div class="table-responsive">
                <asp:GridView ID="gvCategorias" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover table-modern" GridLines="None"
                    DataKeyNames="cat_id" OnRowCommand="gvCategorias_RowCommand"
                    AllowPaging="True" PageSize="10" OnPageIndexChanging="gvCategorias_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="cat_id" HeaderText="ID" />
                        <asp:BoundField DataField="cat_nombre" HeaderText="Nombre" />
                        
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <span class='<%# Eval("cat_estado").ToString() == "A" ? "badge-activo" : "badge-inactivo" %>'>
                                    <%# Eval("cat_estado").ToString() == "A" ? "Activo" : "Inactivo" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("cat_id") %>' CssClass="btn-action btn-edit mr-2" ToolTip="Editar">
                                    <i class="fa-solid fa-pencil mr-1"></i> Editar
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnDesactivar" runat="server" CommandName="Desactivar" CommandArgument='<%# Eval("cat_id") %>' 
                                    CssClass="btn-action btn-deactivate btn-swal-confirm mr-2" data-swal-title="&iquest;Desactivar categor&iacute;a?" data-swal-text="La categor&iacute;a ya no estar&aacute; disponible para nuevos productos."
                                    Visible='<%# Eval("cat_estado").ToString() == "A" %>'>
                                    <i class="fa-solid fa-ban mr-1"></i> Desactivar
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnActivar" runat="server" CommandName="Activar" CommandArgument='<%# Eval("cat_id") %>' 
                                    CssClass="btn-action btn-activate btn-swal-confirm mr-2" data-swal-title="&iquest;Activar categor&iacute;a?" data-swal-text="La categor&iacute;a volver&aacute; a estar disponible."
                                    Visible='<%# Eval("cat_estado").ToString() == "I" %>'>
                                    <i class="fa-solid fa-circle-check mr-1"></i> Activar
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("cat_id") %>' 
                                    CssClass="btn-action btn-delete btn-swal-confirm" data-swal-title="&iquest;Eliminar permanentemente?" data-swal-text="&iexcl;No podr&aacute;s revertir esto!" data-swal-type="warning">
                                    <i class="fa-solid fa-trash-can mr-1"></i> Eliminar
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center py-5">
                            <i class="fa-solid fa-tags fa-3x text-muted mb-3"></i>
                            <h5 class="text-muted font-weight-bold">No se encontraron categor&iacute;as</h5>
                        </div>
                    </EmptyDataTemplate>
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="&laquo;" LastPageText="&raquo;" PageButtonCount="6" />
                    <PagerStyle CssClass="gridview-pager" />
                </asp:GridView>
            </div>

        </div>
    </div>

    <!-- Script para interceptar botones con clase btn-swal-confirm y usar SweetAlert2 (Con Delegación de Eventos para Paginación) -->
    <script type="text/javascript">
        /* global Swal */
        document.addEventListener('DOMContentLoaded', function () {
            // Usamos delegación de eventos en el cuerpo para no perder escuchadores al recargar el GridView (Paginación)
            document.body.addEventListener('click', function (e) {
                // @ts-ignore
                var targetElement = e.target || e.srcElement;
                if (!targetElement || !targetElement.closest) return;

                var btn = targetElement.closest('.btn-swal-confirm');
                if (!btn) return;

                e.preventDefault();
                var targetUrl = btn.getAttribute('href');
                var title = btn.getAttribute('data-swal-title') || '¿Estás seguro?';
                var text = btn.getAttribute('data-swal-text') || '';
                var type = btn.getAttribute('data-swal-type') || 'question';

                // @ts-ignore
                if (typeof Swal !== 'undefined') {
                    // @ts-ignore
                    Swal.fire({
                        title: title,
                        text: text,
                        icon: type,
                        showCancelButton: true,
                        confirmButtonColor: '#2563eb',
                        cancelButtonColor: '#ef4444',
                        confirmButtonText: 'Sí, continuar',
                        cancelButtonText: 'Cancelar'
                    }).then(function(result) {
                        if (result.isConfirmed) {
                            if (targetUrl && targetUrl.indexOf('javascript:') === 0) {
                                // Evaluar el postback nativo de ASP.NET
                                eval(targetUrl.replace('javascript:', ''));
                            } else if (targetUrl) {
                                window.location.href = targetUrl;
                            }
                        }
                    });
                }
            });
        });
    </script>
</asp:Content>
