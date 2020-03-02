using GerarCargaDez.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class AlocaTotalPedido : Form
    {

        public AlocaTotalPedido()
        {
            InitializeComponent();
        }

        DataTable dt_LoadAlocItem;

        private void AlocaTotalPedido_Load(object sender, EventArgs e)
        {
            // Carrega todos pedidos da aba pedidos selecionado MontarCarga
            dt_LoadAlocItem = new DataTable();
            dt_LoadAlocItem.Columns.Add(new DataColumn("Pedido", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Item", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Quantidade (CX)", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Quantidade Disp. (CX)", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Série", typeof(string)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Sequência Item", typeof(int)));
            dataGridView1.DataSource = dt_LoadAlocItem;

            int count = MontarCarga.bunifuCustomDataGrid2.RowCount;

            if (count > 0)
            {
                for (int i = 0; i < count; i ++)
                {
                    int pedido        = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[0].Value.ToString());
                    long item         = Convert.ToInt64(MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[1].Value.ToString());
                    int qnt_ped       = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[2].Value.ToString());
                    int saldo_vst     = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[5].Value.ToString());
                    string serie      = MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[7].Value.ToString();
                    int sequencia     = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[8].Value.ToString());
                    int multiplicador = CoreViaSoft.ObterMultiplicadorItem(item);
                    bool temMult      = multiplicador > 0;
                    int qnt_disp      = qnt_ped - (temMult ? (CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item) / multiplicador) : CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item));

                    if (qnt_disp > 0 && saldo_vst > 0)
                    {
                        dt_LoadAlocItem.Rows.Add(pedido, item, qnt_disp, qnt_disp, serie, sequencia);
                    }
                }
            }

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = false;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;
            dataGridView1.Columns[5].ReadOnly = true;
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {

            if (dataGridView1.ColumnCount > 0 && dataGridView1.RowCount > 0)
            {
                int count = dataGridView1.RowCount;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        int pedido         = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value.ToString());
                        long item          = Convert.ToInt64(dataGridView1.Rows[i].Cells[1].Value.ToString());
                        int qnt_aloc       = Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value.ToString());
                        string serie       = dataGridView1.Rows[i].Cells[4].Value.ToString();
                        int sequencia      = Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value.ToString());
                        int multiplicador  = CoreViaSoft.ObterMultiplicadorItem(item);
                        bool temMult       = multiplicador > 0;
                        int qnt_envia      = temMult ? (qnt_aloc * multiplicador) : qnt_aloc;

                        CoreViaSoft.AlteraQuantidadeEntregue(pedido, serie, item, qnt_envia); // Alterar codigo

                        int qnt_ped = temMult ? (CoreViaSoft.ObterQuantidadePedido(pedido, serie, item) / multiplicador) : CoreViaSoft.ObterQuantidadePedido(pedido, serie, item);
                        int qnt_ent = temMult ? (CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item) / multiplicador) : CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item);

                        // Dados do Grid de Pedidos Selecionados
                        int tipo = 0;
                        int countGridSelecionados = MontarCarga.bunifuCustomDataGrid2.RowCount;
                        if (countGridSelecionados > 0)
                        {
                            for (int x = countGridSelecionados; x > 0; x--)
                            {
                                int pedido_   = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid2.Rows[x - 1].Cells[0].Value.ToString());
                                long item_    = Convert.ToInt64(MontarCarga.bunifuCustomDataGrid2.Rows[x - 1].Cells[1].Value.ToString());
                                string serie_ = MontarCarga.bunifuCustomDataGrid2.Rows[x - 1].Cells[7].Value.ToString();

                                if (pedido_ == pedido && item_ == item && serie_ == serie)
                                {
                                    // Verifica se o pedido está completamente entregue
                                    if (qnt_ped == qnt_ent)
                                    {
                                        MontarCarga.dt_LoadPedidosSelecionados.Rows.RemoveAt(x - 1);
                                        tipo = 1;
                                    }
                                    // Se ele não estiver completado a quantidade 
                                    // Colori a linha de cor laranja para diferenciar.
                                    else
                                    {
                                        MontarCarga.bunifuCustomDataGrid2.Rows[x - 1].Cells[3].Value = (qnt_ped - qnt_ent);
                                        MontarCarga.bunifuCustomDataGrid2.Rows[x - 1].DefaultCellStyle.ForeColor = Color.OrangeRed;
                                        tipo = 2;
                                    }
                                }
                            }
                        }

                        MontarCarga.RemovePedidoSelecionado(pedido, item, serie);
                        switch (tipo)
                        {
                            case 1:
                                MontarCarga.InsertItensSelecionados(pedido, item, qnt_aloc, serie, sequencia, 2);
                                break;
                            case 2:
                                MontarCarga.InsertItensSelecionados(pedido, item, qnt_aloc, serie, sequencia, 1);
                                break;
                        }

                        MontarCarga.AtualizaVisualSaldo(item);
                        MontarCarga.dt_LoadItensSelecionados.Rows.Add(pedido, item, qnt_aloc, serie, sequencia);
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            if (row >= 0)
            {
                 int pedido        = Convert.ToInt32(dataGridView1.Rows[row].Cells[0].Value.ToString());
                 long item         = Convert.ToInt64(dataGridView1.Rows[row].Cells[1].Value.ToString());
                 string serie      = MontarCarga.bunifuCustomDataGrid2.Rows[row].Cells[7].Value.ToString();
                 int multiplicador = CoreViaSoft.ObterMultiplicadorItem(item);
                 bool temMult      = multiplicador > 0;
                 int qnt_ped       = temMult ? (CoreViaSoft.ObterQuantidadePedido(pedido, serie, item) / multiplicador) : CoreViaSoft.ObterQuantidadePedido(pedido, serie, item);

                 int qnt_aloc      = Convert.ToInt32(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                 int qnt_disp      = qnt_ped - (temMult ? (CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item) / multiplicador) : CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item));

                 if (qnt_aloc > qnt_disp)
                 {
                    MessageBox.Show("Quantidade disponível insuficiente!\r\nO valor será reajustado para a quantidade disponível no sistema caso tenha saldo!", "Saldo Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (qnt_disp > 0) {

                        qnt_aloc = qnt_disp;
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = qnt_aloc;
                    }
                 }
            }
        }
    }
}
