using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class RespuestaGuardarEncuestaPSI
    {
        public string Respuesta { get; set; }
        public WsEncuestasPSI.MensajeAgradecimiento Encuesta { get; set; }
    }
}