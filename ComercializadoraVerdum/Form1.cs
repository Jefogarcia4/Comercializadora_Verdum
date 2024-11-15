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


namespace ComercializadoraVerdum
{
    public partial class FrmHome : Form
    {
        private OleDbConnection connection;
        private Historial _historial;
        private int consecutivo = 0;
        private DateTime fechaActual;
        private IConfigurationRoot configuration;
        private Dictionary<(string producto, string precio), List<ProductoDetalle>> resumenProductos = new Dictionary<(string producto, string precio), List<ProductoDetalle>>();



        public FrmHome(Historial historial)
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
            _historial = historial;

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

                ActualizarResumenVentaLabel();
            }
        }

        private void ActualizarResumenVentaLabel()
        {
            StringBuilder resumenVenta = new StringBuilder();
            var cultura = new CultureInfo("es-CO");
            resumenVenta.AppendLine("Producto    | Precio    | Canastas   | Cantidad   | Valor");
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
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
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

        private void SaveButton_Click(object sender, EventArgs e)
        {

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                if (string.IsNullOrWhiteSpace(txtCliente.Text) || string.IsNullOrWhiteSpace(txtAbona.Text))
                {
                    MessageBox.Show("Por favor, complete los campos Nombre Cliente y valor pagado.", "Campos requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                }
                else
                {
                    try
                    {
                        decimal totalCompra = 0;
                        int totalCanastas = 0;
                        double totalPesoBruto = 0;
                        decimal total = 0;
                        decimal abono = 0;
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


                        string abonoTexto = txtAbona.Text;
                        string reemplazoabonoTexto = abonoTexto.Replace(".", "").Replace(",", "");

                        CultureInfo cultureColombia = new CultureInfo("es-CO");

                        decimal saldoFavor = ObtenerSaldoFavor(txtCliente.Text);
                        decimal saldoEnContra = ObtenerSaldoEnContra(txtCliente.Text);

                        if (decimal.TryParse(numero.ToString(), NumberStyles.Any, cultureColombia, out decimal totalValorCompra) &&
                            decimal.TryParse(reemplazoabonoTexto, NumberStyles.Any, cultureColombia, out abono))
                        {
                            if (abono > totalValorCompra)
                            {
                                decimal cambio = abono - totalValorCompra;
                                MessageBox.Show($"Devolver al cliente: {cambio.ToString("C", cultureColombia)}", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else if (abono == totalValorCompra)
                            {
                                //MessageBox.Show("El pago fue completado.", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                decimal deuda = totalValorCompra - abono;
                                MessageBox.Show($"El cliente queda debiendo: {deuda.ToString("C", cultureColombia)}", "Pendiente por Pagar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ActualizarSaldoClienteEnContra(txtCliente.Text, totalValorCompra, deuda, abono);
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
                        string insertVentaQuery = "INSERT INTO Ventas (consecutivo, nombreCliente, totalproductos, totalcanastas, totalpesobruto, totalcompra, descuento, totalabona, totalpagar, fecha) " +
                                                  "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?,?)";
                        using (OleDbCommand ventaCommand = new OleDbCommand(insertVentaQuery, connection))
                        {
                            ventaCommand.Parameters.AddWithValue("consecutivo", nuevoConsecutivo);
                            ventaCommand.Parameters.AddWithValue("@nombreCliente", txtCliente.Text);
                            ventaCommand.Parameters.AddWithValue("@totalproductos", 0);
                            ventaCommand.Parameters.AddWithValue("@totalcanastas", 0);
                            ventaCommand.Parameters.AddWithValue("@totalpesobruto", 0);
                            ventaCommand.Parameters.AddWithValue("@totalcompra", totalValorCompra);
                            ventaCommand.Parameters.AddWithValue("@descuento", 0);
                            ventaCommand.Parameters.AddWithValue("@totalabona", 0);
                            ventaCommand.Parameters.AddWithValue("@totalpagar", abono);
                            ventaCommand.Parameters.AddWithValue("@fecha", DateTime.Now.Date);

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
                        _historial.ImprimirFacturaCompra(ventaId);
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
                Application.Exit();
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
            ValidarCliente(txtCliente.Text);
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

            //lblDescuento.Text = $"Descuento: $ {valorMostrar.ToString("N0")}";
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
            dataGridView1.Rows.Clear();
            resumenProductos.Clear();
            lblResumenVenta.Text = "No se han agredado productos a la factura";
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
            if (txtAbona.Text != string.Empty)
            {
                decimal valorPagado = decimal.Parse(txtAbona.Text);
                if (valorPagado < 0)
                {
                    SaveButton.Enabled = false;
                }
                else
                {
                    SaveButton.Enabled = true;
                }

            }
            else
            {
                SaveButton.Enabled = false;
            }

        }

        private void txtAbona_KeyPress(object sender, KeyPressEventArgs e)
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

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si el usuario presiona el punto
            if (e.KeyChar == '.')
            {
                // Reemplaza el punto por una coma
                e.KeyChar = ',';
            }
        }
    }
}
