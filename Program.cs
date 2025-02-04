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
            Form1 form1 = new Form1(); 


            if (settings != null)
            {
                switch (settings.Modo)
                {
                    case "Terminal":
                        Application.Run(new PanelInicio(form1));
                       
                        break;

                    case "Cajero":
                        Application.Run(new PanelInicio(form1));
                        break;

                    default:
                        Application.Run(new PanelInicio(form1));
                        break;
                }
            }
            else
            {
                Application.Run(new PanelInicio(form1)); 
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
        public bool MostrarJuegos { get; set; } = true;
    }
}
