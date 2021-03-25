using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class Media
    {
        public List<FotosMedia> Fotos { get; set; }
        public List<VideoMedia> Videos { get; set; }
    }
    public class FotosMedia
    {
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
        public string RutaLocalMini { get; set; }
        public string RutaLocal { get; set; }
    }

    public class VideoMedia
    {
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
        public string RutaLocalMini { get; set; }
        public string RutaLocal { get; set; }
    }
}