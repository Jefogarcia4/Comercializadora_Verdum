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
        public FrmHome()
        {
           
            InitializeComponent();
            this.Load += FrmHome_Load;
            txtCliente.KeyDown += txtCliente_KeyDown;
            InitializeDatabaseConnection();
            LoadProductsIntoComboBox();
            InitializeDataGridView();
        }

        private void InitializeDatabaseConnection()
        {
            // Cambia la ruta de acceso a tu archivo de base de datos Access.
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Verdum.accdb";
            connection = new OleDbConnection(connectionString);
        }

        private void LoadProductsIntoComboBox()
        {
            try
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("SELECT id, nombre FROM Productos", connection);
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
            //Prueba
            txtCliente.Focus();
            lblFecha.Text = DateTime.Now.ToShortDateString();
            dataGridView1.Enabled = false; // Inhabilitar el DataGridView
            btnLimpiar.Visible = false; // Ocultar el botón Limpiar
        }





        private void CalculateQuantity(int rowIndex)
        {
            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            if (row.Cells["Canastas"].Value != null && row.Cells["PesoBruto"].Value != null)
            {
                if (int.TryParse(row.Cells["Canastas"].Value.ToString(), out int canastas) &&
                    double.TryParse(row.Cells["PesoBruto"].Value.ToString(), out double pesoBruto))
                {
                    // Suponiendo que la cantidad se calcula como Canastas * Peso Bruto
                    double cantidad = pesoBruto - (canastas * 1.7);
                    row.Cells["Cantidad"].Value = cantidad;

                    // Si la celda de Precio tiene un valor, calcular el total
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
            
            label4.Text = $"{Convert.ToDecimal(totalSum).ToString("C0")}";
        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            //Prueba de guardar en github
            try
            {
                connection.Open();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;
                    bool isSaved = row.Cells["IsSaved"].Value != null && Convert.ToBoolean(row.Cells["IsSaved"].Value);
                    if (!isSaved)
                    {
                        int canastas = Convert.ToInt32(row.Cells["Canastas"].Value);
                        double pesoBruto = Convert.ToDouble(row.Cells["PesoBruto"].Value);
                        int idProducto = Convert.ToInt32(row.Cells["Producto"].Value);
                        int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                        decimal precio = Convert.ToDecimal(row.Cells["Precio"].Value);
                        decimal total = Convert.ToDecimal(row.Cells["Total"].Value);
                        string query = "INSERT INTO Ventas (canastas, pesobruto,fecha,productoid,cantidad, precio, total) VALUES (?, ?, ?, ?,?,?,?,?)";

                        OleDbCommand command = new OleDbCommand(query, connection);
                        command.Parameters.AddWithValue("@canastas", canastas);
                        command.Parameters.AddWithValue("@pesobruto", pesoBruto);
                        command.Parameters.AddWithValue("@fecha", DateTime.Now.ToShortDateString());
                        command.Parameters.AddWithValue("@productoid", idProducto);
                        command.Parameters.AddWithValue("@cantidad", cantidad);
                        command.Parameters.AddWithValue("@precio", precio);
                        command.Parameters.AddWithValue("@total", total);
                        command.ExecuteNonQuery();

                        // Marcar la fila como guardada
                        row.Cells["IsSaved"].Value = true;
                    }
                    
                }

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

    }
}
