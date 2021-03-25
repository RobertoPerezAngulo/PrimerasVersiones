using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class CotizacionPreorden
    {
        public string Id { get; set; }
        public string IdAgencia { get; set; }
        public string IdPreorden { get; set; }
        public List<DetalleCotizacion> Detalle { get; set; }
    }
}