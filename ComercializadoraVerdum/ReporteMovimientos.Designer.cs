namespace ComercializadoraVerdum
{
    partial class ReporteMovimientos
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
            this.dtpFechaMovimiento = new System.Windows.Forms.DateTimePicker();
            this.txtClienteMovimiento = new System.Windows.Forms.TextBox();
            this.btnFiltrarMovimientos = new System.Windows.Forms.Button();
            this.btnRefrescarMovimientos = new System.Windows.Forms.Button();
            this.btnVolverAnt = new System.Windows.Forms.Button();
            this.dgvReporteMovimientos = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporteMovimientos)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpFechaMovimiento
            // 
            this.dtpFechaMovimiento.Location = new System.Drawing.Point(12, 8);
            this.dtpFechaMovimiento.Name = "dtpFechaMovimiento";
            this.dtpFechaMovimiento.Size = new System.Drawing.Size(200, 20);
            this.dtpFechaMovimiento.TabIndex = 0;
            // 
            // txtClienteMovimiento
            // 
            this.txtClienteMovimiento.Location = new System.Drawing.Point(215, 6);
            this.txtClienteMovimiento.Multiline = true;
            this.txtClienteMovimiento.Name = "txtClienteMovimiento";
            this.txtClienteMovimiento.Size = new System.Drawing.Size(142, 23);
            this.txtClienteMovimiento.TabIndex = 1;
            // 
            // btnFiltrarMovimientos
            // 
            this.btnFiltrarMovimientos.BackColor = System.Drawing.SystemColors.Control;
            this.btnFiltrarMovimientos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFiltrarMovimientos.Location = new System.Drawing.Point(359, 5);
            this.btnFiltrarMovimientos.Name = "btnFiltrarMovimientos";
            this.btnFiltrarMovimientos.Size = new System.Drawing.Size(64, 25);
            this.btnFiltrarMovimientos.TabIndex = 2;
            this.btnFiltrarMovimientos.Text = "Filtrar";
            this.btnFiltrarMovimientos.UseVisualStyleBackColor = false;
            this.btnFiltrarMovimientos.Click += new System.EventHandler(this.btnFiltrarMovimientos_Click);
            // 
            // btnRefrescarMovimientos
            // 
            this.btnRefrescarMovimientos.Location = new System.Drawing.Point(424, 5);
            this.btnRefrescarMovimientos.Name = "btnRefrescarMovimientos";
            this.btnRefrescarMovimientos.Size = new System.Drawing.Size(24, 25);
            this.btnRefrescarMovimientos.TabIndex = 7;
            this.btnRefrescarMovimientos.UseVisualStyleBackColor = true;
            this.btnRefrescarMovimientos.Click += new System.EventHandler(this.btnRefrescarMovimientos_Click);
            // 
            // btnVolverAnt
            // 
            this.btnVolverAnt.Location = new System.Drawing.Point(450, 5);
            this.btnVolverAnt.Name = "btnVolverAnt";
            this.btnVolverAnt.Size = new System.Drawing.Size(24, 25);
            this.btnVolverAnt.TabIndex = 8;
            this.btnVolverAnt.UseVisualStyleBackColor = true;
            this.btnVolverAnt.Click += new System.EventHandler(this.btnVolverAnt_Click);
            // 
            // dgvReporteMovimientos
            // 
            this.dgvReporteMovimientos.AllowUserToAddRows = false;
            this.dgvReporteMovimientos.AllowUserToDeleteRows = false;
            this.dgvReporteMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReporteMovimientos.Location = new System.Drawing.Point(12, 32);
            this.dgvReporteMovimientos.Name = "dgvReporteMovimientos";
            this.dgvReporteMovimientos.ReadOnly = true;
            this.dgvReporteMovimientos.Size = new System.Drawing.Size(461, 404);
            this.dgvReporteMovimientos.TabIndex = 9;
            // 
            // ReporteMovimientos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 442);
            this.Controls.Add(this.dgvReporteMovimientos);
            this.Controls.Add(this.btnVolverAnt);
            this.Controls.Add(this.btnRefrescarMovimientos);
            this.Controls.Add(this.btnFiltrarMovimientos);
            this.Controls.Add(this.txtClienteMovimiento);
            this.Controls.Add(this.dtpFechaMovimiento);
            this.Name = "ReporteMovimientos";
            this.Text = "Informe de Movimientos";
            this.Load += new System.EventHandler(this.ReporteMovimientos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporteMovimientos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpFechaMovimiento;
        private System.Windows.Forms.TextBox txtClienteMovimiento;
        private System.Windows.Forms.Button btnFiltrarMovimientos;
        private System.Windows.Forms.Button btnRefrescarMovimientos;
        private System.Windows.Forms.Button btnVolverAnt;
        private System.Windows.Forms.DataGridView dgvReporteMovimientos;
    }
}