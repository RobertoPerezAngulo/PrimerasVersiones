using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class CompraNotificacion
    {
        public Compra compra { get; set; }
        public Notificacion  notificacion {get; set;}
    }
}