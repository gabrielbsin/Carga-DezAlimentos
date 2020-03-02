using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Etiqueta
{
    public partial class QuantidadeImpressao : Form
    {
        public QuantidadeImpressao()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox1.Text) < 1 || Convert.ToInt32(textBox1.Text) > 99)
            {
                MessageBox.Show("Estes valores não são permitidos!");
                return;
            }

            NovaEtiqueta.varQuantidadeImpressao = Convert.ToInt32(textBox1.Text);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}