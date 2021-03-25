using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Notificaciones
{
    public class EstadoNotificaciones
    {
        public string IdEstado { get; set; }
        public string Descripcion { get; set; }
        public List<AsuntoNotificaciones> Asuntos {get; set;}

    }
}