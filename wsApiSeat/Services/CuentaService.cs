using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using wsApiSeat.Services;
using wsApiSeat.Models;
using System.Net;
using System.IO;

namespace wsApiSeat.Services
{
    public class CuentaService
    {
        internal Respuesta RegistraCuenta(CuentaService cuentaJson) {

            Respuesta respuesta = new Respuesta();
            return respuesta;
        }

        public static string GenerarClaveActivacion()
        {
            string clave = "";
            string[] caracteres = new string[] { "a", "b", "c", "d", "e", "f", "g", "h",
            "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "A", "B", "C", "D", "E", "F",
            "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            Random ran = new Random();
            while (clave.Length != 8)
            {
                int aleatorio = ran.Next(61);
                clave += caracteres[aleatorio];
            }
            return clave;
        }

        public static string ObtenerStrHtmlClaveActivacion(string clave)
        {
            string strHtml = "";

            try
            {
                string rutaArchivo = obtenerURLServidor() + "Resources/" + "claveActivacionHtml.html";
                strHtml = leerArchivoWeb(rutaArchivo);

                strHtml = strHtml.Replace("[{Clave}]", clave);
            }
            catch (Exception ex)
            {

            }

            return strHtml;
        }

        public static string ObtenerStrHtmlAutoMeInteresa(AutoUsado autoJson)
        {
            string strHtml = "";

            try
            {
                string rutaArchivo = obtenerURLServidor() + "Resources/" + "autoMeInteresaHtml.html";
                strHtml = leerArchivoWeb(rutaArchivo);

                strHtml = strHtml.Replace("[{Marca}]", autoJson.Marca);
                strHtml = strHtml.Replace("[{Modelo}]", autoJson.Modelo);
                strHtml = strHtml.Replace("[{Version}]", autoJson.Version);
                strHtml = strHtml.Replace("[{Anio}]", autoJson.Anio);
                strHtml = strHtml.Replace("[{Color}]", autoJson.Color);
                strHtml = strHtml.Replace("[{Km}]", autoJson.Kilometraje);
            }
            catch (Exception ex) { }

            return strHtml;
        }

        public static string ObtenerStrHtmlAutoMeInteresaMiAuto(Cuenta cuentaModel, AutoUsado autoJson)
        {
            string strHtml = "";

            try
            {
                string rutaArchivo = obtenerURLServidor() + "Resources/" + "autoMeInteresaNissanHtml.html";
                strHtml = leerArchivoWeb(rutaArchivo);

                strHtml = strHtml.Replace("[{Cuenta}]", cuentaModel.IdCuenta);
                strHtml = strHtml.Replace("[{Nombre}]", cuentaModel.Nombre.Trim() + " " + cuentaModel.ApellidoPaterno.Trim());
                strHtml = strHtml.Replace("[{Telefono}]", "-");
                strHtml = strHtml.Replace("[{Correo}]", cuentaModel.Correo.Trim());

                strHtml = strHtml.Replace("[{Marca}]", autoJson.Marca);
                strHtml = strHtml.Replace("[{Modelo}]", autoJson.Modelo);
                strHtml = strHtml.Replace("[{Version}]", autoJson.Version);
                strHtml = strHtml.Replace("[{Anio}]", autoJson.Anio);
                strHtml = strHtml.Replace("[{Color}]", autoJson.Color);
                strHtml = strHtml.Replace("[{Km}]", autoJson.Kilometraje);
            }
            catch (Exception ex) { }

            return strHtml;
        }

        public static string obtenerURLServidor()
        {
            HttpRequest request = HttpContext.Current.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";

            return baseUrl;
        }

        public static string leerArchivoWeb(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string output = reader.ReadToEnd();
            response.Close();

            return output;
        }


    }
}