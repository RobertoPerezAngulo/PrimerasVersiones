using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class Vehiculo
    {
        private string strId;
        public string Id
        {
            get { return strId; }
            set { strId = value; }
        }

        private string strNumeroSerie;
        public string NumeroSerie
        {
            get { return strNumeroSerie; }
            set { strNumeroSerie = value; }
        }

        private string strNumeroMotor;
        public string NumeroMotor
        {
            get { return strNumeroMotor; }
            set { strNumeroMotor = value; }
        }

        private string strPlacas;
        public string Placas
        {
            get { return strPlacas; }
            set { strPlacas = value; }
        }

        private string strOdometro;
        public string Odometro
        {
            get { return strOdometro; }
            set { strOdometro = value; }
        }

        private string strVersion;
        public string Version
        {
            get { return strVersion; }
            set { strVersion = value; }
        }

        private string strAnio;
        public string Anio
        {
            get { return strAnio; }
            set { strAnio = value; }
        }

        private string strModelo;
        public string Modelo
        {
            get { return strModelo; }
            set { strModelo = value; }
        }

        private string strMarca;
        public string Marca
        {
            get { return strMarca; }
            set { strMarca = value; }
        }

        public string IdColor { get; set; }
        public string IdMarca { get; set; }
        public string IdModelo { get; set; }
        public string Permiso { get; set; }

        private string strColor;
        public string Color
        {
            get { return strColor; }
            set { strColor = value; }
        }

        public string IdCuenta { get; set; }
        public string NumeroPoliza { get; set; }
        public string IdAseguradora { get; set; }
        public string Aseguradora { get; set; }
        public string TelAseguradora { get; set; }
        public string FechaVencimiento { get; set; }
        public string IdEngomado { get; set; }
        public string FechaGarantiaExt { get; set; }
        public string NombreLogo { get; set; }
    }
}