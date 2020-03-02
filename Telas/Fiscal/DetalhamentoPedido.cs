using GerarCargaDez.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Fiscal
{
    public partial class DetalhamentoPedido : Form
    {
        private int pedido_id = 0;
        public DetalhamentoPedido(int pedido)
        {
            InitializeComponent();
            this.pedido_id = pedido;
        }

        private void CarregaDadosPedido()
        {
            treeView1.Nodes.Clear();

            List<Tuple<int, int, int, string>> pessoas = new List<Tuple<int, int, int, string>>();
            List<Tuple<int, int, int, string>> pessoas_or = new List<Tuple<int, int, int, string>>();
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + this.pedido_id + "' AND selecionado='1' ORDER BY pedido_id", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        int numPedido = Convert.ToInt32(drCommand["pedido_id"]);
                        string serie  = drCommand["serie"].ToString();
                        int numClient = CoreViaSoft.ObterPessoaByPedido(numPedido, serie);
                        int numItem   = Convert.ToInt32(drCommand["item"]);
                        pessoas.Add(new Tuple<int, int, int, string>(numClient, numPedido, numItem, serie));
                        pessoas_or.Add(new Tuple<int, int, int, string>(numClient, numPedido, numItem, serie));
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

            pessoas.Sort();
            Int32 index = pessoas.Count - 1;
            while (index > 0)
            {
                if (pessoas[index].Item1 == pessoas[index - 1].Item1)
                {
                    if (index < pessoas.Count - 1)
                        (pessoas[index], pessoas[pessoas.Count - 1]) = (pessoas[pessoas.Count - 1], pessoas[index]);
                    pessoas.RemoveAt(pessoas.Count - 1);
                    index--;
                }
                else
                    index--;
            }

            int countUniquePessoas = pessoas.Count;
            int countGeralPessoas  = pessoas_or.Count;

            for (int i=0; i < countUniquePessoas; i++)
            {
                int pessoa_id = pessoas[i].Item1;
                TreeNode node_p = new TreeNode("Cliente nº " + pessoa_id);
                for (int x = 0; x < countGeralPessoas; x++)
                {
                    int pessoa_id_ = pessoas_or[x].Item1;
                    int pedido_    = pessoas_or[x].Item2;
                    int item_      = pessoas_or[x].Item3;
                    string serie   = pessoas_or[x].Item4;
                    if (pessoa_id == pessoa_id_)
                    {
                        TreeNode node = new TreeNode("Pedido nº " + pedido_);
                        try
                        {
                            conexao.Open();
                            MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + this.pedido_id + "' AND pedido_id='" + pedido_ + "' AND item='" + item_ + "' AND serie='" + serie + "' AND selecionado ='1'", conexao);
                            MySqlDataReader drCommand = comando.ExecuteReader();
                            if (drCommand.HasRows)
                            {
                                while (drCommand.Read())
                                {
                                    node.Nodes.Add("Código do Item: " + drCommand["item"].ToString());
                                    node.Nodes.Add("Descrição do Item: " + drCommand["descricao"].ToString());
                                    node.Nodes.Add("Quantidade Pedido: " + drCommand["quantidade"].ToString());
                                    node.Nodes.Add("Peso Liquido: " + string.Format("{0:N}", Convert.ToInt32(drCommand["peso_liq"].ToString())));
                                    node.Nodes.Add("Peso Bruto: " + string.Format("{0:N}", Convert.ToInt32(drCommand["peso_bru"].ToString())));
                                    node_p.Nodes.Add(node);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        } finally
                        {
                            conexao.Close();
                        } 
                    }
                }
                treeView1.Nodes.Add(node_p);
            }
        }

        private void DetalhamentoPedido_Load(object sender, EventArgs e)
        {
            CarregaDadosPedido();
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
