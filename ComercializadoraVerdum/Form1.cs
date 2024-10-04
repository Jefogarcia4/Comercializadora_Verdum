using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace ComercializadoraVerdum
{
    public partial class FrmHome : Form
    {
        private OleDbConnection connection;
        private int consecutivo = 0;
        private DateTime fechaActual;
        public FrmHome()
        {
           
            InitializeComponent();
            fechaActual = DateTime.Now.Date;
            this.Load += FrmHome_Load;
            txtCliente.KeyDown += txtCliente_KeyDown;
            fechaActual = DateTime.Now;
            InitializeDatabaseConnection();
            LoadProductsIntoComboBox();
            InitializeDataGridView();
            if (CargarRegistrosDelDia())
            {
                GenerarNuevaFactura();
            }
            else
            {
                lblnumerofactura.Text = $"Número Factura: {fechaActual:yyyyMMdd}0001";
            }
            this.Shown += new EventHandler(FrmHome_Shown);
        }

        private bool CargarRegistrosDelDia()
        {

            consecutivo = 0;
            return consecutivo > 0;
        }

        private void GenerarNuevaFactura()
        {
            if (fechaActual.Date == DateTime.Now.Date)
            {
                string numeroActualTexto = lblnumerofactura.Text.Replace("Número Factura: ", "");

                if (long.TryParse(numeroActualTexto, out long numeroActual))
                {
                    numeroActual++; 
                    lblnumerofactura.Text = $"Número Factura: {numeroActual:D15}"; 
                }
                else
                {
                    MessageBox.Show("Error al convertir el número de factura actual.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            fechaActual = DateTime.Now.Date;
            lblnumerofactura.Text = $"Número Factura: {fechaActual:yyyyMMdd}0001";
        }


        private void InitializeDatabaseConnection()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Verdum.accdb";
            connection = new OleDbConnection(connectionString);
        }

        private void LoadProductsIntoComboBox()
        {
            try
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("SELECT id, nombre FROM Productos", connection);
                dataGridView1.Columns.Add("Canasta P. KG", "Canasta P. KG");
                OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                DataTable productsTable = new DataTable();
                adapter.Fill(productsTable);
                DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = "Producto",
                    HeaderText = "Producto",
                    DataSource = productsTable,
                    DisplayMember = "nombre",
                    ValueMember = "id"
                };

                dataGridView1.Columns.Add(comboBoxColumn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void InitializeDataGridView()
        {
            int rowIndex = dataGridView1.Rows.Add();
            DataGridViewRow newRow = dataGridView1.Rows[rowIndex];
            newRow.Cells["Canasta P. KG"].Value = 1.7;   
            dataGridView1.Columns.Add("Precio", "Precio");
            dataGridView1.Columns.Add("Canastas", "Canastas");
            dataGridView1.Columns.Add("PesoBruto", "PesoBruto");
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
            dataGridView1.Columns.Add("Total", "Total");


            DataGridViewTextBoxColumn isSavedColumn = new DataGridViewTextBoxColumn
            {
                Name = "IsSaved",
                Visible = false
            };
            dataGridView1.Columns.Add(isSavedColumn);
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
        }

        private void FrmHome_Load(object sender, EventArgs e)
        { 

            txtCliente.Focus();
<<<<<<< HEAD
=======
            lblFecha.Text = DateTime.Now.ToShortDateString();
>>>>>>> 817c74755b4c193018c0e6f16d6532a1e87ac2a7
            dataGridView1.Enabled = false; 
            btnLimpiar.Visible = false; 
        }

        private void CalculateQuantity(int rowIndex)
        {
            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            if (row.Cells["Canastas"].Value != null && row.Cells["PesoBruto"].Value != null)
            {
                if (int.TryParse(row.Cells["Canastas"].Value.ToString(), out int canastas) &&
                    double.TryParse(row.Cells["PesoBruto"].Value.ToString(), out double pesoBruto))
                {
                    double cantidad = pesoBruto - (canastas * 1.7);
                    row.Cells["Cantidad"].Value = cantidad;

                    if (double.TryParse(row.Cells["Precio"].Value?.ToString(), out double precio))
                    {
                        row.Cells["Total"].Value = cantidad * precio;
                    }
                }
            }
        }

        private void CalculateTotalSum()
        {
            decimal totalSum = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                if (decimal.TryParse(row.Cells["Total"].Value?.ToString(), out decimal total))
                {
                    totalSum += total;
                }
            }
            
            label3.Text = $"Total: {Convert.ToDecimal(totalSum).ToString("C0")}";
        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
=======
            //Prueba de guardar en github
            try
            {
                connection.Open();
>>>>>>> 817c74755b4c193018c0e6f16d6532a1e87ac2a7

            if (string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtAbona.Text))
            {
                MessageBox.Show("Por favor, complete los campos Nombre Cliente y valor pagado.", "Campos requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    connection.Open();

                    decimal totalCompra = 0;
                    int totalCanastas = 0;
                    double totalPesoBruto = 0;
                    string valor1Texto = label3.Text.Replace("Total: $ ", "").Replace(",", "");
                    string valor2Texto = lblDescuento.Text.Replace("Descuento: $ ", "").Replace(",", "");
                    string abonoTexto = txtAbona.Text.Replace(",", "");
                    if (decimal.TryParse(valor1Texto, out decimal valor1) &&
                    decimal.TryParse(valor2Texto, out decimal valor2) &&
                    decimal.TryParse(abonoTexto, out decimal abono))
                    {
                        decimal total = Math.Abs(valor1 - valor2 - abono);
                        if (abono < valor1)
                        {
                            total = valor1 - abono;
                            MessageBox.Show($"El cliente: {txtCliente.Text} deja una deuda de: ${total:N0}", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Debe devolver al cliente {txtCliente.Text} un valor de: ${total:N0}", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            total = valor1 - valor2 - abono; 
                        }

                       
                    }
                    else
                    {
                        MessageBox.Show("Por favor, ingrese valores válidos.");
                    }


                    decimal descuento = decimal.Parse(lblDescuento.Text);
                    decimal totalPagar = totalCompra - descuento;

                    string insertVentaQuery = "INSERT INTO Ventas (consecutivo, nombreCliente, totalproductos, totalcanastas, totalpesobruto, totalcompra, descuento, totalpagar) VALUES (?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand ventaCommand = new OleDbCommand(insertVentaQuery, connection))
                    {
                        ventaCommand.Parameters.AddWithValue("consecutivo", consecutivo);
                        ventaCommand.Parameters.AddWithValue("@nombreCliente", txtCliente.Text);
                        ventaCommand.Parameters.AddWithValue("@totalproductos", dataGridView1.Rows.Count - 1);
                        ventaCommand.Parameters.AddWithValue("@totalcanastas", totalCanastas);
                        ventaCommand.Parameters.AddWithValue("@totalpesobruto", totalPesoBruto);
                        ventaCommand.Parameters.AddWithValue("@totalcompra", totalCompra);
                        ventaCommand.Parameters.AddWithValue("@descuento", descuento);
                        ventaCommand.Parameters.AddWithValue("@totalpagar", totalPagar);

                        ventaCommand.ExecuteNonQuery();
                    }

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;
                        bool isSaved = row.Cells["IsSaved"].Value != null && Convert.ToBoolean(row.Cells["IsSaved"].Value);
                        if (!isSaved)
                        {
                            decimal pesocanastaKG = Convert.ToDecimal(row.Cells["Canasta P. KG"].Value);
                            int idProducto = Convert.ToInt32(row.Cells["Producto"].Value);
                            decimal precio = Convert.ToDecimal(row.Cells["Precio"].Value);
                            int canastas = Convert.ToInt32(row.Cells["Canastas"].Value);
                            double pesoBruto = Convert.ToDouble(row.Cells["PesoBruto"].Value);
                            int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                            decimal valortotal = Convert.ToDecimal(row.Cells["Total"].Value);
                            string nombreCliente = txtCliente.Text;
                            decimal abona = decimal.Parse(txtAbona.Text);

                            totalCompra += valortotal;
                            totalCanastas += canastas;
                            totalPesoBruto += pesoBruto;

                            string query = "INSERT INTO DetalleVentas (productoid, precio, canastas, pesobruto, cantidad, valortotal) VALUES (?, ?, ?, ?, ?, ?)";

                            OleDbCommand command = new OleDbCommand(query, connection);
                            command.Parameters.AddWithValue("@productoid", idProducto);
                            command.Parameters.AddWithValue("@precio", precio);
                            command.Parameters.AddWithValue("@canastas", canastas);
                            command.Parameters.AddWithValue("@pesobruto", pesoBruto);
                            command.Parameters.AddWithValue("@cantidad", cantidad);
                            command.Parameters.AddWithValue("@valortotal", valortotal);

                            command.ExecuteNonQuery();

                            row.Cells["IsSaved"].Value = true;
                        }
                    }

                    GenerarNuevaFactura();
                    MessageBox.Show("Ventas guardadas exitosamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving sales: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }  
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && (dataGridView1.Columns[e.ColumnIndex].Name == "Canastas" || dataGridView1.Columns[e.ColumnIndex].Name == "PesoBruto"))
            {
                CalculateQuantity(e.RowIndex);
                CalculateTotalSum();
            }
        }

        private void buttonHistorial_Click(object sender, EventArgs e)
        {
            Historial formHistorial = new Historial();
            formHistorial.Show();
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            txtCliente.Enabled = false;
            btnLimpiar.Visible = true;
            dataGridView1.Enabled = true;
            ValidarCliente(txtCliente.Text);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtCliente.Clear();
            txtCliente.Enabled = true;
            btnLimpiar.Visible = false;
            dataGridView1.Enabled = false;
            dataGridView1.DataSource = null;
            lblDescuento.Text = $"Descuento: ";

        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; 
            }
        }

        private void ValidarCliente(string nombreCliente)
        {
            decimal valorMostrar = 0; 

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT SaldoFavor, SaldoDeuda FROM clientes WHERE NombreCliente = @NombreCliente";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreCliente", nombreCliente);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            decimal saldoFavor = reader.GetDecimal(0);
                            decimal saldoDeuda = reader.GetDecimal(1);
                            if (saldoFavor != 0)
                            {
                                valorMostrar = saldoFavor; 
                            }
                            else if (saldoDeuda != 0)
                            {
                                valorMostrar = -saldoDeuda;
                            }
                        }
                        else 
                        {
                            CrearCliente(nombreCliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close(); 
                }
            }

            lblDescuento.Text = $"Descuento: $ {valorMostrar.ToString("N0")}";
        }


        private void CrearCliente(string nombreCliente)
        {
            try
            {
                string insertQuery = "INSERT INTO Clientes (NombreCliente, SaldoFavor, SaldoDeuda) VALUES (@NombreCliente, 0, 0)";
                using (OleDbCommand command = new OleDbCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@NombreCliente", nombreCliente);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["Canasta P. KG"].Value = 1.7;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmHome_Shown(object sender, EventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                txtCliente.Focus(); 
            });
        }
    }
}
