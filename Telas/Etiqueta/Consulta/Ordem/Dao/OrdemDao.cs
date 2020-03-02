using GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace GerarCargaDez.Telas.Etiqueta.Consulta.Ordem.Dao
{
    public class OrdemDao
    {

        OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");

        public DataTable Listar()
        {
            try
            {
                string query;
                OracleCommand cmd;

                query = "SELECT * FROM ORDEMPRODPA ORDER BY IDORDEMPROD";

                conn.Open();

                cmd = new OracleCommand(query, conn);

                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();

                adapter.SelectCommand = cmd;
                adapter.Fill(dt);

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable Pesquisar(OrdemModel ordem)
        {
            try
            {
                string query;
                OracleCommand cmd;

                query = "SELECT * FROM ORDEMPRODPA WHERE IDORDEMPROD LIKE '%" + ordem.Id + "%' ORDER BY IDORDEMPROD";
                conn.Open();
                cmd = new OracleCommand(query, conn);


                DataTable dt = new DataTable();
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                adapter.SelectCommand = cmd;

                adapter.Fill(dt);

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
