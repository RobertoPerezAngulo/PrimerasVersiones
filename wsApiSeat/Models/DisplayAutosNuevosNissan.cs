using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class DisplayAutosNuevosNissan
    {

            public string IdAgencia {get; set;}   //FIAPIDCIAU, 
           // public string IdMarca {get; set;}   //FIAPIDMARC,
            public string IdPublicidad {get; set;}   //FIAPIDPUBL,
            public string NomPublicidad { get; set;}   //FSAPPUBLIC,
            public string DescPublicidad { get; set;}   //FSAPDESPUB,
            public string IniciaPublicidad { get; set;}   //FFAPINIPUB,
            public string TerminaPublicidad { get; set;}   //FFAPFINPUB,
            public RutaTamanioImagen LinkPublicidad {get; set;}   //FSAPLINPUB,
            public string  RutaFoto {get; set;}   //FSAPRUTFOT,
            public string  RutaVideo {get; set;}   //FSAPRUTVID,
           // public string  Restringido {get; set;}   //FIAPRESTRI,
            


    }
}