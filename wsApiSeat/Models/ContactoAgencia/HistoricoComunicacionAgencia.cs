using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.ContactoAgencia
{
    public class HistoricoComunicacionAgencia
    {
        public string IdAgencia {get; set;}//FIAPIDCIAU	ID AGENCIA
        public string IdCompra {get; set;}//FIAPIDCOMP	ID COMPRA
        //public string Fecha{get; set;}//FFAPFECHA	FECHA
        //public string Hora {get; set;}//FHAPHORA	HORA							
        public string IdEstadoMovimiento {get; set;}//FIAPIDESTA	ID ESTADO DE MOVIMIENTO
        public string IdCheck { get; set; }//FIAPIDPCKL ID CHECK
        public string DescripcionCheck { get; set; }//FSAPDESCCK    DESCRIPCION DE CHECK
        public string DescripcionEstadoMovimiento {get; set;}//FSAPDESEST	DESCRIPCION ESTADO DE MOVIMIENTO
        public string Mensaje {get; set;}//FSAPNOTIFI	MENSAJE
    }
}