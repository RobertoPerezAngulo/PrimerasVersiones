using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class CuentasBancariasPorAgencia
    {
        public string RazonSocial { get; set; }
        public string Ubicacion { get; set; }
        public List<CuentaBancaria> CuentasBancarias { get; set; }   


    }
}