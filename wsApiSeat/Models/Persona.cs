using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models
{
    public class Persona
    {

        private string strId;
        public string IdPersona
        {
            get { return strId; }
            set { strId = value; }
        }
        public string IdCuenta { get; set; }
        private string strRFC;
        public string RFC
        {
            get { return strRFC; }
            set { strRFC = value; }
        }
        private string strTipo;
        public string TipoPersona
        {
            get { return strTipo; }
            set { strTipo = value; }
        }
        private string strNombre;
        public string NombrePersona
        {
            get { return strNombre; }
            set { strNombre = value; }
        }
        public string LadaPersona { get; set; }
        private string strTelefono;
        public string TelefonoPersona
        {
            get { return strTelefono; }
            set { strTelefono = value; }
        }
        private string strCorreo;
        public string CorreoPersona
        {
            get { return strCorreo; }
            set { strCorreo = value; }
        }
        public string CallePersona { get; set; }
        public string NumExtPersona { get; set; }
        public string NumIntPersona { get; set; }
        public string ColoniaPersona { get; set; }
        public string CodigoPostalPersona { get; set; }
        public string DelegacionPersona { get; set; }
       
        public string RazonSocialFactura { get; set; }
        public string CalleFactura { get; set; }
        public string NumExtFactura { get; set; }
        public string NumIntFactura { get; set; }
        public string ColoniaFactura { get; set; }
        public string CodigoPostalFactura { get; set; }
        public string DelegacionFactura { get; set; }
        public string EstadoFactura { get; set; }
        public string Clave { get; set; }

        private string strApellidoPaterno;
        public string ApellidoPaterno
        {
            get { return strApellidoPaterno; }
            set { strApellidoPaterno = value; }
        }

        private string strApellidoMaterno;
        public string ApellidoMaterno
        {
            get { return strApellidoMaterno; }
            set { strApellidoMaterno = value; }
        }
    }
}