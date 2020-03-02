using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Estoque.Inventario
{
    public partial class Inventario : Form
    {
        public Inventario()
        {
            InitializeComponent();
        }

        string conexao_viasoft = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";";


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
            // VARIAVEIS
            int localMin = 0, localMax = 0;
            int itemMin = 0, itemMax = 0;

            // LOCAL
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.Length < 1 && textBox1.Text.Trim().Length == 0) localMin = 0;
            if (!string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.Length > 0 && textBox1.Text.Trim().Length > 0) localMin = Convert.ToInt32(textBox1.Text);
            if (string.IsNullOrEmpty(textBox2.Text) || textBox2.Text.Length < 1 && textBox2.Text.Trim().Length == 0) localMax = 0;
            if (!string.IsNullOrEmpty(textBox2.Text) || textBox2.Text.Length > 0 && textBox2.Text.Trim().Length > 0) localMax = Convert.ToInt32(textBox2.Text);

            // ITEM 
            if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text.Length < 1 && textBox3.Text.Trim().Length == 0) itemMin = 0;
            if (!string.IsNullOrEmpty(textBox3.Text) || textBox3.Text.Length > 0 && textBox3.Text.Trim().Length > 0) itemMin = Convert.ToInt32(textBox3.Text);
            if (string.IsNullOrEmpty(textBox4.Text) || textBox4.Text.Length < 1 && textBox4.Text.Trim().Length == 0) itemMax = 0;
            if (!string.IsNullOrEmpty(textBox4.Text) || textBox4.Text.Length > 0 && textBox4.Text.Trim().Length > 0) itemMax = Convert.ToInt32(textBox4.Text);


            //OracleConnection conexao = new OracleConnection(conexao_viasoft);
            //try
            //{
            //    conexao.Open();

            //    OracleCommand comando = new OracleCommand("", conexao);
            //    OracleDataReader odr = comando.ExecuteReader();
            //}

        }
    }
}
