using GerarCargaDez.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class VerificaAlocacao : Form
    {
        public long item_id = 0;
        public VerificaAlocacao(long item)
        {
            InitializeComponent();
            this.item_id = item;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CarregaItensAlocados()
        {
            treeView1.Nodes.Clear();

            List<Tuple<int, int, long, string>> item = new List<Tuple<int, int, long, string>>();
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE item='" + this.item_id + "' AND selecionado='1' ORDER BY item", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int numEmbarque = Convert.ToInt32(drCommand["embarque_id"]);
                        int numPedido = Convert.ToInt32(drCommand["pedido_id"]);
                        string serie = drCommand["serie"].ToString();
                        int numClient = CoreViaSoft.ObterPessoaByPedido(numPedido, serie);
                        long numItem = Convert.ToInt32(drCommand["item"]);
                        item.Add(new Tuple<int, int, long, string>(numEmbarque, numClient, numItem, serie));
                    }
                }
                drCommand.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar dados do pedido!\n" + ex.ToString(), "Detalhamento do Pedido", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }

            int count = item.Count;
            for (int i = 0; i < count; i++)
            {
                int embarque_id = item[i].Item1;

                if (VerificaEmbarque(embarque_id) == false)
                {
                    long numItem   = item[i].Item3;
                    string serie   = item[i].Item4;
                    try
                    {
                        conexao.Open();
                        MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE item='" + numItem + "' AND serie='" + serie + "' AND embarque_id='" + embarque_id + "' AND selecionado ='1'", conexao);
                        MySqlDataReader drCommand = comando.ExecuteReader();
                        if (drCommand.HasRows)
                        {
                            while (drCommand.Read())
                            {
                                TreeNode node = new TreeNode("Embarque nº " + drCommand["embarque_id"].ToString());
                                node.Nodes.Add("Pedido: " + drCommand["pedido_id"].ToString());
                                node.Nodes.Add("Código do Item: " + drCommand["item"].ToString());
                                node.Nodes.Add("Descrição do Item: " + drCommand["descricao"].ToString());
                                node.Nodes.Add("Quantidade Pedido: " + drCommand["quantidade"].ToString());
                                node.Nodes.Add("Peso Liquido: " + string.Format("{0:N}", Convert.ToInt32(drCommand["peso_liq"].ToString())));
                                node.Nodes.Add("Peso Bruto: " + string.Format("{0:N}", Convert.ToInt32(drCommand["peso_bru"].ToString())));
                                treeView1.Nodes.Add(node);
                            }
                        }
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
            }
        }

        private bool VerificaEmbarque(int embarque_id)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            bool carregado = false;
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE WHERE embarque_id='" + embarque_id + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int status = Convert.ToInt32(drCommand["status"]);
                        switch (status)
                        {
                            case 1:
                                carregado = false;
                                break;
                            case 2:
                            case 3:
                            case 4:
                                carregado = true;
                                break;
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                conexao.Close();
            }
            return carregado;
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            CarregaItensAlocados();
        }
    }
}
