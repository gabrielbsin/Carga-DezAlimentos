using GerarCargaDez.Core;
using GerarCargaDez.Telas.Etiqueta.Consulta;
using GerarCargaDez.Telas.Etiqueta.Consulta.Item;
using GerarCargaDez.Telas.Etiqueta.Models;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Etiqueta
{
    public partial class NovaEtiqueta : Form
    {

        MySqlConnection conexao = null;
        MySqlCommand comando;
        string conecta = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=" + Properties.Settings.Default.db_mysql_etiqueta + "; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        public static int varOrdemId { get; set; }
        public static string varDescricao { get; set; }
        public static int varSequencia { get; set; }
        public static int varItemId { get; set; }
        public static string varItemDesc { get; set; }
        public static string varDataProd { get; set; }
        public static string varDataVal { get; set; }
        public static string varGrupoQualidade { get; set; }
        public static string varLoteSerie { get; set; }
        public static string varPesoLiq { get; set; }
        public static string varPesoBru { get; set; }
        public static int Ult_Seq { get; set; }
        public static string varHoraImpressao { get; set; }
        public static bool Considera { get; set; }
        public static string varTara { get; set; }
        public static string qualidadeConsistencia { get; set; }
        public static string qualidadeCor { get; set; }
        public static string qualidadeStatus { get; set; }
        public static int varQuantidadeImpressao { get; set; }
        public static string varObservacao { get; set; }


        public NovaEtiqueta()
        {
            InitializeComponent();

            this.ActiveControl = campoOrdemProd;               // Força o preenchimento da Ordem de Produção
            this.campoOrdemProd.Focus();                       // Força o preenchimento da Ordem de Produção
            this.campoGrupoQualidade.Enabled = false;          // Desabilita inicialmente Grupo Qualidade
            this.campoLoteSerie.Enabled = false;               // Desabilita inicialmente Lote/Serie
            this.campoDataProd.Enabled = false;                // Desabilita inicialmente Data Produção
            this.campoDataVal.Enabled = false;                 // Desabilita inicialmente Data Validade
            this.campoPesoLiq.Enabled = false;                 // Desabilita inicialmente Peso Líquido
            this.campoPesoBruto.Enabled = false;               // Desabilita inicialmente Peso Bruto
            this.campoSequencia.Enabled = false;               // Desabilita inicialmente textbox aonde é exibido sequência
            this.checkBoxImpressao.Checked = true;             // Habilita inicialmente checkbox de impressão 
            this.checkBoxReimpressao.Enabled = false;          // Desabilita inicialmente checkbox de reimpressão 
            this.botaoPesquisaSeq.Enabled = false;             // Desabilita inicialmente consultar sequência 
            this.comboBoxManutencao.Enabled = false;           // Desabilita inicialmente considera/desconsidera


            // Verifica a finalidade do usuário 
            // caso seja verdadeiro = exibe campo "Tara"
            // se não, oculta campo.
            if (!Properties.Settings.Default.finalidadeUsuario)
            {
                this.campoTara.Hide();
                this.labelTara.Hide();
            } else
            {
                this.campoTara.Enabled = false;                // Desabilita inicialmente Tara
            }

            this.comboBox2.Hide();                            // Desabilita edição de Grupo Qualidade
            this.comboBox3.Hide();                            // Desabilita edição de Grupo Qualidade
            this.comboBox4.Hide();                            // Desabilita edição de Grupo Qualidade

            CarregaFinalidade();
        }

        // Responsavel por carregar a finalidade  se é apontamento ou asseptico.
        public void CarregaFinalidade()
        {
            if (Properties.Settings.Default.finalidadeUsuario)
            {
                // Verifica se o checkBoxImpressao marcado é de Impressão
                if (checkBoxImpressao.Checked == true)                                           
                {
                    // Desabilita edição Grupo de Qualidade
                    campoGrupoQualidade.Enabled = false;                                            

                    try
                    {
                        conexao = new MySqlConnection(conecta);
                        string sql = "SELECT * FROM config_ql WHERE asseptico=" + Properties.Settings.Default.asseptico;
                        comando = new MySqlCommand(sql, conexao);
                        conexao.Open();
                        MySqlDataReader drCommand = comando.ExecuteReader();

                        if (drCommand.Read())
                        {
                            qualidadeConsistencia         = drCommand["consistencia"].ToString();
                            qualidadeCor                  = drCommand["cor"].ToString();
                            qualidadeStatus               = drCommand["status_ql"].ToString();
                            // Coloca Grupo de Qualidade conforme ex: 2/2.01
                            campoGrupoQualidade.Text      = qualidadeConsistencia + "/" + qualidadeCor + "." + qualidadeStatus; 

                            // Desabilita campos para alteração
                            campoOrdemProd.Enabled        = false;
                            botaoPesquisaOrdem.Enabled    = false;

                            // Carrega informações da Ordem de Produção
                            campoOrdemProd.Text           = drCommand["ordem_producao"].ToString();
                            varOrdemId                    = Convert.ToInt32(drCommand["ordem_producao"].ToString());
                            campoItem.Text                = CoreViaSoft.ObterItemOrdemProd(varOrdemId).ToString();
                            varItemId                     = CoreViaSoft.ObterItemOrdemProd(varOrdemId);
                            labelDescItem.Text            = drCommand["item_desc"].ToString();
                            varItemDesc                   = drCommand["item_desc"].ToString();
                            varObservacao                 = drCommand["observacao"].ToString();

                            // Habilita campos para alteração
                            campoDataProd.Enabled         = true;
                            campoDataVal.Enabled          = true;
                            campoLoteSerie.Enabled        = true;
                            campoTara.Enabled             = true;
                            campoPesoLiq.Enabled          = true;
                            campoPesoBruto.Enabled        = true;

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Houve um erro com a conexão com banco de dados!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    finally
                    {
                        conexao.Close();
                    }
                }
            }
        }

        // Faz a consulta de Ordem de Produção  no banco da ViaSoft e retorna
        // dados (IDORDEMPROD, ITEM) que são usados na tabela (etiquetas_prontas)
        private void Button4_Click(object sender, EventArgs e)
        {
            ConsultaOrdem ci = new ConsultaOrdem();
            if (ci.ShowDialog() == DialogResult.OK)
            {

                campoOrdemProd.Text        = varOrdemId.ToString();                     // Id da Op
                varSequencia               = getLastID();                               // Recupera ultima sequência
                campoSequencia.Text        = varSequencia.ToString();                   // Altera sequencia no textbox
                campoItem.Text             = varItemId.ToString();                      // Id do Item
                labelDescItem.Text         = varItemDesc;                               // Descrição do Item
                campoLoteSerie.Text        = Properties.Settings.Default.lote_fixo;     // Busca sempre lote cadastrado no autopreenchimento


                // Habilitar edição de textbox (Grupo de Qualidade)
                if (!Properties.Settings.Default.finalidadeUsuario)
                {
                    campoGrupoQualidade.Enabled = true;
                    campoGrupoQualidade.Text    = Properties.Settings.Default.grupoQualidade;
                }

                campoDataProd.Enabled   = true;                         // Data produção
                campoDataVal.Enabled    = true;                         // Data validade
                campoLoteSerie.Enabled  = true;                         // Lote/Serie
                campoPesoLiq.Enabled    = true;                         // Peso Líquido
                campoPesoBruto.Enabled  = true;                         // Peso Bruto
                campoTara.Enabled       = true;                         // Tara

            }
        }

        // Responsavel por consultar o ultimo ID de Sequência gerado no banco
        public int getLastID()
        {
            int lastId = 1;
            conexao = new MySqlConnection(conecta);
            conexao.Open();
            try
            {
                comando = new MySqlCommand("SELECT MAX(seq) FROM etiquetas_prontas", conexao);
                lastId = Convert.ToInt32(comando.ExecuteScalar().ToString()) + 1;
            }
            catch
            { }
            finally
            {
                conexao.Close();
            }
            return lastId;
        }

        public void GeraImpressaoEtiqueta(bool Messages)
        {
            if (checkBoxImpressao.Checked == true)
            {
                if (campoOrdemProd.Text.Equals(string.Empty))
                {
                    MessageBox.Show("É necessario informar uma Ordem de Produção!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            else
            {
                if (campoSequencia.Text == String.Empty && campoSequencia.Text.Length < 0)
                {
                    MessageBox.Show("É necessario informar uma Sequência!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            // Realiza verificações de todos campos necessários para impressão.
            if (string.IsNullOrEmpty(campoGrupoQualidade.Text) || campoGrupoQualidade.Text.Length > 0 && campoGrupoQualidade.Text.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, insira um Grupo de Qualidade!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = campoGrupoQualidade;
                campoGrupoQualidade.Focus();
                return;
            }

            if (string.IsNullOrEmpty(campoLoteSerie.Text) || campoLoteSerie.Text.Length > 0 && campoLoteSerie.Text.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, insira um Lote/Serie!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = campoLoteSerie;
                campoLoteSerie.Focus();
                return;
            }

            if (Properties.Settings.Default.finalidadeUsuario && (string.IsNullOrEmpty(campoTara.Text) || campoTara.Text.Length > 0 && campoTara.Text.Trim().Length == 0))
            {
                MessageBox.Show("Por favor, insira uma Tara!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = campoTara;
                campoTara.Focus();
                return;
            }

            if (string.IsNullOrEmpty(campoPesoLiq.Text) || campoPesoLiq.Text.Length > 0 && campoPesoLiq.Text.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, insira um Peso Líquido!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = campoPesoLiq;
                campoPesoLiq.Focus();
                return;
            }

            if (string.IsNullOrEmpty(campoPesoBruto.Text) || campoPesoBruto.Text.Length > 0 && campoPesoBruto.Text.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, insira um Peso Bruto!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = campoPesoBruto;
                campoPesoBruto.Focus();
                return;
            }

            if (CoreViaSoft.ObterStatusOrdem(varOrdemId))
            {
                MessageBox.Show("A Ordem de Produção esta fechada!", "Ordem de Produção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.ActiveControl = campoOrdemProd;
                campoOrdemProd.Focus();
                return;
            }

            if (Properties.Settings.Default.finalidadeUsuario)
            {
                double a, b;
                a = double.Parse(campoPesoLiq.Text);
                b = double.Parse(campoPesoBruto.Text);

                if (a <= 10 || a > 99999)
                {
                    MessageBox.Show("O peso inserido não é válido!", "Valores incorretos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    this.ActiveControl = campoPesoLiq;
                    campoPesoLiq.Focus();
                    return;
                }

                if (b <= 10 || b > 99999)
                {
                    MessageBox.Show("O peso inserido não é válido!", "Valores incorretos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    this.ActiveControl = campoPesoBruto;
                    campoPesoBruto.Focus();
                    return;
                }
            }

            // Impressão de uma nova Etiqueta
            if (checkBoxImpressao.Checked == true)
            {
                conexao = new MySqlConnection(conecta);
                string sql;
                if (!Properties.Settings.Default.finalidadeUsuario)
                {
                    sql = "INSERT INTO etiquetas_prontas(op, grupo_qualidade, item_id, item_desc, data_prod, data_val, lote_serie, peso_liq, peso_bruto, hora_imp, usuario, considera, tara, asseptico, data_imp) VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12,@param13,@param14,@param18)";
                }
                else
                {
                    sql = "INSERT INTO etiquetas_prontas(op, grupo_qualidade, item_id, item_desc, data_prod, data_val, lote_serie, peso_liq, peso_bruto, hora_imp, usuario, considera, tara, asseptico, ql_consistencia, ql_cor, ql_status, data_imp) VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12,@param13,@param14,@param15,@param16,@param17, @param18)";
                }

                comando = new MySqlCommand(sql, conexao);
                comando.Parameters.Add("@param1", MySqlDbType.Int32).Value   = varOrdemId;                                              // Ordem de Produção
                comando.Parameters.Add("@param2", MySqlDbType.String).Value  = campoGrupoQualidade.Text.ToString();                     // Grupo Qualidade
                comando.Parameters.Add("@param3", MySqlDbType.Int32).Value   = varItemId;                                               // Id do item
                comando.Parameters.Add("@param4", MySqlDbType.String).Value  = varItemDesc;                                             // Descrição do Item
                comando.Parameters.Add("@param5", MySqlDbType.String).Value  = campoDataProd.Value.ToShortDateString();                 // Data de produção
                comando.Parameters.Add("@param6", MySqlDbType.String).Value  = campoDataVal.Value.ToShortDateString();                  // Data de validade
                comando.Parameters.Add("@param7", MySqlDbType.String).Value  = campoLoteSerie.Text.ToString();                          // Lote e Serie
                comando.Parameters.Add("@param8", MySqlDbType.Decimal).Value = campoPesoLiq.Text;                                       // Peso liquido
                comando.Parameters.Add("@param9", MySqlDbType.Decimal).Value = campoPesoBruto.Text;                                     // Peso bru
                comando.Parameters.Add("@param10", MySqlDbType.String).Value = DateTime.Now.ToShortTimeString();                        // Hora impressão
                comando.Parameters.Add("@param11", MySqlDbType.String).Value = Environment.UserName;                                    // Usuario que imprimiu
                comando.Parameters.Add("@param12", MySqlDbType.Int32).Value  = 0;                                                       // Nova etiqueta considera (sempre)
                comando.Parameters.Add("@param13", MySqlDbType.String).Value = campoTara.Text.ToString();                               // Tara
                comando.Parameters.Add("@param14", MySqlDbType.Int32).Value  = Properties.Settings.Default.finalidadeUsuario ? 1 : 0;   // Considera Asspetico

                if (Properties.Settings.Default.finalidadeUsuario)
                {
                    comando.Parameters.Add("@param15", MySqlDbType.String).Value = qualidadeConsistencia;                               // Separa consistencia para fins de relatorio
                    comando.Parameters.Add("@param16", MySqlDbType.String).Value = qualidadeCor;                                        // Separa cor para fins de relatorio
                    comando.Parameters.Add("@param17", MySqlDbType.String).Value = qualidadeStatus;                                     // Separa status para fins de relatorio
                }

                DateTime dateValue = DateTime.Now;
                string MySQLFormatDate = dateValue.ToString("dd/MM/yyyy");
                comando.Parameters.Add("@param18", MySqlDbType.String).Value = MySQLFormatDate;                                          // Data Impressão
                conexao.Open();

                MySqlDataReader drCommand = comando.ExecuteReader();
                if (!drCommand.Read())
                {
                    if (Messages)
                    {
                        MessageBox.Show("Uma nova etiqueta foi gerada com a sequência {" + varSequencia + "}!", "Inserindo dados...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    CarregaFinalidade();

                    string modeloImpressao         = ModeloZebra.ModeloEtiquetaNova(varOrdemId, varItemId, varItemDesc, campoLoteSerie.Text.ToString(), campoDataProd.Value.ToShortDateString(), campoDataVal.Value.ToShortDateString(), varSequencia, campoPesoLiq.Text, campoPesoBruto.Text, campoGrupoQualidade.Text.ToString(), null, true, Properties.Settings.Default.usa_cx, varObservacao);
                    PrinterSettings configuracoes  = new PrinterSettings();
                    string nomePadraoImpressora    = configuracoes.PrinterName;
                    RawPrinterHelper.SendStringToPrinter(nomePadraoImpressora, modeloImpressao);
                    conexao.Close();
                    return;
                }
                else
                {
                    MessageBox.Show("Houve um erro ao inserir um nova Etiqueta!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    conexao.Close();
                    return;
                }
            }
            // Reimpressão ou Manutenção Etiqueta
            else
            {
                // Considera item ou altera item para considerado
                if (comboBoxManutencao.SelectedIndex == 0)
                {
                    string sql = "";
                    try
                    {
                        conexao = new MySqlConnection(conecta);
                        sql = "SELECT * FROM etiquetas_prontas WHERE seq=" + varSequencia + "";
                        comando = new MySqlCommand(sql, conexao);
                        conexao.Open();
                        MySqlDataReader drCommand = comando.ExecuteReader();

                        if (drCommand.Read())
                        {
                            if (int.Parse(drCommand["considera"].ToString()) != 0)
                            {
                                conexao = new MySqlConnection(conecta);
                                comando = new MySqlCommand("UPDATE etiquetas_prontas SET considera='0' WHERE seq='" + varSequencia + "'", conexao);
                                conexao.Open();
                                comando.ExecuteNonQuery();
                                comando.Dispose();


                                DialogResult dr = MessageBox.Show("Você alterou o estado da etiqueta para Considerado novamente, deseja imprimir a etiqueta?", "Atualizando...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                CarregaFinalidade();

                                // Faz a reimpressão de uma etiqueta e busca informações inseridas na inclusão
                                if (dr == DialogResult.Yes)
                                {
                                    string modeloImpressao        = ModeloZebra.ModeloEtiquetaNova(varOrdemId, varItemId, varItemDesc, varLoteSerie, varDataProd, varDataVal, varSequencia, varPesoLiq.ToString(), varPesoBru.ToString(), varGrupoQualidade, varHoraImpressao, false, Properties.Settings.Default.usa_cx, varObservacao);
                                    PrinterSettings configuracoes = new PrinterSettings();
                                    string nomePadraoImpressora   = configuracoes.PrinterName;
                                    RawPrinterHelper.SendStringToPrinter(nomePadraoImpressora, modeloImpressao);
                                }
                            }
                            else
                            {
                                // Faz a reimpressão de uma etiqueta e busca informações inseridas na inclusão
                                CarregaFinalidade();

                                string modeloImpressao        = ModeloZebra.ModeloEtiquetaNova(varOrdemId, varItemId, varItemDesc, varLoteSerie, varDataProd, varDataVal, varSequencia, varPesoLiq.ToString(), varPesoBru.ToString(), varGrupoQualidade, varHoraImpressao, false, Properties.Settings.Default.usa_cx, varObservacao);
                                PrinterSettings configuracoes = new PrinterSettings();
                                string nomePadraoImpressora   = configuracoes.PrinterName;
                                RawPrinterHelper.SendStringToPrinter(nomePadraoImpressora, modeloImpressao);
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Houve um erro ao alterar status da Etiqueta!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    finally
                    {
                        conexao.Close();
                    }
                    return;
                }
                // Altera etiqueta para desconsiderada
                else if (comboBoxManutencao.SelectedIndex == 1)
                {
                    int considera = 0;
                    try
                    {
                        conexao = new MySqlConnection(conecta);
                        comando = new MySqlCommand("SELECT considera FROM etiquetas_prontas WHERE seq='" + varSequencia + "'", conexao);
                        conexao.Open();
                        MySqlDataReader drCommand = comando.ExecuteReader();

                        if (!drCommand.Read())
                        {
                            considera = Convert.ToInt32(drCommand["considera"]);
                        }

                    }
                    catch
                    {
                        MessageBox.Show("Houve um erro ao selecionar Etiqueta!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    finally
                    {
                        conexao.Close();
                    }

                    if (considera != 1)
                    {
                        try
                        {
                            conexao = new MySqlConnection(conecta);
                            comando = new MySqlCommand("UPDATE etiquetas_prontas SET considera='1' WHERE seq='" + varSequencia + "'", conexao);
                            conexao.Open();
                            comando.ExecuteNonQuery();
                            comando.Dispose();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Houve um erro ao ao desconsiderar Etiqueta!", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return;
                        }
                        finally
                        {
                            conexao.Close();
                        }
                    }

                    botaoExecutar.Enabled = false;
                    MessageBox.Show("A etiqueta foi desconsidera!", "Atualizando...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Atualiza informações da etiqueta
                else if (comboBoxManutencao.SelectedIndex == 2)
                {
                    try
                    {
                        conexao = new MySqlConnection(conecta);
                        string sql;
                        if (CoreEtiqueta.ObterAsseptico(varSequencia))
                        {
                            sql = "UPDATE etiquetas_prontas SET grupo_qualidade=@param1, data_prod=@param2, data_val=@param3, lote_serie=@param4, peso_liq=@param5, peso_bruto=@param6, tara=@param7, usuario=@param8, ql_consistencia=@param9, ql_cor=@param10, ql_status=@param11 WHERE seq=" + varSequencia + "";
                            comando = new MySqlCommand(sql, conexao);
                            comando.Parameters.AddWithValue("@param1", "" + comboBox2.Text.ToString() + "/" + comboBox3.Text.ToString() + "." + comboBox4.Text.ToString() + "");
                            comando.Parameters.AddWithValue("@param2", campoDataProd.Value.ToShortDateString());
                            comando.Parameters.AddWithValue("@param3", campoDataVal.Value.ToShortDateString());
                            comando.Parameters.AddWithValue("@param4", campoLoteSerie.Text.ToString());
                            comando.Parameters.AddWithValue("@param5", campoPesoLiq.Text.Replace(",", ".").ToString());
                            comando.Parameters.AddWithValue("@param6", campoPesoBruto.Text.Replace(",", ".").ToString());
                            comando.Parameters.AddWithValue("@param7", campoTara.Text.ToString());
                            comando.Parameters.AddWithValue("@param8", Environment.UserName);
                            comando.Parameters.AddWithValue("@param9", comboBox2.Text.ToString());
                            comando.Parameters.AddWithValue("@param10", comboBox3.Text.ToString());
                            comando.Parameters.AddWithValue("@param11", comboBox4.Text.ToString());
                        }
                        else
                        {
                            sql = "UPDATE etiquetas_prontas SET grupo_qualidade=@param1, data_prod=@param2, data_val=@param3, lote_serie=@param4, peso_liq=@param5, peso_bruto=@param6, tara=@param7, usuario=@param8 WHERE seq=" + varSequencia + "";
                            comando = new MySqlCommand(sql, conexao);
                            comando.Parameters.AddWithValue("@param1", campoGrupoQualidade.Text.ToString());
                            comando.Parameters.AddWithValue("@param2", campoDataProd.Value.ToShortDateString());
                            comando.Parameters.AddWithValue("@param3", campoDataVal.Value.ToShortDateString());
                            comando.Parameters.AddWithValue("@param4", campoLoteSerie.Text.ToString());
                            comando.Parameters.AddWithValue("@param5", campoPesoLiq.Text.ToString());
                            comando.Parameters.AddWithValue("@param6", campoPesoBruto.Text.ToString());
                            comando.Parameters.AddWithValue("@param7", campoTara.Text.ToString());
                            comando.Parameters.AddWithValue("@param8", Environment.UserName);
                        }


                        conexao.Open();
                        comando.ExecuteNonQuery();
                        comando.Dispose();

                        if (CoreEtiqueta.ObterAsseptico(varSequencia))
                        {
                            varGrupoQualidade     = comboBox2.Text.ToString() + "/" + comboBox3.Text.ToString() + "." + comboBox4.Text.ToString() + "";
                            qualidadeConsistencia = comboBox2.Text;
                            qualidadeCor          = comboBox3.Text;
                            qualidadeStatus       = comboBox4.Text;
                        }
                        else
                        {
                            varGrupoQualidade = campoGrupoQualidade.Text;
                        }

                        varDataProd   = campoDataProd.Value.ToShortDateString();
                        varDataVal    = campoDataVal.Value.ToShortDateString();
                        varLoteSerie  = campoLoteSerie.Text;
                        varPesoLiq    = campoPesoLiq.Text.ToString();
                        varPesoBru    = campoPesoBruto.Text.ToString();
                        varTara       = campoTara.Text;

                        MessageBox.Show("A etiqueta foi atualizada!", "Atualizando...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Houve um erro com a conexão com banco de dados!\n" + ex, "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    finally
                    {
                        conexao.Close();
                    }
                }
                // Baixar saldo da Etiqueta
                else 
                {
                    // Somente usuarios com finalidade Asspetico podem alterar
                    if (!CoreEtiqueta.ObterAsseptico(varSequencia))
                    {
                        MessageBox.Show("Está função esta habilitada somente para etiquetas do Asséptico!", "Atualizar dados...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    try
                    {
                        conexao = new MySqlConnection(conecta);
                        comando = new MySqlCommand("UPDATE etiquetas_prontas SET considera='2' WHERE seq=" + varSequencia + "", conexao);
                        conexao.Open();
                        comando.ExecuteNonQuery();
                        comando.Dispose();

                        botaoExecutar.Enabled = false;
                        MessageBox.Show("A etiqueta foi baixada no Estoque!", "Atualizando...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Houve um erro baixar saldo Etiqueta!\n" + ex, "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                    finally
                    {
                        conexao.Close();
                    }
                    return;
                }
            }
        }

        public void LimpaTextos()
        {
            foreach (Control c in Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
            }


            campoOrdemProd.Text    = "";
            campoTara.Text         = "";
            campoSequencia.Text    = "";
            labelDescItem.Text     = "";

            varOrdemId             = 0;
            varSequencia           = 0;
            varItemId              = 0;
            varDescricao           = null;
            varItemDesc            = null;
            varDataProd            = null;
            varDataVal             = null;
            varGrupoQualidade      = null;
            varLoteSerie           = null;
            varPesoLiq             = "";
            varPesoBru             = "";
            varHoraImpressao       = null;

            comboBoxManutencao.SelectedIndex = -1;
            CarregaFinalidade();

        }

        private void ImpressaoCheckBox_Click(object sender, EventArgs e)
        {
            if (checkBoxImpressao.Checked)
            {
                checkBoxImpressao.Checked    = true;     // Diz que o checkbox1(Impressão) está marcado
                checkBoxReimpressao.Checked  = false;    // Diz que o checkbox2 está desmarcado
                campoSequencia.Enabled       = false;    // Desabilita Edição Sequencia
                campoOrdemProd.Enabled       = true;     // Caso esteja marcado habilita Edição Op / Pesquisa
                botaoPesquisaOrdem.Enabled   = true;     // Caso esteja marcado habilita Edição Op / Pesquisa
                comboBoxManutencao.Enabled   = false;    // Caso esteja marcado desabilita Manutenção
                campoDataProd.Enabled        = false;    // Caso esteja marcado desabilita Data Produção
                campoDataVal.Enabled         = false;    // Caso esteja marcado desabilita Data Validade
                LimpaTextos();                           // Caso seja marcado ele limpa as informações antigas
            }
            else
            {
                botaoPesquisaOrdem.Enabled   = false;    // Após desmarcar impressão desabilita butao de pesquisa de Op em Impressão
                campoOrdemProd.Enabled       = false;    // Após desmarcar impressão desabilita campo de pesquisa de Op em Impressão
                checkBoxReimpressao.Checked  = true;     // Assina-la que o checkbox2(Reimpressão) está ativado
                checkBoxReimpressao.Enabled  = true;     // Faz um check no checkbox2(Reimpressão) e deixa marcado
                checkBoxImpressao.Enabled    = false;    // Desativa a marcação no checkbox1(Impressão) para habilitar o checkbox1(Reimpressão)
                botaoPesquisaSeq.Enabled     = true;     // Desabilita a opção de edição na caixa de texto "Ordem de Produção"
                campoLoteSerie.Enabled       = false;    // Desabilita a opção de edição na caixa de texto "Lote/Série"
                campoPesoLiq.Enabled         = false;    // Desabilita a opção de edição na caixa de texto "Peso Liq"
                campoTara.Enabled            = false;    // Desabilita a opção de edição na caixa de texto "Tara"
                campoPesoBruto.Enabled       = false;    // Desabilita a opção de edição na caixa de texto "Peso Bruto"
                campoGrupoQualidade.Enabled  = false;    // Desabilita a opção de edição na caixa de texto "Grupo Qualidade"
                campoDataProd.Enabled        = false;    // Desabilita a opção de edição na caixa de texto "Data Produção"
                campoDataVal.Enabled         = false;    // Desabilita a opção de edição na caixa de texto "Data de Validade"
                LimpaTextos();
            }
        }

        private void ReimpressaoCheckBox_Click(object sender, EventArgs e)
        {
            if (checkBoxReimpressao.Checked)
            {
                checkBoxReimpressao.Checked  = true;    // Diz que o checkbox1(Reimpressão) está marcado
                checkBoxImpressao.Checked    = false;   // Diz que o checkbox2(Impressão) está desmarcado
                campoOrdemProd.Enabled       = false;   // Caso esteja marcado desabilita Edição Op / Pesquisa
                botaoPesquisaOrdem.Enabled   = false;   // Caso esteja marcado desabilita Edição Op / Pesquisa
                campoDataProd.Enabled        = false;   // Caso esteja marcado desabilita Data Produção
                campoDataVal.Enabled         = false;   // Caso esteja marcado desabilita Data Validade
                LimpaTextos();                          // Caso seja marcado ele limpa as informações antigas
            }
            else
            {
                botaoPesquisaOrdem.Enabled   = true;    // Após desmarcar reimpressão habilita butao de pesquisa de Op em Impressão
                campoOrdemProd.Enabled       = true;    // Após desmarcar reimpressão habilita campo de pesquisa de Op em Impressão
                checkBoxImpressao.Enabled    = true;    // Assina-la que o checkbox1 está ativado
                checkBoxImpressao.Checked    = true;    // Faz um check no checkbox1 e deixa marcado
                checkBoxReimpressao.Enabled  = false;   // Desativa a marcação no checkbox2(Reimpressão) para habilitar o checkbox1(Impressão)
                campoSequencia.Enabled       = false;   // Desabilita a opção de edição na caixa de texto "Sequência"
                botaoPesquisaSeq.Enabled     = false;   // Desabilita a opção de busca de Sequência/Op 
                comboBoxManutencao.Enabled   = false;   // Desabilita a opção de Manutenção
                campoLoteSerie.Enabled       = false;   // Desabilita a opção de edição na caixa de texto "Lote/Série"
                campoPesoLiq.Enabled         = false;   // Desabilita a opção de edição na caixa de texto "Peso Liq."
                campoTara.Enabled            = false;   // Desabilita a opção de edição na caixa de texto "Tara"
                campoPesoBruto.Enabled       = false;   // Desabilita a opção de edição na caixa de texto "Peso Bruto"
                campoGrupoQualidade.Enabled  = false;   // Desabilita a opção de edição na caixa de texto "Grupo Qualidade"
                campoDataProd.Enabled        = false;   // Desabilita a opção de edição na caixa de texto "Data Produção"
                campoDataVal.Enabled         = false;   // Desabilita a opção de edição na caixa de texto "Data Validade"
                LimpaTextos();                          // Caso seja marcado ele limpa as informações antigas
            }

        }

        private void CalculaPeso(bool toZero)
        {
            if (!toZero)
            {
                if (campoSequencia.Text != String.Empty && campoSequencia.Text.Length > 0)
                {
                    int valor = Convert.ToInt32(campoPesoLiq.Text.Replace(",", "").ToString()) * CoreViaSoft.ObterMultiplicadorItem(varItemId);
                    campoPesoBruto.Text = valor.ToString();
                }
            }
            else
            {
                campoPesoBruto.Text = "0";
            }
        }

        private void CalculaTara(bool toZero)
        {
            if (!toZero)
            {
                if (campoTara.Text != String.Empty && campoTara.Text.Length > 0 && campoPesoBruto.Text != String.Empty && campoPesoBruto.Text.Length > 0)
                {
                    decimal tbox5, tbox6;
                    try
                    {
                        tbox5 = Convert.ToDecimal(campoTara.Text);
                    }
                    catch
                    {
                        return;
                    }
                    try
                    {
                        tbox6 = Convert.ToDecimal(campoPesoBruto.Text);
                    }
                    catch
                    {
                        return;
                    }

                    decimal peso_tara = tbox6 - tbox5;
                    campoPesoLiq.Text = peso_tara.ToString();
                }
            }
        }

        private void CarregaConsistencia()
        {
            comboBox2.Items.Clear();
            using (var connection = new MySqlConnection(conecta))
            {
                connection.Open();
                var query = "SELECT grupo, consistencia FROM consistencia";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString("grupo") != qualidadeConsistencia)
                            {
                                comboBox2.Items.Add(reader.GetString("grupo"));
                            }
                        }
                    }
                }
            }
        }

        private void CarregaCor()
        {
            comboBox3.Items.Clear();
            using (var connection = new MySqlConnection(conecta))
            {
                connection.Open();
                var query = "SELECT grupo, cor FROM qualidade_cor";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString("grupo") != qualidadeCor)
                            {
                                comboBox3.Items.Add(reader.GetString("grupo"));
                            }
                        }
                    }
                }
            }
        }

        private void CarregaStatus()
        {
            comboBox4.Items.Clear();
            using (var connection = new MySqlConnection(conecta))
            {
                connection.Open();
                var query = "SELECT codigo, identificacao FROM cod_status";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString("codigo") != qualidadeStatus)
                            {
                                comboBox4.Items.Add(reader.GetString("codigo"));
                            }
                        }
                    }
                }
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxManutencao.SelectedIndex)
            {
                case 0: // Considerar
                    botaoExecutar.Enabled       = true;
                    campoGrupoQualidade.Enabled = false;
                    campoDataProd.Enabled       = false;
                    campoDataVal.Enabled        = false;
                    campoLoteSerie.Enabled      = false;
                    campoPesoLiq.Enabled        = false;
                    campoTara.Enabled           = false;
                    campoPesoBruto.Enabled      = false;

                    comboBox2.Enabled           = false;
                    comboBox3.Enabled           = false;
                    comboBox4.Enabled           = false;

                    comboBox2.Text              = qualidadeConsistencia.ToString();
                    comboBox3.Text              = qualidadeCor.ToString();
                    comboBox4.Text              = qualidadeStatus.ToString();

                    campoGrupoQualidade.Text    = varGrupoQualidade;
                    campoDataProd.Value         = Convert.ToDateTime(varDataProd.ToString());
                    campoDataVal.Value          = Convert.ToDateTime(varDataVal.ToString());
                    campoLoteSerie.Text         = varLoteSerie;
                    campoPesoLiq.Text           = varPesoLiq.ToString();
                    campoPesoBruto.Text         = varPesoBru.ToString();

                    comboBox2.Hide();
                    comboBox3.Hide();
                    comboBox4.Hide();

                    campoGrupoQualidade.Show();

                    if (Properties.Settings.Default.finalidadeUsuario) campoTara.Text = varTara;
                 
                    break;
                case 1: // Desconsiderar

                    campoDataProd.Enabled       = false;
                    campoDataVal.Enabled        = false;
                    campoLoteSerie.Enabled      = false;
                    campoPesoLiq.Enabled        = false;
                    campoTara.Enabled           = false;
                    campoPesoBruto.Enabled      = false;
                    campoGrupoQualidade.Enabled = false;

                    comboBox2.Text              = qualidadeConsistencia.ToString();
                    comboBox3.Text              = qualidadeCor.ToString();
                    comboBox4.Text              = qualidadeStatus.ToString();

                    campoGrupoQualidade.Text    = varGrupoQualidade;
                    campoDataProd.Value         = Convert.ToDateTime(varDataProd.ToString());
                    campoDataVal.Value          = Convert.ToDateTime(varDataVal.ToString());
                    campoLoteSerie.Text         = varLoteSerie;
                    campoPesoLiq.Text           = varPesoLiq.ToString();
                    campoPesoBruto.Text         = varPesoBru.ToString();

                    comboBox2.Enabled           = false;
                    comboBox3.Enabled           = false;
                    comboBox4.Enabled           = false;

                    campoGrupoQualidade.Show();

                    if (Properties.Settings.Default.finalidadeUsuario) campoTara.Text = varTara;
                    

                    // Verificar no banco se já esta mudado para desconsiderado
                    // Se estiver o butao de imprimir continua desabilitado
                    // caso nao, habilita para poder desconsiderar.
                    // Criar essa funcao e usar no butao de impressão
                    if (CoreEtiqueta.ObterStatusEtiqueta(varSequencia)) botaoExecutar.Enabled = false;
                    
                    break;
                case 2: // Atualizar Informações

                    if (CoreEtiqueta.ObterStatusEtiqueta(varSequencia))
                    {
                        botaoExecutar.Enabled = false;
                    }
                    else if (CoreEtiqueta.ObterBaixa(varSequencia))
                    {
                        botaoExecutar.Enabled = false;
                    }
                    else
                    {
                        if (CoreEtiqueta.ObterAsseptico(varSequencia))
                        {
                            campoGrupoQualidade.Hide();

                            comboBox2.Show();
                            comboBox3.Show();
                            comboBox4.Show();

                            comboBox2.Enabled = true;
                            comboBox3.Enabled = true;
                            comboBox4.Enabled = true;

                            comboBox2.Text    = qualidadeConsistencia;
                            comboBox3.Text    = qualidadeCor;
                            comboBox4.Text    = qualidadeStatus;

                            CarregaConsistencia();
                            CarregaCor();
                            CarregaStatus();

                        }
                        else
                        {
                            campoGrupoQualidade.Enabled = true;
                        }
                        campoDataProd.Enabled       = true;
                        campoDataVal.Enabled        = true;
                        campoLoteSerie.Enabled      = true;
                        campoPesoLiq.Enabled        = true;
                        campoPesoBruto.Enabled      = true;
                        botaoExecutar.Enabled       = true;
                        campoTara.Enabled           = true;
                        campoGrupoQualidade.Enabled = true;
                        campoDataProd.Enabled       = true;
                        campoDataVal.Enabled        = true;
                    }
                    break;
                case 3: // Baixar estoque
                    campoDataProd.Enabled       = false;
                    campoDataVal.Enabled        = false;
                    campoLoteSerie.Enabled      = false;
                    campoPesoLiq.Enabled        = false;
                    campoTara.Enabled           = false;
                    campoPesoBruto.Enabled      = false;
                    campoGrupoQualidade.Enabled = false;

                    comboBox2.Enabled           = false;
                    comboBox3.Enabled           = false;
                    comboBox4.Enabled           = false;

                    comboBox2.Text              = qualidadeConsistencia.ToString();
                    comboBox3.Text              = qualidadeCor.ToString();
                    comboBox4.Text              = qualidadeStatus.ToString();


                    campoGrupoQualidade.Text    = varGrupoQualidade;
                    campoDataProd.Value         = Convert.ToDateTime("01/01/2001");
                    campoDataVal.Value          = Convert.ToDateTime("31/12/9998");
                    campoLoteSerie.Text         = varLoteSerie;
                    campoPesoLiq.Text           = varPesoLiq.ToString();
                    campoPesoBruto.Text         = varPesoBru.ToString();


                    comboBox2.Hide();
                    comboBox3.Hide();
                    comboBox4.Hide();

                    if (Properties.Settings.Default.finalidadeUsuario) campoTara.Text = varTara;
   
                    // Verificar no banco se já esta mudado para desconsiderado
                    // Se estiver o butao de imprimir continua desabilitado
                    // caso nao, habilita para poder desconsiderar.
                    // Criar essa funcao e usar no butao de impressão
                    if (CoreEtiqueta.ObterStatusEtiqueta(varSequencia)) botaoExecutar.Enabled = false;
                    if (CoreEtiqueta.ObterBaixa(varSequencia)) botaoExecutar.Enabled          = false;
                    
                    break;
            }
        }

        private void bunifuMaterialTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {
            ConsultaOrdem ci = new ConsultaOrdem();
            if (ci.ShowDialog() == DialogResult.OK)
            {

                campoOrdemProd.Text     = varOrdemId.ToString();                    // Id da Op
                varSequencia            = getLastID();                              // Recupera ultima sequência
                campoSequencia.Text     = varSequencia.ToString();                  // Altera sequencia no textbox
                campoItem.Text          = varItemId.ToString();                     // Id do Item
                labelDescItem.Text      = varItemDesc;                              // Descrição do Item
                campoLoteSerie.Text     = Properties.Settings.Default.lote_fixo;    // Busca sempre lote cadastrado no autopreenchimento


                // Habilitar edição de textbox (Grupo de Qualidade)
                if (!Properties.Settings.Default.finalidadeUsuario)
                {
                    campoGrupoQualidade.Enabled = true;
                    campoGrupoQualidade.Text = Properties.Settings.Default.grupoQualidade;
                }

                campoDataProd.Enabled  = true;                                    // Data produção
                campoDataVal.Enabled   = true;                                    // Data validade
                campoLoteSerie.Enabled = true;                                    // Lote/Serie
                campoPesoLiq.Enabled   = true;                                    // Peso Líquido
                campoPesoBruto.Enabled = true;                                    // Peso Bruto
                campoTara.Enabled      = true;
            }
        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            ConsultaSequencia ciq = new ConsultaSequencia();
            if (ciq.ShowDialog() == DialogResult.OK)
            {
                campoOrdemProd.Text       = varOrdemId.ToString();                      // Id da Op
                campoSequencia.Text       = varSequencia.ToString();                    // Sequencia
                campoGrupoQualidade.Text  = varGrupoQualidade.ToString();               // Grupo Qualidade
                campoItem.Text            = varItemId.ToString();                       // Id do Item
                labelDescItem.Text        = varItemDesc.ToString();                     // Descrição do Item
                campoDataProd.Value       = Convert.ToDateTime(varDataProd.ToString()); // Data de Produção
                campoDataVal.Value        = Convert.ToDateTime(varDataVal.ToString());  // Data de Validade
                campoLoteSerie.Text       = varLoteSerie.ToString();                    // Lote/Serie
                campoPesoLiq.Text         = varPesoLiq.ToString();                      // Peso Liq.
                campoPesoBruto.Text       = varPesoBru.ToString();                      // Peso Bruto

                comboBoxManutencao.Enabled = true;
                comboBoxManutencao.SelectedIndex = Considera ? 0 : 1;                   // Considera ou Desconsidera

                if (CoreEtiqueta.ObterStatusEtiqueta(varSequencia))
                {
                    botaoExecutar.Enabled = false;
                }
                else
                {
                    botaoExecutar.Enabled = true;
                }
            }
        }

        private void bunifuMaterialTextbox6_OnValueChanged(object sender, EventArgs e)
        {
            string n = string.Empty;
            double v = 0;

            try
            {
                n = campoTara.Text.Replace(",", "").Replace(".", "");
                if (n.Equals(""))
                    n = "";

                n = n.PadLeft(3, '0');
                if (n.Length > 3 & n.Substring(0, 1) == "0")
                    n = n.Substring(1, n.Length - 1);

                v = Convert.ToDouble(n) / 100;
                campoTara.Text = string.Format("{0:N}", v);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void bunifuMaterialTextbox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void bunifuMaterialTextbox7_OnValueChanged(object sender, EventArgs e)
        {
            string n = string.Empty;
            double v = 0;
            try
            {
                n = campoPesoLiq.Text.Replace(",", "").Replace(".", "");
                if (n.Equals(""))
                    n = "";

                n = n.PadLeft(3, '0');
                if (n.Length > 3 & n.Substring(0, 1) == "0")
                    n = n.Substring(1, n.Length - 1);

                v = Convert.ToDouble(n) / 100;
                campoPesoLiq.Text = string.Format("{0:N}", v);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }

            if (!Properties.Settings.Default.finalidadeUsuario)
            {
                if (campoPesoLiq.Text.Length > 0)
                {
                    CalculaPeso(false);
                }
                else
                {
                    CalculaPeso(true);
                }
            }
        }

        private void bunifuMaterialTextbox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Properties.Settings.Default.finalidadeUsuario)
            {
                if (e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {

                    if (campoPesoLiq.Text.Length > 0)
                    {
                        CalculaPeso(false);
                    }
                    else
                    {
                        CalculaPeso(true);
                    }
                }
            }
        }

        private void bunifuMaterialTextbox8_OnValueChanged(object sender, EventArgs e)
        {
            string n = string.Empty;
            double v = 0;

            try
            {
                n = campoPesoBruto.Text.Replace(",", "").Replace(".", "");
                if (n.Equals(""))
                    n = "";

                n = n.PadLeft(3, '0');
                if (n.Length > 3 & n.Substring(0, 1) == "0")
                    n = n.Substring(1, n.Length - 1);

                v = Convert.ToDouble(n) / 100;
                campoPesoBruto.Text = string.Format("{0:N}", v);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }


            if (Properties.Settings.Default.finalidadeUsuario)
            {
                if (campoTara.Text.Length > 0)
                {
                    CalculaTara(false);
                }
                else
                {
                    CalculaTara(true);
                }
            }
        }

        private void bunifuMaterialTextbox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Properties.Settings.Default.finalidadeUsuario)
            {
                if (e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && campoPesoBruto.Text != ",")
                {

                    if (campoTara.Text.Length > 0)
                    {
                        CalculaTara(false);
                    }
                    else
                    {
                        CalculaTara(true);
                    }
                }
            }
        }


        // Responsavel por gerar uma nova etiqueta gerando dados no MySQL usando as tabelas
        // (etiquetas_prontas) que poderão ser consultadas que poderão ser consultadas
        // posteriormente pela numeração da sequência ou ordem de produção.
        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if (comboBoxManutencao.SelectedIndex == 0 && checkBoxImpressao.Checked == true || comboBoxManutencao.SelectedIndex == -1 && checkBoxImpressao.Checked == true)
            {
                QuantidadeImpressao qi = new QuantidadeImpressao();
                if (qi.ShowDialog() == DialogResult.OK)
                {
                    if (checkBoxImpressao.Checked == true && varQuantidadeImpressao > 1)
                    {
                        MessageBox.Show("Será gerada a sequência de " + getLastID() + " até " + (getLastID() + varQuantidadeImpressao - 1) + ".");
                    }
                    int n = 0;
                    while (n < varQuantidadeImpressao)
                    {
                        varSequencia = getLastID();
                        campoSequencia.Text = varSequencia.ToString();
                        GeraImpressaoEtiqueta(false);
                        n++;
                    }
                }
            }
            else
            {
                GeraImpressaoEtiqueta(true);
            }
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
