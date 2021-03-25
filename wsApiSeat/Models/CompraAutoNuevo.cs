using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class CompraAutoNuevo
    {
        public string IdCompra{ get; set; }
        public string IdPaso { get; set; }
        public string Folio { get; set; }
        public string IdCuenta { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string IdEstado { get; set; }
        public string RutaReferenciaBancaria { get; set; }

        public PedidoAutoNuevo Pedido { get; set; }
        public Cuenta CuentaUsuario { get; set; }

        public List<AccesoriosUOtros> AccesoriosOtros { get; set; }


    }
}