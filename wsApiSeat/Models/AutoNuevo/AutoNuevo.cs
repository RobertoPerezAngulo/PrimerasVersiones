using DVAModelsReflection.Models.TESO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiSeat.Models;

namespace wsApiSeat.Models
{
    public class AutoNuevo
    {
        public AutoNuevo()
        {
            Modelos = new List<Modelos>();
        }
        public string IdMarca { get; set; }
        public string Nombre { get; set; }
        public string UrlMarca { get; set; }
        public string Armadora { get; set; }
        public string UrlArmadora { get; set; }

        public List<Modelos> Modelos { get; set; }
    }

    public class Modelos {
        public Modelos()
        {
            Anios = new List<Anios>();
        }
        public string IdModelo { get; set; }
        public string Nombre { get; set; }
        public string UrlModelo { get; set; }
        public List<Anios> Anios { get; set; }

    }

    public class Anios {

        public Anios()
        {
            Versiones = new List<Versiones>();

        }
        public string Numero { get; set; }
        public List<Versiones> Versiones { get; set; }
    }

    public class Versiones {

        public Versiones()
        {
            Equipamiento = new List<Equipamiento>();
            Caracteristicas = new List<Caracteristicas>();
            Generales = new List<Generales>();
            Fotos = new Fotos();
            
        }
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
        public string CodigoManufactura { get; set; }

        public List<Equipamiento> Equipamiento { get; set; }
        public List<Caracteristicas> Caracteristicas { get; set; }
        public List<Generales> Generales { get; set; }
        public Fotos Fotos { get; set; }
     
        
        
    }

    public class Equipamiento
    {
        public Equipamiento()
        {
            Atributos = new List<AtributoEquip>();
        }
        public string IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public string Nombre { get; set; }
        public string Disponible { get; set; }
        public string Valor { get; set; }

        public List<AtributoEquip> Atributos { get; set; }

    }

    public class AtributoEquip {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }

    public class Caracteristicas
    {
        public string IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

    }

    public class Generales {
        public string IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
    }

    public class Fotos {
        public Fotos()
        {
            Exteriores = new List<CaracteristicasFotos>();
            Interiores = new List<CaracteristicasFotos>();
        }


        public List<CaracteristicasFotos> Exteriores { get; set; }
        public List<CaracteristicasFotos> Interiores { get; set; }
    }

    public class CaracteristicasFotos {

        public CaracteristicasFotos()
        {
            Rutas = new List<RutaP>();
        }
        public string Vista { get; set; }
        public List<RutaP> Rutas { get; set; }

    }
    public class RutaP {
        public string Ruta { get; set; }
    }
}
