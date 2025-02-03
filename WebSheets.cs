using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WebviewAlberto
{
    public partial class WebSheets : Form
    {
        public WebSheets()
        {
            InitializeComponent();
        }

        private async void WebSheets_Load(object sender, EventArgs e)
        {
            try
            {
                
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "aciertalaapp" 
                );

            
                var environment = await CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: null,
                    userDataFolder: userDataFolder,
                    options: null  
                );

                
                await browser.EnsureCoreWebView2Async(environment);

                
                string url = "https://docs.google.com/spreadsheets/u/0/";

                
                if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    browser.CoreWebView2.Navigate(url);
                }
                else
                {
                    MessageBox.Show(
                        "URL no válida o vacía.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al inicializar WebView2: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
