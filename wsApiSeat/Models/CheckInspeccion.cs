using System;
using System.Collections.Generic;

namespace wsApiSeat.Models
{
    public class CheckInspeccion
    {
        public string IdCompania { get; set; }
        public string IdPreOrden { get; set; }
        public string Id { get; set; }
        public string Observaciones { get; set; }
        public List<PuntoInspeccion> PuntosInspeccion { get; set; }
    }
}
