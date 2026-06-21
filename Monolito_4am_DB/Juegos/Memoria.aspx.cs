using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Datos.Modelos;

namespace Monolito_4am_DB.Juegos
{
    public partial class Memoria : System.Web.UI.Page
    {
        // Clase interna para representar una carta
        [Serializable]
        public class Carta
        {
            public int Id { get; set; }
            public string Icono { get; set; }
            public bool EstaVolteada { get; set; }
            public bool EstaEmparejada { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Seguridad: Validar que sea Usuario (ID 2)
                var usuario = Session["usuario"] as tbl_usuario;
                if (usuario == null || usuario.tusu_id != "2")
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                IniciarJuego();
            }
        }

        private void IniciarJuego()
        {
            // Lista de emojis (8 pares)
            string[] emojis = { "🍎", "🍌", "🍇", "🍉", "🍓", "🍒", "🥝", "🍍" };
            var mazo = new List<Carta>();

            int idCounter = 1;
            foreach (var emoji in emojis)
            {
                // Agregamos el par
                mazo.Add(new Carta { Id = idCounter++, Icono = emoji, EstaVolteada = false, EstaEmparejada = false });
                mazo.Add(new Carta { Id = idCounter++, Icono = emoji, EstaVolteada = false, EstaEmparejada = false });
            }

            // Revolver usando LINQ y Guid
            var mazoMezclado = mazo.OrderBy(x => Guid.NewGuid()).ToList();

            // Guardar el estado en Session
            Session["Memoria_Mazo"] = mazoMezclado;
            Session["Memoria_PrimeraCartaId"] = null;
            Session["Memoria_Turnos"] = 0;
            Session["Memoria_ParesEncontrados"] = 0;
            Session["Memoria_Puntaje"] = 0;
            Session["Memoria_Bloqueado"] = false; // Bloquea clics mientras el Timer hace su trabajo

            ActualizarInterfaz();
            lblMensaje.Text = "";
        }

        private void ActualizarInterfaz()
        {
            var mazo = Session["Memoria_Mazo"] as List<Carta>;
            rptCartas.DataSource = mazo;
            rptCartas.DataBind();

            lblPuntaje.Text = Session["Memoria_Puntaje"].ToString();
            lblTurnos.Text = Session["Memoria_Turnos"].ToString();
            lblPares.Text = Session["Memoria_ParesEncontrados"] + " / 8";

            if ((int)Session["Memoria_ParesEncontrados"] == 8)
            {
                lblMensaje.Text = "🎉 ¡Ganaste! ¡Excelente memoria! 🎉";
                lblMensaje.CssClass = "d-block mb-3 h4 text-success fw-bold";
            }
        }

        protected void rptCartas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Voltear")
            {
                // Si el tablero está bloqueado (esperando timer), ignorar clic
                if ((bool)Session["Memoria_Bloqueado"]) return;

                int cartaId = Convert.ToInt32(e.CommandArgument);
                var mazo = Session["Memoria_Mazo"] as List<Carta>;
                var cartaClicada = mazo.FirstOrDefault(c => c.Id == cartaId);

                // Si ya está volteada o emparejada, no hacer nada
                if (cartaClicada == null || cartaClicada.EstaVolteada || cartaClicada.EstaEmparejada) return;

                // Voltear la carta
                cartaClicada.EstaVolteada = true;

                // Logica del juego
                var primeraCartaId = Session["Memoria_PrimeraCartaId"];

                if (primeraCartaId == null)
                {
                    // Es la primera carta que se voltea
                    Session["Memoria_PrimeraCartaId"] = cartaId;
                }
                else
                {
                    // Es la segunda carta
                    int idPrimera = (int)primeraCartaId;
                    var cartaPrimera = mazo.FirstOrDefault(c => c.Id == idPrimera);

                    Session["Memoria_Turnos"] = (int)Session["Memoria_Turnos"] + 1;

                    if (cartaPrimera.Icono == cartaClicada.Icono)
                    {
                        // ¡Hay un par!
                        cartaPrimera.EstaEmparejada = true;
                        cartaClicada.EstaEmparejada = true;
                        Session["Memoria_ParesEncontrados"] = (int)Session["Memoria_ParesEncontrados"] + 1;
                        Session["Memoria_Puntaje"] = (int)Session["Memoria_Puntaje"] + 100;
                        Session["Memoria_PrimeraCartaId"] = null;
                    }
                    else
                    {
                        // No son iguales. Bloquear tablero y activar Timer
                        Session["Memoria_Puntaje"] = Math.Max(0, (int)Session["Memoria_Puntaje"] - 10);
                        Session["Memoria_Bloqueado"] = true;
                        Session["Memoria_SegundaCartaId"] = cartaId;
                        tmrOcultar.Enabled = true; // El timer se disparará en 1000ms
                    }
                }

                ActualizarInterfaz();
            }
        }

        protected void tmrOcultar_Tick(object sender, EventArgs e)
        {
            // Detener el timer
            tmrOcultar.Enabled = false;

            var mazo = Session["Memoria_Mazo"] as List<Carta>;
            int idPrimera = (int)Session["Memoria_PrimeraCartaId"];
            int idSegunda = (int)Session["Memoria_SegundaCartaId"];

            // Voltear ambas cartas hacia abajo
            mazo.FirstOrDefault(c => c.Id == idPrimera).EstaVolteada = false;
            mazo.FirstOrDefault(c => c.Id == idSegunda).EstaVolteada = false;

            // Restablecer estado
            Session["Memoria_PrimeraCartaId"] = null;
            Session["Memoria_SegundaCartaId"] = null;
            Session["Memoria_Bloqueado"] = false;

            ActualizarInterfaz();
        }

        protected void btnReiniciar_Click(object sender, EventArgs e)
        {
            IniciarJuego();
        }
    }
}
