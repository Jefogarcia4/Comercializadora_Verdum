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
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Drawing.Printing;
using System.Globalization;

namespace ComercializadoraVerdum
{
    public partial class Historial : Form
    {

        private OleDbConnection connection;
        private IConfigurationRoot configuration;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        private string _consecutivo, _fecha, _nombreCliente, _totalvalorventa, _totalpeso;
        private List<DetalleVenta> _detalleventas;
        private int offsetY = 10; 
        private bool isNewPage = true;
        private Dictionary<string, decimal> saldoDeudasCache = new Dictionary<string, decimal>();
        public Historial()
        {
            this.Icon = new Icon("Images/verdum-logo-icono.ico");
            InitializeComponent();
            InitializeDatabaseConnection();
            InitializeDataGridView();
            SetButtonImageFromUrl();
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
        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Add("VentaId", "VentaId");
            dataGridView1.Columns["VentaId"].Visible = false;
            dataGridView1.Columns.Add("Fecha", "Fecha");
            dataGridView1.Columns.Add("Consecutivo", "Consecutivo");
            dataGridView1.Columns.Add("NombreCliente", "Nombre Cliente");
            dataGridView1.Columns.Add("TotalProductos", "Total Productos");
            dataGridView1.Columns.Add("TotalCanastas", "Total Canastas");
            dataGridView1.Columns.Add("TotalPesoBruto", "Total Peso Bruto");
            dataGridView1.Columns.Add("TotalCompra", "Total Compra");
            //dataGridView1.Columns.Add("Descuento", "Descuento");
            //dataGridView1.Columns.Add("TotalAbona", "TotalAbona");
            dataGridView1.Columns.Add("TotalPagar", "Total Pagar");

            DataGridViewButtonColumn printButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Imprimir",
                Text = "Imprimir",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(printButtonColumn);

            DataGridViewButtonColumn printDetalleCompra = new DataGridViewButtonColumn
            {
                Name = "Detalle Venta",
                Text = "Detalle Venta",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(printDetalleCompra);

            DataGridViewButtonColumn saldarDeudaPendiente = new DataGridViewButtonColumn
            {
                Name = "Saldar Deuda",
                Text = "Saldar Deuda",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(saldarDeudaPendiente);

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnVolver, "Cerrar y volver al formulario anterior");
            toolTip.SetToolTip(btnRefrescar, "Refrescar información");
            this.Controls.Add(btnRefrescar);
            this.Controls.Add(btnVolver);
            printDocument.BeginPrint += new PrintEventHandler(PrintDocument_BeginPrint);
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
            printPreviewDialog.Document = printDocument;
            //dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            btnFiltrar.Click += BtnFiltrar_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;
            this.Controls.Add(datePickerStart);
            this.Controls.Add(txtCliente);
            this.Controls.Add(btnFiltrar);
            this.Controls.Add(dataGridView1);

            LoadData();
        }

        //private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (dataGridView1.Columns[e.ColumnIndex].Name == "Saldar Deuda")
        //    {
        //        string nombreCliente = dataGridView1.Rows[e.RowIndex].Cells["nombreCliente"].Value.ToString();

        //        if (!saldoDeudasCache.ContainsKey(nombreCliente))
        //        {
        //            decimal saldoDeuda = ObtenerSaldoDeuda(nombreCliente);
        //            saldoDeudasCache[nombreCliente] = saldoDeuda;
        //        }

        //        decimal saldo = saldoDeudasCache[nombreCliente];

        //        e.Value = saldo > 0 ? "Saldar Deuda" : "";  
        //    }
        //}
        private decimal ObtenerSaldoDeuda(string nombreCliente)
        {
            decimal saldoDeuda = 0;
            try
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT SaldoDeuda FROM Clientes WHERE NombreCliente = ?";
                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", nombreCliente);
                        saldoDeuda = (decimal)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener saldoDeuda: {ex.Message}");
            }
            return saldoDeuda;
        }
        private void LoadData(string filterQuery = "")
        {
            try
            {
                var colombianCulture = new CultureInfo("es-CO");

                connection.Open();
                string query = "SELECT * FROM Ventas";

                if (!string.IsNullOrEmpty(filterQuery))
                {
                    query += " WHERE " + filterQuery;
                    query += " ORDER BY Fecha";
                }

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                DataTable ventasTable = new DataTable();
                adapter.Fill(ventasTable);

                dataGridView1.Rows.Clear();

                foreach (DataRow row in ventasTable.Rows)
                {
                    string totalPesoBruto = row["TotalPesoBruto"] != DBNull.Value ? Convert.ToDecimal(row["TotalPesoBruto"]).ToString("N2", colombianCulture) : "0";
                    string totalCompra = row["TotalCompra"] != DBNull.Value ? Convert.ToDecimal(row["TotalCompra"]).ToString("N2", colombianCulture) : "0";
                    //string descuento = row["Descuento"] != DBNull.Value ? Convert.ToDecimal(row["Descuento"]).ToString("N2", colombianCulture) : "0";
                    //string totalAbona = row["TotalAbona"] != DBNull.Value ? Convert.ToDecimal(row["TotalAbona"]).ToString("N2", colombianCulture) : "0";
                    string totalPagar = row["TotalPagar"] != DBNull.Value ? Convert.ToDecimal(row["TotalPagar"]).ToString("N2", colombianCulture) : "0";

                    dataGridView1.Rows.Add(
                        row["VentaId"] != DBNull.Value ? Convert.ToInt32(row["VentaId"]) : 0,
                        row["Fecha"] != DBNull.Value ? Convert.ToDateTime(row["Fecha"]).ToShortDateString() : "",
                        row["Consecutivo"] != DBNull.Value ? row["Consecutivo"].ToString() : "",
                        row["NombreCliente"] != DBNull.Value ? row["NombreCliente"].ToString() : "",
                        row["TotalProductos"] != DBNull.Value ? row["TotalProductos"].ToString() : "0",
                        row["TotalCanastas"] != DBNull.Value ? row["TotalCanastas"].ToString() : "0",
                        totalPesoBruto,
                        totalCompra,
                        //descuento,
                        //totalAbona,
                        totalPagar
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            if (datePickerStart.Value == null && string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Por favor, seleccione una Fecha o ingrese un Cliente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime selectedDate = datePickerStart.Value;
            string cliente = txtCliente.Text;

            string filterQuery = "1=1";
            if (datePickerStart.Value != null)
            {
                filterQuery += $" AND Fecha = #{selectedDate:MM-dd-yyyy}#";
            }
            if (!string.IsNullOrWhiteSpace(cliente))
            {
                filterQuery += $" AND NombreCliente = '{cliente}'";
            }

            LoadData(filterQuery);
        }
        private void ConsultarDatos(DateTime fecha, string cliente)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM Ventas " +
                                   "WHERE Fecha = @Fecha AND NombreCliente LIKE @Cliente";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Fecha", fecha.Date);
                        command.Parameters.AddWithValue("@Cliente", "%" + cliente + "%");

                        OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al consultar la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Imprimir")
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    int ventaId = Convert.ToInt32(row.Cells["VentaId"].Value);
                    _fecha = row.Cells["Fecha"].Value.ToString();
                    _consecutivo = row.Cells["Consecutivo"].Value.ToString();
                    _nombreCliente = row.Cells["NombreCliente"].Value.ToString();
                    ImprimirFacturaCompra(ventaId);
                }
                if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Detalle Venta")
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    int ventaId = Convert.ToInt32(row.Cells["VentaId"].Value);
                    ObtenerDetallesVenta(ventaId);
                }
                if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Saldar Deuda")
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    int ventaId = Convert.ToInt32(row.Cells["VentaId"].Value);
                    string nombreCliente = row.Cells["NombreCliente"].Value.ToString();
                    SaldarDeuda(nombreCliente, ventaId);
                }
            }
        }
        private void SaldarDeuda(string nombreCliente, int ventaId)
        {
            var colombianCulture = new CultureInfo("es-CO");

            decimal saldoDeuda = ObtenerSaldoDeuda(nombreCliente);

            if (saldoDeuda <= 0)
            {
                MessageBox.Show($"El cliente: {nombreCliente} se encuentra al día. No tiene deuda pendiente.", "Sin Pendientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string saldoPendiente = saldoDeuda.ToString("N2", colombianCulture);
                DialogResult result = MessageBox.Show(
                    $"Valor de la deuda: ${saldoPendiente}. Nombre Cliente: {nombreCliente}. ¿Deseas saldar la deuda pendiente?",
                    "Saldar Deuda",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaldarDeudaCliente(nombreCliente, saldoDeuda, ventaId);
                }
            }
        }
        private void SaldarDeudaCliente(string nombreCliente, decimal saldo, int ventaId)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                using (var connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Clientes SET SaldoDeuda = 0 WHERE NombreCliente = ?";
                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", nombreCliente);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show($"La deuda de {nombreCliente} ha sido saldada exitosamente.");
                        }
                        else
                        {
                            //MessageBox.Show("Error al saldar la deuda. Por favor, intenta nuevamente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    string query2 = "UPDATE Ventas SET TotalPagar = TotalCompra WHERE VentaId = ?";
                    using (var command = new OleDbCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("?", ventaId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"La deuda del Cliente: {nombreCliente} ha sido saldada exitosamente.", "Exitoso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Error al saldar la deuda. Por favor, intenta nuevamente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al saldar la deuda: {ex.Message}");
            }
        }
        public void ImprimirFacturaCompra(int ventaId)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            decimal totalVenta = 0;
            decimal totalPeso = 0;
            string query = @"
                    SELECT p.Nombre, dv.Precio, SUM(dv.Cantidad) AS TotalPesoBruto, SUM(dv.ValorTotal) AS TotalValorTotal
                    FROM ((DetalleVentas dv
                    INNER JOIN Productos p ON dv.ProductoId = p.Id)
                    INNER JOIN Ventas v ON dv.VentaId = v.VentaId)
                    INNER JOIN Clientes cl ON CStr(v.NombreCliente) = CStr(cl.NombreCliente)
                    WHERE dv.VentaId = ?
                    GROUP BY p.Nombre, dv.Precio";

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
                                detalle.Nombre = reader.GetString(0);
                                detalle.Precio = Convert.ToDecimal(reader["Precio"]); // Conversión manual a decimal
                                detalle.PesoBruto = Convert.ToDecimal(reader["TotalPesoBruto"]); // Conversión a entero para PesoBruto
                                detalle.ValorTotal = Convert.ToDecimal(reader["TotalValorTotal"]); // Conversión manual a decimal
                                
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
        private void ObtenerDetallesVenta(int ventaId)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            string query = @"
            SELECT dv.DetalleVentaId, p.Nombre, dv.Precio, dv.Canastas, dv.PesoBruto, dv.Cantidad, dv.ValorTotal, cl.SaldoFavor, cl.SaldoDeuda
            FROM ((DetalleVentas dv
            INNER JOIN Productos p ON dv.ProductoId = p.Id)
            INNER JOIN Ventas v ON dv.VentaId = v.VentaId)
            INNER JOIN Clientes cl ON CStr(v.NombreCliente) = CStr(cl.NombreCliente)
            WHERE dv.VentaId = ?";

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
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            MostrarDetalles(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
        private void MostrarDetalles(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron detalles para esta venta.", "Detalles de la Venta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string nombreComercializadora = "COMERCIALIZADORA VERDUM";
            string mensaje = $"                       {nombreComercializadora}\n" +
                             "-------------------------------------------------------------------------------\n";

            var colombianCulture = new CultureInfo("es-CO");

            var resumenProductos = new Dictionary<string, List<(decimal Precio, decimal Peso, decimal ValorTotal)>>();

            foreach (DataRow row in dt.Rows)
            {
                string nombreProducto = row["Nombre"].ToString();
                decimal precio = Convert.ToDecimal(row["Precio"]);
                decimal peso = Convert.ToDecimal(row["PesoBruto"]);
                decimal valorTotal = Convert.ToDecimal(row["ValorTotal"]);
                decimal peso_neto = Convert.ToDecimal(row["Cantidad"]);
                string precioFormateado = precio.ToString("N2", colombianCulture);
                string pesoFormateado = peso.ToString("N1", colombianCulture);
                string valorTotalFormateado = valorTotal.ToString("N2", colombianCulture);

                mensaje += $"Nombre Producto: {nombreProducto}\n" +
                           $"Precio: {precioFormateado}\n" +
                           $"Total Canastas: {row["Canastas"]}\n" +
                           $"Peso Bruto: {pesoFormateado}\n" +
                           $"Cantidad: {row["Cantidad"]}\n" +
                           $"Valor Total: {valorTotalFormateado}\n" +
                           "-------------------------------------------------------------------------------\n";

                if (!resumenProductos.ContainsKey(nombreProducto))
                {
                    resumenProductos[nombreProducto] = new List<(decimal Precio, decimal Peso, decimal ValorTotal)>();
                }
                resumenProductos[nombreProducto].Add((precio, peso_neto, valorTotal));
            }

            mensaje += "\n";
            mensaje += "                                      RESUMEN:\n";
            mensaje += "-------------------------------------------------------------------------------\n";

            foreach (var producto in resumenProductos)
            {
                string nombreProducto = producto.Key;
                var detalles = producto.Value;

                mensaje += $"Producto: {nombreProducto}\n";
                foreach (var detalle in detalles)
                {
                    string precioFormateado = detalle.Precio.ToString("N2", colombianCulture);
                    string pesoFormateado = detalle.Peso.ToString("N1", colombianCulture);
                    string valorTotalFormateado = detalle.ValorTotal.ToString("N2", colombianCulture);

                    mensaje += $"Precio: {precioFormateado}, Peso: {pesoFormateado}, Valor Total: {valorTotalFormateado}\n";
                }
                mensaje += "-------------------------------------------------------------------------------\n";
            }

            MessageBox.Show(mensaje, "Detalles de la Venta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void SetButtonImageFromUrl()
        {
            try
            {
                string imageUrl = "https://img.icons8.com/material-two-tone/16/refresh.png";
                string back = "https://img.icons8.com/material-two-tone/16/return.png";

                using (WebClient webClient = new WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(imageUrl);
                    byte[] imageBack = webClient.DownloadData(back);

                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        Image image = Image.FromStream(ms);
                        btnRefrescar.Image = image;
                        btnRefrescar.ImageAlign = ContentAlignment.MiddleLeft;
                    }

                    using (var rt = new System.IO.MemoryStream(imageBack))
                    {
                        Image imageback = Image.FromStream(rt);
                        btnVolver.Image = imageback;
                        btnVolver.ImageAlign = ContentAlignment.MiddleLeft;
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al descargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Historial_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1096, 589);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            LoadData();
            txtCliente.Text = "";
            datePickerStart.Value = DateTime.Now;
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex <= 10)
                {
                    MessageBox.Show("Solo se permite Imprimir o ver el Detalle de una Venta.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string prefacturaVenta = "PREFECTURA DE VENTA";
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
        private void ImprimirDocumento()
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument,
                UseEXDialog = true
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                if (printDialog.PrinterSettings.PrinterName == "Microsoft Print to PDF")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "PDF Files (*.pdf)|*.pdf",
                        Title = "Guardar como PDF",
                        FileName = $"FacturaDeVenta_{DateTime.Now.ToString("yyyy-MM-dd")}"
                    };

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                        printDocument.PrinterSettings.PrintFileName = saveFileDialog.FileName;
                        printDocument.PrinterSettings.PrintToFile = true;
                        printDocument.Print();
                    }
                    MessageBox.Show("Se exportó correctamente la Factura de Venta.", "Exitoso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    printDocument.Print();
                    MessageBox.Show("Se exportó correctamente la Factura de Venta.", "Exitoso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        public class DetalleVenta
        {
            public int DetalleVentaId { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public decimal PesoBruto { get; set; }
            public decimal ValorTotal { get; set; }
        }
    }
}
