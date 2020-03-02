using GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Dao;
using GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Model;
using System;
using System.Data;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Controller
{
    public class SequenciaController
    {
        public DataTable Listar()
        {
            try
            {
                SequenciaDao sd = new SequenciaDao();
                DataTable dt = new DataTable();

                dt = sd.Listar();

                return dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }

        public DataTable Pesquisar(SequenciaModel sm, int selecao)
        {
            try
            {
                SequenciaDao od = new SequenciaDao();
                DataTable dt = new DataTable();

                dt = od.Pesquisar(sm, selecao);

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
