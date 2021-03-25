using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Nissan
{
    public class DatosGeneralesCliente
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public string IdAgencia { get; set; }
        public string Agencia { get; set; }

    }
}