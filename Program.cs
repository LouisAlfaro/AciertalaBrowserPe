using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WebviewAlberto
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            AppSettings settings = LeerDatos();

            if (settings != null)
            {
                switch (settings.Modo)
                {
                    case "Terminal":
                        Application.Run(new PanelInicio());
                       
                        break;

                    case "Cajero":
                        Application.Run(new PanelInicio());
                        
                        break;

                    default:
                        Application.Run(new PanelInicio());
                        break;
                }
            }
            else
            {
                Application.Run(new PanelInicio()); 
            }
        }

        private static AppSettings LeerDatos()
        {
            try
            {
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AciertalaApp");
                string filePath = Path.Combine(appDataPath, "config.json");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<AppSettings>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
    }

    public class AppSettings
    {
        public string UrlRegistro { get; set; }
        public string LinkQR { get; set; }
        public string Modo { get; set; }
    }
}
