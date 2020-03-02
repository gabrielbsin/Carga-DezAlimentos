using GerarCargaDez.Telas.Home.Consulta.Ramais.Dao;
using GerarCargaDez.Telas.Home.Consulta.Ramais.Model;
using System;
using System.Data;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Home.Consulta.Ramais.Controller
{
    class RamaisController
    {
        public DataTable Listar()
        {
            try
            {
                RamaisDao rd = new RamaisDao();
                DataTable dt = new DataTable();

                dt = rd.Listar();

                return dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }

        public DataTable Pesquisar(RamaisModel rm, int selecao)
        {
            try
            {
                RamaisDao rd = new RamaisDao();
                DataTable dt = new DataTable();

                dt = rd.Pesquisar(rm, selecao);

                return dt;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }
    }
}
