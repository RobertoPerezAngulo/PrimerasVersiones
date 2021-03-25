using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class ClasificacionDocumento
    {
        public string IdTipoPersona { get; set; }  // FIAPIDTPPR, FSAPTIPPER, FIAPIDTIPO, FSAPDOCUME   FROM
        public string TipoPersona { get; set; }
        public string IdTipoDocumento { get; set; }
        public string Documento { get; set; }
        
    }
}