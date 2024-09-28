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
    public partial class Historial : Form
    {
        private OleDbConnection connection;

        public Historial()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            InitializeDataGridView();
        }

        private void InitializeDatabaseConnection()
        {
            
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Verdum.accdb";
            connection = new OleDbConnection(connectionString);
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Add("VentaID", "ID Venta");
            dataGridView1.Columns.Add("Canastas", "Canastas");
            dataGridView1.Columns.Add("PesoBruto", "Peso Bruto");
            dataGridView1.Columns.Add("Fecha", "Fecha");
            dataGridView1.Columns.Add("Cliente", "Cliente");
            dataGridView1.Columns.Add("Producto", "Producto");
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
            dataGridView1.Columns.Add("Precio", "Precio");
            dataGridView1.Columns.Add("Total", "Total");

            DataGridViewButtonColumn printButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Imprimir",
                Text = "Imprimir",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Add(printButtonColumn);

            dataGridView1.CellClick += DataGridView1_CellClick;

            // Añadir controles de filtrado
            DateTimePicker datePickerStart = new DateTimePicker { Name = "datePickerStart", Location = new System.Drawing.Point(10, 10) };
           
            TextBox txtCliente = new TextBox { Name = "txtCliente", Location = new System.Drawing.Point(300, 10) };
            Button btnFiltrar = new Button { Name = "btnFiltrar", Text = "Filtrar", Location = new System.Drawing.Point(450, 10) };

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
                        row["id"],
                        row["canastas"],
                        row["pesobruto"],
                        Convert.ToDateTime(row["fecha"]).ToShortDateString(),
                        row["cliente"],
                        row["productoid"],
                        row["cantidad"],
                        row["precio"],
                        row["total"]
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
            DateTimePicker datePickerStart = (DateTimePicker)this.Controls["datePickerStart"];
            TextBox txtCliente = (TextBox)this.Controls["txtCliente"];

            string filterQuery = "";

            if (!string.IsNullOrEmpty(txtCliente.Text))
            {
                filterQuery += $"cliente LIKE '%{txtCliente.Text}%'";
            }


            if (!string.IsNullOrEmpty(filterQuery))
            {
                filterQuery += " AND ";
            }

            filterQuery += $"fecha = #{datePickerStart.Value.Date.ToShortDateString()}#";


            LoadData(filterQuery);
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Imprimir")
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Obtener los datos de la fila seleccionada
                string idVenta = row.Cells["VentaID"].Value.ToString();
                string canastas = row.Cells["Canastas"].Value.ToString();
                string pesoBruto = row.Cells["PesoBruto"].Value.ToString();
                string fecha = row.Cells["Fecha"].Value.ToString();
                string cliente = row.Cells["Cliente"].Value.ToString();
                string producto = row.Cells["Producto"].Value.ToString();
                string cantidad = row.Cells["Cantidad"].Value.ToString();
                string precio = row.Cells["Precio"].Value.ToString();
                string total = row.Cells["Total"].Value.ToString();

                // Aquí puedes implementar la lógica para imprimir la información, por ejemplo, mostrar un MessageBox
                MessageBox.Show($"Imprimir Venta\n\nID: {idVenta}\nCliente: {cliente}\nProducto: {producto}\nCantidad: {cantidad}\nPrecio: {precio}\nTotal: {total}", "Imprimir Venta");
            }
        }
    }
}
