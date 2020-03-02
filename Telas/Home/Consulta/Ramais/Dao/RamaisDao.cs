using GerarCargaDez.Telas.Home.Consulta.Ramais.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace GerarCargaDez.Telas.Home.Consulta.Ramais.Dao
{
    class RamaisDao
    {
        string conecta = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=" + Properties.Settings.Default.db_mysql + "; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        MySqlConnection conexao = null;
        MySqlCommand comando;

        public DataTable Listar()
        {
            try
            {
                conexao = new MySqlConnection(conecta);
                comando = new MySqlCommand("SELECT * FROM  cargadez_ramais", conexao);

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

        public DataTable Pesquisar(RamaisModel ramal, int selecao)
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                DataTable dt = new DataTable();

                conexao = new MySqlConnection(conecta);

                switch(selecao)
                {
                    case 0:
                        comando = new MySqlCommand("SELECT * FROM cargadez_ramais WHERE ramal LIKE '%" + ramal.Numero + "%' ORDER BY ramal", conexao);
                        break;
                    case 1:
                        comando = new MySqlCommand("SELECT * FROM cargadez_ramais WHERE nome LIKE '%" + ramal.Nome + "%' ORDER BY nome", conexao);
                        break;
                    case 2:
                        comando = new MySqlCommand("SELECT * FROM cargadez_ramais WHERE departamento LIKE '%" + ramal.Departamento + "%' ORDER BY departamento", conexao);
                        break;
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
