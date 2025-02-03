partial class PanelInicio
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Button btnAceptar;
    private System.Windows.Forms.Button btnCancelar;
    private System.Windows.Forms.TextBox txtUrlRegistro;
    private System.Windows.Forms.TextBox txtLinkQR;
    private System.Windows.Forms.ComboBox comboModo;
    private System.Windows.Forms.Label lblUrlRegistro;
    private System.Windows.Forms.Label lblLinkQR;
    private System.Windows.Forms.Label lblModo;

    private void InitializeComponent()
    {
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.txtUrlRegistro = new System.Windows.Forms.TextBox();
            this.txtLinkQR = new System.Windows.Forms.TextBox();
            this.comboModo = new System.Windows.Forms.ComboBox();
            this.lblUrlRegistro = new System.Windows.Forms.Label();
            this.lblLinkQR = new System.Windows.Forms.Label();
            this.lblModo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.Lime;
            this.btnAceptar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAceptar.Location = new System.Drawing.Point(67, 255);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 30);
            this.btnAceptar.TabIndex = 0;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            this.btnAceptar.Click += new System.EventHandler(this.BtnAceptar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.Red;
            this.btnCancelar.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancelar.Location = new System.Drawing.Point(261, 255);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 30);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // txtUrlRegistro
            // 
            this.txtUrlRegistro.Location = new System.Drawing.Point(101, 29);
            this.txtUrlRegistro.Name = "txtUrlRegistro";
            this.txtUrlRegistro.Size = new System.Drawing.Size(200, 20);
            this.txtUrlRegistro.TabIndex = 2;
            this.txtUrlRegistro.Text = "Ingresar URL de registro";
            this.txtUrlRegistro.Enter += new System.EventHandler(this.Txt_Enter);
            this.txtUrlRegistro.Leave += new System.EventHandler(this.Txt_Leave);
            // 
            // txtLinkQR
            // 
            this.txtLinkQR.Location = new System.Drawing.Point(101, 69);
            this.txtLinkQR.Name = "txtLinkQR";
            this.txtLinkQR.Size = new System.Drawing.Size(200, 20);
            this.txtLinkQR.TabIndex = 3;
            this.txtLinkQR.Text = "Ingresar Link de QR";
            this.txtLinkQR.Enter += new System.EventHandler(this.Txt_Enter);
            this.txtLinkQR.Leave += new System.EventHandler(this.Txt_Leave);
            // 
            // comboModo
            // 
            this.comboModo.Items.AddRange(new object[] {
            "Terminal",
            "Cajero"});
            this.comboModo.Location = new System.Drawing.Point(101, 109);
            this.comboModo.Name = "comboModo";
            this.comboModo.Size = new System.Drawing.Size(200, 21);
            this.comboModo.TabIndex = 4;
            // 
            // lblUrlRegistro
            // 
            this.lblUrlRegistro.Location = new System.Drawing.Point(101, 9);
            this.lblUrlRegistro.Name = "lblUrlRegistro";
            this.lblUrlRegistro.Size = new System.Drawing.Size(150, 20);
            this.lblUrlRegistro.TabIndex = 5;
            this.lblUrlRegistro.Text = "URL de Registro:";
            // 
            // lblLinkQR
            // 
            this.lblLinkQR.Location = new System.Drawing.Point(101, 49);
            this.lblLinkQR.Name = "lblLinkQR";
            this.lblLinkQR.Size = new System.Drawing.Size(150, 20);
            this.lblLinkQR.TabIndex = 6;
            this.lblLinkQR.Text = "Link de QR:";
            // 
            // lblModo
            // 
            this.lblModo.Location = new System.Drawing.Point(101, 89);
            this.lblModo.Name = "lblModo";
            this.lblModo.Size = new System.Drawing.Size(150, 20);
            this.lblModo.TabIndex = 7;
            this.lblModo.Text = "Modo:";
            // 
            // PanelInicio
            // 
            this.ClientSize = new System.Drawing.Size(431, 341);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.txtUrlRegistro);
            this.Controls.Add(this.txtLinkQR);
            this.Controls.Add(this.comboModo);
            this.Controls.Add(this.lblUrlRegistro);
            this.Controls.Add(this.lblLinkQR);
            this.Controls.Add(this.lblModo);
            this.Name = "PanelInicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Aciértala";
            this.Load += new System.EventHandler(this.PanelInicio_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
}
