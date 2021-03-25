using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class EstatusSeguimiento
    {

        public string IdSeguimientoOrden { get; set; }
        public string IdCuenta { get; set; }
        public string IdAgencia { get; set; }
        public string IdPreOrden { get; set; }
        public string Autorizado { get; set; }
        public string IdCotizacion { get; set; }

        public List<DetalleAutoCotizacion> IdsDetallesCotizacion { get; set; }

    }


    public class DetalleAutoCotizacion {
        public string IdDetalleCotizacion { get; set; }
    }
}