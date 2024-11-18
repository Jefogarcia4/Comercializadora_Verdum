
namespace ComercializadoraVerdum
{
    partial class FrmHome
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblNombreCliente = new System.Windows.Forms.Label();
            this.txtCliente = new System.Windows.Forms.TextBox();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.lblnumerofactura = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTituloResumen = new System.Windows.Forms.Label();
            this.lblResumenVenta = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.txtAbona = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.lblAbona = new System.Windows.Forms.Label();
            this.buttonHistorial = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grbResumenDeVenta = new System.Windows.Forms.GroupBox();
            this.lblDevuelta = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdbTransferencia = new System.Windows.Forms.RadioButton();
            this.rdB_Efectivo = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTransferencia = new System.Windows.Forms.Label();
            this.txtAbonaTransferencia = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grbResumenDeVenta.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(11, 68);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(622, 161);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_DefaultValuesNeeded);
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            // 
            // lblNombreCliente
            // 
            this.lblNombreCliente.AutoSize = true;
            this.lblNombreCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreCliente.Location = new System.Drawing.Point(8, 40);
            this.lblNombreCliente.Name = "lblNombreCliente";
            this.lblNombreCliente.Size = new System.Drawing.Size(103, 16);
            this.lblNombreCliente.TabIndex = 9;
            this.lblNombreCliente.Text = "Nombre Cliente:";
            this.lblNombreCliente.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtCliente
            // 
            this.txtCliente.Location = new System.Drawing.Point(112, 34);
            this.txtCliente.Multiline = true;
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.Size = new System.Drawing.Size(443, 27);
            this.txtCliente.TabIndex = 10;
            this.txtCliente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCliente_KeyDown);
            this.txtCliente.Leave += new System.EventHandler(this.txtCliente_Leave);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.SystemColors.Control;
            this.btnLimpiar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Location = new System.Drawing.Point(558, 34);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(75, 27);
            this.btnLimpiar.TabIndex = 12;
            this.btnLimpiar.Text = "Limpiar ";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // lblnumerofactura
            // 
            this.lblnumerofactura.AutoSize = true;
            this.lblnumerofactura.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnumerofactura.Location = new System.Drawing.Point(7, 8);
            this.lblnumerofactura.Name = "lblnumerofactura";
            this.lblnumerofactura.Size = new System.Drawing.Size(167, 24);
            this.lblnumerofactura.TabIndex = 15;
            this.lblnumerofactura.Text = "Número Factura:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblnumerofactura);
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Controls.Add(this.btnLimpiar);
            this.groupBox1.Controls.Add(this.txtCliente);
            this.groupBox1.Controls.Add(this.lblNombreCliente);
            this.groupBox1.Location = new System.Drawing.Point(12, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(641, 234);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // lblTituloResumen
            // 
            this.lblTituloResumen.AutoSize = true;
            this.lblTituloResumen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloResumen.Location = new System.Drawing.Point(3, 2);
            this.lblTituloResumen.Name = "lblTituloResumen";
            this.lblTituloResumen.Size = new System.Drawing.Size(146, 15);
            this.lblTituloResumen.TabIndex = 17;
            this.lblTituloResumen.Text = "RESUMEN DE VENTA";
            // 
            // lblResumenVenta
            // 
            this.lblResumenVenta.AutoSize = true;
            this.lblResumenVenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResumenVenta.Location = new System.Drawing.Point(3, 23);
            this.lblResumenVenta.Name = "lblResumenVenta";
            this.lblResumenVenta.Size = new System.Drawing.Size(239, 15);
            this.lblResumenVenta.TabIndex = 16;
            this.lblResumenVenta.Text = "No se han agredado productos a la factura";
            // 
            // txtAbona
            // 
            this.txtAbona.Location = new System.Drawing.Point(315, 379);
            this.txtAbona.Name = "txtAbona";
            this.txtAbona.Size = new System.Drawing.Size(82, 20);
            this.txtAbona.TabIndex = 22;
            this.txtAbona.TextChanged += new System.EventHandler(this.txtAbona_TextChanged);
            this.txtAbona.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAbona_KeyPress);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.SaveButton.Enabled = false;
            this.SaveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(23, 403);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(309, 41);
            this.SaveButton.TabIndex = 17;
            this.SaveButton.Text = "Registrar Venta";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // lblAbona
            // 
            this.lblAbona.AutoSize = true;
            this.lblAbona.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbona.Location = new System.Drawing.Point(190, 376);
            this.lblAbona.Name = "lblAbona";
            this.lblAbona.Size = new System.Drawing.Size(128, 24);
            this.lblAbona.TabIndex = 21;
            this.lblAbona.Text = "Valor efectivo:";
            this.lblAbona.Click += new System.EventHandler(this.lblAbona_Click);
            // 
            // buttonHistorial
            // 
            this.buttonHistorial.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.buttonHistorial.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHistorial.Location = new System.Drawing.Point(338, 403);
            this.buttonHistorial.Name = "buttonHistorial";
            this.buttonHistorial.Size = new System.Drawing.Size(315, 42);
            this.buttonHistorial.TabIndex = 18;
            this.buttonHistorial.Text = "Historial";
            this.buttonHistorial.UseVisualStyleBackColor = false;
            this.buttonHistorial.Click += new System.EventHandler(this.buttonHistorial_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 379);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 24);
            this.label3.TabIndex = 19;
            this.label3.Text = "Total:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(211, 407);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 24);
            this.label4.TabIndex = 20;
            // 
            // grbResumenDeVenta
            // 
            this.grbResumenDeVenta.Controls.Add(this.lblDevuelta);
            this.grbResumenDeVenta.Controls.Add(this.label2);
            this.grbResumenDeVenta.Controls.Add(this.rdbTransferencia);
            this.grbResumenDeVenta.Controls.Add(this.rdB_Efectivo);
            this.grbResumenDeVenta.Controls.Add(this.label1);
            this.grbResumenDeVenta.Controls.Add(this.panel1);
            this.grbResumenDeVenta.Location = new System.Drawing.Point(12, 236);
            this.grbResumenDeVenta.Name = "grbResumenDeVenta";
            this.grbResumenDeVenta.Size = new System.Drawing.Size(641, 137);
            this.grbResumenDeVenta.TabIndex = 23;
            this.grbResumenDeVenta.TabStop = false;
            // 
            // lblDevuelta
            // 
            this.lblDevuelta.AutoSize = true;
            this.lblDevuelta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDevuelta.Location = new System.Drawing.Point(412, 81);
            this.lblDevuelta.Name = "lblDevuelta";
            this.lblDevuelta.Size = new System.Drawing.Size(16, 17);
            this.lblDevuelta.TabIndex = 24;
            this.lblDevuelta.Text = "0";
            this.lblDevuelta.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(348, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 23;
            this.label2.Text = "Devuelta:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // rdbTransferencia
            // 
            this.rdbTransferencia.AutoSize = true;
            this.rdbTransferencia.Location = new System.Drawing.Point(351, 52);
            this.rdbTransferencia.Name = "rdbTransferencia";
            this.rdbTransferencia.Size = new System.Drawing.Size(90, 17);
            this.rdbTransferencia.TabIndex = 22;
            this.rdbTransferencia.TabStop = true;
            this.rdbTransferencia.Text = "Transferencia";
            this.rdbTransferencia.UseVisualStyleBackColor = true;
            // 
            // rdB_Efectivo
            // 
            this.rdB_Efectivo.AutoSize = true;
            this.rdB_Efectivo.Checked = true;
            this.rdB_Efectivo.Location = new System.Drawing.Point(351, 29);
            this.rdB_Efectivo.Name = "rdB_Efectivo";
            this.rdB_Efectivo.Size = new System.Drawing.Size(64, 17);
            this.rdB_Efectivo.TabIndex = 21;
            this.rdB_Efectivo.TabStop = true;
            this.rdB_Efectivo.Text = "Efectivo";
            this.rdB_Efectivo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(348, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 15);
            this.label1.TabIndex = 20;
            this.label1.Text = "Tipo Pago:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.lblTituloResumen);
            this.panel1.Controls.Add(this.lblResumenVenta);
            this.panel1.Location = new System.Drawing.Point(6, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(336, 130);
            this.panel1.TabIndex = 18;
            // 
            // lblTransferencia
            // 
            this.lblTransferencia.AutoSize = true;
            this.lblTransferencia.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransferencia.Location = new System.Drawing.Point(404, 376);
            this.lblTransferencia.Name = "lblTransferencia";
            this.lblTransferencia.Size = new System.Drawing.Size(171, 24);
            this.lblTransferencia.TabIndex = 24;
            this.lblTransferencia.Text = "Valor transferencia:";
            // 
            // txtAbonaTransferencia
            // 
            this.txtAbonaTransferencia.Location = new System.Drawing.Point(571, 379);
            this.txtAbonaTransferencia.Name = "txtAbonaTransferencia";
            this.txtAbonaTransferencia.Size = new System.Drawing.Size(82, 20);
            this.txtAbonaTransferencia.TabIndex = 25;
            this.txtAbonaTransferencia.TextChanged += new System.EventHandler(this.txtAbonaTransferencia_TextChanged);
            this.txtAbonaTransferencia.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAbonaTransferencia_KeyPress);
            // 
            // FrmHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(657, 448);
            this.Controls.Add(this.txtAbonaTransferencia);
            this.Controls.Add(this.lblTransferencia);
            this.Controls.Add(this.grbResumenDeVenta);
            this.Controls.Add(this.txtAbona);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.lblAbona);
            this.Controls.Add(this.buttonHistorial);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmHome";
            this.Text = "Comercializadora Verdum - Registro de Ventas";
            this.Load += new System.EventHandler(this.FrmHome_Load);
            this.Shown += new System.EventHandler(this.FrmHome_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbResumenDeVenta.ResumeLayout(false);
            this.grbResumenDeVenta.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblNombreCliente;
        private System.Windows.Forms.TextBox txtCliente;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label lblnumerofactura;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblTituloResumen;
        private System.Windows.Forms.Label lblResumenVenta;
        private System.Windows.Forms.TextBox txtAbona;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label lblAbona;
        private System.Windows.Forms.Button buttonHistorial;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grbResumenDeVenta;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdbTransferencia;
        private System.Windows.Forms.RadioButton rdB_Efectivo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDevuelta;
        private System.Windows.Forms.Label lblTransferencia;
        private System.Windows.Forms.TextBox txtAbonaTransferencia;
    }
}

