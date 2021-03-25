using System;
namespace wsApiSeat.Models
{
    public class HorarioServicio
    {
        public string IdAgencia { get; set; }
        public string IdAsesor { get; set; }
        public string NombreAsesor { get; set; }
        public string ApellidoPaternoAsesor { get; set; }
        public string ApellidoMaternoAsesor { get; set; }
        public string IdDia { get; set; }
        public string Rango { get; set; }
        public string HoraInicial { get; set; }
        public string HoraFinal { get; set; }
    }
}
