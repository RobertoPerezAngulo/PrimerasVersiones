using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Pedido
{
    public class DatosFiscales
    {
            public string IdCompra {get;set;} //FIAPIDCOMP, 
            public string RfcFisica {get;set;}//FSAPRFCFIS,
            public string NombreFisica {get;set;} //FSAPNMBFIS, 
            public string ApellidoPaternoFisica {get;set;}//FSAPAPTFIS,
            public string ApellidoMaternoFisica {get;set;}//FSAPAMTFIS,
            public string LadaFisica {get;set;}//FIAPLDTFIS,
            public string NumeroTelefonoFisica{get;set;}//FIAPNMTFIS,
            public string CorreoFisica{get;set;}//FSAPCRRFIS,
            public string RfcRazonSocial {get;set;}//FSAPRFCRSC,
            public string RazonSocial{get;set; }//FSAPRAZSOC,
            public string RfcRepresentanteLegal { get; set; }//FSAPRFCRSC,
            public string NombreRepresentanteLegal{get;set;}//FSAPNMBRLG, 
            public string ApellidoPaternoRepresentanteLegal{get;set;}//FSAPAPTRLG,
            public string ApellidoMaternoRepresentantelegal{get;set;}//FSAPAMTRLG,
            public string LadaRepresentantelegal { get;set;}//FIAPLDTRLG, 
            public string NumeroTelefonoRepresentanteLegal { get; set; }//FIAPNMTRLG
            public string CorreoRepresentantelegal { get;set;}//FSAPCRRRLG
            public string ClaveUsoCfdi { get; set; }// FSAPCUCFDI
            public string DescripcionUsoCfdi { get; set; }// FSAPDESCRI



    }
}