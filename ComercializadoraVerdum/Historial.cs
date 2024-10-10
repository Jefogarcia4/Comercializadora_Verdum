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

namespace ComercializadoraVerdum
{
    public partial class Historial : Form
    {
        private OleDbConnection connection;
        private IConfigurationRoot configuration;
        public Historial()
        {
            this.Icon = new Icon("Icons/facturacion-color.ico");
            this.Width = 927;
            this.Height = 489;
            this.MinimumSize = new Size(927, 489);
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
            dataGridView1.Columns.Add("Fecha", "Fecha");
            dataGridView1.Columns.Add("Consecutivo", "Consecutivo");
            dataGridView1.Columns.Add("NombreCliente", "Nombre Cliente");
            dataGridView1.Columns.Add("TotalProductos", "Total Productos");
            dataGridView1.Columns.Add("TotalCanastas", "Total Canastas");
            dataGridView1.Columns.Add("TotalPesoBruto", "Total Peso Bruto");
            dataGridView1.Columns.Add("TotalCompra", "Total Compra");
            dataGridView1.Columns.Add("Descuento", "Descuento");
            dataGridView1.Columns.Add("TotalAbona", "TotalAbona");
            dataGridView1.Columns.Add("TotalPagar", "Total Pagar");

            DataGridViewButtonColumn printButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Imprimir",
                Text = "Imprimir",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(printButtonColumn);

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnVolver, "Cerrar y volver al formulario anterior");
            toolTip.SetToolTip(btnRefrescar, "Refrescar información");
            this.Controls.Add(btnRefrescar);
            this.Controls.Add(btnVolver);


            dataGridView1.CellClick += DataGridView1_CellClick;
            btnFiltrar.Click += BtnFiltrar_Click;

            this.Controls.Add(datePickerStart);
            this.Controls.Add(txtCliente);
            this.Controls.Add(btnFiltrar);
            this.Controls.Add(dataGridView1);

            LoadData();
        }

        private void LoadData(string filterQuery = "")
        {
            try
            {
                connection.Open();
                string query = "SELECT * FROM Ventas";

                if (!string.IsNullOrEmpty(filterQuery))
                {
                    query += " WHERE " + filterQuery;
                }

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                DataTable ventasTable = new DataTable();
                adapter.Fill(ventasTable);

                dataGridView1.Rows.Clear();

                foreach (DataRow row in ventasTable.Rows)
                {
                    dataGridView1.Rows.Add(
                        row["Fecha"] != DBNull.Value ? Convert.ToDateTime(row["Fecha"]).ToShortDateString() : "",
                        row["Consecutivo"] != DBNull.Value ? row["Consecutivo"].ToString() : "",
                        row["NombreCliente"] != DBNull.Value ? row["NombreCliente"].ToString() : "",
                        row["TotalProductos"] != DBNull.Value ? row["TotalProductos"].ToString() : "0",
                        row["TotalCanastas"] != DBNull.Value ? row["TotalCanastas"].ToString() : "0",
                        row["TotalPesoBruto"] != DBNull.Value ? row["TotalPesoBruto"].ToString() : "0",
                        row["TotalCompra"] != DBNull.Value ? row["TotalCompra"].ToString() : "0",
                        row["Descuento"] != DBNull.Value ? row["Descuento"].ToString() : "0",
                        row["TotalAbona"] != DBNull.Value ? row["TotalAbona"].ToString() : "0",
                        row["TotalPagar"] != DBNull.Value ? row["TotalPagar"].ToString() : "0"
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
            if (datePickerStart.Value == null)
            {
                MessageBox.Show("Por favor, seleccione una fecha.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre del cliente.", "Advertencia!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime selectedDate = datePickerStart.Value;
            string cliente = txtCliente.Text;
            string filterQuery = $"Fecha = #{selectedDate:MM-dd-yyyy}# AND NombreCliente = '{cliente}'";
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
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Imprimir")
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Obtener los datos de la fila seleccionada
                string fecha = row.Cells["Fecha"].Value.ToString();
                string consecutivo = row.Cells["Consecutivo"].Value.ToString();
                string nombrecliente = row.Cells["NombreCliente"].Value.ToString();
                string totalproductos = row.Cells["TotalProductos"].Value.ToString();
                string totalcanastas = row.Cells["TotalCanastas"].Value.ToString();
                string totalpesobruto = row.Cells["totalPesoBruto"].Value.ToString();
                string totalcompra = row.Cells["totalCompra"].Value.ToString();
                string descuento = row.Cells["Descuento"].Value.ToString();
                string abona = row.Cells["TotalAbona"].Value.ToString();
                string totalpagar = row.Cells["TotalPagar"].Value.ToString();
                // Aquí puedes implementar la lógica para imprimir la información, por ejemplo, mostrar un MessageBox
                //MessageBox.Show($"Imprimir Venta\n\nID: {idVenta}\nCliente: {cliente}\nProducto: {producto}\nCantidad: {cantidad}\nPrecio: {precio}\nTotal: {total}", "Imprimir Venta");
            }
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

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
    }
}
