using System;
using System.Text;
using System.Web.UI;

namespace Monolito_4am_DB.Mantenimiento
{
    public partial class rombo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Generador de Rombo";
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int tamaño;
                if (int.TryParse(txtTamano.Text, out tamaño))
                {
                    if (tamaño >= 3 && tamaño <= 50)
                    {
                        GenerarRombo(tamaño);
                        MostrarMensaje($"¡Matriz de tamaño {tamaño} generada matemáticamente!", "success");
                    }
                    else
                    {
                        MostrarMensaje("El tamaño debe ser mayor o igual a 3 y máximo 50.", "warning");
                    }
                }
                else
                {
                    MostrarMensaje("Por favor, ingrese un número entero válido.", "error");
                }
            }
            else
            {
                MostrarMensaje("Por favor, revise que los campos sean correctos.", "error");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string icon = tipo;
            string titulo = tipo == "success" ? "¡Éxito!" : (tipo == "warning" ? "Atención" : "¡Error!");
            string script = $"window.onload = function() {{ Swal.fire('{titulo}', '{mensaje}', '{icon}'); }};";
            ScriptManager.RegisterStartupScript(this, GetType(), "swal_msg", script, true);
        }

        private void GenerarRombo(int tamaño)
        {
            int dimensionesMatriz = tamaño * 2 - 1;
            char[,] matriz = new char[dimensionesMatriz, dimensionesMatriz];

            // Inicializamos toda la matriz con espacios vacíos
            for (int i = 0; i < dimensionesMatriz; i++)
                for (int j = 0; j < dimensionesMatriz; j++)
                    matriz[i, j] = ' ';

            // --- ALGORITMO VECTORIAL (Genera siempre hacia la derecha primero) ---
            int f = tamaño - 1;
            int c = dimensionesMatriz - 1;
            int dir = 1;

            // 0: Abajo-Derecha, 1: Abajo-Izquierda, 2: Arriba-Izquierda, 3: Arriba-Derecha
            int[] df = { 1, 1, -1, -1 };
            int[] dc = { 1, -1, -1, 1 };

            while (true)
            {
                matriz[f, c] = '*';

                int sigF = f + df[dir];
                int sigC = c + dc[dir];
                bool debeGirar = false;

                if (sigF < 0 || sigF >= dimensionesMatriz || sigC < 0 || sigC >= dimensionesMatriz)
                {
                    debeGirar = true;
                }
                else
                {
                    int f2 = sigF + df[dir];
                    int c2 = sigC + dc[dir];

                    if (f2 >= 0 && f2 < dimensionesMatriz && c2 >= 0 && c2 < dimensionesMatriz && matriz[f2, c2] == '*')
                    {
                        debeGirar = true;
                    }

                    int dirSiguiente = (dir + 1) % 4;
                    int ladoF = sigF + df[dirSiguiente];
                    int ladoC = sigC + dc[dirSiguiente];

                    if (ladoF >= 0 && ladoF < dimensionesMatriz && ladoC >= 0 && ladoC < dimensionesMatriz && matriz[ladoF, ladoC] == '*')
                    {
                        bool enElNucleo = sigF >= tamaño - 2 && sigF <= tamaño && sigC >= tamaño - 2 && sigC <= tamaño;

                        if (!(sigF == tamaño - 1 && sigC == tamaño - 1) && !enElNucleo)
                        {
                            debeGirar = true;
                        }
                    }
                }

                if (debeGirar)
                {
                    dir = (dir + 1) % 4;
                    sigF = f + df[dir];
                    sigC = c + dc[dir];
                }

                int diagonalFrenadoF = sigF + df[dir];
                int diagonalFrenadoC = sigC + dc[dir];

                if (diagonalFrenadoF >= 0 && diagonalFrenadoF < dimensionesMatriz &&
                    diagonalFrenadoC >= 0 && diagonalFrenadoC < dimensionesMatriz)
                {
                    if (matriz[diagonalFrenadoF, diagonalFrenadoC] == '*' && sigF >= tamaño - 3 && sigF <= tamaño + 1)
                    {
                        break;
                    }
                }

                if (sigF < 0 || sigF >= dimensionesMatriz || sigC < 0 || sigC >= dimensionesMatriz || matriz[sigF, sigC] == '*')
                {
                    break;
                }

                f = sigF;
                c = sigC;
            }

            // --- NUEVO: CORRECCIÓN PARA EL CENTRO EN TAMAÑOS PARES ---
            // Borramos literalmente "la punta" (el último asterisco) del espiral 
            // para que no quede ese punto solitario en el medio.
            if (tamaño % 2 == 0)
            {
                matriz[f, c] = ' '; 
            }

            // 2. INVERSIÓN DE ESPIRAL: Si es PAR, invertimos la matriz horizontalmente
            // Esto hace que la entrada del espiral comience desde la izquierda.
            if (tamaño % 2 == 0)
            {
                for (int i = 0; i < dimensionesMatriz; i++)
                {
                    for (int j = 0; j < dimensionesMatriz / 2; j++)
                    {
                        char temp = matriz[i, j];
                        matriz[i, j] = matriz[i, dimensionesMatriz - 1 - j];
                        matriz[i, dimensionesMatriz - 1 - j] = temp;
                    }
                }
            }

            // === CONSTRUCCIÓN DEL STRING (MARCO MATEMÁTICAMENTE CORREGIDO) ===
            StringBuilder sb = new StringBuilder();
            int maxCharsInRow = (tamaño * 2) - 1; 
            int anchoInterno = maxCharsInRow + 2; 

            sb.AppendLine("╔" + new string('═', anchoInterno) + "╗");

            for (int i = 0; i < dimensionesMatriz; i++)
            {
                sb.Append("║"); 

                int elementosEnFila = (i < tamaño) ? (i + 1) : (dimensionesMatriz - i);
                int charsInRow = (elementosEnFila * 2) - 1; 
                
                int espaciosIzquierda = (anchoInterno - charsInRow) / 2;
                int espaciosDerecha = anchoInterno - charsInRow - espaciosIzquierda;

                sb.Append(new string(' ', espaciosIzquierda));

                for (int j = 0; j < elementosEnFila; j++)
                {
                    int columnaMatriz = (tamaño - elementosEnFila) + (j * 2);
                    sb.Append(matriz[i, columnaMatriz]);

                    if (j < elementosEnFila - 1)
                        sb.Append(" "); 
                }

                sb.Append(new string(' ', espaciosDerecha));
                sb.AppendLine("║"); 
            }

            sb.AppendLine("╚" + new string('═', anchoInterno) + "╝");

            // Imprimir en el Literal del HTML
            litRombo.Text = sb.ToString();
            pnlResultado.Visible = true;
        }
    }
}
