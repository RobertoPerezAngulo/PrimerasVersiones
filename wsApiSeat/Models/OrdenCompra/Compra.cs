using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class Compra
    {


        public string IdCompra { get; set; }
        public string FolioCompra { get; set; }
        public string IdCuenta { get; set; }
        public string FechaCompra { get; set; }
        public string HoraCompra { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string IVA { get; set; }
        public string Total { get; set; }
        public string IdEstado { get; set; }
        public string DescripcionEstado { get; set; }

        public string IdProceso { get; set; }
        public string IdPaso { get; set; }
        
        public string RutaReferenciaBancaria { get; set; }
        public Cuenta CuentaUsuario { get; set; }
        public PedidoVehiculo Pedido { get; set; }

        public List<Accesorio> Accesorios { get; set; }

    }
}