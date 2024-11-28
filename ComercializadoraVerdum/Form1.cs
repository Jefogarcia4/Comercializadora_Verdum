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
using System.Net;
using System.Globalization;
using System.Data.Common;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Threading;
using static ComercializadoraVerdum.Historial;
using System.Drawing.Printing;


namespace ComercializadoraVerdum
{
    public partial class FrmHome : Form
    {
        private OleDbConnection connection;
        private int consecutivo = 0;
        private DateTime fechaActual;
        private IConfigurationRoot configuration;
        private Dictionary<(string producto, string precio), List<ProductoDetalle>> resumenProductos = new Dictionary<(string producto, string precio), List<ProductoDetalle>>();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _backgroundTask;
        private List<DetalleVenta> _detalleventas;
        private string _consecutivo, _fecha, _nombreCliente, _totalvalorventa, _totalpeso;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

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
            this.Icon = new Icon("Images/verdum-logo-icono.ico");
            this.Shown += new EventHandler(FrmHome_Shown);
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            printDocument.BeginPrint += new PrintEventHandler(PrintDocument_BeginPrint);
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
            printPreviewDialog.Document = printDocument;
            dataGridView1.AllowUserToDeleteRows = true;
            _cancellationTokenSource = new CancellationTokenSource();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            FormManager.IncrementOpenForms();

        }
        private async void StartBackgroundTask()
        {
            _backgroundTask = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    Thread.Sleep(100);
                }
            });
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource.Cancel();
            this.Dispose();
            FormManager.DecrementOpenForms();
        }
        private void ScrollPanel()
        {
            Panel panel1 = new Panel();
            panel1.AutoScroll = true;
            grbResumenDeVenta.Controls.Add(panel1);
            panel1.Controls.Add(lblResumenVenta);
        }

        // Cambia el tipo de resumenProductos para usar una clave compuesta

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridView1.Rows[e.RowIndex];
            string producto = row.Cells["Producto"].FormattedValue?.ToString();
            string precio = row.Cells["Precio"].Value?.ToString();
            string canastasStr = row.Cells["Canastas"].Value?.ToString();
            string pesoBrutoStr = row.Cells["Pesobruto"].Value?.ToString();
            string cantidadStr = row.Cells["Cantidad"].Value?.ToString();
            string total = row.Cells["Total"].Value?.ToString();

            if (!string.IsNullOrEmpty(producto) &&
                !string.IsNullOrEmpty(precio) &&
                decimal.TryParse(canastasStr, out decimal canastas) &&
                decimal.TryParse(pesoBrutoStr, out decimal pesoBruto) &&
                decimal.TryParse(cantidadStr, out decimal cantidad))
            {
                var clave = (producto, precio);

                if (resumenProductos.ContainsKey(clave))
                {
                    // Actualizar los valores acumulativos
                    var existente = resumenProductos[clave].FirstOrDefault();
                    if (existente != null)
                    {
                        existente.Canastas = (decimal.Parse(existente.Canastas) + canastas).ToString();
                        existente.PesoBruto = (decimal.Parse(existente.PesoBruto) + pesoBruto).ToString();
                        existente.Cantidad = (decimal.Parse(existente.Cantidad) + cantidad).ToString();
                        existente.Total = (decimal.Parse(existente.Total.Replace("$", "")) +  (decimal.Parse(total.Replace("$","")))).ToString();
                    }
                }
                else
                {
                    // Agregar un nuevo detalle si no existe
                    var detalle = new ProductoDetalle
                    {
                        Precio = precio,
                        Canastas = canastas.ToString(),
                        PesoBruto = pesoBruto.ToString(),
                        Cantidad = cantidad.ToString(),
                        Total = total
                    };
                    resumenProductos[clave] = new List<ProductoDetalle> { detalle };
                }

                ActualizarResumenVentaLabel();
            }
        }

        private void ActualizarResumenVentaLabel()
        {
            StringBuilder resumenVenta = new StringBuilder();
            var cultura = new CultureInfo("es-CO");
            resumenVenta.AppendLine("Producto | Precio | Canastas | Peso Neto| Valor");
            resumenVenta.AppendLine("---------------------------------------------------------");

            foreach (var producto in resumenProductos)
            {
                string productoNombre = producto.Key.producto;
                string precio = producto.Key.precio;

                // Sumamos los valores dentro del grupo de producto-precio
                int totalCanastas = producto.Value.Sum(d => int.Parse(d.Canastas));
                decimal totalPesoBruto = producto.Value.Sum(d => decimal.Parse(d.PesoBruto));
                decimal totalCantidad = producto.Value.Sum(d => decimal.Parse(d.Cantidad));
                decimal totalSumado = producto.Value.Sum(d =>
                           decimal.Parse(d.Total.Replace("$", "")));


                resumenVenta.AppendLine($"{productoNombre,-12}| {precio,-9}| {totalCanastas,-6}| {totalCantidad,-8}| {totalSumado.ToString("C0")}");

                resumenVenta.AppendLine();
            }

            lblResumenVenta.Text = resumenVenta.ToString();
            AjustarAlturaResumenVenta();
        }


        private void AjustarAlturaResumenVenta()
        {
            lblResumenVenta.Height = TextRenderer.MeasureText(lblResumenVenta.Text, lblResumenVenta.Font, lblResumenVenta.Size, TextFormatFlags.WordBreak).Height;
        }

        private void IconoFormularioRegistroVentas()
        {
            try
            {
                string tempIconPath = Path.Combine(Path.GetTempPath(), "tempIcon.ico");
                string url = "https://img.icons8.com/ios/50/receipt-dollar.png";
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(url, tempIconPath);
                }

                this.Icon = new Icon(tempIconPath);

                File.Delete(tempIconPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar o establecer el ícono: " + ex.Message);
            }
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
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

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
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                OleDbCommand command = new OleDbCommand("SELECT id, nombre, precio FROM Productos", connection);
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
                comboBoxColumn.Width = 124;
                dataGridView1.Columns.Add(comboBoxColumn);

                dataGridView1.Columns.Add("Precio", "Precio");

                dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                dataGridView1.CurrentCellDirtyStateChanged += DataGridView1_CurrentCellDirtyStateChanged;
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

        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private decimal GetProductPriceById(int productId)
        {
            decimal price = 0;

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                OleDbCommand command = new OleDbCommand("SELECT precio FROM Productos WHERE id = ?", connection);
                command.Parameters.AddWithValue("?", productId);
                price = (decimal)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el precio: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return price;
        }

        private void InitializeDataGridView()
        {
            int rowIndex = dataGridView1.Rows.Add();
            DataGridViewRow newRow = dataGridView1.Rows[rowIndex];
            newRow.Cells["Canasta P. KG"].Value = 1.7;

            dataGridView1.Columns.Add("Canastas", "Canastas");
            dataGridView1.Columns.Add("PesoBruto", "PesoBruto");
            dataGridView1.Columns.Add("Cantidad", "PesoNeto");
            dataGridView1.Columns.Add("Total", "Total");

            dataGridView1.Columns["Canasta P. KG"].Width = 80;
            dataGridView1.Columns["Producto"].Width = 80;
            dataGridView1.Columns["Canastas"].Width = 80;
            dataGridView1.Columns["PesoBruto"].Width = 80;
            dataGridView1.Columns["Cantidad"].Width = 80;
            dataGridView1.Columns["Total"].Width = 80;

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
            StartBackgroundTask();
            txtCliente.Focus();
            //this.Size = new Size(820, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            dataGridView1.Enabled = false;
            btnLimpiar.Visible = false;
        }

        private void CalculateQuantity(int rowIndex)
        {
            // Cultura colombiana
            var culturaColombiana = new CultureInfo("es-CO");

            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            if (row.Cells["Canastas"].Value != null && row.Cells["PesoBruto"].Value != null && row.Cells["Canasta P. KG"].Value != null)
            {
                if (int.TryParse(row.Cells["Canastas"].Value.ToString(), out int canastas) &&
                    double.TryParse(row.Cells["PesoBruto"].Value.ToString(), NumberStyles.Any, culturaColombiana, out double pesoBruto) &&
                    double.TryParse(row.Cells["Canasta P. KG"].Value.ToString(), NumberStyles.Any, culturaColombiana, out double canastapkg))
                {
                    // Cálculo de la cantidad
                    double cantidad = pesoBruto - (canastas * canastapkg);
                    row.Cells["Cantidad"].Value = cantidad.ToString("N1", culturaColombiana);

                    // Cálculo del total si el precio es válido
                    if (row.Cells["Precio"].Value is string precioString)
                    {
                        // Elimina el símbolo "$" y formatea el valor
                        string valorNumerico = precioString.Replace("$", "").Trim();

                        if (decimal.TryParse(valorNumerico, NumberStyles.Any, culturaColombiana, out decimal precio))
                        {
                            double total = cantidad * (double)precio;
                            row.Cells["Total"].Value = $"${total.ToString("N2", culturaColombiana)}";
                        }
                    }
                }
            }
        }

        private void CalculateTotalSum()
        {
            // Cultura colombiana
            var culturaColombiana = new CultureInfo("es-CO");
            decimal totalSum = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["Total"].Value is string totalString)
                {
                    // Elimina el símbolo "$" y formatea el valor
                    string valorNumerico = totalString.Replace("$", "").Trim();

                    if (decimal.TryParse(valorNumerico, NumberStyles.Any, culturaColombiana, out decimal total))
                    {
                        totalSum += total;
                    }
                }
            }

            // Muestra el total formateado con la cultura colombiana
            label3.Text = $"Total: {totalSum.ToString("N2", culturaColombiana)}";
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

        private void ActualizarSaldoClienteEnContra(string nombreCliente, decimal deudaHoy, decimal abonaHoy)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            try
            {
                // Obtener el total de deudas acumuladas en la tabla Ventas
                string sumaDeudasQuery = "SELECT SUM(TotalDeuda) FROM Ventas WHERE NombreCliente = @nombreCliente";
                decimal sumaDeudas = 0;

                using (OleDbCommand command = new OleDbCommand(sumaDeudasQuery, connection))
                {
                    command.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        sumaDeudas = Convert.ToDecimal(result);
                    }
                }

                decimal totalDeudas = sumaDeudas + deudaHoy;

                // Actualizar la tabla Clientes con la suma de las deudas acumuladas
                string actualizarClienteQuery = "UPDATE Clientes SET SaldoDeuda = @saldoDeuda WHERE NombreCliente = @nombreCliente";
                using (OleDbCommand updateCommand = new OleDbCommand(actualizarClienteQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@saldoDeuda", totalDeudas);
                    updateCommand.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        //MessageBox.Show("El saldo del cliente ha sido actualizado correctamente.", "Actualizado!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        //MessageBox.Show("No se pudo actualizar el saldo del cliente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la base de datos: " + ex.Message);
            }
        }

        private void ActualizarSaldoClienteAFavor(string nombreCliente, decimal valorcomprahoy, decimal saldopendiente, decimal abona)
        {

            try
            {
                //Valida si la conexión esta abierta
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


            if (e.ColumnIndex == dataGridView1.Columns["Producto"].Index && e.RowIndex >= 0)
            {
                var productId = dataGridView1.Rows[e.RowIndex].Cells["Producto"].Value;
                var row = dataGridView1.Rows[e.RowIndex];
                var comboBoxCell = (DataGridViewComboBoxCell)row.Cells["Producto"];
                string nombreProducto = comboBoxCell.FormattedValue?.ToString();
                DataRowView selectedProduct = comboBoxCell.Value as DataRowView;
                if (selectedProduct != null)
                {
                    row.Cells["Precio"].Value = selectedProduct["precio"].ToString();
                }

                if (productId != null)
                {
                    decimal precio = GetProductPriceById(Convert.ToInt32(productId));
                    dataGridView1.Rows[e.RowIndex].Cells["Precio"].Value = $"${precio:N0}";
                }
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
            //Pendiente revisar con que fin se utiliza
            //CrearCliente(txtCliente.Text);
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();

        }
        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
            }

        }
        //private void ValidarCliente(string nombreCliente)
        //{
        //    decimal valorMostrar = 0;

        //    try
        //    {
        //        if (connection.State == ConnectionState.Closed)
        //        {
        //            connection.Open();
        //        }

        //        string query = "SELECT SaldoDeuda FROM clientes WHERE NombreCliente = @NombreCliente";
        //        using (OleDbCommand command = new OleDbCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@NombreCliente", nombreCliente);

        //            using (OleDbDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    //decimal saldoFavor = reader.GetDecimal(0);
        //                    decimal saldoDeuda = reader.GetDecimal(1);
        //                    if (saldoDeuda != 0)
        //                    {
        //                        valorMostrar = -saldoDeuda;
        //                    }
        //                }
        //                else
        //                {
        //                    CrearCliente(nombreCliente);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //    }

        //    //lblDescuento.Text = $"Descuento: $ {valorMostrar.ToString("N0")}";
        //}
        private void ValidarExistenciaCliente(string nombreCliente)
        {
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM Clientes WHERE NombreCliente = @NombreCliente";

                using (OleDbCommand checkCommand = new OleDbCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@NombreCliente", nombreCliente);

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        //MessageBox.Show("El cliente ya existe en la base de datos.");
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO Clientes (NombreCliente, SaldoDeuda) VALUES (@NombreCliente, 0)";

                        using (OleDbCommand insertCommand = new OleDbCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@NombreCliente", nombreCliente);
                            insertCommand.ExecuteNonQuery();
                            //MessageBox.Show("Cliente insertado exitosamente.");
                        }
                    }
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
            txtAbonaTransferencia.Text = string.Empty;
            btnLimpiar.Visible = false;
            txtCliente.Enabled = true;
            label3.Text = "Total: ";
            dataGridView1.Rows.Clear();
            resumenProductos.Clear();
            lblResumenVenta.Text = "No se han agredado productos a la factura";
            lblDevuelta.Text = "0";
            SiguienteConsecutivo();
        }
        public class ProductoDetalle
        {
            public string Precio { get; set; }
            public string Canastas { get; set; }
            public string PesoBruto { get; set; }
            public string Cantidad { get; set; }
            public string Total { get; set; }
        }
        private void txtAbona_TextChanged(object sender, EventArgs e)
        {
            // Verifica si el campo está vacío
            if (string.IsNullOrWhiteSpace(txtAbona.Text))
            {
                SaveButton.Enabled = false;
                lblDevuelta.Text = "0";
                return;
            }

            // Guarda la posición actual del cursor
            int cursorPosition = txtAbona.SelectionStart;

            // Elimina caracteres no numéricos temporalmente
            string rawText = new string(txtAbona.Text.Where(c => char.IsDigit(c)).ToArray());

            if (decimal.TryParse(rawText, out decimal valorPagado))
            {
                // Verifica si el valor es válido
                SaveButton.Enabled = valorPagado >= 0;

                // Aplica el formato con separadores de miles
                txtAbona.Text = valorPagado.ToString("N0");

                // Restaura la posición del cursor ajustado según los separadores de miles
                int delta = txtAbona.Text.Length - rawText.Length;
                txtAbona.SelectionStart = cursorPosition + delta;

                // Cálculo del cambio (devuelta)
                string valor1Texto = label3.Text.Replace("Total:", "").Trim();
                if (decimal.TryParse(valor1Texto, out decimal valorVenta))
                {
                    if (decimal.TryParse(txtAbonaTransferencia.Text, out decimal valorTransferencia))
                    {
                        valorPagado += valorTransferencia;
                    }

                    decimal devuelta = valorPagado - valorVenta;
                    lblDevuelta.Text = devuelta.ToString("C0");
                }
            }
            else
            {
                // Si el texto no es válido, limpia el campo
                txtAbona.Text = string.Empty;
                SaveButton.Enabled = false;
                lblDevuelta.Text = "0";
            }
        }
         private void txtAbonaTransferencia_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAbonaTransferencia.Text))
            {
                SaveButton.Enabled = false;
                lblDevuelta.Text = "0";
                return;
            }

            // Guarda la posición actual del cursor
            int cursorPosition = txtAbonaTransferencia.SelectionStart;

            // Elimina caracteres no numéricos temporalmente
            string rawText = new string(txtAbonaTransferencia.Text.Where(c => char.IsDigit(c)).ToArray());

            if (decimal.TryParse(rawText, out decimal valorPagado))
            {
                // Verifica si el valor es válido
                SaveButton.Enabled = valorPagado >= 0;

                // Aplica el formato con separadores de miles
                txtAbonaTransferencia.Text = valorPagado.ToString("N0");

                // Restaura la posición del cursor ajustado según los separadores de miles
                int delta = txtAbonaTransferencia.Text.Length - rawText.Length;
                txtAbonaTransferencia.SelectionStart = cursorPosition + delta;

                // Cálculo del cambio (devuelta)
                string valor1Texto = label3.Text.Replace("Total:", "").Trim();
                if (decimal.TryParse(valor1Texto, out decimal valorVenta))
                {
                    if (decimal.TryParse(txtAbona.Text, out decimal valorEfectivo))
                    {
                        valorPagado += valorEfectivo;
                    }

                    decimal devuelta = valorPagado - valorVenta;
                    lblDevuelta.Text = devuelta.ToString("C0");
                }
            }
            else
            {
                // Si el texto no es válido, limpia el campo
                txtAbonaTransferencia.Text = string.Empty;
                SaveButton.Enabled = false;
                lblDevuelta.Text = "0";
            }
        }
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si el usuario presiona el punto
            if (e.KeyChar == '.')
            {
                // Reemplaza el punto por una coma
                e.KeyChar = ',';
            }
        }
        private void txtAbona_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtAbonaTransferencia_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }


        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Verifica si la columna es "Canasta P. KG" o "Cantidad"
            if (dataGridView1.CurrentCell.ColumnIndex >= 0 &&
                (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Canasta P. KG" ||
                 dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "PesoBruto"))
            {
                // Obtiene el control de edición actual
                if (e.Control is TextBox textBox)
                {
                    // Elimina cualquier suscripción anterior para evitar múltiples llamadas
                    textBox.KeyPress -= TextBox_KeyPress;
                    // Agrega el evento KeyPress para manejar el reemplazo
                    textBox.KeyPress += TextBox_KeyPress;
                }
            }
        }
        private void lblAbona_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RecalcularResumenProductos();
        }
        private void RecalcularResumenProductos()
        {
            // Limpia el resumen de productos actual
            resumenProductos.Clear();

            // Recorre todas las filas del DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Ignora las filas nuevas que aún no están completas
                if (row.IsNewRow) continue;

                string producto = row.Cells["Producto"].FormattedValue?.ToString();
                string precio = row.Cells["Precio"].Value?.ToString();
                string canastas = row.Cells["Canastas"].Value?.ToString();
                string pesoBruto = row.Cells["Pesobruto"].Value?.ToString();
                string cantidad = row.Cells["Cantidad"].Value?.ToString();
                string total = row.Cells["Total"].Value?.ToString();

                if (!string.IsNullOrEmpty(producto) &&
                    !string.IsNullOrEmpty(precio) &&
                    !string.IsNullOrEmpty(canastas) &&
                    !string.IsNullOrEmpty(pesoBruto) &&
                    !string.IsNullOrEmpty(cantidad) &&
                    !string.IsNullOrEmpty(total))
                {
                    var detalle = new ProductoDetalle
                    {
                        Precio = precio,
                        Canastas = canastas,
                        PesoBruto = pesoBruto,
                        Cantidad = cantidad,
                        Total = total
                    };

                    var clave = (producto, precio);

                    if (!resumenProductos.ContainsKey(clave))
                    {
                        resumenProductos[clave] = new List<ProductoDetalle>();
                    }
                    resumenProductos[clave].Add(detalle);
                }
            }

            // Actualiza el resumen de venta
            ActualizarResumenVentaLabel();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            CultureInfo cultureColombia = new CultureInfo("es-CO");
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                try
                {
                    decimal totalCompra = 0;
                    int totalCanastas = 0;
                    double totalPesoBruto = 0;
                    decimal total = 0;
                    decimal abono = 0;
                    string cliente = txtCliente.Text;
                    decimal cambio = 0;
                    decimal deuda = 0;
                    // Conversión de texto a número
                    string valor1Texto = label3.Text.Replace("Total:$", "");
                    string reemplazovalor1Texto = valor1Texto.Replace("Total: ", "").Replace(".", "").Replace(",", "");

                    if (int.TryParse(reemplazovalor1Texto, out int numero))
                    {
                        if (numero % 100 == 0)
                        {
                            int ultimosDosDigitos = numero % 100;

                            if (ultimosDosDigitos >= 50)
                            {

                                numero = (numero / 100) * 100 + 100;
                            }
                            else
                            {

                                numero = numero / 100;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("El valor no es un número válido.");
                    }


                    string abonoEfectivo = txtAbona.Text == "" ? "0" : txtAbona.Text;
                    string abonaTransaferencia = txtAbonaTransferencia.Text == "" ? "0" : txtAbonaTransferencia.Text;

                    decimal reemplazoAbono = decimal.Parse(abonoEfectivo, cultureColombia);
                    decimal reemplazoEfectivo = decimal.Parse(abonaTransaferencia, cultureColombia);

                    // Suma los valores como decimales
                    decimal sumaPagos = reemplazoAbono + reemplazoEfectivo;

                    // Si necesitas el resultado como texto (con formato colombiano)
                    string sumaPagosTexto = sumaPagos.ToString("N0", cultureColombia);


                    //decimal saldoFavor = ObtenerSaldoFavor(txtCliente.Text);
                    decimal saldoEnContra = ObtenerSaldoEnContra(txtCliente.Text);

                    ValidarExistenciaCliente(txtCliente.Text);

                    if (decimal.TryParse(numero.ToString(), NumberStyles.Any, cultureColombia, out decimal totalValorCompra) &&
                        decimal.TryParse(sumaPagosTexto, NumberStyles.Any, cultureColombia, out abono))
                    {
                        if (abono > totalValorCompra)
                        {
                            cambio = abono - totalValorCompra;
                            //Saldar Deuda
                            SaldarDeudas(cliente, cambio);
                        }
                        else if (abono == totalValorCompra)
                        {
                            //MessageBox.Show("El pago fue completado.", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            deuda = totalValorCompra - abono;
                            MessageBox.Show($"El cliente queda debiendo: {deuda.ToString("C", cultureColombia)}", "Pendiente por Pagar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ActualizarSaldoClienteEnContra(txtCliente.Text, deuda, abono);
                        }
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
                    string insertVentaQuery = "INSERT INTO Ventas (consecutivo, nombreCliente, totalproductos, totalcanastas, totalpesobruto, totalcompra, totalabona, totaldeuda, fecha) " +
                                              "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                    using (OleDbCommand ventaCommand = new OleDbCommand(insertVentaQuery, connection))
                    {
                        ventaCommand.Parameters.AddWithValue("consecutivo", nuevoConsecutivo);
                        ventaCommand.Parameters.AddWithValue("@nombreCliente", txtCliente.Text);
                        ventaCommand.Parameters.AddWithValue("@totalproductos", 0);
                        ventaCommand.Parameters.AddWithValue("@totalcanastas", 0);
                        ventaCommand.Parameters.AddWithValue("@totalpesobruto", 0);
                        ventaCommand.Parameters.AddWithValue("@totalcompra", totalValorCompra);
                        ventaCommand.Parameters.AddWithValue("@totalabona", abono);
                        ventaCommand.Parameters.AddWithValue("@totaldeuda", deuda);
                        ventaCommand.Parameters.AddWithValue("@fecha", DateTime.Now.Date);

                        ventaCommand.ExecuteNonQuery();

                        ventaCommand.CommandText = "SELECT @@IDENTITY";
                        ventaId = Convert.ToInt32(ventaCommand.ExecuteScalar());
                    }


                    decimal valorEfectivo = string.IsNullOrWhiteSpace(txtAbona.Text) ? 0 : decimal.Parse(txtAbona.Text, cultureColombia);
                    decimal valorTransferencia = string.IsNullOrWhiteSpace(txtAbonaTransferencia.Text) ? 0 : decimal.Parse(txtAbonaTransferencia.Text, cultureColombia);
                    if (valorEfectivo > 0 && valorTransferencia <= 0)
                    {
                        string insertAbonoQuery = "INSERT INTO MovimientoVentas (VentaId, TipoPago, ValorAbono, Fecha) " +
                         "VALUES (?, ?, ?, ?)";
                        using (OleDbCommand AbonoCommand = new OleDbCommand(insertAbonoQuery, connection))
                        {
                            AbonoCommand.Parameters.AddWithValue("@VentaId", ventaId);
                            AbonoCommand.Parameters.AddWithValue("@TipoPago", "Efectivo");
                            AbonoCommand.Parameters.AddWithValue("@ValorAbono", valorEfectivo);
                            AbonoCommand.Parameters.AddWithValue("@Fecha", DateTime.Now.Date);

                            AbonoCommand.ExecuteNonQuery();
                        }
                    }
                    else if (valorTransferencia > 0 && valorEfectivo <= 0)
                    {
                        string insertAbonoQuery = "INSERT INTO MovimientoVentas (VentaId, TipoPago, ValorAbono, Fecha) " +
                            "VALUES (?, ?, ?, ?)";
                        using (OleDbCommand AbonoCommand = new OleDbCommand(insertAbonoQuery, connection))
                        {
                            AbonoCommand.Parameters.AddWithValue("@VentaId", ventaId);
                            AbonoCommand.Parameters.AddWithValue("@TipoPago", "Transferencia");
                            AbonoCommand.Parameters.AddWithValue("@ValorAbono", valorTransferencia);
                            AbonoCommand.Parameters.AddWithValue("@Fecha", DateTime.Now.Date);

                            AbonoCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertAbonoQuery = "INSERT INTO MovimientoVentas (VentaId, TipoPago, ValorAbono, Fecha) " +
                                             "VALUES (?, ?, ?, ?)";
                        using (OleDbCommand AbonoCommand = new OleDbCommand(insertAbonoQuery, connection))
                        {
                            AbonoCommand.Parameters.AddWithValue("@VentaId", ventaId);
                            AbonoCommand.Parameters.AddWithValue("@TipoPago", "Efectivo");
                            AbonoCommand.Parameters.AddWithValue("@ValorAbono", valorEfectivo);
                            AbonoCommand.Parameters.AddWithValue("@Fecha", DateTime.Now.Date);

                            AbonoCommand.ExecuteNonQuery();
                        }

                        {
                            string insertAbonoQuery2 = "INSERT INTO MovimientoVentas (VentaId, TipoPago, ValorAbono, Fecha) " +
                                             "VALUES (?, ?, ?, ?)";
                            using (OleDbCommand AbonoCommand = new OleDbCommand(insertAbonoQuery2, connection))
                            {
                                AbonoCommand.Parameters.AddWithValue("@VentaId", ventaId);
                                AbonoCommand.Parameters.AddWithValue("@TipoPago", "Transferencia");
                                AbonoCommand.Parameters.AddWithValue("@ValorAbono", valorTransferencia);
                                AbonoCommand.Parameters.AddWithValue("@Fecha", DateTime.Now.Date);

                                AbonoCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    //Cierre Movimientos

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        bool isSaved = row.Cells["IsSaved"].Value != null && Convert.ToBoolean(row.Cells["IsSaved"].Value);
                        if (!isSaved)
                        {
                            decimal pesocanastaKG = Convert.ToDecimal(row.Cells["Canasta P. KG"].Value);
                            int idProducto = Convert.ToInt32(row.Cells["Producto"].Value);

                            decimal precio;
                            if (row.Cells["Precio"].Value is string precioString)
                            {
                                string valorNumerico = precioString.Replace("$", "").Trim();
                                if (!decimal.TryParse(valorNumerico, out precio))
                                {
                                    MessageBox.Show($"Error al convertir el precio en la fila {row.Index + 1}.");
                                    continue;
                                }
                            }
                            else
                            {
                                precio = Convert.ToDecimal(row.Cells["Precio"].Value);
                            }

                            int canastas = Convert.ToInt32(row.Cells["Canastas"].Value);
                            double pesoBruto = Convert.ToDouble(row.Cells["PesoBruto"].Value);
                            double cantidad = Convert.ToDouble(row.Cells["Cantidad"].Value);

                            decimal valortotal;
                            if (row.Cells["Total"].Value is string totalString)
                            {
                                string valorNumericoTotal = totalString.Replace("$", "").Trim();
                                if (!decimal.TryParse(valorNumericoTotal, out valortotal))
                                {
                                    MessageBox.Show($"Error al convertir el total en la fila {row.Index + 1}.");
                                    continue;
                                }
                            }
                            else
                            {
                                valortotal = Convert.ToDecimal(row.Cells["Total"].Value);
                            }

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
                    ImprimirFacturaCompra(ventaId);
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
            catch (Exception ex)
            {
                MessageBox.Show("Error en la conexión: " + ex.Message);
            }
        }

        private void SaldarDeudas(string cliente, decimal montoAbono)
        {
            CultureInfo cultureColombia = new CultureInfo("es-CO");
            decimal abonoAplicado = 0;
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                // Obtener las deudas del cliente ordenadas por fecha (más antiguas primero)
                string obtenerDeudasQuery = @"
                SELECT VentaId, TotalCompra, TotalAbona, TotalDeuda AS DeudaPendiente 
                FROM Ventas 
                WHERE NombreCliente = ? AND TotalDeuda > 0 
                ORDER BY Fecha ASC";

                List<(int VentaId, decimal DeudaPendiente)> deudas = new List<(int, decimal)>();
                using (var command = new OleDbCommand(obtenerDeudasQuery, connection))
                {
                    command.Parameters.AddWithValue("?", cliente);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ventaId = reader.GetInt32(0);
                            decimal deudaPendiente = reader.GetDecimal(3);
                            deudas.Add((ventaId, deudaPendiente));
                        }
                    }
                }

                decimal montoRestante = montoAbono;

                // Aplicar el abono a las deudas
                foreach (var (ventaId, deudaPendiente) in deudas)
                {
                    if (montoRestante <= 0)
                        break;

                    abonoAplicado = Math.Min(montoRestante, deudaPendiente);
                    montoRestante -= abonoAplicado;

                    // Actualizar el registro de la deuda
                    string actualizarDeudaQuery = "UPDATE Ventas SET TotalAbona = TotalAbona + ? WHERE VentaId = ?";
                    using (var updateCommand = new OleDbCommand(actualizarDeudaQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", abonoAplicado);
                        updateCommand.Parameters.AddWithValue("?", ventaId);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Actualizar el valor total de la deuda
                    string actualizarDeudaPendienteQuery = "UPDATE Ventas SET TotalDeuda = (TotalDeuda - ?) WHERE VentaId = ?";
                    using (var updateCommand = new OleDbCommand(actualizarDeudaPendienteQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", abonoAplicado);
                        updateCommand.Parameters.AddWithValue("?", ventaId);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Registrar el movimiento del abono
                    string registrarMovimientoQuery = "INSERT INTO MovimientoVentas (VentaId, TipoPago, ValorAbono, Fecha) VALUES (?, ?, ?, ?)";
                    using (var movimientoCommand = new OleDbCommand(registrarMovimientoQuery, connection))
                    {
                        movimientoCommand.Parameters.AddWithValue("?", ventaId);
                        movimientoCommand.Parameters.AddWithValue("?", "Abono a Deuda");
                        movimientoCommand.Parameters.AddWithValue("?", abonoAplicado);
                        movimientoCommand.Parameters.AddWithValue("?", DateTime.Now.Date);
                        movimientoCommand.ExecuteNonQuery();
                    }
                }

                // Si queda algún saldo restante, notificar o manejar el exceso
                if (montoRestante > 0)
                {
                    MessageBox.Show($"El cliente tiene un saldo a favor de: {montoRestante.ToString("C", cultureColombia)}",
                                    "Saldo a favor",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    //MessageBox.Show("Las deudas han sido saldadas exitosamente.",
                    //                "Deudas Saldadas",
                    //                MessageBoxButtons.OK,
                    //                MessageBoxIcon.Information);
                }

                // Verificar si el cliente tiene deudas pendientes
                string verificarDeudasQuery = @"
                SELECT COUNT(*) 
                FROM Ventas 
                WHERE NombreCliente = ? AND TotalDeuda > 0";

                int deudasPendientes = 0;
                using (var command = new OleDbCommand(verificarDeudasQuery, connection))
                {
                    command.Parameters.AddWithValue("?", cliente);
                    deudasPendientes = Convert.ToInt32(command.ExecuteScalar());
                }

                // Si no tiene deudas pendientes, actualizar SaldoDeuda en la tabla Clientes a 0
                if (deudasPendientes == 0)
                {
                    string actualizarSaldoClienteQuery = "UPDATE Clientes SET SaldoDeuda = 0 WHERE NombreCliente = ?";
                    using (var updateCommand = new OleDbCommand(actualizarSaldoClienteQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", cliente);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    string actualizarSaldoClienteQuery = "UPDATE Clientes SET SaldoDeuda = (SaldoDeuda - ?) WHERE NombreCliente = ?";
                    using (var updateCommand = new OleDbCommand(actualizarSaldoClienteQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", abonoAplicado);
                        updateCommand.Parameters.AddWithValue("?", cliente);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar las deudas: " + ex.Message);
            } 
        }
        public void ImprimirFacturaCompra(int ventaId)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            decimal totalVenta = 0;
            decimal totalPeso = 0;
            string query = @"
            SELECT cl.NombreCliente, p.Nombre, dv.Precio, SUM(dv.Cantidad) AS TotalPesoBruto, SUM(dv.ValorTotal) AS TotalValorTotal, v.Consecutivo, v.Fecha 
            FROM ((DetalleVentas dv
            INNER JOIN Productos p ON dv.ProductoId = p.Id)
            INNER JOIN Ventas v ON dv.VentaId = v.VentaId)
            INNER JOIN Clientes cl ON CStr(v.NombreCliente) = CStr(cl.NombreCliente)
            WHERE dv.VentaId = ?
            GROUP BY cl.NombreCliente, p.Nombre, dv.Precio, v.Consecutivo, v.Fecha"
            ;

            _detalleventas = new List<DetalleVenta>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.Add("?", OleDbType.Integer).Value = ventaId;

                    try
                    {
                        connection.Open();
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                var detalle = new DetalleVenta();
                                detalle.Nombre = reader.GetString(1);
                                detalle.Precio = Convert.ToDecimal(reader["Precio"]); // Conversión manual a decimal
                                detalle.PesoBruto = Convert.ToDecimal(reader["TotalPesoBruto"]); // Conversión a entero para PesoBruto
                                detalle.ValorTotal = Convert.ToDecimal(reader["TotalValorTotal"]); // Conversión manual a decimal
                                _consecutivo = Convert.ToString(reader["Consecutivo"]);
                                _fecha = Convert.ToString(reader["Fecha"]);
                                _nombreCliente = Convert.ToString(reader["NombreCliente"]);

                                if (DateTime.TryParse(_fecha, out DateTime fechaParsed))
                                {
                                    _fecha = fechaParsed.ToString("dd/MM/yyyy"); // Formatea solo la fecha
                                }
                                _detalleventas.Add(detalle);
                                totalVenta += Convert.ToDecimal(detalle.ValorTotal);
                                totalPeso += detalle.PesoBruto;
                            }
                            _totalvalorventa = $"${totalVenta.ToString("N0")}";
                            _totalpeso = totalPeso.ToString("N1");
                            printPreviewDialog.ShowDialog();
                            //printDocument.Print();
                            //MessageBox.Show("Se Imprimió correctamente la Factura de Venta.", "Exitoso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //ImprimirDocumento();
                            //
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            PrintDocument printDocument = (PrintDocument)sender;

            float widthInInches = 7.2f / 2.54f;
            float heightInInches = 10.99f / 2.54f;
            float topeInChes = 20.99f / 2.54f;

            PaperSize customPaperSize = new PaperSize("CustomSize", (int)(widthInInches * 100), (int)(heightInInches * 100));
            printDocument.DefaultPageSettings.PaperSize = customPaperSize;
        }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 9);
            Brush brush = Brushes.Black;
            int startX = 10;
            int startY = 10;
            int offsetY = 25;

            int pageWidth = e.PageBounds.Width;
            int pageHeight = e.PageBounds.Height;

            Image logo = Image.FromFile("Images/verdum-logo.png");
            if (logo != null)
            {
                int logoWidth = 80;
                int logoX = (pageWidth - logoWidth) / 2;
                g.DrawImage(logo, logoX, startY, logoWidth, 80);
                offsetY += 60;
            }

            string prefacturaVenta = "PREFACTURA DE VENTA";
            float prefacturaVentaWidth = g.MeasureString(prefacturaVenta, new Font("Arial", 8, FontStyle.Bold)).Width;
            float prefacturaVentaX = (pageWidth - prefacturaVentaWidth) / 2;
            g.DrawString(prefacturaVenta, new Font("Arial", 8, FontStyle.Bold), brush, prefacturaVentaX, startY + offsetY);
            offsetY += 15;

            string medellinColombia = "MEDELLIN - COLOMBIA";
            float medellinColombiaWidth = g.MeasureString(medellinColombia, new Font("Arial", 8, FontStyle.Bold)).Width;
            float medellinColombiaX = (pageWidth - medellinColombiaWidth) / 2;
            g.DrawString(medellinColombia, new Font("Arial", 8, FontStyle.Bold), brush, medellinColombiaX, startY + offsetY);
            offsetY += 15;

            string direccion = "DIRECCION: PLAZA MAYORISTA DE ANTIOQUIA";
            float direccionWidth = g.MeasureString(direccion, new Font("Arial", 8, FontStyle.Bold)).Width;
            float direccionX = (pageWidth - direccionWidth) / 2;
            g.DrawString(direccion, new Font("Arial", 8, FontStyle.Bold), brush, direccionX, startY + offsetY);
            offsetY += 20;

            g.DrawString($"PREFACTURA No: {_consecutivo}", new Font("Arial", 9, FontStyle.Bold), brush, direccionX, startY + offsetY);
            offsetY += 15;

            g.DrawString($"FECHA: {_fecha}", new Font("Arial", 9, FontStyle.Bold), brush, direccionX, startY + offsetY);
            offsetY += 20;

            string comercializadoraText = "COMERCIALIZADORA VERDUM SAS";
            float comercializadoraTextWidth = g.MeasureString(comercializadoraText, new Font("Arial", 9, FontStyle.Bold)).Width;
            float comercializadoraTextX = (pageWidth - comercializadoraTextWidth) / 2;
            g.DrawString(comercializadoraText, new Font("Arial", 9, FontStyle.Bold), brush, comercializadoraTextX, startY + offsetY);
            offsetY += 20;

            g.DrawString($"CLIENTE: {_nombreCliente}", new Font("Arial", 9, FontStyle.Bold), brush, direccionX, startY + offsetY);
            offsetY += 15;

            g.DrawString("PRODUCTO", new Font("Arial", 9, FontStyle.Bold), brush, direccionX, startY + offsetY);
            g.DrawString("PESO", new Font("Arial", 9, FontStyle.Bold), brush, startX + 80, startY + offsetY);
            g.DrawString("PRECIO", new Font("Arial", 9, FontStyle.Bold), brush, startX + 130, startY + offsetY);
            g.DrawString("VALOR", new Font("Arial", 9, FontStyle.Bold), brush, startX + 200, startY + offsetY);
            offsetY += 15;

            foreach (var detalle in _detalleventas)
            {
                g.DrawString(detalle.Nombre, font, brush, direccionX, startY + offsetY);
                g.DrawString(detalle.PesoBruto.ToString("N1"), font, brush, startX + 80, startY + offsetY);
                g.DrawString($"${detalle.Precio.ToString("N0")}", font, brush, startX + 130, startY + offsetY);
                g.DrawString($"${detalle.ValorTotal.ToString("N0")}", font, brush, startX + 200, startY + offsetY);
                offsetY += 15;

            }
            offsetY += 10;

            string totalLabel = "TOTAL VENTA:";
            g.DrawString(totalLabel, new Font("Arial", 9, FontStyle.Bold), brush, direccionX, startY + offsetY);
            float totalLabelWidth = g.MeasureString(totalLabel, new Font("Arial", 14, FontStyle.Bold)).Width;

            string totalPesoValue = _totalpeso;
            string totalVentaText = _totalvalorventa;

            float spaceWidth = e.PageBounds.Width - totalLabelWidth - g.MeasureString(totalPesoValue, font).Width - g.MeasureString(totalVentaText, font).Width - 440;
            float spaceWidth2 = e.PageBounds.Width - totalLabelWidth - g.MeasureString(totalPesoValue, font).Width - g.MeasureString(totalVentaText, font).Width - 300;
            g.DrawString(totalPesoValue, font, brush, startX + 95, startY + offsetY);
            g.DrawString(totalVentaText, font, brush, startX + 170 + g.MeasureString(totalPesoValue, font).Width, startY + offsetY);
            offsetY += 15;

            g.DrawString($"Fecha Generación Prefactura:", new Font("Arial", 9, FontStyle.Bold), brush, comercializadoraTextX + 20, startY + offsetY);
            offsetY += 15;
            g.DrawString($"{DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm")}", new Font("Arial", 9, FontStyle.Bold), brush, comercializadoraTextX + 15, startY + offsetY);
        }
    }
}
