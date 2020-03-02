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
    public partial class DialogLoader : Form
    {
        public DialogLoader()
        {
            InitializeComponent();
        }

        int count = 0;
        private void TimerLoaderForm_Tick(object sender, EventArgs e)
        {
            if (count > 40)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            count++;
        }
    }
}
