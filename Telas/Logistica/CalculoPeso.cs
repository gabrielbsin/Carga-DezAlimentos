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
    public partial class CalculoPeso : Form
    {
        public CalculoPeso()
        {
            InitializeComponent();
        }
        private void CalculoPeso_Load(object sender, EventArgs e)
        {
            double liq = MontarCarga.liquido;
            double bru = MontarCarga.bruto;
            double tot = MontarCarga.totalItens;
            textBox1.Text = string.Format("{0:N}", liq);
            textBox2.Text = string.Format("{0:N}", bru);
            textBox3.Text = string.Format("{0:N}", tot);
        }
    }
}
