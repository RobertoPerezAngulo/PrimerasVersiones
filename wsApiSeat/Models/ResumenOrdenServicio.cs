using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class ResumenOrdenServicio
    {
        public string IdAgencia { get; set; }
        public string IdPreorden { get; set; }
        public string Refacciones { get; set; }
        public string Paquetes { get; set; }
        public string OtrosTrabajos { get; set; }
        public string Operaciones { get; set; }
        public string Cargos { get; set; }
        public string Descuentos { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
    }
}