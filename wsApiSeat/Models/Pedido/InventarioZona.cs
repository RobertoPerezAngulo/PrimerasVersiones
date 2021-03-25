using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Pedido
{
    public class InventarioZona
    {
        public string IdZona { get; set; }
        public string NombreZona { get; set; }
        public List<AgenciasPorZona> Agencias { get; set; }       

    }


    
    public class AgenciasPorZona {
        public string IdAgencia { get; set;}
        public string Nombre { get; set; }

    }


    public class Zona {

        public string IdZona { get; set; }
        public string NombreZona { get; set; }

    }
    

}