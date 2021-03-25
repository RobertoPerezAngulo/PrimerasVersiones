using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class SeguimientoOrden
    {
        public string IdSegOrden { get; set; }
        public string IdCuenta { get; set; }
        public string IdAgencia { get; set; }
        public string IdPreorden { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Titulo { get; set; }
        public string Observacion { get; set; }
        public string IdCotizacion { get; set; }
        public string Autorizado { get; set; }
        public string Visto { get; set; }
    }
}