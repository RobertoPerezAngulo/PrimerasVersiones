using System;
namespace wsApiSeat.Models
{
    public class OperacionOrdenServicio
    {
        public string IdAgencia { get; set; }
        public string IdPreorden { get; set; }
        public string IdOperacion { get; set; }
        public string DescOperacion { get; set; }
        public string NumUnidades { get; set; }
        public string PrecioVentaPorUnidades { get; set; }
        public string PrecioTotal { get; set; }
    }
}
