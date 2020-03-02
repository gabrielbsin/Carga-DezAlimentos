using GerarCargaDez.Telas.Home.Consulta.Ramais.Controller;
using GerarCargaDez.Telas.Home.Consulta.Ramais.Model;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Home
{
    public partial class Ramais : Form
    {
        public Ramais()
        {
            InitializeComponent(); 
            metroComboBox1.SelectedIndex = 0;
        }

        private void botaoMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void botaoFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Ramais_Load(object sender, EventArgs e)
        {
            RamaisController rc = new RamaisController();

            bunifuCustomDataGrid4.DataSource = rc.Listar();

            bunifuCustomDataGrid4.Columns[0].HeaderText = "Ramal";
            bunifuCustomDataGrid4.Columns[1].HeaderText = "Nome";
            bunifuCustomDataGrid4.Columns[2].HeaderText = "Departamento";
        }

        private void Pesquisar(RamaisModel rm)
        {
            int comboIndex = metroComboBox1.SelectedIndex;
            switch (comboIndex)
            {
                case 0: // Numero 
                    rm.Numero       = bunifuMaterialTextbox1.Text;
                    break;
                case 1: // Nome 
                    rm.Nome         = bunifuMaterialTextbox1.Text;
                    break;
                case 2: // Departamento
                    rm.Departamento = bunifuMaterialTextbox1.Text;
                    break;
            }

            RamaisController rc = new RamaisController();

            bunifuCustomDataGrid4.DataSource = rc.Pesquisar(rm, metroComboBox1.SelectedIndex);
        }

        private void bunifuMaterialTextbox1_OnValueChanged(object sender, EventArgs e)
        {
            if (bunifuMaterialTextbox1.Text == "")
            {
                RamaisController rc = new RamaisController();
                bunifuCustomDataGrid4.DataSource = rc.Listar();
            }
            else
            {
                RamaisModel rm = new RamaisModel();
                Pesquisar(rm);
            }
        }
    }
}
