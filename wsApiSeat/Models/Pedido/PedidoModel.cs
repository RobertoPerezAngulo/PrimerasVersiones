using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class PedidoModel
    {
        public string IdPedido { get; set; }
        public string IdAgencia { get; set; }
        public string Fecha { get; set; }
        public string IdCliente { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string IdInventario { get; set; }
        public string IdVehiculo { get; set; }
        public string IdVersion { get; set; }
        public string NumeroSerie { get; set; }
        public string SubTotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
    }
}