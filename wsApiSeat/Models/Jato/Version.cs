using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace wsApiSeat.Models.Jato
{
    public class Version
    {
        public string IdVehiculo { get; set; }
        public string NombreCorto { get; set; }
        public string Nombre { get; set; }
        public string NumeroPuertas { get; set; }
        public string RuedasMotriz { get; set; }
        public string Transmision { get; set; }
        public string Precio { get; set; }
        public string RutaFoto { get; set; }
        public string Estilo { get; set; }
        public string Descripcion { get; set; }
        public string ConsumoGasolinaCiudad { get; set; }
        public string ConsumoGasolinaCarretera { get; set; }
        public string CodigoManufactura { get; set; }
        public FotosJato Fotos { get; set; }
        public List<Models.Jato.General> Generales { get; set; }
       public List<Models.Jato.Caracteristica> Caracteristicas { get; set; }
        public List<Models.Jato.Equipo> Equipamiento { get; set; }

    }
}