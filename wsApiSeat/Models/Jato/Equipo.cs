using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiSeat.Models.Jato;

namespace wsApiSeat.Models.Jato
{
    public class Equipo
    {
        public string IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public string Nombre { get; set; }
        public string Disponible { get; set; }
        public string Valor { get; set; }
        public List<Atributo> Atributos { get; set; }
    }
}