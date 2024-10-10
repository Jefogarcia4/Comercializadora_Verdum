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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ComercializadoraVerdum
{
    public partial class FrmHome : Form
    {
        private OleDbConnection connection;
        private int consecutivo = 0;
        private DateTime fechaActual;
        private IConfigurationRoot configuration;
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
            SiguienteConsecutivo();
            this.Shown += new EventHandler(FrmHome_Shown);
        }

        private bool CargarRegistrosDelDia()
        {

            consecutivo = 0;
            return consecutivo > 0;
        }

        private void SiguienteConsecutivo() 
        {
            try
            {
                connection.Open();

                string fechaActual = DateTime.Now.ToString("yyyyMMdd");

                string query = "SELECT TOP 1 consecutivo FROM Ventas WHERE consecutivo LIKE ? ORDER BY consecutivo DESC";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", fechaActual + "%");

                    object result = command.ExecuteScalar();
                    string nuevoConsecutivo;

                    if (result != null)
                    {
                        string ultimoConsecutivo = result.ToString();
                        string numeroConsecutivoStr = ultimoConsecutivo.Substring(8); 

                        if (int.TryParse(numeroConsecutivoStr, out int numeroConsecutivo))
                        {
                            numeroConsecutivo++; 
                            nuevoConsecutivo = fechaActual + numeroConsecutivo.ToString("D5"); 
                        }
                        else
                        {
                            MessageBox.Show("Error al parsear el número del consecutivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        nuevoConsecutivo = fechaActual + "0001";
                    }

                    lblnumerofactura.Text = $"Número Factura: {nuevoConsecutivo}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el consecutivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
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
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");
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
            this.Size = new Size(820, 440);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            dataGridView1.Enabled = false; 
            btnLimpiar.Visible = false; 
        }

        private void CalculateQuantity(int rowIndex)
        {
            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            if (row.Cells["Canastas"].Value != null && row.Cells["PesoBruto"].Value != null && row.Cells["Canasta P. KG"].Value != null)
            {
                if (int.TryParse(row.Cells["Canastas"].Value.ToString(), out int canastas) &&
                    double.TryParse(row.Cells["PesoBruto"].Value.ToString(), out double pesoBruto) &&
                    double.TryParse(row.Cells["Canasta P. KG"].Value.ToString(), out double canastapkg))
                {
                    double cantidad = pesoBruto - (canastas * canastapkg);
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

            try
            {
                connection.Open();

                if (string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtAbona.Text))
                {
                    MessageBox.Show("Por favor, complete los campos Nombre Cliente y valor pagado.", "Campos requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        decimal totalCompra = 0;
                        int totalCanastas = 0;
                        double totalPesoBruto = 0;
                        decimal total = 0;
                        string valor1Texto = label3.Text.Replace("Total: $ ", "").Replace(",", "");
                        string valor2Texto = lblDescuento.Text.Replace("Descuento: $ ", "").Replace(",", "");
                        string abonoTexto = txtAbona.Text.Replace(",", "");

                        decimal saldoFavor = ObtenerSaldoFavor(txtCliente.Text);
                        decimal saldoEnContra = ObtenerSaldoEnContra(txtCliente.Text);

                        if (decimal.TryParse(valor1Texto, out decimal totalValorCompra) &&
                            decimal.TryParse(valor2Texto, out decimal descuento) &&
                            decimal.TryParse(abonoTexto, out decimal abono))
                        {
                            if (saldoFavor > 0)
                            {
                                totalValorCompra -= Math.Min(descuento, saldoFavor);
                            }

                            total = totalValorCompra - abono;

                            if (total > 0) 
                            {
                                decimal deuda = total;
                                MessageBox.Show($"El cliente: {txtCliente.Text} deja una deuda de: ${deuda:N0}",
                                                "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ActualizarSaldoClienteEnContra(txtCliente.Text, totalValorCompra, deuda, abono);
                            }
                            else if (total == 0) // Pago completo exitoso
                            {
                                MessageBox.Show($"El cliente: {txtCliente.Text} pagó el total de la compra. Transacción exitosa.",
                                                "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                // Guardar compra exitosa sin deuda
                            }
                            else // Total es negativo (saldo a favor)
                            {
                                decimal saldoNuevoAFavor = -total; // Convertimos a positivo
                                MessageBox.Show($"Debe devolver al cliente: {txtCliente.Text} un valor de: ${saldoNuevoAFavor:N0}",
                                                "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ActualizarSaldoClienteAFavor(txtCliente.Text, totalValorCompra, saldoNuevoAFavor, abono);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por favor, ingrese valores válidos.");
                            return;
                        }



                        string fechaActual = DateTime.Now.ToString("yyyyMMdd");
                        string nuevoConsecutivo = "";
                        int nuevoNumero = 1;

                        string lastConsecutivoQuery = "SELECT TOP 1 consecutivo FROM Ventas WHERE consecutivo LIKE ? ORDER BY consecutivo DESC";
                        using (var command = new OleDbCommand(lastConsecutivoQuery, connection))
                        {
                            command.Parameters.AddWithValue("?", fechaActual + "%");
                            object result = command.ExecuteScalar();

                            if (result != null)
                            {
                                string lastConsecutivo = result.ToString();
                                if (lastConsecutivo.Length > 8)
                                {
                                    string lastNumberStr = lastConsecutivo.Substring(8);
                                    if (int.TryParse(lastNumberStr, out int lastNumber))
                                    {
                                        nuevoNumero = lastNumber + 1;
                                    }
                                }
                            }

                            nuevoConsecutivo = fechaActual + nuevoNumero.ToString("D5");
                        }


                        int ventaId;
                        string insertVentaQuery = "INSERT INTO Ventas (consecutivo, nombreCliente, totalproductos, totalcanastas, totalpesobruto, totalcompra, descuento, totalpagar) " +
                                                  "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                        using (OleDbCommand ventaCommand = new OleDbCommand(insertVentaQuery, connection))
                        {
                            ventaCommand.Parameters.AddWithValue("consecutivo", nuevoConsecutivo);
                            ventaCommand.Parameters.AddWithValue("@nombreCliente", txtCliente.Text);
                            ventaCommand.Parameters.AddWithValue("@totalproductos", 0);
                            ventaCommand.Parameters.AddWithValue("@totalcanastas", 0);
                            ventaCommand.Parameters.AddWithValue("@totalpesobruto", 0);
                            ventaCommand.Parameters.AddWithValue("@totalcompra", 0);
                            ventaCommand.Parameters.AddWithValue("@descuento", descuento);
                            ventaCommand.Parameters.AddWithValue("@totalpagar", total);

                            ventaCommand.ExecuteNonQuery();

                            ventaCommand.CommandText = "SELECT @@IDENTITY";
                            ventaId = Convert.ToInt32(ventaCommand.ExecuteScalar());
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

                                totalCompra += valortotal;
                                totalCanastas += canastas;
                                totalPesoBruto += pesoBruto;

                                string query = "INSERT INTO DetalleVentas (ventaId, productoid, precio, canastas, pesobruto, cantidad, valortotal) " +
                                               "VALUES (?, ?, ?, ?, ?, ?, ?)";

                                using (OleDbCommand command = new OleDbCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@ventaId", ventaId);
                                    command.Parameters.AddWithValue("@productoid", idProducto);
                                    command.Parameters.AddWithValue("@precio", precio);
                                    command.Parameters.AddWithValue("@canastas", canastas);
                                    command.Parameters.AddWithValue("@pesobruto", pesoBruto);
                                    command.Parameters.AddWithValue("@cantidad", cantidad);
                                    command.Parameters.AddWithValue("@valortotal", valortotal);

                                    command.ExecuteNonQuery();
                                }

                                row.Cells["IsSaved"].Value = true;
                            }
                        }

                        string updateVentaQuery = "UPDATE Ventas SET totalproductos = ?, totalcanastas = ?, totalpesobruto = ?, totalcompra = ? WHERE VentaId = ?";
                        using (OleDbCommand updateCommand = new OleDbCommand(updateVentaQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@totalproductos", dataGridView1.Rows.Count - 1);
                            updateCommand.Parameters.AddWithValue("@totalcanastas", totalCanastas);
                            updateCommand.Parameters.AddWithValue("@totalpesobruto", totalPesoBruto);
                            updateCommand.Parameters.AddWithValue("@totalcompra", totalCompra);
                            updateCommand.Parameters.AddWithValue("@VentaId", ventaId);

                            updateCommand.ExecuteNonQuery();
                        }

                        GenerarNuevaFactura();
                        MessageBox.Show("Venta registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error guardando la venta: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la conexión: " + ex.Message);
            }
        }

        private decimal ObtenerSaldoFavor(string nombreCliente)
        {
            decimal saldofavor = 0;
            string query = "SELECT SaldoFavor FROM Clientes WHERE NombreCliente = @NombreCliente";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NombreCliente", nombreCliente);
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    saldofavor = Convert.ToDecimal(result);
                }
            }
            return saldofavor;
        }

        private decimal ObtenerSaldoEnContra(string nombreCliente)
        {
            decimal saldoEnContra = 0;
            string query = "SELECT SaldoDeuda FROM Clientes WHERE NombreCliente = @NombreCliente";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NombreCliente", nombreCliente);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    saldoEnContra = Convert.ToDecimal(result);
                }
            }
            return saldoEnContra;
        }

        private void ActualizarSaldoClienteEnContra(string nombreCliente, decimal valorcomprahoy, decimal saldopendiente, decimal abona)
        {

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            try
            {
                decimal saldoFavor = 0;
                decimal saldoEnContra = 0;

                string query = "SELECT SaldoFavor, SaldoDeuda FROM Clientes WHERE NombreCliente = @nombreCliente";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            saldoFavor = reader.GetDecimal(0);  
                            saldoEnContra = reader.GetDecimal(1); 
                        }
                    }
                }

                decimal nuevoSaldoDeuda = saldopendiente;

                if (saldoFavor > 0)
                {
                    if (saldoFavor >= saldopendiente)
                    {
                        saldoFavor -= nuevoSaldoDeuda;
                        nuevoSaldoDeuda = 0;
                    }
                    else
                    {
                        nuevoSaldoDeuda -= saldoFavor;
                        saldoFavor = 0;
                    }
                }

                string updateQuery = "UPDATE Clientes SET SaldoFavor = @nuevoSaldoFavor, SaldoDeuda = @nuevoSaldoDeuda WHERE NombreCliente = @nombreCliente";
                using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@nuevoSaldoFavor", saldoFavor);
                    updateCommand.Parameters.AddWithValue("@nuevoSaldoDeuda", nuevoSaldoDeuda);
                    updateCommand.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("El saldo del cliente ha sido actualizado correctamente.", "Actualizado!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el saldo del cliente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la base de datos: " + ex.Message);
                Application.Exit();
            }
    }
        private void ActualizarSaldoClienteAFavor(string nombreCliente, decimal valorcomprahoy, decimal saldopendiente, decimal abona)
        {

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                decimal saldoFavor = 0;
                decimal saldoEnContra = 0;

                string query = "SELECT SaldoFavor, SaldoDeuda FROM Clientes WHERE NombreCliente = @nombreCliente";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            saldoFavor = reader.GetDecimal(0);
                            saldoEnContra = reader.GetDecimal(1);
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el cliente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                decimal nuevoSaldoFavor = 0;
                if (abona > valorcomprahoy && saldopendiente > 0)
                {
                    nuevoSaldoFavor += saldopendiente;
                }
                
                string updateQuery = "UPDATE Clientes SET SaldoFavor = @nuevoSaldoFavor, SaldoDeuda = @nuevoSaldoDeuda WHERE NombreCliente = @nombreCliente";
                using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@nuevoSaldoFavor", nuevoSaldoFavor);
                    updateCommand.Parameters.AddWithValue("@nuevoSaldoDeuda", 0);
                    updateCommand.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("El saldo a favor del cliente ha sido actualizado correctamente.", "Actualizado!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el saldo a favor del cliente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la base de datos: " + ex.Message);
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
            //if (valorMostrar < 0)
            //{
            //    decimal total;
            //    total = Math.Abs(valorMostrar);
            //    MessageBox.Show($"El cliente: {txtCliente.Text} tiene actualmente una deuda de: ${total:N0}. Por favor ponerse al día para continuar.", "Advertencia.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    LimpiarCampos();
            //}
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

        private void LimpiarCampos()
        {
            txtCliente.Text = string.Empty;
            txtAbona.Text = string.Empty;
            btnLimpiar.Visible = false;
            txtCliente.Enabled = true;
            label3.Text = "Total: ";
            lblDescuento.Text = "Descuento: ";
            dataGridView1.Rows.Clear();
            SiguienteConsecutivo();
        }

    }
}
