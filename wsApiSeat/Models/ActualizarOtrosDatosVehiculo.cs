using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class ActualizarOtrosDatosVehiculo
    {
        public string IdCuenta { get; set; }
        public string IdVehiculo { get; set; }
        public string IdAseguradora {get; set;}
        public string IdEngomado { get; set; }
        public string FechaGarantiaExt { get; set; }

    }
}