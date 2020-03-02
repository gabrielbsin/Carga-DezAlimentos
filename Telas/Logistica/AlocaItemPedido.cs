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
    public partial class AlocaItemPedido : Form
    {
        public AlocaItemPedido()
        {
            InitializeComponent();
        }

        DataTable dt_LoadAlocItem;

        private void BunifuFlatButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AlocaItemPedido_Load(object sender, EventArgs e)
        {
            dt_LoadAlocItem = new DataTable();
            dt_LoadAlocItem.Columns.Add(new DataColumn("Pedido", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Item", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Quantidade (CX)", typeof(int)));
            dt_LoadAlocItem.Columns.Add(new DataColumn("Sequência Item", typeof(int)));
            dataGridView1.DataSource = dt_LoadAlocItem;

            dt_LoadAlocItem.Rows.Add(MontarCarga.alocaItemPedido, MontarCarga.alocaItemId, MontarCarga.alocaItemQntd, MontarCarga.alocaItemSeq);

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MontarCarga.alocaItemQntd = Convert.ToInt32(dataGridView1.Rows[0].Cells[2].Value.ToString());
            MessageBox.Show(MontarCarga.alocaItemQntd.ToString());
        }

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
