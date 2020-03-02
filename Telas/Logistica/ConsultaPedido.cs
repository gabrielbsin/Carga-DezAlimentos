using GerarCargaDez.Telas.Logistica.Consulta.Pedido.Controller;
using GerarCargaDez.Telas.Logistica.Consulta.Pedido.Model;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class ConsultaPedido : Form
    {
        public ConsultaPedido()
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

        private void ConsultaPedido_Load(object sender, EventArgs e)
        {
            PedidoController tc = new PedidoController();

            bunifuCustomDataGrid4.DataSource = tc.Listar();

            bunifuCustomDataGrid4.Columns[0].HeaderText = "Cód. Pedido";
            bunifuCustomDataGrid4.Columns[1].HeaderText = "Cód. Cliente";
            bunifuCustomDataGrid4.Columns[2].HeaderText = "Nome Cliente";
            bunifuCustomDataGrid4.Columns[3].HeaderText = "Nome Cidade";
            bunifuCustomDataGrid4.Columns[4].HeaderText = "UF";
            bunifuCustomDataGrid4.Columns[5].HeaderText = "Usuário";
        }

        private void Pesquisar(PedidoModel pm)
        {
            int comboIndex = metroComboBox1.SelectedIndex;
            switch (comboIndex)
            {
                case 0: // Pedido 
                    pm.Pedido_Id = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                    break;
                case 1: // Cod. Cliente
                    pm.Pessoa_Id = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                    break;
                case 2: // Nome do Cliente
                    pm.Pessoa_Desc = bunifuMaterialTextbox1.Text.ToString();
                    break;
                case 3: // Nome da Cidade
                    pm.Cidade_nome = bunifuMaterialTextbox1.Text.ToString();
                    break;
                case 4: // Estado
                    pm.UF_Desc = bunifuMaterialTextbox1.Text.ToString();
                    break;
                case 5: // Usuario
                    pm.User_Desc = bunifuMaterialTextbox1.Text.ToString();
                    break;
            }

            PedidoController pc = new PedidoController();

            bunifuCustomDataGrid4.DataSource = pc.Pesquisar(pm, metroComboBox1.SelectedIndex);
        }

        private void BunifuMaterialTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int option = metroComboBox1.SelectedIndex;
            switch (option)
            {
                case 0: // Pedido 
                case 1: // Cod. Cliente
                    e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
                    break;
                case 2: // Nome do Cliente
                    break;
                case 3: // Nome da Cidade
                    break;
                case 4: // Estado
                    break;
                case 5: // Usuario
                    break;
            }
        }

        private void BunifuMaterialTextbox1_OnValueChanged(object sender, EventArgs e)
        {
            if (bunifuMaterialTextbox1.Text == String.Empty)
            {
                PedidoController ec = new PedidoController();
                bunifuCustomDataGrid4.DataSource = ec.Listar();
            }
            else
            {
                PedidoModel tm = new PedidoModel();
                Pesquisar(tm);
            }
        }

        private void BunifuCustomDataGrid4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (bunifuCustomDataGrid4["pedido_id", e.RowIndex].Value.ToString() == String.Empty) return;

            MontarCarga.pedido_final = Convert.ToInt32(bunifuCustomDataGrid4["pedido_id", e.RowIndex].Value.ToString());
            MontarCarga.serie_final = bunifuCustomDataGrid4["serie", e.RowIndex].Value.ToString();

            this.DialogResult = DialogResult.OK;
        }
    }
}
