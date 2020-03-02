using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Home
{
    public partial class Aniversariantes : Form
    {
        public Aniversariantes()
        {
            InitializeComponent();
        }

        private void botaoMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void botaoFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
