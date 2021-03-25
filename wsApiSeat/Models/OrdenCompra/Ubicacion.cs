using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class Ubicaciones
    {
        public string IdAgencia { get; set; }
        public string IdUbicacion { get; set; }
        public string Ubicacion { get; set; }
        public string Direccion { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}