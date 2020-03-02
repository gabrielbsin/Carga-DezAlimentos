using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class PreLoader : UserControl
    {
        public PreLoader()
        {
            InitializeComponent();
        }

        int dir = 1;
        private void TempLoader_Tick(object sender, EventArgs e)
        {
            if (bunifuCircleProgressbar1.Value == 90)
            {
               --dir;
                bunifuCircleProgressbar1.animationIterval = 4;
            } else if (bunifuCircleProgressbar1.Value == 10)
            {
                dir++;
                bunifuCircleProgressbar1.animationIterval = 2;
            }
            bunifuCircleProgressbar1.Value += dir;
        }
    }
}
