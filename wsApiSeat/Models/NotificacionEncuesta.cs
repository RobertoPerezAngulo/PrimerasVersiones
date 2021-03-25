using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class NotificacionEncuesta
    {
        public string IdPersona { get; set; }
        public string IdEncuesta { get; set; }
        public string IdAgencia { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
    }
}