using System;
using System.Collections.Generic;

namespace wsApiSeat.Models
{
    public class PuntoInspeccion
    {
        public string Id { get; set; }
        public string IdInspeccion { get; set; }
        public string Descripcion { get; set; }
        public string ImagenInspeccion { get; set; }
        public string PorcentajeVida { get; set; }
        public string Estado { get; set; }
        public string ImagenEstado { get; set; }
        public List<ImagenesMultimedia> Imagenes { get; set; }
        public List<VideosMultimedia> Videos { get; set; }
    }
}
