using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComercializadoraVerdum
{
    public partial class ReporteMovimientos : Form
    {
        private OleDbConnection connection;
        private IConfigurationRoot configuration;
        private DataTable movimientos;
        public ReporteMovimientos()
        {
            this.Icon = new Icon("Images/verdum-logo-icono.ico");
            InitializeComponent();
            InitializeDatabaseConnection();
            SetButtonImageFromUrl();
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnVolverAnt, "Cerrar y volver al formulario anterior");
            toolTip.SetToolTip(btnRefrescarMovimientos, "Refrescar información");
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
        private void ReporteMovimientos_Load(object sender, EventArgs e)
        {
            CargarMovimientos();
        }
        public void SetButtonImageFromUrl()
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
                        btnRefrescarMovimientos.Image = image;
                        btnRefrescarMovimientos.ImageAlign = ContentAlignment.MiddleLeft;
                    }

                    using (var rt = new System.IO.MemoryStream(imageBack))
                    {
                        Image imageback = Image.FromStream(rt);
                        btnVolverAnt.Image = imageback;
                        btnVolverAnt.ImageAlign = ContentAlignment.MiddleLeft;
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al descargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarMovimientos()
        {
            try
            {
                connection.Open();
                string consulta = @"
            SELECT v.NombreCliente, mv.TipoPago, mv.ValorAbono, mv.Fecha
            FROM MovimientoVentas mv
            INNER JOIN Ventas v ON mv.VentaId = v.VentaId
            ORDER BY mv.Fecha DESC";

                OleDbDataAdapter adapter = new OleDbDataAdapter(consulta, connection);
                movimientos = new DataTable();
                adapter.Fill(movimientos);

                dgvReporteMovimientos.DataSource = movimientos;

                if (dgvReporteMovimientos.Columns["ValorAbono"] != null)
                {
                    dgvReporteMovimientos.Columns["ValorAbono"].DefaultCellStyle.Format = "N2";
                    dgvReporteMovimientos.Columns["ValorAbono"].DefaultCellStyle.FormatProvider =
                        System.Globalization.CultureInfo.CreateSpecificCulture("es-CO");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los movimientos: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void btnFiltrarMovimientos_Click(object sender, EventArgs e)
        {
            try
            {
                string filtroCliente = txtClienteMovimiento.Text.Trim();
                DateTime filtroFecha = dtpFechaMovimiento.Value.Date;

                DataView vistaFiltrada = new DataView(movimientos);

                string filtro = "";

                if (dtpFechaMovimiento.Checked) 
                {
                    filtro = $"Fecha = #{filtroFecha:MM/dd/yyyy}#";
                }

                if (!string.IsNullOrEmpty(filtroCliente))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        filtro += " AND ";
                    }
                    filtro += $"NombreCliente = '{filtroCliente}'";
                }

                vistaFiltrada.RowFilter = filtro;

                dgvReporteMovimientos.DataSource = vistaFiltrada;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar los movimientos: " + ex.Message);
            }
        }



        private void btnRefrescarMovimientos_Click(object sender, EventArgs e)
        {
            CargarMovimientos();
            txtClienteMovimiento.Text = "";
            dtpFechaMovimiento.Value = DateTime.Now;
        }

        private void btnVolverAnt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
