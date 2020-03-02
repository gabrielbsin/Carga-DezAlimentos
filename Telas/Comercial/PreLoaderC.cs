using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Comercial
{
    public partial class PreLoaderC : UserControl
    {
        public PreLoaderC()
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
