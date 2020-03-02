using GerarCargaDez.Telas.Logistica.Consulta.Controller;
using GerarCargaDez.Telas.Logistica.Consulta.Model;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class PesquisaTransportador : Form
    {
        public PesquisaTransportador()
        {
            InitializeComponent();
            metroComboBox1.SelectedIndex = 0;
        }

        private void PesquisaTransportador_Load(object sender, EventArgs e)
        {
            TransportadorController tc = new TransportadorController();

            bunifuCustomDataGrid4.DataSource = tc.Listar();

            bunifuCustomDataGrid4.Columns[0].HeaderText = "Estabelicimento";
            bunifuCustomDataGrid4.Columns[1].HeaderText = "Código";
            bunifuCustomDataGrid4.Columns[2].HeaderText = "Nome do Prestador";
        }

        private void Pesquisar(TransportadorModel om)
        {
            if (metroComboBox1.SelectedIndex == 0)
            {
                om.Numerocm = Convert.ToInt32(bunifuMaterialTextbox1.Text);
            }
            else
            {
                om.Nome = bunifuMaterialTextbox1.Text.ToString();
            }

            TransportadorController tc = new TransportadorController();

            bunifuCustomDataGrid4.DataSource = tc.Pesquisar(om, metroComboBox1.SelectedIndex);
        }

        private void BunifuMaterialTextbox1_OnValueChanged(object sender, EventArgs e)
        {
            if (bunifuMaterialTextbox1.Text == "")
            {
                TransportadorController tc = new TransportadorController();
                bunifuCustomDataGrid4.DataSource = tc.Listar();
            }
            else
            {
                TransportadorModel tm = new TransportadorModel();
                Pesquisar(tm);
            }
        }

        private void BunifuMaterialTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);

            if (metroComboBox1.SelectedIndex == 0)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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

        private void BunifuCustomDataGrid4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (bunifuCustomDataGrid4["Prestador", e.RowIndex].Value.ToString() == "") return;

            ManutencaoEmbarque.Numerocm = Convert.ToInt32(bunifuCustomDataGrid4["prestador", e.RowIndex].Value.ToString());

            ManutencaoEmbarque.Nome = bunifuCustomDataGrid4["nome", e.RowIndex].Value.ToString();

            this.DialogResult = DialogResult.OK;
        }
    }
}
