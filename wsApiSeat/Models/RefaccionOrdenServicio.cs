using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class RefaccionOrdenServicio
    {
        public string IdAgencia { get; set; }
        public string IdPreorden { get; set; }
        public string NumeroParte { get; set; }
        public string DescRefaccion { get; set; }
        public string Cantidad { get; set; }
        public string PrecioVenta { get; set; }
        public string PrecioTotal { get; set; }
    }
}