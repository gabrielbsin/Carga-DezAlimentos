using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Expedicao
{
    public partial class Autorizacao : Form
    {
        public Autorizacao()
        {
            InitializeComponent();
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bunifuMaterialTextbox1.Text) || bunifuMaterialTextbox1.Text.Length > 0 && bunifuMaterialTextbox1.Text.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, é necesário informar uma Senha!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = bunifuMaterialTextbox1;
                bunifuMaterialTextbox1.Focus();
                return;
            }

            string senha = bunifuMaterialTextbox1.Text.ToString();

            if (senha == Properties.Settings.Default.senha_exp)
            {
                this.DialogResult = DialogResult.OK;
            } else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
