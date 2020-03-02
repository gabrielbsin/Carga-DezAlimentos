using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Expedicao
{
    public partial class MudaStatus : Form
    {
        private int embarque_id;
        private int options;
        public MudaStatus(int embarque_id, int options)
        {
            InitializeComponent();
            this.embarque_id = embarque_id;
            this.options = options;

            setupToOptions(options);
        }

        private int SELECAO = 1;
        private int SEPARACAO = 2;
        private int CARREGANDO = 3;
        private int CARREGADO = 4;

        private void setupToOptions(int options)
        {
            switch(options)
            {
                case 0: // 0 = Seleção de Pedidos
                    comboBox1.Items.Add("Separação");
                    comboBox1.Items.Add("Carregando");
                    comboBox1.Items.Add("Carregado");
                    break;
                case 1: // 1 = Separação
                    comboBox1.Items.Add("Seleção de Pedidos");
                    comboBox1.Items.Add("Carregando");
                    comboBox1.Items.Add("Carregado");
                    break;
                case 2: // 2 = Carregando
                    comboBox1.Items.Add("Seleção de Pedidos");
                    comboBox1.Items.Add("Separação");
                    comboBox1.Items.Add("Carregado");
                    break;
                case 3: // 3 = Carregado
                    comboBox1.Items.Add("Seleção de Pedidos");
                    comboBox1.Items.Add("Separação");
                    comboBox1.Items.Add("Carregando");
                    break;
            }
        }

        private int GetStatusPedido(int pedido_id)
        {
            OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");
            int pedidosAlterados = 0;
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT STATUS FROM PEDCAB WHERE NUMERO='" + pedido_id + "' AND SERIE='PV'", conn);
                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                        string status = odr["STATUS"].ToString();
                        if (status == "B" || status == "C")
                        {
                            pedidosAlterados++;
                        }
                    }
                }

            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                conn.Close();
            }
            return pedidosAlterados;
        }

        private void MudaStatusPedido(int embarque_id, int option)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");
            List<int> pedidosAlterar = new List<int>();
            try
            {

                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarque_id + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        pedidosAlterar.Add(Convert.ToInt32(drCommand["pedido_id"]));
                    }
                }
                drCommand.Dispose();

                int count = pedidosAlterar.Count;

                if (count > 0)
                {
                    int bloqueia = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (GetStatusPedido(pedidosAlterar[i]) > 0) bloqueia++;
                    }

                    if (bloqueia > 0)
                    {
                        MessageBox.Show("Este pedido não pode ser alterado porque já esta baixado/cancelado!", "Alterar Embarque", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }

                    conn.Open();
                    OracleCommand cmd = null;

                    for (int i = 0; i < count; i++)
                    {
                        switch (option)
                        {
                            case 1:
                                cmd = new OracleCommand("UPDATE PEDCAB SET STATUSCARGA=NULL WHERE NUMERO='" + pedidosAlterar[i] + "' AND SERIE='PV'", conn);
                                break;
                            case 2:
                            case 3:
                                cmd = new OracleCommand("UPDATE PEDCAB SET STATUSCARGA='A' WHERE NUMERO='" + pedidosAlterar[i] + "' AND SERIE='PV'", conn);
                                break;
                            case 4:
                                cmd = new OracleCommand("UPDATE PEDCAB SET STATUSCARGA='F' WHERE NUMERO='" + pedidosAlterar[i] + "' AND SERIE='PV'", conn);
                                break;
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao alterar status pedido ViaSoft!\n" + ex.ToString(), "Alterar Embarque", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conn.Close();
                conexao.Close();
                pedidosAlterar.Clear();
            }
        }


        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            Autorizacao auth = new Autorizacao();
            if (auth.ShowDialog() == DialogResult.OK)
            {
                int selecionado = comboBox1.SelectedIndex;
                if (selecionado >= 0)
                {
                    MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
                    try
                    {
                        conexao.Open();
                        MySqlCommand comando = null;
                        MySqlDataReader drCommand;
                        // 1 = Seleção de Pedidos
                        // 2 = Separação
                        // 3 = Carregando
                        // 4 = Carregado
                        switch (this.options)
                        {
                            case 0: // Tab - Seleção de Pedidos
                                switch (selecionado)
                                {
                                    case 0:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + SEPARACAO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, SEPARACAO);
                                        MudaStatusPedido(embarque_id, SEPARACAO);
                                        break;
                                    case 1:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + CARREGANDO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, CARREGANDO);
                                        MudaStatusPedido(embarque_id, CARREGANDO);
                                        break;
                                    case 2:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + CARREGADO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, CARREGADO);
                                        MudaStatusPedido(embarque_id, CARREGADO);
                                        break;

                                }
                                break;
                            case 1: // Tab - Separação
                                switch (selecionado)
                                {
                                    case 0:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + SELECAO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, SELECAO);
                                        MudaStatusPedido(embarque_id, SELECAO);
                                        break;
                                    case 1:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + CARREGANDO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, CARREGANDO);
                                        MudaStatusPedido(embarque_id, CARREGANDO);
                                        break;
                                    case 2:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + CARREGADO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, CARREGADO);
                                        MudaStatusPedido(embarque_id, CARREGADO);
                                        break;

                                }
                                break;
                            case 2: // Tab - Carregando
                                switch (selecionado)
                                {
                                    case 0:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + SELECAO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, SELECAO);
                                        MudaStatusPedido(embarque_id, SELECAO);
                                        break;
                                    case 1:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + SEPARACAO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, SEPARACAO);
                                        MudaStatusPedido(embarque_id, SEPARACAO);
                                        break;
                                    case 2:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + CARREGADO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, CARREGADO);
                                        MudaStatusPedido(embarque_id, CARREGADO);
                                        break;

                                }
                                break;
                            case 3: // Tab - Carregado
                                switch (selecionado)
                                {
                                    case 0:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + SELECAO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, SELECAO);
                                        MudaStatusPedido(embarque_id, SELECAO);
                                        break;
                                    case 1:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + SEPARACAO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, SEPARACAO);
                                        MudaStatusPedido(embarque_id, SEPARACAO);
                                        break;
                                    case 2:
                                        comando = new MySqlCommand("UPDATE CARGADEZ_EMBARQUE SET STATUS='" + CARREGANDO + "', DATA_ALTER=NOW() WHERE embarque_id='" + embarque_id + "'", conexao);
                                        AddEmbarqueLista(embarque_id, CARREGANDO);
                                        MudaStatusPedido(embarque_id, CARREGANDO);
                                        break;

                                }
                                break;

                        }
                        drCommand = comando.ExecuteReader();
                        DialogResult = DialogResult.OK;
                    }

                    catch (Exception ex)
                    {
                       MessageBox.Show("Houve um erro ao alterar status do embarque!\n" + ex.ToString(), "Alterar Status", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    finally
                    {
                        conexao.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Você precisa selecionar algum status!", "Alterar Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } else
            {
                MessageBox.Show("A senha informada esta incorreta!", "Alterar Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddEmbarqueLista(int embarque_id, int list)
        {
            MySqlConnection conexao = new MySqlConnection("SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "");
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE WHERE embarque_id='" + embarque_id + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
                {
                    while(drCommand.Read())
                    {
                        int status = Convert.ToInt32(drCommand["status"]);
                        string motorista = drCommand["motorista"].ToString();
                        string placa = drCommand["placa"].ToString();
                        string peso = string.Format("{0:N}", Convert.ToDouble(drCommand["peso"]));
                        string cidade_destino = drCommand["cidade_destino"].ToString();
                        if (cidade_destino != null)
                        {
                            cidade_destino = drCommand["cidade_destino"].ToString() + " - " + drCommand["uf_destino"].ToString();
                        }
                        else
                        {
                            cidade_destino = "-";
                        }

                        switch (list)
                        {
                            case 1:
                                ListarEmbarques.dt_SelecaoPedidos.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                            case 2:
                                ListarEmbarques.dt_Separacao.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                            case 3:
                                ListarEmbarques.dt_Carregamento.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                            case 4:
                                ListarEmbarques.dt_Carregado.Rows.Add(embarque_id, motorista, placa, peso, cidade_destino);
                                break;
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao alterar status do embarque!\n" + ex.ToString(), "Alterar Status", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                
            } finally
            {
                conexao.Close();
            }
        }
    }
}
