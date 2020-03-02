using GerarCargaDez.Telas.Expedicao.Relatorio;
using Microsoft.Reporting.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Expedicao
{
    public partial class EmiteRomaneio : Form
    {
        public EmiteRomaneio()
        {
            InitializeComponent();
        }

        string conexao_carga = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private int VerificaEmbarque(int embarque_id)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                string query = "SELECT * FROM cargadez_embarque WHERE embarque_id='" + embarque_id + "'";
                MySqlCommand comando = new MySqlCommand(query, conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        return Convert.ToInt32(drCommand["status"].ToString());
                    }
                } 
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } finally
            {
                conexao.Close();
            }
            return 0;
        }

        private bool ExisteItensEmbarque(int embarque_id)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarque_id +"'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();
                if (drCommand.HasRows)
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

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bunifuMaterialTextbox1.Text) || bunifuMaterialTextbox1.Text.Length > 0 && bunifuMaterialTextbox1.Text.Trim().Length == 0)
            {
                MessageBox.Show("Você precisa preencher o campo com número do Embarque!", "Campo Vazio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int embarque_id = Convert.ToInt32(bunifuMaterialTextbox1.Text);

            if (VerificaEmbarque(embarque_id) == 0)
            {
                MessageBox.Show("O número do Embarque não é valido!", "Campo Invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ExisteItensEmbarque(embarque_id))
            {
                MessageBox.Show("Não existem itens para exibir o romaneio!", "Campo Invalido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                bool resumo = checkBox1.Checked == true;
                string doc = resumo ? "RomaneioDeCarga_.rdlc" : "RomaneioDeCarga.rdlc";

                System.Data.DataSet ds = new System.Data.DataSet();
                System.Data.DataSet ds_ = new System.Data.DataSet();

                string query = "SELECT E.EMBARQUE_ID as EMB, C.TRANSPORTADOR, C.TRANSPORTADOR_NOME, E.ITEM, E.DESCRICAO, SUM(E.QUANTIDADE) as QUANTIDADE, SUM(E.PESO_LIQ) as PESO_LIQ, SUM(E.PESO_BRU) as PESO_BRU, TRUNCATE((E.QUANTIDADE) / U.CAIXAXPALETES, 0) PALLETS, TRUNCATE((E.QUANTIDADE) / U.CAIXAXPALETES, 0) PALLETSEMB_OLD, MOD(((E.QUANTIDADE) / U.CAIXAXPALETES), 2) * U.CAIXAXPALETES CAIXAS, TRUNCATE(SUM((TRUNCATE(ROUND(((E.QUANTIDADE) / U.CAIXAXPALETES) * 100), 0) / 100)), 0) quantidade_palete, TRUNCATE(SUM((TRUNCATE(ROUND(((E.QUANTIDADE) / U.CAIXAXPALETES) * 100), 0) / 100)), 0) * U.CAIXAXPALETES QTDCAIXASPALLETS, SUM(E.quantidade) - TRUNCATE(SUM((TRUNCATE(ROUND(((E.QUANTIDADE) / (U.CAIXAXPALETES)) * 100), 0) / 100)), 0) * (U.CAIXAXPALETES)DIFERENCA_CX, C.PLACA, C.MOTORISTA, C.UF_MOTORISTA FROM CARGADEZ_EMBARQUE_ITENS E INNER JOIN CARGADEZ_EMBARQUE C ON C.EMBARQUE_ID = E.EMBARQUE_ID LEFT JOIN CARGADEZ_ITEM_U U ON E.ITEM = U.ITEM WHERE E.EMBARQUE_ID = '"+ embarque_id + "' AND SELECIONADO = '1' GROUP BY E.ITEM";
                MySqlDataAdapter adp = new MySqlDataAdapter(query, conexao);
                adp.Fill(ds);

                string query_ = "SELECT cp.pedido_id AS pedf, cp.pessoa_id, cp.pessoa_desc, cp.cidade_desc, cp.cidade_uf, e.item AS itemp, e.descricao AS descp, SUM(E.QUANTIDADE) as QUANTIDADE, SUM(E.PESO_LIQ) as PESO_LIQ, SUM(E.PESO_BRU) as PESO_BRU,  TRUNCATE(SUM((TRUNCATE(ROUND(((E.QUANTIDADE) / U.CAIXAXPALETES) * 100), 0) / 100)), 0) quantidade_palete, SUM(E.quantidade) - TRUNCATE(SUM((TRUNCATE(ROUND(((E.QUANTIDADE) / (U.CAIXAXPALETES)) * 100), 0) / 100)), 0) * (U.CAIXAXPALETES)DIFERENCA_CX FROM cargadez_embarque_itens e INNER JOIN cargadez_pedido cp ON cp.pedido_id = e.pedido_id LEFT JOIN CARGADEZ_ITEM_U U ON E.ITEM = U.ITEM WHERE embarque_id = '" + embarque_id + "' AND e.selecionado = '1' GROUP BY e.item, e.pedido_id  ORDER BY cp.pessoa_desc";
                adp = new MySqlDataAdapter(query_, conexao);
                adp.Fill(ds_);

                ReportDataSource fuente = new ReportDataSource("DataSet1", ds.Tables[0]);
                fuente.Name = "DataSet1";

                ReportDataSource fuente_ = new ReportDataSource("DataSet2", ds_.Tables[0]);
                fuente_.Name = "DataSet2";

                TelaDeImpressao tdi = new TelaDeImpressao();
                tdi.Show();

                tdi.reportViewer1.LocalReport.DataSources.Clear();
                tdi.reportViewer1.LocalReport.DataSources.Add(fuente);
                tdi.reportViewer1.LocalReport.DataSources.Add(fuente_);
                tdi.reportViewer1.LocalReport.ReportEmbeddedResource = "GerarCargaDez.Telas.Expedicao.Relatorio." + doc;

                tdi.reportViewer1.LocalReport.Refresh();
                tdi.reportViewer1.Refresh();
                tdi.reportViewer1.RefreshReport();

            } catch (Exception ex)
            {
                MessageBox.Show("Não foi possível gerar o relatório!" + ex.ToString(), "Gerando relatório...", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            } finally
            {
                conexao.Close();
            }
        }

        private void BunifuMaterialTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
