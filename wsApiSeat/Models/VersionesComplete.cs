using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
  
      public class Color
    {
        public string IdColor { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string RutaMini { get; set; }
        public string RutaLocalMini { get; set; }
        public string RutaLocal { get; set; }
    }
    public class ModelosCarros
    {
        public string IdModelo { get; set; }
        public string NombreModelo { get; set; }
        public string AnioModelo { get; set; }
        public string Motor { get; set; }
        public string Potencia { get; set; }
        public string Aceleracion { get; set; }
        public string VelocidadMax { get; set; }
        public List<Version> Versiones { get; set; }
        public List<Color> GamaColores { get; set; }
        public List<Consumo> ConsumoGasolina { get; set; }
        public List<TransmicionCarro> Transmision { get; set; }
    }

    public class Version
    {
        public string Nombre { get; set; }
        public string RutaVideo { get; set; }
        public string RutaFichaTecnica { get; set; }
        public string Ruta360 { get; set; }
        public List<PrecioVenta> PrecioTransmision { get; set; }
        public List<Color> Colores { get; set; }
    }

    public class PrecioVenta
    {
        public string IdVersion { get; set; }
        public string Transmision { get; set; }
        public string Precio { get; set; }
        public string Iva { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
    }

    public class Consumo
    {
        public string Tipo { get; set; }
    }

    public class TransmicionCarro
    {
        public string Tipo { get; set; }
    }

}