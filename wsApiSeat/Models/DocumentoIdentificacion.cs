using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class DocumentoIdentificacion
    {
        
      public string IdTipoPersona { get; set; }
      public string DescripcionTipoPersona { get; set; }

     public List<Nacionalidades> Nacionalidades { get; set;}

    }


    public class Nacionalidades {

        public string Nacionalidad { get; set; }
        public List<DocumentacionRequerida> DocumentacionRequerida { get; set; }
    }


    public class DocumentacionRequerida
    {
         public string IdTipo { get; set; }
         public string DescripcionDocumento { get; set; }
         public string Obligatorio { get; set; }
    }


}