using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class MediaProc2
    {
        public string Seccion { get; set; }
        public List<FotosMedia> Fotos { get; set; }
        public List<VideoMedia> Videos { get; set; }
    }
    public class FotosMediaProc2
    {
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
        public string RutaLocalMini { get; set; }
        public string RutaLocal { get; set; }
    }

    public class VideoMediaProc2
    {
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
        public string RutaLocalMini { get; set; }
        public string RutaLocal { get; set; }
    }
}