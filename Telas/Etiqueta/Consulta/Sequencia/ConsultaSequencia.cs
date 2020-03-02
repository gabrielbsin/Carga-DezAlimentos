using GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Controller;
using GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Model;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Etiqueta.Consulta
{
    public partial class ConsultaSequencia : Form
    {
        public ConsultaSequencia()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void ConsultaSequencia_Load(object sender, EventArgs e)
        {
            SequenciaController sc = new SequenciaController();

            dataGridView1.DataSource = sc.Listar();
            dataGridView1.Columns[0].HeaderText = "Sequência";
            dataGridView1.Columns[1].HeaderText = "Ordem de Produção";
            dataGridView1.Columns[2].HeaderText = "Grupo de Qualidade";
            dataGridView1.Columns[3].HeaderText = "Item";
            dataGridView1.Columns[4].HeaderText = "Descrição do Item";
            dataGridView1.Columns[5].HeaderText = "Data Producao";
            dataGridView1.Columns[6].HeaderText = "Data Validade";
            dataGridView1.Columns[7].HeaderText = "Lote/Série";
            dataGridView1.Columns[8].HeaderText = "Peso Líq.";
            dataGridView1.Columns[9].HeaderText = "Peso Bruto";
            dataGridView1.Columns[10].HeaderText = "Hora Impressão";
            dataGridView1.Columns[11].HeaderText = "Data Impressão";
            dataGridView1.Columns[12].HeaderText = "Tara";
            dataGridView1.Columns[13].HeaderText = "Finalidade";
            dataGridView1.Columns[14].HeaderText = "Etiqueta Disponível";
            dataGridView1.Columns[15].HeaderText = "Consistência";
            dataGridView1.Columns[16].HeaderText = "Cor";
            dataGridView1.Columns[17].HeaderText = "Status";
            dataGridView1.Columns[18].HeaderText = "Usuário";
        }

        private void Pesquisar(SequenciaModel sm)
        {

            if (comboBox1.SelectedIndex == 0)
            {
                sm.Seq = Convert.ToInt32(textBox1.Text);
            }
            else
            {
                sm.OrdemProd = textBox1.Text.ToString();
            }

            SequenciaController sc = new SequenciaController();

            dataGridView1.DataSource = sc.Pesquisar(sm, comboBox1.SelectedIndex);

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                SequenciaController sc = new SequenciaController();
                dataGridView1.DataSource = sc.Listar();
            }
            else
            {
                SequenciaModel om = new SequenciaModel();
                Pesquisar(om);
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView1["op", e.RowIndex].Value.ToString() == "") return;

            NovaEtiqueta.varOrdemId = Convert.ToInt32(dataGridView1["op", e.RowIndex].Value.ToString());
            NovaEtiqueta.varSequencia = Convert.ToInt32(dataGridView1["seq", e.RowIndex].Value.ToString());


            NovaEtiqueta.varGrupoQualidade = dataGridView1["grupo_qualidade", e.RowIndex].Value.ToString();

            NovaEtiqueta.varItemId = Convert.ToInt32(dataGridView1["item_id", e.RowIndex].Value.ToString());
            NovaEtiqueta.varItemDesc = dataGridView1["item_desc", e.RowIndex].Value.ToString();

            NovaEtiqueta.varDataProd = dataGridView1["data_prod", e.RowIndex].Value.ToString();
            NovaEtiqueta.varDataVal = dataGridView1["data_val", e.RowIndex].Value.ToString();

            NovaEtiqueta.varLoteSerie = dataGridView1["lote_serie", e.RowIndex].Value.ToString();

            //if (Properties.Settings.Default.finalidade)
            //{
            NovaEtiqueta.varPesoLiq = dataGridView1["peso_liq", e.RowIndex].Value.ToString();
            NovaEtiqueta.varPesoBru = dataGridView1["peso_bruto", e.RowIndex].Value.ToString();
            //} else
            //{
            //    Form2.Peso_liq = Convert.ToInt32(dataGridView1["peso_liq", e.RowIndex].Value.ToString());
            //    Form2.Peso_bru = Convert.ToInt32(dataGridView1["peso_bruto", e.RowIndex].Value.ToString());
            //}

            NovaEtiqueta.varHoraImpressao = dataGridView1["hora_imp", e.RowIndex].Value.ToString();

            if (Convert.ToInt32(dataGridView1["considera", e.RowIndex].Value.ToString()) == 0)
            {
                NovaEtiqueta.Considera = true;
            }
            else
            {
                NovaEtiqueta.Considera = false;
            }

            if (Properties.Settings.Default.finalidadeUsuario)
            {
                NovaEtiqueta.varTara = dataGridView1["tara", e.RowIndex].Value.ToString();
            }

            NovaEtiqueta.qualidadeConsistencia = dataGridView1["ql_consistencia", e.RowIndex].Value.ToString();
            NovaEtiqueta.qualidadeCor = dataGridView1["ql_cor", e.RowIndex].Value.ToString();
            NovaEtiqueta.qualidadeStatus = dataGridView1["ql_status", e.RowIndex].Value.ToString();


            /*####################################################################################*/

            //Manutencao_etiqueta.Id = Convert.ToInt32(dataGridView1["op", e.RowIndex].Value.ToString());
            //Manutencao_etiqueta.Seq = Convert.ToInt32(dataGridView1["seq", e.RowIndex].Value.ToString());


            //Manutencao_etiqueta.Grupo_Ql = dataGridView1["grupo_qualidade", e.RowIndex].Value.ToString();

            //Manutencao_etiqueta.Item_Id = Convert.ToInt32(dataGridView1["item_id", e.RowIndex].Value.ToString());
            //Manutencao_etiqueta.Item_Desc = dataGridView1["item_desc", e.RowIndex].Value.ToString();

            //Manutencao_etiqueta.Data_prod = DateTime.Parse(dataGridView1["data_prod", e.RowIndex].Value.ToString()).ToShortDateString();
            //Manutencao_etiqueta.Data_val = DateTime.Parse(dataGridView1["data_val", e.RowIndex].Value.ToString()).ToShortDateString();

            //Manutencao_etiqueta.Lote_Serie = dataGridView1["lote_serie", e.RowIndex].Value.ToString();

            //Manutencao_etiqueta.Peso_liq = dataGridView1["peso_liq", e.RowIndex].Value.ToString();
            //Manutencao_etiqueta.Peso_bru = dataGridView1["peso_bruto", e.RowIndex].Value.ToString();

            //Manutencao_etiqueta.Hora_imp = dataGridView1["hora_imp", e.RowIndex].Value.ToString();

            //if (Convert.ToInt32(dataGridView1["considera", e.RowIndex].Value.ToString()) == 0)
            //{
            //    Manutencao_etiqueta.Considera = "Sim";
            //}
            //else
            //{
            //    Manutencao_etiqueta.Considera = "Não";
            //}

            //if (Properties.Settings.Default.finalidade)
            //{
            //    Manutencao_etiqueta.Tara = dataGridView1["tara", e.RowIndex].Value.ToString();
            //}

            //Manutencao_etiqueta.Ql_consistencia = dataGridView1["ql_consistencia", e.RowIndex].Value.ToString();
            //Manutencao_etiqueta.Ql_cor = dataGridView1["ql_cor", e.RowIndex].Value.ToString();
            //Manutencao_etiqueta.Ql_status = dataGridView1["ql_status", e.RowIndex].Value.ToString();


            this.DialogResult = DialogResult.OK;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
