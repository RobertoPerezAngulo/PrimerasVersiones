using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class SolicitudPedido
    {
        public string Serie { get; set; }
        public long IdCliente { get; set; }
        public long IdContacto { get; set; }
        public long IdAgente { get; set; }
        public int IdTipoDeVenta { get; set; }
        public decimal Total { get; set; }

    }
}