using GerarCargaDez.Core;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class InclusaoEmbarque : Form
    {

        public InclusaoEmbarque()
        {
            InitializeComponent();
            bunifuMaterialTextbox1.Text = CoreViaSoft.ObterUltimarOrdem().ToString();
            bunifuMaterialTextbox2.Text = "Carregamento Normal - 1";
            bunifuMaterialTextbox12.Text = "Ordem de Carregamento";
            bunifuMaterialTextbox3.Text = "1000";
            bunifuMaterialTextbox4.Text = "DEZ ALIMENTOS LTDA";
            bunifuDatepicker1.Value = DateTime.Now;
        }

        string conexao_viasoft = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";";
        string conexao_carga = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";
       
        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {

            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            MySqlConnection conexao_ = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                conexao_.Open();

                int ultimaOrdem = CoreViaSoft.ObterUltimarOrdem();

                OracleCommand comando = new OracleCommand("INSERT INTO ORDEMCARGACAB (IDCARGA, ESTAB, CONFIG, DESCRICAO, DTINCLUSAO, STATUS) VALUES (" + ultimaOrdem + ", " + 1000 + ", " + 1 + ", '" + bunifuMaterialTextbox5.Text.ToString() + "', CURRENT_DATE, 0)", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();




                MySqlCommand comando_ = new MySqlCommand("INSERT INTO ORDEMCARGACAB (IDCARGA, ESTAB, CONFIG, DESCRICAO, DTINCLUSAO, STATUS) VALUES (" + ultimaOrdem + ", " + 1000 + ", " + 1 + ", '" + bunifuMaterialTextbox5.Text.ToString() + "', NOW(), 0, 1)", conexao_);
                comando_.ExecuteNonQuery();
                comando_.Dispose();


                int OrdemId = ultimaOrdem;
                bunifuMaterialTextbox1.Text = (OrdemId + 1).ToString();
                ClearFields();
                MessageBox.Show("Ordem número " + OrdemId + " criado com sucesso!", "Inclusão de Ordem", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao incluir embarque!\n" + ex.ToString(), "Inclusão de Ordem", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
                conexao_.Close();
            }
        }

        private void ClearFields()
        {
            bunifuMaterialTextbox5.Text = "";
            bunifuDatepicker1.Value = DateTime.Now;
        }
    }
}
