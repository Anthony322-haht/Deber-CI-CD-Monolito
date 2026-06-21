<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Monolito_4am_DB._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        <!-- Título de la página -->
        <div class="row mb-3">
            <div class="col-12 d-flex justify-content-between align-items-center">
                <h3 class="m-0" style="color: #343a40;">Dashboard</h3>
                <ol class="breadcrumb m-0 bg-transparent p-0">
                    <li class="breadcrumb-item"><a href="#">Home</a></li>
                    <li class="breadcrumb-item active">Dashboard</li>
                </ol>
            </div>
        </div>

        <!-- Tarjetas de Métricas -->
        <div class="row">
            <!-- Tarjeta: Usuarios -->
            <div class="col-lg-3 col-6 mb-4">
                <div class="card shadow-sm border-0 h-100">
                    <div class="card-body p-0 d-flex">
                        <div class="bg-info text-white d-flex align-items-center justify-content-center" style="width: 80px; font-size: 2rem;">
                            <i class="fa-solid fa-user"></i>
                        </div>
                        <div class="p-3">
                            <h6 class="text-muted text-uppercase mb-1" style="font-size: 0.8rem;">Usuarios</h6>
                            <h3 class="mb-0 fw-bold">
                                <asp:Label ID="lblCantidadUsuarios" runat="server" Text="0"></asp:Label>
                            </h3>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tarjeta: Categorías -->
            <div class="col-lg-3 col-6 mb-4">
                <div class="card shadow-sm border-0 h-100">
                    <div class="card-body p-0 d-flex">
                        <div class="bg-danger text-white d-flex align-items-center justify-content-center" style="width: 80px; font-size: 2rem;">
                            <i class="fa-solid fa-briefcase"></i>
                        </div>
                        <div class="p-3">
                            <h6 class="text-muted text-uppercase mb-1" style="font-size: 0.8rem;">Categor&iacute;as</h6>
                            <h3 class="mb-0 fw-bold">41,410</h3>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tarjeta: Productos -->
            <div class="col-lg-3 col-6 mb-4">
                <div class="card shadow-sm border-0 h-100">
                    <div class="card-body p-0 d-flex">
                        <div class="bg-success text-white d-flex align-items-center justify-content-center" style="width: 80px; font-size: 2rem;">
                            <i class="fa-solid fa-box"></i>
                        </div>
                        <div class="p-3">
                            <h6 class="text-muted text-uppercase mb-1" style="font-size: 0.8rem;">Productos</h6>
                            <h3 class="mb-0 fw-bold">760</h3>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tarjeta: Ventas -->
            <div class="col-lg-3 col-6 mb-4">
                <div class="card shadow-sm border-0 h-100">
                    <div class="card-body p-0 d-flex">
                        <div class="bg-warning text-dark d-flex align-items-center justify-content-center" style="width: 80px; font-size: 2rem;">
                            <i class="fa-solid fa-cart-shopping"></i>
                        </div>
                        <div class="p-3">
                            <h6 class="text-muted text-uppercase mb-1" style="font-size: 0.8rem;">Ventas</h6>
                            <h3 class="mb-0 fw-bold">2,000</h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Novedad: Carrusel de Productos tipo Mercado Libre -->
        <div class="row mt-4">
            <div class="col-12">
                <h4 class="font-weight-bold text-dark mb-4"><i class="fa-solid fa-fire text-danger mr-2"></i> Nuestros Productos Destacados</h4>
                
                <!-- Swiper Contenedor Principal (Carrusel de Tarjetas de Productos) -->
                <div class="swiper swiper-productos" style="padding-bottom: 50px;">
                    <div class="swiper-wrapper">
                        <asp:Repeater ID="rptProductos" runat="server" OnItemDataBound="rptProductos_ItemDataBound">
                            <ItemTemplate>
                                <div class="swiper-slide">
                                    <div class="card border-0 shadow-sm" style="border-radius: 12px; overflow: hidden; height: 100%; border: 1px solid #e2e8f0 !important; transition: transform 0.2s;">
                                        
                                        <!-- Bootstrap Carousel Anidado (Carrusel de Múltiples Imágenes por Producto) -->
                                        <div id="carousel-<%# Eval("pro_id") %>" class="carousel slide" data-bs-ride="false" style="height: 220px; background-color: #f8fafc;">
                                            <div class="carousel-inner" style="height: 100%;">
                                                <asp:Repeater ID="rptImagenes" runat="server">
                                                    <ItemTemplate>
                                                        <div class='<%# Container.ItemIndex == 0 ? "carousel-item active" : "carousel-item" %>' style="height: 100%;">
                                                            <img src='<%# ResolveUrl(Container.DataItem.ToString()) %>' class="d-block w-100" style="object-fit: cover; height: 100%;" alt="Foto Producto" onerror="this.onerror=null; this.src='https://dummyimage.com/300x300/e2e8f0/64748b.png&text=Sin+Imagen';" />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                            <!-- Botones Bootstrap 5 Carousel -->
                                            <button class="carousel-control-prev" type="button" data-bs-target="#carousel-<%# Eval("pro_id") %>" data-bs-slide="prev" style="background: transparent; border: none;">
                                                <span class="carousel-control-prev-icon" aria-hidden="true" style="background-color: rgba(0,0,0,0.3); border-radius: 50%;"></span>
                                                <span class="visually-hidden">Anterior</span>
                                            </button>
                                            <button class="carousel-control-next" type="button" data-bs-target="#carousel-<%# Eval("pro_id") %>" data-bs-slide="next" style="background: transparent; border: none;">
                                                <span class="carousel-control-next-icon" aria-hidden="true" style="background-color: rgba(0,0,0,0.3); border-radius: 50%;"></span>
                                                <span class="visually-hidden">Siguiente</span>
                                            </button>
                                        </div>
                                        
                                        <div class="card-body d-flex flex-column">
                                            <small class="text-muted text-uppercase font-weight-bold" style="font-size: 0.75rem;"><%# Eval("cat_nombre") %></small>
                                            <h5 class="card-title font-weight-bold text-dark mt-1 mb-2" style="font-size: 1rem; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;"><%# Eval("pro_nombre") %></h5>
                                            <div class="mt-auto">
                                                <h4 class="text-dark font-weight-bold m-0"><%# Eval("pro_precio") %></h4>
                                                <small class="text-success font-weight-bold">Envío gratis</small>
                                            </div>
                                        </div>
                                        <div class="card-footer bg-white border-0 pb-3 pt-0">
                                            <button type="button" class="btn btn-light w-100 font-weight-bold text-primary" style="border-radius: 8px; background-color: #eff6ff;">Ver detalles</button>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <!-- Paginación y navegación del carrusel principal -->
                    <div class="swiper-pagination swiper-productos-pagination"></div>
                    <div class="swiper-button-next swiper-productos-next" style="color: #2563eb;"></div>
                    <div class="swiper-button-prev swiper-productos-prev" style="color: #2563eb;"></div>
                </div>
            </div>
        </div>

    </div>

    <!-- Swiper CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/swiper@10/swiper-bundle.min.css" />
    
    <!-- Swiper JS -->
    <script src="https://cdn.jsdelivr.net/npm/swiper@10/swiper-bundle.min.js"></script>
    
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            
            // Inicializar Swiper Principal (Productos)
            var swiperProductos = new Swiper(".swiper-productos", {
                slidesPerView: 1,
                spaceBetween: 20,
                pagination: {
                    el: ".swiper-productos-pagination",
                    clickable: true,
                    dynamicBullets: true,
                },
                navigation: {
                    nextEl: ".swiper-productos-next",
                    prevEl: ".swiper-productos-prev",
                },
                breakpoints: {
                    // responsive
                    640: {
                        slidesPerView: 2,
                        spaceBetween: 20,
                    },
                    768: {
                        slidesPerView: 3,
                        spaceBetween: 30,
                    },
                    1024: {
                        slidesPerView: 4,
                        spaceBetween: 30,
                    },
                    1280: {
                        slidesPerView: 5,
                        spaceBetween: 30,
                    },
                },
            });
        });
    </script>
</asp:Content>
