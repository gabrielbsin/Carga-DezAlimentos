using GerarCargaDez.Core;
using GerarCargaDez.Telas;
using GerarCargaDez.Telas.Comercial;
using GerarCargaDez.Telas.Estoque.Inventario;
using GerarCargaDez.Telas.Etiqueta;
using GerarCargaDez.Telas.Expedicao;
using GerarCargaDez.Telas.Fiscal;
using GerarCargaDez.Telas.Home;
using GerarCargaDez.Telas.Logistica;
using GerarCargaDez.Telas.Painel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Integrar2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ImportarPedidos ip = new ImportarPedidos();
            //ip.Show();
        }

        private void InclusãoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InclusaoEmbarque ie = new InclusaoEmbarque();
            ie.Show();
        }

        private void MontarCargaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MontarCarga mc = new MontarCarga();
            mc.Show();
        }

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BunifuFlatButton6_Click(object sender, EventArgs e)
        {
            MontarCarga mc = new MontarCarga();
            mc.Show();
        }

        private void BunifuFlatButton4_Click(object sender, EventArgs e)
        {
            InclusaoEmbarque ie = new InclusaoEmbarque();
            ie.Show();
        }

        private void BunifuFlatButton7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void BunifuFlatButton9_Click(object sender, EventArgs e)
        {
            ListarEmbarques le = new ListarEmbarques();
            le.Show();
        }

        private void BunifuFlatButton10_Click(object sender, EventArgs e)
        {
            EmiteRomaneio er = new EmiteRomaneio();
            er.Show();
        }

        private void BunifuFlatButton2_Click(object sender, EventArgs e)
        {
            ImportarPedidos ip = new ImportarPedidos(false);
            ip.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)    {
            //Etiquetas
            labelQntEtiquetas.Text = String.Format("{0:N0}", Convert.ToInt32(CoreEtiqueta.ObterQntImpressa().ToString()));

            //Pedidos
            var cultureInfo                 = Thread.CurrentThread.CurrentCulture;
            var numberFormatInfo            = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormatInfo.CurrencySymbol = "R$";
            var valorFormatado              = string.Format(numberFormatInfo, "{0:C}", CoreViaSoft.ObterTotalValorPedidos());
            labelTotalPed.Text              = valorFormatado;
            labelAtentido.Text              = CoreViaSoft.ObterTotalPedidoEntregue().ToString();
            labelPendente.Text              = CoreViaSoft.ObterTotalPedidoAguardando().ToString();

            sortUp.Hide();
            sortDown.Hide();
            if (CoreViaSoft.ObterTotalValorPedidosAnt() > CoreViaSoft.ObterTotalValorPedidos())
            {
                sortDown.Show();
                sortUp.Hide();
            } else
            {
                sortDown.Hide();
                sortUp.Show();
            }

            //Embarques
        }

        private void BunifuFlatButton2_Click_1(object sender, EventArgs e)
        {
            AcompanhamentoFiscal af = new AcompanhamentoFiscal();
            af.ShowDialog();
        }

        private void BunifuFlatButton8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void BunifuFlatButton11_Click(object sender, EventArgs e)
        {
            ImportarPedidos ip = new ImportarPedidos(false);
            ip.ShowDialog();
        }

        private void BunifuFlatButton5_Click(object sender, EventArgs e)
        {
            ManutencaoEmbarque me = new ManutencaoEmbarque();
            me.Show();
        }

        private void BunifuFlatButton12_Click(object sender, EventArgs e)
        {
            ExibicaoPedidos ep = new ExibicaoPedidos();
            ep.Show();
        }

        private void BunifuFlatButton13_Click(object sender, EventArgs e)
        {
            ImportarPedidos ip = new ImportarPedidos(false);
            ip.ShowDialog();
        }

        private void bunifuFlatButton14_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void bunifuFlatButton16_Click(object sender, EventArgs e)
        {
            NovaEtiqueta ne = new NovaEtiqueta();
            ne.Show();
        }

        private void bunifuFlatButton19_Click(object sender, EventArgs e)
        {
            Ramais rm = new Ramais();
            rm.Show();
        }

        private void bunifuFlatButton22_Click(object sender, EventArgs e)
        {
            Aniversariantes ant = new Aniversariantes();
            ant.Show();
        }

        private void bunifuFlatButton15_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
        }

        private void bunifuFlatButton23_Click(object sender, EventArgs e)
        {
            Inventario iv = new Inventario();
            iv.Show();
        }
    }
}
