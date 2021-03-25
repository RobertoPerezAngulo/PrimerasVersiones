using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using wsApiSeat.Models;
using wsApiSeat.Models.Nissan;
using wsApiSeat.Services;

namespace wsApiSeat.Bussiness
{
    public class BAutosSeminuevosNissan
    {
        internal static Respuesta PostMeInteresa(MeIntersa obj)
        {
            Respuesta response = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                String strSql = string.Empty;
                DVAConstants.Constants constantes = new DVAConstants.Constants();


                Cuenta cuenta = new Cuenta();
                cuenta.Nombre = obj.DatosCliente.Nombre;
                cuenta.ApellidoPaterno = obj.DatosCliente.ApellidoPaterno;
                cuenta.Correo = obj.DatosCliente.Email;
                cuenta.TelefonoMovil = obj.DatosCliente.Telefono;

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                response = PostRegistraCuenta(cuenta, dbCnx, constantes);
                if (response.Ok.Equals("SI"))
                {
                    #region REGISTRA DETALLE 
                    Cuenta persona = jss.Deserialize<Cuenta>(response.Objeto);
                    //GET CONSEVUTIVE
                    int idDetalle = 0;
                    strSql = "";
                    strSql = "SELECT COALESCE(MAX(FIAPIDCONS),0) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APDCOMNS";
                    DataTable dtdn = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dtdn.Rows.Count == 1)
                    {

                        idDetalle = int.Parse(dtdn.Rows[0]["Id"].ToString());
                        string jsonAuto = jss.Serialize(obj.AutoInteres);
                        string jsonCliente = jss.Serialize(obj.DatosCliente);

                        strSql = "";
                        strSql = @"INSERT INTO PRODAPPS.APDCOMNS
	 		                  (
	 		                  FIAPIDCONS, /*ID CONSECUTIVO*/                              
	 		                  FIAPIDCUEN, /*ID CUENTA*/
	 		                 FIAPIDTIAU,  /*TIPO AUTO*/
	 		                  FSAPTIPAUT, /*DESCRIPCION TIPO AUTO*/
	 		                  FSAPAUTJSN, /*JSON AUTO*/
	 		                  FSAPCLIJSN, /*JSON CLIENTE*/
	 		                  FIAPIDESTA, /*ESTADO*/
	 		                  FIAPSTATUS, /*ESTATUS*/     
                              USERCREAT,  /*USUARIO CREACION */
                              DATECREAT,  /*FECHA CREACION   */
                              TIMECREAT,  /*HORA CREACION    */
                              PROGCREAT   /*PROGRAMA CREACION*/)
                              VALUES(" +
                          idDetalle + " , " +
                          persona.IdCuenta + " , " +
                          2 + " , " +
                          "'SEMINUEVO'" + " , '" +
                          jsonAuto + "' , '" +
                          jsonCliente + "' , " +
                          1 + " , " +
                          1 + " , " +
                          "'APP' , " +
                          "CURRENT DATE , " +
                          "CURRENT TIME , " +
                          "'APP'" + " )";
                        dbCnx.SetQuery(strSql);

                        strSql = "";
                        strSql = @"SELECT * FROM " + constantes.Ambiente + "APPS.APDCNCST WHERE FIAPIDCIAU = " + obj.DatosCliente.IdAgencia + " AND  FSAPTIPAUT = " + (int)ETipoAuto.SEMINUEVO;
                        string subject = "Nissan";
                        DataTable dtCorreos = dbCnx.GetDataSet(strSql).Tables[0];
                        foreach (DataRow dr in dtCorreos.Rows)
                        {
                            String correo = dr["FSAPCORREO"].ToString().Trim();
                            /*Código para enviar correo*/
                            HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, correo.Trim(), ObtenerStrHtmlCompra(obj.AutoInteres));
                            Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreo.EnvioCorreo));
                            hilo.Start();

                        }

                        HiloEnvioCorreo hiloEnvioCorreoClit = new HiloEnvioCorreo(subject, obj.DatosCliente.Email.Trim(), ObtenerStrHtmlCompra(obj.AutoInteres));
                        Thread hiloClient = new Thread(new ThreadStart(hiloEnvioCorreoClit.EnvioCorreo));
                        hiloClient.Start();
                        response.Ok = "SI";
                        response.Mensaje = "El auto de interés se guardó correctamente.";
                        response.Objeto = jss.Serialize(persona);
                        #endregion
                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();
                    }

                }

            }
            catch (Exception _exc)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                response.Ok = "NO";
                response.Mensaje = "No se pudo guardar su auto de interés , por favor inténtelo más tarde";
                response.Objeto = null;
            }

            return response;
        }
        internal static Respuesta PostRegistraCuenta(Cuenta cuenta, DVADB.DB2 dbCnx, DVAConstants.Constants constantes)
        {

            Respuesta respuesta = new Respuesta();
            try
            {

                #region REGISTRA CUENTA
                long idCuenta = 0;
                string strSql = "";
                strSql = "SELECT  * FROM PRODAPPS.APCCTAST WHERE lower(TRIM(FSAPCORREO)) = '" + cuenta.Correo.Trim().ToLower() + "' ORDER BY DATECREAT DESC FETCH FIRST 1 ROWS ONLY ";
                DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];

                if (dtValidaCuenta.Rows.Count > 0)
                {
                    cuenta.IdCuenta = (dtValidaCuenta.Rows[0]["FIAPIDCUEN"].ToString());
                    cuenta.IdPersona = (dtValidaCuenta.Rows[0]["FIAPIDPERS"].ToString());
                    cuenta.Nombre = (dtValidaCuenta.Rows[0]["FSAPNOMBRE"].ToString());
                    cuenta.ApellidoPaterno = (dtValidaCuenta.Rows[0]["FSAPAPEPAT"].ToString());
                    cuenta.ApellidoMaterno = (dtValidaCuenta.Rows[0]["FSAPAPEMAT"].ToString());
                    cuenta.Correo = (dtValidaCuenta.Rows[0]["FSAPCORREO"].ToString());
                    cuenta.Token = (dtValidaCuenta.Rows[0]["FSAPTOKEN"].ToString());
                    cuenta.Clave = (dtValidaCuenta.Rows[0]["FSAPCVEACT"].ToString());
                }
                else
                {

                    strSql = "";
                    strSql = "SELECT MAX(FIAPIDCUEN) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST ";

                    DataTable dt_cuenta = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt_cuenta.Rows.Count == 1)
                    {
                        idCuenta = long.Parse(dt_cuenta.Rows[0]["Id"].ToString());
                        cuenta.IdCuenta = idCuenta.ToString();

                        strSql = "";
                        strSql = "INSERT INTO " + constantes.Ambiente + "APPS.APCCTAST ";
                        strSql += "(FIAPIDCUEN , FSAPNOMBRE, FSAPAPEPAT, FSAPCORREO, FSAPTOKEN, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT) VALUES ";
                        strSql += "(" + idCuenta + ",'" + cuenta.Nombre.Trim().ToUpper() + "', '" + cuenta.ApellidoPaterno.Trim().ToUpper() + "', '" + cuenta.Correo.Trim().ToLower() + "','" + " " +
                            "', 1, 1, 'APP', CURRENT DATE, CURRENT TIME, 'APP')";
                        dbCnx.SetQuery(strSql);
                    }
                }
                #endregion

                respuesta.Ok = "SI";
                respuesta.Objeto = JsonConvert.SerializeObject(cuenta);
            }
            catch (Exception ex)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No pudimos registrar tu cuenta, por favor inténtelo más tarde.";
                return respuesta;
            }
            return respuesta;
        }
        internal static string getResponse(string response)
        {
            var data = (JObject)JsonConvert.DeserializeObject(response);
            var result = data["Respuesta"].Value<String>();
            return result;
        }
        internal static string ObtenerStrHtmlCompra(Auto auto)
        {
            string strHtml = "";

            try
            {
                string rutaArchivo = obtenerURLServidor() + "Resources/" + "autoMeInteresaHtml.html";
                //string rutaArchivo = ruta + "Resources/Cupra/" + "autoMeInteresaHtml.html";
                //strHtml = File.ReadAllText(@"C:\Users\eromerom\Desktop\wsApiSeat\Resources\autoMeInteresaHtml.html"); //Descomentar para pruebas locales...
                strHtml = leerArchivoWeb(rutaArchivo);

                strHtml = strHtml.Replace("[{Marca}]", auto.Marca);
                strHtml = strHtml.Replace("[{Modelo}]", auto.Modelo);
                strHtml = strHtml.Replace("[{Version}]", auto.Version);
                strHtml = strHtml.Replace("[{Anio}]", auto.Anio);
                strHtml = strHtml.Replace("[{Color}]", auto.Color);
                strHtml = strHtml.Replace("[{Km}]", auto.Kilometraje);
                                
            }
            catch (Exception ex)
            {

            }

            return strHtml;
        }
        internal static string obtenerURLServidor()
        {

            HttpRequest request = HttpContext.Current.Request;

            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";

            return baseUrl;
        }
        internal static string leerArchivoWeb(string url)
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