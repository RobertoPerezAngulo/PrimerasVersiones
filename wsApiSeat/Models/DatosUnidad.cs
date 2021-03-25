using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class DatosUnidad
    {
        public string SerieAuto { get; set; }
        public decimal Iva { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int IdVehiculo { get; set; }
        public int IdInventario { get; set; }
    }
}