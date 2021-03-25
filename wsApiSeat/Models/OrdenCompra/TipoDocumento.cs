using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace wsApiSeat.Models.OrdenCompra
{
    public class TipoDocumento
    {
       public List<ClasificacionDocumento> PersonaFisica { get; set; }
       public List<ClasificacionDocumento> PersonaMoral { get; set; }
        
    }
}