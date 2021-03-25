using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class SubirArchivo
    {
            public string IdCompra {get; set;} //IdCompra, 
            public string IdConsecutivo { get; set; }
            public string TipoDocumento {get; set;} //TipoDocumento, s
            public string NombreDocumento {get; set;} //Documento, 
            public string Base64 {get; set;} //base64            
            public string ExtensionArch { get; set; }

    }

    public class SubirArchivoSmartIT
    {
        public string IdCompra { get; set; }
        public string IdConsecutivo { get; set; }
        public string IdCuenta { get; set; }
        public string NombreAseguradora { get; set; }
        public string Total { get; set; }
        public string Cobertura { get; set; }
        public string Tipo { get; set; }
        public string NombreDocumento { get; set; } 
        public string Base64 { get; set; }            
        public string ExtensionArch { get; set; }

    }

    public class SeleccionaSeguroCliente
    {
        public string IdConsecutivo { get; set; }
        public string IdCompra { get; set; }
        public string IdCuenta { get; set; }
        public string Tipo { get; set; }
    }

    public class SegurosCliente
    {
        public string IdConsecutivo { get; set; }
        public string IdCompra { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string Cobertura { get; set; }
        public string Cantidad { get; set; }
        public string Tipo { get; set; }
        public string NombreDocumento { get; set; }
        public string Ruta { get; set; }
    }

    public class SegurosClienteSmartIt
    {
        public string IdConsecutivo { get; set; }
        public string IdCompra { get; set; }
        public string IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string Cobertura { get; set; }
        public string Cantidad { get; set; }
        public string Tipo { get; set; }
        public string NombreDocumento { get; set; }
        public string Ruta { get; set; }
        public string selecciono { get; set; }
    }

}