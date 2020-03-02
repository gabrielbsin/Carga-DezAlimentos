using GerarCargaDez.Core;
using GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Controller;
using GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Model;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Etiqueta.Consulta.Item
{
    public partial class ConsultaOrdem : Form
    {
        public ConsultaOrdem()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void ConsultaItem_Load(object sender, EventArgs e)
        {
            OrdemController oc = new OrdemController();

            dataGridView1.DataSource = oc.Listar();

            dataGridView1.Columns[0].HeaderText = "Id";
            dataGridView1.Columns[1].HeaderText = "Ordem de Produção";
            dataGridView1.Columns[2].HeaderText = "Estabelecimento";
            dataGridView1.Columns[3].HeaderText = "Item";
            dataGridView1.Columns[4].HeaderText = "Quantidade";
            dataGridView1.Columns[5].HeaderText = "Local";
            dataGridView1.Columns[6].HeaderText = "Status";
            dataGridView1.Columns[7].HeaderText = "Embalagem";
            dataGridView1.Columns[8].HeaderText = "Número FCI";
            dataGridView1.Columns[9].HeaderText = "Data Entrada";
            dataGridView1.Columns[10].HeaderText = "Hora";
            dataGridView1.Columns[11].HeaderText = "Valor Item NF";
            dataGridView1.Columns[12].HeaderText = "Percentual";
            dataGridView1.Columns[13].HeaderText = "Custo";
            dataGridView1.Columns[14].HeaderText = "CPV";
            dataGridView1.Columns[15].HeaderText = "Percentual Médio";
            dataGridView1.Columns[16].HeaderText = "Sequência PA";
            dataGridView1.Columns[17].HeaderText = "A Partir De";
            dataGridView1.Columns[18].HeaderText = "Cancelado";

        }

        private void Pesquisar(OrdemModel om)
        {
            om.Id = int.Parse(textBox1.Text.Trim());

            OrdemController oc = new OrdemController();

            dataGridView1.DataSource = oc.Pesquisar(om);

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                OrdemController oc = new OrdemController();
                dataGridView1.DataSource = oc.Listar();
            }
            else
            {
                OrdemModel om = new OrdemModel();
                Pesquisar(om);
            }
        }


        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView1["IDORDEMPROD", e.RowIndex].Value.ToString() == "") return;

            NovaEtiqueta.varOrdemId = Convert.ToInt32(dataGridView1["IDORDEMPROD", e.RowIndex].Value.ToString());
            NovaEtiqueta.varItemId = Convert.ToInt32(dataGridView1["ITEM", e.RowIndex].Value.ToString());
            NovaEtiqueta.varItemDesc = CoreViaSoft.ObterDescricaoItem(Convert.ToInt32(dataGridView1["ITEM", e.RowIndex].Value.ToString()));

            //Config_Qualidade.Id = Convert.ToInt32(dataGridView1["IDORDEMPROD", e.RowIndex].Value.ToString());
            //Config_Qualidade.Item_Id = Convert.ToInt32(dataGridView1["ITEM", e.RowIndex].Value.ToString());
            //Config_Qualidade.Item_Desc = Config_Qualidade.GetDescriptionItem(Convert.ToInt32(dataGridView1["ITEM", e.RowIndex].Value.ToString()));

            //Config_Qualidade2.Id = Convert.ToInt32(dataGridView1["IDORDEMPROD", e.RowIndex].Value.ToString());
            //Config_Qualidade2.Item_Id = Convert.ToInt32(dataGridView1["ITEM", e.RowIndex].Value.ToString());
            //Config_Qualidade2.Item_Desc = Config_Qualidade2.GetDescriptionItem(Convert.ToInt32(dataGridView1["ITEM", e.RowIndex].Value.ToString()));

            this.DialogResult = DialogResult.OK;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }
    }
}
