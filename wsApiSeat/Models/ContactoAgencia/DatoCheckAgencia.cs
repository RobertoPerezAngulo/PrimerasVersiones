using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.ContactoAgencia
{
    public class DatoCheckAgencia
    {

        public string IdAgencia { get; set; } //FIAPIDCIAU ID AGENCIA
        public string IdCompra { get; set; } //FIAPIDCOMP  ID COMPRA
        public string IdCheck { get; set; } //FIAPIDPCKL ID CHECK
        public string DescripcionCheck { get; set; } //FSAPDESCCK  DESCRIPCION PASO { PRODAPPS.APCESSST }
        public string DescripcionEstado { get; set; } //FSAPDESEST DESCRIPCION ESTADO
        public string IdEstado { get; set; } //FIAPIDESTA  IDESTADO {con este se jala la descripcion}

    }
}