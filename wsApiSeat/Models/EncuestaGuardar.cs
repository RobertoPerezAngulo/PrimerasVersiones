using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class EncuestaGuardar
    {
        public string IdArea { get; set; }
        public string IdCompania { get; set; }
        public string IdEncuesta { get; set; }
        public string IdPlantilla { get; set; }
        public List<RespuestaEncuestaPSI> Respuestas { get; set; }
        public string Comentarios { get; set; }
    }
}