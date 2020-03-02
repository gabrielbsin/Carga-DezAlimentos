using GerarCargaDez.Telas.Logistica.Consulta.Dao;
using GerarCargaDez.Telas.Logistica.Consulta.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica.Consulta.Controller
{
    class TransportadorController
    {
        public DataTable Listar()
        {
            try
            {
                TransportadorDao td = new TransportadorDao();
                DataTable dt = new DataTable();

                dt = td.Listar();

                return dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }

        public DataTable Pesquisar(TransportadorModel tm, int selecao)
        {
            try
            {
                TransportadorDao td = new TransportadorDao();
                DataTable dt = new DataTable();

                dt = td.Pesquisar(tm, selecao);

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
