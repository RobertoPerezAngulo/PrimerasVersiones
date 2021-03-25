using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class DatosAsesor
    {
        public string IdUsuario { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public List<IdsAsesor> Asesor { get; set; }
    }
}