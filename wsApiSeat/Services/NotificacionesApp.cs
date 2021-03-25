using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using RestSharp;
using RestSharp.Deserializers;
using Newtonsoft.Json;

namespace wsApiSeat.Services
{
    public static class NotificacionesApp
    {
        public static string EnviarNotificacionAppBajaj(string IdCuenta, string mensaje, string titulo, string sistema)
        {
            String respuesta = "";



            //string applicationID = "AIzaSyBMMJWUMz-vXk88Nb6t9l-6shB--XRMepo";
            string senderId = "";
            string deviceId = "";
            string serverApiKey = "";


            senderId = "569519633473";
            serverApiKey = "AIzaSyCGGfdq9pm-qHlEOnDyQ3befgHqbMwPB9Y";



            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
            strSql += "WHERE FIAPSTATUS = 1 ";
            strSql += "AND FIAPIDCUEN = " + IdCuenta;



            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                deviceId = dt.Rows[0]["FSAPTOKEN"].ToString().Trim();

                try
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = deviceId,
                        notification = new
                        {
                            body = mensaje,
                            title = titulo,
                            icon = "myicon"
                        }
                    };

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    //Response.Write(sResponseFromServer);
                                    respuesta = sResponseFromServer;

                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    respuesta = ex.Message;
                }
            }

            return respuesta;

        }

        public static string EnviarNotificacionApp(string IdCuenta, string mensaje, string titulo, string sistema)
        {
            String respuesta = "";

            //string applicationID = "AIzaSyBMMJWUMz-vXk88Nb6t9l-6shB--XRMepo";
            string senderId = "";
            string deviceId = "";
            string serverApiKey = "";

            senderId = "216835876102";
            serverApiKey = "AIzaSyCICwPS4zZvZP0pkl82fueEAFq8WK-2w4c";

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
            strSql += "WHERE FIAPSTATUS = 1 ";
            strSql += "AND FIAPIDCUEN = " + IdCuenta;
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                deviceId = dt.Rows[0]["FSAPTOKEN"].ToString().Trim();

                try
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";
                    var data = new
                    {
                        to = deviceId,
                        notification = new
                        {
                            body = mensaje,
                            title = titulo,
                            icon = "myicon"
                        }
                    };

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                    tRequest.ContentLength = byteArray.Length;

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);

                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    //Response.Write(sResponseFromServer);
                                    respuesta = sResponseFromServer;

                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    respuesta = ex.Message;
                }
            }

            return respuesta;

        }
        
    }

    public class NotificacionesAsync
    {
        #region Constantes de la clase
        const string llaveAutorizacionNotificaciones = "Key=AAAAE2lxxJI:APA91bH_HW8bSZLgp4JIzVZhGoR6ziMZb-uPAYO6e0zXTt34AuxAp7W9bhdvAY5H03GwPggVNl3AmYPTy0KbMz4wQWyPIPsV7o-awXmhjdrx1NxQJK0b_xLatKpqeBqKBv7HZeZKFVlM";
        #endregion

        string idCuenta;
        string asunto;
        string mensaje;
        string token;

        public NotificacionesAsync(string IdCuenta, string Asunto, string Mensaje)
        {
            idCuenta = IdCuenta;
            asunto = Asunto;
            mensaje = Mensaje;
        }

        public void EnviarNotificacionAppTribuCupra()
        {
            try
            {

                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
                strSql += "WHERE FIAPSTATUS = 1 ";
                strSql += "AND FIAPIDCUEN = " + idCuenta;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    token = dt.Rows[0]["FSAPTOKEN"].ToString().Trim();
                }
                else
                {
                    throw new Exception();
                }

                Alerta alerta = new Alerta();
                alerta.to = token;
                notification notification = new notification();
                notification.title = asunto;
                notification.body = mensaje;
                alerta.notification = notification;
                data data = new data();
                // data.CUPRA = "Prueba";
                data.CUPRA = "";
                alerta.data = data;

                var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", llaveAutorizacionNotificaciones);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class Alerta
    {
        public string to { get; set; }
        public notification notification { get; set; }
        public data data { get; set; }
    }
    public class notification
    {
        public string title { get; set; }
        public string body { get; set; }
    }
    public class data
    {
        public string CUPRA { get; set; }
    }
}