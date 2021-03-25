using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class PublicidadNissan
    {

        public string IdAgencia { get; set; }
        public string IdPublicidad { get; set; }
        public string NomPublicidad { get; set; }
        public string DescPublicidad { get; set; }
        public string IniciaPublicidad { get; set; }
        public string TerminaPublicidad { get; set; }
        public RutaTamanioImagen LinkPublicidad { get; set; }
        public string RutaFoto { get; set; }
        public string RutaVideo { get; set; }


    }
}