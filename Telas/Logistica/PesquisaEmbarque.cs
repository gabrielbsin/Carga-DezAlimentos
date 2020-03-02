using GerarCargaDez.Telas.Logistica.Consulta.Embarque.Controller;
using GerarCargaDez.Telas.Logistica.Consulta.Embarque.Model;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class PesquisaEmbarque : Form
    {
        public PesquisaEmbarque()
        {
            InitializeComponent();
            metroComboBox1.SelectedIndex = 0;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BunifuMaterialTextbox1_OnValueChanged(object sender, EventArgs e)
        {
            if (bunifuMaterialTextbox1.Text == String.Empty)
            {
                EmbarqueController ec = new EmbarqueController();
                bunifuCustomDataGrid4.DataSource = ec.Listar();
            }
            else
            {
                EmbarqueModel tm = new EmbarqueModel();
                Pesquisar(tm);
            }
        }

        private void PesquisaEmbarque_Load(object sender, EventArgs e)
        {
            EmbarqueController tc = new EmbarqueController();

            bunifuCustomDataGrid4.DataSource = tc.Listar();

            bunifuCustomDataGrid4.Columns[0].HeaderText = "Estabelecimento";
            bunifuCustomDataGrid4.Columns[1].HeaderText = "Nº Embarque";
            bunifuCustomDataGrid4.Columns[2].HeaderText = "Preparador";
            bunifuCustomDataGrid4.Columns[3].HeaderText = "Código Transportador";
            bunifuCustomDataGrid4.Columns[4].HeaderText = "Nome Transportador";
            bunifuCustomDataGrid4.Columns[5].HeaderText = "Motorista";
            bunifuCustomDataGrid4.Columns[6].HeaderText = "Placa";
            bunifuCustomDataGrid4.Columns[7].HeaderText = "UF";
            bunifuCustomDataGrid4.Columns[8].HeaderText = "Data Embarque";
        }

        private void Pesquisar(EmbarqueModel em)
        {
            int comboIndex = metroComboBox1.SelectedIndex;
            switch (comboIndex)
            {
                case 0:
                    em.Embarque_id = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                    break;
                case 1:
                    em.Transportador_desc = bunifuMaterialTextbox1.Text.ToString();
                    break;
                case 2:
                    em.Motorista_desc = bunifuMaterialTextbox1.Text.ToString();
                    break;
            }

            EmbarqueController ec = new EmbarqueController();

            bunifuCustomDataGrid4.DataSource = ec.Pesquisar(em, metroComboBox1.SelectedIndex);
        }

        private void BunifuMaterialTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (metroComboBox1.SelectedIndex == 0)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        public void BunifuCustomDataGrid4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (bunifuCustomDataGrid4["embarque_id", e.RowIndex].Value.ToString() == String.Empty) return;

            MontarCarga.embarque_final = Convert.ToInt32(bunifuCustomDataGrid4["embarque_id", e.RowIndex].Value.ToString());
            ManutencaoEmbarque.embarque_final = Convert.ToInt32(bunifuCustomDataGrid4["embarque_id", e.RowIndex].Value.ToString());

            DialogResult = DialogResult.OK;
        }
    }
}
