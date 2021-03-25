using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class CitaApp
    {
        public string IdTipoCita { get; set; }
        public string IdCompra { get; set; }
        public string IdTurno { get; set; }
        public string DescripcionCita { get; set; }
        public string Fecha { get; set; }
        public string HoraInicial { get; set; }
        public string HoraFinal { get; set; }
        public string Ubicacion { get; set; }
    }


    public class CitaAppGenerica
    {
        public string IdTipoCita { get; set; }
        public string IdTurnoMatVes { get; set; }
        public string IdCompra { get; set; }                     
        public string Fecha { get; set; }
        public string HoraInicial { get; set; }
        public string HoraFinal { get; set; }
        public string Ubicacion { get; set; }
    }

}


