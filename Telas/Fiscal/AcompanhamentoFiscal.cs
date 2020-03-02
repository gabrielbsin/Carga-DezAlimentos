using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Fiscal
{
    public partial class AcompanhamentoFiscal : Form
    {
        public AcompanhamentoFiscal()
        {
            InitializeComponent();
        }

        public static DataTable dt_Carregado;

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void AcompanhamentoFiscal_Load(object sender, EventArgs e)
        {
            CarregaTelas(false);
        }

        private void CarregaTelas(bool update)
        {

            dt_Carregado = new DataTable();
            dt_Carregado.Columns.Add(new DataColumn("Embarque", typeof(int)));
            dt_Carregado.Columns.Add(new DataColumn("Motorista", typeof(string)));
            dt_Carregado.Columns.Add(new DataColumn("Placa", typeof(string)));
            dt_Carregado.Columns.Add(new DataColumn("Peso", typeof(string)));
            dt_Carregado.Columns.Add(new DataColumn("Cid. Destino", typeof(string)));
            bunifuCustomDataGrid4.DataSource = dt_Carregado;

            if (update)
            {
                dt_Carregado.Clear();
            }

            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE WHERE status = '4' ORDER BY data_alter", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int status = Convert.ToInt32(drCommand["status"]);
                        int embarque_id = Convert.ToInt32(drCommand["embarque_id"]);
                        string motorista = drCommand["motorista"].ToString();
                        string placa = drCommand["placa"].ToString();
                        string peso = string.Format("{0:N}", Convert.ToDouble(drCommand["peso"]));
                        string cidade_destino = drCommand["cidade_destino"].ToString();
                        if (cidade_destino != null)
                        {
                            cidade_destino = drCommand["cidade_destino"].ToString() + " - " + drCommand["uf_destino"].ToString();
                        }
                        else
                        {
                            cidade_destino = "-";
                        }
                        dt_Carregado.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar embarques!\n" + ex.ToString(), "Acompanhar Embarques", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            CarregaTelas(true);
        }

        private void BunifuCustomDataGrid4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = dt_Carregado.Rows.Count;
            if (count > 0)
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid4.Rows[index].Cells[0].Value.ToString());
                    DetalhamentoPedido dp = new DetalhamentoPedido(pedido_id);
                    dp.Show();
                }
            }
        }
    }
}
