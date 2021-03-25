using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class CitaServicioAgendada
    {
        public string IdAgencia { get; set; }
        public string Agencia { get; set; }
        public string IdCita { get; set; }
        public string FolioCita { get; set; }
        public string Asunto { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string ModeloVersion { get; set; }
    }
}