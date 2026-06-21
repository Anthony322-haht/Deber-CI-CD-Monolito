<%@ Page Title="Importar Excel" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="importar_excel.aspx.cs" Inherits="Monolito_4am_DB.Mantenimiento.importar_excel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-custom { border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.03); border: none; background: #fff; }
        .alert-soft { background-color: #f0f7ff; color: #334155; border-radius: 8px; border: none; font-weight: 500; border-left: 4px solid #2563eb; }
        .form-control-soft { background-color: #ffffff; border: 1px solid #cbd5e1; border-radius: 8px; padding: 12px 15px; color: #334155; font-weight: 500; }
        .form-control-soft:focus { border-color: #2563eb; box-shadow: 0 0 0 4px rgba(37,99,235,0.1); }
        .btn-upload { background-color: #059669; color: white; font-weight: bold; border-radius: 8px; padding: 12px 20px; transition: background 0.3s; border: none; }
        .btn-upload:hover { background-color: #047857; color: white; }
        .alert-msg { border-radius: 8px; font-weight: 600; padding: 12px 18px; }
        .log-box { background: #1e293b; color: #f8fafc; border-radius: 8px; padding: 15px; font-family: monospace; font-size: 0.9rem; max-height: 400px; overflow-y: auto; }
        .log-success { color: #4ade80; }
        .log-info { color: #60a5fa; }
        .log-error { color: #f87171; }
        .log-warning { color: #fbbf24; }
    </style>

    <div class="card card-custom">
        <div class="card-body p-5">
            <!-- Encabezado -->
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h2 class="font-weight-bold text-dark m-0"><i class="fa-solid fa-file-excel text-success mr-2"></i> Importación Masiva</h2>
                <span class="badge text-white px-3 py-2" style="background-color: #10b981; border-radius: 50rem;">EXCEL / CSV</span>
            </div>

            <!-- Alerta -->
            <div class="alert alert-soft mb-4 py-3" role="alert">
                <i class="fa-solid fa-circle-info text-primary mr-2"></i> 
                Sube un archivo Excel (.xlsx, .xls) o CSV con las siguientes columnas (la primera fila debe ser el encabezado): <strong>Nombre, Cantidad, Precio, Categoria, Proveedor</strong>.<br />
                - Si el producto ya existe, se <strong>actualizará</strong>.<br />
                - Si no existe, se <strong>insertará</strong>.<br />
                - Si la categoría o el proveedor no existen, se <strong>crearán automáticamente</strong>.
            </div>

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="mb-4">
                <asp:Label ID="lblMensaje" runat="server" CssClass="d-block alert-msg"></asp:Label>
            </asp:Panel>

            <div class="row align-items-center mb-5">
                <div class="col-md-8">
                    <label class="font-weight-bold text-dark">Archivo a importar:</label>
                    <asp:FileUpload ID="fileUploadExcel" runat="server" CssClass="form-control form-control-soft" accept=".xls,.xlsx,.csv" />
                </div>
                <div class="col-md-4 mt-4 text-right">
                    <asp:LinkButton ID="btnPrevisualizar" runat="server" CssClass="btn btn-upload w-100" OnClick="btnPrevisualizar_Click" OnClientClick="return validarArchivoJS();">
                        <i class="fa-solid fa-eye mr-2"></i> Previsualizar
                    </asp:LinkButton>
                </div>
            </div>

            <!-- Panel de Previsualización -->
            <asp:Panel ID="pnlPreview" runat="server" Visible="false">
                <hr style="border-color: #e2e8f0;" class="my-4" />
                <h5 class="font-weight-bold text-dark mb-3"><i class="fa-solid fa-table-list mr-2 text-primary"></i> Previsualización de Datos</h5>
                
                <div class="table-responsive mb-4">
                    <table class="table table-hover table-modern" style="border: 1px solid #f1f5f9;">
                        <thead style="background-color: #f8fafc; color: #475569; font-weight: 700;">
                            <tr>
                                <th style="padding: 15px;">Fila</th>
                                <th style="padding: 15px;">Acción</th>
                                <th style="padding: 15px;">Imágenes</th>
                                <th style="padding: 15px;">Nombre</th>
                                <th style="padding: 15px;">Cantidad</th>
                                <th style="padding: 15px;">Precio</th>
                                <th style="padding: 15px;">Categoría</th>
                                <th style="padding: 15px;">Proveedor</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptPreview" runat="server">
                                <ItemTemplate>
                                    <tr style="border-bottom: 1px solid #f8fafc;">
                                        <td style="padding: 15px; vertical-align: middle;"><%# Eval("Fila") %></td>
                                        <td style="padding: 15px; vertical-align: middle;">
                                            <span class='<%# Eval("Accion").ToString() == "Nuevo" ? "badge badge-success px-2 py-1" : (Eval("Accion").ToString() == "Actualizar" ? "badge badge-primary px-2 py-1" : "badge badge-danger px-2 py-1") %>'>
                                                <%# Eval("Accion") %>
                                            </span>
                                        </td>
                                        <td style="padding: 15px; vertical-align: middle; width: 150px;">
                                            <div id="carousel-prev-<%# Eval("Fila") %>" class="carousel slide" data-bs-ride="false" style="height: 100px; width: 100px; background-color: #f8fafc; border-radius: 8px; overflow: hidden; border: 1px solid #e2e8f0;">
                                                <div class="carousel-inner" style="height: 100%;">
                                                    <asp:Repeater ID="rptPreviewImg" runat="server" DataSource='<%# Eval("Imagenes") %>'>
                                                        <ItemTemplate>
                                                            <div class='<%# Container.ItemIndex == 0 ? "carousel-item active" : "carousel-item" %>' style="height: 100%;">
                                                                <img src='<%# ResolveUrl(Container.DataItem.ToString()) %>' class="d-block w-100" style="object-fit: cover; height: 100%;" alt="Img" onerror="this.onerror=null; this.src='https://dummyimage.com/100x100/e2e8f0/64748b.png&text=Sin+Imagen';" />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <button class="carousel-control-prev" type="button" data-bs-target="#carousel-prev-<%# Eval("Fila") %>" data-bs-slide="prev" style="background: transparent; border: none; <%# ((List<string>)Eval("Imagenes")).Count <= 1 ? "display:none;" : "" %>">
                                                    <span class="carousel-control-prev-icon" aria-hidden="true" style="background-color: rgba(0,0,0,0.3); border-radius: 50%; width: 20px; height: 20px;"></span>
                                                    <span class="visually-hidden">Anterior</span>
                                                </button>
                                                <button class="carousel-control-next" type="button" data-bs-target="#carousel-prev-<%# Eval("Fila") %>" data-bs-slide="next" style="background: transparent; border: none; <%# ((List<string>)Eval("Imagenes")).Count <= 1 ? "display:none;" : "" %>">
                                                    <span class="carousel-control-next-icon" aria-hidden="true" style="background-color: rgba(0,0,0,0.3); border-radius: 50%; width: 20px; height: 20px;"></span>
                                                    <span class="visually-hidden">Siguiente</span>
                                                </button>
                                            </div>
                                        </td>
                                        <td style="padding: 15px; vertical-align: middle; color: #64748b; font-weight: 500;"><%# Eval("Nombre") %></td>
                                        <td style="padding: 15px; vertical-align: middle; color: #64748b;"><%# Eval("Cantidad") %></td>
                                        <td style="padding: 15px; vertical-align: middle; color: #64748b;"><%# string.Format("{0:C2}", Eval("Precio")) %></td>
                                        <td style="padding: 15px; vertical-align: middle; color: #64748b;"><%# Eval("Categoria") %></td>
                                        <td style="padding: 15px; vertical-align: middle; color: #64748b;"><%# Eval("Proveedor") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <div class="text-right">
                    <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-secondary font-weight-bold px-4 py-2 mr-2" style="border-radius: 8px;" OnClick="btnCancelar_Click">
                        <i class="fa-solid fa-xmark mr-2"></i> Cancelar
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnConfirmar" runat="server" CssClass="btn btn-primary font-weight-bold px-4 py-2" style="border-radius: 8px;" OnClick="btnConfirmar_Click">
                        <i class="fa-solid fa-check mr-2"></i> Confirmar e Importar
                    </asp:LinkButton>
                </div>
            </asp:Panel>

            <!-- Panel de Resultados (Log) -->
            <asp:Panel ID="pnlResultados" runat="server" Visible="false">
                <hr style="border-color: #e2e8f0;" class="my-4" />
                <h5 class="font-weight-bold text-dark mb-3"><i class="fa-solid fa-terminal mr-2 text-secondary"></i> Log de Procesamiento</h5>
                <div class="log-box" id="divLog" runat="server">
                </div>
            </asp:Panel>
        </div>
    </div>

    <!-- Script de Validación -->
    <script>
        function validarArchivoJS() {
            var input = document.getElementById('<%= fileUploadExcel.ClientID %>');
            if (input.files.length === 0) {
                Swal.fire('Atención', 'Selecciona un archivo Excel o CSV para procesar.', 'warning');
                return false;
            }

            var allowedExtensions = /(\.xlsx|\.xls|\.csv)$/i;
            var file = input.files[0];
            if (!allowedExtensions.exec(file.name)) {
                Swal.fire('Formato Inválido', 'Solo se permiten archivos .xlsx, .xls o .csv', 'error');
                return false;
            }

            // Mostrar spinner de carga si se usa SweetAlert2
            Swal.fire({
                title: 'Procesando archivo...',
                html: 'Por favor, no cierres esta ventana.',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading()
                }
            });

            return true;
        }
    </script>
</asp:Content>
