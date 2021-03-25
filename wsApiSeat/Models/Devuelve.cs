using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class Devuelve
    {
        public List<ImagenesMultimedia> Imagenes { get; set; }
        public List<VideosMultimedia> Videos { get; set; }
    }
}