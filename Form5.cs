using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WebviewAlberto
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private async void Form5_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            double modelWidth = 1585;
            double modelHeight = 790;
            double topOffset = 80;

            double scaleFactor = Math.Min(screenWidth / modelWidth, screenHeight / (modelHeight + topOffset));

            this.Left = 0;
            this.Top = (int)(topOffset * scaleFactor);
            this.Width = (int)(modelWidth * scaleFactor);
            this.Height = (int)(modelHeight * scaleFactor);
            this.TopMost = false;


            try
            {
                await browser.EnsureCoreWebView2Async();
                ConfigureWebView2PopupBlocking();

                browser.CoreWebView2.NavigationCompleted += OnNavigationCompleted;

                browser.CoreWebView2.Navigate("https://www.rojadirectvonline.com/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando WebView2: " + ex.Message);
            }
        }

        private void ConfigureWebView2PopupBlocking()
        {
            browser.CoreWebView2.NewWindowRequested += HandleNewWindowRequested;
        }

        private void HandleNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            try
            {
                e.Handled = true;
                Debug.WriteLine($"Ventana emergente bloqueada: {e.Uri}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error manejando ventana emergente: {ex.Message}");
            }
        }

        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                browser.CoreWebView2.ExecuteScriptAsync(@"
                
                document.querySelectorAll('header, footer').forEach(function(el) {
                    el.style.display = 'none';
                });

                
                var button = document.getElementById('btnIframe');
                if (button) {
                    button.style.display = 'none';
                }
            ");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inyectando script: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (browser.CoreWebView2.CanGoBack)
            {
                browser.CoreWebView2.GoBack();
            }
            else
            {
                MessageBox.Show("No hay historial para volver atrás.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            browser.CoreWebView2.Reload();
        }
    }

}
