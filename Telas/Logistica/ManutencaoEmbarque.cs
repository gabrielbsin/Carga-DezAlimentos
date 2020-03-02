using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class ManutencaoEmbarque : Form
    {
        public ManutencaoEmbarque()
        {
            InitializeComponent();
            bunifuFlatButton3.Hide();
            HabilitaDesabilita(false, true);
        }

        public static int embarque_final = 0;

        public static int Numerocm { get; set; }
        public static string Nome { get; set; }

        private void HabilitaDesabilita(bool update, bool clear)
        {
            if (!update)
            {
                bunifuMaterialTextbox1.Enabled = true;
                bunifuImageButton5.Enabled = true;

                bunifuFlatButton1.Enabled = false;
                bunifuFlatButton3.Enabled = false;
                bunifuFlatButton3.Hide();

                bunifuDatepicker1.Enabled = false;

                bunifuMaterialTextbox2.Enabled = false;
                bunifuMaterialTextbox3.Enabled = false;
                bunifuMaterialTextbox4.Enabled = false;
                bunifuMaterialTextbox5.Enabled = false;
                bunifuMaterialTextbox6.Enabled = false;
                bunifuMaterialTextbox7.Enabled = false;
                bunifuMaterialTextbox8.Enabled = false;
                bunifuMaterialTextbox9.Enabled = false;
                bunifuMaterialTextbox10.Enabled = false;
                bunifuMaterialTextbox11.Enabled = false;

                if (clear)
                {
                    bunifuMaterialTextbox1.Text = "";
                    bunifuMaterialTextbox5.Text = "";
                    bunifuMaterialTextbox6.Text = "";
                    bunifuMaterialTextbox7.Text = "";
                    bunifuMaterialTextbox8.Text = "";
                    bunifuMaterialTextbox9.Text = "";
                    bunifuMaterialTextbox10.Text = "";
                    bunifuMaterialTextbox11.Text = "";
                }

            } else
            {
                bunifuMaterialTextbox1.Enabled = false;
                bunifuImageButton5.Enabled = false;

                bunifuFlatButton1.Enabled = true;

                bunifuFlatButton3.Show();
                bunifuFlatButton3.Enabled = true;

                bunifuDatepicker1.Enabled = true;

                bunifuMaterialTextbox1.Enabled = true;
                bunifuMaterialTextbox2.Enabled = true;
                bunifuMaterialTextbox3.Enabled = false;
                bunifuMaterialTextbox4.Enabled = false;
                bunifuMaterialTextbox5.Enabled = true;
                bunifuMaterialTextbox6.Enabled = false;
                bunifuMaterialTextbox7.Enabled = true;
                bunifuMaterialTextbox8.Enabled = true;
                bunifuMaterialTextbox9.Enabled = true;
                bunifuMaterialTextbox10.Enabled = true;
                bunifuMaterialTextbox11.Enabled = true;
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

        private void BunifuImageButton5_Click(object sender, EventArgs e)
        {
            PesquisaEmbarque pe = new PesquisaEmbarque();
            if (pe.ShowDialog() == DialogResult.OK)
            {
                MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
                try
                {
                    conexao.Open();
                    MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE WHERE embarque_id='" + embarque_final + "'", conexao);
                    MySqlDataReader drCommand = comando.ExecuteReader();

                    if (drCommand.HasRows)
                    {
                        while (drCommand.Read())
                        {
                            bunifuMaterialTextbox1.Text = drCommand["embarque_id"].ToString();
                            bunifuMaterialTextbox2.Text = drCommand["preparador"].ToString();
                            bunifuMaterialTextbox3.Text = "1000";
                            bunifuMaterialTextbox4.Text = "DEZ ALIMENTOS LTDA";
                            bunifuDatepicker1.Value = Convert.ToDateTime(drCommand["data_embarque"]);
                            bunifuMaterialTextbox5.Text = drCommand["transportador"].ToString();
                            bunifuMaterialTextbox6.Text = drCommand["transportador_nome"].ToString();
                            bunifuMaterialTextbox7.Text = drCommand["motorista"].ToString();
                            bunifuMaterialTextbox8.Text = drCommand["placa"].ToString();
                            bunifuMaterialTextbox9.Text = drCommand["uf_motorista"].ToString();
                            bunifuMaterialTextbox10.Text = drCommand["cidade_destino"].ToString();
                            bunifuMaterialTextbox11.Text = drCommand["uf_destino"].ToString();
                        }

                        HabilitaDesabilita(true, false);

                    }
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                } finally
                {
                    conexao.Close();
                }
            }
        }

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
            HabilitaDesabilita(false, true);
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if (bunifuMaterialTextbox1.Text == String.Empty && bunifuMaterialTextbox1.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar um Embarque!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox1;
                bunifuMaterialTextbox1.Focus();
                return;
            }
            if (bunifuMaterialTextbox2.Text == String.Empty && bunifuMaterialTextbox2.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar um Preparador!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox2;
                bunifuMaterialTextbox2.Focus();
                return;
            }
            if (bunifuMaterialTextbox2.Text == String.Empty && bunifuMaterialTextbox2.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar um Preparador!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox2;
                bunifuMaterialTextbox2.Focus();
                return;
            }
            if (bunifuMaterialTextbox5.Text == String.Empty && bunifuMaterialTextbox5.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar um Transportador!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox5;
                bunifuMaterialTextbox5.Focus();
                return;
            }
            if (bunifuMaterialTextbox6.Text == String.Empty && bunifuMaterialTextbox6.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar um Transportador!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox6;
                bunifuMaterialTextbox6.Focus();
                return;
            }
            if (bunifuMaterialTextbox7.Text == String.Empty && bunifuMaterialTextbox7.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar um Motorista!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox7;
                bunifuMaterialTextbox7.Focus();
                return;
            }
            if (bunifuMaterialTextbox8.Text == String.Empty && bunifuMaterialTextbox8.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar uma Placa!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox8;
                bunifuMaterialTextbox8.Focus();
                return;
            }
            if (bunifuMaterialTextbox9.Text == String.Empty && bunifuMaterialTextbox9.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar uma UF do Motorista!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox9;
                bunifuMaterialTextbox9.Focus();
                return;
            }
            if (bunifuMaterialTextbox10.Text == String.Empty && bunifuMaterialTextbox10.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar uma Cidade de Destino!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox10;
                bunifuMaterialTextbox10.Focus();
                return;
            }
            if (bunifuMaterialTextbox11.Text == String.Empty && bunifuMaterialTextbox11.Text.Length < 0)
            {
                MessageBox.Show("É necessario informar uma UF de Destino!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox11;
                bunifuMaterialTextbox11.Focus();
                return;
            }


            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET preparador=@param1, transportador=@param2, transportador_nome=@param3, motorista=@param4, placa=@param5, uf_motorista=@param6, cidade_destino=@param7, uf_destino=@param8, data_embarque=@param9 WHERE embarque_id='" + embarque_final + "'", conexao);
                comando.Parameters.AddWithValue("@param1", bunifuMaterialTextbox2.Text.ToString());
                comando.Parameters.AddWithValue("@param2", Convert.ToInt32(bunifuMaterialTextbox5.Text));
                comando.Parameters.AddWithValue("@param3", bunifuMaterialTextbox6.Text.ToString());
                comando.Parameters.AddWithValue("@param4", bunifuMaterialTextbox7.Text.ToString());
                comando.Parameters.AddWithValue("@param5", bunifuMaterialTextbox8.Text.ToString());
                comando.Parameters.AddWithValue("@param6", bunifuMaterialTextbox9.Text.ToString());
                comando.Parameters.AddWithValue("@param7", bunifuMaterialTextbox10.Text.ToString());
                comando.Parameters.AddWithValue("@param8", bunifuMaterialTextbox11.Text.ToString());
                comando.Parameters.AddWithValue("@param9", bunifuDatepicker1.Value.ToString("yyyy-MM-dd"));
                comando.ExecuteNonQuery();
                comando.Dispose();

                MessageBox.Show("Embarque atualizado com sucesso!");

            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                conexao.Close();
            }
        }

        private void BunifuImageButton4_Click(object sender, EventArgs e)
        {
            PesquisaTransportador pt = new PesquisaTransportador();
            if (pt.ShowDialog() == DialogResult.OK)
            {
                bunifuMaterialTextbox5.Text = Numerocm.ToString();
                bunifuMaterialTextbox6.Text = Nome.ToString();
            }
        }
    }
}
