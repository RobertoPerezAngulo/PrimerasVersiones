using System;
using System.Collections.Generic;
using wsApiSeat.Models.Jato;

namespace wsApiSeat.Models.Jato
{
    public class ModeloJato
    {


        public string IdModelo { get; set; }
        public string Nombre { get; set; }
        public string UrlModelo { get; set; }
        public string RutaFoto { get; set; }
        public List<Estilo> Estilos { get; set; }
        public List<Models.Jato.Anio> Anios { get; set; }

    }
}