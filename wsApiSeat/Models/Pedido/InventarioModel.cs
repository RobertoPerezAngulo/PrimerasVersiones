using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Pedido
{
    public class InventarioModel
    {
        public string IdAgencia { get; set; }
        public string IdInventario { get; set; }
        public string IdVehiculo { get; set; }
        public string IdVersion { get; set; }
        public string IdColor { get; set; }
        public string NumeroSerie { get; set; }
        public string Color { get; set; }
        public string NumeroInventario { get; set; }
        public string Ruta { get; set; }
    }
}