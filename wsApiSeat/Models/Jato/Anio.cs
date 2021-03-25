using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace wsApiSeat.Models.Jato
{
    public class Anio
    {
        public string Numero { get; set; }
       public List<Models.Jato.Version> Versiones { get; set; }
    }
}