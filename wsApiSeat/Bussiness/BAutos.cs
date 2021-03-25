using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using wsApiSeat.Models;

namespace wsApiSeat.Bussiness
{
    public class BAutos
    {

        private string obtenerURLServidor()
        {
            HttpRequest request = HttpContext.Current.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";

            return baseUrl;
        }

        private string obtenerURLInternaServidor()
        {
            HttpRequest request = HttpContext.Current.Request;
            string baseUrl = "C:\\inetpub\\wwwroot\\wsApiSeat\\";

            return baseUrl;
        }

        private string leerArchivoWeb(string url)
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

        private string leerArchivoLocal(string url)
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

        //internal List<VersionesComplete> ObtenerVersiones()
        //{
        //    List<VersionesComplete> lstVersion = new List<VersionesComplete>();

        //    try
        //    {
        //        string rutaArchivo = obtenerURLServidor() + "ResourcesJson/" + "Versiones.json";
        //        string strJSON = leerArchivoWeb(rutaArchivo);
        //        lstVersion = JsonConvert.DeserializeObject<List<VersionesComplete>>(strJSON);

        //    }
        //    catch (Exception _exc)
        //    {
        //        lstVersion = new List<VersionesComplete>();
        //        lstVersion.Add(new VersionesComplete());
        //    }
        //    return lstVersion;


        //}

        //internal List<Modelo> ObtenerVersiones()
        //{
        //    List<Modelo> lstVersion = new List<Modelo>();

        //    try
        //    {
        //        string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Versiones.json");
        //        lstVersion = JsonConvert.DeserializeObject<List<Modelo>>(strJSON);

        //    }
        //    catch (Exception _exc)
        //    {
        //        lstVersion = new List<Modelo>();
        //        lstVersion.Add(new Modelo());
        //    }
        //    return lstVersion;


        //}


        //respaldo original (06/10/2020) este metodo si funciona pero no ordena los colores
        //internal List<VersionesComplete> ObtenerVersionesProc2()
        //{
        //    List<VersionesComplete> lstVersion = new List<VersionesComplete>();

        //    try
        //    {

        //        //string rutaArchivo = "c:\\inetpub\\wwwroot\\wsApiSeat\\ResourcesJson\\Versiones.json";
        //        string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\VersionesProc2.json");
        //        lstVersion = JsonConvert.DeserializeObject<List<VersionesComplete>>(strJSON);

        //    }
        //    catch (Exception _exc)
        //    {
        //        lstVersion = new List<VersionesComplete>();
        //        lstVersion.Add(new VersionesComplete());
        //    }
        //    return lstVersion;


        //}


        internal List<ModelosCarros> GetObtenerModelos()
        {
            List<ModelosCarros> respuesta = new List<ModelosCarros>();
            try
            {
                string strJSON = File.ReadAllText(@"C:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Versiones.json");
                //string strJSON = File.ReadAllText(@"C:\Compartida\Roberto Perez\Migracion de servicios\Servicio Seat\wsApiSeat\ResourcesJson\Versiones.json");
                respuesta = JsonConvert.DeserializeObject<List<ModelosCarros>>(strJSON);
                
            }
            catch (Exception)
            {
                respuesta = null;
            }
            return respuesta;

        }





        //internal Media ObtenerMedia()
        //{
        //    Media media = new Media();

        //    try
        //    {
        //        string rutaArchivo = //obtenerURLServidor() + "ResourcesJson/" + "Media.json";
        //        string strJSON = leerArchivoWeb(rutaArchivo);
        //        media = JsonConvert.DeserializeObject<Media>(strJSON);
        //    }
        //    catch (Exception _exc)
        //    {
        //        media = new Media();
        //    }

        //    return media;
        //}
        

        internal List<MediaProc2> ObtenerMedia()
        {
            List<MediaProc2> lstMedia = new List<MediaProc2>();

            try
            {
                //string rutaArchivo = "c:\\inetpub\\wwwroot\\wsApiSeat\\ResourcesJson\\Media.json";
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Media.json");
                lstMedia = JsonConvert.DeserializeObject<List<MediaProc2>>(strJSON);
            }
            catch (Exception _exc)
            {
                lstMedia = new List<MediaProc2>();
            }

            return lstMedia;
        }

    }
}