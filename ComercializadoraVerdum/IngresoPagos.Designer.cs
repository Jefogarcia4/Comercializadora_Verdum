namespace ComercializadoraVerdum
{
    partial class IngresoPagos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grbIngresoPagos = new System.Windows.Forms.GroupBox();
            this.btnIngresoPagos = new System.Windows.Forms.Button();
            this.txttransferencia = new System.Windows.Forms.TextBox();
            this.lblTransferencia = new System.Windows.Forms.Label();
            this.lblefectivo = new System.Windows.Forms.Label();
            this.txtefectivo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDeuda = new System.Windows.Forms.Label();
            this.lblDevuelve = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.grbIngresoPagos.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbIngresoPagos
            // 
            this.grbIngresoPagos.Controls.Add(this.btnIngresoPagos);
            this.grbIngresoPagos.Controls.Add(this.txttransferencia);
            this.grbIngresoPagos.Controls.Add(this.lblTransferencia);
            this.grbIngresoPagos.Controls.Add(this.lblefectivo);
            this.grbIngresoPagos.Controls.Add(this.txtefectivo);
            this.grbIngresoPagos.Location = new System.Drawing.Point(12, 89);
            this.grbIngresoPagos.Name = "grbIngresoPagos";
            this.grbIngresoPagos.Size = new System.Drawing.Size(257, 105);
            this.grbIngresoPagos.TabIndex = 0;
            this.grbIngresoPagos.TabStop = false;
            // 
            // btnIngresoPagos
            // 
            this.btnIngresoPagos.Enabled = false;
            this.btnIngresoPagos.Location = new System.Drawing.Point(6, 62);
            this.btnIngresoPagos.Name = "btnIngresoPagos";
            this.btnIngresoPagos.Size = new System.Drawing.Size(241, 27);
            this.btnIngresoPagos.TabIndex = 4;
            this.btnIngresoPagos.Text = "Guardar";
            this.btnIngresoPagos.UseVisualStyleBackColor = true;
            this.btnIngresoPagos.Click += new System.EventHandler(this.btnIngresoPagos_Click);
            // 
            // txttransferencia
            // 
            this.txttransferencia.Location = new System.Drawing.Point(139, 36);
            this.txttransferencia.Multiline = true;
            this.txttransferencia.Name = "txttransferencia";
            this.txttransferencia.Size = new System.Drawing.Size(108, 20);
            this.txttransferencia.TabIndex = 3;
            this.txttransferencia.TextChanged += new System.EventHandler(this.txttransferencia_TextChanged);
            // 
            // lblTransferencia
            // 
            this.lblTransferencia.AutoSize = true;
            this.lblTransferencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransferencia.Location = new System.Drawing.Point(136, 13);
            this.lblTransferencia.Name = "lblTransferencia";
            this.lblTransferencia.Size = new System.Drawing.Size(102, 18);
            this.lblTransferencia.TabIndex = 2;
            this.lblTransferencia.Text = "Transferencia:";
            // 
            // lblefectivo
            // 
            this.lblefectivo.AutoSize = true;
            this.lblefectivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblefectivo.Location = new System.Drawing.Point(7, 13);
            this.lblefectivo.Name = "lblefectivo";
            this.lblefectivo.Size = new System.Drawing.Size(65, 18);
            this.lblefectivo.TabIndex = 1;
            this.lblefectivo.Text = "Efectivo:";
            // 
            // txtefectivo
            // 
            this.txtefectivo.Location = new System.Drawing.Point(9, 36);
            this.txtefectivo.Multiline = true;
            this.txtefectivo.Name = "txtefectivo";
            this.txtefectivo.Size = new System.Drawing.Size(108, 20);
            this.txtefectivo.TabIndex = 0;
            this.txtefectivo.TextChanged += new System.EventHandler(this.txtefectivo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Valor Deuda:";
            // 
            // lblDeuda
            // 
            this.lblDeuda.AutoSize = true;
            this.lblDeuda.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeuda.Location = new System.Drawing.Point(111, 18);
            this.lblDeuda.Name = "lblDeuda";
            this.lblDeuda.Size = new System.Drawing.Size(0, 18);
            this.lblDeuda.TabIndex = 3;
            // 
            // lblDevuelve
            // 
            this.lblDevuelve.AutoSize = true;
            this.lblDevuelve.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDevuelve.Location = new System.Drawing.Point(129, 46);
            this.lblDevuelve.Name = "lblDevuelve";
            this.lblDevuelve.Size = new System.Drawing.Size(16, 18);
            this.lblDevuelve.TabIndex = 5;
            this.lblDevuelve.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Valor Devuelve:";
            // 
            // IngresoPagos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 200);
            this.Controls.Add(this.lblDevuelve);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblDeuda);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grbIngresoPagos);
            this.Name = "IngresoPagos";
            this.Text = "Saldar Deuda";
            this.Load += new System.EventHandler(this.IngresoPagos_Load);
            this.grbIngresoPagos.ResumeLayout(false);
            this.grbIngresoPagos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbIngresoPagos;
        private System.Windows.Forms.Label lblefectivo;
        private System.Windows.Forms.TextBox txtefectivo;
        private System.Windows.Forms.Button btnIngresoPagos;
        private System.Windows.Forms.TextBox txttransferencia;
        private System.Windows.Forms.Label lblTransferencia;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDeuda;
        private System.Windows.Forms.Label lblDevuelve;
        private System.Windows.Forms.Label label3;
    }
}