using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComercializadoraVerdum
{

    public partial class IngresoPagos : Form
    {
        public decimal ValorEfectivo { get; private set; } = 0;
        public decimal ValorTransferencia { get; private set; } = 0;
        public decimal ValorDeuda { get; private set; } = 0;
        public IngresoPagos(string valorDeuda)
        {
            this.Icon = new Icon("Images/verdum-logo-icono.ico");
            InitializeComponent();
            lblDeuda.Text = valorDeuda;
            ValorDeuda = Convert.ToDecimal(valorDeuda);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IngresoPagos_FormClosing);
        }
        private void IngresoPagos_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnIngresoPagos_Click(object sender, EventArgs e)
        {
            decimal efectivo = 0;
            decimal transferencia = 0;

            if (!string.IsNullOrWhiteSpace(txtefectivo.Text))
            {
                if (!decimal.TryParse(txtefectivo.Text, out efectivo))
                {
                    MessageBox.Show("El valor ingresado en 'Efectivo' no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txttransferencia.Text))
            {
                if (!decimal.TryParse(txttransferencia.Text, out transferencia))
                {
                    MessageBox.Show("El valor ingresado en 'Transferencia' no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Asignar los valores obtenidos
            ValorEfectivo = efectivo;
            ValorTransferencia = transferencia;

            // Cerrar el formulario y retornar el resultado
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtefectivo_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtefectivo.Text))
            {
                btnIngresoPagos.Enabled = false;
                return;
            }

            // Guarda la posición actual del cursor
            int cursorPosition = txtefectivo.SelectionStart;

            // Elimina caracteres no numéricos temporalmente
            string rawText = new string(txtefectivo.Text.Where(c => char.IsDigit(c)).ToArray());

            if (decimal.TryParse(rawText, out decimal valorPagado))
            {
                // Verifica si el valor es válido
                btnIngresoPagos.Enabled = valorPagado >= 0;

                // Aplica el formato con separadores de miles
                txtefectivo.Text = valorPagado.ToString("N0");

                if (decimal.TryParse(txttransferencia.Text, out decimal valorTransferencia))
                {
                    valorPagado += valorTransferencia;
                }

                decimal devuelta = valorPagado - ValorDeuda;
                lblDevuelve.Text = devuelta.ToString("C0");

                // Restaura la posición del cursor ajustado según los separadores de miles
                int delta = txtefectivo.Text.Length - rawText.Length;
                txtefectivo.SelectionStart = cursorPosition + delta;
            }
            else
            {
                // Si el texto no es válido, limpia el campo
                txtefectivo.Text = string.Empty;
                btnIngresoPagos.Enabled = false;
            }
        }


        private void txttransferencia_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttransferencia.Text))
            {
                btnIngresoPagos.Enabled = false;
                return;
            }

            // Guarda la posición actual del cursor
            int cursorPosition = txttransferencia.SelectionStart;

            // Elimina caracteres no numéricos temporalmente
            string rawText = new string(txttransferencia.Text.Where(c => char.IsDigit(c)).ToArray());

            if (decimal.TryParse(rawText, out decimal valorPagado))
            {
                // Verifica si el valor es válido
                btnIngresoPagos.Enabled = valorPagado >= 0;

                // Aplica el formato con separadores de miles
                txttransferencia.Text = valorPagado.ToString("N0");

                if (decimal.TryParse(txtefectivo.Text, out decimal valorEfectivo))
                {
                    valorPagado += valorEfectivo;
                }

                decimal devuelta = valorPagado - ValorDeuda;
                lblDevuelve.Text = devuelta.ToString("C0");

                // Restaura la posición del cursor ajustado según los separadores de miles
                int delta = txttransferencia.Text.Length - rawText.Length;
                txttransferencia.SelectionStart = cursorPosition + delta;
            }
            else
            {
                // Si el texto no es válido, limpia el campo
                txttransferencia.Text = string.Empty;
                btnIngresoPagos.Enabled = false;
            }
        }

        private void IngresoPagos_Load(object sender, EventArgs e)
        {

        }
    }
}
