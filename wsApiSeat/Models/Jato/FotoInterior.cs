﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Jato
{
    public class FotoInterior
    {
        public string Vista { get; set; }
        public List<Models.Jato.RutaFoto> Rutas { get; set; }
    }
}