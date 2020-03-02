using GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace GerarCargaDez.Telas.Etiqueta.Consulta.Sequencia.Dao
{
    public class SequenciaDao
    {
        string conecta = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=" + Properties.Settings.Default.db_mysql_etiqueta + "; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        MySqlConnection conexao = null;
        MySqlCommand comando;

        public DataTable Listar()
        {
            try
            {
                conexao = new MySqlConnection(conecta);
                int asseptico = Properties.Settings.Default.finalidadeUsuario ? 1 : 0;
                comando = new MySqlCommand("SELECT * FROM  etiquetas_prontas WHERE asseptico=" + asseptico, conexao);

                MySqlDataAdapter da = new MySqlDataAdapter();
                DataTable dt = new DataTable();

                da.SelectCommand = comando;
                da.Fill(dt);

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable Pesquisar(SequenciaModel sequencia, int selecao)
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                DataTable dt = new DataTable();

                conexao = new MySqlConnection(conecta);

                if (selecao == 0)
                {
                    comando = new MySqlCommand("SELECT * FROM etiquetas_prontas WHERE seq LIKE '%" + sequencia.Seq + "%' ORDER BY seq", conexao);
                }
                else
                {
                    comando = new MySqlCommand("SELECT * FROM etiquetas_prontas WHERE op LIKE '%" + sequencia.OrdemProd + "%' ORDER BY op", conexao);
                }

                da.SelectCommand = comando;

                da.Fill(dt);

                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
