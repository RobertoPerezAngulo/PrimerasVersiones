using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using wsApiSeat.Models;

namespace wsApiSeat.Bussiness
{
    public class BUsuario
    {
     
        
        //public Respuesta PostResgitraClenteCuentaPedido(PedidoAll pedido)
        //{
        //    Respuesta respuesta = new Respuesta();
        //    DVADB.DB2 dbCnx = new DVADB.DB2();
        //    DVAConstants.Constants constantes = new DVAConstants.Constants();
        //    string jsonPedido = string.Empty;
        //    try
        //    {
        //        /*Cuenta cuenta = new Cuenta();
        //        cuenta = pedido.Cuenta;*/
        //        dbCnx.AbrirConexion();
        //        dbCnx.BeginTransaccion();
        //        long idClienteMoral = 0;
        //        RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

        //        #region INSERT CLIENTE
        //        #region INSERT PERSONA FISICA Y/O MORAL
        //        DVARegistraPersona.RegistraPersona persona = new DVARegistraPersona.RegistraPersona();
        //        persona.Almacen = string.Empty;
        //        persona.Nombre = pedido.Cuenta.Nombre;
        //        persona.ApellidoPaterno = pedido.Cuenta.ApellidoPaterno;
        //        persona.ApellidoMaterno = pedido.Cuenta.ApellidoMaterno;
        //        persona.TelefonoCasa = null;
        //        persona.NumeroCelular = pedido.Cuenta.NumeroMobil;
        //        persona.Email = pedido.Cuenta.Correo;
        //        /*if (pedido.DatosFiscales != null && !string.IsNullOrEmpty(pedido.DatosFiscales.Nombre)) {
        //            persona.RFC = pedido.DatosFiscales.Rfc;
        //        }*/

        //        string json = JsonConvert.SerializeObject(persona);
        //        string valor = "36|7244|1|" + DateTime.Now;

        //        DVAAutosystServerClasses.Seguridad.Seguridad seg = new DVAAutosystServerClasses.Seguridad.Seguridad();
        //        string token = seg.EncriptarCadena(valor);

        //        //string url = "http://localhost:31076/api/Persona/registrarApps/valor/39/7244/1";
        //        string url = "http://10.5.2.21:7070/wsRegistraPersona/api/Persona/registrarApps/valor/36/7244/1";
        //        url = url.Replace("valor", token);

        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //        httpWebRequest.ContentType = "application/json";
        //        httpWebRequest.Method = "POST";

        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            streamWriter.Write(json);
        //            streamWriter.Flush();
        //            streamWriter.Close();
        //        }

        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            var result = getResponse(streamReader.ReadToEnd());
        //            long idCliente = 0;
        //            bool isRegister = long.TryParse(result, out idCliente);
        //            if (isRegister)
        //            {
        //                pedido.IdCliente = idCliente;
        //            }
        //            else
        //            {
        //                respuesta.Ok = "NO";
        //                respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: " + result;
        //                dbCnx.RollbackTransaccion();
        //                dbCnx.CerrarConexion();
        //                return respuesta;
        //            }

        //        }
        //        #region PERSONA MORAL
        //        if (pedido.DatosFiscales != null)
        //        {
        //            if (!String.IsNullOrEmpty(pedido.DatosFiscales.RazonSocial))
        //            {

        //                DVARegistraPersona.RegistraPersona persona_m = new DVARegistraPersona.RegistraPersona();
        //                persona_m.RFC = pedido.DatosFiscales.Rfc;
        //                persona_m.RazonSocial = pedido.DatosFiscales.RazonSocial;
        //                persona_m.Email = pedido.Cuenta.Correo;
        //                string json_m = JsonConvert.SerializeObject(persona_m);
        //                //string valor_m = "27|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //                string valor_m = "36|7244|1|" + DateTime.Now;

        //                DVAAutosystServerClasses.Seguridad.Seguridad seg_m = new DVAAutosystServerClasses.Seguridad.Seguridad();
        //                string token_m = seg.EncriptarCadena(valor_m);
        //                url = "http://10.5.2.21:7070/wsRegistraPersona/api/Persona/registrarApps/valor/36/7244/1";
        //                url = url.Replace("valor", token_m);

        //                var httpWebRequest_m = (HttpWebRequest)WebRequest.Create(url);
        //                httpWebRequest_m.ContentType = "application/json";
        //                httpWebRequest_m.Method = "POST";

        //                using (var streamWriter = new StreamWriter(httpWebRequest_m.GetRequestStream()))
        //                {
        //                    streamWriter.Write(json_m);
        //                    streamWriter.Flush();
        //                    streamWriter.Close();
        //                }

        //                var httpResponse_m = (HttpWebResponse)httpWebRequest_m.GetResponse();
        //                using (var streamReader = new StreamReader(httpResponse_m.GetResponseStream()))
        //                {
        //                    var result = getResponse(streamReader.ReadToEnd());
        //                    long idCliente = 0;
        //                    bool isRegister = long.TryParse(result, out idCliente);
        //                    if (isRegister)
        //                    {
        //                        idClienteMoral = idCliente;

        //                        pedido.IdCliente = idClienteMoral;
        //                        pedido.IdContacto = pedido.IdCliente;
        //                    }
        //                    else
        //                    {
        //                        respuesta.Ok = "NO";
        //                        respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: " + result;
        //                        respuesta.Objeto = null;
        //                        dbCnx.RollbackTransaccion();
        //                        dbCnx.CerrarConexion();
        //                        return respuesta;
        //                    }

        //                }

        //            }
        //            else if (!String.IsNullOrEmpty(pedido.DatosFiscales.Nombre))
        //            {


        //                RestSharp.Deserializers.JsonDeserializer deserial_fact = new RestSharp.Deserializers.JsonDeserializer();

        //                DVARegistraPersona.RegistraPersona persona_fact = new DVARegistraPersona.RegistraPersona();
        //                persona_fact.Nombre = pedido.DatosFiscales.Nombre;
        //                persona_fact.ApellidoPaterno = pedido.DatosFiscales.APaterno;
        //                persona_fact.ApellidoMaterno = pedido.DatosFiscales.AMaterno;
        //                persona_fact.RFC = pedido.DatosFiscales.Rfc;
        //                persona_fact.Email = pedido.Cuenta.Correo;
        //                string json_pf = JsonConvert.SerializeObject(persona_fact);
        //                string valor_pf = "36|7244|1|" + DateTime.Now;

        //                DVAAutosystServerClasses.Seguridad.Seguridad seg_pf = new DVAAutosystServerClasses.Seguridad.Seguridad();
        //                string token_pf = seg_pf.EncriptarCadena(valor_pf);

        //                string url_pf = "http://10.5.2.21:7070/wsRegistraPersona/api/Persona/registrarApps/valor/36/7244/1";
        //                url_pf = url_pf.Replace("valor", token_pf);

        //                var httpWebRequest_pf = (HttpWebRequest)WebRequest.Create(url_pf);
        //                httpWebRequest_pf.ContentType = "application/json";
        //                httpWebRequest_pf.Method = "POST";

        //                using (var streamWriter = new StreamWriter(httpWebRequest_pf.GetRequestStream()))
        //                {
        //                    streamWriter.Write(json_pf);
        //                    streamWriter.Flush();
        //                    streamWriter.Close();
        //                }

        //                var httpResponse_pf = (HttpWebResponse)httpWebRequest_pf.GetResponse();
        //                using (var streamReader = new StreamReader(httpResponse_pf.GetResponseStream()))
        //                {
        //                    var result = getResponse(streamReader.ReadToEnd());

        //                    long idCliente_pf = 0;
        //                    bool isRegister = long.TryParse(result, out idCliente_pf);
        //                    if (isRegister)
        //                    {
        //                        pedido.IdContacto = pedido.IdCliente;
        //                        pedido.IdCliente = idCliente_pf;
        //                    }
        //                    else
        //                    {
        //                        respuesta.Ok = "NO";
        //                        respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: " + result;
        //                        respuesta.Objeto = null;
        //                        dbCnx.RollbackTransaccion();
        //                        dbCnx.CerrarConexion();
        //                        return respuesta;
        //                    }


        //                }


        //            }
        //        }
        //        #endregion
        //        #endregion
        //        #endregion

        //        #region REGISTRA CUENTA

        //        string strSql = "";
        //        strSql = "SELECT 1 FROM PRODAPPS.APCCTAST WHERE TRIM(FSAPCORREO) = '" + pedido.Cuenta.Correo.Trim() + "'";
        //        DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];

        //        if (dtValidaCuenta.Rows.Count > 0)
        //        {
        //            respuesta.Ok = "NO";
        //            respuesta.Mensaje = "Ya existe una cuenta ligada a su correo.";
        //            respuesta.Objeto = null;
        //            dbCnx.RollbackTransaccion();
        //            dbCnx.CerrarConexion();
        //            return respuesta;
        //        }

        //        strSql = "";
        //        strSql = "SELECT MAX(FIAPIDCUEN) + 1 Id ";
        //        strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST ";

        //        DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

        //        if (dt.Rows.Count == 1)
        //        {
        //            pedido.Cuenta.IdCuenta = dt.Rows[0]["Id"].ToString();


        //            #region INSERT CUENTA
        //            strSql = "";
        //            strSql = "INSERT INTO " + constantes.Ambiente + "APPS.APCCTAST ";
        //            strSql += "(FIAPIDCUEN,FIAPIDPERS , FSAPNOMBRE, FSAPAPEPAT, FSAPCORREO, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT) VALUES ";
        //            strSql += "(" + pedido.Cuenta.IdCuenta + "," + pedido.IdCliente + ",'" + pedido.Cuenta.Nombre.Trim().ToUpper() + "', '" + pedido.Cuenta.ApellidoPaterno.Trim().ToUpper() + "', '" + pedido.Cuenta.Correo.Trim() +
        //                "', 1, 1, 'APP', CURRENT DATE, CURRENT TIME, 'APP')";
        //            dbCnx.SetQuery(strSql);
        //            #endregion

        //        }
        //        else
        //        {
        //            respuesta.Ok = "NO";
        //            respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde";
        //            respuesta.Objeto = null;
        //            dbCnx.RollbackTransaccion();
        //            dbCnx.CerrarConexion();

        //            return respuesta;
        //        }
        //        #endregion

        //        //AGREGAR CONDICION PARA DATOS FISCALES PERSONA FISICA!!

        //        #region  REGISTRO PEDIDO
        //        if (idClienteMoral > 0)
        //        {
        //            SolicitudPedido solicitudPedido = new SolicitudPedido();
        //            solicitudPedido.IdCliente = pedido.IdCliente;
        //            solicitudPedido.Serie = pedido.DatosUnidad.SerieAuto;
        //            solicitudPedido.IdAgente = 999999;
        //            solicitudPedido.IdContacto = pedido.IdContacto;
        //            solicitudPedido.IdTipoDeVenta = 52;
        //            solicitudPedido.Total = pedido.Total;
        //            jsonPedido = JsonConvert.SerializeObject(solicitudPedido);
        //        }
        //        else if (!string.IsNullOrEmpty(pedido.DatosFiscales.Nombre))
        //        {
        //            SolicitudPedido solicitudPedido = new SolicitudPedido();
        //            solicitudPedido.IdCliente = pedido.IdCliente;
        //            solicitudPedido.Serie = pedido.DatosUnidad.SerieAuto;
        //            solicitudPedido.IdAgente = 999999;
        //            solicitudPedido.IdContacto = pedido.IdContacto;
        //            solicitudPedido.IdTipoDeVenta = 52;
        //            solicitudPedido.Total = pedido.Total;
        //            jsonPedido = JsonConvert.SerializeObject(solicitudPedido);
        //        }
        //        else
        //        {
        //            SolicitudPedido solicitudPedido = new SolicitudPedido();
        //            solicitudPedido.IdCliente = pedido.IdCliente;
        //            solicitudPedido.Serie = pedido.DatosUnidad.SerieAuto;
        //            solicitudPedido.IdAgente = 999999;
        //            solicitudPedido.IdContacto = 0;
        //            solicitudPedido.IdTipoDeVenta = 52;
        //            solicitudPedido.Total = pedido.Total;
        //            jsonPedido = JsonConvert.SerializeObject(solicitudPedido);
        //        }


        //        //string valor_ped = "27|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //        //string valor_ped = "39|7244|1|" + DateTime.Now;
        //        string valor_ped = "36|7244|1|" + DateTime.Now;

        //        DVAAutosystServerClasses.Seguridad.Seguridad seg_ped = new DVAAutosystServerClasses.Seguridad.Seguridad();
        //        string token_ped = seg_ped.EncriptarCadena(valor_ped);

        //        //string url_ped = "http://localhost:31076/api/Pedido/registrar/valor/27/7244/1";
        //        string url_ped = "http://10.5.2.21:7070/wsRegistraPersona/api/Pedido/registrar/valor/36/7244/1";
        //        url_ped = url_ped.Replace("valor", token_ped);



        //        var httpWebRequest_ped = (HttpWebRequest)WebRequest.Create(url_ped);
        //        httpWebRequest_ped.ContentType = "application/json";
        //        httpWebRequest_ped.Method = "POST";

        //        using (var streamWriter = new StreamWriter(httpWebRequest_ped.GetRequestStream()))
        //        {
        //            streamWriter.Write(jsonPedido);
        //            streamWriter.Flush();
        //            streamWriter.Close();
        //        }

        //        var httpResponse_ped = (HttpWebResponse)httpWebRequest_ped.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse_ped.GetResponseStream()))
        //        {
        //            var result = getResponse(streamReader.ReadToEnd());
        //            long idp;
        //            bool isRegisterPedido = long.TryParse(result, out idp);
        //            if (isRegisterPedido)
        //            {
        //                pedido.IdPedido = idp;
        //            }
        //            else
        //            {
        //                respuesta.Ok = "NO";
        //                respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: " + result;
        //                respuesta.Objeto = null;

        //                dbCnx.RollbackTransaccion();
        //                dbCnx.CerrarConexion();
        //            }
        //        }

        //        #endregion

        //        #region Construir Objeto completo
        //        respuesta.Ok = "SI";
        //        respuesta.Mensaje = "Pedido registrado, en un momento seras contactado";
        //        PedidoAll pedidoComplete = new PedidoAll();
        //        pedidoComplete.Cuenta = new Cuenta();
        //        pedidoComplete.Cuenta = pedido.Cuenta;
        //        pedidoComplete.DatosFiscales = new DatosFiscales();
        //        pedidoComplete.DatosFiscales = pedido.DatosFiscales;
        //        pedidoComplete.IdCliente = pedido.IdCliente;
        //        pedidoComplete.IdPedido = pedido.IdPedido;
        //        pedidoComplete.IdContacto = pedido.IdContacto;
        //        pedidoComplete.DatosUnidad = new DatosUnidad();
        //        pedidoComplete.DatosUnidad.SerieAuto = pedido.DatosUnidad.SerieAuto;
        //        pedidoComplete.Total = pedido.Total;
        //        pedidoComplete.Cuenta.IdPersona = (idClienteMoral > 0 ? pedido.IdContacto.ToString() : pedido.IdCliente.ToString());
        //        //respuesta.Objeto = JsonConvert.SerializeObject(pedidoComplete);
        //        #endregion


        //        #region REGISTRO COMPRA
        //        strSql = "";
        //        strSql = "SELECT MAX(FIAPIDCOMP) + 1 Id ";
        //        strSql += "FROM " + constantes.Ambiente + "APPS.APECMPST ";

        //        DataTable dtc = dbCnx.GetDataSet(strSql).Tables[0];
        //        if (dtc.Rows.Count == 1)
        //        {
        //            int idOut = 0;
        //            bool isCorrect = int.TryParse(dtc.Rows[0]["Id"].ToString(), out idOut);
        //            if (!isCorrect)
        //            {
        //                pedido.IdCompra = 1;
        //            }
        //            else
        //            {
        //                pedido.IdCompra = int.Parse(dtc.Rows[0]["Id"].ToString());
        //            }
        //            pedidoComplete.IdCompra = pedido.IdCompra;
        //            pedidoComplete.FolioCompra = pedido.IdCompra.ToString();



        //            strSql = "";
        //            strSql = @"INSERT INTO PRODAPPS.APECMPST
        //            (
        //            FIAPIDCOMP,      /*ID COMPRA   */
        //            FIAPFOLCOM,     /* FOLIO COMPRA*/
        //            FIAPIDCUEN,      /*ID CUENTA   */
        //            FFAPFECCOM,      /*FECHA COMPRA*/
        //            FHAPHORCOM,      /*HORA COMPRA */
        //            FDAPSUBTOT,      /*SUBTOTAL */
        //            FDAPDESCUE,      /*DESCUENTO*/
        //            FDAPIVA,         /*IVA      */
        //            FDAPTOTAL,      /* TOTAL*/    
        //            FIAPIDESTA,      /*ID ESTADO*/
        //            FIAPSTATUS,      /*ESTATUS     */     
        //            USERCREAT,       /*USUARIO CREACION */
        //            DATECREAT,       /*FECHA CREACION   */
        //            TIMECREAT,       /*HORA CREACION    */
        //            PROGCREAT       /*PROGRAMA CREACION*/) VALUES(" +
        //            pedido.IdCompra + " , " +
        //            pedido.IdCompra + " , " +
        //            pedido.Cuenta.IdCuenta + " , " +
        //            "CURRENT DATE , " +
        //            "CURRENT TIME , " +
        //            pedido.Subtotal + " , " +
        //            0 + " , " + //DESCUENTO
        //            pedido.Iva + " , " +
        //            pedido.Total + " , " +
        //            1 + " , " + //ID ESTADO
        //            1 + " , " +
        //            "'APP' , " +
        //            "CURRENT DATE , " +
        //            "CURRENT TIME , " +
        //            "'APP'" + ")";
        //            dbCnx.SetQuery(strSql);

        //        }

        //        #endregion

        //        #region REGISTRO PEDIDOP APP AUTO              
        //        int cotizarSeguro = (pedido.CotizarSeguro ? 1 : 0);
        //        strSql = "";
        //        strSql = @"INSERT INTO PRODAPPS.APEPANST
        //            (
        //            FIAPIDCOMP,     /* ID COMPRA    */
        //            FIAPIDCIAU,      /*ID CIA. UNICA*/
        //            FIAPIDPEDI,      /*ID PEDIDO    */
        //            FFAPFECPED,      /*FECHA PEDIDO */
        //            FHAPHORPED,      /*HORA PEDIDO  */
        //            FIAPIDPERS,      /*ID PERSONA   */
        //            FIAPIDVEHI,     /*ID VEHICULO  */
        //            FIAPIDINVE,      /*ID INVENTARIO*/
        //            FSAPNUMSER,      /*NUMERO SERIE*/ 
        //            FDAPSUBTOT,      /*SUBTOTAL     */
        //            FDAPDESCUE,      /*DESCUENTO      */ 
        //            FDAPIVA,         /*IVA             */
        //            FDAPTOTAL,       /*TOTAL      */     
        //            FIAPCOTSEG,    /*COTIZAR SEGURO*/
        //            FIAPSTATUS,      /*ESTATUS       */  
        //            USERCREAT,       /*USUARIO CREACION*/
        //            DATECREAT,       /*FECHA CREACION   */
        //            TIMECREAT,       /*HORA CREACION    */
        //            PROGCREAT       /*PROGRAMA CREACION*/
        //            ) VALUES(" +
        //        pedido.IdCompra + " , " +
        //        36 + " , " +
        //        pedido.IdPedido + " , " +
        //        "CURRENT DATE ," +
        //        "CURRENT TIME , " +
        //        pedido.Cuenta.IdPersona + " , " +
        //        pedido.DatosUnidad.IdVehiculo + " , " +
        //        pedido.DatosUnidad.IdInventario + " , " +
        //        "'" + pedido.DatosUnidad.SerieAuto + "'" + " , " +
        //        pedido.DatosUnidad.Subtotal + " , " +
        //        0 + " , " +
        //        pedido.DatosUnidad.Iva + " , " +
        //        pedido.DatosUnidad.Total + " , " +
        //        cotizarSeguro + " , " +
        //        1 + " , " +
        //        "'APP' , " +
        //        "CURRENT DATE , " +
        //        "CURRENT TIME , " +
        //        "'APP'" + ")";

        //        dbCnx.SetQuery(strSql);


        //        #endregion

        //        #region REGISTRO DETALLE PEDIDO ACCESORIOS

        //        pedidoComplete.AccesoriosOtros = new List<AccesoriosUOtros>();

        //        int counter = 0;
        //        foreach (var accesorios in pedido.AccesoriosOtros)
        //        {
        //            strSql = "";
        //            strSql = "SELECT MAX(FIAPIDCONS) + 1 Id ";
        //            strSql += "FROM " + constantes.Ambiente + "APPS.APDPANST WHERE FIAPIDCIAU = 36";

        //            DataTable dtdp = dbCnx.GetDataSet(strSql).Tables[0];

        //            if (dtdp.Rows.Count == 1)
        //            {

        //                int idOut = 0;
        //                bool isCorrect = int.TryParse(dtdp.Rows[0]["Id"].ToString(), out idOut);
        //                if (!isCorrect)
        //                {
        //                    pedido.IdCompra = 1;
        //                }
        //                else
        //                {
        //                    pedido.AccesoriosOtros[counter].IdDetallePedido = int.Parse(dtdp.Rows[0]["Id"].ToString());
        //                }
        //                strSql = "";

        //                //pedido.IdDetallePedido = int.Parse(dtdp.Rows[0]["Id"].ToString());
        //                strSql = @"INSERT INTO PRODAPPS.APDPANST
        //                (
        //                FIAPIDCIAU,      /*ID CIA. UNICA */
        //                FIAPIDPEDI,      /*ID PEDIDO     */
        //                FIAPIDCONS,      /*ID CONSECUTIVO*/
        //                FSAPCONCEP,      /*CONCEPTO      */
        //                FDAPSUBTOT,      /*SUBTOTAL      */
        //                FDAPDESCUE,      /*DESCUENTO       */
        //                FDAPIVA,         /*IVA             */
        //                FDAPTOTAL,       /*TOTAL      */     
        //                FIAPSTATUS,     /*ESTATUS       */  
        //                USERCREAT,       /*USUARIO CREACION*/
        //                DATECREAT,       /*FECHA CREACION   */
        //                TIMECREAT,       /*HORA CREACION    */
        //                PROGCREAT       /*PROGRAMA CREACION*/
        //                ) VALUES 
        //                (" +
        //                36 + " , " +
        //                pedido.IdPedido + " , " +
        //                pedido.AccesoriosOtros[counter].IdDetallePedido + " , " +
        //                "'" + pedido.AccesoriosOtros[counter].Concepto + "'" + " , " +
        //                pedido.AccesoriosOtros[counter].Subtotal + " , " +
        //                0 + " , " + // DESCUENTO
        //                pedido.AccesoriosOtros[counter].Iva + " , " +
        //                pedido.AccesoriosOtros[counter].Total + " , " +
        //                1 + " , " +
        //                "'APP' , " +
        //                "CURRENT DATE , " +
        //                "CURRENT TIME , " +
        //                "'APP'" + ")";

        //                pedidoComplete.AccesoriosOtros.Add(pedido.AccesoriosOtros[counter]);
        //                dbCnx.SetQuery(strSql);
        //                counter++;
        //            }

        //        }

        //        #endregion

        //        #region REGISTRO SEGUIMIENTO PEDIDO
        //        strSql = "";
        //        strSql = "SELECT MAX(FIAPIDSEGU) + 1 Id ";
        //        strSql += "FROM " + constantes.Ambiente + "APPS.APDSGCST ";

        //        DataTable dtdt = dbCnx.GetDataSet(strSql).Tables[0];
        //        if (dtdt.Rows.Count == 1)
        //        {


        //            int idOut = 0;
        //            bool isCorrect = int.TryParse(dtdt.Rows[0]["Id"].ToString(), out idOut);
        //            if (!isCorrect)
        //            {
        //                pedido.IdSeguimientoPedido = 1;
        //            }
        //            else
        //            {
        //                pedido.IdSeguimientoPedido = int.Parse(dtdt.Rows[0]["Id"].ToString());
        //            }
        //            strSql = "";

        //            strSql = @"INSERT INTO PRODAPPS.APDSGCST
        //            (
        //                FIAPIDCOMP,      /*ID COMPRA     */
        //                FIAPIDSEGU,      /*ID SEGUIMIENTO*/
        //                FSAPTITSEG,     /*TITULO SEGUIM */
        //                FIAPIDESTA,      /*ID ESTADO     */
        //                FIAPSTATUS,      /*ESTATUS       */
        //                USERCREAT,       /*USUARIO CREACION */
        //                DATECREAT,       /*FECHA CREACION   */
        //                TIMECREAT,       /*HORA CREACION    */
        //                PROGCREAT       /*PROGRAMA CREACION*/
        //                ) VALUES
        //                (" +
        //                pedido.IdCompra + " , " +
        //                pedido.IdSeguimientoPedido + " , " +
        //                "'seguimiento pedido #" + pedido.IdPedido + "' , " +
        //                1 + " , " +
        //                1 + " , " +
        //                "'APP' , " +
        //                "CURRENT DATE , " +
        //                "CURRENT TIME , " +
        //                "'APP'" + ")";

        //            dbCnx.SetQuery(strSql);
        //        }





        //        #endregion

        //        #region ACTUALIZACION ESTADO COMPRA
        //        strSql = "";
        //        strSql = @"UPDATE PRODAPPS.APECMPST
        //        SET
        //        FIAPIDESTA = 2 ,/*ID ESTADO*/
        //        USERCREAT = 'APP' ,/*USUARIO CREACION*/
        //        DATECREAT =CURRENT DATE ,/*FECHA CREACION*/
        //        TIMECREAT =CURRENT TIME ,/*HORA CREACION*/
        //        PROGCREAT = 'APP' /*PROGRAMA CREACION*/
        //        WHERE FIAPIDCOMP = /*ID COMPRA*/" + pedido.IdCompra;

        //        dbCnx.SetQuery(strSql);

        //        #endregion
        //        respuesta.Ok = "SI";
        //        respuesta.Mensaje = "Registro con éxito.";
        //        respuesta.Objeto = JsonConvert.SerializeObject(pedidoComplete);
        //        dbCnx.CommitTransaccion();
        //        dbCnx.CerrarConexion();


        //    }
        //    catch (Exception _exc)
        //    {
        //        dbCnx.RollbackTransaccion();
        //        dbCnx.CerrarConexion();

        //        respuesta.Ok = "NO";
        //        respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde :" + _exc.Message;
        //        respuesta.Objeto = null;

        //    }
        //    return respuesta;
        //}

        
     

    }
}