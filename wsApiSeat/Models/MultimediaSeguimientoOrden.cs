using System;
using System.Collections.Generic;

namespace wsApiSeat.Models
{

    public class MultimediaSeguimientoOrden
    {
        public string IdSegOrden { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public List<ImagenSeguimientoOrden> Imagenes { get; set; }
        public List<VideoSeguimientoOrden> Videos { get; set; }

    }
}
