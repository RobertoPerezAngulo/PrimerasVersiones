using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Jato
{
    public class MarcaJato
    {
        public string IdMarca { get; set; }
        public string Nombre { get; set; }
        public string UrlMarca { get; set; }
        public string Armadora { get; set; }
        public string UrlArmadora { get; set; }
        public List<ModeloJato> Modelos { get; set; }

    }
}