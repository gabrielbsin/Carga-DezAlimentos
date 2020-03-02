using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Core
{
    class CoreEtiqueta
    {
        static MySqlConnection conexao_etiqueta = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=" + Properties.Settings.Default.db_mysql_etiqueta + "; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
        static OracleConnection conexao_viasoft = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");

        public static bool ObterAsseptico(int sequencia)
        {
            bool volta = false;
            try
            {
                conexao_etiqueta.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM etiquetas_prontas WHERE seq='" + sequencia + "' AND asseptico='1'", conexao_etiqueta);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.Read())
                {
                    if (int.Parse(drCommand["asseptico"].ToString()) == 1)
                    {
                        volta = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Houve um erro com a conexão com banco de dados!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            finally
            {
                conexao_etiqueta.Close();
            }
            return volta;
        }

        public static bool ObterBaixa(int sequencia)
        {
            bool volta = false;
            try
            {
                conexao_etiqueta.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM etiquetas_prontas WHERE seq='" + sequencia + "'", conexao_etiqueta);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.Read())
                {
                    if (int.Parse(drCommand["considera"].ToString()) == 2)
                    {
                        volta = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Houve um erro com a conexão com banco de dados!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            finally
            {
                conexao_etiqueta.Close();
            }
            return volta;
        }

        public static bool ObterStatusEtiqueta(int sequencia)
        {
            bool volta = false;
            try
            {
                conexao_etiqueta.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM etiquetas_prontas WHERE seq='" + sequencia + "'", conexao_etiqueta);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.Read())
                {
                    if (int.Parse(drCommand["considera"].ToString()) == 1)
                    {
                        volta = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Houve um erro com a conexão com banco de dados!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            }
            finally
            {
                conexao_etiqueta.Close();
            }
            return volta;
        }

        public static int ObterQntImpressa()
        {
            int qntImpressa = 0;
            try
            {
                conexao_etiqueta.Open();
                MySqlCommand comando = new MySqlCommand("SELECT MAX(seq) FROM etiquetas_prontas", conexao_etiqueta);
                qntImpressa = Convert.ToInt32(comando.ExecuteScalar().ToString());
                
            }
            catch
            {
                MessageBox.Show("Houve um erro com a conexão com banco de dados!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return 0;
            }
            finally
            {
                conexao_etiqueta.Close();
            }
            return qntImpressa;
        }
    }
}
