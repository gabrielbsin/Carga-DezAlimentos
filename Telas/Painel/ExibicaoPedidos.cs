using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Painel
{
    public partial class ExibicaoPedidos : Form
    {
        public ExibicaoPedidos()
        {
            InitializeComponent();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
                int minx, miny, maxx, maxy;
                minx = miny = int.MaxValue;
                maxx = maxy = int.MinValue;

                foreach (Screen screen in Screen.AllScreens)
                {
                    var bounds = screen.Bounds;
                    minx = Math.Min(minx, bounds.X);
                    miny = Math.Min(miny, bounds.Y);
                    maxx = Math.Max(maxx, bounds.Right);
                    maxy = Math.Max(maxy, bounds.Bottom);
                }

                ExibicaoPedidos fs = new ExibicaoPedidos();
                fs.Activate();
                Rectangle tempRect = new Rectangle(1, 0, maxx, maxy);
                this.DesktopBounds = tempRect;
            
        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void TableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
