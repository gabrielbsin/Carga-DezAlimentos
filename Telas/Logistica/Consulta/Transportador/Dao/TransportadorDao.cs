using GerarCargaDez.Telas.Logistica.Consulta.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace GerarCargaDez.Telas.Logistica.Consulta.Dao
{
    class TransportadorDao
    {
        OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Properties.Settings.Default.host_oracle + ")(PORT=" + Properties.Settings.Default.port_oracle + "))) (CONNECT_DATA=(SERVICE_NAME=" + Properties.Settings.Default.sv_oracle + "))); User Id=" + Properties.Settings.Default.user_oracle + "; Password=" + Properties.Settings.Default.pass_oracle + ";");

        public DataTable Listar()
        {
            try
            {
                string query;
                OracleCommand cmd;

                query = "SELECT C.ESTABPRESTADOR, C.PRESTADOR, M.NOME FROM CONTAMOVPES C INNER JOIN CONTAMOV M ON M.NUMEROCM = C.NUMEROCM WHERE C.PRESTADOR > 0";

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

        public DataTable Pesquisar(TransportadorModel value, int selecao)
        {
            try
            {
                string query = null;
                OracleCommand cmd;

                switch(selecao)
                {
                    case 0:
                        query = "SELECT C.ESTABPRESTADOR, C.PRESTADOR, M.NOME FROM CONTAMOVPES C INNER JOIN CONTAMOV M ON M.NUMEROCM = C.NUMEROCM WHERE C.PRESTADOR > 0 AND C.NUMEROCM LIKE'%" + value.Numerocm + "%' ORDER BY C.NUMEROCM";
                        break;
                    case 1:
                        query = "SELECT C.ESTABPRESTADOR, C.PRESTADOR, M.NOME FROM CONTAMOVPES C INNER JOIN CONTAMOV M ON M.NUMEROCM = C.NUMEROCM WHERE C.PRESTADOR > 0 AND M.NOME LIKE'%" + value.Nome + "%' ORDER BY M.NOME";
                        break;
                }

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
