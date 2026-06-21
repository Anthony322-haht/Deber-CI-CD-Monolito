<%@ Page Title="Nuevo Producto" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="nuevo_tbl_producto.aspx.cs" Inherits="Monolito_4am_DB.Mantenimiento.nuevo_tbl_producto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-custom { border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.03); border: none; background: #fff; }
        .alert-soft { background-color: #f0f7ff; color: #334155; border-radius: 8px; border: none; font-weight: 500; border-left: 4px solid #2563eb; }
        .form-control-soft { background-color: #ffffff; border: 1px solid #cbd5e1; border-radius: 8px; padding: 12px 15px; color: #334155; font-weight: 500; }
        .form-control-soft:focus { border-color: #2563eb; box-shadow: 0 0 0 4px rgba(37,99,235,0.1); }
        .form-control-soft.input-error { border-color: #ef4444; box-shadow: 0 0 0 4px rgba(239,68,68,0.1); }
        .btn-top-save { background-color: #f8fafc; border: 1px solid #e2e8f0; color: #475569; font-weight: 600; border-radius: 8px; }
        .btn-bottom-save { background-color: #059669; color: white; font-weight: bold; border-radius: 8px; padding: 12px; font-size: 1.1rem; transition: background 0.3s; }
        .btn-bottom-save:hover { background-color: #047857; color: white; text-decoration: none; }
        .btn-back { background-color: #f8fafc; border: 1px solid #e2e8f0; color: #334155; font-weight: bold; border-radius: 8px; padding: 12px; }
        .alert-msg { border-radius: 8px; font-weight: 600; padding: 12px 18px; }
        .text-error { color: #ef4444; font-size: 0.85rem; font-weight: 500; margin-top: 4px; }
    </style>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="card card-custom">
                <div class="card-body p-5">
                    
                    <!-- Encabezado -->
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <h2 class="font-weight-bold text-dark m-0">Productos</h2>
                        <span class="badge text-white px-3 py-2" style="background-color: #2563eb; border-radius: 50rem;">NUEVO REGISTRO</span>
                    </div>
                    
                    <!-- Mensaje de resultado -->
                    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="mb-4">
                        <asp:Label ID="lblMensaje" runat="server" CssClass="d-block alert-msg"></asp:Label>
                    </asp:Panel>

                    <!-- Alerta -->
                    <div class="alert alert-soft mb-4 py-3" role="alert">
                        <i class="fa-solid fa-file-lines text-danger mr-2"></i> 
                        Completa los campos requeridos para registrar un nuevo producto en el sistema.
                    </div>

                    <!-- Botones de Acci&oacute;n Superiores -->
                    <div class="mb-4 d-flex">
                        <a href="nuevo_tbl_producto.aspx" class="btn text-white font-weight-bold px-3 mr-2" style="background-color: #2563eb; border-radius: 8px;">
                            + Nuevo Producto
                        </a>
                        <asp:LinkButton ID="btnGuardarTop" runat="server" CssClass="btn btn-top-save px-3" OnClick="btnGuardar_Click">
                            <i class="fa-solid fa-floppy-disk text-primary mr-1"></i> Guardar
                        </asp:LinkButton>
                    </div>

                    <!-- Formulario -->
                    <div class="row">
                        <div class="col-md-12 mb-4">
                            <label class="font-weight-bold">Nombre del Producto *</label>
                            <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control form-control-soft" Placeholder="Ingresa el nombre del producto" MaxLength="50"></asp:TextBox>
                            <asp:Label ID="lblErrorNombre" runat="server" CssClass="text-error" Visible="false"></asp:Label>
                        </div>
                        
                        <div class="col-md-6 mb-4">
                            <label class="font-weight-bold">Cantidad *</label>
                            <asp:TextBox ID="txtCantidad" runat="server" TextMode="Number" CssClass="form-control form-control-soft" Text="0" min="0"></asp:TextBox>
                            <asp:Label ID="lblErrorCantidad" runat="server" CssClass="text-error" Visible="false"></asp:Label>
                        </div>
                        
                        <div class="col-md-6 mb-4">
                            <label class="font-weight-bold">Precio *</label>
                            <asp:TextBox ID="txtPrecio" runat="server" TextMode="Number" step="0.01" CssClass="form-control form-control-soft" Text="0.00" min="0"></asp:TextBox>
                            <asp:Label ID="lblErrorPrecio" runat="server" CssClass="text-error" Visible="false"></asp:Label>
                        </div>
                        
                        <div class="col-md-6 mb-5">
                            <label class="font-weight-bold">Proveedor *</label>
                            <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-control form-control-soft">
                                <asp:ListItem Text="-- Selecciona un proveedor --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="lblErrorProveedor" runat="server" CssClass="text-error" Visible="false"></asp:Label>
                        </div>
                        
                        <div class="col-md-6 mb-5">
                            <label class="font-weight-bold">Categoría *</label>
                            <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control form-control-soft">
                                <asp:ListItem Text="-- Selecciona una categoría --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="lblErrorCategoria" runat="server" CssClass="text-error" Visible="false"></asp:Label>
                        </div>

                        <!-- Sección de Imágenes -->
                        <div class="col-md-12 mb-4 mt-2">
                            <hr style="border-color: #f1f5f9;" />
                            <h5 class="font-weight-bold text-dark mb-3"><i class="fa-solid fa-images text-primary mr-2"></i> Imágenes del Producto</h5>
                            <div class="alert alert-soft mb-3" role="alert" style="background-color: #f8fafc; border-left-color: #94a3b8;">
                                <i class="fa-solid fa-circle-info mr-2"></i> Puedes subir múltiples imágenes. Usa el botón "Previsualizar" para verlas antes de guardar.
                            </div>
                            
                            <div class="d-flex align-items-center mb-3">
                                <asp:FileUpload ID="fileUploadImagenes" runat="server" AllowMultiple="true" CssClass="form-control form-control-soft mr-2" accept=".jpg,.jpeg,.png,.gif" />
                                <asp:LinkButton ID="btnPrevisualizar" runat="server" CssClass="btn font-weight-bold px-4" style="background-color: #f1f5f9; color: #475569; border-radius: 8px; border: 1px solid #cbd5e1;" OnClientClick="return validarImagenesJS();" OnClick="btnPrevisualizar_Click">
                                    <i class="fa-solid fa-eye mr-1"></i> Previsualizar
                                </asp:LinkButton>
                            </div>
                            <asp:Label ID="lblErrorImagenes" runat="server" CssClass="text-error d-block mb-3" Visible="false"></asp:Label>

                            <!-- Galería de Previsualización -->
                            <asp:Panel ID="pnlGaleria" runat="server" Visible="false" CssClass="p-3" style="background-color: #f8fafc; border-radius: 12px; border: 1px dashed #cbd5e1;">
                                <h6 class="font-weight-bold text-secondary mb-3">Previsualización (Aún no guardadas):</h6>
                                <div class="row">
                                    <asp:Repeater ID="rptImagenesTemp" runat="server" OnItemCommand="rptImagenesTemp_ItemCommand">
                                        <ItemTemplate>
                                            <div class="col-md-3 col-6 mb-3 text-center">
                                                <div style="border-radius: 8px; overflow: hidden; border: 1px solid #e2e8f0; background: #fff; padding: 5px; position: relative;">
                                                    <img src='<%# Eval("RutaVirtual") %>' alt="Preview" class="img-fluid" style="height: 120px; object-fit: cover; width: 100%; border-radius: 4px;" />
                                                    <div class="mt-2 d-flex justify-content-between align-items-center px-1">
                                                        <small class="text-muted font-weight-bold" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 80%; text-align: left;" title='<%# Eval("Nombre") %>'><%# Eval("Nombre") %></small>
                                                        <asp:LinkButton ID="btnEliminarTemp" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("RutaFisica") %>' CssClass="text-danger" ToolTip="Quitar imagen"><i class="fa-solid fa-trash-can"></i></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:Panel>

                            <!-- Galería de Imágenes Actuales (Solo al Editar) -->
                            <asp:Panel ID="pnlImagenesActuales" runat="server" Visible="false" CssClass="p-3 mt-3" style="background-color: #f1f5f9; border-radius: 12px; border: 1px solid #cbd5e1;">
                                <h6 class="font-weight-bold text-dark mb-3"><i class="fa-solid fa-photo-film text-secondary mr-2"></i> Imágenes Actuales Guardadas:</h6>
                                <div class="row">
                                    <asp:Repeater ID="rptImagenesActuales" runat="server" OnItemCommand="rptImagenesActuales_ItemCommand">
                                        <ItemTemplate>
                                            <div class="col-md-3 col-6 mb-3 text-center">
                                                <div style="border-radius: 8px; overflow: hidden; border: 1px solid #cbd5e1; background: #fff; padding: 5px; position: relative;">
                                                    <img src='<%# Eval("imgp_ruta") %>' alt="Saved" class="img-fluid" style="height: 120px; object-fit: cover; width: 100%; border-radius: 4px;" />
                                                    <div class="mt-2 d-flex justify-content-between align-items-center px-1">
                                                        <small class="text-muted font-weight-bold" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 80%; text-align: left;" title='<%# Eval("imgp_nombre") %>'><%# Eval("imgp_nombre") %></small>
                                                        <asp:LinkButton ID="btnEliminarActual" runat="server" CommandName="EliminarActual" CommandArgument='<%# Eval("imgp_id") %>' 
                                                            CssClass="text-danger btn-swal-confirm" ToolTip="Eliminar imagen"
                                                            data-swal-title="¿Eliminar foto?" data-swal-text="Se borrará permanentemente." data-swal-type="warning">
                                                            <i class="fa-solid fa-trash-can"></i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>

                    <!-- Botones de Acción Inferiores -->
                    <div class="row mt-2">
                        <div class="col-md-6 pr-2">
                            <asp:LinkButton ID="btnGuardarBottom" runat="server" CssClass="btn btn-bottom-save w-100" OnClick="btnGuardar_Click" OnClientClick="return validarFormularioJS();">
                                <i class="fa-solid fa-check mr-2"></i> Guardar Producto
                            </asp:LinkButton>
                        </div>
                        <div class="col-md-6 pl-2">
                            <a href="listar_tbl_producto.aspx" class="btn btn-back w-100 text-center">
                                &larr; Regresar
                            </a>
                        </div>
                    </div>

                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnPrevisualizar" />
            <asp:PostBackTrigger ControlID="btnGuardarTop" />
            <asp:PostBackTrigger ControlID="btnGuardarBottom" />
        </Triggers>
    </asp:UpdatePanel>

    <!-- Scripts de Validación JS -->
    <script>
        function validarImagenesJS() {
            var input = document.getElementById('<%= fileUploadImagenes.ClientID %>');
            if (input.files.length === 0) {
                Swal.fire('Atenci\u00f3n', 'Selecciona al menos una imagen para previsualizar.', 'warning');
                return false;
            }

            var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;
            var maxSize = 2 * 1024 * 1024; // 2MB

            for (var i = 0; i < input.files.length; i++) {
                var file = input.files[i];
                if (!allowedExtensions.exec(file.name)) {
                    Swal.fire('Formato Inv\u00e1lido', 'El archivo ' + file.name + ' no es una imagen v\u00e1lida (.jpg, .png, .gif).', 'error');
                    return false;
                }
                if (file.size > maxSize) {
                    Swal.fire('Archivo Muy Pesado', 'El archivo ' + file.name + ' pesa m\u00e1s de 2MB.', 'error');
                    return false;
                }
            }
            return true;
        }

        function validarFormularioJS() {
            // Se puede agregar más validación front-end aquí si se desea.
            // Por ahora retorna true para dejar que el servidor valide.
            return true;
        }

        // SweetAlert2 con delegación de eventos para btn-swal-confirm
        document.body.addEventListener('click', function (e) {
            var btn = e.target.closest('.btn-swal-confirm');
            if (!btn) return;

            e.preventDefault();
            var targetUrl = btn.getAttribute('href');
            var title = btn.getAttribute('data-swal-title') || '¿Estás seguro?';
            var text = btn.getAttribute('data-swal-text') || '';
            var type = btn.getAttribute('data-swal-type') || 'question';

            Swal.fire({
                title: title,
                text: text,
                icon: type,
                showCancelButton: true,
                confirmButtonColor: '#2563eb',
                cancelButtonColor: '#ef4444',
                confirmButtonText: 'Sí, eliminar',
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
    </script>
</asp:Content>
