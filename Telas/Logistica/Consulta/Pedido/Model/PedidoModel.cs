using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerarCargaDez.Telas.Logistica.Consulta.Pedido.Model
{
    class PedidoModel
    {
        public int Pedido_Id { get; set; }

        public int Pessoa_Id { get; set; }

        public string Pessoa_Desc { get; set; }

        public string Cidade_nome { get; set; }

        public string UF_Desc { get; set; }

        public string User_Desc { get; set; }
    }
}
