<%@ Page Title="Juego de Memoria" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Memoria.aspx.cs" Inherits="Monolito_4am_DB.Juegos.Memoria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .memory-board {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 15px;
            max-width: 500px;
            margin: 20px auto;
            perspective: 1000px;
        }

        .memory-card-container {
            width: 100%;
            aspect-ratio: 1;
            position: relative;
            background: transparent;
            cursor: pointer;
        }

        .memory-card {
            width: 100%;
            height: 100%;
            position: absolute;
            transition: transform 0.6s;
            transform-style: preserve-3d;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            border-radius: 10px;
        }

        /* Clase aplicada desde el CodeBehind para voltear la carta */
        .memory-card.flipped {
            transform: rotateY(180deg);
        }

        .card-face {
            position: absolute;
            width: 100%;
            height: 100%;
            backface-visibility: hidden;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 3rem;
            border-radius: 10px;
        }

        .card-front {
            background: linear-gradient(135deg, #2563eb, #3b82f6);
            color: white;
            font-weight: bold;
        }

        .card-back {
            background: white;
            transform: rotateY(180deg);
            border: 2px solid #3b82f6;
        }

        .card-matched .card-back {
            background: #dcfce7;
            border-color: #22c55e;
        }

        /* Ocultar el linkbutton nativo y expandirlo sobre toda la carta */
        .invisible-btn {
            position: absolute;
            top: 0; left: 0; right: 0; bottom: 0;
            opacity: 0;
            width: 100%;
            height: 100%;
            z-index: 10;
        }
    </style>

    <div class="container text-center mt-4">
        <h2 style="color: #2563eb; font-weight: bold;">Adivinanza de Cartas</h2>
        <p class="text-muted">&#161;Encuentra todos los pares! La l&oacute;gica se procesa 100% en C#.</p>

        <asp:UpdatePanel ID="upGame" runat="server">
            <ContentTemplate>
                <div class="d-flex justify-content-center gap-4 mb-3">
                    <div class="h5">Puntaje: <asp:Label ID="lblPuntaje" runat="server" CssClass="fw-bold text-warning">0</asp:Label></div>
                    <div class="h5">Turnos: <asp:Label ID="lblTurnos" runat="server" CssClass="fw-bold text-primary">0</asp:Label></div>
                    <div class="h5">Pares: <asp:Label ID="lblPares" runat="server" CssClass="fw-bold text-success">0 / 8</asp:Label></div>
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="d-block mb-3 h4"></asp:Label>

                <div class="memory-board">
                    <asp:Repeater ID="rptCartas" runat="server" OnItemCommand="rptCartas_ItemCommand">
                        <ItemTemplate>
                            <div class="memory-card-container">
                                <!-- La clase 'flipped' se aplica si la carta esta volteada o ya fue emparejada -->
                                <div class='memory-card <%# (bool)Eval("EstaVolteada") || (bool)Eval("EstaEmparejada") ? "flipped" : "" %> <%# (bool)Eval("EstaEmparejada") ? "card-matched" : "" %>'>
                                    <div class="card-face card-front">
                                        ?
                                    </div>
                                    <div class="card-face card-back">
                                        <%# Eval("Icono") %>
                                    </div>
                                </div>
                                <!-- El boton invisible captura el clic y lo manda al servidor (C#) -->
                                <asp:LinkButton ID="btnVoltear" runat="server" 
                                    CommandName="Voltear" 
                                    CommandArgument='<%# Eval("Id") %>' 
                                    CssClass="invisible-btn"
                                    Enabled='<%# !(bool)Eval("EstaVolteada") && !(bool)Eval("EstaEmparejada") %>'>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnReiniciar" runat="server" Text="Reiniciar Juego" CssClass="btn btn-primary" OnClick="btnReiniciar_Click" />
                </div>
                
                <!-- Timer usado para volver a voltear las cartas incorrectas despues de 1 segundo -->
                <asp:Timer ID="tmrOcultar" runat="server" Interval="1000" Enabled="false" OnTick="tmrOcultar_Tick"></asp:Timer>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
