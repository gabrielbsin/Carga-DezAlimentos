using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Expedicao
{
    public partial class ListarEmbarques : Form
    {
        public ListarEmbarques()
        {
            InitializeComponent();
        }

        public static DataTable dt_Carregado;
        public static DataTable dt_Carregamento;
        public static DataTable dt_Separacao;
        public static DataTable dt_SelecaoPedidos;

        private void ListarEmbarques_Load(object sender, EventArgs e)
        {
            CarregaTelas();
        }

        private void CarregaTelas()
        {

            dt_SelecaoPedidos = new DataTable();
            dt_SelecaoPedidos.Columns.Add(new DataColumn("Embarque", typeof(int)));
            dt_SelecaoPedidos.Columns.Add(new DataColumn("Motorista", typeof(string)));
            dt_SelecaoPedidos.Columns.Add(new DataColumn("Placa", typeof(string)));
            dt_SelecaoPedidos.Columns.Add(new DataColumn("Peso", typeof(string)));
            dt_SelecaoPedidos.Columns.Add(new DataColumn("Cid. Destino", typeof(string)));
            bunifuCustomDataGrid1.DataSource = dt_SelecaoPedidos;

            dt_Separacao = new DataTable();
            dt_Separacao.Columns.Add(new DataColumn("Embarque", typeof(int)));
            dt_Separacao.Columns.Add(new DataColumn("Motorista", typeof(string)));
            dt_Separacao.Columns.Add(new DataColumn("Placa", typeof(string)));
            dt_Separacao.Columns.Add(new DataColumn("Peso", typeof(string)));
            dt_Separacao.Columns.Add(new DataColumn("Cid. Destino", typeof(string)));
            bunifuCustomDataGrid2.DataSource = dt_Separacao;

            dt_Carregamento = new DataTable();
            dt_Carregamento.Columns.Add(new DataColumn("Embarque", typeof(int)));
            dt_Carregamento.Columns.Add(new DataColumn("Motorista", typeof(string)));
            dt_Carregamento.Columns.Add(new DataColumn("Placa", typeof(string)));
            dt_Carregamento.Columns.Add(new DataColumn("Peso", typeof(string)));
            dt_Carregamento.Columns.Add(new DataColumn("Cid. Destino", typeof(string)));
            bunifuCustomDataGrid3.DataSource = dt_Carregamento;

            dt_Carregado = new DataTable();
            dt_Carregado.Columns.Add(new DataColumn("Embarque", typeof(int)));
            dt_Carregado.Columns.Add(new DataColumn("Motorista", typeof(string)));
            dt_Carregado.Columns.Add(new DataColumn("Placa", typeof(string)));
            dt_Carregado.Columns.Add(new DataColumn("Peso", typeof(string)));
            dt_Carregado.Columns.Add(new DataColumn("Cid. Destino", typeof(string)));
            bunifuCustomDataGrid4.DataSource = dt_Carregado;


            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE", conexao);
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

                        switch (status)
                        {
                            case 1:
                                dt_SelecaoPedidos.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                            case 2:
                                dt_Separacao.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                            case 3:
                                dt_Carregamento.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                            case 4:
                                dt_Carregado.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar embarques!\n" + ex.ToString(), "Carregar Embarques", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public static void BunifuCustomDataGrid1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                int embarque_id = Convert.ToInt32(bunifuCustomDataGrid1.Rows[index].Cells[0].Value.ToString());
                MudaStatus ms = new MudaStatus(embarque_id, 0);
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    bunifuCustomDataGrid1.Rows.RemoveAt(index);
                    MessageBox.Show("Embarque alterado com sucesso!", "Alterar Embarque", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public static void BunifuCustomDataGrid2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                int embarque_id = Convert.ToInt32(bunifuCustomDataGrid2.Rows[index].Cells[0].Value.ToString());
                MudaStatus ms = new MudaStatus(embarque_id, 1);
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    bunifuCustomDataGrid2.Rows.RemoveAt(index);
                    MessageBox.Show("Embarque alterado com sucesso!", "Alterar Embarque", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public static void BunifuCustomDataGrid3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                int embarque_id = Convert.ToInt32(bunifuCustomDataGrid3.Rows[index].Cells[0].Value.ToString());
                MudaStatus ms = new MudaStatus(embarque_id, 2);
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    bunifuCustomDataGrid3.Rows.RemoveAt(index);
                    MessageBox.Show("Embarque alterado com sucesso!", "Alterar Embarque", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public static void BunifuCustomDataGrid4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (index >= 0)
            {
                int embarque_id = Convert.ToInt32(bunifuCustomDataGrid4.Rows[index].Cells[0].Value.ToString());
                MudaStatus ms = new MudaStatus(embarque_id, 3);
                if (ms.ShowDialog() == DialogResult.OK)
                {
                    bunifuCustomDataGrid4.Rows.RemoveAt(index);
                    MessageBox.Show("Embarque alterado com sucesso!", "Alterar Embarque", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }
    }
}
