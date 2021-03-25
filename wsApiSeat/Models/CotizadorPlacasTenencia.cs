using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class CotizadorPlacasTenencia
    {
        public string IdTramite { get; set; }
        public string DescripcionTramite { get; set; }
        public string PrecioPlacas { get; set; }            
        public string PorcentajeMensual { get; set; }    
        public string PrecioTenencia { get; set; }
        public string PrecioFacturaSinIva { get; set; }
        public string Total { get; set; }

    }
    
}