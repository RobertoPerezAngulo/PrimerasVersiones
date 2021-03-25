using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class PromocionNissan
    {
        private string strIdAgencia;
        public string IdAgencia
        {
            get { return strIdAgencia; }
            set { strIdAgencia = value; }
        }

        private string strId;
        public string Id
        {
            get { return strId; }
            set { strId = value; }
        }

        public string IdTipo { get; set; }
        public string Tipo { get; set; }

        private string strNombrePromocion;
        public string NombrePromocion
        {
            get { return strNombrePromocion; }
            set { strNombrePromocion = value; }
        }

        private string strDescripcionPromocion;
        public string DescripcionPromocion
        {
            get { return strDescripcionPromocion; }
            set { strDescripcionPromocion = value; }
        }

        private string strFechaInicio;
        public string FechaInicio
        {
            get { return strFechaInicio; }
            set { strFechaInicio = value; }
        }

        private string strFechaFin;
        public string FechaFin
        {
            get { return strFechaFin; }
            set { strFechaFin = value; }
        }

        private RutaTamanioImagen strRutaImagen;
        public RutaTamanioImagen RutaImagen
        {
            get { return strRutaImagen; }
            set { strRutaImagen = value; }
        }


        private string strRutaFoto;
        public string RutaFoto
        {
            get { return strRutaFoto; }
            set { strRutaFoto = value; }
        }


        private string strRutaVideo;
        public string RutaVideo
        {
            get { return strRutaVideo; }
            set { strRutaVideo = value; }
        }

    }
}