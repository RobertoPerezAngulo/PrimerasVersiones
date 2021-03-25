using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class NotificacionCompra
    {
        
         public string IdEstado { get; set; }
         public string IdCompra { get; set; }
        public string IdCuenta { get; set; }
        public string Asunto { get; set; }
        public string DescripcionNotificacion { get; set; }

    }
}