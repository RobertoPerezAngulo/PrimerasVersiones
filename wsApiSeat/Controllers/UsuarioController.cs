using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using wsApiSeat.Models;
using wsApiSeat.Services;
using RestSharp;
using wsApiSeat.Bussiness;
using wsApiSeat.Models.OrdenCompra;
using wsApiSeat.Models.Notificaciones;

namespace wsApiSeat.Controllers
{
    public class UsuarioController : ApiController
    {
        #region Constantes de la clase
        const string llaveAutorizacionNotificaciones = "Key=AAAAE2lxxJI:APA91bH_HW8bSZLgp4JIzVZhGoR6ziMZb-uPAYO6e0zXTt34AuxAp7W9bhdvAY5H03GwPggVNl3AmYPTy0KbMz4wQWyPIPsV7o-awXmhjdrx1NxQJK0b_xLatKpqeBqKBv7HZeZKFVlM";
        #endregion


        // POST api/Usuario/PostLigarCliente
        [Route("api/Usuario/PostLigarCliente", Name = "PostLigarCliente")]
        public Respuesta PostLigarCliente(long IdPersona, string Correo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "No es posible ligar su número de cliente.";
            respuesta.Objeto = "";

            try
            {
                string correo = "";

                string strSql = "";

                strSql = "";
                strSql += "SELECT    1 ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                strSql += "WHERE	LOWER(TRIM(CTECV.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' ";
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    respuesta.Mensaje = "No existe una cuenta.";
                    throw new Exception();
                }
                else
                {
                    strSql = "";
                    strSql += "SELECT   TRIM(RPMAI.FSMPEMAIL) FSMPEMAIL, TRIM(PERSO.FSPFNOMBRE) FSPFNOMBRE, TRIM(PERSO.FSPFAPATER) FSPFAPATER, TRIM(PERSO.FSPFAMATER) FSPFAMATER ";
                    strSql += "FROM " + constantes.Ambiente + "PERS.CTDRPMAI RPMAI ";
                    strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCPERFI PERSO ";
                    strSql += "ON PERSO.FDPFIDPERS = RPMAI.FDMPIDPERS ";
                    strSql += "WHERE	RPMAI.FDMPIDPERS = " + IdPersona.ToString() + " ";
                    strSql += "AND	RPMAI.FBCTDEFAUL = 1 ";
                    strSql += "AND	RPMAI.FDMPESTATU = 1 ";
                    strSql += "FETCH FIRST 1 ROW ONLY";
                    DataTable dtValidaCliente = dbCnx.GetDataSet(strSql).Tables[0];
                    foreach (DataRow drValidaCliente in dtValidaCliente.Rows)
                    {
                        if (Correo.Trim().ToLower().Equals(drValidaCliente["FSMPEMAIL"].ToString().Trim().ToLower()))
                        {
                            correo = drValidaCliente["FSMPEMAIL"].ToString().Trim().ToLower();
                        }
                    }
                    if (string.IsNullOrEmpty(correo))
                    {
                        respuesta.Mensaje = "El correo ligado al número de cliente no coincide con el correo proporcionado.";
                        throw new Exception();
                    }

                    strSql = "";
                    strSql += "UPDATE " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                    strSql += "SET CTECV.FIAPIDPERS = " + IdPersona.ToString() + " ";
                    strSql += "WHERE	LOWER(TRIM(CTECV.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' ";

                    try
                    {
                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        dbCnx.SetQuery(strSql);

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Número de cliente ligado con éxito.";
                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();
                        throw new Exception();
                    }
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                return respuesta;
            }
        }

        // GET api/Usuario/GetGenerarClaveActivacion
        [Route("api/Usuario/GetGenerarClaveActivacion", Name = "GetGenerarClaveActivacion")]
        public string GetGenerarClaveActivacion()
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

        // POST api/Usuario/PostActualizarTokenCuenta
        [Route("api/Usuario/PostActualizarTokenCuenta", Name = "PostActualizarTokenCuenta")]
        public List<RespuestaServidor> PostActualizarTokenCuenta(string IdCuenta, string Token)
        {
            String respuesta = "";

            DVADB.DB2 dbcnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {

                dbcnx.AbrirConexion();
                dbcnx.BeginTransaccion();

                string strSql = "";
                strSql += "UPDATE " + constantes.Ambiente + "APPS.APCCTAST ";
                strSql += " SET FSAPTOKEN = '" + Token + "'";
                strSql += " ,USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP'";
                strSql += " WHERE FIAPIDCUEN = " + IdCuenta;

                dbcnx.SetQuery(strSql);

                dbcnx.CommitTransaccion();
                dbcnx.CerrarConexion();

                respuesta = "SI";

                RespuestaServidor respuestaServidor;
                List<RespuestaServidor> coleccionRespuestas = new List<RespuestaServidor>();

                respuestaServidor = new RespuestaServidor();
                respuestaServidor.Respuesta = respuesta;

                coleccionRespuestas.Add(respuestaServidor);

                return coleccionRespuestas;

            }

            catch (Exception e)
            {
                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuesta = "No se pudo actualizar el token de la cuenta";

                RespuestaServidor respuestaServidor;
                List<RespuestaServidor> coleccionRespuestas = new List<RespuestaServidor>();

                respuestaServidor = new RespuestaServidor();
                respuestaServidor.Respuesta = respuesta;

                coleccionRespuestas.Add(respuestaServidor);

                return coleccionRespuestas;

            }
        }
        
        // GET api/Usuario/GetActualizarClave
        [Route("api/Usuario/GetActualizarClave", Name = "GetActualizarClave")]
        public RespuestaTest<Cuenta> GetActualizarClave(string Correo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "No es posible actualizar su clave.";
            respuesta.Objeto = null;

            try
            {
                string correo = "";
                correo = Correo;

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

                string strSql = "";

                strSql = "";
                strSql += "SELECT    1 ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                strSql += "WHERE	LOWER(TRIM(CTECV.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' ";
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    respuesta.Mensaje = "No existe una cuenta.";
                    throw new Exception();
                }
                else
                {
                    strSql = "";
                    strSql += "UPDATE " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                    strSql += "SET CTECV.FSAPCVEACT = '" + clave + "' ";
                    strSql += ", FIAPSTATUS = 1 ";
                    strSql += "WHERE	LOWER(TRIM(CTECV.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' ";

                    try
                    {
                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();
                        dbCnx.SetQuery(strSql);
                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();
                        respuesta.Ok = "SI";
                        respuesta.Mensaje = Correo + "\nSe generó clave con éxito.";
                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();
                        throw new Exception();
                    }
                }
                if (respuesta.Ok.Equals("SI"))
                {
                    if (!string.IsNullOrEmpty(correo))
                    {
                        string subject = "Furia- Clave de activación";
                        string strHtml = "";
                        try
                        {
                            HttpRequest request = HttpContext.Current.Request;
                            BPedido p = new BPedido();
                            //string rutaArchivo = p.obtenerURLServidor() + "Resources/Email/" + "claveActivacionCupraHtml.html"; // lee archivo web
                            string rutaArchivo = p.obtenerURLServidor() + "Resources/Email/" + "claveActivacionSEAT.html";
                            strHtml = p.leerArchivoWeb(rutaArchivo);
                            
                            strHtml = strHtml.Replace("[{Clave}]", clave);
                        }
                        catch (Exception ex)
                        {

                        }
         
                        HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, correo, strHtml);
                        Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreo.EnvioCorreo));
                        hilo.Start();

                    }
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                return respuesta;
            }
        }      
                    
        public List<Persona> ObtenerDatosPersonaActiva(long IdPersona, string ClaveActivacion)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    CTECV.FIAPIDCUEN, PERSO.FDPEIDPERS, PERSO.FSPERFC, PERSO.FDPEIDTIPO, ";
            strSql += "COALESCE(TRIM(PERMO.FSPMRAZON), TRIM(PERFI.FSPFNOMBRE)||' '||TRIM(PERFI.FSPFAPATER)||' '||TRIM(PERFI.FSPFAMATER)) NOMBRE, ";

            strSql += "COALESCE(TRIM(CONTAC.FSPFNOMBRE)||' '||TRIM(CONTAC.FSPFAPATER)||' '||TRIM(CONTAC.FSPFAMATER), TRIM(PERFI.FSPFNOMBRE)||' '||TRIM(PERFI.FSPFAPATER)||' '||TRIM(PERFI.FSPFAMATER)) RAZSOC, ";

            strSql += "RPTEL.FDPTPLADA, RPTEL.FDPTNUMTEL TELEFONO, ";
            strSql += "RPDOM.FDPDIDTDOM, RPDOM.FBCTDEFAUL, TRIM(RPDOM.FSPDCALLE) CALLE, TRIM(RPDOM.FSPDNUMEXT) NUMEXT, TRIM(RPDOM.FSPDNUMINT) NUMINT, TRIM(RPDOM.FSPDCOLON) COLON, TRIM(RPDOM.FDPDCODPOS) CODPOS, ";
            strSql += "FISCAL.FDPDIDTDOM, FISCAL.FBCTDEFAUL, FISCAL.FSPDCALLE, FISCAL.FSPDNUMEXT, FISCAL.FSPDNUMINT, FISCAL.FSPDCOLON, FISCAL.FDPDCODPOS, ";

            strSql += "DELEG.FSDMNOMBRE, ESTAD.FSEDNOMBRE, ";
            strSql += "TRIM(DEL.FSDMNOMBRE) DELPERS, ";

            strSql += "TRIM(RPMAI.FSMPEMAIL) FSMPEMAIL ";

            strSql += "FROM	" + constantes.Ambiente + "PERS.CTEPERSO PERSO ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCPERFI PERFI ";
            strSql += "ON	PERSO.FDPEIDPERS = PERFI.FDPFIDPERS ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTDPERMO PERMO ";
            strSql += "ON	PERSO.FDPEIDPERS = PERMO.FDPMIDPERS ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTDRPTEL RPTEL ";
            strSql += "ON	PERSO.FDPEIDPERS = RPTEL.FDPTIDPERS ";
            strSql += "AND	RPTEL.FBCTDEFAUL = 1 ";
            strSql += "AND	RPTEL.FDPTESTATU = 1 ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTDRPMAI RPMAI ";
            strSql += "ON	PERSO.FDPEIDPERS = RPMAI.FDMPIDPERS ";
            strSql += "AND	RPMAI.FBCTDEFAUL = 1 ";
            strSql += "AND	RPMAI.FDMPESTATU = 1 ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTDRPDOM RPDOM ";
            strSql += "ON	PERSO.FDPEIDPERS = RPDOM.FDPDIDPERS ";
            strSql += "AND	RPDOM.FBCTDEFAUL = 1 ";
            strSql += "AND	RPDOM.FDPDIDTDOM = 1 ";
            strSql += "AND	RPDOM.FDPDESTATU = 1 ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTDRPDOM FISCAL ";
            strSql += "ON	PERSO.FDPEIDPERS = FISCAL.FDPDIDPERS ";
            //strSql += "AND	FISCAL.FBCTDEFAUL = 1 ";
            strSql += "AND	FISCAL.FDPDESTATU = 1 ";
            strSql += "AND	FISCAL.FDPDIDTDOM = 3 ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCCODPO CODPO ";
            strSql += "ON	FISCAL.FDPDCODPOS = CODPO.FDCPCODPOS ";
            strSql += "AND	FISCAL.FSPDCOLON = CODPO.FSCPCOLON ";
            strSql += "AND	CODPO.FICTSTATUS = 1 ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCCODPO CP ";
            strSql += "ON	RPDOM.FDPDCODPOS = CP.FDCPCODPOS ";
            strSql += "AND	RPDOM.FSPDCOLON = CP.FSCPCOLON ";
            strSql += "AND	CP.FICTSTATUS = 1 ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCDELEG DELEG ";
            strSql += "ON	DELEG.FSDMIDDLMN = CODPO.FSCPIDLMPO ";
            strSql += "AND	DELEG.FSDMIDEDO = CODPO.FSCPIDEDO ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCDELEG DEL ";
            strSql += "ON	DEL.FSDMIDDLMN = CP.FSCPIDLMPO ";
            strSql += "AND	DEL.FSDMIDEDO = CP.FSCPIDEDO ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCESTAD ESTAD ";
            strSql += "ON	ESTAD.FSEDIDEDO = CODPO.FSCPIDEDO ";

            strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTCPERFI CONTAC ";
            strSql += "ON	PERMO.FICTIDRPLG = CONTAC.FDPFIDPERS ";

            //strSql += "LEFT JOIN " + constantes.Ambiente + "PERS.CTDRPPCR RELAC ";
            //strSql += "ON	ESTAD.FSEDIDEDO = CODPO.FSCPIDEDO ";

            strSql += "INNER JOIN " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
            strSql += "ON	PERSO.FDPEIDPERS = CTECV.FIAPIDPERS ";
            strSql += "AND	TRIM(CTECV.FSAPCVEACT) = '" + ClaveActivacion.Trim() + "' ";
            strSql += "WHERE PERSO.FDPEIDPERS = " + IdPersona.ToString();
            strSql += " FETCH FIRST 1 ROW ONLY";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            Persona persona;
            List<Persona> coleccionPersonas = new List<Persona>();
            foreach (DataRow dr in dt.Rows)
            {
                persona = new Persona();
                persona.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                persona.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                persona.RFC = dr["FSPERFC"].ToString().Trim();
                persona.TipoPersona = dr["FDPEIDTIPO"].ToString().Trim();
                persona.NombrePersona = dr["NOMBRE"].ToString().Trim();
                persona.LadaPersona = dr["FDPTPLADA"].ToString().Trim();
                persona.TelefonoPersona = dr["TELEFONO"].ToString().Trim();
                persona.CorreoPersona = dr["FSMPEMAIL"].ToString().Trim();
                persona.CallePersona = dr["CALLE"].ToString().Trim();
                persona.NumExtPersona = dr["NUMEXT"].ToString().Trim();
                persona.NumIntPersona = dr["NUMINT"].ToString().Trim();
                persona.ColoniaPersona = dr["COLON"].ToString().Trim();
                persona.DelegacionPersona = dr["FSDMNOMBRE"].ToString().Trim();
                persona.CodigoPostalPersona = dr["CODPOS"].ToString().Trim();
                if (persona.TipoPersona == "1")
                {
                    persona.CalleFactura = dr["CALLE"].ToString().Trim();
                    persona.NumExtFactura = dr["NUMEXT"].ToString().Trim();
                    persona.NumIntFactura = dr["NUMINT"].ToString().Trim();
                    persona.ColoniaFactura = dr["COLON"].ToString().Trim();
                    persona.CodigoPostalFactura = dr["CODPOS"].ToString().Trim();
                    persona.DelegacionFactura = dr["FSDMNOMBRE"].ToString().Trim();
                    persona.RazonSocialFactura = dr["NOMBRE"].ToString().Trim();
                    persona.EstadoFactura = dr["FSEDNOMBRE"].ToString().Trim();
                }
                else
                {
                    persona.CalleFactura = dr["FSPDCALLE"].ToString().Trim();
                    persona.NumExtFactura = dr["FSPDNUMEXT"].ToString().Trim();
                    persona.NumIntFactura = dr["FSPDNUMINT"].ToString().Trim();
                    persona.ColoniaFactura = dr["FSPDCOLON"].ToString().Trim();
                    persona.CodigoPostalFactura = dr["FDPDCODPOS"].ToString().Trim();
                    persona.DelegacionFactura = dr["FSDMNOMBRE"].ToString().Trim();
                    persona.RazonSocialFactura = dr["RAZSOC"].ToString().Trim();
                    persona.EstadoFactura = dr["FSEDNOMBRE"].ToString().Trim();
                }

                //persona.EstadoFactura = dr["FSEDNOMBRE"].ToString().Trim();

                coleccionPersonas.Add(persona);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPersonas));
            return coleccionPersonas;
        }

        // POST api/Usuario/ObtenerNotificacionesCuenta
        [Route("api/Usuario/GetObtenerNotificacionesCuenta", Name = "GetObtenerNotificacionesCuenta")]
        public List<Notificacion> GetObtenerNotificacionesCuenta(string IdCuenta)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDNOTST NOTI ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "GRAL.GECCIAUN AGEN ";
            strSql += "ON	NOTI.FIAPIDCIAU = AGEN.FIGEIDCIAU ";
            strSql += "WHERE NOTI.FIAPSTATUS = 1 ";
            strSql += "AND NOTI.FIAPIDCUEN = " + IdCuenta + " ORDER BY FFAPNOTIFI, FHAPNOTIFI DESC";


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            DataView dtX = dt.DefaultView;
            dtX.Sort = "FFAPNOTIFI DESC";
            dt = dtX.ToTable();


            Notificacion notificacion;
            List<Notificacion> coleccionNotificaciones = new List<Notificacion>();
            foreach (DataRow dr in dt.Rows)
            {
                notificacion = new Notificacion();
                notificacion.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                notificacion.IdNotificacion = dr["FIAPIDNOTI"].ToString().Trim();
                notificacion.FechaNotificacion = DateTime.Parse(dr["FFAPNOTIFI"].ToString().Trim()).ToString("dd-MM-yyyy");
                notificacion.HoraNotificacion = DateTime.Parse(dr["FHAPNOTIFI"].ToString().Trim()).ToString("HH:mm:ss");
                notificacion.Asunto = dr["FSAPASUNTO"].ToString().Trim();
                notificacion.DescripcionNotificacion = dr["FSAPNOTIFI"].ToString().Trim();
                notificacion.AplicaSeguimiento = dr["FIAPAPLSEG"].ToString().Trim();
                notificacion.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                notificacion.IdPreorden = dr["FIAPIDPREO"].ToString().Trim();
                notificacion.AplicaEncuesta = dr["FIAPAPLENC"].ToString().Trim();
                notificacion.NombreLogo = dr["FSGENOMLOG"].ToString().Trim();
                notificacion.IdEncuesta = dr["FIAPIDENPE"].ToString().Trim();
                notificacion.Instrucciones = dr["FSAPINSTRU"].ToString().Trim();
                notificacion.Visto = dr["FIAPVISTO"].ToString().Trim();
                coleccionNotificaciones.Add(notificacion);
            }

            //coleccionNotificaciones = coleccionNotificaciones.OrderByDescending(o => o.IdNotificacion).ToList();

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionNotificaciones));
            return coleccionNotificaciones;
        }

        // POST api/Usuario/GetNotificacionesCompra
        [Route("api/Usuario/GetNotificacionesCompra", Name = "GetNotificacionesCompra")]
        public List<Notificacion> GetNotificacionesCompra(string IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "select noti.* ";
            strSql += "from	PRODAPPS.APECMPST compr ";
            strSql += "inner join PRODAPPS.APDNOTST noti ";
            strSql += "on compr.FIAPIDCUEN = noti.FIAPIDCUEN and noti.FIAPSTATUS = 1 ";
            strSql += "left join " + constantes.Ambiente + "GRAL.GECCIAUN agen ";
            strSql += "on noti.FIAPIDCIAU = agen.FIGEIDCIAU ";
            strSql += "where compr.FIAPIDCOMP = " + IdCompra + " order by FFAPNOTIFI, FHAPNOTIFI DESC";


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            DataView dtX = dt.DefaultView;
            dtX.Sort = "FFAPNOTIFI DESC";
            dt = dtX.ToTable();


            Notificacion notificacion;
            List<Notificacion> coleccionNotificaciones = new List<Notificacion>();
            foreach (DataRow dr in dt.Rows)
            {
                notificacion = new Notificacion();
                notificacion.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                notificacion.IdNotificacion = dr["FIAPIDNOTI"].ToString().Trim();
                notificacion.FechaNotificacion = DateTime.Parse(dr["FFAPNOTIFI"].ToString().Trim()).ToString("dd-MM-yyyy");
                notificacion.HoraNotificacion = DateTime.Parse(dr["FHAPNOTIFI"].ToString().Trim()).ToString("HH:mm:ss");
                notificacion.Asunto = dr["FSAPASUNTO"].ToString().Trim();
                notificacion.DescripcionNotificacion = dr["FSAPNOTIFI"].ToString().Trim();
                //notificacion.AplicaSeguimiento = dr["FIAPAPLSEG"].ToString().Trim();
                //notificacion.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                //notificacion.IdPreorden = dr["FIAPIDPREO"].ToString().Trim();
                //notificacion.AplicaEncuesta = dr["FIAPAPLENC"].ToString().Trim();
                //notificacion.NombreLogo = dr["FSGENOMLOG"].ToString().Trim();
                //notificacion.IdEncuesta = dr["FIAPIDENPE"].ToString().Trim();
                notificacion.Instrucciones = dr["FSAPINSTRU"].ToString().Trim();
                notificacion.Visto = dr["FIAPVISTO"].ToString().Trim();
                coleccionNotificaciones.Add(notificacion);
            }
            
            return coleccionNotificaciones;
        }

        [Route("api/Usuario/PostEnviaNotificacionesTribuCupra", Name = "PostEnviaNotificacionesTribuCupra")]
        public Respuesta PostEnviaNotificacionesTribuCupra([FromBody] NotificacionCompra NotificacionCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();

            string notificacionesSinLeer = "0";

            string strSqlSeg = "";
            strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APECMPST "; //  SE ACTUALIZA EL CABECERO
            strSqlSeg += "SET FIAPIDESTA = " + NotificacionCompra.IdEstado + ", ";
            strSqlSeg += "USERUPDAT = 'APPS', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APPS' ";
            strSqlSeg += "WHERE FIAPIDCOMP = " + NotificacionCompra.IdCompra + " ";
            strSqlSeg += "AND FIAPSTATUS = 1";


            string strInsCabComp = ""; // se inserta el detalle
            strInsCabComp += "INSERT INTO " + constantes.Ambiente + "APPS.APDSGCST ";
            strInsCabComp += "(FIAPIDCOMP, FIAPIDSEGU, FSAPTITSEG, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT)";
            strInsCabComp += "VALUES ";
            strInsCabComp += "(";
            strInsCabComp += NotificacionCompra.IdCompra + ", ";
            strInsCabComp += "(SELECT coalesce(MAX(FIAPIDSEGU),0)+1 ID FROM PRODAPPS.APDSGCST WHERE FIAPIDCOMP = " + NotificacionCompra.IdCompra + "),";
            strInsCabComp += "'Movimiento generado en SmartIt' ,";
            strInsCabComp += NotificacionCompra.IdEstado + ",";
            strInsCabComp += "1, 'APPS' ,CURRENT DATE, CURRENT TIME, 'APPS'";
            strInsCabComp += ")";

            string instruccion = "{\"Vista\":\"miCupra\",\"Parametros\":{\"IdCompra\":\"" + NotificacionCompra.IdCompra + "\"}}";

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
            strSql += "'" +instruccion + "'" + ",";
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

                dbCnx.SetQuery(strSqlSeg);
                dbCnx.SetQuery(strSql);
                dbCnx.SetQuery(strInsCabComp);

                if (NotificacionCompra.IdEstado.Equals("15"))
                {
                    strSql = "";
                    strSql += "select FIAPIDCIAU, FIAPIDPEDI, FIAPIDINVE from prodapps.APEPANST where FIAPIDCOMP = " + NotificacionCompra.IdCompra;
                    DataTable dtPedido = dbCnx.GetDataSet(strSql).Tables[0];

                    foreach (DataRow drPedido in dtPedido.Rows)
                    {
                        if (Convert.ToInt64(drPedido["FIAPIDPEDI"]) > 0) {

                            strSql = "";
                            strSql += "UPDATE PRODAUT.ANEPEDAU SET FIANPASTP = 4, USERUPDAT = 'SmartIt', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'SmartIt' WHERE FNANPAAGE = " + drPedido["FIAPIDCIAU"].ToString() + " AND FNANPAIDE = " + drPedido["FIAPIDPEDI"].ToString();
                            dbCnx.SetQuery(strSql);
                        }

                        strSql = "";
                        strSql += "UPDATE PRODAUT.ANCAUTOM SET FNAUTOEST = 10, USERUPDAT = 'SmartIt', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'SmartIt' WHERE FNAUTOEST in (11,50) AND FNAUTOAGE = " + drPedido["FIAPIDCIAU"].ToString() + " AND FIANIDINVE = " + drPedido["FIAPIDINVE"].ToString();
                        dbCnx.SetQuery(strSql);
                    }
                }                

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

                //NotificacionesAsync notificacion = new NotificacionesAsync(NotificacionCompra.IdCuenta, NotificacionCompra.Asunto, NotificacionCompra.DescripcionNotificacion);
                //Thread hilo = new Thread(new ThreadStart(notificacion.EnviarNotificacionAppTribuCupra));
                //hilo.Start();

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

        //Notificaciones de usuario
        [Route("api/Usuario/GetCuentaNotificaciones", Name = "GetCuentaNotificaciones")]
        public long GetCuentaNotificaciones(int Idcuenta)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string strSqlSeg = "";
            strSqlSeg = "SELECT * FROM PRODAPPS.APCCTAST WHERE FIAPIDCUEN = " + Idcuenta;
            DataTable dt = dbCnx.GetDataSet(strSqlSeg).Tables[0];
            if (dt.Rows.Count > 0)
            {
               strSqlSeg = "SELECT COUNT(*) NOTIFICACION FROM prodapps.APDNOTST WHERE FIAPIDCUEN = '" + Idcuenta + "' AND FIAPVISTO = 0 AND FIAPSTATUS = 1";
               DataTable dt1 = dbCnx.GetDataSet(strSqlSeg).Tables[0];
               return Convert.ToInt32(dt1.Rows[0]["NOTIFICACION"].ToString().Trim());
            }
            else
            {
               return Convert.ToInt32("0");
            }            
        }

        // POST api/Usuario/PostEnviaNotificacionesTribuCupra
        [Route("api/Usuario/PutActualizaNotificacionVisto", Name = "PutActualizaNotificacionVisto")]
        public RespuestaTest<Cuenta> PutActualizaNotificacionVisto(int IdCuenta, int IdNotificacion)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            
            string strSqlSeg = "";
            strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APDNOTST "; //  SE ACTUALIZA EL CAMPO DE VISTO EN LA NOTIFICACION
            strSqlSeg += "SET FIAPVISTO = " + 1 + ", ";
            strSqlSeg += "USERUPDAT = 'APPS', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APPS' ";
            strSqlSeg += "WHERE FIAPIDCUEN = " + IdCuenta + " ";
            strSqlSeg += "AND FIAPSTATUS = 1 ";
            strSqlSeg += "AND FIAPIDNOTI = " + IdNotificacion;
            
            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                dbCnx.SetQuery(strSqlSeg);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se cambió el estado a visto de la notificación";
                respuesta.Objeto = null;
            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo cambiar el estado a visto la notificación";
                respuesta.Objeto = null;
            }
            return respuesta;
        }
        
        // POST api/Usuario/GetAsuntos
        [Route("api/Usuario/GetAsuntos", Name = "GetAsuntos")]
        public List<Asunto> GetAsuntos(string IdEstado)
        {

            List<Asunto> lstAsunto = new List<Asunto>();
            Asunto asunto = new Asunto();

            try
            {
               
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Asunto.json");

                lstAsunto = JsonConvert.DeserializeObject<List<Asunto>>(strJSON);

                lstAsunto = lstAsunto.FindAll(c=>c.IdEstado == IdEstado);

               

                if (lstAsunto.Count == 0) {

                    lstAsunto.Add(new Asunto());
                    
                }


            }
            catch (Exception _exc)
            {
                lstAsunto = new List<Asunto>();
                lstAsunto.Add(new Asunto());
            }



            return lstAsunto;


        }
        
        // POST api/Usuario/GetMensajesPorIdAsunto
        [Route("api/Usuario/GetMensajesPorIdAsunto", Name = "GetMensajesPorIdAsunto")]
        public List<MensajeNotificacion> GetMensajesPorIdAsunto(string IdAsunto)
        {          
            List<MensajeNotificacion> lstMensaje = new List<MensajeNotificacion>();
            MensajeNotificacion mensaje = new MensajeNotificacion();

            try
            {

                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Mensaje.json");
                lstMensaje = JsonConvert.DeserializeObject<List<MensajeNotificacion>>(strJSON);

                lstMensaje = lstMensaje.FindAll(c => c.IdAsunto == IdAsunto);

                if (lstMensaje.Count == 0)
                {

                    lstMensaje.Add(new MensajeNotificacion());

                }
            }
            catch (Exception _exc)
            {
                lstMensaje = new List<MensajeNotificacion>();
                lstMensaje.Add(new MensajeNotificacion());
            }
            return lstMensaje;
        }


        // POST api/Usuario/PostRegistraCuenta
        [Route("api/Usuario/PostRegistraCuenta", Name = "PostRegistraCuenta")]
        public RespuestaTest<Cuenta> PostRegistraCuenta([FromBody] Cuenta cuentaJson)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>
            {
                Ok = "NO",
                Mensaje = "No es posible registrar su cuenta, intente más tarde.",
                Objeto = null
            };

            // Persona persona = new Persona();
            try
            {
                string clave = CuentaService.GenerarClaveActivacion();

                string strSql = "";

                strSql = "";
                strSql += "SELECT    1 ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                strSql += "WHERE LOWER(TRIM(CTECV.FSAPCORREO)) = '" + cuentaJson.Correo.Trim().ToLower() + "' ";
                strSql += " AND FIAPSTATUS = 1 ";

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    respuesta.Mensaje = "Ya existe una cuenta con su correo.";
                    return respuesta;
                }

                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();

                    strSql = "";
                    strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APCCTAST ";
                    strSql += "(FIAPIDCUEN, FSAPNOMBRE, FSAPAPEPAT, FSAPAPEMAT, FSAPCORREO, FIAPLADMOV, FIAPNUMMOV, FSAPCVEACT, FSAPTOKEN, FIAPIDESTA, FIAPSTATUS, USERCREAT, PROGCREAT) VALUES ";
                    strSql += "(";
                    strSql += "(SELECT COALESCE(MAX(FIAPIDCUEN), 0) + 1 FROM " + constantes.Ambiente + "APPS.APCCTAST),'";
                    strSql += cuentaJson.Nombre.Trim().ToUpper() + "','";
                    strSql += cuentaJson.ApellidoPaterno.Trim().ToUpper() + "','";
                    strSql += cuentaJson.ApellidoMaterno.Trim().ToUpper() + "','";
                    strSql += cuentaJson.Correo.Trim().ToLower() + "',";
                    strSql += "52" + "," + (string.IsNullOrEmpty(cuentaJson.TelefonoMovil.ToString().Trim()) ? "null" : cuentaJson.TelefonoMovil.ToString().Trim()) + ",";
                    strSql += "'" + clave + "'" + "," + "'" + cuentaJson.Token.ToString().Trim() + "'" + "," + "1,1,'APP','APP'";
                    strSql += ")";
                    dbCnx.SetQuery(strSql);


                    strSql = "";
                    strSql += "SELECT    FIAPIDCUEN ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                    strSql += "WHERE LOWER(TRIM(CTECV.FSAPCORREO)) = '" + cuentaJson.Correo.Trim().ToLower() + "' ";
                    strSql += "AND TRIM(CTECV.FSAPCVEACT) = '" + clave + "' ";

                    DataTable dtCuenta = dbCnx.GetDataSet(strSql).Tables[0];
                    if (dtCuenta.Rows.Count == 1)
                    {
                        cuentaJson.IdCuenta = dtCuenta.Rows[0]["FIAPIDCUEN"].ToString().Trim();
                        cuentaJson.Clave = clave;
                    }
                    else
                    {
                        throw new Exception();
                    }

                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();

                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Registro de cuenta con éxito.";
                    respuesta.Objeto = cuentaJson;
                }

                catch (Exception ex)
                {
                    respuesta.Mensaje = ex.ToString();
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    throw new Exception();
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                return respuesta;
            }
        }

        // POST api/Usuario/GetIniciarSesion
        [Route("api/Usuario/GetIniciarSesion", Name = "GetIniciarSesion")]
        public RespuestaTest<Cuenta> GetIniciarSesion(string Correo, string Clave)
        {

            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {
                string strSql = "";
                strSql = "SELECT    CTECV.FIAPSTATUS, CTECV.FIAPIDCUEN IdCuenta, CTECV.FIAPLADMOV, CTECV.FIAPNUMMOV, CTECV.FSAPTOKEN, ";
                strSql += "TRIM(CTECV.FSAPNOMBRE) Nombre, TRIM(CTECV.FSAPAPEPAT) ApellidoPaterno, TRIM(CTECV.FSAPAPEMAT) ApellidoMaterno, ";
                strSql += "TRIM(CTECV.FSAPCORREO) Correo ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                strSql += "WHERE LOWER(TRIM(CTECV.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' ";
                strSql += "AND TRIM(CTECV.FSAPCVEACT) = '" + Clave.Trim() + "' ";                
                strSql += " FETCH FIRST 1 ROW ONLY";
                
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                Cuenta cuenta;

                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["FIAPSTATUS"].ToString().Trim() == "0") {

                            respuesta.Ok = "NO";
                            respuesta.Mensaje = "La cuenta existe pero está inactiva, por favor actualice la clave.";
                            respuesta.Objeto = null;

                            return respuesta;

                        }

                        cuenta = new Cuenta();
                        cuenta.Clave = Clave.Trim();

                        cuenta.IdCuenta = dr["IdCuenta"].ToString().Trim();
                        cuenta.Nombre = dr["Nombre"].ToString().Trim();
                        cuenta.Token = dr["FSAPTOKEN"].ToString().Trim();
                        cuenta.ApellidoPaterno = dr["ApellidoPaterno"].ToString().Trim();
                        cuenta.ApellidoMaterno = dr["ApellidoMaterno"].ToString().Trim();
                        cuenta.Correo = dr["Correo"].ToString().Trim();
                        cuenta.LadaMovil = dr["FIAPLADMOV"].ToString().Trim();
                        cuenta.TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim();

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Inicio de sesión con éxito.";
                        respuesta.Objeto = cuenta;
                    }
                }
                else {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No es posible iniciar sesión.";
                    respuesta.Objeto = null;
                }                
                
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No es posible iniciar sesión.";
                respuesta.Objeto = null;
            }
            return respuesta;
        }
        

        // POST api/Usuario/GetIniciarSesionRedesSociales
        [Route("api/Usuario/GetIniciarSesionRedesSociales", Name = "GetIniciarSesionRedesSociales")]
        public RespuestaTest<Cuenta> GetIniciarSesionRedesSociales(string Correo)
        {
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            
            try
            {
                string strSql = "";
                strSql = "SELECT    CTECV.FIAPSTATUS, CTECV.FIAPIDCUEN IdCuenta, trim(FSAPCVEACT) Clave, CTECV.FIAPLADMOV, CTECV.FIAPNUMMOV, CTECV.FSAPTOKEN, ";
                strSql += "TRIM(CTECV.FSAPNOMBRE) Nombre, TRIM(CTECV.FSAPAPEPAT) ApellidoPaterno, TRIM(CTECV.FSAPAPEMAT) ApellidoMaterno, ";
                strSql += "TRIM(CTECV.FSAPCORREO) Correo ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTECV ";
                strSql += "WHERE LOWER(TRIM(CTECV.FSAPCORREO)) = '" + Correo.Trim().ToLower() + "' ";               
                strSql += "FETCH FIRST 1 ROW ONLY";
                
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                
                Cuenta cuenta;
                if (dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["FIAPSTATUS"].ToString().Trim() == "0") {

                            respuesta.Ok = "NO";
                            respuesta.Mensaje = "La cuenta existe pero está inactiva, por favor actualice la clave.";
                            respuesta.Objeto = null;                           

                            return respuesta;
                        }


                        cuenta = new Cuenta();
                        cuenta.Clave = dr["Clave"].ToString().Trim();
                        cuenta.Token = dr["FSAPTOKEN"].ToString().Trim();
                        cuenta.IdCuenta = dr["IdCuenta"].ToString().Trim();
                        cuenta.Nombre = dr["Nombre"].ToString().Trim();
                        cuenta.ApellidoPaterno = dr["ApellidoPaterno"].ToString().Trim();
                        cuenta.ApellidoMaterno = dr["ApellidoMaterno"].ToString().Trim();
                        cuenta.Correo = dr["Correo"].ToString().Trim();
                        cuenta.LadaMovil = dr["FIAPLADMOV"].ToString().Trim();
                        cuenta.TelefonoMovil = dr["FIAPNUMMOV"].ToString().Trim();
                        

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Inicio de sesión con éxito.";
                        respuesta.Objeto = cuenta;
                    }
                }
                else
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No existe la cuenta.";
                    respuesta.Objeto = null;
                }
            }

            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No es posible iniciar sesión.";
                respuesta.Objeto = null;

            }            
            return respuesta;
        }
        
    }
}