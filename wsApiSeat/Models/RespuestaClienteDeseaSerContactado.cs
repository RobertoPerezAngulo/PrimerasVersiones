using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class RespuestaClienteDeseaSerContactado
    {
        public string Respuesta { get; set; }
        public WsEncuestasPSI.Mensaje Mensaje { get; set; }
    }
}