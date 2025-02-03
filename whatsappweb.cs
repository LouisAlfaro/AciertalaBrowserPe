using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WebviewAlberto
{
    public partial class WhatsappWeb : Form
    {
        public WhatsappWeb()
        {
            InitializeComponent();
        }

        private async void WhatsappWeb_Load(object sender, EventArgs e)
        {
            try
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aciertalaapp");
                var envOptions = new CoreWebView2EnvironmentOptions();
                var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, envOptions);

                await browser.EnsureCoreWebView2Async(environment);

                

                string url = "https://web.whatsapp.com/";
                if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    browser.CoreWebView2.Navigate(url);
                }
                else
                {
                    MessageBox.Show("URL no válida o vacía.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            
            var currentScreen = Screen.FromPoint(Cursor.Position);
            int screenWidth = currentScreen.Bounds.Width;
            int fixedHeight = 1000;

            
            this.ClientSize = new Size(screenWidth, fixedHeight);
            this.Location = new Point(currentScreen.Bounds.X, currentScreen.Bounds.Y + 80); 
        }
    }
}
