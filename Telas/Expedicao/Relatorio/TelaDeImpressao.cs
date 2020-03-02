using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Expedicao.Relatorio
{
    public partial class TelaDeImpressao : Form
    {
        public TelaDeImpressao()
        {
            InitializeComponent();
        }

        private void TelaDeImpressao_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
