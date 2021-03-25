using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class HorariosCitas
    {
        public string TipoDeCita { get; set; }
        //public List<CitasExistentes> CitasAgendadas { get; set; }
        //public List<string> FechasPeriodo { get; set; }
        //public List<Rangos> Horarios { get; set; }
        public List<Fechas> Fechas { get; set; }
        public List<Ubicaciones> Ubicaciones { get; set; }
    }
    public class Rangos
    {
        public string Dia { get; set; }
        public string Horario { get; set; }
        public List<Intervalos> Intervalos { get; set; }
    }
    public class Intervalos
    {
        public string Dia { get; set; }
        public string IdTurno { get; set; }
        public string Intervalo { get; set; }
    }
    public class CitasExistentes
    {
        public string TipoCita { get; set; }
        public string Intervalo { get; set; }
        public string Fecha { get; set; }
        public string Turno { get; set; }

    }

    public class Fechas
    {
        public string Fecha { get; set; }
        public string Dia { get; set; }
        //public string Horario { get; set; }
        public List<Horarios> Horarios { get; set; }
        public List<Intervalos> Intervalos { get; set; }
    }

    public class Horarios
    {
        public string Hora { get; set; }
        public string Dia { get; set; }
        public string IdTurno { get; set; }

    }

}