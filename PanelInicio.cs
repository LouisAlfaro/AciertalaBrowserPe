using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebviewAlberto;

public partial class PanelInicio : Form
{
    private AppSettings settings;
    private Label lblEstado;
    private ProgressBar progressBarDescarga;

    // Para cancelar descargas en curso.
    private CancellationTokenSource cancelTokenSource;

    public PanelInicio()
    {
        InitializeComponent();
        ConfigurarInterfaz();
    }

    /// <summary>
    /// Ajusta la interfaz: Tamaño del Form, Label para estado y ProgressBar para descargas.
    /// </summary>
    private void ConfigurarInterfaz()
    {
        this.Width = 400;
        this.Height = 350;

        lblEstado = new Label
        {
            Text = "Esperando selección...",
            AutoSize = false,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            Width = 300,
            Height = 30,
            Location = new System.Drawing.Point((this.Width - 300) / 2, 130)
        };
        this.Controls.Add(lblEstado);

        progressBarDescarga = new ProgressBar
        {
            Location = new System.Drawing.Point((this.Width - 320) / 2, 170),
            Width = 320,
            Minimum = 0,
            Maximum = 100,
            Value = 0
        };
        this.Controls.Add(progressBarDescarga);
    }

    /// <summary>
    /// Evento Load: primero descarga (si no existe) y descomprime BotonesAciertala.rar, 
    /// luego lee la configuración y ejecuta el modo Terminal o Cajero si corresponde.
    /// </summary>
    private async void PanelInicio_Load(object sender, EventArgs e)
    {
        // Descarga y descomprime BotonesAciertala.rar (solo si no existe).
        await DescargarYDescomprimirBotonesAciertala();

        // Carga la configuración guardada, si existe.
        settings = LeerDatos();

        if (settings != null && !string.IsNullOrEmpty(settings.Modo))
        {
            string modo = settings.Modo.Trim().ToLower();
            if (modo == "terminal")
            {
                EjecutarAciertala();
                return;
            }
            else if (modo == "cajero")
            {
                EjecutarAplicacionCajero();
                return;
            }
        }

        // Si no hay configuración, se muestra el formulario para que el usuario seleccione el modo.
        this.Show();
    }

    /// <summary>
    /// Descarga y descomprime BotonesAciertala.rar en C:\BotonesAciertala
    /// </summary>
    private async Task DescargarYDescomprimirBotonesAciertala()
    {
        string carpetaDestino = @"C:\BotonesAciertala";
        string archivoDestino = Path.Combine(carpetaDestino, "BotonesAciertala.rar");

        if (!Directory.Exists(carpetaDestino))
        {
            Directory.CreateDirectory(carpetaDestino);
        }

        // Si no existe el archivo RAR, se descarga.
        if (!File.Exists(archivoDestino))
        {
            string urlDescarga = "https://universalrace.net/download/BotonesAciertala.rar";

            cancelTokenSource = new CancellationTokenSource();
            var token = cancelTokenSource.Token;

            try
            {
                using (WebClient client = new WebClient())
                {
                    // Evento para actualizar progreso y detectar velocidad lenta.
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            progressBarDescarga.Value = e.ProgressPercentage;
                            lblEstado.Text = $"Descargando BotonesAciertala.rar... {e.ProgressPercentage}%";

                            // Detectar si la velocidad es menor a ~100 KB/s
                            if (e.BytesReceived > 0 && e.TotalBytesToReceive > 0 && e.ProgressPercentage > 0)
                            {
                                double kbReceived = e.BytesReceived / 1024.0;
                                // Se asume que e.ProgressPercentage va de 1 a 100
                                double velocidadKBps = kbReceived / e.ProgressPercentage;
                                if (velocidadKBps < 100)
                                {
                                    lblEstado.Text = "Internet lento. Verifique su conexión.";
                                }
                            }
                        }));
                    };

                    // Inicio de la descarga
                    this.Invoke((Action)(() =>
                    {
                        lblEstado.Text = "Iniciando descarga de BotonesAciertala.rar...";
                        progressBarDescarga.Value = 0;
                    }));

                    // Descargar el archivo
                    await client.DownloadFileTaskAsync(new Uri(urlDescarga), archivoDestino);

                    // Al finalizar la descarga
                    this.Invoke((Action)(() =>
                    {
                        lblEstado.Text = "Descarga completada. Descomprimiendo...";
                        progressBarDescarga.Value = 100;
                    }));
                }
            }
            catch (OperationCanceledException)
            {
                // Si se canceló la descarga, se borran los archivos parciales
                BorrarDescarga(archivoDestino);
                this.Invoke((Action)(() =>
                {
                    lblEstado.Text = "Descarga cancelada.";
                }));
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al descargar BotonesAciertala.rar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Verificar que el archivo exista realmente
        if (!File.Exists(archivoDestino))
        {
            MessageBox.Show("El archivo BotonesAciertala.rar no se encuentra después de la descarga.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Descomprimir RAR (si lo deseas realmente). 
        // Actualmente tu código sólo descarga BotonesAciertala.rar y no la descomprime con UnRAR.
        // Si quieres usar unrar, descomenta y ajusta la ruta:
        
        try
        {
            DescomprimirRAR(archivoDestino, carpetaDestino);
            this.Invoke((Action)(() =>
            {
                lblEstado.Text = "Descompresión completada.";
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al descomprimir BotonesAciertala.rar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
    }

    /// <summary>
    /// Método para descomprimir un RAR usando UnRAR.exe
    /// </summary>
    private void DescomprimirRAR(string archivoRar, string carpetaDestino)
    {
        if (!File.Exists(archivoRar))
        {
            throw new Exception("El archivo RAR no existe, no se puede descomprimir.");
        }

        // Ajusta la ruta a UnRAR.exe según tu instalación de WinRAR
        string unrarPath = @"C:\Program Files\WinRAR\UnRAR.exe";

        if (!File.Exists(unrarPath))
        {
            throw new Exception("No se encontró UnRAR.exe. Asegúrate de tener WinRAR instalado.");
        }

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = unrarPath,
                Arguments = $"x \"{archivoRar}\" \"{carpetaDestino}\\\" -y",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };

            using (Process proc = Process.Start(psi))
            {
                proc.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al descomprimir el archivo RAR: " + ex.Message);
        }
    }

    /// <summary>
    /// Cancela la descarga y borra el archivo parcial si existe.
    /// </summary>
    private void BorrarDescarga(string archivo)
    {
        try
        {
            if (File.Exists(archivo))
            {
                File.Delete(archivo);
            }

            string carpeta = Path.GetDirectoryName(archivo);
            if (Directory.Exists(carpeta))
            {
                // Si quieres borrar la carpeta entera
                // Directory.Delete(carpeta, true);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar archivos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Botón Cancelar: cierra la ventana y, si hay una descarga en curso, la cancela.
    /// </summary>
    private void BtnCancelar_Click(object sender, EventArgs e)
    {
        if (cancelTokenSource != null)
        {
            cancelTokenSource.Cancel();
        }
        this.Close();
    }

    /// <summary>
    /// Descarga y descomprime Aciertala-setup-2.7.2.zip, luego ejecuta la aplicación si no existe en la carpeta local.
    /// </summary>
    private async void EjecutarAciertala()
    {
        string appFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Aciertala");
        string downloadUrl = "https://releases.xpressgaming.net/tech.xpress.aciertala/win32/Aciertala-setup-2.7.2.zip";
        string zipFilePath = Path.Combine(Path.GetTempPath(), "Aciertala-setup-2.7.2.zip");
        string extractPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AciertalaSetup");

        this.Invoke((Action)(() => lblEstado.Text = "Verificando aplicación..."));

        // Comprueba si Aciertala.exe ya está descargado
        string appPath = Directory.Exists(appFolderPath)
                            ? Directory.GetFiles(appFolderPath, "*.exe", SearchOption.AllDirectories).FirstOrDefault()
                            : null;

        // Si no está, se descarga y descomprime
        if (string.IsNullOrEmpty(appPath) || !File.Exists(appPath))
        {
            this.Invoke((Action)(() => lblEstado.Text = "Descargando nueva versión..."));

            await DescargarYDescomprimir(downloadUrl, zipFilePath, extractPath);

            // Tras la descompresión, busca el .exe
            appPath = Directory.Exists(extractPath)
                        ? Directory.GetFiles(extractPath, "*.exe", SearchOption.AllDirectories).FirstOrDefault()
                        : null;

            if (string.IsNullOrEmpty(appPath) || !File.Exists(appPath))
            {
                this.Invoke((Action)(() => lblEstado.Text = "Error: No se encontró la aplicación."));
                return;
            }

            // Copia el EXE a la carpeta local
            try
            {
                if (!Directory.Exists(appFolderPath))
                    Directory.CreateDirectory(appFolderPath);

                string destinoAppPath = Path.Combine(appFolderPath, Path.GetFileName(appPath));
                File.Copy(appPath, destinoAppPath, true);
                appPath = destinoAppPath;
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() => lblEstado.Text = $"Error: {ex.Message}"));
                return;
            }
        }

        // Ejecutar la aplicación
        try
        {
            this.Invoke((Action)(() => lblEstado.Text = "Ejecutando aplicación..."));

            Process.Start(new ProcessStartInfo
            {
                FileName = appPath,
                UseShellExecute = true,
                Verb = "runas"
            });

            this.Invoke((Action)(() =>
            {
                lblEstado.Text = "Aplicación ejecutada con éxito.";
                this.Hide();
                Form formulario = new Form1();
                formulario.ShowDialog();
                this.Close();
            }));
        }
        catch (Exception ex)
        {
            this.Invoke((Action)(() => lblEstado.Text = $"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Método auxiliar para descargar y descomprimir Aciertala (ZIP)
    /// </summary>
    private async Task DescargarYDescomprimir(string url, string destinoZip, string destinoExtract)
    {
        try
        {
            // Para cancelar la descarga en caso necesario
            cancelTokenSource = new CancellationTokenSource();
            var token = cancelTokenSource.Token;

            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    this.Invoke((Action)(() =>
                    {
                        progressBarDescarga.Value = e.ProgressPercentage;
                        lblEstado.Text = $"Descargando... {e.ProgressPercentage}%";

                        // Detectar baja velocidad de internet
                        if (e.BytesReceived > 0 && e.TotalBytesToReceive > 0 && e.ProgressPercentage > 0)
                        {
                            double kbReceived = e.BytesReceived / 1024.0;
                            double velocidadKBps = kbReceived / e.ProgressPercentage;
                            if (velocidadKBps < 100)
                            {
                                lblEstado.Text = "Internet lento. Verifique su conexión.";
                            }
                        }
                    }));
                };

                this.Invoke((Action)(() =>
                {
                    lblEstado.Text = "Iniciando descarga...";
                    progressBarDescarga.Value = 0;
                }));

                await client.DownloadFileTaskAsync(new Uri(url), destinoZip);

                if (File.Exists(destinoZip))
                {
                    this.Invoke((Action)(() =>
                    {
                        lblEstado.Text = "Descarga completada. Descomprimiendo...";
                        progressBarDescarga.Value = 100;
                    }));

                    // Eliminar carpeta previa si existe
                    if (Directory.Exists(destinoExtract))
                    {
                        Directory.Delete(destinoExtract, true);
                    }
                    Directory.CreateDirectory(destinoExtract);

                    ZipFile.ExtractToDirectory(destinoZip, destinoExtract);

                    this.Invoke((Action)(() =>
                    {
                        lblEstado.Text = "Descompresión completada.";
                    }));
                }
                else
                {
                    this.Invoke((Action)(() => lblEstado.Text = "Error: Archivo ZIP no encontrado."));
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Borra el archivo parcial si se canceló
            BorrarDescarga(destinoZip);
            this.Invoke((Action)(() =>
            {
                lblEstado.Text = "Descarga cancelada.";
            }));
        }
        catch (Exception ex)
        {
            this.Invoke((Action)(() => lblEstado.Text = $"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Ejecuta el modo Cajero: busca "VBOX.appref-ms" en el Escritorio
    /// </summary>
    private void EjecutarAplicacionCajero()
    {
        string appFileName = "VBOX.appref-ms";
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string appPath = Path.Combine(desktopPath, appFileName);

        if (File.Exists(appPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = true
                });

                this.Invoke((Action)(() =>
                {
                    this.Hide();
                    Form formularioCajero = new Form2();
                    formularioCajero.ShowDialog();
                    this.Close();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar la aplicación de cajero: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Evento para TextBox: al entrar, si tiene texto por defecto, lo limpia
    /// </summary>
    private void Txt_Enter(object sender, EventArgs e)
    {
        if (sender is TextBox txt)
        {
            if (txt.Text == "Ingresar URL de registro" || txt.Text == "Ingresar Link de QR")
            {
                txt.Text = "";
                txt.ForeColor = System.Drawing.Color.Black;
            }
        }
    }

    /// <summary>
    /// Evento para TextBox: al salir, si está vacío, le pone el texto por defecto
    /// </summary>
    private void Txt_Leave(object sender, EventArgs e)
    {
        if (sender is TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                if (txt == txtUrlRegistro)
                {
                    txt.Text = "Ingresar URL de registro";
                }
                else if (txt == txtLinkQR)
                {
                    txt.Text = "Ingresar Link de QR";
                }
                txt.ForeColor = System.Drawing.Color.Gray;
            }
        }
    }

    /// <summary>
    /// Cuando el usuario completa el panel y da Aceptar, se guarda la configuración y se ejecuta según el modo
    /// </summary>
    private void BtnAceptar_Click(object sender, EventArgs e)
    {
        string urlRegistro = txtUrlRegistro.Text;
        string linkQR = txtLinkQR.Text;
        string modoSeleccionado = comboModo.SelectedItem?.ToString() ?? "";

        if (string.IsNullOrEmpty(urlRegistro) || string.IsNullOrEmpty(linkQR) ||
            urlRegistro == "Ingresar URL de registro" || linkQR == "Ingresar Link de QR" ||
            string.IsNullOrEmpty(modoSeleccionado))
        {
            MessageBox.Show("Por favor, complete todos los campos correctamente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        settings = new AppSettings
        {
            UrlRegistro = urlRegistro,
            LinkQR = linkQR,
            Modo = modoSeleccionado
        };
        GuardarDatos(settings);

        if (modoSeleccionado.Trim().ToLower() == "terminal")
        {
            EjecutarAciertala();
        }
        else if (modoSeleccionado.Trim().ToLower() == "cajero")
        {
            EjecutarAplicacionCajero();
        }
    }

    /// <summary>
    /// Guarda la configuración en JSON en LocalApplicationData\AciertalaApp\config.json
    /// </summary>
    private void GuardarDatos(AppSettings settings)
    {
        try
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AciertalaApp");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            string filePath = Path.Combine(appDataPath, "config.json");
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Lee la configuración desde config.json
    /// </summary>
    private AppSettings LeerDatos()
    {
        try
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AciertalaApp", "config.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<AppSettings>(json);
            }
        }
        catch { }
        return null;
    }

    /// <summary>
    /// Clase con la configuración del aplicativo
    /// </summary>
    public class AppSettings
    {
        public string UrlRegistro { get; set; }
        public string LinkQR { get; set; }
        public string Modo { get; set; }
    }
}
