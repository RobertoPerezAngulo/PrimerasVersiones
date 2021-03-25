using System;
namespace wsApiSeat.Models
{
    public class EncuestaPSI
    {

        public string Respuesta { get; set; }
        public string Guirnalda { get; set; }
        public WsEncuestasPSI.Encuesta Encuesta { get; set; }

    }
}
