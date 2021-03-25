using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiSeat.WsEncuestasPSI;

namespace wsApiSeat.Models.Notificaciones
{
    public class AsuntoNotificaciones
    { 
        public string IdAsunto { get; set; }
        public string Descripcion { get; set; }
        public List<MensajeNotificaciones> Mensajes { get; set; }

    }
}