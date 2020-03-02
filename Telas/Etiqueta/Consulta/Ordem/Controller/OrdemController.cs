using GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Dao;
using GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Model;
using System;
using System.Data;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Controller
{
    public class OrdemController
    {
        public DataTable Listar()
        {
            try
            {
                OrdemDao od = new OrdemDao();
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

        public DataTable Pesquisar(OrdemModel odm)
        {
            try
            {
                OrdemDao od = new OrdemDao();
                DataTable dt = new DataTable();

                dt = od.Pesquisar(odm);

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
