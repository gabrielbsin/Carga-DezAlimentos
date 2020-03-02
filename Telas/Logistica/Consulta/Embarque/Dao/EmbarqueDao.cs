using GerarCargaDez.Telas.Logistica.Consulta.Embarque.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace GerarCargaDez.Telas.Logistica.Consulta.Embarque.Dao
{
    class EmbarqueDao
    {
        string conecta = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=" + Properties.Settings.Default.db_mysql + "; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        MySqlConnection conexao = null;
        MySqlCommand comando;

        public DataTable Listar()
        {
            try
            {
                conexao = new MySqlConnection(conecta);
                comando = new MySqlCommand("SELECT estab, embarque_id, preparador, transportador, transportador_nome, motorista, placa, uf_motorista, data_embarque FROM cargadez_embarque ORDER BY embarque_id", conexao);

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

        public DataTable Pesquisar(EmbarqueModel em, int selecao)
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                DataTable dt = new DataTable();

                conexao = new MySqlConnection(conecta);

                switch(selecao)
                {
                    case 0: // Número do Embarque
                        comando = new MySqlCommand("SELECT estab, embarque_id, preparador, transportador, transportador_nome, motorista, placa, uf_motorista, data_embarque FROM cargadez_embarque WHERE embarque_id LIKE '%" + em.Embarque_id + "%' ORDER BY embarque_id", conexao);
                        break;
                    case 1: // Transportador
                        comando = new MySqlCommand("SELECT estab, embarque_id, preparador, transportador, transportador_nome, motorista, placa, uf_motorista, data_embarque FROM cargadez_embarque WHERE transportador_nome LIKE '%" + em.Transportador_desc + "%' ORDER BY transportador_nome", conexao);
                        break;
                    case 2: // Motorista
                        comando = new MySqlCommand("SELECT estab, embarque_id, preparador, transportador, transportador_nome, motorista, placa, uf_motorista, data_embarque FROM cargadez_embarque WHERE motorista LIKE '%" + em.Motorista_desc + "%' ORDER BY motorista", conexao);
                        break;
                }

                da.SelectCommand = comando;

                da.Fill(dt);

                conexao.Close();

                return dt;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
