﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace wsApiSeat.Models.OrdenCompra
{
    public class Accesorio
    {

        public string IdAgencia { get; set; }
        public string IdPedido { get; set; }
        public string IdConsecutivo { get; set; }
        public string Concepto { get; set; }
        public string Subtotal { get; set; }
        public string Descuento { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }
        public string RutaFoto { get; set; }

    }
}