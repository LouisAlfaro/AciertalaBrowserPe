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
    public partial class Form1 : Form
    {
        // ========== HOTKEYS GLOBALES ==========
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_NONE = 0x0000;  // Sin modificadores

        // ========== CAMPOS DEL FORMULARIO ==========
        private Panel panelLateral;
        private Panel panelBotonesContainer;
        private FlowLayoutPanel panelBotones;
        private Button btnHome;
        private CheckBox chkMostrarAlgo;
        private Button btnJuegosVirtuales;
       

        private bool botonesVisibles = false;
        private int alturaBotones = 0;

        public Form1()
        {
            InitializeComponent();

            // Configuración del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.Size = new Size(182, 60);
            this.DoubleBuffered = true;
            this.MouseDown += Form1_MouseDown;
            this.KeyPreview = true;

            // Generar interfaz y botones
            GenerarInterfaz();
            GenerarBotones();
            CalcularAlturaBotones();
            PosicionarArribaDerecha();

            // Registrar HotKeys globales
            RegistrarHotKeys();

            // Eventos del sistema (cambio de resolución, etc.)
            SystemEvents.DisplaySettingsChanged += (s, e) => PosicionarArribaDerecha();
            SystemEvents.UserPreferenceChanged += (s, e) => PosicionarArribaDerecha();
        }

        // ========== REGISTRAR HOTKEYS ==========
        private void RegistrarHotKeys()
        {
            // F5 → ID 1
            RegisterHotKey(this.Handle, 1, MOD_NONE, (uint)Keys.F5);
            // F6 → ID 2
            RegisterHotKey(this.Handle, 2, MOD_NONE, (uint)Keys.F6);
            // F11 → ID 3
            RegisterHotKey(this.Handle, 3, MOD_NONE, (uint)Keys.F11);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Liberar las hotkeys al cerrar
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
        }

        // ========== PROCESAR MENSAJES (WM_HOTKEY) ==========
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

        // ========== LÓGICA DE REINICIO / EJECUCIÓN ==========
        private void ReiniciarAciertalaGlobal()
        {
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

        // ========== POSICIONAR VENTANA EN LA PARTE SUPERIOR DERECHA ==========
        private void PosicionarArribaDerecha()
        {
            Rectangle wa = Screen.PrimaryScreen.WorkingArea;
            int screenWidth = wa.Width;

            int offsetDerecha = 360;
            int offsetArriba = 10;

            if (screenWidth < 1280)
            {
                offsetDerecha = 300;
            }
            if (screenWidth <= 1440)
            {
                offsetDerecha = 280;
            }

            int x = wa.Right - this.Width - offsetDerecha;
            int y = wa.Top + offsetArriba;

            x = Math.Max(x, wa.Left);
            y = Math.Max(y, wa.Top);

            this.Location = new Point(x, y);
        }

        // ========== PERMITIR ARRASTRAR LA VENTANA SIN BORDE ==========
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        // ========== GENERAR INTERFAZ (Panel Lateral, CheckBox, etc.) ==========
        private void GenerarInterfaz()
        {
            // Panel Lateral
            panelLateral = new Panel
            {
                Width = 220,
                BackColor = ColorTranslator.FromHtml("#313439"),
                Dock = DockStyle.Left,
                Padding = new Padding(0)
            };
            this.Controls.Add(panelLateral);

            // Panel que contendrá los botones
            panelBotonesContainer = new Panel
            {
                Width = 200,
                Height = 0,
                Dock = DockStyle.Top,
                Padding = new Padding(0),
                Margin = new Padding(0),
                BackColor = ColorTranslator.FromHtml("#313439"),
                Visible = false
            };
            panelLateral.Controls.Add(panelBotonesContainer);

            // FlowLayoutPanel para los botones
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

            // Botón HOME (para mostrar/ocultar botones)
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

            // EJEMPLO de CheckBox en Form1 (opcional):
            chkMostrarAlgo = new CheckBox
            {
                Text = "Mostrar algo",
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#313439"),
                AutoSize = true,
                Location = new Point(10, 70)
            };
            chkMostrarAlgo.CheckedChanged += (s, e) =>
            {
                if (chkMostrarAlgo.Checked)
                    MessageBox.Show("CheckBox ACTIVADO en Form1.");
                else
                    MessageBox.Show("CheckBox DESACTIVADO en Form1.");
            };
            panelLateral.Controls.Add(chkMostrarAlgo);
        }

        // ========== GENERAR BOTONES (Terminal Login, Caballos, etc.) ==========
        private void GenerarBotones()
        {
            ButtonConfig[] buttonConfigs = new ButtonConfig[]
            {
                new ButtonConfig("TERMINAL LOGIN", "icons/WEB.png", (s, e) => AbrirTerminalLogin()),
                new ButtonConfig("CABALLOS", "icons/Caballos.png", (s, e) => AbrirPagina("https://retailhorse.aciertala.com/")),
                new ButtonConfig("JUEGOS VIRTUALES", "icons/Caballos.png", (s, e) => AbrirPagina("https://retailhorse.aciertala.com/")),
                new ButtonConfig("RESULTADO EN VIVO", "icons/lives.png", (s, e) => AbrirPagina("https://statsinfo.co/live/1/")),
                new ButtonConfig("MARCADORES EN VIVO", "icons/scores.png", (s, e) => AbrirPagina("https://statshub.sportradar.com/novusoft/es/sport/1")),
                new ButtonConfig("ESTADISTICA", "icons/stats.png", (s, e) => AbrirPagina("https://statsinfo.co/stats/1/c/26/")),
                new ButtonConfig("TRANSMISIÓN", "icons/stream.png", (s, e) => AbrirPagina("https://365livesport.org/")),
                new ButtonConfig("CHROME", "icons/browser.png", (s, e) => AbrirPagina("https://www.google.com/")),
                new ButtonConfig("REGISTRO", "icons/register.png", (s, e) => AbrirPagina("https://www.registro.com/")),
                new ButtonConfig("REGISTRO QR", "icons/register.png", (s, e) => AbrirPagina("https://www.configuracion.com/")),
                new ButtonConfig("TIPOS DE APUESTAS", "icons/bets.png", (s, e) => AbrirPagina("https://www.configuracion.com/")),
                new ButtonConfig("ACTUALIZAR", "icons/update.png", (s, e) => AbrirPagina("https://www.configuracion.com/")),
                new ButtonConfig("CONEXIÓN REMOTA", "icons/remote.png", (s, e) => AbrirPagina("https://www.google.com/")),
                new ButtonConfig("APAGAR / REINICIAR", "icons/power.png", (s, e) => AbrirPagina("https://www.configuracion.com/"))
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

                if (config.Texto == "JUEGOS VIRTUALES")
                {
                    btnJuegosVirtuales = btn;
                }

                panelBotones.Controls.Add(btn);
            }
        }

        public void MostrarJuegosVirtuales(bool mostrar)
        {
            if (btnJuegosVirtuales != null)
            {
                btnJuegosVirtuales.Visible = mostrar;
            }
        }

        // ========== MOSTRAR / OCULTAR BOTONES ==========
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

        // ========== LÓGICA ADICIONAL ==========
        private void AbrirTerminalLogin()
        {
            TerminalLogin form = new TerminalLogin();
            form.Show();
        }

        private void AbrirPagina(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void CalcularAlturaBotones()
        {
            alturaBotones = 0;
            foreach (Control control in panelBotones.Controls)
            {
                alturaBotones += control.Height + control.Margin.Bottom;
            }
        }
    }
}
