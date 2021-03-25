using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class ConceptosCotizacion
    {
        public string IdDetalle { get; set; }
        public string IdConcepto { get; set; }
        public string TipoConcepto { get; set; }
        public string Concepto { get; set; }
        public string Precio { get; set; }
    }
}