using GerarCargaDez.Telas.Logistica.Consulta.Pedido.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace GerarCargaDez.Telas.Logistica.Consulta.Pedido.Dao
{
    class PedidoDao
    {
        string conecta = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=" + Properties.Settings.Default.db_mysql + "; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        MySqlConnection conexao = null;
        MySqlCommand comando;

        public DataTable Listar()
        {
            try
            {
                conexao = new MySqlConnection(conecta);
                comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE status='N' ORDER BY pedido_id", conexao);

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

        public DataTable Pesquisar(PedidoModel em, int selecao)
        {
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter();
                DataTable dt = new DataTable();

                conexao = new MySqlConnection(conecta);

                switch (selecao)
                {
                    case 0: // Pedido
                        comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE pedido_id LIKE '%" + em.Pedido_Id + "%' AND status='N' ORDER BY pedido_id", conexao);
                        break;
                    case 1: // Cod. Cliente
                        comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE pessoa_id LIKE '%" + em.Pessoa_Id + "%' AND status='N' ORDER BY pessoa_id", conexao);
                        break;
                    case 2: // Client Descricao
                        comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE pessoa_desc LIKE '%" + em.Pessoa_Desc + "%' AND status='N' ORDER BY pessoa_desc", conexao);
                        break;
                    case 3: // Cidade
                        comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE cidade_desc LIKE '%" + em.Cidade_nome + "%' AND status='N' ORDER BY cidade_desc", conexao);
                        break;
                    case 4: // UF
                        comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE cidade_uf LIKE '%" + em.UF_Desc + "%' AND status='N' ORDER BY cidade_uf", conexao);
                        break;
                    case 5: // Usuario
                        comando = new MySqlCommand("SELECT pedido_id, pessoa_id, pessoa_desc, cidade_desc, cidade_uf, usuario FROM cargadez_pedido WHERE usuario LIKE '%" + em.User_Desc + "%' AND status='N' ORDER BY usuario", conexao);
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
