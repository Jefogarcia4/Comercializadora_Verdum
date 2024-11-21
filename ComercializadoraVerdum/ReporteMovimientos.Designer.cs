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
            this.button1 = new System.Windows.Forms.Button();
            this.btnRefrescarMovimientos = new System.Windows.Forms.Button();
            this.btnVolverAnt = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.txtClienteMovimiento.Location = new System.Drawing.Point(218, 6);
            this.txtClienteMovimiento.Multiline = true;
            this.txtClienteMovimiento.Name = "txtClienteMovimiento";
            this.txtClienteMovimiento.Size = new System.Drawing.Size(233, 23);
            this.txtClienteMovimiento.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(455, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Filtrar";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnRefrescarMovimientos
            // 
            this.btnRefrescarMovimientos.Location = new System.Drawing.Point(533, 4);
            this.btnRefrescarMovimientos.Name = "btnRefrescarMovimientos";
            this.btnRefrescarMovimientos.Size = new System.Drawing.Size(24, 25);
            this.btnRefrescarMovimientos.TabIndex = 7;
            this.btnRefrescarMovimientos.UseVisualStyleBackColor = true;
            // 
            // btnVolverAnt
            // 
            this.btnVolverAnt.Location = new System.Drawing.Point(560, 4);
            this.btnVolverAnt.Name = "btnVolverAnt";
            this.btnVolverAnt.Size = new System.Drawing.Size(24, 25);
            this.btnVolverAnt.TabIndex = 8;
            this.btnVolverAnt.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(572, 404);
            this.dataGridView1.TabIndex = 9;
            // 
            // ReporteMovimientos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 442);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnVolverAnt);
            this.Controls.Add(this.btnRefrescarMovimientos);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtClienteMovimiento);
            this.Controls.Add(this.dtpFechaMovimiento);
            this.Name = "ReporteMovimientos";
            this.Text = "Informe de Movimientos";
            this.Load += new System.EventHandler(this.ReporteMovimientos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpFechaMovimiento;
        private System.Windows.Forms.TextBox txtClienteMovimiento;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnRefrescarMovimientos;
        private System.Windows.Forms.Button btnVolverAnt;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}