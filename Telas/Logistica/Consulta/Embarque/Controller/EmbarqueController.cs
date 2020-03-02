using GerarCargaDez.Telas.Logistica.Consulta.Embarque.Dao;
using GerarCargaDez.Telas.Logistica.Consulta.Embarque.Model;
using System;
using System.Data;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica.Consulta.Embarque.Controller
{
    class EmbarqueController
    {
        public DataTable Listar()
        {
            try
            {
                EmbarqueDao od = new EmbarqueDao();
                DataTable dt = new DataTable();

                dt = od.Listar();

                return dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }

        public DataTable Pesquisar(EmbarqueModel odm, int selecao)
        {
            try
            {
                EmbarqueDao od = new EmbarqueDao();
                DataTable dt = new DataTable();

                dt = od.Pesquisar(odm, selecao);

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
