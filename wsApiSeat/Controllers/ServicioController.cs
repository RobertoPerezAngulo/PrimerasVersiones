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
using System.Text;
using System.Net.Cache;
using wsApiSeat.WsServ;
using System.Web.Script.Services;
using System.Web.Services;
using System.Runtime.Remoting.Contexts;

namespace wsApiSeat.Controllers
{    
    public class ServicioController : ApiController
    {
        
        string UserName = "zlvO7khPpVo=";
        string Password = "HS47sBOr7JQ=";
        byte SistemaId = 1;

        /*Servicio de seguridad SmartIT*/
        string UsrSmartIT = "ARamirez07";
        string PssSmartIT = "ARamirez77";
        //string servidorSeguridadSmartIT = "http://10.5.2.120/";   //Pruebas
        string servidorSeguridadSmartIT = "http://10.5.2.122/";     //Producción
        

        // POST api/Servicio/GetObtenerAsesoresHorariosDisponibles
        [Route("api/Servicio/GetObtenerAsesoresHorariosDisponibles", Name = "GetObtenerAsesoresHorariosDisponibles")]
        public List<HorarioServicio> GetObtenerAsesoresHorariosDisponibles(int Dia, int Mes, int Año, int IdAgencia)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            String FechaCita = $"{Año}-{Mes}-{Dia}";
            int diaFecha = (int)DateTime.Parse(FechaCita).DayOfWeek;


            DateTime Hoy = DateTime.Today;

            string FechaHoy = Hoy.ToString("yyyy-MM-dd");


            string horaActual = DateTime.Now.ToString("HH.mm.ss");
            //string horaActual = "09.00.00";

            string DiaSemana = "";

            switch (diaFecha)
            {
                case 0:

                    DiaSemana = "7";

                    break;

                case 1:

                    DiaSemana = "1";

                    break;

                case 2:

                    DiaSemana = "2";

                    break;

                case 3:

                    DiaSemana = "3";

                    break;

                case 4:

                    DiaSemana = "4";

                    break;

                case 5:

                    DiaSemana = "5";

                    break;

                case 6:

                    DiaSemana = "6";

                    break;
            }


            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEDHOCIA HORAR ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESOR ";
            strSql += "ON ASESOR.FISEIDASES = HORAR.FISEIDASES ";
            strSql += "AND ASESOR.FISEIDCIAU = HORAR.FISEIDCIAU ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASCIT CITA ";
            strSql += "ON HORAR.FISEIDASES = CITA.FISEIDASES ";
            strSql += "AND HORAR.FISEIDCIAU = CITA.FISEIDCIAU ";
            strSql += "AND HORAR.FISEIDDIA = CITA.FISEIDDIA ";
            strSql += "AND CITA.FISESTATUS = 1 ";

            strSql += "LEFT JOIN( ";
            strSql += "SELECT CIT.FISEIDCIAU, DETCIT.FISEIDASES, CIT.FFSEFECINC, CIT.FHSEHRAINC ";
            strSql += "FROM  " + constantes.Ambiente + "SERV.SEDCITAS DETCIT ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEECITAS CIT ";
            strSql += "ON CIT.FISEIDCITA = DETCIT.FISEIDCITA ";
            strSql += "AND CIT.FISEIDCIAU = DETCIT.FISEIDCIAU ";
            strSql += "WHERE DETCIT.FISEIDCIAU = " + IdAgencia;
            strSql += " AND CIT.FFSEFECINC = '" + FechaCita + "' ";

            /*if (FechaCita == FechaHoy)
            {
                strSql += " AND CIT.FHSEHRAINC > '" + horaActual + "' ";
            }*/
            strSql += ") OCUPA ";
            strSql += "ON HORAR.FISEIDCIAU = OCUPA.FISEIDCIAU ";
            strSql += "AND HORAR.FISEIDASES = OCUPA.FISEIDASES ";

            if (FechaCita == FechaHoy)
            {
                strSql += "AND HORAR.FHSERANGOI < '" + horaActual + "' ";
            }
            else
            {
                strSql += "AND HORAR.FHSERANGOI = OCUPA.FHSEHRAINC ";
            }

            strSql += "WHERE HORAR.FISESTATUS = 1 ";
            strSql += "AND HORAR.FISEIDCIAU = " + IdAgencia;
            strSql += " AND HORAR.FISEIDDIA = " + DiaSemana;
            strSql += " AND OCUPA.FISEIDASES IS NULL";


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            //DataView dtX = dt.DefaultView;
            //dtX.Sort = "FSGERAZSOC";
            //dt = dtX.ToTable();

            HorarioServicio horario;
            List<HorarioServicio> coleccionHorarios = new List<HorarioServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                horario = new HorarioServicio();
                horario.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                horario.IdAsesor = dr["FISEIDASES"].ToString().Trim();
                horario.NombreAsesor = dr["FSSENOMBRE"].ToString().Trim();
                horario.ApellidoPaternoAsesor = dr["FSSEAPELPA"].ToString().Trim();
                horario.ApellidoMaternoAsesor = dr["FSSEAPELMA"].ToString().Trim();
                horario.IdDia = dr["FISEIDDIA"].ToString().Trim();
                horario.Rango = dr["FSSEDESRAN"].ToString().Trim();
                horario.HoraInicial = Convert.ToDateTime(dr["FHSERANGOI"]).ToString("HH:mm:ss");
                horario.HoraFinal = Convert.ToDateTime(dr["FHSERANGOF"]).ToString("HH:mm:ss");

                coleccionHorarios.Add(horario);
            }

            return coleccionHorarios;
        }

        // POST api/Servicio/PostObtenerAsesoresxIdAgencia
        [Route("api/Servicio/PostObtenerAsesoresxIdAgencia", Name = "PostObtenerAsesoresxIdAgencia")]
        public List<AsesorServicio> PostObtenerAsesoresxIdAgencia(string IdAgencia)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SECASESO ";
            strSql += "WHERE FISESTATUS = 1 ";
            strSql += "AND FISEIDCIAU = " + IdAgencia;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            //DataView dtX = dt.DefaultView;
            //dtX.Sort = "FSGERAZSOC";
            //dt = dtX.ToTable();

            AsesorServicio asesor;
            List<AsesorServicio> coleccionAsesores = new List<AsesorServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                asesor = new AsesorServicio();
                asesor.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                asesor.IdAsesor = dr["FISEIDASES"].ToString().Trim();
                asesor.NombreAsesor = dr["FSSENOMBRE"].ToString().Trim();
                asesor.ApellidoPaternoAsesor = dr["FSSEAPELPA"].ToString().Trim();
                asesor.ApellidoMaternoAsesor = dr["FSSEAPELMA"].ToString().Trim();

                coleccionAsesores.Add(asesor);
            }

            return coleccionAsesores;
        }

        // POST api/Servicio/PostDeshabilitarNotificacionEncuesta
        [Route("api/Servicio/PostDeshabilitarNotificacionEncuesta", Name = "PostDeshabilitarNotificacionEncuesta")]
        public string PostDeshabilitarNotificacionEncuesta(string IdEncuesta)
        {
            String respuesta = "";

            DVADB.DB2 dbcnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {

                dbcnx.AbrirConexion();
                dbcnx.BeginTransaccion();

                string strSql = "";
                strSql += "UPDATE " + constantes.Ambiente + "APPS.APDNOTST";
                strSql += " SET FIAPSTATUS = 0";
                strSql += " WHERE FIAPIDENPE = " + IdEncuesta;
                dbcnx.SetQuery(strSql);

                dbcnx.CommitTransaccion();
                dbcnx.CerrarConexion();

                respuesta = "SI";

                /*RespuestaServidor respuestaServidor;
                List<RespuestaServidor> coleccionRespuestas = new List<RespuestaServidor>();

                respuestaServidor = new RespuestaServidor();
                respuestaServidor.Respuesta = respuesta;

                coleccionRespuestas.Add(respuestaServidor);

                JavaScriptSerializer jss = new JavaScriptSerializer();
                Context.Response.ContentType = "application/json";
                Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));*/
            }

            catch (Exception e)
            {
                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuesta = e.Message;

                /*RespuestaServidor respuestaServidor;
                List<RespuestaServidor> coleccionRespuestas = new List<RespuestaServidor>();

                respuestaServidor = new RespuestaServidor();
                respuestaServidor.Respuesta = respuesta;

                coleccionRespuestas.Add(respuestaServidor);

                JavaScriptSerializer jss = new JavaScriptSerializer();
                Context.Response.ContentType = "application/json";
                Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));*/

            }

            return respuesta;
        }

        // POST api/Servicio/PostEncuestaPSIClienteDeseaSerContactado
        [Route("api/Servicio/PostEncuestaPSIClienteDeseaSerContactado", Name = "PostEncuestaPSIClienteDeseaSerContactado")]
        public List<RespuestaClienteDeseaSerContactado> PostEncuestaPSIClienteDeseaSerContactado(string IdEncuesta, bool DeseaSerContactado)
        {
            string respuesta = "";
            try
            {

                WsEncuestasPSI.ServicioPSI_APPClient clientePSI = new WsEncuestasPSI.ServicioPSI_APPClient();


                //WsEncuestasPSI.MensajeAgradecimiento mensajeAgradecimiento = clientePSI.GuardarEncuesta(encuestaGuardar);

                WsEncuestasPSI.Mensaje mensaje = clientePSI.GuardaDeseaSerContactado(int.Parse(IdEncuesta), DeseaSerContactado);


                if (mensaje.EsError == false)
                {
                    respuesta = "SI";
                }
                else
                {
                    respuesta = "NO";
                    //mensaje = null;
                }


                RespuestaClienteDeseaSerContactado respuestaGuardar;
                List<RespuestaClienteDeseaSerContactado> coleccionRespuestas = new List<RespuestaClienteDeseaSerContactado>();
                respuestaGuardar = new RespuestaClienteDeseaSerContactado();

                respuestaGuardar.Respuesta = respuesta;
                respuestaGuardar.Mensaje = mensaje;


                coleccionRespuestas.Add(respuestaGuardar);

                return coleccionRespuestas;
            }


            catch (Exception e)
            {


                respuesta = e.Message;

                RespuestaClienteDeseaSerContactado respuestaGuardar;
                List<RespuestaClienteDeseaSerContactado> coleccionRespuestas = new List<RespuestaClienteDeseaSerContactado>();
                respuestaGuardar = new RespuestaClienteDeseaSerContactado();

                respuestaGuardar.Respuesta = respuesta;


                coleccionRespuestas.Add(respuestaGuardar);

                return coleccionRespuestas;

            }
        }

        // POST api/Servicio/PostEnviarCorreoAreaContacto
        [Route("api/Servicio/PostEnviarCorreoAreaContacto", Name = "PostEnviarCorreoAreaContacto")]
        public List<RespuestaServidor> PostEnviarCorreoAreaContacto([FromBody] EnviarCorreoAreaContacto AreaContacto)
        {

            AreaContacto.Texto = AreaContacto.Texto.Replace("[[SPACE]]", " ");
            AreaContacto.NombreCliente = AreaContacto.NombreCliente.Replace("[[SPACE]]", " ");

            String respuesta = "";

            try
            {

                string mailFrom = "notificaciones@grupoautofin.com";
                string password = "RXPJPJJ2013llx";
                string smtpServidor = "smtp.office365.com";
                string mensaje = "A quien corresponda:" + "\n";
                mensaje += AreaContacto.Texto + "\n\n";
                mensaje += AreaContacto.NombreCliente + "\n";

                string subject = "Contacto";

                SmtpClient client = new SmtpClient(smtpServidor, 587);
                MailAddress from = new MailAddress(mailFrom);
                MailAddress to = new MailAddress(AreaContacto.Correo);
                MailMessage message = new MailMessage(from, to);

                message.Body = mensaje;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                client.EnableSsl = true;
                client.Send(message);

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


                respuesta = e.Message;

                RespuestaServidor respuestaServidor;
                List<RespuestaServidor> coleccionRespuestas = new List<RespuestaServidor>();

                respuestaServidor = new RespuestaServidor();
                respuestaServidor.Respuesta = respuesta;

                coleccionRespuestas.Add(respuestaServidor);

                return coleccionRespuestas;

            }
        }

        // POST api/Servicio/PostEnviarCorreoContactoDeCliente
        [Route("api/Servicio/PostEnviarCorreoContactoDeCliente", Name = "PostEnviarCorreoContactoDeCliente")]
        public Respuesta PostEnviarCorreoContactoDeCliente([FromBody] EnviarCorreoContactoCliente CorreoContactoCliente)
        {

            Respuesta respuesta = new Respuesta();


            try
            {

                CorreoContactoCliente.Texto = CorreoContactoCliente.Texto.Replace("[[SPACE]]", " ");
                CorreoContactoCliente.NombreCliente = CorreoContactoCliente.NombreCliente.Replace("[[SPACE]]", " ");


                //string mailFrom = "soportegarage@cupragarage.com.mx";
                //string password = "Cupra.2020";
                //string smtpServidor = "mail.cupragarage.com.mx";
                //string mailFrom = "jrperez@grupoautofin.com";
                //string password = "Pasword01";
                //string smtpServidor = "smtp-mail.outlook.com";
                string mailFrom = "notificaciones@grupoautofin.com";
                string password = "RXPJPJJ2013llx";
                string smtpServidor = "smtp.office365.com";
                string mensaje = "A quien corresponda:" + "\n";
                mensaje += CorreoContactoCliente.Texto + "\n\n";
                mensaje += CorreoContactoCliente.NombreCliente + "\n";

                string subject = "Contacto";

                SmtpClient client = new SmtpClient(smtpServidor, 587);
                MailAddress from = new MailAddress(mailFrom);
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(CorreoContactoCliente.CorreoContacto));
                message.To.Add(new MailAddress(CorreoContactoCliente.CorreoCliente));
                message.From = from;

                message.Body = mensaje;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                client.EnableSsl = true;
                client.Send(message);


                respuesta.Ok = "SI";
                respuesta.Mensaje = "Correo enviado satisfactoriamente.";
                respuesta.Objeto = "";

                return respuesta;
            }

            catch (Exception ex)
            {
                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo enviar el correo.";
                respuesta.Objeto = "";

                return respuesta;

            }
        }

        // POST api/Servicio/PostEnviarNotificacionXIdsPersonas
        [Route("api/Servicio/PostEnviarNotificacionXIdsPersonas", Name = "PostEnviarNotificacionXIdsPersonas")]
        public string PostEnviarNotificacionXIdsPersonas(List<NotificacionEncuesta> notificacionesEncuestas)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "";
            string mensaje = "";
            int EncuestasEnviadas = 0;

            /*List<NotificacionEncuesta> notificacionesEncuestas = new List<NotificacionEncuesta>();

            NotificacionEncuesta notificacionEjemplo = new NotificacionEncuesta();

            notificacionEjemplo.IdPersona = "322873";
            notificacionEjemplo.IdEncuesta = "1";
            notificacionEjemplo.Titulo = "Ejemplo";
            notificacionEjemplo.Mensaje = "Mensaje Ejemplo";
            notificacionEjemplo.IdAgencia = "98";

            notificacionesEncuestas.Add(notificacionEjemplo);*/

            foreach (NotificacionEncuesta notificacion in notificacionesEncuestas)
            {
                string IdPersona = notificacion.IdPersona;
                string IdEncuesta = notificacion.IdEncuesta;
                string Titulo = notificacion.Titulo;
                string Mensaje = notificacion.Mensaje;
                string IdAgencia = notificacion.IdAgencia;

                string strSql = "";
                strSql = "SELECT * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
                strSql += "WHERE FIAPSTATUS = 1 ";
                strSql += "AND FIAPIDPERS = " + IdPersona;


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    string IdCuenta = dt.Rows[0]["FIAPIDCUEN"].ToString().Trim();

                    string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionApp(IdCuenta, Mensaje, Titulo, "");

                    try
                    {

                        //  Inicia transaccion
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
                        strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDCIAU, FIAPIDPREO, FIAPAPLENC, FIAPIDENPE, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdCuenta + ",";
                        strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Mensaje + "',";
                        strSql += "0,";
                        strSql += IdAgencia + ",";
                        strSql += "default,";
                        strSql += "1,";
                        strSql += IdEncuesta + ",";
                        strSql += "1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        EncuestasEnviadas++;


                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        //dbCnx.CerrarConexion();

                        respuesta = "NO";
                        mensaje = "Fallo al enviar notificaciones de encuestas. (" + ex.Message + ")";
                    }


                    dbCnx.CommitTransaccion();

                }




            }

            dbCnx.CerrarConexion();

            respuesta = "SI";
            mensaje = "" + EncuestasEnviadas + " encuesta(s) enviada(s) correctamente.";



            return mensaje;
        }

        public class GenerarCitaBody
        {
            public string IdAgencia;
            public string IdPersona;
            public string IdAsesor;
            public string IdVehiculo;
            public string IdMarca;
            public string Usuario;
            public string Programa;
            public string FechaI;
            public string FechaF;
            public string HoraI;
            public string HoraF;
            public string Observaciones;
            public string Anio;
            public string IdModelo;
            public string Modelo;
            public string IdColor;
            public string Color;
            public string Placa;
            public string Permiso;
            public string NumSerie;
            public string TipoCita;
        }

        // POST api/Servicio/PostGenerarCitaServicio
        [Route("api/Servicio/PostGenerarCitaServicio", Name = "PostGenerarCitaServicio")]
        public RespuestaTest<GenerarCitaBody> PostGenerarCitaServicio([FromBody]GenerarCitaBody body)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();


            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "WHERE VEHIC.FIANSTATU = 1 ";
            strSql += "AND FNVEHICID = " + body.IdVehiculo;

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            string km = dt.Rows[0]["FIANODOMET"].ToString().Trim();
            int intKm = 0;
            int.TryParse(km, out intKm);

            strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SECGPCIT GPCIT ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECTIPOP TIPOP ";
            strSql += "ON	GPCIT.FISEIDCIAU = TIPOP.FISEIDCIAU ";
            strSql += "AND 	GPCIT.FISEIDTIPP = TIPOP.FISEIDTIPP ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEDRPRPR RPR ";
            strSql += "ON	GPCIT.FISEIDCIAU = RPR.FISEIDCIAU ";
            strSql += "AND 	GPCIT.FISEIDTIPP = RPR.FISETIPPRE ";
            strSql += "AND 	TIPOP.FISEIDTIPP = RPR.FISEIDTIPP ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECTIPRE TIPRE ";
            strSql += "ON	RPR.FISEIDCIAU = TIPRE.FISEIDCIAU ";
            strSql += "AND 	RPR.FISETIPPRE = TIPRE.FISETIPPRE ";

            strSql += "WHERE GPCIT.FISESTATUS = 1 ";
            strSql += "AND GPCIT.FISEAPLAMA = 1 ";
            strSql += "AND GPCIT.FISEIDCIAU = " + body.IdAgencia + " ";
            strSql += "AND GPCIT.FSSEDESGCI LIKE '%" + body.TipoCita + "%'";

            DataTable dtT = dbCnx.GetDataSet(strSql).Tables[0];

            string idTipoPreorden = dtT.Rows[0]["FISEIDTIPP"].ToString().Trim();
            string idTipoPrecio = dtT.Rows[0]["FISETIPPRE"].ToString().Trim();
            string TipoPreorden = dtT.Rows[0]["FSSEDESTPR"].ToString().Trim();
            string TipoPrecio = dtT.Rows[0]["FSSEDESTPR"].ToString().Trim();
            string IdTipoCita = dtT.Rows[0]["FISEIDGCIT"].ToString().Trim();

            body.Observaciones = body.Observaciones.Replace("[[SPACE]]", " ");
            body.Modelo = body.Modelo.Replace("[[SPACE]]", " ");
            body.TipoCita = body.TipoCita.Replace("[[SPACE]]", " ");

            int idModu = 1;
            int idMovi = 4;
            int idTico = 3;
            int idEsta = 1;

            try
            {
              

                wsServicioClient client = new wsServicioClient();

                var res = client.GuardaCitaConPreorden(
                    int.Parse(body.IdAgencia), int.Parse(body.IdPersona), int.Parse(body.IdAsesor),
                     int.Parse(body.IdVehiculo), int.Parse(body.IdMarca), body.Usuario, body.Programa, body.Observaciones,
                     body.FechaI, body.HoraI, body.FechaF, body.HoraF, body.Observaciones, idModu, idMovi, idTico,
                     idEsta, int.Parse(body.Anio), int.Parse(body.IdModelo), body.Modelo, int.Parse(body.IdColor),
                     body.Color, body.Placa, body.Permiso, IdTipoCita, intKm, TipoPreorden, int.Parse(idTipoPrecio),
                     TipoPrecio, 0, 0, 0);


                
                string[] strSplit = res.Split(',');

                res = strSplit[0];

                Decimal folio = 0;

                folio = decimal.Parse(res);
                res = folio.ToString();
                
                //client.GuardaCitaConPreordenAsync(
                //    int.Parse(body.IdAgencia), int.Parse(body.IdPersona), int.Parse(body.IdAsesor),
                //     int.Parse(body.IdVehiculo), int.Parse(body.IdMarca), body.Usuario, body.Programa, body.Observaciones,
                //     body.FechaI, body.HoraI, body.FechaF, body.HoraF, body.Observaciones, idModu, idMovi, idTico,
                //     idEsta, int.Parse(body.Anio), int.Parse(body.IdModelo), body.Modelo, int.Parse(body.IdColor),
                //     body.Color, body.Placa, body.Permiso, IdTipoCita, intKm, TipoPreorden, int.Parse(idTipoPrecio),
                //     TipoPrecio, 0, 0, 0
                //    );


                // client.GuardaCitaConPreordenCompleted += prueba;


                //WsServ.GuardaCitaConPreordenRequest guardarCita = new WsServ.GuardaCitaConPreordenRequest(int.Parse(IdAgencia), int.Parse(IdPersona), int.Parse(IdAsesor),
                //     int.Parse(IdVehiculo), int.Parse(IdMarca), Usuario, Programa, Observaciones, FechaI, HoraI, FechaF, HoraF, Observaciones, idModu, idMovi, idTico,
                //     idEsta, int.Parse(Anio), int.Parse(IdModelo), Modelo, int.Parse(IdColor), Color, Placa, Permiso, IdTipoCita, intKm, TipoPreorden, int.Parse(idTipoPrecio), TipoPrecio, 0, 0, 0);


                //cliente.GuardaCitaConPreordenAsync(guardarCita);

                //cliente.GuardaCitaConPreordenCompleted += prueba;


                RespuestaTest<GenerarCitaBody> respuestaServidor;
                respuestaServidor = new RespuestaTest<GenerarCitaBody>();
                respuestaServidor.Ok = "SI";
                respuestaServidor.Mensaje = res;
                
                return respuestaServidor;
            }
            catch (Exception e)
            {
                RespuestaTest<GenerarCitaBody> respuestaServidor;
                respuestaServidor = new RespuestaTest<GenerarCitaBody>();
                respuestaServidor.Ok = "NO";
                respuestaServidor.Mensaje = e.Message;

                return respuestaServidor;
            }


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        private void prueba(object sender, WsServ.GuardaCitaConPreordenCompletedEventArgs e)
        {

            string respuesta = "";
            string mensaje = "";
            try
            {
                //respuesta = e.Result.GuardaCitaConPreordenResult.ToString();
                respuesta = e.Result;//.GuardaCitaConPreordenResult.ToString();

                string[] strSplit = respuesta.Split(',');

                respuesta = strSplit[0];

                Decimal folio = 0;

                folio = decimal.Parse(respuesta);
                respuesta = folio.ToString();

                mensaje = "SI";

            }
            catch (Exception ex)
            {
                respuesta = "No se pudo generar cita";
                mensaje = "NO";
            }


            RespuestaServidorGenerica respuestaServidor;
            List<RespuestaServidorGenerica> coleccionRespuestas = new List<RespuestaServidorGenerica>();

            respuestaServidor = new RespuestaServidorGenerica();
            respuestaServidor.Respuesta = respuesta;
            respuestaServidor.Mensaje = mensaje;

            coleccionRespuestas.Add(respuestaServidor);

            JavaScriptSerializer jss = new JavaScriptSerializer();

           // Response.ContentType = "application/json";
         //   Response.Output.Write(jss.Serialize(coleccionRespuestas));

            
        }

        public class GenerarCitaServicioSimpleBody
        {
            public int IdAgencia; public int IdAsesor; public string FechaI; public string FechaF; public string HoraI; public string HoraF; public string Observaciones; public string Placa; public string Lada; public string Telefono; public string TipoCita; public string Usuario; public string Programa;
        }

        // POST api/Servicio/PostGenerarCitaServicioSimple
        [Route("api/Servicio/PostGenerarCitaServicioSimple", Name = "PostGenerarCitaServicioSimple")]
        public RespuestaTest<GenerarCitaServicioSimpleBody> PostGenerarCitaServicioSimple([FromBody] GenerarCitaServicioSimpleBody body)
        {
            body.Observaciones += " Lada: " + body.Lada + " Teléfono: " + body.Telefono;

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SECGPCIT GPCIT ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECTIPOP TIPOP ";
            strSql += "ON	GPCIT.FISEIDCIAU = TIPOP.FISEIDCIAU ";
            strSql += "AND 	GPCIT.FISEIDTIPP = TIPOP.FISEIDTIPP ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEDRPRPR RPR ";
            strSql += "ON	GPCIT.FISEIDCIAU = RPR.FISEIDCIAU ";
            strSql += "AND 	GPCIT.FISEIDTIPP = RPR.FISETIPPRE ";
            strSql += "AND 	TIPOP.FISEIDTIPP = RPR.FISEIDTIPP ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECTIPRE TIPRE ";
            strSql += "ON	RPR.FISEIDCIAU = TIPRE.FISEIDCIAU ";
            strSql += "AND 	RPR.FISETIPPRE = TIPRE.FISETIPPRE ";

            strSql += "WHERE GPCIT.FISESTATUS = 1 ";
            strSql += "AND GPCIT.FISEAPLAMA = 1 ";
            strSql += "AND GPCIT.FISEIDCIAU = " + body.IdAgencia + " ";
            strSql += "AND GPCIT.FSSEDESGCI LIKE '%" + body.TipoCita + "%'";

            DataTable dtT = dbCnx.GetDataSet(strSql).Tables[0];

            string idTipoPreorden = dtT.Rows[0]["FISEIDTIPP"].ToString().Trim();
            string idTipoPrecio = dtT.Rows[0]["FISETIPPRE"].ToString().Trim();
            string TipoPreorden = dtT.Rows[0]["FSSEDESTPR"].ToString().Trim();
            string TipoPrecio = dtT.Rows[0]["FSSEDESTPR"].ToString().Trim();
            string IdTipoCita = dtT.Rows[0]["FISEIDGCIT"].ToString().Trim();

            body.Observaciones = body.Observaciones.Replace("[[SPACE]]", " ");
            body.TipoCita = body.TipoCita.Replace("[[SPACE]]", " ");

            int idModu = 1;
            int idMovi = 4;
            int idTico = 3;
            int idEsta = 1;

            RespuestaTest<GenerarCitaServicioSimpleBody> respuesta = new RespuestaTest<GenerarCitaServicioSimpleBody>();

            try
            {

                wsApiSeat.WsServ.wsServicioClient client = new wsApiSeat.WsServ.wsServicioClient();

                client.GuardaCitaSimple(body.IdAgencia, -1, body.IdAsesor, 0, 0, body.Usuario, body.Programa, "", body.FechaI, body.HoraI, body.FechaF, body.HoraF, body.Observaciones, idModu, idMovi, idTico,
                  idEsta, 0, 0, "", 0, "", body.Placa, "", "", IdTipoCita);
                

                ////WsServ.GuardaCitaSimpleRequest guardarCita = new WsServ.GuardaCitaSimpleRequest(IdAgencia, -1, IdAsesor, 0, 0, Usuario, Programa, "", FechaI, HoraI, FechaF, HoraF, Observaciones, idModu, idMovi, idTico,
                ////     idEsta, 0, 0, "", 0, "", Placa, "", "", IdTipoCita);
                ////
                ////cliente.GuardaCitaSimpleAsync(guardarCita);
                ////cliente.GuardaCitaSimpleCompleted += seCompletoGuardaCita;
                respuesta.Ok = "SI";

            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message;
            }
            return respuesta;
        }

        // POST api/Servicio/PostAppAsesorObtenerCotizacionesPreOrden
        [Route("api/Servicio/PostAppAsesorObtenerCotizacionesPreOrden", Name = "PostAppAsesorObtenerCotizacionesPreOrden")]
        public List<CotizacionPreorden> PostAppAsesorObtenerCotizacionesPreOrden(string IdPreorden, string IdAgencia)
        {
            string respuesta = "";

            string Usr = UsrSmartIT;
            string Pwd = PssSmartIT;
            //string Pwd = "Sony";
            string method = "POST";
            string body = string.Empty;
            string address = "";
            string parameters = "";
            string cookieHeader = "";


            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT FSGEVALOR ";
            strSql += "FROM	" + constantes.Ambiente + "GRAL.GEDPARAM ";
            strSql += "WHERE FIGESTATUS = 1 ";
            strSql += "AND FIGEINDIP = 6 ";
            strSql += "AND FIGEIDCIAU = " + IdAgencia;



            DataTable dtx = dbCnx.GetDataSet(strSql).Tables[0];

            float ValorIva = float.Parse(dtx.Rows[0]["FSGEVALOR"].ToString());

            address = "api/segu/IniciarSesion/";
            parameters = Usr + "/" + Pwd;

            var request = (HttpWebRequest)WebRequest.Create(servidorSeguridadSmartIT + address + parameters);
            request.Method = method;
            //request.Credentials = new NetworkCredential("demo@leankit.com", "demopassword");
            request.PreAuthenticate = true;

            if (method == "POST")
            {
                if (!string.IsNullOrEmpty(body))
                {
                    var requestBody = Encoding.UTF8.GetBytes(body);
                    request.ContentLength = requestBody.Length;
                    request.ContentType = "application/json";
                    //request.CookieContainer.GetCookieHeader
                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(requestBody, 0, requestBody.Length);
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }
            }

            request.Timeout = 150000;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            string output = string.Empty;
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                    {
                        output = stream.ReadToEnd();
                        cookieHeader = response.Headers[HttpResponseHeader.SetCookie];

                    }

                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    using (var stream = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        output = stream.ReadToEnd();
                    }
                }
                else if (ex.Status == WebExceptionStatus.Timeout)
                {
                    output = "Request timeout is expired.";
                }
            }
            respuesta = output;
            if (!string.IsNullOrEmpty(cookieHeader))
            {
                address = "serv/cotizaciontotalpreorden/ObtenerCotizaciontotalPorPreorden/";
                parameters = IdAgencia + "/" + IdPreorden + "/1";
                method = "GET";

                request = (HttpWebRequest)WebRequest.Create(servidorSeguridadSmartIT + address + parameters);
                request.Method = method;
                //request.Credentials = new NetworkCredential("demo@leankit.com", "demopassword");
                CookieContainer cookiecontainer = new CookieContainer();
                string[] cookies = cookieHeader.Split(';');

                cookiecontainer.SetCookies(new Uri(servidorSeguridadSmartIT), cookies[0]);

                request.CookieContainer = cookiecontainer;
                request.PreAuthenticate = true;

                if (method == "GET")
                {
                    if (!string.IsNullOrEmpty(body))
                    {
                        var requestBody = Encoding.UTF8.GetBytes(body);
                        request.ContentLength = requestBody.Length;
                        request.ContentType = "application/json";
                        //request.CookieContainer.GetCookieHeader


                        using (var requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(requestBody, 0, requestBody.Length);
                        }
                    }
                    else
                    {
                        request.ContentLength = 0;
                    }
                }

                request.Timeout = 150000;
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

                output = string.Empty;
                try
                {
                    using (var response = request.GetResponse())
                    {
                        using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                        {
                            output = stream.ReadToEnd();
                            //cookieHeader = response.Headers[HttpResponseHeader.SetCookie];
                        }

                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        using (var stream = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            output = stream.ReadToEnd();
                        }
                    }
                    else if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        output = "Request timeout is expired.";
                    }
                }
                respuesta = output;

                //respuesta = respuesta.Replace('\"', '"');
                respuesta = respuesta.Replace("\\\"", "");
                //respuesta = "[" + respuesta + "]";

                if (!respuesta.Contains("error"))
                {

                    Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(respuesta);

                    string Cotizacion = json.GetValue("Cotizacion").ToString();

                    string CotizacionDetalle = json.GetValue("CotizacionDetalle").ToString().Replace("[]", "[{}]");
                    string CotizacionOperaciones = json.GetValue("CotizacionOperaciones").ToString().Replace("[]", "[{}]");
                    string CotizacionRefacciones = json.GetValue("CotizacionRefacciones").ToString().Replace("[]", "[{}]");
                    Newtonsoft.Json.Linq.JArray jsonDetalle = Newtonsoft.Json.Linq.JArray.Parse(CotizacionDetalle);

                    DataTable dtCotizacion = JsonConvert.DeserializeObject<DataTable>("[" + Cotizacion + "]");

                    string Id = dtCotizacion.Rows[0]["Id"].ToString().Trim();

                    DataTable dtCotizacionDetalle = JsonConvert.DeserializeObject<DataTable>(CotizacionDetalle);

                    DetalleCotizacion detalleCotizacion;
                    List<DetalleCotizacion> listadoDetalleCotizacion = new List<DetalleCotizacion>();

                    DataTable dtCotizacionOperaciones = JsonConvert.DeserializeObject<DataTable>(CotizacionOperaciones);
                    DataTable dtCotizacionRefacciones = JsonConvert.DeserializeObject<DataTable>(CotizacionRefacciones);

                    foreach (DataRow dr in dtCotizacionDetalle.Rows)
                    {
                        ConceptosCotizacion conceptosCotizacion;
                        List<ConceptosCotizacion> listadoConceptosCotizacion = new List<ConceptosCotizacion>();

                        detalleCotizacion = new DetalleCotizacion();

                        string IdDetalleCotizacion = dr["IdDetalleCotizacion"].ToString().Trim();

                        detalleCotizacion.IdDetalle = IdDetalleCotizacion;
                        detalleCotizacion.Fecha = Convert.ToDateTime(dr["Fecha"]).ToString("dd-MM-yyyy");



                        foreach (DataRow drow in dtCotizacionOperaciones.Rows)
                        {
                            if (drow.Table.Columns.Contains("IdDetalleCotizacion"))
                            {
                                string IdDetalle = drow["IdDetalleCotizacion"].ToString().Trim();

                                if (IdDetalleCotizacion == IdDetalle)
                                {
                                    conceptosCotizacion = new ConceptosCotizacion();

                                    conceptosCotizacion.IdDetalle = IdDetalle;
                                    conceptosCotizacion.IdConcepto = drow["IdOperacion"].ToString().Trim();
                                    conceptosCotizacion.TipoConcepto = "Operacion";
                                    conceptosCotizacion.Concepto = drow["Operacion"].ToString().Trim().Replace("/", " ").Replace(":", " ");

                                    string NumeroUnidades = drow["NumeroUnidades"].ToString().Trim();

                                    if (NumeroUnidades != "")
                                    {
                                        NumeroUnidades = float.Parse(drow["NumeroUnidades"].ToString().Trim()).ToString("####0.00");
                                    }
                                    else
                                    {
                                        NumeroUnidades = (float.Parse("0")).ToString("####0.00");
                                    }

                                    string PrecioVentaPorUT = drow["PrecioVentaPorUT"].ToString().Trim();

                                    if (PrecioVentaPorUT != "")
                                    {
                                        PrecioVentaPorUT = float.Parse(drow["PrecioVentaPorUT"].ToString().Trim()).ToString("####0.00");
                                    }
                                    else
                                    {
                                        PrecioVentaPorUT = (float.Parse("0")).ToString("####0.00");
                                    }

                                    conceptosCotizacion.Precio = (float.Parse(NumeroUnidades) * float.Parse(PrecioVentaPorUT)).ToString("####0.00");
                                    listadoConceptosCotizacion.Add(conceptosCotizacion);
                                }
                            }
                        }

                        foreach (Newtonsoft.Json.Linq.JObject objeto in jsonDetalle)
                        {
                            string CotizacionTOTS = objeto.GetValue("TOTS").ToString();
                            DataTable dtCotizacionTOTS = JsonConvert.DeserializeObject<DataTable>(CotizacionTOTS);

                            foreach (DataRow drow in dtCotizacionTOTS.Rows)
                            {
                                if (drow.Table.Columns.Contains("IdDcot"))
                                {
                                    string IdDetalle = drow["IdDcot"].ToString().Trim();

                                    if (IdDetalleCotizacion == IdDetalle)
                                    {
                                        conceptosCotizacion = new ConceptosCotizacion();

                                        conceptosCotizacion.IdDetalle = IdDetalle;
                                        conceptosCotizacion.IdConcepto = drow["IdOperacion"].ToString().Trim();
                                        conceptosCotizacion.TipoConcepto = "Trabajo Otros Talleres";
                                        conceptosCotizacion.Concepto = drow["Operacion"].ToString().Trim().Replace("/", " ").Replace(":", " ");

                                        string NumeroUnidades = drow["NumeroUnidades"].ToString().Trim();

                                        if (NumeroUnidades != "")
                                        {
                                            NumeroUnidades = float.Parse(drow["NumeroUnidades"].ToString().Trim()).ToString("####0.00");
                                        }
                                        else
                                        {
                                            NumeroUnidades = (float.Parse("0")).ToString("####0.00");
                                        }

                                        string PrecioVentaPorUT = drow["PrecioVenta"].ToString().Trim();

                                        if (PrecioVentaPorUT != "")
                                        {
                                            PrecioVentaPorUT = float.Parse(drow["PrecioVenta"].ToString().Trim()).ToString("####0.00");
                                        }
                                        else
                                        {
                                            PrecioVentaPorUT = (float.Parse("0")).ToString("####0.00");
                                        }

                                        conceptosCotizacion.Precio = (float.Parse(NumeroUnidades) * float.Parse(PrecioVentaPorUT)).ToString("####0.00");
                                        listadoConceptosCotizacion.Add(conceptosCotizacion);
                                    }
                                }
                            }

                        }

                        foreach (DataRow drow in dtCotizacionRefacciones.Rows)
                        {
                            //conceptosCotizacion = new ConceptosCotizacion();
                            if (drow.Table.Columns.Contains("IdDetalleCotizacion"))
                            {


                                string IdDetalle = drow["IdDetalleCotizacion"].ToString().Trim();

                                if (IdDetalleCotizacion == IdDetalle)
                                {
                                    conceptosCotizacion = new ConceptosCotizacion();

                                    conceptosCotizacion.IdDetalle = IdDetalle;
                                    conceptosCotizacion.IdConcepto = drow["IdParte"].ToString().Trim();
                                    conceptosCotizacion.TipoConcepto = "Refaccion";
                                    conceptosCotizacion.Concepto = drow["DescripcionDeParte"].ToString().Trim().Replace("/", " ").Replace(":", " ");

                                    string NumeroUnidades = drow["Cantidad"].ToString().Trim();

                                    if (NumeroUnidades != "")
                                    {
                                        NumeroUnidades = float.Parse(drow["Cantidad"].ToString().Trim()).ToString("####0.00");
                                    }
                                    else
                                    {
                                        NumeroUnidades = (float.Parse("0")).ToString("####0.00");
                                    }

                                    string PrecioVentaPorUT = drow["PrecioVentaConDescuento"].ToString().Trim();

                                    if (PrecioVentaPorUT != "")
                                    {
                                        PrecioVentaPorUT = float.Parse(drow["PrecioVentaConDescuento"].ToString().Trim()).ToString("####0.00");
                                    }
                                    else
                                    {
                                        PrecioVentaPorUT = (float.Parse("0")).ToString("####0.00");
                                    }

                                    conceptosCotizacion.Precio = (float.Parse(NumeroUnidades) * float.Parse(PrecioVentaPorUT)).ToString("####0.00");

                                    listadoConceptosCotizacion.Add(conceptosCotizacion);
                                }
                            }

                        }

                        detalleCotizacion.Conceptos = listadoConceptosCotizacion;
                        float total = 0;
                        for (int x = 0; x < detalleCotizacion.Conceptos.Count(); x++)
                        {
                            total += float.Parse(detalleCotizacion.Conceptos[x].Precio);
                        }

                        string IVA = "1." + ValorIva;
                        total = total * float.Parse(IVA);

                        detalleCotizacion.TotalCotizacion = total.ToString("####0.00");

                        listadoDetalleCotizacion.Add(detalleCotizacion);
                    }





                    CotizacionPreorden cotizacionPreorden;
                    List<CotizacionPreorden> listadoCotizacionPreorden = new List<CotizacionPreorden>();

                    List<DetalleCotizacionTotales> IdsDetalle = new List<DetalleCotizacionTotales>();
                    DetalleCotizacionTotales detalleCotizacionTotales;

                    /*for (int x = 0; x < listadoConceptosCotizacion.Count(); x++)
                    {
                        detalleCotizacionTotales = new DetalleCotizacionTotales();

                        detalleCotizacionTotales.IdDetalle = listadoConceptosCotizacion[x].IdDetalle;
                        detalleCotizacionTotales.Total = listadoConceptosCotizacion[x].Precio;

                        IdsDetalle.Add(detalleCotizacionTotales);
                    }

                    IdsDetalle  = IdsDetalle.OrderBy(o => o.IdDetalle).ToList();

                    List<String> TotalesIdsDetalle = new List<string>();

                    for (int x = 0; x < IdsDetalle.Count; x++)
                    {
                        string IdDetalle = IdsDetalle[x].IdDetalle;
                        string Total = IdsDetalle[x].Total;

                        if (x == 0)
                        {
                            TotalesIdsDetalle.Add(Total);
                        }
                        else
                        {
                            if (IdDetalle == IdsDetalle[x - 1].IdDetalle)
                            {
                                TotalesIdsDetalle[TotalesIdsDetalle.Count - 1] = (float.Parse(IdsDetalle[IdsDetalle.Count - 1].Total)).ToString("####0.00") + (float.Parse(Total)).ToString("####0.00");
                            }
                            else
                            {
                                TotalesIdsDetalle.Add(Total);
                            }
                        }

                    }

                    for (int x = 0; x < listadoDetalleCotizacion.Count(); x++)
                    {
                        listadoDetalleCotizacion[x].TotalCotizacion = TotalesIdsDetalle[x];
                    }*/

                    cotizacionPreorden = new CotizacionPreorden();

                    cotizacionPreorden.Id = Id;
                    cotizacionPreorden.IdAgencia = IdAgencia;
                    cotizacionPreorden.IdPreorden = IdPreorden;
                    cotizacionPreorden.Detalle = listadoDetalleCotizacion;
                    //cotizacionPreorden.Conceptos = listadoConceptosCotizacion;

                    listadoCotizacionPreorden.Add(cotizacionPreorden);

                    return listadoCotizacionPreorden;
                }
                else
                {
                    List<CotizacionPreorden> listadoCotizacionPreorden = new List<CotizacionPreorden>();
                    return listadoCotizacionPreorden;
                }

            }
            else
            {
                List<CotizacionPreorden> listadoCotizacionPreorden = new List<CotizacionPreorden>();
                return listadoCotizacionPreorden;
            }
        }

        // POST api/Servicio/GetAppAsesorObtenerCotizacionesSeguimiento
        [Route("api/Servicio/GetAppAsesorObtenerCotizacionesSeguimiento", Name = "GetAppAsesorObtenerCotizacionesSeguimiento")]
        public List<CotizacionSeguimiento> GetAppAsesorObtenerCotizacionesSeguimiento(string IdSeguimiento)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDCOTMB COTMB ";
            strSql += "INNER JOIN " + constantes.Ambiente + "APPS.APDSCOMB SCOMB ";
            strSql += "ON COTMB.FIAPIDSEGO = SCOMB.FIAPIDSEGO ";
            strSql += "AND  COTMB.FIAPIDDETA = SCOMB.FIAPIDDETA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "APPS.APDSEGMB SEGMB ";
            strSql += "ON SEGMB.FIAPIDSEGO = SCOMB.FIAPIDSEGO ";
            strSql += "WHERE COTMB.FIAPSTATUS = 1 ";
            strSql += "AND COTMB.FIAPIDSEGO = " + IdSeguimiento;

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDSCOMB SCOMB ";
            strSql += "WHERE SCOMB.FIAPSTATUS = 1 ";
            strSql += "AND SCOMB.FIAPIDSEGO = " + IdSeguimiento;

            DataTable dtA = dbCnx.GetDataSet(strSql).Tables[0];

            CotizacionSeguimiento cotizacionSeguimiento;
            List<CotizacionSeguimiento> listadoCotizacionesSeguimiento = new List<CotizacionSeguimiento>();

            DetalleCotizacionSeguimiento Detalle;
            List<DetalleCotizacionSeguimiento> coleccionCotizaciones = new List<DetalleCotizacionSeguimiento>();

            ConceptosCotizacionSeguimiento Concepto;
            List<ConceptosCotizacionSeguimiento> coleccionConceptos = new List<ConceptosCotizacionSeguimiento>();

            cotizacionSeguimiento = new CotizacionSeguimiento();

            if (dtA.Rows.Count > 0)
            {

                cotizacionSeguimiento.IdSegOrden = dt.Rows[0]["FIAPIDSEGO"].ToString().Trim();
                cotizacionSeguimiento.Id = dt.Rows[0]["FIAPIDCOTI"].ToString().Trim();
                cotizacionSeguimiento.IdAgencia = dt.Rows[0]["FIAPIDCIAU"].ToString().Trim();
                cotizacionSeguimiento.IdPreorden = dt.Rows[0]["FIAPIDPREO"].ToString().Trim();

                foreach (DataRow dr in dtA.Rows)
                {
                    coleccionConceptos = new List<ConceptosCotizacionSeguimiento>();
                    Detalle = new DetalleCotizacionSeguimiento();

                    string IdDetalle = dr["FIAPIDDETA"].ToString().Trim();
                    Detalle.IdDetalle = IdDetalle;
                    Detalle.Fecha = Convert.ToDateTime(dr["FFAPFECHA"]).ToString("dd-MM-yyyy");
                    Detalle.TotalCotizacion = dr["FIAPPRECIO"].ToString().Trim();
                    Detalle.Autorizado = dr["FIAPAUTORI"].ToString().Trim();

                    foreach (DataRow drow in dt.Rows)
                    {
                        string IdDetalleConcepto = drow["FIAPIDDETA"].ToString().Trim();
                        if (IdDetalle.Contains(IdDetalleConcepto))
                        {
                            Concepto = new ConceptosCotizacionSeguimiento();

                            Concepto.IdDetalle = IdDetalleConcepto;
                            Concepto.IdConcepto = drow["FIAPIDCONC"].ToString().Trim();
                            Concepto.TipoConcepto = drow["FSAPTIPCON"].ToString().Trim();
                            Concepto.Concepto = drow["FSAPCONCEP"].ToString().Trim();
                            Concepto.Precio = drow["FIAPPRECIO"].ToString().Trim();

                            coleccionConceptos.Add(Concepto);
                        }
                    }
                    Detalle.Conceptos = coleccionConceptos;

                    coleccionCotizaciones.Add(Detalle);
                }

                cotizacionSeguimiento.Detalle = coleccionCotizaciones;

                listadoCotizacionesSeguimiento.Add(cotizacionSeguimiento);
            }

            return listadoCotizacionesSeguimiento;
        }

        // POST api/Servicio/PostAppAsesorObtenerDatosAsesor
        [Route("api/Servicio/PostAppAsesorObtenerDatosAsesor", Name = "PostAppAsesorObtenerDatosAsesor")]
        public List<DatosAsesor> PostAppAsesorObtenerDatosAsesor(string Usr, string Pwd)
        {

            String Respuesta = "SI";

            WS_Security.AuthenticationHeader header = new WS_Security.AuthenticationHeader();
            WS_Security.WS_SecuritySoapClient WS_Security = new WS_Security.WS_SecuritySoapClient();

            //Cabecero de seguridad
            //header.Password = "+LSjzpdjQpxFn312GJNamA=="; 20190125
            //header.UserName = "8MBBkarzpuw=";
            //header.SystemId = 17;
            //header.Password = "AU8T2KkG/Y8AYydtd85kiw==";
            //header.UserName = "s802fF0yEiUCdq7lt8DiJQ==";
            //header.SystemId = 21;
            /*Usuario de seguridad propio para la app*/
            header.UserName = UserName;
            header.Password = Password;
            header.SystemId = SistemaId;

            WS_Security = new WS_Security.WS_SecuritySoapClient();

            byte systemId = 1;

            try
            {

                int IdUsuario = WS_Security.GetUserByLogin(header, systemId, Usr, Pwd).Id;
                //int IdAgencia = WS_Security.GetUserByLogin(header, systemId, Usr, Pwd).Company.Id;

                DataTable dt = null;

                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();


                string strsql = "";
                strsql += "SELECT * ";
                strsql += " FROM " + constantes.Ambiente + "SEGU.SGCUSUAR USUAR ";
                strsql += " INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO";
                //strsql += " ON USUAR.FSSGNOMBRE = ASESO.FSSENOMBRE";
                //strsql += " AND USUAR.FSSGAPEPAT = ASESO.FSSEAPELPA";
                //strsql += " AND USUAR.FSSGAPEMAT = ASESO.FSSEAPELMA";
                strsql += " ON USUAR.FISGIDUSUA = ASESO.FISEIDUSUA";
                strsql += " WHERE USUAR.FISGIDUSUA = " + IdUsuario;

                //consulta
                dt = dbCnx.GetDataSet(strsql).Tables[0];

                DatosAsesor asesor;

                asesor = new DatosAsesor();
                asesor.IdUsuario = dt.Rows[0]["FISGIDUSUA"].ToString().Trim();
                //asesor.IdAsesor = dr["FISEIDASES"].ToString().Trim();
                asesor.Nombre = dt.Rows[0]["FSSGNOMBRE"].ToString().Trim();
                asesor.ApellidoPaterno = dt.Rows[0]["FSSGAPEPAT"].ToString().Trim();
                asesor.ApellidoMaterno = dt.Rows[0]["FSSGAPEMAT"].ToString().Trim();
                asesor.Email = dt.Rows[0]["FCSGEMPRIM"].ToString().Trim();

                List<IdsAsesor> listadoAsesores = new List<IdsAsesor>();

                foreach (DataRow dr in dt.Rows)
                {
                    IdsAsesor objAsesor = new IdsAsesor();

                    objAsesor.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                    objAsesor.IdAsesor = dr["FISEIDASES"].ToString().Trim();

                    listadoAsesores.Add(objAsesor);


                }
                asesor.Asesor = listadoAsesores;

                List<DatosAsesor> listadoDatosAsesor = new List<DatosAsesor>();

                listadoDatosAsesor.Add(asesor);

                return listadoDatosAsesor;

            }
            catch (Exception e)
            {
                string error = e.Message;
                Console.WriteLine(error);
                Respuesta = error;
                List<DatosAsesor> listadoDatosAsesor = new List<DatosAsesor>();
                return listadoDatosAsesor;
            }


        }

        // POST api/Servicio/PostAppAsesorObtenerOrdenesServicioPorFecha
        [Route("api/Servicio/PostAppAsesorObtenerOrdenesServicioPorFecha", Name = "PostAppAsesorObtenerOrdenesServicioPorFecha")]
        public List<OrdenServicio> PostAppAsesorObtenerOrdenesServicioPorFecha(string Fecha, string[] IdsAgencias)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string idsAgencias = "";
            foreach (string id in IdsAgencias)
            {
                if (string.IsNullOrEmpty(idsAgencias))
                {
                    idsAgencias += id;
                }
                else
                {
                    idsAgencias += ",";
                    idsAgencias += id;
                }
            }
            idsAgencias = idsAgencias.Replace("[", "(");
            idsAgencias = idsAgencias.Replace("]", ")");

            string strSql = "";
            strSql = "SELECT PREOR.FISEIDCIAU, PREOR.FISEIDPREO, PREOR.FISEODOMET, PREOR.FSSEDESTIP, ";
            strSql += "PREOR.FDSEVTAOPE, PREOR.FDSEVTAPAQ, PREOR.FDSEVTAOTA, PREOR.FDSEVTAREF, PREOR.FDSEIMPPRE, PREOR.FDSEVTACAR, PREOR.FDSEVTADES, PREOR.FDSEIVAPRE, ";
            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";
            strSql += "WHERE ORDEN.FFSEFECING = '" + Fecha + "' ";
            strSql += "AND PREOR.FISEIDCIAU IN " + idsAgencias + " ";
            strSql += "AND ORDEN.FISEIDESTA = " + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() + " ";
            strSql += "AND ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca";
            dt = dtX.ToTable();

            OrdenServicio ordenServicio;
            List<OrdenServicio> coleccionOrdenesServicio = new List<OrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicio();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();

                string Operaciones = dr["FDSEVTAOPE"].ToString().Trim();

                if (Operaciones != "")
                {
                    ordenServicio.Operaciones = (float.Parse(dr["FDSEVTAOPE"].ToString().Trim())).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Operaciones = (float.Parse("0")).ToString("####0.00");
                }

                string Paquetes = dr["FDSEVTAPAQ"].ToString().Trim();

                if (Paquetes != "")
                {
                    ordenServicio.Paquetes = float.Parse(dr["FDSEVTAPAQ"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Paquetes = (float.Parse("0")).ToString("####0.00");
                }

                string OtrosTrabajos = dr["FDSEVTAOTA"].ToString().Trim();

                if (OtrosTrabajos != "")
                {
                    ordenServicio.OtrosTrabajos = float.Parse(dr["FDSEVTAOTA"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.OtrosTrabajos = (float.Parse("0")).ToString("####0.00");
                }

                string refacciones = dr["FDSEVTAREF"].ToString().Trim();

                if (refacciones != "")
                {
                    ordenServicio.Refacciones = float.Parse(dr["FDSEVTAREF"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Refacciones = (float.Parse("0")).ToString("####0.00");
                }

                string Subtotal = dr["FDSEIMPPRE"].ToString().Trim();

                if (Subtotal != "")
                {
                    ordenServicio.Subtotal = float.Parse(dr["FDSEIMPPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Subtotal = (float.Parse("0")).ToString("####0.00");
                }

                string Cargos = dr["FDSEVTACAR"].ToString().Trim();

                if (Cargos != "")
                {
                    ordenServicio.Cargos = float.Parse(dr["FDSEVTACAR"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Cargos = (float.Parse("0")).ToString("####0.00");
                }

                string Descuentos = dr["FDSEVTADES"].ToString().Trim();

                if (Descuentos != "")
                {
                    ordenServicio.Descuentos = float.Parse(dr["FDSEVTADES"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Descuentos = (float.Parse("0")).ToString("####0.00");
                }

                string Iva = dr["FDSEIVAPRE"].ToString().Trim();

                if (Iva != "0")
                {
                    ordenServicio.Iva = float.Parse(dr["FDSEIVAPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Iva = (float.Parse("0")).ToString("####0.00");
                }

                ordenServicio.Total = (float.Parse(ordenServicio.Subtotal) + float.Parse(ordenServicio.Iva)).ToString("####0.00");

                coleccionOrdenesServicio.Add(ordenServicio);
            }

            return coleccionOrdenesServicio;
        }

        // POST api/Servicio/PostAppAsesorObtenerOrdenesServicioPorFolio
        [Route("api/Servicio/PostAppAsesorObtenerOrdenesServicioPorFolio", Name = "PostAppAsesorObtenerOrdenesServicioPorFolio")]
        public List<OrdenServicio> PostAppAsesorObtenerOrdenesServicioPorFolio(string Folio, string[] IdsAgencias)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string idsAgencias = "";
            foreach (string id in IdsAgencias)
            {
                if (string.IsNullOrEmpty(idsAgencias))
                {
                    idsAgencias += id;
                }
                else
                {
                    idsAgencias += ",";
                    idsAgencias += id;
                }
            }
            idsAgencias = idsAgencias.Replace("[", "(");
            idsAgencias = idsAgencias.Replace("]", ")");

            string strSql = "";
            strSql = "SELECT PREOR.FISEIDCIAU, PREOR.FISEIDPREO, PREOR.FISEODOMET, PREOR.FSSEDESTIP, ";
            strSql += "PREOR.FDSEVTAOPE, PREOR.FDSEVTAPAQ, PREOR.FDSEVTAOTA, PREOR.FDSEVTAREF, PREOR.FDSEIMPPRE, PREOR.FDSEVTACAR, PREOR.FDSEVTADES, PREOR.FDSEIVAPRE, ";
            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";
            strSql += "WHERE ORDEN.FISEFOLIO = '" + Folio + "' ";
            strSql += "AND PREOR.FISEIDCIAU IN " + idsAgencias + " ";
            strSql += "AND ORDEN.FISEIDESTA = " + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() + " ";
            strSql += "AND ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca";
            dt = dtX.ToTable();

            OrdenServicio ordenServicio;
            List<OrdenServicio> coleccionOrdenesServicio = new List<OrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicio();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();

                string Operaciones = dr["FDSEVTAOPE"].ToString().Trim();

                if (Operaciones != "")
                {
                    ordenServicio.Operaciones = (float.Parse(dr["FDSEVTAOPE"].ToString().Trim())).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Operaciones = (float.Parse("0")).ToString("####0.00");
                }

                string Paquetes = dr["FDSEVTAPAQ"].ToString().Trim();

                if (Paquetes != "")
                {
                    ordenServicio.Paquetes = float.Parse(dr["FDSEVTAPAQ"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Paquetes = (float.Parse("0")).ToString("####0.00");
                }

                string OtrosTrabajos = dr["FDSEVTAOTA"].ToString().Trim();

                if (OtrosTrabajos != "")
                {
                    ordenServicio.OtrosTrabajos = float.Parse(dr["FDSEVTAOTA"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.OtrosTrabajos = (float.Parse("0")).ToString("####0.00");
                }

                string refacciones = dr["FDSEVTAREF"].ToString().Trim();

                if (refacciones != "")
                {
                    ordenServicio.Refacciones = float.Parse(dr["FDSEVTAREF"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Refacciones = (float.Parse("0")).ToString("####0.00");
                }

                string Subtotal = dr["FDSEIMPPRE"].ToString().Trim();

                if (Subtotal != "")
                {
                    ordenServicio.Subtotal = float.Parse(dr["FDSEIMPPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Subtotal = (float.Parse("0")).ToString("####0.00");
                }

                string Cargos = dr["FDSEVTACAR"].ToString().Trim();

                if (Cargos != "")
                {
                    ordenServicio.Cargos = float.Parse(dr["FDSEVTACAR"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Cargos = (float.Parse("0")).ToString("####0.00");
                }

                string Descuentos = dr["FDSEVTADES"].ToString().Trim();

                if (Descuentos != "")
                {
                    ordenServicio.Descuentos = float.Parse(dr["FDSEVTADES"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Descuentos = (float.Parse("0")).ToString("####0.00");
                }

                string Iva = dr["FDSEIVAPRE"].ToString().Trim();

                if (Iva != "0")
                {
                    ordenServicio.Iva = float.Parse(dr["FDSEIVAPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Iva = (float.Parse("0")).ToString("####0.00");
                }

                ordenServicio.Total = (float.Parse(ordenServicio.Subtotal) + float.Parse(ordenServicio.Iva)).ToString("####0.00");

                coleccionOrdenesServicio.Add(ordenServicio);
            }

            return coleccionOrdenesServicio;
        }

        // POST api/Servicio/PostAppAsesorObtenerOrdenesServicioPropias
        [Route("api/Servicio/PostAppAsesorObtenerOrdenesServicioPropias", Name = "PostAppAsesorObtenerOrdenesServicioPropias")]
        public List<OrdenServicio> PostAppAsesorObtenerOrdenesServicioPropias(string IdsJson)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            DataTable dtAsesor = JsonConvert.DeserializeObject<DataTable>(IdsJson);

            IdsAsesor idsAsesor;
            List<IdsAsesor> listadoIdsAsesor = new List<IdsAsesor>();

            foreach (DataRow dr in dtAsesor.Rows)
            {
                idsAsesor = new IdsAsesor();

                idsAsesor.IdAsesor = dr["IdAsesor"].ToString();
                idsAsesor.IdAgencia = dr["IdAgencia"].ToString();

                listadoIdsAsesor.Add(idsAsesor);
            }



            string strSql = "";
            strSql = "SELECT PREOR.FISEIDCIAU, PREOR.FISEIDPREO, PREOR.FISEIDASES, PREOR.FISEODOMET, PREOR.FSSEDESTIP, ";

            strSql += "PREOR.FDSEVTAOPE, PREOR.FDSEVTAPAQ, PREOR.FDSEVTAOTA, PREOR.FDSEVTAREF, PREOR.FDSEIMPPRE, PREOR.FDSEVTACAR, PREOR.FDSEVTADES, PREOR.FDSEIVAPRE, ";

            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "AND ORDEN.FISEIDESTA = " + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() + " ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";

            for (int x = 0; x < listadoIdsAsesor.Count(); x++)
            {
                IdsAsesor ids = listadoIdsAsesor[x];

                if (x == 0)
                {
                    strSql += "WHERE (PREOR.FISEIDASES = " + ids.IdAsesor + " ";
                    strSql += "AND PREOR.FISEIDCIAU = " + ids.IdAgencia + ") ";
                }
                else
                {
                    strSql += "OR (PREOR.FISEIDASES = " + ids.IdAsesor + " ";
                    strSql += "AND PREOR.FISEIDCIAU = " + ids.IdAgencia + ") ";
                }
            }

            strSql += "AND ORDEN.FISEIDESTA =" + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() + " ";
            strSql += "AND ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca";
            dt = dtX.ToTable();

            OrdenServicio ordenServicio;
            List<OrdenServicio> coleccionOrdenesServicio = new List<OrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicio();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();

                string Operaciones = dr["FDSEVTAOPE"].ToString().Trim();

                if (Operaciones != "")
                {
                    ordenServicio.Operaciones = (float.Parse(dr["FDSEVTAOPE"].ToString().Trim())).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Operaciones = (float.Parse("0")).ToString("####0.00");
                }

                string Paquetes = dr["FDSEVTAPAQ"].ToString().Trim();

                if (Paquetes != "")
                {
                    ordenServicio.Paquetes = float.Parse(dr["FDSEVTAPAQ"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Paquetes = (float.Parse("0")).ToString("####0.00");
                }

                string OtrosTrabajos = dr["FDSEVTAOTA"].ToString().Trim();

                if (OtrosTrabajos != "")
                {
                    ordenServicio.OtrosTrabajos = float.Parse(dr["FDSEVTAOTA"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.OtrosTrabajos = (float.Parse("0")).ToString("####0.00");
                }

                string refacciones = dr["FDSEVTAREF"].ToString().Trim();

                if (refacciones != "")
                {
                    ordenServicio.Refacciones = float.Parse(dr["FDSEVTAREF"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Refacciones = (float.Parse("0")).ToString("####0.00");
                }

                string Subtotal = dr["FDSEIMPPRE"].ToString().Trim();

                if (Subtotal != "")
                {
                    ordenServicio.Subtotal = float.Parse(dr["FDSEIMPPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Subtotal = (float.Parse("0")).ToString("####0.00");
                }

                string Cargos = dr["FDSEVTACAR"].ToString().Trim();

                if (Cargos != "")
                {
                    ordenServicio.Cargos = float.Parse(dr["FDSEVTACAR"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Cargos = (float.Parse("0")).ToString("####0.00");
                }

                string Descuentos = dr["FDSEVTADES"].ToString().Trim();

                if (Descuentos != "")
                {
                    ordenServicio.Descuentos = float.Parse(dr["FDSEVTADES"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Descuentos = (float.Parse("0")).ToString("####0.00");
                }

                string Iva = dr["FDSEIVAPRE"].ToString().Trim();

                if (Iva != "0")
                {
                    ordenServicio.Iva = float.Parse(dr["FDSEIVAPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Iva = (float.Parse("0")).ToString("####0.00");
                }

                ordenServicio.Total = (float.Parse(ordenServicio.Subtotal) + float.Parse(ordenServicio.Iva)).ToString("####0.00");


                coleccionOrdenesServicio.Add(ordenServicio);
            }

            return coleccionOrdenesServicio;
        }

        // POST api/Servicio/GetObtenerSeguimientoPreordenAppAsesor
        [Route("api/Servicio/GetObtenerSeguimientoPreordenAppAsesor", Name = "GetObtenerSeguimientoPreordenAppAsesor")]
        public List<SeguimientoOrden> GetObtenerSeguimientoPreordenAppAsesor(String IdAgencia, String IdPreOrden)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDSEGMB SEGUI ";
            strSql += "WHERE SEGUI.FIAPSTATUS = 1 ";
            strSql += " AND SEGUI.FIAPIDCIAU = " + IdAgencia;
            strSql += " AND SEGUI.FIAPIDPREO = " + IdPreOrden;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            SeguimientoOrden seguimiento;
            List<SeguimientoOrden> coleccionSeguimiento = new List<SeguimientoOrden>();
            foreach (DataRow dr in dt.Rows)
            {
                seguimiento = new SeguimientoOrden();
                seguimiento.IdSegOrden = dr["FIAPIDSEGO"].ToString().Trim();
                seguimiento.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                seguimiento.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                seguimiento.IdPreorden = dr["FIAPIDPREO"].ToString().Trim();
                seguimiento.Fecha = Convert.ToDateTime(dr["FFAPFECHA"]).ToString("dd-MM-yyyy");
                seguimiento.Hora = Convert.ToDateTime(dr["FHAPHORA"]).ToString("HH:mm:ss");
                seguimiento.Titulo = dr["FSAPTITSEG"].ToString().Trim();
                seguimiento.Observacion = dr["FSAPOBSERV"].ToString().Trim();
                seguimiento.IdCotizacion = dr["FIAPIDCOTI"].ToString().Trim();
                seguimiento.Autorizado = dr["FIAPAUTORI"].ToString().Trim();
                seguimiento.Visto = dr["FIAPVISTO"].ToString().Trim();


                coleccionSeguimiento.Add(seguimiento);
            }

            return coleccionSeguimiento;
        }

        // POST api/Servicio/PostGuardarEncuestaPSI
        [Route("api/Servicio/PostGuardarEncuestaPSI", Name = "PostGuardarEncuestaPSI")]
        public List<RespuestaGuardarEncuestaPSI> PostGuardarEncuestaPSI(string JsonEncuestaPSI)
        {
            string respuesta = "";

            try
            {

                WsEncuestasPSI.ServicioPSI_APPClient clientePSI = new WsEncuestasPSI.ServicioPSI_APPClient();

                EncuestaGuardar encuesta = new EncuestaGuardar();

                encuesta = JsonConvert.DeserializeObject<EncuestaGuardar>(JsonEncuestaPSI);

                WsEncuestasPSI.Encuesta encuestaGuardar = new WsEncuestasPSI.Encuesta();

                encuestaGuardar.IdArea = short.Parse(encuesta.IdArea);
                encuestaGuardar.IdCompania = short.Parse(encuesta.IdCompania);
                encuestaGuardar.IdEncuesta = int.Parse(encuesta.IdEncuesta);

                WsEncuestasPSI.Plantilla plantilla = new WsEncuestasPSI.Plantilla();

                plantilla.IdPlantilla = int.Parse(encuesta.IdPlantilla);
                plantilla.Comentarios = encuesta.Comentarios;

                WsEncuestasPSI.Pregunta preguntaG;

                WsEncuestasPSI.Pregunta[] coleccionPreguntas = new WsEncuestasPSI.Pregunta[encuesta.Respuestas.Count];

                for (int x = 0; x < encuesta.Respuestas.Count; x++)
                {
                    RespuestaEncuestaPSI pregunta = encuesta.Respuestas[x];

                    preguntaG = new WsEncuestasPSI.Pregunta();

                    preguntaG.IdPregunta = short.Parse(pregunta.IdPregunta);

                    WsEncuestasPSI.Respuesta respuestaG;
                    WsEncuestasPSI.Respuesta[] coleccionRespuestas = new WsEncuestasPSI.Respuesta[1];

                    respuestaG = new WsEncuestasPSI.Respuesta();

                    respuestaG.IdRespuesta = int.Parse(pregunta.IdRespuesta);

                    coleccionRespuestas[0] = respuestaG;

                    preguntaG.Respuestas = coleccionRespuestas;

                    coleccionPreguntas[x] = preguntaG;
                }

                plantilla.Preguntas = coleccionPreguntas;

                encuestaGuardar.PlantillaEncuesta = plantilla;

                WsEncuestasPSI.MensajeAgradecimiento mensajeAgradecimiento = clientePSI.GuardarEncuesta(encuestaGuardar);

                if (mensajeAgradecimiento.EsError == false)
                {
                    respuesta = "SI";
                }

                else
                {
                    respuesta = "NO";
                }
                RespuestaGuardarEncuestaPSI respuestaGuardar;
                List<RespuestaGuardarEncuestaPSI> coleccionEncuestas = new List<RespuestaGuardarEncuestaPSI>();
                respuestaGuardar = new RespuestaGuardarEncuestaPSI();

                respuestaGuardar.Respuesta = respuesta;
                respuestaGuardar.Encuesta = mensajeAgradecimiento;


                coleccionEncuestas.Add(respuestaGuardar);

                return coleccionEncuestas;

            }


            catch (Exception e)
            {


                respuesta = e.Message;

                RespuestaGuardarEncuestaPSI respuestaGuardar;
                List<RespuestaGuardarEncuestaPSI> coleccionEncuestas = new List<RespuestaGuardarEncuestaPSI>();
                respuestaGuardar = new RespuestaGuardarEncuestaPSI();

                respuestaGuardar.Respuesta = respuesta;


                coleccionEncuestas.Add(respuestaGuardar);

                return coleccionEncuestas;

            }
        }

        
        // POST api/Servicio/PutActualizarVistoSeguimiento
        [Route("api/Servicio/PutActualizarVistoSeguimiento", Name = "PutActualizarVistoSeguimiento")]
        public RespuestaTest<string> PutActualizarVistoSeguimiento(string IdSegOrden)
        {
            var respuestaServidor = new RespuestaTest<string>();

            DVADB.DB2 dbcnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {
                dbcnx.AbrirConexion();
                dbcnx.BeginTransaccion();

                string strSql = "";
                strSql += "UPDATE " + constantes.Ambiente + "APPS.APDSEGMB ";
                strSql += " SET FIAPVISTO = 1";
                strSql += " WHERE FIAPIDSEGO = " + IdSegOrden;
                dbcnx.SetQuery(strSql);

                dbcnx.CommitTransaccion();
                dbcnx.CerrarConexion();

                respuestaServidor.Ok = "SI";
                return respuestaServidor;
            }
            catch (Exception e)

            {

                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuestaServidor.Ok = "NO";
                respuestaServidor.Mensaje = e.Message;                
                return respuestaServidor;

            }
        }


        [Route("api/Servicio/PostAppAsesorActualizarCotizacionesSeguimientoPreorden", Name = "PostAppAsesorActualizarCotizacionesSeguimientoPreorden")]
        public List<RespuestaServidorImagenVideo> AppAsesorActualizarCotizacionesSeguimientoPreorden(string IdAgencia, string IdPreorden, string Titulo, string Descripcion, string JsonCotizacion, string NumeroFotos, string NumeroVideos, string ExtensionVideo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            //  Abre conexion
            dbCnx.AbrirConexion();

            string IdMarca = "";

            string sqlstr = "";
            sqlstr = " SELECT FIGEIDCIAU, FIGEIDMARC FROM " + constantes.Ambiente + "GRAL.GECCIAUN ";
            sqlstr += " WHERE FIGESTATUS = 1 ";
            sqlstr += " AND FIGEIDCIAU = " + IdAgencia;

            DataTable dTable = dbCnx.GetDataSet(sqlstr).Tables[0];

            if (dTable.Rows.Count != 0)
            {
                IdMarca = dTable.Rows[0]["FIGEIDMARC"].ToString().Trim();
            }


            string estatus = "";
            string IdCuenta = "";
            string IdSeguimiento = "";
            string Token = "";
            string SistemaOperativo = "";

            Devuelve devuelve;

            int TotalFotos = 0;
            int TotalVideos = 0;

            if (NumeroFotos != string.Empty)
            {
                TotalFotos = int.Parse(NumeroFotos);
            }

            if (NumeroVideos != string.Empty)
            {
                TotalVideos = int.Parse(NumeroVideos);
            }

            devuelve = new Devuelve();

            try
            {

                //Obtener Datos de cuenta a la que esta asociada la preorden

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTA ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEPREOR PRE ";
                strSql += "ON CTA.FIAPIDPERS = PRE.FISEIDPERS ";
                strSql += "WHERE CTA.FIAPSTATUS = 1 ";
                strSql += " AND PRE.FISEIDCIAU = " + IdAgencia;
                strSql += " AND PRE.FISEIDPREO = " + IdPreorden;


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    DataRow dataRow = dt.Rows[0];

                    IdCuenta = dataRow["FIAPIDCUEN"].ToString().Trim();
                    Token = dataRow["FSAPTOKEN"].ToString().Trim();
                    SistemaOperativo = dataRow["FSAPSO"].ToString().Trim();

                    strSql = "";
                    strSql = "SELECT coalesce(MAX(FIAPIDSEGO),0)+1 ID FROM PRODAPPS.APDSEGMB";

                    DataTable dt2 = dbCnx.GetDataSet(strSql).Tables[0];



                    DataRow dataRow2 = dt2.Rows[0];

                    IdSeguimiento = dataRow2["ID"].ToString().Trim();


                    //Deserializar Objeto

                    string IdCotizacion = "default";
                    string JsonDetalle = "";
                    Newtonsoft.Json.Linq.JArray arrJsonDetalle = null;

                    if (JsonCotizacion != string.Empty)
                    {

                        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(JsonCotizacion);

                        IdCotizacion = json.GetValue("Id").ToString();
                        JsonDetalle = json.GetValue("Detalle").ToString();

                        arrJsonDetalle = Newtonsoft.Json.Linq.JArray.Parse(JsonDetalle);
                    }

                    try
                    {

                        //  Inicia transaccion seguimiento
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSEGMB ";
                        strSql += "(FIAPIDSEGO, FIAPIDCUEN, FIAPIDCIAU, FIAPIDPREO, FFAPFECHA, FHAPHORA, FSAPTITSEG, FSAPOBSERV, FIAPIDCOTI, FIAPAUTORI, FIAPVISTO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdSeguimiento + ",";
                        strSql += IdCuenta + ",";
                        strSql += IdAgencia + ",";
                        strSql += IdPreorden + ",";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Descripcion + "',";
                        strSql += IdCotizacion + ",";
                        strSql += "0,0,1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        if (arrJsonDetalle != null)
                        {

                            foreach (Newtonsoft.Json.Linq.JObject jsonDetalle in arrJsonDetalle)
                            {

                                string IdDetalle = jsonDetalle.GetValue("IdDetalle").ToString();
                                string TotalCotizacion = jsonDetalle.GetValue("TotalCotizacion").ToString();
                                string Conceptos = jsonDetalle.GetValue("Conceptos").ToString();

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSCOMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDDETA, FFAPFECHA, FIAPPRECIO, FIAPAUTORI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += IdDetalle + ",";
                                strSql += "default" + ",";
                                strSql += TotalCotizacion + ",";
                                strSql += "0,1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                DataTable dtConceptos = JsonConvert.DeserializeObject<DataTable>(Conceptos);

                                foreach (DataRow dr in dtConceptos.Rows)
                                {
                                    string IdConcepto = dr["IdConcepto"].ToString().Trim();
                                    string TipoConcepto = dr["TipoConcepto"].ToString().Trim();
                                    string Concepto = dr["Concepto"].ToString().Trim();
                                    string Precio = dr["Precio"].ToString().Trim();

                                    strSql = "";

                                    strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDCOTMB ";
                                    strSql += "(FIAPIDSEGO, FIAPIDDETA, FIAPIDCONC, FSAPTIPCON, FSAPCONCEP, FIAPPRECIO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                    strSql += "VALUES ";
                                    strSql += "(";
                                    strSql += IdSeguimiento + ",";
                                    strSql += IdDetalle + ",";
                                    strSql += IdConcepto + ",";
                                    strSql += "'" + TipoConcepto + "',";
                                    strSql += "'" + Concepto + "',";
                                    strSql += Precio + ",";
                                    strSql += "1,'CENEASM','CENEASM'";
                                    strSql += ")";
                                    dbCnx.SetQuery(strSql);
                                }
                            }
                        }

                        dbCnx.CommitTransaccion();
                        estatus = "SI";

                    }// try guardar seguimiento
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        estatus = "No se pudo guardar seguimiento.";

                        throw new Exception();
                    }

                    // Envia notificacion
                    if (IdMarca == "")
                    {
                        string RespuestaEnviarNotificacion = "NO";
                    }
                    else if (IdMarca == "71")
                    {
                        string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionAppBajaj(IdCuenta, Descripcion, Titulo, SistemaOperativo);
                    }
                    else
                    {
                        string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionApp(IdCuenta, Descripcion, Titulo, SistemaOperativo);
                    }

                    try
                    {

                        //  Inicia transaccion
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
                        strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDCIAU, FIAPIDPREO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdCuenta + ",";
                        strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Descripcion + "',";
                        strSql += "1,";
                        strSql += IdAgencia + ",";
                        strSql += IdPreorden + ",";
                        strSql += "1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        ImagenesMultimedia imagenMultimedia;
                        List<ImagenesMultimedia> listadoImagenes = new List<ImagenesMultimedia>();

                        if (TotalFotos > 0)
                        {

                            for (int x = 0; x < TotalFotos; x++)
                            {
                                imagenMultimedia = new ImagenesMultimedia();

                                int index = x + 1;
                                string nombreFoto = "IMG_" + IdSeguimiento + "_" + index + ".jpg";
                                string nombreMini = "THMB_" + IdSeguimiento + "_" + index + ".jpg";
                                string Servidor = "http://data.divisionautomotriz.com:8080/AppMiAuto/uploads/";

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSFVMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDCONS, FFAPFECHA, FHAPHORA, FIAPIDTIPS, FSAPRUTMED, FSAPRUMEMI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += index + ",";
                                strSql += "default" + ",";
                                strSql += "default" + ",";
                                strSql += "1,";
                                strSql += "'" + Servidor + nombreFoto + "',";
                                strSql += "'" + Servidor + nombreMini + "',";
                                strSql += "1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                imagenMultimedia.NombreRutaMedia = nombreFoto;
                                imagenMultimedia.NombreRutaMini = nombreMini;
                                //imagenMultimedia.RutaMedia = Servidor + nombreFoto;
                                //imagenMultimedia.RutaMini = Servidor + nombreMini;

                                listadoImagenes.Add(imagenMultimedia);
                            }
                        }

                        VideosMultimedia videoMultimedia;
                        List<VideosMultimedia> listadoVideos = new List<VideosMultimedia>();

                        if (TotalVideos > 0)
                        {

                            for (int y = 0; y < TotalVideos; y++)
                            {
                                videoMultimedia = new VideosMultimedia();

                                int index = y + 1 + TotalFotos;
                                string nombreVideo = "VID_" + IdSeguimiento + "_" + index + "." + ExtensionVideo;
                                string nombreMini = "THMB_" + IdSeguimiento + "_" + index + "." + "jpg";
                                string Servidor = "http://data.divisionautomotriz.com:8080/AppMiAuto/uploads/";

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSFVMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDCONS, FFAPFECHA, FHAPHORA, FIAPIDTIPS, FSAPRUTMED, FSAPRUMEMI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += index + ",";
                                strSql += "default" + ",";
                                strSql += "default" + ",";
                                strSql += "2,";
                                strSql += "'" + Servidor + nombreVideo + "',";
                                strSql += "'" + Servidor + nombreMini + "',";
                                strSql += "1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                videoMultimedia.NombreRutaMedia = nombreVideo;
                                videoMultimedia.NombreRutaMini = nombreMini;
                                //videoMultimedia.RutaMedia = Servidor + nombreVideo;
                                //videoMultimedia.RutaMini = Servidor + nombreMini;

                                listadoVideos.Add(videoMultimedia);
                            }
                        }

                        devuelve.Imagenes = listadoImagenes;
                        devuelve.Videos = listadoVideos;

                        estatus = "SI";
                        dbCnx.CommitTransaccion();

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();

                        throw new Exception();
                    }

                }
                else
                {
                    estatus = "No existen cuentas asociadas a esa orden.";
                }

            }
            catch (Exception e)
            {
                dbCnx.CerrarConexion();
            }
            finally
            {
                //  Cierra
                dbCnx.CerrarConexion();



            }
            //return estatus;
            RespuestaServidorImagenVideo respuestaServ = new RespuestaServidorImagenVideo();
            List<RespuestaServidorImagenVideo> objetoRespuesta = new List<RespuestaServidorImagenVideo>();

            respuestaServ.Respuesta = estatus;
            respuestaServ.Devuelve = devuelve;

            objetoRespuesta.Add(respuestaServ);

            return objetoRespuesta;


        }

        // POST api/Servicio/PostAppAsesorActualizarCotizacionesSeguimientoPreordenIOS
        [Route("api/Servicio/PostAppAsesorActualizarCotizacionesSeguimientoPreordenIOS", Name = "PostAppAsesorActualizarCotizacionesSeguimientoPreordenIOS")]
        public List<RespuestaServidorImagenVideo> PostAppAsesorActualizarCotizacionesSeguimientoPreordenIOS(string IdAgencia, string IdPreorden, string Titulo, string Descripcion, string NumeroFotos, string NumeroVideos, string ExtensionVideo, string JsonCotizacion)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            //  Abre conexion
            dbCnx.AbrirConexion();

            string IdMarca = "";

            string sqlstr = "";
            sqlstr = " SELECT FIGEIDCIAU, FIGEIDMARC FROM " + constantes.Ambiente + "GRAL.GECCIAUN ";
            sqlstr += " WHERE FIGESTATUS = 1 ";
            sqlstr += " AND FIGEIDCIAU = " + IdAgencia;

            DataTable dTable = dbCnx.GetDataSet(sqlstr).Tables[0];

            if (dTable.Rows.Count != 0)
            {
                IdMarca = dTable.Rows[0]["FIGEIDMARC"].ToString().Trim();
            }


            string estatus = "";
            string IdCuenta = "";
            string IdSeguimiento = "";
            string Token = "";
            string SistemaOperativo = "";

            Devuelve devuelve;

            int TotalFotos = 0;
            int TotalVideos = 0;

            if (NumeroFotos != string.Empty)
            {
                TotalFotos = int.Parse(NumeroFotos);
            }

            if (NumeroVideos != string.Empty)
            {
                TotalVideos = int.Parse(NumeroVideos);
            }

            devuelve = new Devuelve();

            try
            {

                //Obtener Datos de cuenta a la que esta asociada la preorden

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTA ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEPREOR PRE ";
                strSql += "ON CTA.FIAPIDPERS = PRE.FISEIDPERS ";
                strSql += "WHERE CTA.FIAPSTATUS = 1 ";
                strSql += " AND PRE.FISEIDCIAU = " + IdAgencia;
                strSql += " AND PRE.FISEIDPREO = " + IdPreorden;


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    DataRow dataRow = dt.Rows[0];

                    IdCuenta = dataRow["FIAPIDCUEN"].ToString().Trim();
                    Token = dataRow["FSAPTOKEN"].ToString().Trim();
                    SistemaOperativo = dataRow["FSAPSO"].ToString().Trim();

                    strSql = "";
                    strSql = "SELECT coalesce(MAX(FIAPIDSEGO),0)+1 ID FROM PRODAPPS.APDSEGMB";

                    DataTable dt2 = dbCnx.GetDataSet(strSql).Tables[0];



                    DataRow dataRow2 = dt2.Rows[0];

                    IdSeguimiento = dataRow2["ID"].ToString().Trim();


                    //Deserializar Objeto

                    string IdCotizacion = "default";
                    string JsonDetalle = "";
                    Newtonsoft.Json.Linq.JArray arrJsonDetalle = null;

                    if (JsonCotizacion != string.Empty)
                    {

                        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(JsonCotizacion);

                        IdCotizacion = json.GetValue("Id").ToString();
                        JsonDetalle = json.GetValue("Detalle").ToString();

                        arrJsonDetalle = Newtonsoft.Json.Linq.JArray.Parse(JsonDetalle);
                    }

                    try
                    {

                        //  Inicia transaccion seguimiento
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSEGMB ";
                        strSql += "(FIAPIDSEGO, FIAPIDCUEN, FIAPIDCIAU, FIAPIDPREO, FFAPFECHA, FHAPHORA, FSAPTITSEG, FSAPOBSERV, FIAPIDCOTI, FIAPAUTORI, FIAPVISTO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdSeguimiento + ",";
                        strSql += IdCuenta + ",";
                        strSql += IdAgencia + ",";
                        strSql += IdPreorden + ",";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Descripcion + "',";
                        strSql += IdCotizacion + ",";
                        strSql += "0,0,1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        if (arrJsonDetalle != null)
                        {

                            foreach (Newtonsoft.Json.Linq.JObject jsonDetalle in arrJsonDetalle)
                            {

                                string IdDetalle = jsonDetalle.GetValue("IdDetalle").ToString();
                                string TotalCotizacion = jsonDetalle.GetValue("TotalCotizacion").ToString();
                                string Conceptos = jsonDetalle.GetValue("Conceptos").ToString();

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSCOMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDDETA, FFAPFECHA, FIAPPRECIO, FIAPAUTORI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += IdDetalle + ",";
                                strSql += "default" + ",";
                                strSql += TotalCotizacion + ",";
                                strSql += "0,1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                DataTable dtConceptos = JsonConvert.DeserializeObject<DataTable>(Conceptos);

                                foreach (DataRow dr in dtConceptos.Rows)
                                {
                                    string IdConcepto = dr["IdConcepto"].ToString().Trim();
                                    string TipoConcepto = dr["TipoConcepto"].ToString().Trim();
                                    string Concepto = dr["Concepto"].ToString().Trim();
                                    string Precio = dr["Precio"].ToString().Trim();

                                    strSql = "";

                                    strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDCOTMB ";
                                    strSql += "(FIAPIDSEGO, FIAPIDDETA, FIAPIDCONC, FSAPTIPCON, FSAPCONCEP, FIAPPRECIO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                    strSql += "VALUES ";
                                    strSql += "(";
                                    strSql += IdSeguimiento + ",";
                                    strSql += IdDetalle + ",";
                                    strSql += IdConcepto + ",";
                                    strSql += "'" + TipoConcepto + "',";
                                    strSql += "'" + Concepto + "',";
                                    strSql += Precio + ",";
                                    strSql += "1,'CENEASM','CENEASM'";
                                    strSql += ")";
                                    dbCnx.SetQuery(strSql);
                                }
                            }
                        }

                        dbCnx.CommitTransaccion();
                        estatus = "SI";

                    }// try guardar seguimiento
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        estatus = "No se pudo guardar seguimiento.";

                        throw new Exception();
                    }

                    // Envia notificacion
                    if (IdMarca == "")
                    {
                        string RespuestaEnviarNotificacion = "NO";
                    }
                    else if (IdMarca == "71")
                    {
                        string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionAppBajaj(IdCuenta, Descripcion, Titulo, SistemaOperativo);
                    }
                    else
                    {
                        string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionApp(IdCuenta, Descripcion, Titulo, SistemaOperativo);
                    }

                    try
                    {

                        //  Inicia transaccion
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
                        strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDCIAU, FIAPIDPREO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdCuenta + ",";
                        strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Descripcion + "',";
                        strSql += "1,";
                        strSql += IdAgencia + ",";
                        strSql += IdPreorden + ",";
                        strSql += "1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        ImagenesMultimedia imagenMultimedia;
                        List<ImagenesMultimedia> listadoImagenes = new List<ImagenesMultimedia>();

                        if (TotalFotos > 0)
                        {

                            for (int x = 0; x < TotalFotos; x++)
                            {
                                imagenMultimedia = new ImagenesMultimedia();

                                int index = x + 1;
                                string nombreFoto = "IMG_" + IdSeguimiento + "_" + index + ".jpg";
                                string nombreMini = "THMB_" + IdSeguimiento + "_" + index + ".jpg";
                                string Servidor = "http://data.divisionautomotriz.com:8080/AppMiAuto/uploads/";

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSFVMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDCONS, FFAPFECHA, FHAPHORA, FIAPIDTIPS, FSAPRUTMED, FSAPRUMEMI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += index + ",";
                                strSql += "default" + ",";
                                strSql += "default" + ",";
                                strSql += "1,";
                                strSql += "'" + Servidor + nombreFoto + "',";
                                strSql += "'" + Servidor + nombreMini + "',";
                                strSql += "1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                imagenMultimedia.NombreRutaMedia = nombreFoto;
                                imagenMultimedia.NombreRutaMini = nombreMini;
                                //imagenMultimedia.RutaMedia = Servidor + nombreFoto;
                                //imagenMultimedia.RutaMini = Servidor + nombreMini;

                                listadoImagenes.Add(imagenMultimedia);
                            }
                        }

                        VideosMultimedia videoMultimedia;
                        List<VideosMultimedia> listadoVideos = new List<VideosMultimedia>();

                        if (TotalVideos > 0)
                        {

                            for (int y = 0; y < TotalVideos; y++)
                            {
                                videoMultimedia = new VideosMultimedia();

                                int index = y + 1 + TotalFotos;
                                string nombreVideo = "VID_" + IdSeguimiento + "_" + index + "." + ExtensionVideo;
                                string nombreMini = "THMB_" + IdSeguimiento + "_" + index + "." + "jpg";
                                string Servidor = "http://data.divisionautomotriz.com:8080/AppMiAuto/uploads/";

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSFVMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDCONS, FFAPFECHA, FHAPHORA, FIAPIDTIPS, FSAPRUTMED, FSAPRUMEMI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += index + ",";
                                strSql += "default" + ",";
                                strSql += "default" + ",";
                                strSql += "2,";
                                strSql += "'" + Servidor + nombreVideo + "',";
                                strSql += "'" + Servidor + nombreMini + "',";
                                strSql += "1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                videoMultimedia.NombreRutaMedia = nombreVideo;
                                videoMultimedia.NombreRutaMini = nombreMini;
                                //videoMultimedia.RutaMedia = Servidor + nombreVideo;
                                //videoMultimedia.RutaMini = Servidor + nombreMini;

                                listadoVideos.Add(videoMultimedia);
                            }
                        }

                        devuelve.Imagenes = listadoImagenes;
                        devuelve.Videos = listadoVideos;

                        estatus = "SI";
                        dbCnx.CommitTransaccion();

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();

                        throw new Exception();
                    }

                }
                else
                {
                    estatus = "No existen cuentas asociadas a esa orden.";
                }

            }
            catch (Exception e)
            {
                dbCnx.CerrarConexion();
            }
            finally
            {
                //  Cierra
                dbCnx.CerrarConexion();



            }
            //return estatus;
            RespuestaServidorImagenVideo respuestaServ = new RespuestaServidorImagenVideo();
            List<RespuestaServidorImagenVideo> objetoRespuesta = new List<RespuestaServidorImagenVideo>();

            respuestaServ.Respuesta = estatus;
            respuestaServ.Devuelve = devuelve;

            objetoRespuesta.Add(respuestaServ);

            return objetoRespuesta;

        }

        // POST api/Servicio/PostAppAsesorActualizarCotizacionesSeguimientoPreordenSOAP
        [Route("api/Servicio/PostAppAsesorActualizarCotizacionesSeguimientoPreordenSOAP", Name = "PostAppAsesorActualizarCotizacionesSeguimientoPreordenSOAP")]
        public string PostAppAsesorActualizarCotizacionesSeguimientoPreordenSOAP(string IdAgencia, string IdPreorden, string Titulo, string Descripcion, string JsonCotizacion, string NumeroFotos, string NumeroVideos, string ExtensionVideo)
        {
            string Respuesta = "";
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            //  Abre conexion
            dbCnx.AbrirConexion();

            string IdMarca = "";

            string sqlstr = "";
            sqlstr = " SELECT FIGEIDCIAU, FIGEIDMARC FROM " + constantes.Ambiente + "GRAL.GECCIAUN ";
            sqlstr += " WHERE FIGESTATUS = 1 ";
            sqlstr += " AND FIGEIDCIAU = " + IdAgencia;

            DataTable dTable = dbCnx.GetDataSet(sqlstr).Tables[0];

            if (dTable.Rows.Count != 0)
            {
                IdMarca = dTable.Rows[0]["FIGEIDMARC"].ToString().Trim();
            }


            string estatus = "";
            string IdCuenta = "";
            string IdSeguimiento = "";
            string Token = "";
            string SistemaOperativo = "";

            Devuelve devuelve;

            int TotalFotos = 0;
            int TotalVideos = 0;

            if (NumeroFotos != string.Empty)
            {
                TotalFotos = int.Parse(NumeroFotos);
            }

            if (NumeroVideos != string.Empty)
            {
                TotalVideos = int.Parse(NumeroVideos);
            }

            devuelve = new Devuelve();

            try
            {

                //Obtener Datos de cuenta a la que esta asociada la preorden

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CTA ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEPREOR PRE ";
                strSql += "ON CTA.FIAPIDPERS = PRE.FISEIDPERS ";
                strSql += "WHERE CTA.FIAPSTATUS = 1 ";
                strSql += " AND PRE.FISEIDCIAU = " + IdAgencia;
                strSql += " AND PRE.FISEIDPREO = " + IdPreorden;


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    DataRow dataRow = dt.Rows[0];

                    IdCuenta = dataRow["FIAPIDCUEN"].ToString().Trim();
                    Token = dataRow["FSAPTOKEN"].ToString().Trim();
                    SistemaOperativo = dataRow["FSAPSO"].ToString().Trim();

                    strSql = "";
                    strSql = "SELECT coalesce(MAX(FIAPIDSEGO),0)+1 ID FROM PRODAPPS.APDSEGMB";

                    DataTable dt2 = dbCnx.GetDataSet(strSql).Tables[0];



                    DataRow dataRow2 = dt2.Rows[0];

                    IdSeguimiento = dataRow2["ID"].ToString().Trim();


                    //Deserializar Objeto

                    string IdCotizacion = "default";
                    string JsonDetalle = "";
                    Newtonsoft.Json.Linq.JArray arrJsonDetalle = null;

                    if (JsonCotizacion != string.Empty)
                    {

                        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(JsonCotizacion);

                        IdCotizacion = json.GetValue("Id").ToString();
                        JsonDetalle = json.GetValue("Detalle").ToString();

                        arrJsonDetalle = Newtonsoft.Json.Linq.JArray.Parse(JsonDetalle);
                    }

                    try
                    {

                        //  Inicia transaccion seguimiento
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSEGMB ";
                        strSql += "(FIAPIDSEGO, FIAPIDCUEN, FIAPIDCIAU, FIAPIDPREO, FFAPFECHA, FHAPHORA, FSAPTITSEG, FSAPOBSERV, FIAPIDCOTI, FIAPAUTORI, FIAPVISTO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdSeguimiento + ",";
                        strSql += IdCuenta + ",";
                        strSql += IdAgencia + ",";
                        strSql += IdPreorden + ",";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Descripcion + "',";
                        strSql += IdCotizacion + ",";
                        strSql += "0,0,1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        if (arrJsonDetalle != null)
                        {

                            foreach (Newtonsoft.Json.Linq.JObject jsonDetalle in arrJsonDetalle)
                            {

                                string IdDetalle = jsonDetalle.GetValue("IdDetalle").ToString();
                                string TotalCotizacion = jsonDetalle.GetValue("TotalCotizacion").ToString();
                                string Conceptos = jsonDetalle.GetValue("Conceptos").ToString();

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSCOMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDDETA, FFAPFECHA, FIAPPRECIO, FIAPAUTORI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += IdDetalle + ",";
                                strSql += "default" + ",";
                                strSql += TotalCotizacion + ",";
                                strSql += "0,1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                DataTable dtConceptos = JsonConvert.DeserializeObject<DataTable>(Conceptos);

                                foreach (DataRow dr in dtConceptos.Rows)
                                {
                                    string IdConcepto = dr["IdConcepto"].ToString().Trim();
                                    string TipoConcepto = dr["TipoConcepto"].ToString().Trim();
                                    string Concepto = dr["Concepto"].ToString().Trim();
                                    string Precio = dr["Precio"].ToString().Trim();

                                    strSql = "";

                                    strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDCOTMB ";
                                    strSql += "(FIAPIDSEGO, FIAPIDDETA, FIAPIDCONC, FSAPTIPCON, FSAPCONCEP, FIAPPRECIO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                    strSql += "VALUES ";
                                    strSql += "(";
                                    strSql += IdSeguimiento + ",";
                                    strSql += IdDetalle + ",";
                                    strSql += IdConcepto + ",";
                                    strSql += "'" + TipoConcepto + "',";
                                    strSql += "'" + Concepto + "',";
                                    strSql += Precio + ",";
                                    strSql += "1,'CENEASM','CENEASM'";
                                    strSql += ")";
                                    dbCnx.SetQuery(strSql);
                                }
                            }
                        }

                        dbCnx.CommitTransaccion();
                        estatus = "SI";

                    }// try guardar seguimiento
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        estatus = "No se pudo guardar seguimiento.";

                        throw new Exception();
                    }

                    // Envia notificacion
                    if (IdMarca == "")
                    {
                        string RespuestaEnviarNotificacion = "NO";
                    }
                    else if (IdMarca == "71")
                    {
                        string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionAppBajaj(IdCuenta, Descripcion, Titulo, SistemaOperativo);
                    }
                    else
                    {
                        string RespuestaEnviarNotificacion = NotificacionesApp.EnviarNotificacionApp(IdCuenta, Descripcion, Titulo, SistemaOperativo);
                    }

                    try
                    {

                        //  Inicia transaccion
                        dbCnx.BeginTransaccion();

                        strSql = "";

                        strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
                        strSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDCIAU, FIAPIDPREO, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                        strSql += "VALUES ";
                        strSql += "(";
                        strSql += IdCuenta + ",";
                        strSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
                        strSql += "default" + ",";
                        strSql += "default" + ",";
                        strSql += "'" + Titulo + "',";
                        strSql += "'" + Descripcion + "',";
                        strSql += "1,";
                        strSql += IdAgencia + ",";
                        strSql += IdPreorden + ",";
                        strSql += "1,'CENEASM','CENEASM'";
                        strSql += ")";
                        dbCnx.SetQuery(strSql);

                        ImagenesMultimedia imagenMultimedia;
                        List<ImagenesMultimedia> listadoImagenes = new List<ImagenesMultimedia>();

                        if (TotalFotos > 0)
                        {

                            for (int x = 0; x < TotalFotos; x++)
                            {
                                imagenMultimedia = new ImagenesMultimedia();

                                int index = x + 1;
                                string nombreFoto = "IMG_" + IdSeguimiento + "_" + index + ".jpg";
                                string nombreMini = "THMB_" + IdSeguimiento + "_" + index + ".jpg";
                                string Servidor = "http://data.divisionautomotriz.com:8080/AppMiAuto/uploads/";

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSFVMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDCONS, FFAPFECHA, FHAPHORA, FIAPIDTIPS, FSAPRUTMED, FSAPRUMEMI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += index + ",";
                                strSql += "default" + ",";
                                strSql += "default" + ",";
                                strSql += "1,";
                                strSql += "'" + Servidor + nombreFoto + "',";
                                strSql += "'" + Servidor + nombreMini + "',";
                                strSql += "1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                imagenMultimedia.NombreRutaMedia = nombreFoto;
                                imagenMultimedia.NombreRutaMini = nombreMini;
                                //imagenMultimedia.RutaMedia = Servidor + nombreFoto;
                                //imagenMultimedia.RutaMini = Servidor + nombreMini;

                                listadoImagenes.Add(imagenMultimedia);
                            }
                        }

                        VideosMultimedia videoMultimedia;
                        List<VideosMultimedia> listadoVideos = new List<VideosMultimedia>();

                        if (TotalVideos > 0)
                        {

                            for (int y = 0; y < TotalVideos; y++)
                            {
                                videoMultimedia = new VideosMultimedia();

                                int index = y + 1 + TotalFotos;
                                string nombreVideo = "VID_" + IdSeguimiento + "_" + index + "." + ExtensionVideo;
                                string nombreMini = "THMB_" + IdSeguimiento + "_" + index + "." + "jpg";
                                string Servidor = "http://data.divisionautomotriz.com:8080/AppMiAuto/uploads/";

                                strSql = "";

                                strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDSFVMB ";
                                strSql += "(FIAPIDSEGO, FIAPIDCONS, FFAPFECHA, FHAPHORA, FIAPIDTIPS, FSAPRUTMED, FSAPRUMEMI, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                                strSql += "VALUES ";
                                strSql += "(";
                                strSql += IdSeguimiento + ",";
                                strSql += index + ",";
                                strSql += "default" + ",";
                                strSql += "default" + ",";
                                strSql += "2,";
                                strSql += "'" + Servidor + nombreVideo + "',";
                                strSql += "'" + Servidor + nombreMini + "',";
                                strSql += "1,'CENEASM','CENEASM'";
                                strSql += ")";
                                dbCnx.SetQuery(strSql);

                                videoMultimedia.NombreRutaMedia = nombreVideo;
                                videoMultimedia.NombreRutaMini = nombreMini;
                                //videoMultimedia.RutaMedia = Servidor + nombreVideo;
                                //videoMultimedia.RutaMini = Servidor + nombreMini;

                                listadoVideos.Add(videoMultimedia);
                            }
                        }

                        devuelve.Imagenes = listadoImagenes;
                        devuelve.Videos = listadoVideos;

                        estatus = "SI";
                        dbCnx.CommitTransaccion();

                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();

                        throw new Exception();
                    }

                }
                else
                {
                    estatus = "No existen cuentas asociadas a esa orden.";
                }

            }
            catch (Exception e)
            {
                dbCnx.CerrarConexion();
            }
            finally
            {
                //  Cierra
                dbCnx.CerrarConexion();

                //return estatus;
                RespuestaServidorImagenVideo respuestaServ = new RespuestaServidorImagenVideo();
                List<RespuestaServidorImagenVideo> objetoRespuesta = new List<RespuestaServidorImagenVideo>();

                respuestaServ.Respuesta = estatus;
                respuestaServ.Devuelve = devuelve;

                objetoRespuesta.Add(respuestaServ);

                JavaScriptSerializer jss2 = new JavaScriptSerializer();
                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss2.Serialize(objetoRespuesta));

                Respuesta = jss2.Serialize(objetoRespuesta);
            }

            return Respuesta;

        }

        // GET api/Servicio/GetObtenerOrdenesServicioHistorial
        [Route("api/Servicio/GetObtenerOrdenesServicioHistorial", Name = "GetObtenerOrdenesServicioHistorial")]
        public List<OrdenServicio> GetObtenerOrdenesServicioHistorial(long IdPersona, string IdsVehiculos)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string idsVehiculos = "";
            foreach (string id in IdsVehiculos.Split(','))
            {
                if (string.IsNullOrEmpty(idsVehiculos))
                {
                    idsVehiculos += id;
                }
                else
                {
                    idsVehiculos += ",";
                    idsVehiculos += id;
                }
            }
            idsVehiculos = idsVehiculos.Replace("[", "");
            idsVehiculos = idsVehiculos.Replace("]", "");

            string strSql = "";
            strSql = "SELECT    PREOR.FISEIDCIAU, PREOR.FISEIDPREO, PREOR.FISEODOMET, PREOR.FSSEDESTIP, ";
            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON	PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND	PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON	ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON	PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND	PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON	PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON	VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON	VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON	PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";
            strSql += "WHERE	PREOR.FISEIDPERS = " + IdPersona.ToString() + " ";
            strSql += "AND	PREOR.FISEIDVEHI IN (" + idsVehiculos + ") ";
            //strSql += "AND	YEAR(ORDEN.FFSEFECING) = YEAR('" + Convert.ToDateTime(Fecha).ToString("dd-MM-yyyy") + "') ";
            //strSql += "AND	MONTH(ORDEN.FFSEFECING) = MONTH('" + Convert.ToDateTime(Fecha).ToString("dd-MM-yyyy") + "') ";
            strSql += "AND	ORDEN.FISEIDESTA NOT IN (" + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() +
                "," + constantes.CA_SE_STATUS_ORDEN_PREFACTURADA.ToString() + ") ";
            strSql += "AND	ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            //String RutaArchivo = obtenerURLServidor() + "Recursos/Json/" + "HistorialOrdenes.json";

            //String strJSON = leerArchivoWeb(RutaArchivo);

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(strJSON);
            /*dt.TableName = "HistorialOrdenes";
            String RowFilterText = "";
            RowFilterText = "IdPersona = " + IdPersona.ToString() + " AND IdVehiculo IN (" + idsVehiculos + ") ";
            //RowFilterText += "AND YEAR(" + dt.TableName + ", FechaIngreso) = YEAR('" + FechaIngreso + "') ";
            //RowFilterText += "AND MONTH(" + dt.TableName + ", FechaIngreso) = MONTH('" + Convert.ToDateTime(Fecha).ToString("dd-MM-yyyy") + "') ";
            RowFilterText += " AND (Convert(FechaIngreso,System.String) like '%" + Convert.ToDateTime(Fecha).ToString("yyyy") + "%') AND (Convert(FechaIngreso,System.String) like '%" + Convert.ToDateTime(Fecha).ToString("MM") + "%')";

            dt.DefaultView.RowFilter = "";
            dt.DefaultView.RowFilter = RowFilterText;*/
            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca"
            dt = dtX.ToTable();

            OrdenServicio ordenServicio;
            List<OrdenServicio> coleccionOrdenesServicio = new List<OrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicio();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();
                coleccionOrdenesServicio.Add(ordenServicio);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionOrdenesServicio));

            return coleccionOrdenesServicio;
        }
        
        // POST api/Servicio/GetObtenerOrdenesServicioFacturacion
        [Route("api/Servicio/GetObtenerOrdenesServicioFacturacion", Name = "GetObtenerOrdenesServicioFacturacion")]
        public List<OrdenServicioFacturada> GetObtenerOrdenesServicioFacturacion(long IdPersona, string IdsVehiculos)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    PREOR.FISEIDCIAU, PREOR.FISEIDPREO, PREOR.FISEODOMET, PREOR.FSSEDESTIP, ";
            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "FACTU.FDCAIVA, FACTU.FDCASUBTOT, FACTU.FDCATOTAL, FACTU.FCCAPREIN, FACTU.FICAFOLIN, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON	PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND	PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";

            strSql += "INNER JOIN " + constantes.Ambiente + "CAJA.CAEFACTU FACTU ";
            strSql += "ON	FACTU.FICAIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND	FACTU.FICAIDFACT = ORDEN.FISEIDFACT ";

            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON	ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON	PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND	PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON	PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON	VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON	VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON	PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";
            strSql += "WHERE	PREOR.FISEIDPERS = " + IdPersona.ToString() + " ";
            strSql += "AND	PREOR.FISEIDVEHI IN (" + IdsVehiculos + ") ";
            //strSql += "AND	YEAR(ORDEN.FFSEFECING) = YEAR('" + Convert.ToDateTime(Fecha).ToString("dd-MM-yyyy") + "') ";
            //strSql += "AND	MONTH(ORDEN.FFSEFECING) = MONTH('" + Convert.ToDateTime(Fecha).ToString("dd-MM-yyyy") + "') ";
            strSql += "AND	ORDEN.FISEIDESTA IN (" + constantes.CA_SE_STATUS_ORDEN_FACTURADA.ToString() + ") ";
            strSql += "AND	ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            //String RutaArchivo = obtenerURLServidor() + "Recursos/Json/" + "HistorialOrdenes.json";

            //String strJSON = leerArchivoWeb(RutaArchivo);

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(strJSON);
            /*dt.TableName = "HistorialOrdenes";
            String RowFilterText = "";
            RowFilterText = "IdPersona = " + IdPersona.ToString() + " AND IdVehiculo IN (" + idsVehiculos + ") ";
            //RowFilterText += "AND YEAR(" + dt.TableName + ", FechaIngreso) = YEAR('" + FechaIngreso + "') ";
            //RowFilterText += "AND MONTH(" + dt.TableName + ", FechaIngreso) = MONTH('" + Convert.ToDateTime(Fecha).ToString("dd-MM-yyyy") + "') ";
            RowFilterText += " AND (Convert(FechaIngreso,System.String) like '%" + Convert.ToDateTime(Fecha).ToString("yyyy") + "%') AND (Convert(FechaIngreso,System.String) like '%" + Convert.ToDateTime(Fecha).ToString("MM") + "%')";

            dt.DefaultView.RowFilter = "";
            dt.DefaultView.RowFilter = RowFilterText;*/
            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca"
            dt = dtX.ToTable();

            OrdenServicioFacturada ordenServicio;
            List<OrdenServicioFacturada> coleccionOrdenesServicio = new List<OrdenServicioFacturada>();

            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicioFacturada();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();
                ordenServicio.SubTotal = dr["FDCASUBTOT"].ToString().Trim();
                ordenServicio.IVA = dr["FDCAIVA"].ToString().Trim();
                ordenServicio.Total = dr["FDCATOTAL"].ToString().Trim();
                ordenServicio.FolioInicial = dr["FICAFOLIN"].ToString().Trim();
                ordenServicio.PrefijoInicial = dr["FCCAPREIN"].ToString().Trim();
                coleccionOrdenesServicio.Add(ordenServicio);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionOrdenesServicio));
            return coleccionOrdenesServicio;
        }

        
        [Route("api/Servicio/GetObtenerOrdenServicioAplicaSeguimiento", Name = "GetObtenerOrdenServicioAplicaSeguimiento")]
        public List<OrdenServicio> GetObtenerOrdenServicioAplicaSeguimiento(string IdAgencia, string IdPreorden)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();


            string strSql = "";
            strSql = "SELECT PREOR.FISEIDCIAU, PREOR.FISEIDPERS, PREOR.FISEIDPREO, PREOR.FISEODOMET, PREOR.FSSEDESTIP, PREOR.FISEIDVEHI, ";
            strSql += "PREOR.FDSEVTAOPE, PREOR.FDSEVTAPAQ, PREOR.FDSEVTAOTA, PREOR.FDSEVTAREF, PREOR.FDSEIMPPRE, PREOR.FDSEVTACAR, PREOR.FDSEVTADES, PREOR.FDSEIVAPRE, ";
            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";
            strSql += "WHERE PREOR.FISEIDCIAU = " + IdAgencia + " ";
            strSql += "AND PREOR.FISEIDPREO = " + IdPreorden + " ";
            strSql += "AND ORDEN.FISEIDESTA IN (" + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() +
                "," + constantes.CA_SE_STATUS_ORDEN_PREFACTURADA.ToString() + ") ";
            strSql += "AND ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            /*String RutaArchivo = obtenerURLServidor() + "Recursos/Json/" + "OrdenesProceso.json";

            String strJSON = leerArchivoWeb(RutaArchivo);

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(strJSON);

            String RowFilterText = "";
            RowFilterText = "IdPersona = " + IdPersona.ToString() + " AND IdVehiculo IN (" + idsVehiculos + ")";

            dt.DefaultView.RowFilter = "";
            dt.DefaultView.RowFilter = RowFilterText;*/
            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca";
            dt = dtX.ToTable();

            OrdenServicio ordenServicio;
            List<OrdenServicio> coleccionOrdenesServicio = new List<OrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicio();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();

                string Operaciones = dr["FDSEVTAOPE"].ToString().Trim();

                if (Operaciones != "")
                {
                    ordenServicio.Operaciones = (float.Parse(dr["FDSEVTAOPE"].ToString().Trim())).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Operaciones = (float.Parse("0")).ToString("####0.00");
                }

                string Paquetes = dr["FDSEVTAPAQ"].ToString().Trim();

                if (Paquetes != "")
                {
                    ordenServicio.Paquetes = float.Parse(dr["FDSEVTAPAQ"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Paquetes = (float.Parse("0")).ToString("####0.00");
                }

                string OtrosTrabajos = dr["FDSEVTAOTA"].ToString().Trim();

                if (OtrosTrabajos != "")
                {
                    ordenServicio.OtrosTrabajos = float.Parse(dr["FDSEVTAOTA"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.OtrosTrabajos = (float.Parse("0")).ToString("####0.00");
                }

                string refacciones = dr["FDSEVTAREF"].ToString().Trim();

                if (refacciones != "")
                {
                    ordenServicio.Refacciones = float.Parse(dr["FDSEVTAREF"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Refacciones = (float.Parse("0")).ToString("####0.00");
                }

                string Subtotal = dr["FDSEIMPPRE"].ToString().Trim();

                if (Subtotal != "")
                {
                    ordenServicio.Subtotal = float.Parse(dr["FDSEIMPPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Subtotal = (float.Parse("0")).ToString("####0.00");
                }

                string Cargos = dr["FDSEVTACAR"].ToString().Trim();

                if (Cargos != "")
                {
                    ordenServicio.Cargos = float.Parse(dr["FDSEVTACAR"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Cargos = (float.Parse("0")).ToString("####0.00");
                }

                string Descuentos = dr["FDSEVTADES"].ToString().Trim();

                if (Descuentos != "")
                {
                    ordenServicio.Descuentos = float.Parse(dr["FDSEVTADES"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Descuentos = (float.Parse("0")).ToString("####0.00");
                }

                string Iva = dr["FDSEIVAPRE"].ToString().Trim();

                if (Iva != "0")
                {
                    ordenServicio.Iva = float.Parse(dr["FDSEIVAPRE"].ToString().Trim()).ToString("####0.00");
                }
                else
                {
                    ordenServicio.Iva = (float.Parse("0")).ToString("####0.00");
                }

                ordenServicio.Total = (float.Parse(ordenServicio.Subtotal) + float.Parse(ordenServicio.Iva)).ToString("####0.00");

                coleccionOrdenesServicio.Add(ordenServicio);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionOrdenesServicio));
            return coleccionOrdenesServicio;
        }

        [Route("api/Servicio/GetObtenerOperacionesOrdenServicio", Name = "GetObtenerOperacionesOrdenServicio")]
        public List<OperacionOrdenServicio> GetObtenerOperacionesOrdenServicio(string IdAgencia, string IdPreOrden)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT * ";
            strSql += " FROM	" + constantes.Ambiente + "SERV.SEDOPERP ";
            strSql += " WHERE FISESTATUS = 1";
            strSql += " AND FDSENUMUT >= 0.01 ";
            strSql += " AND FISEIDCIAU = " + IdAgencia;
            strSql += " AND FISEIDPREO = " + IdPreOrden;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataView dtX = dt.DefaultView;
            dt = dtX.ToTable();

            OperacionOrdenServicio operacionOrdenServicio;
            List<OperacionOrdenServicio> coleccionOperacionesServicio = new List<OperacionOrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                operacionOrdenServicio = new OperacionOrdenServicio();
                operacionOrdenServicio.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                operacionOrdenServicio.IdPreorden = dr["FISEIDPREO"].ToString().Trim();
                operacionOrdenServicio.IdOperacion = dr["FISEIOPERA"].ToString().Trim();
                operacionOrdenServicio.DescOperacion = dr["FSSECVEOPE"].ToString().Trim() + " - " + dr["FSSEDESOPE"].ToString().Trim();
                operacionOrdenServicio.NumUnidades = float.Parse(dr["FDSENUMUT"].ToString().Trim()).ToString("####0.00");
                operacionOrdenServicio.PrecioVentaPorUnidades = float.Parse(dr["FDSEPVTAUT"].ToString().Trim()).ToString("####0.00");
                operacionOrdenServicio.PrecioTotal = (float.Parse((dr["FDSENUMUT"]).ToString()) * float.Parse((dr["FDSEPVTAUT"]).ToString())).ToString("####0.00");
                coleccionOperacionesServicio.Add(operacionOrdenServicio);
            }

            strSql = "";
            strSql = "SELECT * ";
            strSql += " FROM	" + constantes.Ambiente + "SERV.SEDTOTPR ";
            strSql += " WHERE FISESTATUS = 1";
            strSql += " AND FISEIDCIAU = " + IdAgencia;
            strSql += " AND FISEIDPREO = " + IdPreOrden;

            dt = dbCnx.GetDataSet(strSql).Tables[0];

            dtX = dt.DefaultView;
            dt = dtX.ToTable();

            foreach (DataRow dr in dt.Rows)
            {
                operacionOrdenServicio = new OperacionOrdenServicio();
                operacionOrdenServicio.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                operacionOrdenServicio.IdPreorden = dr["FISEIDPREO"].ToString().Trim();
                operacionOrdenServicio.IdOperacion = dr["FISEIOPERA"].ToString().Trim();
                operacionOrdenServicio.DescOperacion = dr["FSSECVEOPE"].ToString().Trim() + " - " + dr["FSSEDESOPE"].ToString().Trim();
                operacionOrdenServicio.NumUnidades = float.Parse(dr["FDSENUMUT"].ToString().Trim()).ToString("####0.00");
                operacionOrdenServicio.PrecioVentaPorUnidades = float.Parse(dr["FDSEPVENTA"].ToString().Trim()).ToString("####0.00");
                operacionOrdenServicio.PrecioTotal = (float.Parse((dr["FDSENUMUT"]).ToString()) * float.Parse((dr["FDSEPVENTA"]).ToString())).ToString("####0.00");
                coleccionOperacionesServicio.Add(operacionOrdenServicio);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionOperacionesServicio));
            return coleccionOperacionesServicio;
        }

        [Route("api/Servicio/GetObtenerResumenOrdenServicio", Name = "GetObtenerResumenOrdenServicio")]
        public RespuestaTest<ResumenOrdenServicio> GetObtenerResumenOrdenServicio(string IdAgencia, string IdPreOrden)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            #region strSql
            string strSql = "";
            /*strSql = "SELECT ";
            strSql += "(SELECT SUM(REFAP.FDSEPVENTA * REFAP.FDSECANREF) ";
            strSql += " FROM " + constantes.Ambiente + "SERV.SEDREFAP REFAP ";
            strSql += " WHERE REFAP.FISESTATUS = 1 ";
            strSql += " AND REFAP.FISEIDCIAU = AGE.FIGEIDCIAU ";
            strSql += " AND REFAP.FISEIDPREO = " + IdPreOrden + ") REFAC, ";
            strSql += "(SELECT SUM(OPERP.FDSEPVTAUT * OPERP.FDSENUMUT) ";
            strSql += " FROM " + constantes.Ambiente + "SERV.SEDOPERP OPERP ";
            strSql += " WHERE OPERP.FISESTATUS = 1 ";
            strSql += " AND OPERP.FISEIDCIAU = AGE.FIGEIDCIAU ";
            strSql += " AND OPERP.FISEIDPREO = " + IdPreOrden + ") OPERA, ";
            strSql += "(SELECT PARAM.FSGEVALOR ";
            strSql += " FROM	" + constantes.Ambiente + "GRAL.GEDPARAM PARAM ";
            strSql += " WHERE FIGESTATUS = 1";
            strSql += " AND PARAM.FIGEIDCIAU = AGE.FIGEIDCIAU ";
            strSql += " AND PARAM.FIGEINDIP = 6) IVA ";
            strSql += " FROM	" + constantes.Ambiente + "GRAL.GECCIAUN AGE ";
            strSql += " WHERE AGE.FIGEIDCIAU = " + IdAgencia;*/

            strSql = "SELECT *";
            strSql += " FROM " + constantes.Ambiente + "SERV.SEEPREOR ";
            strSql += " WHERE FISESTATUS = 1 ";
            strSql += " AND FISEIDPREO = " + IdPreOrden;
            strSql += " AND FISEIDCIAU = " + IdAgencia;
            #endregion

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            RespuestaTest<ResumenOrdenServicio> respuesta = new RespuestaTest<ResumenOrdenServicio>();

            DataView dtX = dt.DefaultView;
            dt = dtX.ToTable();

            if (dt.Rows.Count == 0)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se encontró un resumen para la órden de servicio.";
                return respuesta;
            }

            ResumenOrdenServicio resumenOrdenServicio;
            DataRow dr = dt.Rows[0];
            resumenOrdenServicio = new ResumenOrdenServicio();
            /*resumenOrdenServicio.IdAgencia = IdAgencia;
            resumenOrdenServicio.IdPreorden = IdPreOrden;

            String OPERA = float.Parse(dr["OPERA"].ToString().Trim()).ToString("####0.00");
            String REFAC = float.Parse(dr["REFAC"].ToString().Trim()).ToString("####0.00");

            if (OPERA == "")
            {
                OPERA = "0";
            }

            if (REFAC == "")
            {
                REFAC = "0";
            }

            resumenOrdenServicio.TotalOperaciones = OPERA;
            resumenOrdenServicio.TotalRefacciones = REFAC;


            resumenOrdenServicio.Subtotal = (float.Parse(OPERA) + float.Parse(REFAC)).ToString("####0.00");
            String IVA = dr["IVA"].ToString().Trim();
            resumenOrdenServicio.Iva = (float.Parse(resumenOrdenServicio.Subtotal) * float.Parse(IVA)).ToString("####0.00");
            resumenOrdenServicio.Total = (float.Parse(resumenOrdenServicio.Subtotal) * float.Parse("1." + IVA)).ToString("####0.00");*/

            resumenOrdenServicio.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
            resumenOrdenServicio.IdPreorden = dr["FISEIDPREO"].ToString().Trim();

            string Operaciones = dr["FDSEVTAOPE"].ToString().Trim();

            if (Operaciones != "")
            {
                resumenOrdenServicio.Operaciones = (float.Parse(dr["FDSEVTAOPE"].ToString().Trim())).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Operaciones = (float.Parse("0")).ToString("####0.00");
            }

            string Paquetes = dr["FDSEVTAPAQ"].ToString().Trim();

            if (Paquetes != "")
            {
                resumenOrdenServicio.Paquetes = float.Parse(dr["FDSEVTAPAQ"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Paquetes = (float.Parse("0")).ToString("####0.00");
            }

            string OtrosTrabajos = dr["FDSEVTAOTA"].ToString().Trim();

            if (OtrosTrabajos != "")
            {
                resumenOrdenServicio.OtrosTrabajos = float.Parse(dr["FDSEVTAOTA"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.OtrosTrabajos = (float.Parse("0")).ToString("####0.00");
            }

            string refacciones = dr["FDSEVTAREF"].ToString().Trim();

            if (refacciones != "")
            {
                resumenOrdenServicio.Refacciones = float.Parse(dr["FDSEVTAREF"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Refacciones = (float.Parse("0")).ToString("####0.00");
            }

            string Subtotal = dr["FDSEIMPPRE"].ToString().Trim();

            if (Subtotal != "")
            {
                resumenOrdenServicio.Subtotal = float.Parse(dr["FDSEIMPPRE"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Subtotal = (float.Parse("0")).ToString("####0.00");
            }

            string Cargos = dr["FDSEVTACAR"].ToString().Trim();

            if (Cargos != "")
            {
                resumenOrdenServicio.Cargos = float.Parse(dr["FDSEVTACAR"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Cargos = (float.Parse("0")).ToString("####0.00");
            }

            string Descuentos = dr["FDSEVTADES"].ToString().Trim();

            if (Descuentos != "")
            {
                resumenOrdenServicio.Descuentos = float.Parse(dr["FDSEVTADES"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Descuentos = (float.Parse("0")).ToString("####0.00");
            }

            string Iva = dr["FDSEIVAPRE"].ToString().Trim();

            if (Iva != "0")
            {
                resumenOrdenServicio.Iva = float.Parse(dr["FDSEIVAPRE"].ToString().Trim()).ToString("####0.00");
            }
            else
            {
                resumenOrdenServicio.Iva = (float.Parse("0")).ToString("####0.00");
            }

            resumenOrdenServicio.Total = (float.Parse(resumenOrdenServicio.Subtotal) + float.Parse(resumenOrdenServicio.Iva)).ToString("####0.00");

            respuesta.Ok = "SI";
            respuesta.Objeto = resumenOrdenServicio;

            return respuesta;
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionResumenServicio));
        }

        
        [Route("api/Servicio/GetObtenerRefaccionesOrdenServicio", Name = "GetObtenerRefaccionesOrdenServicio")]
        public List<RefaccionOrdenServicio> GetObtenerRefaccionesOrdenServicio(string IdAgencia, string IdPreOrden)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT * ";
            strSql += " FROM	" + constantes.Ambiente + "SERV.SEDREFAP ";
            strSql += " WHERE FISESTATUS = 1";
            strSql += " AND FISEIDCIAU = " + IdAgencia;
            strSql += " AND FISEIDPREO = " + IdPreOrden;

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataView dtX = dt.DefaultView;
            dt = dtX.ToTable();

            RefaccionOrdenServicio refaccionOrdenServicio;
            List<RefaccionOrdenServicio> coleccionOperacionesServicio = new List<RefaccionOrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                refaccionOrdenServicio = new RefaccionOrdenServicio();
                refaccionOrdenServicio.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                refaccionOrdenServicio.IdPreorden = dr["FISEIDPREO"].ToString().Trim();
                refaccionOrdenServicio.NumeroParte = dr["FSSENUMPAR"].ToString().Trim();
                refaccionOrdenServicio.DescRefaccion = dr["FSSENUMPAR"].ToString().Trim() + " - " + dr["FSSEDESREF"].ToString().Trim();
                refaccionOrdenServicio.Cantidad = float.Parse(dr["FDSECANREF"].ToString().Trim()).ToString("####0.00");
                refaccionOrdenServicio.PrecioVenta = float.Parse(dr["FDSEPVECDE"].ToString().Trim()).ToString("####0.00");
                refaccionOrdenServicio.PrecioTotal = (float.Parse((dr["FDSECANREF"]).ToString()) * float.Parse((dr["FDSEPVECDE"]).ToString())).ToString("####0.00");
                coleccionOperacionesServicio.Add(refaccionOrdenServicio);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionOperacionesServicio));
            return coleccionOperacionesServicio;
        }

        
        [Route("api/Servicio/GetObtenerPrimeraFechaDisponible", Name = "GetObtenerPrimeraFechaDisponible")]
        public RespuestaTest<String> GetObtenerPrimeraFechaDisponible(String IdAgencia)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            RespuestaTest<String> respuesta = new RespuestaTest<String>();
            DateTime Hoy = DateTime.Today;
            string FechaHoy = Hoy.ToString("yyyy-MM-dd");
            DateTime Futuro = Hoy.AddDays(30);

            string FechaFuturo = Futuro.ToString("yyyy-MM-dd");

            string strSql = "";
            strSql = "SELECT MIN(FFSEFECINC) MINIMO FROM  " + constantes.Ambiente + "SERV.SEDCITAS DETCIT ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEECITAS CIT ";
            strSql += "ON CIT.FISEIDCITA = DETCIT.FISEIDCITA ";
            strSql += "AND CIT.FISEIDCIAU = DETCIT.FISEIDCIAU ";
            strSql += "WHERE CIT.FFSEFECINC IN ('" + FechaHoy + "','" + FechaFuturo + "') ";
            strSql += "AND CIT.FISEIDCIAU = " + IdAgencia;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            string fecha = dt.Rows[0]["MINIMO"].ToString().Trim();
            //"DataSource=192.192.192.2;UserID=USYSDEVEL;Password=JCAJI2007;MaximumUseCount=100000;Pooling=false;ConnectionTimeout=600;DefaultIsolationLevel=ReadUncommitted;"

            if (fecha != "")
            {
                respuesta.Ok = "SI";
                respuesta.Objeto = DateTime.Parse(fecha).ToString("dd-MM-yyyy");
            }
            else
            {
                respuesta.Ok = "SI";
                respuesta.Objeto = Hoy.ToString("dd-MM-yyyy");
            }
            return respuesta;


        }

        // GET api/Servicio/GetObtenerOrdenesServicioProceso
        [Route("api/Servicio/GetObtenerOrdenesServicioProceso", Name = "GetObtenerOrdenesServicioProceso")]
        public List<OrdenServicio> GetObtenerOrdenesServicioProceso(long IdPersona, string IdsVehiculos)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT PREOR.FISEIDCIAU, PREOR.FISEIDPERS, PREOR.FISEIDPREO, PREOR.FISEODOMET, PREOR.FSSEDESTIP, PREOR.FISEIDVEHI, ";
            strSql += "ORDEN.FISEFOLIO, ORDEN.FFSEFECPRO, ORDEN.FHSEHRAPRO, ORDEN.FFSEFECING, ORDEN.FHSEHORING, ";
            strSql += "TRIM(STSOR.FSSEDESEST) FSSEDESEST, ";
            strSql += "TRIM(ASESO.FSSENOMBRE) || ' ' || TRIM(ASESO.FSSEAPELPA) || ' ' || TRIM(ASESO.FSSEAPELMA) ASESOR, ";
            strSql += "TRIM(VEHIC.FSVEHIPLA) FSVEHIPLA, ";
            strSql += "TRIM(VERSI.FSVERSDES) FSVERSDES, VERSI.FNVERSANO, ";
            strSql += "TRIM(MODEL.FSMODECLA) FSMODECLA, ";
            strSql += "TRIM(CIAUN.FSGERAZSOC) FSGERAZSOC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
            strSql += "ON PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECSTSOR STSOR ";
            strSql += "ON ORDEN.FISEIDESTA = STSOR.FISEIDESTA ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
            strSql += "ON PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
            strSql += "AND PREOR.FISEIDASES = ASESO.FISEIDASES ";
            strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
            strSql += "ON PREOR.FISEIDVEHI = VEHIC.FNVEHICID ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
            strSql += "ON VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
            strSql += "ON VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
            strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN CIAUN ";
            strSql += "ON PREOR.FISEIDCIAU = CIAUN.FIGEIDCIAU ";
            strSql += "WHERE PREOR.FISEIDPERS = " + IdPersona.ToString() + " ";
            strSql += "AND PREOR.FISEIDVEHI IN (" + IdsVehiculos + ") ";
            strSql += "AND ORDEN.FISEIDESTA IN (" + constantes.CA_SE_STATUS_ORDEN_EN_TALLER_O_PROCESO.ToString() +
                "," + constantes.CA_SE_STATUS_ORDEN_PREFACTURADA.ToString() + ") ";
            strSql += "AND ORDEN.FISESTATUS = 1 ";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            /*String RutaArchivo = obtenerURLServidor() + "Recursos/Json/" + "OrdenesProceso.json";

            String strJSON = leerArchivoWeb(RutaArchivo);

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(strJSON);

            String RowFilterText = "";
            RowFilterText = "IdPersona = " + IdPersona.ToString() + " AND IdVehiculo IN (" + idsVehiculos + ")";

            dt.DefaultView.RowFilter = "";
            dt.DefaultView.RowFilter = RowFilterText;*/
            DataView dtX = dt.DefaultView;
            //dtX.Sort = "DescripcionMarca";
            dt = dtX.ToTable();

            OrdenServicio ordenServicio;
            List<OrdenServicio> coleccionOrdenesServicio = new List<OrdenServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                ordenServicio = new OrdenServicio();
                ordenServicio.IdEmpresa = dr["FISEIDCIAU"].ToString().Trim();
                ordenServicio.IdPreOrden = dr["FISEIDPREO"].ToString().Trim();
                ordenServicio.Odometro = dr["FISEODOMET"].ToString().Trim();
                ordenServicio.TipoOrden = dr["FSSEDESTIP"].ToString().Trim();
                ordenServicio.NumeroOrden = dr["FISEFOLIO"].ToString().Trim();
                ordenServicio.FechaPromesa = Convert.ToDateTime(dr["FFSEFECPRO"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraPromesa = Convert.ToDateTime(dr["FHSEHRAPRO"]).ToString("HH:mm:ss");
                ordenServicio.FechaIngreso = Convert.ToDateTime(dr["FFSEFECING"]).ToString("dd-MM-yyyy");
                ordenServicio.HoraIngreso = Convert.ToDateTime(dr["FHSEHORING"]).ToString("HH:mm:ss");
                ordenServicio.Estado = dr["FSSEDESEST"].ToString().Trim();
                ordenServicio.Asesor = dr["ASESOR"].ToString().Trim();
                ordenServicio.Placas = dr["FSVEHIPLA"].ToString().Trim();
                ordenServicio.Version = dr["FSVERSDES"].ToString().Trim();
                ordenServicio.Anio = dr["FNVERSANO"].ToString().Trim();
                ordenServicio.Modelo = dr["FSMODECLA"].ToString().Trim();
                ordenServicio.Empresa = dr["FSGERAZSOC"].ToString().Trim();
                coleccionOrdenesServicio.Add(ordenServicio);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionOrdenesServicio));
            return coleccionOrdenesServicio;
        }

        public MultimediaSeguimientoOrden GetObtenerMultimediaSeguimientoPreorden(String IdSeguimientoOrden)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDSFVMB SEGUI ";
            strSql += "WHERE SEGUI.FIAPSTATUS = 1 ";
            strSql += " AND SEGUI.FIAPIDSEGO = " + IdSeguimientoOrden;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            DataView dtX = dt.DefaultView;
            dtX.Sort = "FIAPIDTIPS";
            dt = dtX.ToTable();

            MultimediaSeguimientoOrden seguimiento;
            seguimiento = new MultimediaSeguimientoOrden();
            ImagenSeguimientoOrden imagen;
            VideoSeguimientoOrden video;

            List<ImagenSeguimientoOrden> Imagenes = new List<ImagenSeguimientoOrden>();
            List<VideoSeguimientoOrden> Videos = new List<VideoSeguimientoOrden>();

            if (dt.Rows.Count > 0)
            {

                seguimiento.IdSegOrden = dt.Rows[0]["FIAPIDSEGO"].ToString().Trim();
                seguimiento.Fecha = Convert.ToDateTime(dt.Rows[0]["FFAPFECHA"]).ToString("dd-MM-yyyy");
                seguimiento.Hora = Convert.ToDateTime(dt.Rows[0]["FHAPHORA"]).ToString("HH:mm:ss");

                foreach (DataRow dr in dt.Rows)
                {
                    String TipoSeguimiento = dr["FIAPIDTIPS"].ToString().Trim();
                    if (TipoSeguimiento == "1")
                    {
                        imagen = new ImagenSeguimientoOrden();

                        imagen.IdConsecutivo = dr["FIAPIDCONS"].ToString().Trim();
                        imagen.IdTipoSeguimiento = TipoSeguimiento;
                        imagen.RutaMedia = dr["FSAPRUTMED"].ToString().Trim();
                        imagen.RutaMini = dr["FSAPRUMEMI"].ToString().Trim();

                        Imagenes.Add(imagen);
                    }
                    else if (TipoSeguimiento == "2")
                    {
                        video = new VideoSeguimientoOrden();

                        video.IdConsecutivo = dr["FIAPIDCONS"].ToString().Trim();
                        video.IdTipoSeguimiento = TipoSeguimiento;
                        video.RutaMedia = dr["FSAPRUTMED"].ToString().Trim();
                        video.RutaMini = dr["FSAPRUMEMI"].ToString().Trim();

                        Videos.Add(video);
                    }


                }
                seguimiento.Imagenes = Imagenes;
                seguimiento.Videos = Videos;
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionSeguimiento));
            return seguimiento;
        }


        [Route("api/Servicio/GetObtenerFacturaPreorden", Name = "GetObtenerFacturaPreorden")]
        public RespuestaTest<string> GetObtenerFacturaPreorden(string IdAgencia, string IdPreOrden, string IdCliente, string Folio, string Correo, string Prefijo)

        {

            DVAModelsReflection.DB2Database _db = new DVAModelsReflection.DB2Database();
            
            List<String> DocumentoFiscal = new List<string>();
            
            string respuesta = "";
            var respuestaServidor = new RespuestaTest<string>();
            
            try
            {

                DVAFacturacion.FacturaCFD factura = new DVAFacturacion.FacturaCFD(_db, int.Parse(IdAgencia), int.Parse(IdCliente));
                DocumentoFiscal = (factura.GetDocumentoFiscal(Prefijo, int.Parse(Folio)));
                
                List<RespuestaServidor> coleccionRespuestas = new List<RespuestaServidor>();
                

                if (DocumentoFiscal.Count > 0)

                {

                    respuesta = DocumentoFiscal[0];
                    string base64 = String.Empty;

                    //_application.ActiveDocument.Save();                    
                    //string docPath = _application.ActiveDocument.FullName;

                    var webClient = new WebClient();
                    byte[] binarydata = webClient.DownloadData(respuesta);

                    //byte[] binarydata = File.ReadAllBytes(respuesta);
                    base64 = System.Convert.ToBase64String(binarydata, 0, binarydata.Length);                    
                    byte[] bytes = Convert.FromBase64String(base64);
                    
                    string mailFrom = "notificaciones@grupoautofin.com";
                    string password = "RXPJPJJ2013llx";
                    string smtpServidor = "smtp.office365.com";


                    string mensaje = "Estimado(a) Cliente\n\n ";
                    mensaje += "Ha solicitado un archivo de Factura. " + "\n";
                    mensaje += "Se adjunta el archivo en el presente correo.\n\n";
                    mensaje += "Gracias por utilizar nuestros servicios.";
                    string subject = "Solicitud de Factura";                    

                    SmtpClient client = new SmtpClient(smtpServidor, 587);
                    MailAddress from = new MailAddress(mailFrom);
                    MailAddress to = new MailAddress(Correo);
                    MailMessage message = new MailMessage(from, to);


                    message.Body = mensaje;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = subject;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    
                    client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                    client.EnableSsl = true;
                    Attachment attachment;
                    

                    Stream prueba = new MemoryStream(bytes);
                    //attachment = new Attachment(Server.MapPath("PDFtemp.pdf"));
                    attachment = new Attachment(prueba, Prefijo + "_" + Folio + ".pdf");
                    message.Attachments.Add(attachment);
                    
                    client.Send(message);
                    respuesta = base64;

                    respuestaServidor.Ok = "SI";

                }
                else
                {
                    respuestaServidor.Ok = "NO";
                    respuesta = "Archivo no encontrado";
                }

                respuestaServidor.Objeto = respuesta;
                return respuestaServidor;

                //File.Delete(Server.MapPath("PDFtemp.pdf"));

            }

            catch (Exception e)
            {
                respuestaServidor.Mensaje = e.Message;
                respuestaServidor.Ok = "NO";
                JavaScriptSerializer jss = new JavaScriptSerializer();

                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));
                return respuestaServidor;

            }
        }


        [Route("api/Servicio/ObtenerCotizacionSeguroAuto", Name = "ObtenerCotizacionSeguroAuto")]
        public List<CotizacionSeguro> ObtenerCotizacionSeguroAuto(string IdVersion, string ValorFactura, string Aseguradora, string Paquete, string CP, string Correo, string Anio)
        {
            String respuesta = "";

            DateTime Hoy = DateTime.Today;

            string TipoUsoSeguro = "";
            string ValorComercialSeguro = ""; ;
            string CodigoPostalSeguro = "";
            string MarcaSeguro = "";
            string ModeloSeguro = "";
            string AnioSeguro = "";
            string VersionSeguro = "";
            string PaqueteSeguro = "";
            string AseguradoraSeguro = "";
            string CotizacionSeguro = "";

            AnioSeguro = Anio;
            CodigoPostalSeguro = CP;
            AseguradoraSeguro = Aseguradora;
            PaqueteSeguro = Paquete;



            MarcaSeguro = "";
            ModeloSeguro = "";

            VersionSeguro = "";


            CotizacionSeguro = "";

            string VigenciaInicial = Hoy.ToString("dd-MM-yyyy");
            string CondicionDePago = "CNT";

            string anioActual = Hoy.ToString("yyyy");
            string anioSiguiente = (int.Parse(anioActual) + 1).ToString();
            string anioAnterior = (int.Parse(anioActual) - 1).ToString();

            string anioActualMenos2 = (int.Parse(anioActual) - 2).ToString();

            string Uso = "";

            switch (Aseguradora)
            {
                case "MAPFRE":

                    Aseguradora = "MPF";

                    break;

                case "QUALITAS":

                    Aseguradora = "QLT";

                    break;
            }

            /*Error en la asignación de valores Uso(Particular-Comercial) o ValorAsegurado(Factura-Comercial)*/
            //if (Anio == anioActual || Anio == anioSiguiente || Anio == anioAnterior)
            //{
            //    Uso = "1";
            //    TipoUsoSeguro = "PARTICULAR";
            //}

            //else if (int.Parse(Anio) <= int.Parse(anioActualMenos2))
            //{
            //    Uso = "2";
            //    TipoUsoSeguro = "COMERCIAL";
            //    ValorFactura = "0";

            //}
            //else
            //{
            //    Uso = "1";
            //    TipoUsoSeguro = "PARTICULAR";
            //    ValorFactura = "0";
            //}
            Uso = "1";//Particular o Comercial, sólo funciona para particular
            TipoUsoSeguro = "PARTICULAR";
            ValorFactura = "0";

            ValorComercialSeguro = ValorFactura;

            if (Uso != "")
            {

                try
                {
                    //comentado servicio not found MG
                    //WsCotiz.CotizacionesSegurosSoapClient sesion = new WsCotiz.CotizacionesSegurosSoapClient();
                    //respuesta = sesion.Cotiza(int.Parse(Uso), int.Parse(IdVersion), decimal.Parse(ValorFactura), Aseguradora, Paquete, Hoy, CondicionDePago, int.Parse(CP));

                    if (!respuesta.Contains("hubo un error"))
                    {


                        //XDocument xDocu = XDocument.Parse(respuesta.Replace('\"', '"'));


                        var xml = XElement.Parse(respuesta.Replace('\"', '"'));

                        int total = 0;

                        if (Aseguradora == "MPF")
                        {
                            total = xml.Descendants("data").Descendants().Count();
                        }
                        if (Aseguradora == "QLT")
                        {
                            total = xml.Descendants().Descendants().Count();
                        }
                        if (Aseguradora == "GNP")
                        {
                            total = xml.Descendants().Descendants().Count();
                        }

                        string result = "";

                        if (Aseguradora == "MPF")
                        {
                            result += "COBERTURAS: " + "\n\n";
                        }

                        for (int x = 0; x < total; x++)
                        {

                            string name = "";
                            string value = "";

                            if (Aseguradora == "MPF")
                            {
                                name = xml.Descendants("data").Descendants().ElementAt(x).Name.ToString();
                                value = xml.Descendants("data").Descendants().ElementAt(x).Value.ToString();
                            }
                            else
                            {
                                name = xml.Descendants().Descendants().ElementAt(x).Name.ToString();
                                value = xml.Descendants().Descendants().ElementAt(x).Value.ToString();
                            }

                            /*if (name == "COD_COB")
                            {
                                result += "CÓDIGO DE COBERTURA: " + value + "\n";
                            }*/

                            if (Aseguradora == "MPF")
                            {

                                if (name == "COBERTURA")
                                {
                                    result += "TIPO: " + value + "\n";
                                }

                                if (name == "LIMMAXRESP")
                                {
                                    if (value != "0")
                                    {
                                        result += "LÍMITE MÁXIMO DE RESPONSABILIDAD: $" + value + "\n";
                                    }
                                    else
                                    {
                                        result += "LÍMITE MÁXIMO DE RESPONSABILIDAD: AMPARADO\n";
                                    }
                                }

                                if (name == "DEDUCIBLE")
                                {
                                    if (value != "")
                                    {
                                        result += "DEDUCIBLE: " + value + "%\n";
                                    }
                                    else
                                    {
                                        result += "DEDUCIBLE: 0%\n";
                                    }
                                }

                                if (name == "PRIMAS")
                                {
                                    result += "PRIMAS: $" + value + "\n\n";
                                }

                                if (name == "Totales")
                                {
                                    result += "\nTOTALES: " + "\n";
                                }

                                if (name == "total_prima_neta")
                                {
                                    result += "Total prima neta: $" + value + "\n";
                                }

                                if (name == "total_derechos")
                                {
                                    result += "Total derechos: $" + value + "\n";
                                }

                                if (name == "total_recargos")
                                {
                                    result += "Total recargos: $" + value + "\n";
                                }

                                if (name == "total_iva")
                                {
                                    result += "Total IVA: $" + value + "\n";
                                }

                                if (name == "prima_total")
                                {
                                    result += "Prima total: $" + value + "\n";
                                    CotizacionSeguro = "$ " + value;
                                }
                            }

                            if (Aseguradora == "QLT")
                            {

                                if (name == "Nombre")
                                {
                                    result += "\nNombre: " + value + "\n";
                                }

                                if (name == "Direccion")
                                {
                                    result += "Direccion: " + value + "\n";
                                }

                                if (name == "Colonia")
                                {
                                    result += "Colonia: " + value + "\n";
                                }

                                if (name == "Estado")
                                {
                                    result += "Estado: " + value + "\n";
                                }

                                if (name == "CodigoPostal")
                                {
                                    result += "CodigoPostal: " + value + "\n";
                                }

                                if (name == "ClaveAmis")
                                {
                                    result += "Clave Amis: " + value + "\n";
                                }

                                if (name == "Modelo")
                                {
                                    result += "CodigoPostal: " + value + "\n";
                                }

                                if (name == "DescripcionVehiculo")
                                {
                                    result += "DescripcionVehiculo: " + value + "\n";
                                }
                                if (name == "Uso")
                                {
                                    result += "Uso: " + value + "\n";
                                }

                                if (name == "Servicio")
                                {
                                    result += "Servicio: " + value + "\n";
                                }

                                if (name == "Paquete")
                                {
                                    result += "Paquete: " + value + "\n";
                                }

                                if (name == "Motor")
                                {
                                    result += "Motor: " + value + "\n";
                                }

                                if (name == "Serie")
                                {
                                    result += "Serie: " + value + "\n\n";
                                }

                                if (name == "SumaAsegurada")
                                {
                                    result += "SumaAsegurada: " + value + "\n";
                                }

                                if (name == "TipoSuma")
                                {
                                    result += "TipoSuma: " + value + "\n";
                                }

                                if (name == "Deducible")
                                {
                                    result += "Deducible: " + value + "\n";
                                }

                                if (name == "Prima")
                                {
                                    result += "Prima: " + value + "\n\n";
                                }

                                if (name == "FechaEmision")
                                {
                                    result += "FechaEmision: " + value + "\n";
                                }

                                if (name == "PorcentajeDescuento")
                                {
                                    result += "PorcentajeDescuento: " + value + "\n";
                                }

                                if (name == "PrimaNeta")
                                {
                                    result += "PrimaNeta: " + value + "\n\n";
                                }

                                if (name == "Derecho")
                                {
                                    result += "Derecho: " + value + "\n";
                                }

                                if (name == "Recargo")
                                {
                                    result += "Recargo: " + value + "\n";
                                }

                                if (name == "PrimaTotal")
                                {
                                    result += "PrimaTotal: " + value + "\n";
                                    CotizacionSeguro = value;
                                }

                                if (name == "Comision")
                                {
                                    result += "Comision: " + value + "\n";
                                }

                                if (name == "CodigoError")
                                {
                                    break;
                                }

                            }

                            if (Aseguradora == "GNP")
                            {

                                /*if (name == "numeroPrepoliza")
                                {
                                    result += "\nNúmero Prepoliza: " + "\n";
                                }

                                if (name == "estatus")
                                {
                                    result += "Estatus: " + value + "\n";
                                }

                                if (name == "ramo")
                                {
                                    result += "Ramo: " + value + "\n";
                                }

                                if (name == "codigoClienteContratante")
                                {
                                    result += "Código Cliente Contratante: " + value + "\n";
                                }

                                if (name == "fechaGeneracion")
                                {
                                    result += "Fecha Generación: " + value + "\n";
                                }

                                if (name == "nombre")
                                {
                                    result += "Nombre: " + value + "\n";
                                }

                                if (name == "APaterno")
                                {
                                    result += "Apellido Paterno: " + value + "\n";
                                }

                                if (name == "AMaterno")
                                {
                                    result += "Apellido Materno: " + value + "\n";
                                }

                                if (name == "fechaNacimiento")
                                {
                                    result += "Recargo: " + value + "\n";
                                }

                                if (name == "PorcentajeIVA")
                                {
                                    result += "Porcentaje IVA: " + value + "\n";
                                }

                                if (name == "IVA")
                                {
                                    result += "IVA: " + value + "\n";
                                }

                                if (name == "PrimaTotal")
                                {
                                    result += "Prima total: " + value + "\n";
                                }

                                if (name == "Poliza")
                                {
                                    result += "Poliza: " + value + "\n";
                                }

                                if (name == "Serie")
                                {
                                    result += "Serie: " + value + "\n";
                                }

                                if (name == "Comision")
                                {
                                    result += "Comisión: " + value + "\n";
                                }

                                if (name == "PorcentajeUDI")
                                {
                                    result += "Porcentaje UDI: " + value + "\n";
                                }

                                if (name == "UDI")
                                {
                                    result += "UDI: " + value + "\n";
                                }

                                if (name == "Estatus")
                                {
                                    result += "Estatus: " + value + "\n";
                                }*/
                            }
                        }

                        DVADB.DB2 dbCnx = new DVADB.DB2();
                        DVAConstants.Constants constantes = new DVAConstants.Constants();

                        string strSql = "";
                        strSql = "SELECT    * ";
                        strSql += "FROM	" + constantes.Ambiente + "AUT.ANCVERSI VERSI ";
                        strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCMODEL MODEL ";
                        strSql += "ON MODEL.FNMODEIDM = VERSI.FNVERSIDM ";
                        strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCFAMIL FAMIL ";
                        strSql += "ON FAMIL.FNFAMICLA = MODEL.FNMODEFAM ";
                        strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCMARCA MARCA ";
                        strSql += "ON MARCA.FNMARCCLA = FAMIL.FNFAMIMAR ";
                        strSql += "WHERE VERSI.FNVERSIDV = " + IdVersion;
                        strSql += " AND VERSI.FIANSTATU = 1 ";


                        DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                        if (dt.Rows.Count != 0)
                        {
                            MarcaSeguro = dt.Rows[0]["FSMARCDES"].ToString().Trim();
                            ModeloSeguro = dt.Rows[0]["FSMODECLA"].ToString().Trim();
                            VersionSeguro = dt.Rows[0]["FSVERSDES"].ToString().Trim();
                        }

                        String RutaArchivo = CorreoService.obtenerURLServidor() + "Recursos/Adjuntos/" + "SeguroHtml.txt";

                        String strHTML = CorreoService.leerArchivoWeb(RutaArchivo);

                        string RutaImagenPrincipal = CorreoService.obtenerURLServidor() + "Recursos/Adjuntos/" + "mi-auto-seguro-alt.jpg";
                        string RutaImagenMovil = CorreoService.obtenerURLServidor() + "Recursos/Adjuntos/" + "mi-auto-seguro-mobile.jpg";
                        string RutaImagenIcono = CorreoService.obtenerURLServidor() + "Recursos/Adjuntos/" + "mi-auto-seguro-icon.png";
                        string RutaImagenLogo = CorreoService.obtenerURLServidor() + "Recursos/Adjuntos/" + "mi-auto-seguro-logo.png";

                        strHTML = strHTML.Replace("[[TIPO_USO]]", TipoUsoSeguro);
                        strHTML = strHTML.Replace("[[VALOR_COMERCIAL]]", ValorComercialSeguro);
                        strHTML = strHTML.Replace("[[CP]]", CodigoPostalSeguro);
                        strHTML = strHTML.Replace("[[MARCA]]", MarcaSeguro);
                        strHTML = strHTML.Replace("[[MODELO]]", ModeloSeguro);
                        strHTML = strHTML.Replace("[[ANIO]]", AnioSeguro);
                        strHTML = strHTML.Replace("[[VERSION]]", VersionSeguro);
                        strHTML = strHTML.Replace("[[PAQUETE]]", PaqueteSeguro);
                        strHTML = strHTML.Replace("[[ASEGURADORA]]", AseguradoraSeguro);
                        strHTML = strHTML.Replace("[[COTIZACION]]", CotizacionSeguro);
                        strHTML = strHTML.Replace("http://imagen.mailing.divisionautomotriz.com/plataformas/mi-auto-seguro.jpg", RutaImagenPrincipal);
                        strHTML = strHTML.Replace("http://imagen.mailing.divisionautomotriz.com/plataformas/mi-auto-seguro-mobile.jpg", RutaImagenMovil);
                        strHTML = strHTML.Replace("http://imagen.mailing.divisionautomotriz.com/plataformas/mi-auto-seguro-icon.png", RutaImagenIcono);
                        strHTML = strHTML.Replace("http://imagen.mailing.divisionautomotriz.com/plataformas/mi-auto-seguro-logo.png", RutaImagenLogo);


                        CorreoService.EnviarCorreoCotizacionSeguro(Correo, strHTML);


                        /*string mailFrom = "notificaciones@grupoautofin.com";
                        string password = "RXPJPJJ2013llx";
                        string smtpServidor = "smtp.office365.com";
                        string mensaje = "Estimado(a) Cliente:\n\n";
                        mensaje += "Su cotización solicitada se presenta a continuación. " + "\n\n";
                        mensaje += result;
                        mensaje += "\n\n";
                        mensaje += "Gracias por utilizar nuestros servicios.";
                        string subject = "Cotización de Seguro";

                        SmtpClient client = new SmtpClient(smtpServidor, 587);
                        MailAddress from = new MailAddress(mailFrom);
                        MailAddress to = new MailAddress(Correo);
                        MailMessage message = new MailMessage(from, to);

                        message.Body = mensaje;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.Subject = subject;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;

                        client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                        client.EnableSsl = true;


                        client.Send(message);*/

                        respuesta = result;


                        CotizacionSeguro cotizacionSeguro;
                        List<CotizacionSeguro> coleccionRespuestas = new List<CotizacionSeguro>();

                        cotizacionSeguro = new CotizacionSeguro();
                        cotizacionSeguro.Respuesta = "SI";
                        cotizacionSeguro.Cotizacion = respuesta;

                        coleccionRespuestas.Add(cotizacionSeguro);

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        //Context.Response.ContentType = "application/json";
                        //Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));
                        return coleccionRespuestas;
                    }
                    else
                    {
                        respuesta = "Error al intentar recuperar cotización";

                        CotizacionSeguro cotizacionSeguro;
                        List<CotizacionSeguro> coleccionRespuestas = new List<CotizacionSeguro>();

                        cotizacionSeguro = new CotizacionSeguro();
                        cotizacionSeguro.Respuesta = "NO";
                        cotizacionSeguro.Cotizacion = respuesta;

                        coleccionRespuestas.Add(cotizacionSeguro);

                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        //Context.Response.ContentType = "application/json";
                        //Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));
                        return coleccionRespuestas;
                    }
                }



                catch (Exception e)
                {

                    respuesta = e.Message;

                    CotizacionSeguro cotizacionSeguro;
                    List<CotizacionSeguro> coleccionRespuestas = new List<CotizacionSeguro>();

                    cotizacionSeguro = new CotizacionSeguro();
                    cotizacionSeguro.Respuesta = "NO";
                    cotizacionSeguro.Cotizacion = respuesta;

                    coleccionRespuestas.Add(cotizacionSeguro);

                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    //Context.Response.ContentType = "application/json";
                    //Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));
                    return coleccionRespuestas;

                }
            }
            else
            {
                respuesta = "La cotización no se puede realizar con los parámetros enviados";

                CotizacionSeguro cotizacionSeguro;
                List<CotizacionSeguro> coleccionRespuestas = new List<CotizacionSeguro>();

                cotizacionSeguro = new CotizacionSeguro();
                cotizacionSeguro.Respuesta = "NO";
                cotizacionSeguro.Cotizacion = respuesta;

                coleccionRespuestas.Add(cotizacionSeguro);

                JavaScriptSerializer jss = new JavaScriptSerializer();
                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionRespuestas));
                return coleccionRespuestas;
            }
        }

        // POST api/Servicio/ObtenerChecklistInpeccion
        [Route("api/Servicio/ObtenerChecklistInpeccion", Name = "ObtenerChecklistInpeccion")]
        public List<CheckInspeccion> ObtenerChecklistInpeccion(int IdAgencia, long IdPreorden)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    CK.FISEIDCIAU, CK.FISEIDPREO, CK.FISEIDENCA, TRIM(CK.FSSEOBSERV) FSSEOBSERV, ";
            strSql += "CKD.FISEIDDETA, CKD.FISEIDCMCK, TRIM(CCK.FSSEDESCOM) FSSEDESCOM, TRIM(CCK.FSSEIMAGEN) ImagenInspeccion, ";
            strSql += "CKD.FISEESTCAM, CKD.FISEIDESTA, TRIM(ECK.FSSEDESEST) FSSEDESEST, TRIM(ECK.FSSEIMAGEN) ImagenEstado, ";
            strSql += "FOVI.FISEURLARC ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEEPRECK CK ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEDPRECK CKD ";
            strSql += "ON	CK.FISEIDCIAU = CKD.FISEIDCIAU ";
            strSql += "AND	CK.FISEIDENCA = CKD.FISEIDENCA ";
            strSql += "AND	CKD.FISESTATUS = 1 ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECCOMCK CCK ";
            strSql += "ON	CKD.FISEIDCMCK = CCK.FISEIDCMCK ";
            strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SECESTCK ECK ";
            strSql += "ON	CKD.FISEIDESTA = ECK.FISEIDESTA ";
            strSql += "LEFT JOIN " + constantes.Ambiente + "SERV.SEREFOVI FOVI ";
            strSql += "ON	CKD.FISEIDCIAU = FOVI.FISEIDCIAU ";
            strSql += "AND	CKD.FISEIDENCA = FOVI.FISEIDENCA ";
            strSql += "AND	CKD.FISEIDCMCK = FOVI.FISEIDCMCK ";
            strSql += "AND	FOVI.FISESTATUS = 1 ";
            strSql += "WHERE CK.FISESTATUS = 1 ";
            strSql += "AND	CK.FISEIDCIAU = " + IdAgencia.ToString() + " ";
            strSql += "AND	CK.FISEIDPREO = " + IdPreorden.ToString();
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            List<CheckInspeccion> coleccionCheckInspeccion = new List<CheckInspeccion>();
            DataTable dtCheck = dt.DefaultView.ToTable(true, new string[] { "FISEIDCIAU", "FISEIDPREO", "FISEIDENCA", "FSSEOBSERV" });
            if (dtCheck.Rows.Count == 1)
            {
                CheckInspeccion checkInspeccion = new CheckInspeccion();
                DataRow drCheck = dtCheck.Rows[0];
                checkInspeccion.IdCompania = drCheck["FISEIDCIAU"].ToString().Trim();
                checkInspeccion.IdPreOrden = drCheck["FISEIDPREO"].ToString().Trim();
                checkInspeccion.Id = drCheck["FISEIDENCA"].ToString().Trim();
                checkInspeccion.Observaciones = drCheck["FSSEOBSERV"].ToString().Trim();

                List<PuntoInspeccion> puntosInspeccion = new List<PuntoInspeccion>();
                DataTable dtPuntosInspeccion = dt.DefaultView.ToTable(true, new string[] { "FISEIDCIAU", "FISEIDPREO", "FISEIDENCA", "FSSEOBSERV", "FISEIDDETA", "FISEIDCMCK", "FSSEDESCOM", "ImagenInspeccion", "FISEESTCAM", "FISEIDESTA", "FSSEDESEST", "ImagenEstado" });
                foreach (DataRow drPuntoInspeccion in dtPuntosInspeccion.Rows)
                {
                    PuntoInspeccion puntoInspeccion = new PuntoInspeccion();
                    puntoInspeccion.Id = drPuntoInspeccion["FISEIDDETA"].ToString().Trim();
                    puntoInspeccion.IdInspeccion = drPuntoInspeccion["FISEIDCMCK"].ToString().Trim();
                    puntoInspeccion.Descripcion = drPuntoInspeccion["FSSEDESCOM"].ToString().Trim();
                    puntoInspeccion.ImagenInspeccion = drPuntoInspeccion["ImagenInspeccion"].ToString().Trim();
                    puntoInspeccion.PorcentajeVida = drPuntoInspeccion["FISEESTCAM"].ToString().Trim();
                    puntoInspeccion.Estado = drPuntoInspeccion["FSSEDESEST"].ToString().Trim();
                    puntoInspeccion.ImagenEstado = drPuntoInspeccion["ImagenEstado"].ToString().Trim();

                    dt.DefaultView.RowFilter = "FISEIDCMCK = " + puntoInspeccion.IdInspeccion;
                    DataTable dtMultimedia = dt.DefaultView.ToTable();
                    List<ImagenesMultimedia> imagenes = new List<ImagenesMultimedia>();
                    List<VideosMultimedia> videos = new List<VideosMultimedia>();
                    foreach (DataRow drMultimedia in dtMultimedia.Rows)
                    {
                        string multimedia = drMultimedia["FISEURLARC"].ToString().Trim();
                        if (multimedia.Contains(".jpg"))
                        {
                            ImagenesMultimedia imagen = new ImagenesMultimedia();
                            imagen.NombreRutaMedia = drMultimedia["FISEURLARC"].ToString().Trim();
                            imagenes.Add(imagen);
                        }
                        else if (multimedia.Contains(".mp4"))
                        {
                            VideosMultimedia video = new VideosMultimedia();
                            video.NombreRutaMedia = drMultimedia["FISEURLARC"].ToString().Trim();
                            videos.Add(video);
                        }
                    }

                    puntoInspeccion.Imagenes = imagenes;
                    puntoInspeccion.Videos = videos;

                    puntosInspeccion.Add(puntoInspeccion);
                }

                checkInspeccion.PuntosInspeccion = puntosInspeccion;
                coleccionCheckInspeccion.Add(checkInspeccion);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionCheckInspeccion));
            return coleccionCheckInspeccion;
        }


        [Route("api/Servicio/GetObtenerListadoAreasContacto", Name = "GetObtenerListadoAreasContacto")]
        public List<AreaContacto> GetObtenerListadoAreasContacto(string IdAgencia)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDACLST AREA ";
            strSql += "WHERE AREA.FIAPSTATUS = 1 ";
            strSql += "AND AREA.FIAPIDCIAU = " + IdAgencia;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            AreaContacto areaContacto;
            List<AreaContacto> coleccionAseguradoras = new List<AreaContacto>();
            foreach (DataRow dr in dt.Rows)
            {
                areaContacto = new AreaContacto();
                areaContacto.IdArea = dr["FIAPIDAREA"].ToString().Trim();
                areaContacto.Area = dr["FSAPAREA"].ToString().Trim();
                areaContacto.Correo = dr["FSAPCORREO"].ToString().Trim();
                coleccionAseguradoras.Add(areaContacto);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionAseguradoras));
            return coleccionAseguradoras;
        }

        // POST api/Servicio/ObtenerHorariosxIdAsesor
        [Route("api/Servicio/ObtenerHorariosxIdAsesor", Name = "ObtenerHorariosxIdAsesor")]
        public List<HorarioServicio> ObtenerHorariosxIdAsesor(string IdAgencia, string IdAsesor)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SEDHOCIA ";
            strSql += "WHERE FISESTATUS = 1 ";
            strSql += "AND FISEIDCIAU = " + IdAgencia;
            strSql += " AND FISEIDASES = " + IdAsesor;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            //DataView dtX = dt.DefaultView;
            //dtX.Sort = "FSGERAZSOC";
            //dt = dtX.ToTable();

            HorarioServicio horario;
            List<HorarioServicio> coleccionHorarios = new List<HorarioServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                horario = new HorarioServicio();
                horario.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                horario.IdAsesor = dr["FISEIDASES"].ToString().Trim();
                horario.IdDia = dr["FISEIDDIA"].ToString().Trim();
                horario.Rango = dr["FSSEDESRAN"].ToString().Trim();
                horario.HoraInicial = Convert.ToDateTime(dr["FHSERANGOI"]).ToString("HH:mm:ss");
                horario.HoraFinal = Convert.ToDateTime(dr["FHSERANGOF"]).ToString("HH:mm:ss");

                coleccionHorarios.Add(horario);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionHorarios));

            return coleccionHorarios;
        }
    }
}
