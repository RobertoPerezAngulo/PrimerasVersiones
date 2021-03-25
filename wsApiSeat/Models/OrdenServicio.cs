using System;
namespace wsApiSeat.Models
{
    public class OrdenServicio
    {
        private string strIdEmpresa;
        public string IdEmpresa
        {
            get { return strIdEmpresa; }
            set { strIdEmpresa = value; }
        }

        private string strIdPreOrden;
        public string IdPreOrden
        {
            get { return strIdPreOrden; }
            set { strIdPreOrden = value; }
        }

        private string strOdometro;
        public string Odometro
        {
            get { return strOdometro; }
            set { strOdometro = value; }
        }

        private string strTipoOrden;
        public string TipoOrden
        {
            get { return strTipoOrden; }
            set { strTipoOrden = value; }
        }

        private string strNumeroOrden;
        public string NumeroOrden
        {
            get { return strNumeroOrden; }
            set { strNumeroOrden = value; }
        }

        private string strFechaPromesa;
        public string FechaPromesa
        {
            get { return strFechaPromesa; }
            set { strFechaPromesa = value; }
        }

        private string strHoraPromesa;
        public string HoraPromesa
        {
            get { return strHoraPromesa; }
            set { strHoraPromesa = value; }
        }

        private string strFechaIngreso;
        public string FechaIngreso
        {
            get { return strFechaIngreso; }
            set { strFechaIngreso = value; }
        }

        private string strHoraIngreso;
        public string HoraIngreso
        {
            get { return strHoraIngreso; }
            set { strHoraIngreso = value; }
        }

        private string strEstado;
        public string Estado
        {
            get { return strEstado; }
            set { strEstado = value; }
        }

        private string strAsesor;
        public string Asesor
        {
            get { return strAsesor; }
            set { strAsesor = value; }
        }

        private string strPlacas;
        public string Placas
        {
            get { return strPlacas; }
            set { strPlacas = value; }
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

        private string strEmpresa;
        public string Empresa
        {
            get { return strEmpresa; }
            set { strEmpresa = value; }
        }

        public string Refacciones { get; set; }
        public string Paquetes { get; set; }
        public string OtrosTrabajos { get; set; }
        public string Operaciones { get; set; }
        public string Cargos { get; set; }
        public string Descuentos { get; set; }
        public string Subtotal { get; set; }
        public string Iva { get; set; }
        public string Total { get; set; }

    }
}
