using GerarCargaDez.Core;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class MontarCarga : Form
    {
        public MontarCarga()
        {
            InitializeComponent();
            CarregaTela();
        }

        public static double liquido      = 0.0;
        public static double bruto        = 0.0;
        public static double totalItens   = 0.0;

        public static int embarque_Id     = 0;
        public static int pedido_final    = 0;

        public static int embarque_final  = 0;
        public static string serie_final  = "";

        public static int alocaItemPedido = 0;
        public static long alocaItemId    = 0;
        public static int alocaItemQntd   = 0;
        public static int alocaItemSeq    = 0;

        public static DataTable dt_LoadPedidos;
        public static DataTable dt_LoadPedidosSelecionados;
        public static DataTable dt_LoadItensSelecionados;
        public static DataGridView dgv = new DataGridView();

        public static List<Tuple<int, long, int, string, int>> AlocaoGeralItem = new List<Tuple<int, long, int, string, int>>();

        static string conexao_viasoft  = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";";
        static string conexao_carga    = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        private void CarregaTela()
        {
            bunifuCustomDataGrid1.Enabled = false;
            bunifuCustomDataGrid2.Enabled = false;
            bunifuCustomDataGrid3.Enabled = false;

            bunifuFlatButton1.Enabled = false;
            bunifuFlatButton4.Enabled = false;

            bunifuImageButton8.Enabled = false;
            bunifuImageButton5.Enabled = false;
            bunifuImageButton7.Enabled = false;
            bunifuImageButton6.Enabled = false;

            bunifuFlatButton1.Enabled = false;
            bunifuFlatButton1.Hide();
            bunifuFlatButton2.Enabled = false;
            bunifuFlatButton2.Hide();
            bunifuFlatButton3.Enabled = false;
            bunifuFlatButton3.Hide();
            bunifuFlatButton4.Enabled = false;
            bunifuFlatButton4.Hide();
            bunifuFlatButton5.Enabled = false;
            bunifuFlatButton5.Hide();
            bunifuFlatButton6.Enabled = false;
            bunifuFlatButton6.Hide();
        }

        private void MontarCarga_Load(object sender, EventArgs e)
        {
            dt_LoadPedidos = new DataTable();
            dt_LoadPedidos.Columns.Add(new DataColumn("Pedido", typeof(int)));
            dt_LoadPedidos.Columns.Add(new DataColumn("Fornecedor", typeof(string)));
            dt_LoadPedidos.Columns.Add(new DataColumn("Cidade", typeof(string)));
            dt_LoadPedidos.Columns.Add(new DataColumn("UF", typeof(string)));
            dt_LoadPedidos.Columns.Add(new DataColumn("Série", typeof(string)));
            bunifuCustomDataGrid1.DataSource = dt_LoadPedidos;

            dt_LoadPedidosSelecionados = new DataTable();
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Pedido", typeof(int)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Item", typeof(int)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Qtd. Pedido (CX)", typeof(int)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Qtd. Restante (CX)", typeof(int)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Disponível (CX)", typeof(int)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Saldo ViaSoft (CX)", typeof(int)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Descrição", typeof(string)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Série", typeof(string)));
            dt_LoadPedidosSelecionados.Columns.Add(new DataColumn("Sequência Item", typeof(int)));
            bunifuCustomDataGrid2.DataSource = dt_LoadPedidosSelecionados;

            dt_LoadItensSelecionados = new DataTable();
            dt_LoadItensSelecionados.Columns.Add(new DataColumn("Pedido", typeof(int)));
            dt_LoadItensSelecionados.Columns.Add(new DataColumn("Item", typeof(int)));
            dt_LoadItensSelecionados.Columns.Add(new DataColumn("Quantidade (CX)", typeof(int)));
            dt_LoadItensSelecionados.Columns.Add(new DataColumn("Série", typeof(string)));
            dt_LoadItensSelecionados.Columns.Add(new DataColumn("Sequência Item", typeof(int)));
            bunifuCustomDataGrid3.DataSource = dt_LoadItensSelecionados;
        }

        private void CarregaPedidosSelecionados(int pedido, string serie)
        {
            if (VerificaPedido(pedido, true, true))
            {
                OracleConnection conexao = new OracleConnection(conexao_viasoft);
                try
                {
                    conexao.Open();

                    OracleCommand comando = new OracleCommand("SELECT * FROM PEDITEM WHERE NUMERO='" + pedido + "' AND SERIE='" + serie + "'", conexao);
                    OracleDataReader odr = comando.ExecuteReader();

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            long item_id         = Convert.ToInt64(odr["ITEM"]);
                            CoreViaSoft.AtualizaCaixaItemU(item_id);

                            int multiplo         = CoreViaSoft.ObterMultiplicadorItem(item_id);
                            bool temMult         = multiplo > 0;

                            int qnt_pedido       = Convert.ToInt32(odr["QUANTIDADE"]);
                            int qnt_pedido_      = temMult ? qnt_pedido / multiplo : qnt_pedido;
                            int qnt_entregue     = temMult ? CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item_id) / multiplo : CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item_id);
                            int restante         = qnt_pedido_ - qnt_entregue;

                            int sequencia        = Convert.ToInt32(odr["SEQPEDITE"]);
                            string serie_        = odr["SERIE"].ToString();
                            string item_Desc     = CoreViaSoft.ObterDadosItem(item_id, false);

                            int saldoViaSoft     = temMult ? (CoreViaSoft.SaldoItem(item_id) / multiplo) : CoreViaSoft.SaldoItem(item_id);
                            int saldoViaSoftEmb  = temMult ? (CoreViaSoft.SaldoEmbarcado(item_id) / multiplo) : CoreViaSoft.SaldoEmbarcado(item_id);

                            // Faz o insert do item no banco de dados MySql
                            InsertPedidosSelecionados(pedido, item_id, qnt_pedido_, serie_, sequencia);

                            // Vincula pedido ao embarque ViaSoft
                            int embarque_id      = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                            CoreViaSoft.VinculaPedidoEmbarque(embarque_id, pedido, serie_);

                            // Carrega pedidos selecionados
                            dt_LoadPedidosSelecionados.Rows.Add(pedido, item_id, qnt_pedido_, restante, saldoViaSoftEmb, saldoViaSoft, item_Desc, serie_, sequencia);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Houve um erro ao carregar os pedidos selecionados!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                } finally
                {
                    conexao.Close();
                }
            } else
            {
                MessageBox.Show("Este item já foi selecionado!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
        }

        private void CarregaPedidosSelecionado(int pedido, long item, string serie)
        {
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            try
            {
                conexao.Open(); 
                OracleCommand comando = new OracleCommand("SELECT * FROM PEDITEM WHERE NUMERO='" + pedido + "' AND ITEM='" + item + "' AND SERIE='" + serie + "'", conexao);
                OracleDataReader odr  = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        long item_id         = Convert.ToInt64(odr["ITEM"]);
                        int multiplo         = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool temMult         = multiplo > 0;
                        int qnt_pedido       = Convert.ToInt32(odr["QUANTIDADE"]);
                        int qnt_pedido_      = temMult ? qnt_pedido / multiplo : qnt_pedido;
                        int qnt_entregue     = temMult ? CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item_id) / multiplo : CoreViaSoft.ObterQuantidadeEntregue(pedido, serie, item_id);
                        string item_Desc     = CoreViaSoft.ObterDadosItem(item_id, false);
                        int saldoViaSoft     = temMult ? (CoreViaSoft.SaldoItem(item_id) / multiplo) : CoreViaSoft.SaldoItem(item_id);
                        int saldoEmbarcado   = temMult ? (CoreViaSoft.SaldoEmbarcado(item_id) / multiplo) : CoreViaSoft.SaldoEmbarcado(item_id);
                        string serie_        = odr["SERIE"].ToString();
                        int sequencia        = Convert.ToInt32(odr["SEQPEDITE"]);
                        int restante         = qnt_pedido_ - qnt_entregue;

                        // Verifica se ao extorna itens para seção selecionados
                        // se ele estava parcial ou completo.
                        bool adicionado      = false;
                        int countGrid        = dt_LoadPedidosSelecionados.Rows.Count;
                        if (countGrid > 0)
                        {
                            for (int i = 0; i < countGrid; i++)
                            {
                                if (!adicionado)
                                {
                                    int pedido_ = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i].Cells[0].Value.ToString());
                                    long item_id_ = Convert.ToInt64(bunifuCustomDataGrid2.Rows[i].Cells[1].Value.ToString());
                                    string serie__ = bunifuCustomDataGrid2.Rows[i].Cells[7].Value.ToString();

                                    // Se houver encontrado algum pedido no Grid2 ele apenas atualiza
                                    if (pedido_ == pedido && item_id_ == item_id && serie__ == serie_)
                                    {
                                        bunifuCustomDataGrid2.Rows[i].Cells[3].Value = restante;
                                        bunifuCustomDataGrid2.Rows[i].Cells[4].Value = saldoEmbarcado;
                                        adicionado = true;
                                    }
                                    // Se nao encontrar adiciona linha com valores totais do pedido
                                    else
                                    {
                                        dt_LoadPedidosSelecionados.Rows.Add(pedido, item_id, qnt_pedido_, restante, saldoEmbarcado, saldoViaSoft, item_Desc, serie_, sequencia);
                                        adicionado = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar os pedidos selecionados!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private bool VerificaPedido(int pedido, bool pedidosSelecionados, bool itensSelecionados)
        {
            if (pedidosSelecionados)
            {
                int count = dt_LoadPedidosSelecionados.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i].Cells[0].Value.ToString());
                    if (pedido_id == pedido)
                    {
                        return false;
                    }
                }
            }
            if (itensSelecionados)
            {
                int count_ = dt_LoadItensSelecionados.Rows.Count;
                for (int i = 0; i < count_; i++)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i].Cells[0].Value.ToString());
                    if (pedido_id == pedido)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void CarregaPedidos(int embarqueId)
        {
            List<int> pedidosCarregar = new List<int>();
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            MySqlConnection conexao_ = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                OracleCommand comando = new OracleCommand("SELECT PC.NUMERO, PC.PESSOA, PC.SERIE, CM.NUMEROCM, CM.NOME, CD.NOME AS CIDADE, CD.UF AS CIDADE_UF FROM PEDCAB PC INNER JOIN CONTAMOV CM ON CM.NUMEROCM = PC.PESSOA INNER JOIN CIDADE CD ON CD.CIDADE = CM.CIDADE WHERE PC.STATUS = 'N' AND PC.SERIE <> 'PC' ORDER BY PC.NUMERO", conexao);
                OracleDataReader odr = comando.ExecuteReader();

                bool found = false;
                if (odr.HasRows)
                {
                    found = true;
                    while (odr.Read())
                    {
                        int pedido_id          = Convert.ToInt32(odr["NUMERO"]);
                        int fornecedor_id      = Convert.ToInt32(odr["NUMEROCM"]);
                        string fornecedor_desc = odr["NOME"].ToString();
                        string cidade          = odr["CIDADE"].ToString();
                        string cidade_uf       = odr["CIDADE_UF"].ToString();
                        string serie           = odr["SERIE"].ToString();

                        dt_LoadPedidos.Rows.Add(pedido_id, (fornecedor_id + " - " + fornecedor_desc), cidade, cidade_uf, serie);
                        foreach (DataGridViewColumn coluna in bunifuCustomDataGrid1.Columns)
                        {
                            coluna.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                }
                odr.Dispose();

                if (found)
                {
                    conexao_.Open();

                    MySqlCommand comando_     = new MySqlCommand("SELECT * FROM cargadez_embarque_itens WHERE embarque_id='" + embarqueId + "'", conexao_);
                    MySqlDataReader drCommand = comando_.ExecuteReader();
                    if (drCommand.HasRows)
                    {
                        while (drCommand.Read())
                        {
                            int pedido_id = Convert.ToInt32(drCommand["pedido_id"]);
                            pedidosCarregar.Add(pedido_id);
                        }
                    }

                    drCommand.Dispose();

                    int countPedidos = bunifuCustomDataGrid1.RowCount;
                    int countCarregar = pedidosCarregar.Count;
                    for (int i = 0; i < countPedidos; i++)
                    {
                        for (int x = 0; x < countCarregar; x++)
                        {
                            int pedido_id = Convert.ToInt32(bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                            if (pedidosCarregar[x] == pedido_id)
                            {
                                bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                            }
                        } 
                    }
                    drCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar os pedidos!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
                conexao_.Close();
            }
        }

        private bool VerificaEmbarque(int embarqueId)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                MySqlCommand comando      = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE WHERE embarque_id='" + embarqueId + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int status = Convert.ToInt32(drCommand["status"]);
                        switch(status)
                        {
                            case 1:
                                return true;
                            case 2:
                                MessageBox.Show("Embarque já esta em separação!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return false;
                            case 3:
                                MessageBox.Show("Embarque já esta aguardando carregamento!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return false;
                            case 4:
                                MessageBox.Show("Embarque já esta carregado!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return false;
                        }
                    }
                } else
                {
                    MessageBox.Show("Não foi encontrado nenhum Embarque com este número!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao verificar embarque!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return false;
            } finally
            {
                conexao.Close();
            } 
            return true;
        }

        private int ContaResultados(int pedido_id, int embarqueId, string serie)
        {
            int mysqlint = 0;
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando      = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarqueId + "' AND pedido_id='" + pedido_id + "' AND serie='" + serie + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        if (Convert.ToInt32(drCommand["selecionado"]) == 1)
                        {
                            mysqlint++;
                        }
                    }
                }
                comando.Dispose();
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao obter resultados!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
            return mysqlint;
        }

        private int ContaResultados(int pedido_id, long item, int embarqueId, string serie)
        {
            int mysqlint = 0;
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarqueId + "' AND pedido_id='" + pedido_id + "' AND serie='" + serie + "' AND item='" + item + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        if (Convert.ToInt32(drCommand["selecionado"]) == 1)
                        {
                            mysqlint++;
                        }
                    }
                }
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao obter resultados!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
            return mysqlint;
        }

        private void AtualizaStatusPedido(int embarque_id, int pedido_id, string serie, int status)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando = null;

                switch(status)
                {
                    case 0: // Pedido somente na aba selecionado (Não selecionado total nem parcial)
                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE_ITENS SET selecionado=0 WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND serie='" + serie + "'", conexao);
                        break;
                    case 1: // Pedido na aba de selecionado e itens selecionados (Parcialmente carregado)
                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE_ITENS SET selecionado=1 WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND serie='" + serie + "'", conexao);
                        break;
                    case 2: // Pedido somente na aba de itens selecionados (Totalmente carregado)
                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE_ITENS SET selecionado=2 WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND serie='" + serie + "'", conexao);
                        break;
                }
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conexao.Close();
            }
        }
            

        private void CarregaPedidosEmbarque(int embarqueId)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                MySqlCommand comando      = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarqueId + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int pedido_id        = Convert.ToInt32(drCommand["pedido_id"]);
                        long item_id         = Convert.ToInt64(drCommand["item"]);

                        int multiplo         = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool temMult         = multiplo > 0;
                        string serie         = drCommand["serie"].ToString();
                        int qnt_pedido       = temMult ? CoreViaSoft.ObterQuantidadePedido(pedido_id, serie, item_id) / multiplo : CoreViaSoft.ObterQuantidadePedido(pedido_id, serie, item_id);
                        int qnt_entregue     = temMult ? CoreViaSoft.ObterQuantidadeEntregue(pedido_id, serie, item_id) / multiplo : CoreViaSoft.ObterQuantidadeEntregue(pedido_id, serie, item_id);
                        string item_Desc     = CoreViaSoft.ObterDadosItem(item_id, false);
                        int selecionado      = Convert.ToInt32(drCommand["selecionado"]);
                        int saldoViaSoft     = temMult ? (CoreViaSoft.SaldoItem(item_id) / multiplo) : CoreViaSoft.SaldoItem(item_id);
                        int saldoEmbarcado   = temMult ? (CoreViaSoft.SaldoEmbarcado(item_id) / multiplo) : CoreViaSoft.SaldoEmbarcado(item_id);
                        int sequencia        = Convert.ToInt32(drCommand["sequencia"]);
                        int restante         = qnt_pedido - qnt_entregue;
                        


                        Color cor            = ContaResultados(pedido_id, embarqueId, serie) > 0 ? Color.Blue : Color.Red;
                        if (pedido_id > 0)
                        {
                            bool found = false;
                            int count = bunifuCustomDataGrid1.Rows.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (!found)
                                {
                                    int rows = Convert.ToInt32(bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                                    if (pedido_id == rows)
                                    {
                                        bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = cor;
                                        found = true;
                                    }
                                }
                            }
                        }

                        switch (selecionado)
                        {
                            case 0: // Pedido somente na aba selecionado (Não selecionado total nem parcial)
                                dt_LoadPedidosSelecionados.Rows.Add(pedido_id, item_id, qnt_pedido, restante, saldoEmbarcado, saldoViaSoft, item_Desc, serie, sequencia);
                                break;
                            case 1: // Pedido na aba de selecionado e itens selecionados (Parcialmente carregado)
                                dt_LoadPedidosSelecionados.Rows.Add(pedido_id, item_id, qnt_pedido, restante, saldoEmbarcado, saldoViaSoft, item_Desc, serie, sequencia);
                                dt_LoadItensSelecionados.Rows.Add(pedido_id, item_id, qnt_entregue, serie, sequencia);
                                break;
                            case 2: // Pedido somente na aba de itens selecionados (Totalmente carregado)
                                dt_LoadItensSelecionados.Rows.Add(pedido_id, item_id, qnt_entregue, serie, sequencia);
                                break;
                        }

                        bool found_ = false;
                        if (ContaResultados(pedido_id, item_id, embarqueId, serie) > 0)
                        {
                            int count_ = bunifuCustomDataGrid2.Rows.Count;
                            for (int i = 0; i < count_; i++)
                            {
                                if (!found_)
                                {
                                    int rows    = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i].Cells[0].Value.ToString());
                                    long item__ = Convert.ToInt64(bunifuCustomDataGrid2.Rows[i].Cells[1].Value.ToString());
                                    if (pedido_id == rows)
                                    {
                                        if (item_id == item__)
                                        {
                                            bunifuCustomDataGrid2.Rows[i].DefaultCellStyle.ForeColor = Color.OrangeRed;
                                            found_ = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar pedidos do Embarque!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        public static double CalculoPedidosLiquido()
        {
            double liquido = 0.0;
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            try
            {
                conexao.Open();

                int count = bunifuCustomDataGrid3.Rows.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        long item_id = Convert.ToInt64(bunifuCustomDataGrid3.Rows[i].Cells[1].Value.ToString());
                        int item_qtd = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i].Cells[2].Value.ToString());
                        int multiplo = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool temMult = multiplo > 0;

                        OracleCommand cmd = new OracleCommand("SELECT PESOLIQUIDO FROM ITEMAGRO WHERE ITEM=" + item_id + "", conexao);

                        OracleDataReader odr = cmd.ExecuteReader();
                        if (odr.HasRows)
                        {
                            if (odr.Read())
                            {
                                double peso = Convert.ToDouble(odr["PESOLIQUIDO"].ToString());
                                double calc = temMult ? (item_qtd * multiplo) * peso : (item_qtd * peso);
                                liquido += calc;
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao calcular peso liquido!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
            return liquido;
        }

        public static double CalculoPedidosLiquido(long item_id, int quant)
        {
            double liquido = 0.0;
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            try
            {
                conexao.Open();

                int multiplo = CoreViaSoft.ObterMultiplicadorItem(item_id);
                bool temMult = multiplo > 0;

                OracleCommand cmd    = new OracleCommand("SELECT PESOLIQUIDO FROM ITEMAGRO WHERE ITEM=" + item_id + "", conexao);
                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    if (odr.Read())
                    {
                        double peso = Convert.ToDouble(odr["PESOLIQUIDO"].ToString());
                        double calc = temMult ? (quant * multiplo) * peso : (quant * peso);
                        liquido    += calc;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao calcular peso liquido!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
            return liquido;
        }

        public static double CalculoPedidosBruto(long item_id, int quant)
        {
            double bruto = 0.0;
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            try
            {
                conexao.Open();

                int multiplo         = CoreViaSoft.ObterMultiplicadorItem(item_id);
                bool temMult         = multiplo > 0;
                OracleCommand cmd    = new OracleCommand("SELECT PESOBRUTO FROM ITEMAGRO WHERE ITEM=" + item_id + "", conexao);
                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    if (odr.Read())
                    {
                        double peso = Convert.ToDouble(odr["PESOBRUTO"].ToString());
                        double calc = temMult ? (quant * multiplo) * peso : (quant * peso);
                        bruto      += calc;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao calcular peso bruto!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
            return bruto;
        }

        public static double CalculoPedidosBruto()
        {
            double bruto = 0.0;
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            try
            {
                conexao.Open();

                int count = bunifuCustomDataGrid3.Rows.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        long item_id = Convert.ToInt64(bunifuCustomDataGrid3.Rows[i].Cells[1].Value.ToString());
                        int item_qtd = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i].Cells[2].Value.ToString());
                        int multiplo = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool temMult = multiplo > 0;

                        OracleCommand cmd    = new OracleCommand("SELECT PESOBRUTO FROM ITEMAGRO WHERE ITEM=" + item_id + "", conexao);
                        OracleDataReader odr = cmd.ExecuteReader();

                        if (odr.HasRows)
                        {
                            if (odr.Read())
                            {
                                double peso = Convert.ToDouble(odr["PESOBRUTO"].ToString());
                                double calc = temMult ? (item_qtd * multiplo) * peso : (item_qtd * peso);
                                bruto      += calc;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao calcular peso bruto!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
            return bruto;
        }

        private static double CalculaTotalItens()
        {
            double total = 0.0;
            int count    = bunifuCustomDataGrid3.Rows.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (bunifuCustomDataGrid3.Rows[i].Cells[2].Value.ToString() != null)
                    {
                        int total_pedido = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i].Cells[2].Value.ToString());
                        total += total_pedido;
                    }
                }
            }
            return total;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Você deseja realmente cancelar?", "Cancelando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            embarque_final = Convert.ToInt32(bunifuMaterialTextbox1.Text);
            MapaDePedidos mdp = new MapaDePedidos();
            if (mdp.ShowDialog() == DialogResult.OK)
            {
                dt_LoadPedidosSelecionados = MapaDePedidos.dt;
            }
        }

        public static void RemovePedidoSelecionado(int pedido_id, long item, string serie)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                int embarque_id      = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                MySqlCommand comando = new MySqlCommand("DELETE FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND item='" + item + "' AND serie='" + serie + "'", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao remover pedido selecionado!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private void RemoveItensSelecionado(int pedido_id, long item, string serie)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                int embarque_id      = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                MySqlCommand comando = new MySqlCommand("DELETE FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND item='" + item + "' AND serie='" + serie + "' AND selecionado='1'", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao remover item selecionado!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private void AlteraDisponivelExcluidos(int pedido_id, long item, string serie, bool soma)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                int embarque_id = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                MySqlCommand comando = new MySqlCommand("SELECT * FROM cargadez_embarque_itens WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND item='" + item + "' AND serie='" + serie + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        long item_id      = Convert.ToInt64(drCommand["item"]);
                        int quantidade    = Convert.ToInt32(drCommand["quantidade"]);
                        int multiplicador = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool temMult      = multiplicador > 0;
                        int quantidade_r  = temMult ? (quantidade * multiplicador) : quantidade;
                        AlteraDisponivel(item_id, quantidade_r, soma);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao atualizar saldo do Item MySQL!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }


        private void BunifuFlatButton4_Click(object sender, EventArgs e)
        {
            liquido = CalculoPedidosLiquido();
            bruto = CalculoPedidosBruto();
            totalItens = CalculaTotalItens();
            CalculoPeso cp = new CalculoPeso();
            cp.Show();
        }

        private void BunifuImageButton5_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid2.CurrentRow != null)
            {
                int colunaSelecionada = bunifuCustomDataGrid2.CurrentRow.Index;
                if (colunaSelecionada >= 0)
                {
                    int pedido_id    = Convert.ToInt32(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[0].Value.ToString());
                    string serie_ped = bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[7].Value.ToString();
                    if (pedido_id > 0)
                    {
                        bool found = false;
                        int count = bunifuCustomDataGrid1.Rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!found)
                            {
                                int rows = Convert.ToInt32(bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                                if (pedido_id == rows)
                                {
                                    bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                                    found = true;
                                }
                            }
                        }

                        // Remove da coluna de pedidos selecionados
                        int count_ = bunifuCustomDataGrid2.Rows.Count;
                        for (int j = count_; j > 0; j--)
                        {
                            int rows = Convert.ToInt32(bunifuCustomDataGrid2.Rows[j - 1].Cells[0].Value.ToString());
                            if (pedido_id == rows)
                            {
                                long item_id = Convert.ToInt32(bunifuCustomDataGrid2.Rows[j - 1].Cells[1].Value.ToString());
                                string serie = bunifuCustomDataGrid2.Rows[j - 1].Cells[7].Value.ToString();
                                RemovePedidoSelecionado(rows, item_id, serie);
                                bunifuCustomDataGrid2.Rows.RemoveAt(bunifuCustomDataGrid2.Rows[j - 1].Index);
                            }
                        }

                        // Verifica se existe item selecionado e se existir remove
                        int count__ = bunifuCustomDataGrid3.Rows.Count;
                        for (int j = count__; j > 0; j--)
                        {
                            int rows = Convert.ToInt32(bunifuCustomDataGrid3.Rows[j - 1].Cells[0].Value.ToString());
                            if (pedido_id == rows)
                            {
                                long item_id = Convert.ToInt32(bunifuCustomDataGrid3.Rows[j - 1].Cells[1].Value.ToString());
                                string serie = bunifuCustomDataGrid2.Rows[j - 1].Cells[4].Value.ToString();
                                AlteraDisponivelExcluidos(rows, item_id, serie, true);
                                RemovePedidoSelecionado(rows, item_id, serie);

                                bunifuCustomDataGrid3.Rows.RemoveAt(bunifuCustomDataGrid3.Rows[j - 1].Index);
                            }
                        }
                        // Desvincula carga do pedido no ViaSoft
                        CoreViaSoft.DesvinculaPedidoEmbarque(pedido_id, serie_ped);
                    }
                }
            }
        }

        public static void AtualizaVisualSaldo(long item_id)
        {
            int count = bunifuCustomDataGrid2.RowCount;
            if (count > 0)
            {
                
                for (int i=0; i < count; i++)
                {
                    long item = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i].Cells[1].Value.ToString());

                    if (item_id == item)
                    {
                        int multiplo        = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool temMult        = multiplo > 0;
                        int saldoDisponivel = temMult ? (CoreViaSoft.SaldoEmbarcado(item_id) / CoreViaSoft.ObterMultiplicadorItem(item_id)) : CoreViaSoft.SaldoEmbarcado(item_id);
                        bunifuCustomDataGrid2.Rows[i].Cells[4].Value = saldoDisponivel.ToString();
                    }
                }
            }
        }

        private void BunifuImageButton7_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid2.CurrentRow != null)
            {
                int colunaSelecionada = bunifuCustomDataGrid2.CurrentRow.Index;

                if (colunaSelecionada >= 0)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[0].Value.ToString());
                    long item_id  = Convert.ToInt64(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[1].Value.ToString());
                    int qnt_aloc  = Convert.ToInt32(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[2].Value.ToString());
                    string serie  = bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[7].Value.ToString();
                    int sequencia = Convert.ToInt32(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[8].Value.ToString());

                    if (pedido_id > 0)
                    {
                        if (VerificaPedido(pedido_id, false, false))
                        {
                            alocaItemPedido = pedido_id;
                            alocaItemId     = item_id;
                            alocaItemQntd   = qnt_aloc;
                            alocaItemSeq    = sequencia;

                            AlocaItemPedido aip = new AlocaItemPedido();
                            if (aip.ShowDialog() == DialogResult.OK)
                            {
                                int multiplo          = CoreViaSoft.ObterMultiplicadorItem(item_id);
                                bool temMult          = multiplo > 0;
                                int saldo             = temMult ? (CoreViaSoft.SaldoItem(item_id) / CoreViaSoft.ObterMultiplicadorItem(item_id)) : CoreViaSoft.SaldoItem(item_id);
                                int saldoEmbarcado    = temMult ? (CoreViaSoft.SaldoEmbarcado(item_id) / CoreViaSoft.ObterMultiplicadorItem(item_id)) : CoreViaSoft.SaldoEmbarcado(item_id);
                                bool bloqueiaSaldo    = Properties.Settings.Default.bloq_saldo;

                                if (saldoEmbarcado < qnt_aloc && bloqueiaSaldo)
                                {
                                    MessageBox.Show("Atenção!\nO item não tem saldo disponível para embarque!", "Saldo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                AlteraDisponivelExcluidos(pedido_id, item_id, serie, false); 
                                RemovePedidoSelecionado(pedido_id, item_id, serie); 
                                InsertItensSelecionados(pedido_id, item_id, alocaItemQntd, serie, sequencia, 1); 
                                AtualizaVisualSaldo(item_id);

                                dt_LoadItensSelecionados.Rows.Add(pedido_id, item_id, alocaItemQntd, serie, sequencia);
                                dt_LoadPedidosSelecionados.Rows.RemoveAt(colunaSelecionada);

                                bool found = false;
                                int count = bunifuCustomDataGrid1.Rows.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    if (!found)
                                    {
                                        int pedido_id_ = Convert.ToInt32(bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                                        if (pedido_id == pedido_id_)
                                        {
                                            bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = Color.Blue;
                                            found = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Este item já foi selecionado!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        }
                    }
                }
            }
        }

        private bool PedidoJaSelecionado(int pedido, string serie)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE pedido_id='" + pedido + "' AND serie='" + serie + "'", conexao);
                MySqlDataReader mdr = comando.ExecuteReader();

                if (mdr.HasRows)
                {
                    return true;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                conexao.Close();
            }
            return false;
        }

        private void BunifuImageButton8_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid1.CurrentRow != null)
            {
                int colunaSelecionada = bunifuCustomDataGrid1.CurrentRow.Index;

                if (colunaSelecionada >= 0)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid1.Rows[colunaSelecionada].Cells[0].Value.ToString());
                    string serie  = bunifuCustomDataGrid1.Rows[colunaSelecionada].Cells[4].Value.ToString();

                    if (pedido_id > 0)
                    {
                        // Verifica pedido se já esta vinculado ha algum embarque
                        if (!CoreViaSoft.VerificaVinculoPedido(pedido_id, serie))
                        {
                            if (VerificaPedido(pedido_id, true, true))
                            {
                                CarregaPedidosSelecionados(pedido_id, serie);
                                bunifuCustomDataGrid1.Rows[colunaSelecionada].DefaultCellStyle.ForeColor = Color.Red;
                            }
                            else
                            {
                                MessageBox.Show("Este pedido já foi selecionado!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            }
                        } else
                        {
                            MessageBox.Show("Este pedido já esta vinculado a outro Embarque!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        }
                    }
                }
            }
        }

        private void BunifuImageButton6_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid3.CurrentRow != null)
            {
                int colunaSelecionada = bunifuCustomDataGrid3.CurrentRow.Index;

                if (colunaSelecionada >= 0)
                {
                    int pedido_id  = Convert.ToInt32(bunifuCustomDataGrid3.Rows[colunaSelecionada].Cells[0].Value.ToString());
                    long item_id   = Convert.ToInt64(bunifuCustomDataGrid3.Rows[colunaSelecionada].Cells[1].Value.ToString());
                    int quantidade = Convert.ToInt32(bunifuCustomDataGrid3.Rows[colunaSelecionada].Cells[2].Value.ToString());
                    int sequencia  = Convert.ToInt32(bunifuCustomDataGrid3.Rows[colunaSelecionada].Cells[4].Value.ToString());
                    string serie   = bunifuCustomDataGrid3.Rows[colunaSelecionada].Cells[3].Value.ToString();

                    int multiplo   = CoreViaSoft.ObterMultiplicadorItem(item_id);
                    bool temMult   = multiplo > 0;
                    int qnt_fat    = temMult ? (quantidade * multiplo) : quantidade;
                    int qnt_Ent    = CoreViaSoft.ObterQuantidadeEntregue(pedido_id, serie, item_id);
                    int qnt_ped    = CoreViaSoft.ObterQuantidadePedido(pedido_id, serie, item_id);

                    int dif        = 0;
                    if (qnt_fat == qnt_Ent)
                    {
                        dif = qnt_ped;
                    }
                    else if (qnt_fat != qnt_Ent)
                    {
                        dif = qnt_fat + qnt_Ent;
                    }

                    if (pedido_id > 0)
                    {

                        CoreViaSoft.AlteraQuantidadeEntregue(pedido_id, serie, item_id, dif);

                        RemoveItensSelecionado(pedido_id, item_id, serie); 
                        InsertPedidosSelecionados(pedido_id, item_id, dif, serie, sequencia);
                        CarregaPedidosSelecionado(pedido_id, item_id, serie);
                        AtualizaVisualSaldo(item_id);


                        int countGridPedidos = bunifuCustomDataGrid1.Rows.Count;
                        if (ContarItensSelecionados(pedido_id) <= 1)
                        {
                            for (int i = 0; i < countGridPedidos; i++)
                            {
                                int pedidoCarregadoVS = Convert.ToInt32(bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                                if (pedido_id == pedidoCarregadoVS)
                                {
                                    bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                                }
                            }
                        } 
                        dt_LoadItensSelecionados.Rows.RemoveAt(colunaSelecionada);
                    }
                }
            }
        }

        private int ContarItensSelecionados(int pedido_id)
        {
            int contagemItens = 0;
            int count         = bunifuCustomDataGrid3.RowCount;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    int pedido = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i].Cells[0].Value.ToString());

                    if (pedido == pedido_id)
                    {
                        contagemItens++;
                    }
                }
            }
            return contagemItens;
        }

        private void AlteraDisponivel(long item_id, int quantidade, bool soma)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                string operacao      = soma ? "(disponivel + " + quantidade + ")" : "(disponivel - " + quantidade + ")";
                MySqlCommand comando = new MySqlCommand("UPDATE cargadez_saldoitem SET disponivel=" + operacao + " WHERE item='" + item_id + "'", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao atualizar saldo do Item MySQL!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private void InsertPedidosSelecionados(int pedido_id, long item_id, int quantidade, string serie, int sequencia)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                int embarque_id   = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                double peso_liq   = CalculoPedidosLiquido(item_id, quantidade);
                double peso_bru   = CalculoPedidosBruto(item_id, quantidade);
                string item_desc  = CoreViaSoft.ObterDadosItem(item_id, false);

                MySqlCommand comando = new MySqlCommand("INSERT INTO CARGADEZ_EMBARQUE_ITENS (embarque_id, pedido_id, item, descricao, quantidade, peso_liq, peso_bru, serie, sequencia, selecionado) VALUES (" + embarque_id + ", " + pedido_id + ", " + item_id + ", '" + item_desc + "', " + quantidade + ", '" + peso_liq + "', '" + peso_bru + "', '" + serie + "', '" + sequencia + "', 0)", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao inserir o pedido MySQL!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private void UpdatePesoEmbarque()
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                double peso          = CalculoPedidosBruto();
                int embarque_id      = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                MySqlCommand comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET peso='" + peso + "' WHERE embarque_id='" + embarque_id + "'", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao atualizar peso do embarque!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        public static void InsertItensSelecionados(int pedido_id, long item_id, int quantidade, string serie, int sequencia, int selecionado)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();

                int embarque_id   = Convert.ToInt32(bunifuMaterialTextbox1.Text);
                double peso_liq   = CalculoPedidosLiquido(item_id, quantidade);
                double peso_bru   = CalculoPedidosBruto(item_id, quantidade);
                string item_desc  = CoreViaSoft.ObterDadosItem(item_id, false);
                int multiplicador = CoreViaSoft.ObterMultiplicadorItem(item_id);

                MySqlCommand comando = new MySqlCommand("INSERT INTO CARGADEZ_EMBARQUE_ITENS (embarque_id, pedido_id, item, descricao, quantidade, peso_liq, peso_bru, serie, sequencia, selecionado) VALUES (" + embarque_id + ", " + pedido_id + ", " + item_id + ", '" + item_desc + "', " + quantidade + ", '" + peso_liq + "', '" + peso_bru + "', '" + serie + "', '" + sequencia + "', '" + selecionado + "')", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao inserir itens selecionados!\n" + ex.ToString(), "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
        }

        private void BunifuFlatButton2_Click(object sender, EventArgs e)
        {
            UpdatePesoEmbarque();
            MessageBox.Show("Embarque salvo com sucesso!", "Salvando...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Você deseja realmente cancelar?", "Cancelando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            if (dr == DialogResult.Yes)
            {
                dt_LoadPedidos.Clear();
                dt_LoadPedidosSelecionados.Clear();
                dt_LoadItensSelecionados.Clear();

                bunifuMaterialTextbox1.Enabled = true;
                bunifuImageButton4.Enabled = true;
                bunifuImageButton9.Enabled = true;

                bunifuCustomDataGrid1.Enabled = false;
                bunifuCustomDataGrid2.Enabled = false;
                bunifuCustomDataGrid3.Enabled = false;

                bunifuFlatButton1.Enabled = false;
                bunifuFlatButton4.Enabled = false;

                bunifuImageButton8.Enabled = false;
                bunifuImageButton5.Enabled = false;
                bunifuImageButton7.Enabled = false;
                bunifuImageButton6.Enabled = false;

                bunifuFlatButton1.Enabled = false;
                bunifuFlatButton1.Hide();
                bunifuFlatButton2.Enabled = false;
                bunifuFlatButton2.Hide();
                bunifuFlatButton3.Enabled = false;
                bunifuFlatButton3.Hide();
                bunifuFlatButton4.Enabled = false;
                bunifuFlatButton4.Hide();
                bunifuFlatButton5.Enabled = false;
                bunifuFlatButton5.Hide();
                bunifuFlatButton6.Enabled = false;
                bunifuFlatButton6.Hide();

                bunifuMaterialTextbox1.Text = "";

                this.ActiveControl = bunifuMaterialTextbox1;
                bunifuMaterialTextbox1.Focus();
            }
        }
        
        private void CarregaEmbarque()
        {
            if (bunifuImageButton4.Enabled == true)
            {
                if (string.IsNullOrEmpty(bunifuMaterialTextbox1.Text) || bunifuMaterialTextbox1.Text.Length > 0 && bunifuMaterialTextbox1.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Por favor, informe o número do Embarque!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    this.ActiveControl = bunifuMaterialTextbox1;
                    bunifuMaterialTextbox1.Focus();
                    return;
                }

                DialogLoader pl = new DialogLoader();
                if (pl.ShowDialog() == DialogResult.OK)
                {
                    int embarque_id = Convert.ToInt32(bunifuMaterialTextbox1.Text.ToString());
                    embarque_Id = embarque_id;
                    if (VerificaEmbarque(embarque_id))
                    {

                        CarregaPedidos(embarque_id);
                        CarregaPedidosEmbarque(embarque_id);

                        bunifuMaterialTextbox1.Enabled = false;
                        bunifuImageButton4.Enabled = false;
                        bunifuImageButton9.Enabled = false;

                        bunifuCustomDataGrid1.Enabled = true;
                        bunifuCustomDataGrid2.Enabled = true;
                        bunifuCustomDataGrid3.Enabled = true;

                        bunifuFlatButton1.Enabled = true;
                        bunifuFlatButton4.Enabled = true;

                        bunifuImageButton8.Enabled = true;
                        bunifuImageButton5.Enabled = true;
                        bunifuImageButton7.Enabled = true;
                        bunifuImageButton6.Enabled = true;

                        bunifuFlatButton1.Enabled = true;
                        bunifuFlatButton1.Show();
                        bunifuFlatButton2.Enabled = true;
                        bunifuFlatButton2.Show();
                        bunifuFlatButton3.Enabled = true;
                        bunifuFlatButton3.Show();
                        bunifuFlatButton4.Enabled = true;
                        bunifuFlatButton4.Show();
                        bunifuFlatButton5.Enabled = true;
                        bunifuFlatButton5.Show();
                        bunifuFlatButton6.Enabled = true;
                        bunifuFlatButton6.Show();
                    }
                }
            }
        }
        
        private void BunifuImageButton4_Click(object sender, EventArgs e)
        {
            CarregaEmbarque();
        }

        private void BunifuImageButton9_Click(object sender, EventArgs e)
        {
            PesquisaEmbarque pe = new PesquisaEmbarque();
            if (pe.ShowDialog() == DialogResult.OK)
            {
                bunifuMaterialTextbox1.Text = embarque_final.ToString();
            }
        }

        private void BunifuImageButton2_Click_1(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Você deseja realmente sair?", "Saindo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            if (dr == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private static void BunifuMaterialTextbox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
              //  CarregaEmbarque();
            }
        }

        private void BunifuFlatButton5_Click(object sender, EventArgs e)
        {
            ConsultaPedido cp = new ConsultaPedido();
            if (cp.ShowDialog() == DialogResult.OK)
            {
                CarregaPedidosSelecionados(pedido_final, serie_final);
            }
        }

        private void BunifuFlatButton6_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid2.CurrentRow != null)
            {
                int colunaSelecionada = bunifuCustomDataGrid2.CurrentRow.Index;

                if (colunaSelecionada >= 0)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[0].Value.ToString());
                    long item_id = Convert.ToInt64(bunifuCustomDataGrid2.Rows[colunaSelecionada].Cells[1].Value.ToString());

                    VerificaAlocacao va = new VerificaAlocacao(item_id);
                    va.Show();
                }
            }
        }

        private void BunifuImageButton10_Click(object sender, EventArgs e)
        {
            int count = bunifuCustomDataGrid2.RowCount;
            if (count > 0)
            {
                AlocaTotalPedido atp = new AlocaTotalPedido();
                if (atp.ShowDialog() == DialogResult.OK)
                {
                    atp.Close();
                }
            }
            //if (bunifuCustomDataGrid2.CurrentRow != null)
            //{
            //    int count = bunifuCustomDataGrid2.RowCount;
            //    if (count > 0)
            //    {
            //        for (int i = count; i > 0; i--)
            //        {

            //            int pedido_id   = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i - 1].Cells[0].Value.ToString());
            //            long item_id    = Convert.ToInt64(bunifuCustomDataGrid2.Rows[i - 1].Cells[1].Value.ToString());
            //            int qnt_aloc    = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i - 1].Cells[2].Value.ToString());
            //            string serie    = bunifuCustomDataGrid2.Rows[i - 1].Cells[7].Value.ToString();
            //            int sequencia   = Convert.ToInt32(bunifuCustomDataGrid2.Rows[i - 1].Cells[8].Value.ToString());

            //            if (pedido_id > 0)
            //            {
            //                if (VerificaPedido(pedido_id, false, false))
            //                {

            //                    int multiplo        = Funcoes.ObterMultiplicadorItem(item_id);
            //                    bool temMult        = multiplo > 0;
            //                    int saldoDisponivel = temMult ? (SaldoItemDisponivel(item_id) / Funcoes.ObterMultiplicadorItem(item_id)) : SaldoItemDisponivel(item_id);

            //                    if (saldoDisponivel > qnt_aloc)
            //                    {
            //                        AlteraDisponivelExcluidos(pedido_id, item_id, serie, false);
            //                        RemovePedidoSelecionado(pedido_id, item_id, serie);
            //                        InsertItensSelecionados(pedido_id, item_id, qnt_aloc, serie, sequencia);
            //                        AtualizaVisualSaldo(item_id);

            //                        dt_LoadItensSelecionados.Rows.Add(pedido_id, item_id, qnt_aloc, serie, sequencia);
            //                        dt_LoadPedidosSelecionados.Rows.RemoveAt(i - 1);

            //                        bool found = false;
            //                        int countGridPedidos = bunifuCustomDataGrid1.Rows.Count;
            //                        for (int x = 0; x < countGridPedidos; x++)
            //                        {
            //                            if (!found)
            //                            {
            //                                int pedido = Convert.ToInt32(bunifuCustomDataGrid1.Rows[x].Cells[0].Value.ToString());
            //                                if (pedido_id == pedido)
            //                                {
            //                                    bunifuCustomDataGrid1.Rows[x].DefaultCellStyle.ForeColor = Color.Blue;
            //                                    found = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    MessageBox.Show("Este item já foi selecionado!", "Montar Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void BunifuImageButton11_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid3.CurrentRow != null)
            {
                int contarItensSelecionados = bunifuCustomDataGrid3.RowCount;

                if (contarItensSelecionados > 0)
                {

                    for (int i = contarItensSelecionados; i > 0; i--)
                    {

                        int pedido_id = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i -1].Cells[0].Value.ToString());
                        long item_id = Convert.ToInt64(bunifuCustomDataGrid3.Rows[i - 1].Cells[1].Value.ToString());
                        int quantidade = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i - 1].Cells[2].Value.ToString());
                        int sequencia = Convert.ToInt32(bunifuCustomDataGrid3.Rows[i - 1].Cells[4].Value.ToString());
                        string serie = bunifuCustomDataGrid3.Rows[i - 1].Cells[3].Value.ToString();

                        if (pedido_id > 0)
                        {

                            AlteraDisponivelExcluidos(pedido_id, item_id, serie, true);
                            RemoveItensSelecionado(pedido_id, item_id, serie);
                            InsertPedidosSelecionados(pedido_id, item_id, quantidade, serie, sequencia);
                            CarregaPedidosSelecionado(pedido_id, item_id, serie);
                            AtualizaVisualSaldo(item_id);


                            int countGridPedidos = bunifuCustomDataGrid1.Rows.Count;
                            if (ContarItensSelecionados(pedido_id) <= 1)
                            {
                                for (int x = 0; x < countGridPedidos; x++)
                                {
                                    int pedidoCarregadoVS = Convert.ToInt32(bunifuCustomDataGrid1.Rows[x].Cells[0].Value.ToString());
                                    if (pedido_id == pedidoCarregadoVS)
                                    {
                                        bunifuCustomDataGrid1.Rows[x].DefaultCellStyle.ForeColor = Color.Red;
                                    }
                                }
                            }
                            dt_LoadItensSelecionados.Rows.RemoveAt(i - 1);
                        }
                    }
                }
            }
        }
    }
}
