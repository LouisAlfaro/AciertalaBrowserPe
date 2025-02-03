using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WebviewAlberto
{
    public partial class Form2 : Form
    {
        // APIs nativas para registrar/desregistrar HotKeys
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_NONE = 0x0000; // Sin modificadores

        // Para arrastrar la ventana
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private Panel panelLateral;
        private Panel panelBotonesContainer;
        private FlowLayoutPanel panelBotones;
        private Button btnHome;
        private bool botonesVisibles = false;
        private int alturaBotones = 0;

        public Form2()
        {
            InitializeComponent();

            // Configurar formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.Size = new Size(182, 60);
            this.DoubleBuffered = true;
            this.MouseDown += Form2_MouseDown;

            // Registrar hotkeys globales
            RegistrarHotKeys();

            GenerarInterfaz();
            GenerarBotones();
            CalcularAlturaBotones();

            // Ubicar en la parte superior/derecha de la pantalla
            PosicionarArribaDerecha();

            // Manejar cambio de resolución/preferencias
            SystemEvents.DisplaySettingsChanged += (s, e) => PosicionarArribaDerecha();
            SystemEvents.UserPreferenceChanged += (s, e) => PosicionarArribaDerecha();
        }

        // Registrar las teclas F5, F6, F11 como hotkeys globales
        private void RegistrarHotKeys()
        {
            // F5 → ID 1
            RegisterHotKey(this.Handle, 1, MOD_NONE, (uint)Keys.F5);
            // F6 → ID 2
            RegisterHotKey(this.Handle, 2, MOD_NONE, (uint)Keys.F6);
            // F11 → ID 3
            RegisterHotKey(this.Handle, 3, MOD_NONE, (uint)Keys.F11);
        }

        // Al cerrar el formulario, desregistrar los hotkeys para no dejar basura en el sistema
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            UnregisterHotKey(this.Handle, 1); // F5
            UnregisterHotKey(this.Handle, 2); // F6
            UnregisterHotKey(this.Handle, 3); // F11
        }

        // Interceptar mensajes de Windows
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                switch ((int)m.WParam)
                {
                    case 1: // F5
                        ReiniciarAciertalaGlobal();
                        break;
                    case 2: // F6
                        EjecutarProgramaGlobal(@"C:\BotonesAciertala\BotonF6.exe");
                        break;
                    case 3: // F11
                        EjecutarProgramaGlobal(@"C:\BotonesAciertala\BotonF11.exe");
                        break;
                }
            }
        }

        // Mueve la ventana cuando se arrastra con el mouse
        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        // Colocar la ventana en la parte superior derecha según la resolución
        private void PosicionarArribaDerecha()
        {
            Rectangle wa = Screen.PrimaryScreen.WorkingArea;
            int screenWidth = wa.Width;

            // Valores por defecto
            int offsetDerecha = 250;
            int offsetArriba = 10;

            // Ajustar offsetDerecha según la resolución
            if (screenWidth < 1280)
            {
                offsetDerecha = 300;
            }
            else if (screenWidth <= 1440)
            {
                offsetDerecha = 280;
            }

            int x = wa.Right - this.Width - offsetDerecha;
            int y = wa.Top + offsetArriba;

            // Evitar que se salga de límites
            x = Math.Max(x, wa.Left);
            y = Math.Max(y, wa.Top);

            this.Location = new Point(x, y);
        }

        // Crear el panel lateral y el botón "HOME"
        private void GenerarInterfaz()
        {
            panelLateral = new Panel
            {
                Width = 220,
                BackColor = ColorTranslator.FromHtml("#313439"),
                Dock = DockStyle.Left,
                Padding = new Padding(0)
            };
            this.Controls.Add(panelLateral);

            panelBotonesContainer = new Panel
            {
                Width = 200,
                Height = 0,
                Dock = DockStyle.Top,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = ColorTranslator.FromHtml("#313439"),
                Visible = botonesVisibles
            };
            panelLateral.Controls.Add(panelBotonesContainer);

            panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = ColorTranslator.FromHtml("#313439"),
                AutoScroll = false,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            panelBotonesContainer.Controls.Add(panelBotones);

            btnHome = new Button
            {
                Text = " HOME V1.0",
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorTranslator.FromHtml("#1A24B1"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 0, 5)
            };

            if (File.Exists("icons/home.png"))
            {
                btnHome.Image = new Bitmap("icons/home.png");
                btnHome.ImageAlign = ContentAlignment.MiddleLeft;
            }

            btnHome.Click += ToggleBotones;
            panelLateral.Controls.Add(btnHome);
        }

        // Generar el listado de botones en el menú
        private void GenerarBotones()
        {
            ButtonConfig[] buttonConfigs = new ButtonConfig[]
            {
                new ButtonConfig("CABALLOS", "icons/Caballos.png", (s, e) => AbrirPagina("https://retailhorse.aciertala.com/")),
                new ButtonConfig("ADMIN GOLDEN", "icons/lives.png", (s, e) => AbrirPagina("https://statsinfo.co/live/1/")),
                new ButtonConfig("SHEETS", "icons/scores.png", (s, e) => AbrirPagina("https://statshub.sportradar.com/novusoft/es/sport/1")),
                new ButtonConfig("CASHIER ONLINE", "icons/register.png", (s, e) => AbrirPagina("https://statsinfo.co/stats/1/c/26/")),
                new ButtonConfig("CHROME", "icons/browser.png", (s, e) => AbrirPagina("https://www.google.com/")),
                new ButtonConfig("WHATSAPP", "icons/wsp.png", (s, e) => AbrirPagina("https://365livesport.org/")),
                new ButtonConfig("CHAT SOPORTE", "icons/chat.png", (s, e) => AbrirPagina("https://www.registro.com/")),
                new ButtonConfig("TIPOS DE APUESTAS", "icons/bets.png", (s, e) => AbrirTiposApuestas()),
                new ButtonConfig("UTILITARIOS", "icons/utilitarios.png", (s, e) => RestartAciertala()),
                new ButtonConfig("CONEXIÓN REMOTA", "icons/remote.png", (s, e) => AbrirPagina("https://www.google.com/")),
                new ButtonConfig("ACTUALIZAR", "icons/update.png", (s, e) => OpenApagarReiniciarForm()),
                new ButtonConfig("APAGAR / REINICIAR", "icons/power.png", (s, e) => OpenApagarReiniciarForm())
            };

            foreach (var config in buttonConfigs)
            {
                Button btn = new Button
                {
                    Text = " " + config.Texto,
                    Width = panelLateral.Width - 40,
                    Height = 50,
                    BackColor = ColorTranslator.FromHtml("#313439"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(20, 0, 0, 0),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(0, 0, 0, 5),
                    FlatAppearance = { BorderSize = 2, BorderColor = Color.Blue }
                };

                if (File.Exists(config.Icono))
                {
                    btn.Image = new Bitmap(config.Icono);
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                }

                btn.Click += config.EventoClick;
                panelBotones.Controls.Add(btn);
            }
        }

        // Alterna el panel de botones visible o no
        private void ToggleBotones(object sender, EventArgs e)
        {
            botonesVisibles = !botonesVisibles;
            panelBotonesContainer.Visible = botonesVisibles;

            if (botonesVisibles)
            {
                panelBotonesContainer.Height = alturaBotones;
                this.Height += alturaBotones;
            }
            else
            {
                panelBotonesContainer.Height = 0;
                this.Height -= alturaBotones;
            }
        }

        // Calcula la altura total de los botones para expandir o colapsar
        private void CalcularAlturaBotones()
        {
            alturaBotones = 0;
            foreach (Control control in panelBotones.Controls)
            {
                alturaBotones += control.Height + control.Margin.Bottom;
            }
        }

        // MÉTODOS COMPLEMENTARIOS:
        private void AbrirPagina(string url)
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }

        private void AbrirTiposApuestas()
        {
            MessageBox.Show("Abriendo Tipos de Apuestas...");
        }

        private void RestartAciertala()
        {
            MessageBox.Show("Reiniciando Aciértala...");
        }

        private void OpenApagarReiniciarForm()
        {
            MessageBox.Show("Apagar/Reiniciar...");
        }

        // Hotkeys Globales: Lógica
        private void ReiniciarAciertalaGlobal()
        {
            // Versión global de reiniciar aciertala
            string aciertalaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aciertala", "Aciertala.exe");
            if (File.Exists(aciertalaPath))
            {
                try
                {
                    foreach (var proceso in Process.GetProcessesByName("Aciertala"))
                    {
                        proceso.Kill();
                    }

                    System.Threading.Thread.Sleep(1000);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = aciertalaPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reiniciar Aciértala:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No se encontró Aciértala.exe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EjecutarProgramaGlobal(string ruta)
        {
            if (File.Exists(ruta))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ruta,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"No se encontró el archivo:\n{ruta}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
