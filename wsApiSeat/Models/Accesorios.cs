using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class Accesorios
    {
        public string Concepto { get; set; }
        public string Ruta { get; set; }
        public string RutaLocal { get; set; }
        public string SubTotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }

    }
}