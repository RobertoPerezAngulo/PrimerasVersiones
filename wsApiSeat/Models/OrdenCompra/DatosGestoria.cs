using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class DatosGestoria
    {
        public string IdCompra {get; set;}
        public string IdTramiteSeleccionado {get; set;}
        public string ElegioTramite {get; set;}        
        public string PorcentajeMensual {get; set;}
        public string Total { get; set;}

    }
}