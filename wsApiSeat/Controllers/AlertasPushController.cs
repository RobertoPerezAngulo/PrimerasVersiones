using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestSharp;
using Newtonsoft.Json;
using wsApiSeat.Models;
using wsApiSeat.Models.OrdenCompra;
using System.Data;
using wsApiSeat.Models.Notificaciones;
using wsApiSeat.Services;

using RouteAttribute = System.Web.Http.RouteAttribute;

namespace wsApiSeat.Controllers
{
    public class AlertasPushController : ApiController
    {
        #region Constantes de la clase
        const string llaveAutorizacionNotificaciones = "Key=AAAAE2lxxJI:APA91bH_HW8bSZLgp4JIzVZhGoR6ziMZb-uPAYO6e0zXTt34AuxAp7W9bhdvAY5H03GwPggVNl3AmYPTy0KbMz4wQWyPIPsV7o-awXmhjdrx1NxQJK0b_xLatKpqeBqKBv7HZeZKFVlM";
        #endregion

        [Route("api/AlertasPush/GetEnviar", Name = "GetEnviar")]
        public void GetEnviar(string token, string asunto, string mensaje)
        {
            Alerta alerta = new Alerta();
            alerta.to = token;
            notification notification = new notification();
            notification.title = asunto;
            notification.body = mensaje;
            notification.badge = "1";
            alerta.notification = notification;
            data data = new data();
            data.Seat = "Prueba";
            alerta.data = data;
            Console.WriteLine(alerta);

            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", llaveAutorizacionNotificaciones);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
        
        [Route("api/AlertasPush/GetEnviarConGlobo", Name = "GetEnviarConGlobo")]
        public void GetEnviarConGlobo(string token, string asunto, string mensaje)
        {
            
            Alerta alerta = new Alerta();
            alerta.to = token;
            notification notification = new notification();
            notification.title = asunto;
            notification.body = mensaje;
            
            
            notification.badge = "1";
            alerta.notification = notification;
            data data = new data();
            data.Seat = "Prueba";
            alerta.data = data;
            

            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", llaveAutorizacionNotificaciones);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [Route("api/AlertasPush/PostEnviaNotificacionTribuCupra", Name = "PostEnviaNotificacionTribuCupra")]
        public Respuesta PostEnviaNotificacion([FromBody] NotificacionCompra NotificacionCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();

            string notificacionesSinLeer = "0";
            
            string instruccion = "";

            string strSql = ""; // se inserta en tabka de notficaciones
            strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
            strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDPREO, FIAPAPLENC, FIAPIDENPE, FSAPINSTRU, FIAPSTATUS, USERCREAT, PROGCREAT) ";
            strSql += "VALUES ";
            strSql += "(";
            strSql += NotificacionCompra.IdCuenta + ",";
            strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
            strSql += "CURRENT DATE" + ",";
            strSql += "CURRENT TIME" + ",";
            strSql += "'" + NotificacionCompra.Asunto + "',";
            strSql += "'" + NotificacionCompra.DescripcionNotificacion + "',";
            strSql += "0,";
            strSql += "default,";
            strSql += "1,";
            strSql += "default,";
            strSql += "'" + instruccion + "'" + ",";
            strSql += "1,'APPS','APPS'";
            strSql += ")";

            string strSqlNotificaciones = "";
            strSqlNotificaciones += "select  COUNT(*) NOTIFICACIONES from ";
            strSqlNotificaciones += "prodapps.APDNOTST ";
            strSqlNotificaciones += "where FIAPIDCUEN = " + NotificacionCompra.IdCuenta + " ";
            strSqlNotificaciones += " AND FIAPVISTO = 0 AND FIAPSTATUS = 1";

            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                dbCnx.SetQuery(strSql);
                
                DataTable notificacionesNL = dbCnx.GetDataSet(strSqlNotificaciones).Tables[0];

                if (notificacionesNL.Rows.Count > 0)
                {
                    foreach (DataRow dr in notificacionesNL.Rows)
                    {

                        notificacionesSinLeer = dr["NOTIFICACIONES"].ToString().Trim();

                    }
                }

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se envió la notificación de forma satisfactoria";
                respuesta.Objeto = null;

                
                #region Notificacion fija
                string token = "";

                strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
                strSql += "WHERE FIAPSTATUS = 1 ";
                strSql += "AND FIAPIDCUEN = " + NotificacionCompra.IdCuenta;
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
                notification.title = NotificacionCompra.Asunto;
                notification.body = NotificacionCompra.DescripcionNotificacion;
                notification.badge = notificacionesSinLeer.Trim();
                alerta.notification = notification;
                data data = new data();
                // data.CUPRA = "Prueba";
                data.Seat = "";
                alerta.data = data;

                var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //  request.AddHeader("Authorization", "Key=AAAAewaUKJY:APA91bE0HZ6G0XGIy5BZYD0S432bnnyUdwPJwwp7weZhHone-h4espT_rz6NyOhj3ZuCL6Bh2cRwewCWMv5GGRMy3YvHBj8SyfHTUitPbLzPYr_-WJEkWaHyMuXzWwj-DiabwxN0gfNy");
                request.AddHeader("Authorization", llaveAutorizacionNotificaciones);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                #endregion

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo enviar la notificación";
                respuesta.Objeto = null;
            }

            return respuesta;
        }


        [Route("api/AlertasPush/PostEnviaNotificacionAutoLlego", Name = "PostEnviaNotificacionAutoLlego")]
        public Respuesta PostEnviaNotificacionAutoLlego([FromBody] NotificacionAutoLlego NotificacionCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();

            string notificacionesSinLeer = "0";

            string instruccion = "";

            string strSql = ""; // se inserta en tabka de notficaciones
            strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
            strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDPREO, FIAPAPLENC, FIAPIDENPE, FSAPINSTRU, FIAPSTATUS, USERCREAT, PROGCREAT) ";
            strSql += "VALUES ";
            strSql += "(";
            strSql += NotificacionCompra.IdCuenta + ",";
            strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
            strSql += "CURRENT DATE" + ",";
            strSql += "CURRENT TIME" + ",";
            strSql += "'" + "AUTO LLEGÓ" + "',";
            strSql += "'" + "AUTO LLEGÓ" + "',";
            strSql += "0,";
            strSql += "default,";
            strSql += "1,";
            strSql += "default,";
            strSql += "'" + instruccion + "'" + ",";
            strSql += "1,'APPS','APPS'";
            strSql += ")";

            string strSqlNotificaciones = "";
            strSqlNotificaciones += "select  COUNT(*) NOTIFICACIONES from ";
            strSqlNotificaciones += "prodapps.APDNOTST ";
            strSqlNotificaciones += "where FIAPIDCUEN = " + NotificacionCompra.IdCuenta + " ";
            strSqlNotificaciones += " AND FIAPVISTO = 0 AND FIAPSTATUS = 1";


            string strSqlSeg = "";
            strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APDCKLST ";
            strSqlSeg += "SET FIAPREALIZ = 1" + ", ";
            strSqlSeg += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
            strSqlSeg += "WHERE FIAPIDCOMP = " + NotificacionCompra.IdCompra + " ";
            strSqlSeg += "AND FIAPIDPCKL = 17 ";
            strSqlSeg += "AND FIAPSTATUS = 1";


            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                dbCnx.SetQuery(strSql);
                dbCnx.SetQuery(strSqlSeg);

                DataTable notificacionesNL = dbCnx.GetDataSet(strSqlNotificaciones).Tables[0];

                if (notificacionesNL.Rows.Count > 0)
                {
                    foreach (DataRow dr in notificacionesNL.Rows)
                    {

                        notificacionesSinLeer = dr["NOTIFICACIONES"].ToString().Trim();

                    }
                }

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se envió la notificación de forma satisfactoria";
                respuesta.Objeto = null;


                #region Notificacion fija
                string token = "";

                strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
                strSql += "WHERE FIAPSTATUS = 1 ";
                strSql += "AND FIAPIDCUEN = " + NotificacionCompra.IdCuenta;
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
                notification.title = "AUTO LLEGÓ";//NotificacionCompra.Asunto;
                notification.body = "AUTO LLEGÓ";//NotificacionCompra.DescripcionNotificacion;
                notification.badge = notificacionesSinLeer.Trim();
                alerta.notification = notification;
                data data = new data();
                // data.CUPRA = "Prueba";
                data.Seat = "";
                alerta.data = data;

                var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //  request.AddHeader("Authorization", "Key=AAAAewaUKJY:APA91bE0HZ6G0XGIy5BZYD0S432bnnyUdwPJwwp7weZhHone-h4espT_rz6NyOhj3ZuCL6Bh2cRwewCWMv5GGRMy3YvHBj8SyfHTUitPbLzPYr_-WJEkWaHyMuXzWwj-DiabwxN0gfNy");
                request.AddHeader("Authorization", llaveAutorizacionNotificaciones);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(alerta), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                #endregion

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo enviar la notificación";
                respuesta.Objeto = null;
            }

            return respuesta;
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
        public string badge { get; set; }
    }
    public class data
    {
        public string Seat { get; set; }
    }
    
}
