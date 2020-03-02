using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Core
{
    class CoreViaSoft
    {
        static MySqlConnection conexao_carga = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
        static OracleConnection conexao_viasoft = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");

        public static string ObterDadosCidade(string cidade, string uf_, bool uf)
        {
            string dados = "";
            try
            {
                conexao_carga.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_CIDADES WHERE CIDADE='" + cidade + "' AND UF='" + uf_ + "'", conexao_carga);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        dados = uf ? drCommand["UF"].ToString() : drCommand["NOME"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar dados da cidade!\n" + ex.ToString(), "Obter dados da Cidade", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_carga.Close();
            }
            return dados;
        }

        public static int ObterMultiplicadorItem(long itemId)
        {
            int multiplicador = 0;
            try
            {
                OracleCommand cmd;
                string query;

                query = "SELECT * FROM ITEMAGRO WHERE ITEM=" + itemId + "";
                conexao_viasoft.Open();
                cmd = new OracleCommand(query, conexao_viasoft);

                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.Read())
                {
                    multiplicador = Convert.ToInt32(odr["MULTIPLO"].ToString());
                }
            }
            catch (Exception ex)
            {
                return multiplicador;
            }
            finally
            {
                conexao_viasoft.Close();
            }

            return multiplicador;
        }

        public static string ObterDadosItem(long itemId, bool unidade)
        {
            string dadosItem = "";
            try
            {
                OracleCommand cmd;
                string query = "SELECT * FROM ITEMAGRO WHERE ITEM=" + itemId + "";
                conexao_viasoft.Open();
                cmd = new OracleCommand(query, conexao_viasoft);

                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    if (odr.Read())
                    {
                        dadosItem = unidade ? odr["UNIDADE"].ToString() : odr["DESCRICAO"].ToString();
                    }
                }
                else
                {
                    MessageBox.Show("Erro ao buscar " + (unidade ? "unidade" : "descrição") + " do Item: " + itemId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao buscar dados do Item ViaSoft!\n" + ex.ToString(), "Dados do Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return dadosItem;
        }

        public static string ObterSerieItemPedido(int pedido_id)
        {
            string serie = "";
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM PEDITEM WHERE NUMERO=" + pedido_id + "", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    if (odr.Read())
                    {
                        serie = odr["SERIE"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao buscar dados do Item ViaSoft!\n" + ex.ToString(), "Dados do Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return serie;
        }

        public static int SaldoItem(long item)
        {
            int saldoQuantidade = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT b.estab, b.item, a.descricao, b.ano, b.mes, b.local, localest.descricao, b.saldoquantidade FROM itemagro a JOIN itemsaldo b ON b.estab = '1000' AND b.item = a.item AND b.ano = (SELECT max(c.ano) FROM itemsaldo c WHERE c.estab = '1000' AND c.item = a.item AND c.codigosaldo = '2' AND(c.local = b.local)) AND b.mes = (SELECT max(d.mes) FROM itemsaldo d WHERE d.estab = b.estab AND d.item = b.item AND d.ano = (SELECT max(e.ano) FROM itemsaldo e WHERE d.estab = '1000' AND d.item = a.item AND d.ano = b.ano AND d.codigosaldo = '2' AND(d.local = b.local))) JOIN localest ON b.local = localest.local JOIN ITEMAGROESTAB t ON b.item = t.item AND b.estab = localest.estab WHERE (b.local = t.local) AND b.estab = '1000' AND b.codigosaldo = '2' AND a.item = '" + item + "'", conexao_viasoft);
                OracleDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        saldoQuantidade = Convert.ToInt32(drCommand["SALDOQUANTIDADE"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar saldo do Item ViaSoft!\n" + ex.ToString(), "Saldo do Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return saldoQuantidade;
        }

        public static int SaldoEmbarcado(long item)
        {
            int saldoQuantidade = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT b.estab, b.item, a.descricao, b.ano, b.mes, b.local, localest.descricao, b.saldoquantidade FROM itemagro a JOIN itemsaldo b ON b.estab = '1000' AND b.item = a.item AND b.ano = (SELECT max(c.ano) FROM itemsaldo c WHERE c.estab = '1000' AND c.item = a.item AND c.codigosaldo = '20' AND(c.local = b.local)) AND b.mes = (SELECT max(d.mes) FROM itemsaldo d WHERE d.estab = b.estab AND d.item = b.item AND d.ano = (SELECT max(e.ano) FROM itemsaldo e WHERE d.estab = '1000' AND d.item = a.item AND d.ano = b.ano AND d.codigosaldo = '20' AND(d.local = b.local))) JOIN localest ON b.local = localest.local JOIN ITEMAGROESTAB t ON b.item = t.item AND b.estab = localest.estab WHERE (b.local = t.local) AND b.estab = '1000' AND b.codigosaldo = '20' AND a.item = '" + item + "'", conexao_viasoft);
                OracleDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        saldoQuantidade = Convert.ToInt32(drCommand["SALDOQUANTIDADE"]);
                    }
                }
            }
            catch (Exception ex)
            {
                saldoQuantidade = 0;
                //MessageBox.Show("Houve um erro ao verificar saldo do Item ViaSoft!\n" + ex.ToString(), "Saldo do Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return saldoQuantidade;
        }

        private int QntPalleteItem(long item)
        {
            int paletexcx = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM ITEMAGRO_U WHERE ITEM='" + item + "'", conexao_viasoft);
                OracleDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        paletexcx = Convert.ToInt32(drCommand["CAIXAXPALETES"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar quantidade caixa palete ViaSoft!\n" + ex.ToString(), "Quantidade de Pallete", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return paletexcx;
        }

        public static void AtualizaCaixaItemU(long item_id)
        {
            bool atualiza = false;
            try
            {

                try
                {
                    conexao_carga.Open();
                    MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_ITEM_U WHERE ITEM='" + item_id + "'", conexao_carga);
                    MySqlDataReader drCommand = comando.ExecuteReader();

                    if (drCommand.HasRows)
                    {
                        atualiza = true;
                    }

                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                } finally
                {
                    conexao_carga.Close();
                }
                

                conexao_viasoft.Open();
                string query = "SELECT * FROM ITEMAGRO_U WHERE ITEM='" + item_id + "'";
                OracleCommand cmd = new OracleCommand(query, conexao_viasoft);
                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        try
                        {
                            conexao_carga.Open();
                            string query_atualizar;                            
                            if (atualiza)
                            {
                                query_atualizar = "UPDATE CARGADEZ_ITEM_U SET CAIXAXPALETES='" + odr["CAIXAXPALETES"] + "'";
                            }
                            else
                            {
                                query_atualizar = "INSERT INTO CARGADEZ_ITEM_U (ITEM, CODDEZ, CAIXAXPALETES) VALUES ('" + odr["ITEM"] + "', '" + odr["CODDEZ"] + "', '" + odr["CAIXAXPALETES"] + "')";
                            }
                            MySqlCommand comando = new MySqlCommand(query_atualizar, conexao_carga);
                            comando.ExecuteNonQuery();
                            comando.Dispose();
                        } catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        } finally
                        {
                            conexao_carga.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao buscar atualizar do item ItemU ViaSoft!\n" + ex.ToString(), "Atualizar Informações Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao_viasoft.Close();
            }
        }
            
        public static string ObterDescricaoItem(long itemId)
        {
            string descricaoItem = "";
            try
            {
                OracleCommand cmd;
                string query;

                query = "SELECT * FROM ITEMAGRO WHERE ITEM=" + itemId + "";
                conexao_viasoft.Open();
                cmd = new OracleCommand(query, conexao_viasoft);

                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.Read())
                {
                    descricaoItem = odr["DESCRICAO"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao buscar descrição do item ViaSoft!\n" + ex.ToString(), "Descrição do Item", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return descricaoItem;
        }

        public static int ObterPessoaByPedido(int pedido_id, string serie)
        {
            int client = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM PEDCAB WHERE NUMERO='" + pedido_id + "' AND SERIE='" + serie + "'", conexao_viasoft);
                OracleDataReader odr  = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        client = Convert.ToInt32(odr["PESSOA"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return client;
        }

        public static int ObterQuantidadeEntregue(int pedido_id, string serie, long item_id)
        {
            int quantidadeEntregue = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM EXLCARREGADET WHERE PEDIDO='" + pedido_id + "' AND SERIE='" + serie + "' AND ITEM='" + item_id + "'", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        quantidadeEntregue = Convert.ToInt32(odr["QUANTIDADE"]);
                    }
                }
            } catch (Exception ex)
            {
                return quantidadeEntregue;
            } finally
            {
                conexao_viasoft.Close();
            }
            return quantidadeEntregue;
        }

        public static int ObterQuantidadePedido(int pedido_id, string serie, long item_id)
        {
            int quantidadePedido = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM PEDITEM WHERE NUMERO='" + pedido_id + "' AND SERIE='" + serie + "' AND ITEM='" + item_id + "'", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        quantidadePedido = Convert.ToInt32(odr["QUANTIDADE"]);
                    }
                }
            }
            catch (Exception ex)
            {
                return quantidadePedido;
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return quantidadePedido;
        }

        public static void AlteraQuantidadeEntregue(int pedido_id, string serie, long item_id, int novaQuantidade)
        {
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("UPDATE EXLCARREGADET SET QUANTIDADE=(QUANTIDADE + " + novaQuantidade + ") WHERE PEDIDO='" + pedido_id + "' AND SERIE='" + serie + "' AND ITEM='" + item_id + "'", conexao_viasoft);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao alterar quantidade alocada do item ViaSoft!\n" + ex.ToString(), "Alterar Quantidade", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
        }

        public static bool VerificaVinculoPedido(int pedido_id, string serie)
        {
            bool contemVinculo = false;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT CARGA FROM PEDCAB WHERE NUMERO='" + pedido_id + "' AND SERIE='" + serie + "'", conexao_viasoft);
                OracleDataReader odr  = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while(odr.Read())
                    {
                        int carga = odr["CARGA"] != (object)DBNull.Value ? Convert.ToInt32(odr["CARGA"]) : 0;
                        if (carga > 0) contemVinculo = true;
                    }
                }
            }
            catch (Exception ex)
            {
                contemVinculo = false;
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return contemVinculo;
        }

        public static void VinculaPedidoEmbarque(int embarque_id, int pedido_id, string serie)
        {
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("UPDATE PEDCAB SET CARGA='" + embarque_id + "' WHERE NUMERO='" + pedido_id + "' AND SERIE='" + serie + "'", conexao_viasoft);
                comando.ExecuteNonQuery();
                comando.Dispose();
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao vincular pedido ao embarque!\n" + ex.ToString(), "Vincular Pedido", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
        }

        public static void DesvinculaPedidoEmbarque(int pedido_id, string serie)
        {
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("UPDATE PEDCAB SET CARGA=NULL WHERE NUMERO='" + pedido_id + "' AND SERIE='" + serie + "'", conexao_viasoft);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao desvincular pedido ao embarque!\n" + ex.ToString(), "Desvincular Pedido", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
        }

        public static int ObterUltimarOrdem()
        {
            int ultimaOrdem = 1;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT MAX(IDCARGA) FROM ORDEMCARGACAB", conexao_viasoft);
                ultimaOrdem = Convert.ToInt32(comando.ExecuteScalar().ToString()) + 1;
            }
            catch (Exception ex)
            {
                ultimaOrdem = 1;
                MessageBox.Show("Houve um erro ao carregar dados do último embarque!\n" + ex.ToString(), "Inclusão Embarque", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return ultimaOrdem;
        }

        public static int ObterUltimaTransp()
        {
            int ultimaOrdem = 1;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT MAX(IDTRANSP) FROM ORDEMCARGATRANSP", conexao_viasoft);
                ultimaOrdem = Convert.ToInt32(comando.ExecuteScalar().ToString()) + 1;
            }
            catch (Exception ex)
            {
                ultimaOrdem = 1;
                MessageBox.Show("Houve um erro ao carregar dados do último embarque!\n" + ex.ToString(), "Inclusão Embarque", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return ultimaOrdem;
        }

        public static int ObterItemOrdemProd(int opSearch)
        {
            int idItemOrdemProd = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM ORDEMPRODPA WHERE IDORDEMPROD='" + opSearch + "'", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();
                if (odr.Read())
                {
                    idItemOrdemProd = int.Parse(odr["ITEM"].ToString());
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao buscar Id do Item na Ordem de Produção: " + opSearch);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return idItemOrdemProd;
        }

        public static bool ObterStatusOrdem(int opSearch)
        {
            bool produzida = false;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT * FROM ORDEMPRODCAB WHERE IDORDEMPROD='" + opSearch + "'", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();
                if (odr.Read())
                {
                    if (odr["STATUS"].ToString() == "2")
                    {
                        produzida = true;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao verificar Ordem de Produção: " + opSearch);
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return produzida;
        }

        public static decimal ObterTotalValorPedidos()
        {
            decimal valor = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT SUM(PC.VALORMERCADORIA) AS SOMATOTAL FROM PEDCAB PC WHERE PC.SERIE='PV' AND PC.STATUS='B' AND(PC.DTEMISSAO BETWEEN TO_DATE('01-02-2020', 'DD-MM-YYYY') AND TO_DATE('28-02-2020', 'DD-MM-YYYY'))", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        string valorTotal = odr["SOMATOTAL"] == DBNull.Value ? "0" : odr["SOMATOTAL"].ToString();
                        valorTotal.Replace(",", "");
                        valorTotal.Trim();
                        valor = Math.Round(decimal.Parse(valorTotal.ToString()));
                    }
                }

            } catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter valor total de pedidos.\n" + ex.ToString());
                return 0;
            } finally
            {
                conexao_viasoft.Close();
            }
            return valor;
        }

        public static decimal ObterTotalValorPedidosAnt()
        {
            decimal valor = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT SUM(PC.VALORMERCADORIA) AS SOMATOTAL FROM PEDCAB PC WHERE PC.SERIE='PV' AND PC.STATUS='B' AND(PC.DTEMISSAO BETWEEN TO_DATE('01-01-2020', 'DD-MM-YYYY') AND TO_DATE('31-01-2020', 'DD-MM-YYYY'))", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        string valorTotal = odr["SOMATOTAL"].ToString();
                        valorTotal.Replace(",", "");
                        valorTotal.Trim();
                        valor = Math.Round(decimal.Parse(valorTotal.ToString()));
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter valor total de pedidos.\n" + ex.ToString());
                return 0;
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return valor;
        }

        public static long ObterTotalPedidoEntregue()
        {
            long valor = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT COUNT(*) AS SOMA FROM PEDCAB PC WHERE PC.SERIE='PV' AND PC.STATUS='B' AND(PC.DTEMISSAO BETWEEN TO_DATE('01-02-2020', 'DD-MM-YYYY') AND TO_DATE('28-02-2020', 'DD-MM-YYYY'))", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        valor = long.Parse(odr["SOMA"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter valor total de pedidos.\n" + ex.ToString());
                return 0;
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return valor;
        }

        public static long ObterTotalPedidoAguardando()
        {
            long valor = 0;
            try
            {
                conexao_viasoft.Open();
                OracleCommand comando = new OracleCommand("SELECT COUNT(*) AS SOMA FROM PEDCAB PC WHERE PC.SERIE='PV' AND (PC.STATUS BETWEEN 'C' AND 'N') AND(PC.DTEMISSAO BETWEEN TO_DATE('01-02-2020', 'DD-MM-YYYY') AND TO_DATE('28-02-2020', 'DD-MM-YYYY'))", conexao_viasoft);
                OracleDataReader odr = comando.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        valor = long.Parse(odr["SOMA"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter valor total de pedidos.\n" + ex.ToString());
                return 0;
            }
            finally
            {
                conexao_viasoft.Close();
            }
            return valor;
        }
    }
}
