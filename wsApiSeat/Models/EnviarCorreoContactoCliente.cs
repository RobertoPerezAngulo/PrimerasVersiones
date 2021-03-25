using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class EnviarCorreoContactoCliente
    {

        public string CorreoContacto { get; set; }
        public string CorreoCliente { get; set; }
        public string NombreCliente { get; set; }        
        public string Texto { get; set; }
        
    }
}