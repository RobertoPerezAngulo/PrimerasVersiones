using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class Documento
    {
        public string IdCompra { get; set; }
        public string IdTipo { get; set; }
        public string NombreDocumento { get; set; }
        public string RutaDocumento { get; set; }

    }
}