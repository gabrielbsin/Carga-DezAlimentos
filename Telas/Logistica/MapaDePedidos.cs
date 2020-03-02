using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GerarCargaDez.Core;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace GerarCargaDez.Telas.Logistica
{
    public partial class MapaDePedidos : Form
    {
        public static List<PointLatLng> _points;
        public MapaDePedidos()
        {
            InitializeComponent();
            _points = new List<PointLatLng>();
            _points.Add(new PointLatLng(LatInicial, LngInicial));
            this.IniciarMapa();
        }

        public GMarkerGoogle marcador;
        public GMapOverlay marcadorOverlay;
        public static DataTable dt;

        private double LatInicial = -17.770611;
        private double LngInicial = -49.127413;

        private List<PointLatLng> routePath = new List<PointLatLng>();

        string conexao_carga   = "SERVER=" + Properties.Settings.Default.host_mysql + "; DATABASE=cargadez; UID=" + Properties.Settings.Default.user_mysql + "; pwd=" + Properties.Settings.Default.pass_mysql + "";
        string conexao_viasoft = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";";

        public void IniciarMapa()
        {
            try
            {
                GMapProviders.GoogleMap.ApiKey = Properties.Settings.Default.googlekey;
                gMapControl1.Manager.Mode      = AccessMode.ServerOnly;
                gMapControl1.DragButton        = MouseButtons.Left;
                gMapControl1.CanDragMap        = true;
                gMapControl1.MapProvider       = GMapProviders.GoogleMap;
                gMapControl1.Position          = new PointLatLng(this.LatInicial, this.LngInicial);
                gMapControl1.MinZoom           = 0;
                gMapControl1.MaxZoom           = 24;
                gMapControl1.Zoom              = 9;
                gMapControl1.AutoScroll        = true;
                gMapControl1.ShowCenter        = false;

                
                CriarMarcadores();
                CarregaRotas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao carregar mapa de pedidos!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
        }

        private void MapaDePedidos_Load(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt = MontarCarga.dt_LoadPedidosSelecionados;
            bunifuCustomDataGrid1.DataSource = dt;
        }

        private void CriarMarcadores()
        {
            OracleConnection conexao_ = new OracleConnection(conexao_viasoft);
            try
            {
                conexao_.Open();
                OracleCommand comando      = new OracleCommand("SELECT PC.NUMERO, PC.PESSOA, PC.SERIE, CM.NUMEROCM, CM.NOME, CM.ENDERECO, CM.BAIRRO, CD.NOME AS CIDADE, CD.UF AS CIDADE_UF, CM.LATITUDEENDERECOCOB, CM.LONGITUDEENDERECOCOB FROM PEDCAB PC INNER JOIN CONTAMOV CM ON CM.NUMEROCM = PC.PESSOA INNER JOIN CIDADE CD ON CD.CIDADE = CM.CIDADE WHERE PC.STATUS = 'N' AND PC.SERIE <> 'PC' ORDER BY PC.NUMERO", conexao_);
                OracleDataReader odr       = comando.ExecuteReader();

                if (odr.HasRows)
                {
                    while (odr.Read())
                    {
                     
                        int pedidoId            = Convert.ToInt32(odr["NUMERO"]);
                        int fornecedor          = Convert.ToInt32(odr["PESSOA"]);
                        string pedidoSerie      = odr["SERIE"].ToString();
                        string fornecedorDesc   = odr["NOME"].ToString();
                        string fornecedorCidade = odr["CIDADE"].ToString();
                        string fornecedorEnd    = odr["ENDERECO"].ToString();
                        string fornecedorBairro = odr["BAIRRO"].ToString();
                        string fornecedorUF     = odr["CIDADE_UF"].ToString();
                        
                        double lat_fornecedor   = 0.0;
                        double lng_fornecedor   = 0.0;

                        if (lat_fornecedor == 0.0 || lng_fornecedor == 0.0)
                        {
                            GeoCoderStatusCode statusCode;
                            var pointLatLng = GoogleMapProvider.Instance.GetPoint(fornecedorEnd + "," + fornecedorBairro + ", " + fornecedorCidade + "," + fornecedorUF + ",BR", out statusCode);
                            if (statusCode == GeoCoderStatusCode.OK)
                            {
                                double lat_ = Convert.ToDouble(pointLatLng?.Lat);
                                double lng_ = Convert.ToDouble(pointLatLng?.Lng);

                                lat_fornecedor = lat_;
                                lng_fornecedor = lng_;

                            } else
                            {
                                lat_fornecedor = 0.0;
                                lng_fornecedor = 0.0;
                            }
                        }
                        if (lat_fornecedor != 0.0 || lng_fornecedor != 0.0)
                        {
                            bool addPR = AdicionaPontoRota(pedidoId, MontarCarga.embarque_Id);

                            marcadorOverlay = new GMapOverlay("Marcador");
                            PointLatLng t = new PointLatLng(lat_fornecedor, lng_fornecedor);
                            if (!t.IsEmpty)
                            {
                                marcador = new GMarkerGoogle(t, addPR ? GMarkerGoogleType.blue_dot : GMarkerGoogleType.green_dot);

                                if (marcador != null)
                                {
                                    marcadorOverlay.Markers.Add(marcador);
                                    marcador.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                    marcador.ToolTipText = string.Format("Pedido: {0}\nFornecedor: {1} - {2}\nSerie: {3}", pedidoId, fornecedor, fornecedorDesc, pedidoSerie);
                                    if (marcadorOverlay != null)
                                    {
                                        gMapControl1.Overlays.Add(marcadorOverlay);
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao criar todos marcadores do mapa de pedidos!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao_.Close();
            }
        }

        private bool AdicionaPontoRota(int pedidoId, int embarqueid)
        {
            bool adicionaPonto = false;
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando = new MySqlCommand("SELECT * FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarqueid + "' AND pedido_id='" + pedidoId + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    adicionaPonto = true;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao criar rota!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            } finally
            {
                conexao.Close();
            }
            return adicionaPonto;
        }

        public void CarregaRotas()
        {
            List<Tuple<int, string>> pedidos = new List<Tuple<int, string>>();
            int count = MontarCarga.bunifuCustomDataGrid2.RowCount;
            for (int i=0; i < count; i++)
            {
                int pedido_id = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[0].Value.ToString());
                string serie  = MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[7].Value.ToString();
                if (pedido_id > 0)
                {
                    pedidos.Add(new Tuple<int, string>(pedido_id, serie));
                }
            }

            int count_ = MontarCarga.bunifuCustomDataGrid3.RowCount;
            for (int i = 0; i < count_; i++)
            {
                int pedido_id = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid3.Rows[i].Cells[0].Value.ToString());
                string serie  = MontarCarga.bunifuCustomDataGrid2.Rows[i].Cells[3].Value.ToString();
                if (pedido_id > 0)
                {
                    pedidos.Add(new Tuple<int, string>(pedido_id, serie));
                }
            }

            pedidos.Sort();
            Int32 index = pedidos.Count - 1;
            while (index > 0)
            {
                if (pedidos[index] == pedidos[index - 1])
                {
                    if (index < pedidos.Count - 1)
                        (pedidos[index], pedidos[pedidos.Count - 1]) = (pedidos[pedidos.Count - 1], pedidos[index]);
                    pedidos.RemoveAt(pedidos.Count - 1);
                    index--;
                }
                else
                    index--;
            }


            int countP = pedidos.Count;
            if (countP > 0)
            {
                for (int x = 0; x < countP; x++)
                {
                    OracleConnection conexao = new OracleConnection(conexao_viasoft);
                    try
                    {
                        conexao.Open();
                        OracleCommand comando = new OracleCommand("SELECT PC.NUMERO, PC.PESSOA, PC.SERIE, CM.NUMEROCM, CM.NOME, CM.ENDERECO, CM.BAIRRO, CD.NOME AS CIDADE, CD.UF AS CIDADE_UF, CM.LATITUDEENDERECOCOB, CM.LONGITUDEENDERECOCOB FROM PEDCAB PC INNER JOIN CONTAMOV CM ON CM.NUMEROCM = PC.PESSOA INNER JOIN CIDADE CD ON CD.CIDADE = CM.CIDADE WHERE PC.STATUS = 'N' AND PC.SERIE <> 'PC' ORDER BY PC.NUMERO", conexao);
                        OracleDataReader odr = comando.ExecuteReader();

                        if (odr.HasRows)
                        {
                            while (odr.Read())
                            {
                                int pedidoId = Convert.ToInt32(odr["NUMERO"]);
                                if (pedidos[x].Item1 == pedidoId)
                                {
                                    string fornecedorCidade = odr["CIDADE"].ToString();
                                    string fornecedorEnd = odr["ENDERECO"].ToString();
                                    string fornecedorBairro = odr["BAIRRO"].ToString();
                                    string fornecedorUF = odr["CIDADE_UF"].ToString();

                                    GeoCoderStatusCode statusCode;
                                    var pointLatLng = GoogleMapProvider.Instance.GetPoint(fornecedorEnd + "," + fornecedorBairro + ", " + fornecedorCidade + "," + fornecedorUF + ",BR", out statusCode);

                                    if (statusCode == GeoCoderStatusCode.OK)
                                    {
                                        double lat_ = Convert.ToDouble(pointLatLng?.Lat);
                                        double lng_ = Convert.ToDouble(pointLatLng?.Lng);

                                        _points.Add(new PointLatLng(lat_, lng_));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Houve um erro ao carregar rota no mapa de pedidos!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                    finally
                    {
                        conexao.Close();
                    }
                }
            }
        }

        private void GMapControl1_OnMarkerDoubleClick(GMapMarker item, MouseEventArgs e)
        {
            _points.Add(new PointLatLng(item.Position.Lat, item.Position.Lng));

            Match matchPedido = Regex.Match(item.ToolTipText, @"Pedido: ([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase);
            string pedidoString = "";
            if (matchPedido.Success)
            {
                pedidoString = matchPedido.Groups[1].Value;
            }
            Match matchPedidoSerie = Regex.Match(item.ToolTipText, @"Serie: ([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase);
            string pedidoSerieString = "";
            if (matchPedidoSerie.Success)
            {
                pedidoSerieString = matchPedidoSerie.Groups[1].Value;
            }

            if (pedidoString != null && pedidoSerieString != null)
            {
                int pedido_id = Convert.ToInt32(pedidoString);
                if (!PedidoJaSelecionado(pedido_id, pedidoSerieString)) {
                    if (VerificaPedido(pedido_id, true, true))
                    {
                        CarregaPedidosSelecionados(pedido_id, pedidoSerieString);


                        int count = MontarCarga.bunifuCustomDataGrid1.Rows.Count;
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                int pedido_id_s = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                                if (pedido_id == pedido_id_s)
                                {
                                    MontarCarga.bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Este pedido já esta selecionado!", "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }
                } else
                {
                    MessageBox.Show("Este pedido já esta vinculado a outro Embarque!", "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conexao.Close();
            }
            return false;
        }

        private bool VerificaPedido(int pedido, bool pedidosSelecionados, bool itensSelecionados)
        {
            if (pedidosSelecionados)
            {
                int count = dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                    if (pedido_id == pedido)
                    {
                        return false;
                    }
                }
            }
            if (itensSelecionados)
            {
                int count_ = MontarCarga.dt_LoadItensSelecionados.Rows.Count;
                for (int i = 0; i < count_; i++)
                {
                    int pedido_id = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid3.Rows[i].Cells[0].Value.ToString());
                    if (pedido_id == pedido)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void InsertPedidosSelecionados(int pedido_id, long item_id, int quantidade, string serie, int sequencia)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                MySqlCommand comando;
                int embarque_id    = Convert.ToInt32(MontarCarga.embarque_final);
                double peso_liq    = CalculoPedidosLiquido(item_id, quantidade);
                double peso_bru    = CalculoPedidosBruto(item_id, quantidade);
                string item_desc   = CoreViaSoft.ObterDadosItem(item_id, false);

                comando = new MySqlCommand("INSERT INTO CARGADEZ_EMBARQUE_ITENS (embarque_id, pedido_id, item, descricao, quantidade, peso_liq, peso_bru, serie, sequencia, selecionado) VALUES (" + embarque_id + ", " + pedido_id + ", " + item_id + ", '" + item_desc + "', " + quantidade + ", '" + peso_liq + "', '" + peso_bru + "', '" + serie + "', '" + sequencia + "', 0)", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao inserir o pedido MySQL!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        public double CalculoPedidosLiquido(long item_id, int quant)
        {
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            double liquido = 0.0;
            try
            {
                conexao.Open();
                int multiple         = CoreViaSoft.ObterMultiplicadorItem(item_id);
                bool hasMult         = multiple > 0;
                OracleCommand cmd    = new OracleCommand("SELECT PESOLIQUIDO FROM ITEMAGRO WHERE ITEM=" + item_id + "", conexao);
                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    if (odr.Read())
                    {
                        double peso = Convert.ToDouble(odr["PESOLIQUIDO"].ToString());
                        double calc = hasMult ? (quant * multiple) * peso : (quant * peso);
                        liquido += calc;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao calcular peso liquido!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
            return liquido;
        }

        public double CalculoPedidosBruto(long item_id, int quant)
        {
            OracleConnection conexao = new OracleConnection(conexao_viasoft);
            double bruto = 0.0;
            try
            {
                conexao.Open();
                int multiple         = CoreViaSoft.ObterMultiplicadorItem(item_id);
                bool hasMult         = multiple > 0;
                OracleCommand cmd    = new OracleCommand("SELECT PESOBRUTO FROM ITEMAGRO WHERE ITEM=" + item_id + "", conexao);
                OracleDataReader odr = cmd.ExecuteReader();
                if (odr.HasRows)
                {
                    if (odr.Read())
                    {
                        double peso = Convert.ToDouble(odr["PESOBRUTO"].ToString());
                        double calc = hasMult ? (quant * multiple) * peso : (quant * peso);
                        bruto += calc;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao calcular peso bruto!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
            return bruto;
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
                    OracleDataReader odr  = comando.ExecuteReader();

                    if (odr.HasRows)
                    {
                        while (odr.Read())
                        {
                            long item_id = Convert.ToInt32(odr["ITEM"]);
                            this.AtualizaDisponivel(item_id);
                            CoreViaSoft.AtualizaCaixaItemU(item_id);

                            int multiplo        = CoreViaSoft.ObterMultiplicadorItem(item_id);
                            bool hasMult        = multiplo > 0;

                            int qnt_pedido      = Convert.ToInt32(odr["QUANTIDADE"]);
                            int qnt_pedido_     = hasMult ? qnt_pedido / multiplo : qnt_pedido;
                            int qnt_aloc        = hasMult ? qnt_pedido / multiplo : qnt_pedido;

                            int sequencia       = Convert.ToInt32(odr["SEQPEDITE"]);
                            string serie_       = odr["SERIE"].ToString();
                            string item_Desc    = CoreViaSoft.ObterDadosItem(item_id, false);

                            int saldoViaSoft    = hasMult ? (CoreViaSoft.SaldoItem(item_id) / multiplo) : CoreViaSoft.SaldoItem(item_id);
                            int saldoViaSoftEmb = hasMult ? (CoreViaSoft.SaldoEmbarcado(item_id) / multiplo) : CoreViaSoft.SaldoEmbarcado(item_id);

                            InsertPedidosSelecionados(pedido, item_id, qnt_pedido_, serie_, sequencia);
                            dt.Rows.Add(pedido, item_id, qnt_aloc, qnt_pedido_, saldoViaSoftEmb, saldoViaSoft, item_Desc, serie_, sequencia);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Houve um erro ao carregar os pedidos selecionados!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                finally
                {
                    conexao.Close();
                }
            } else
            {
                MessageBox.Show("Este item já foi selecionado!", "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
        }

        private void AtualizaDisponivel(long item_id)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            bool contemSaldo = false;
            try
            {
                conexao.Open();
                MySqlCommand comando      = new MySqlCommand("SELECT * FROM CARGADEZ_SALDOITEM WHERE item='" + item_id + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    contemSaldo = true;
                }

                drCommand.Dispose();

                if (contemSaldo)
                {
                    comando   = new MySqlCommand("SELECT * FROM CARGADEZ_SALDOITEM WHERE item='" + item_id + "'", conexao);
                    drCommand = comando.ExecuteReader();

                    int saldoDisponivel = 0, saldoImplantado = 0;
                    if (drCommand.HasRows)
                    {
                        while (drCommand.Read())
                        {
                            saldoDisponivel = Convert.ToInt32(drCommand["disponivel"].ToString());
                            saldoImplantado = Convert.ToInt32(drCommand["saldo_implantado"].ToString());
                        }
                    }

                    drCommand.Dispose();

                    int saldoDoItem = CoreViaSoft.SaldoItem(item_id);
                    if (saldoDoItem > saldoImplantado)
                    {
                        int dif = saldoDoItem - saldoImplantado;
                        int atualizaSaldo = saldoDisponivel + dif;

                        comando = new MySqlCommand("UPDATE CARGADEZ_SALDOITEM SET disponivel='" + atualizaSaldo + "', saldo_implantado='" + saldoDoItem + "' WHERE item='" + item_id + "'", conexao);
                        comando.ExecuteNonQuery();
                        comando.Dispose();
                    }
                }
                else
                {
                    int saldoDisponivel = CoreViaSoft.SaldoItem(item_id);
                    comando = new MySqlCommand("INSERT INTO CARGADEZ_SALDOITEM (item, disponivel, saldo_implantado) VALUES (" + item_id + ", " + saldoDisponivel + ", " + saldoDisponivel + ")", conexao);
                    comando.ExecuteNonQuery();
                    comando.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao atualizar saldos MySQL!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt32(gMapControl1.Zoom);
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            gMapControl1.Zoom = trackBar1.Value;
        }

        GMapRoute gr;
        GMapOverlay go;
        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if (_points.Count > 1)
            {
                double km = 0.0;
                PointLatLng start = _points[0];
                PointLatLng end = _points[1];
                RoutingProvider rp = gMapControl1.MapProvider as RoutingProvider;

                if (rp == null)
                {
                    rp = GMapProviders.GoogleMap;
                }

                for (int i = 0; i < _points.Count - 1; i++)
                {
                    start = _points[i];
                    end = _points[i + 1];

                    var route = rp.GetRoute(start, end, false, false, (int) gMapControl1.Zoom);
                    routePath.AddRange(route.Points);

                    if (route != null)
                    {
                        gr = new GMapRoute(route.Points, "Minha Rota")
                        {
                            Stroke = new Pen(Color.Green, 5)
                        };

                        go = new GMapOverlay("routes");
                        go.Routes.Add(gr);

                        gMapControl1.Overlays.Add(go);

                        gMapControl1.Zoom = gMapControl1.Zoom + 1;
                        gMapControl1.Zoom = gMapControl1.Zoom - 1;
                        double countKm = route.Distance;
                        km += countKm;
                    }
                }
                label3.Text = km.ToString();
            } else
            {
                MessageBox.Show("Não existem pontos para criar uma rota!");
            }
        }

        private void BunifuFlatButton2_Click(object sender, EventArgs e)
        {
            if (bunifuCustomDataGrid1.CurrentRow != null)
            {
                int colunaSelecionada = bunifuCustomDataGrid1.CurrentRow.Index;
                if (colunaSelecionada >= 0)
                {
                    int pedido_id = Convert.ToInt32(bunifuCustomDataGrid1.Rows[colunaSelecionada].Cells[0].Value.ToString());
                    if (pedido_id > 0)
                    {

                        bool found = false;
                        int count = MontarCarga.bunifuCustomDataGrid1.Rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!found)
                            {
                                int rows = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid1.Rows[i].Cells[0].Value.ToString());
                                if (pedido_id == rows)
                                {
                                    MontarCarga.bunifuCustomDataGrid1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                                    found = true;
                                }
                            }
                        }

                        int count_ = bunifuCustomDataGrid1.Rows.Count;
                        for (int j = count_; j > 0; j--)
                        {
                            int rows = Convert.ToInt32(bunifuCustomDataGrid1.Rows[j - 1].Cells[0].Value.ToString());
                            if (pedido_id == rows)
                            {
                                long item_id = Convert.ToInt32(bunifuCustomDataGrid1.Rows[j - 1].Cells[1].Value.ToString());
                                RemovePedidoSelecionado(rows, item_id);
                                bunifuCustomDataGrid1.Rows.RemoveAt(bunifuCustomDataGrid1.Rows[j - 1].Index);
                            }
                        }

                        int count__ = MontarCarga.bunifuCustomDataGrid3.Rows.Count;
                        for (int j = count__; j > 0; j--)
                        {
                            int rows = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid3.Rows[j - 1].Cells[0].Value.ToString());
                            if (pedido_id == rows)
                            {
                                long item_id = Convert.ToInt32(MontarCarga.bunifuCustomDataGrid3.Rows[j - 1].Cells[1].Value.ToString());
                                AlteraDisponivelExcluidos(rows, item_id, true);
                                RemovePedidoSelecionado(rows, item_id);
                                MontarCarga.bunifuCustomDataGrid3.Rows.RemoveAt(MontarCarga.bunifuCustomDataGrid3.Rows[j - 1].Index);
                            }
                        }
                    }
                }
            }
        }

        private void AlteraDisponivel(long item_id, int quantidade, bool soma)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                string operacao = soma ? "(disponivel + " + quantidade + ")" : "(disponivel - " + quantidade + ")";
                MySqlCommand comando = new MySqlCommand("UPDATE cargadez_saldoitem SET disponivel=" + operacao + " WHERE item='" + item_id + "'", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao atualizar saldo do Item MySQL!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void AlteraDisponivelExcluidos(int pedido_id, long item, bool soma)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                int embarque_id           = Convert.ToInt32(MontarCarga.embarque_final);
                MySqlCommand comando      = new MySqlCommand("SELECT * FROM cargadez_embarque_itens WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND item='" + item + "'", conexao);
                MySqlDataReader drCommand = comando.ExecuteReader();

                if (drCommand.HasRows)
                {
                    while (drCommand.Read())
                    {
                        long item_id      = Convert.ToInt64(drCommand["item"]);
                        int quantidade    = Convert.ToInt32(drCommand["quantidade"]);
                        int multiplicador = CoreViaSoft.ObterMultiplicadorItem(item_id);
                        bool hasMult      = multiplicador > 0;
                        int quantidade_r  = hasMult ? (quantidade * multiplicador) : quantidade;
                        AlteraDisponivel(item_id, quantidade_r, soma);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao atualizar saldo do Item MySQL!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void RemovePedidoSelecionado(int pedido_id, long item)
        {
            MySqlConnection conexao = new MySqlConnection(conexao_carga);
            try
            {
                conexao.Open();
                int embarque_id      = Convert.ToInt32(MontarCarga.embarque_final);
                MySqlCommand comando = new MySqlCommand("DELETE FROM CARGADEZ_EMBARQUE_ITENS WHERE embarque_id='" + embarque_id + "' AND pedido_id='" + pedido_id + "' AND item='" + item + "'", conexao);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro ao remover pedido MySQL!\n" + ex.ToString(), "Mapa de Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void BunifuFlatButton3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void BunifuImageButton4_Click(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMapProviders.GoogleChinaSatelliteMap;
        }

        private void BunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BunifuImageButton5_Click(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
        }
    }
}
