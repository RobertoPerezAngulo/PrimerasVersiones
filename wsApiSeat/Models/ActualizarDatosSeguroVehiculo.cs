using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class ActualizarDatosSeguroVehiculo
    {
        public string IdCuenta { get; set; }
        public string IdVehiculo { get; set; }
        public string NumeroPoliza { get; set; }
        public string IdAseguradora { get; set; }
        public string TelAseguradora { get; set; }
        public string FechaVencimiento { get; set; }

    }
}