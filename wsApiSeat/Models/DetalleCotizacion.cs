using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class DetalleCotizacion
    {
        public string IdDetalle { get; set; }
        public string Fecha { get; set; }
        public List<ConceptosCotizacion> Conceptos { get; set; }
        public string TotalCotizacion { get; set; }
    }
}