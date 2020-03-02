using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Comercial
{
    public partial class ImportarPedidos : Form
    {
        bool force = false;
        public ImportarPedidos(bool force)
        {
            InitializeComponent();
            this.force = force;
        }

        OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");

        private static List<int> pedidos = new List<int>();
        private static List<int> pedidos_excluir = new List<int>();

        private void VerificaPedidosMySQL()
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT * FROM cargadez_pedido", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int pedidoId = Convert.ToInt32(drCommand["pedido_id"]);
                        int status = getPedidoOracleStatus(pedidoId);

                        switch(status)
                        {
                            case 0:
                                UpdatePedidosCargaMySQL(pedidoId, 0);
                                break;
                            case 1:
                                UpdatePedidosCargaMySQL(pedidoId, 1);
                                break;
                            case 2:
                                UpdatePedidosCargaMySQL(pedidoId, 2);
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar pedidos MySQL1!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private int getPedidoOracleStatus(int pedido)
        {
            try
            {
                string query = "SELECT NUMERO, PESSOA, PPESCLI.NOME, STATUS, DTEMISSAO, USERID FROM PEDCAB INNER JOIN PPESCLI ON PPESCLI.CLIENTE = PEDCAB.PESSOA WHERE NUMERO = '" + pedido + "' AND SERIE = 'PV'";
                conn.Open();
                OracleCommand comando = new OracleCommand(query, conn);

                OracleDataReader odr = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        string status = odr["STATUS"].ToString();
                        switch (status)
                        {
                            case "B": return 0;
                            case "C": return 1;
                            case "N": return 4;
                            default: return 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar pedidos ViaSoft!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conn.Close();
            }
            return 3;
        }

        private void UpdatePedidosCargaMySQL(int pedido_id, int option)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM cargadez_pedido WHERE pedido_id='" + pedido_id + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int pedidoId = Convert.ToInt32(drCommand["pedido_id"]);
                        if (pedido_id == pedidoId)
                        {
                            switch(option)
                            {
                                case 0:
                                    AlterarEstado("UPDATE cargadez_pedido SET status='B' WHERE pedido_id='" + pedidoId + "'");
                                    break;
                                case 1:
                                    AlterarEstado("UPDATE cargadez_pedido SET status='C' WHERE pedido_id='" + pedidoId + "'");
                                    break;
                                case 2:
                                    AlterarEstado("DELETE FROM cargadez_pedido WHERE pedido_id='" + pedidoId + "'");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar pedidos MySQL0!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void AlterarEstado(string query)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand(query, conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                conexao.Close();
            }
        }
        
        private void VerificaPedidosCargaMySQL(int pedido_id)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT * FROM cargadez_embarque_itens WHERE pedido_id='" + pedido_id + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int pedidoId = Convert.ToInt32(drCommand["pedido_id"]);
                        if (pedidoId == pedido_id)
                        {
                            comando = new MySqlCommand("DELETE FROM cargadez_embarque_itens WHERE pedido_id='" + pedidoId + "'", conexao);
                            comando.ExecuteNonQuery();
                            comando.Dispose();
                        }
                    }
                }
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar pedidos MySQL3!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void CarregaPedidos()
        {
            try
            {
                conn.Open();
                OracleCommand comando = new OracleCommand("SELECT NUMERO, PESSOA, PPESCLI.NOME, STATUS, DTEMISSAO, USERID FROM PEDCAB INNER JOIN PPESCLI ON PPESCLI.CLIENTE = PEDCAB.PESSOA WHERE SERIE = 'PV'", conn);
                OracleDataReader odr = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        string status = odr["STATUS"].ToString();
                        int pedidoId = Convert.ToInt32(odr["NUMERO"]);
                        switch (status)
                        {
                            case "N":
                            case "B":
                            case "C":
                                if (getPedidoMySQL(pedidoId) == false)
                                {
                                    pedidos.Add(pedidoId);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar pedidos!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                this.Close();
            }
            finally
            {
                conn.Close();
            }
        }

        private bool getPedidoMySQL(int pedido)
        {
            bool contem = false;
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT * FROM cargadez_pedido WHERE pedido_id='" + pedido + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    contem = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar pedidos MySQL!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return true;
            }
            finally
            {
                conexao.Close();
            }

            return contem;
        }

        private bool getPedidoOracle(int pedido)
        {
            bool contem = false;
            try
            {
                string query = "SELECT NUMERO, PESSOA, PPESCLI.NOME, STATUS, DTEMISSAO, USERID FROM PEDCAB INNER JOIN PPESCLI ON PPESCLI.CLIENTE = PEDCAB.PESSOA WHERE NUMERO = '" + pedido + "' AND SERIE = 'PV'";
                conn.Open();
                OracleCommand comando = new OracleCommand(query, conn);

                OracleDataReader odr = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        string status = odr["STATUS"].ToString();
                        if (status == "C")
                        {
                            contem = false;
                        }
                        else
                        {
                            contem = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar pedidos ViaSoft!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return true;
            }
            finally
            {
                conn.Close();
            }
            return contem;
        }

        private void insertMySQLPedido(int pedido)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");

            try
            {
                OracleCommand comando;
                string query = "SELECT NUMERO, PESSOA, PPESCLI.NOME, PPESCLI.CIDADE, CIDADE.NOME AS NOMECIDADE, CIDADE.UF, PPESCLI.CEP, PPESCLI.ENDERECO, PPESCLI.BAIRRO, STATUS, DTEMISSAO, USERID FROM PEDCAB INNER JOIN PPESCLI ON PPESCLI.CLIENTE = PEDCAB.PESSOA INNER JOIN CIDADE ON CIDADE.CIDADE = PPESCLI.CIDADE WHERE NUMERO=" + pedido + " AND SERIE='PV'";
                conn.Open();
                comando = new OracleCommand(query, conn);

                OracleDataReader odr = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                            try
                            {
                                conexao.Open();
                                MySqlCommand comando_mysql = new MySqlCommand("INSERT INTO cargadez_pedido (pedido_id, data_emissao, pessoa_id, pessoa_desc, status, cidade_ibge, cidade_desc, cidade_uf, endereco, bairro, usuario, dataimport) VALUES (@param, @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9, @param10, NOW())", conexao);
                                comando_mysql.Parameters.Add("@param", MySqlDbType.Int32).Value = Convert.ToInt32(odr["NUMERO"]);
                                comando_mysql.Parameters.Add("@param1", MySqlDbType.DateTime).Value = Convert.ToDateTime(odr["DTEMISSAO"]);
                                comando_mysql.Parameters.Add("@param2", MySqlDbType.Int32).Value = Convert.ToInt32(odr["PESSOA"]);
                                comando_mysql.Parameters.Add("@param3", MySqlDbType.VarChar).Value = odr["NOME"].ToString();
                                comando_mysql.Parameters.Add("@param4", MySqlDbType.VarChar).Value = odr["STATUS"].ToString();
                                comando_mysql.Parameters.Add("@param5", MySqlDbType.VarChar).Value = odr["CIDADE"].ToString();
                                comando_mysql.Parameters.Add("@param6", MySqlDbType.VarChar).Value = odr["NOMECIDADE"].ToString();
                                comando_mysql.Parameters.Add("@param7", MySqlDbType.VarChar).Value = odr["UF"].ToString();
                                comando_mysql.Parameters.Add("@param8", MySqlDbType.VarChar).Value = odr["ENDERECO"].ToString();
                                comando_mysql.Parameters.Add("@param9", MySqlDbType.VarChar).Value = odr["BAIRRO"].ToString();
                                comando_mysql.Parameters.Add("@param10", MySqlDbType.VarChar).Value = odr["USERID"].ToString();
                                MySqlDataReader drCommand = comando_mysql.ExecuteReader();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Houve um erro ao inserir pedidos MySQL!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            }
                            finally
                            {
                                conexao.Close();
                            }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao inserir pedidos MySQL!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conn.Close();
            }

        }

        private void insertMySQLPedidoItem(int pedido)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                OracleCommand comando;
                string query = "SELECT NUMERO, SEQPEDITE, ITEM, QUANTIDADE, VALOR, DTEMISSAO, EMBALAGEM, VALORORIGINAL, QTDEMB, VALORUNITARIO, CAIXA, EMBALAGEM2, QTDEMBCOM, VLRUNITEMBCOM FROM PEDITEM WHERE NUMERO='" + pedido + "' AND SERIE='PV'";
                conn.Open();
                comando = new OracleCommand(query, conn);

                OracleDataReader odr = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        try
                        {
                            conexao.Open();
                            MySqlCommand comando_mysql = new MySqlCommand("INSERT INTO cargadez_pedido_item (pedido_id, seqpedite, item, quantidade, valor, dtemissao, dataimport) VALUES (@param, @param1, @param2, @param3, @param4, @param5, NOW())", conexao);
                            comando_mysql.Parameters.Add("@param", MySqlDbType.Int32).Value = Convert.ToInt32(odr["NUMERO"]);
                            comando_mysql.Parameters.Add("@param1", MySqlDbType.Int32).Value = Convert.ToInt32(odr["SEQPEDITE"]);
                            comando_mysql.Parameters.Add("@param2", MySqlDbType.Int32).Value = Convert.ToInt32(odr["ITEM"]);
                            comando_mysql.Parameters.Add("@param3", MySqlDbType.Int32).Value = Convert.ToInt32(odr["QUANTIDADE"]);
                            comando_mysql.Parameters.Add("@param4", MySqlDbType.Double).Value = Convert.ToDouble(odr["VALOR"]);
                            comando_mysql.Parameters.Add("@param5", MySqlDbType.DateTime).Value = Convert.ToDateTime(odr["DTEMISSAO"]);
                            MySqlDataReader drCommand = comando_mysql.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Houve um erro ao inserir itens do pedido MySQL!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                        finally
                        {
                            conexao.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao inserir itens do pedido MySQL!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conn.Close();
            }
        }

        public void deleteMySQLPedido(int pedido)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando_mysql = new MySqlCommand("DELETE FROM cargadez_pedido WHERE pedido_id='" + pedido + "'", conexao);
                comando_mysql.ExecuteNonQuery();
                comando_mysql.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao deletar pedido MySQL!\n" + ex.ToString(), "Integrar Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        public void IniciaImportacao()
        {

            CarregaPedidos();
            for (var i = 0; i < pedidos.Count; i++)
            {
                insertMySQLPedido(pedidos[i]);
                insertMySQLPedidoItem(pedidos[i]);
            }

            VerificaPedidosMySQL();

            pedidos.Clear();

            this.DialogResult = DialogResult.OK;
        }

        public bool loaded = false;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!loaded)
            {
                IniciaImportacao();
                timer1.Stop();
                loaded = false;
            }
        }
    }
}
