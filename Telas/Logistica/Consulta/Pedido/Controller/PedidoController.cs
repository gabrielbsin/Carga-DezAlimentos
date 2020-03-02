using GerarCargaDez.Telas.Logistica.Consulta.Pedido.Dao;
using GerarCargaDez.Telas.Logistica.Consulta.Pedido.Model;
using System;
using System.Data;
using System.Windows.Forms;

namespace GerarCargaDez.Telas.Logistica.Consulta.Pedido.Controller
{
    class PedidoController
    {
        public DataTable Listar()
        {
            try
            {
                PedidoDao od = new PedidoDao();
                DataTable dt = new DataTable();

                dt = od.Listar();

                return dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }

        public DataTable Pesquisar(PedidoModel odm, int selecao)
        {
            try
            {
                PedidoDao od = new PedidoDao();
                DataTable dt = new DataTable();

                dt = od.Pesquisar(odm, selecao);

                return dt;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Problemas de conexão: " + ex.Message);
                return null;
            }
        }
    }
}
