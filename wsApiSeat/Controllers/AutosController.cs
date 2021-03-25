using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using wsApiSeat.Bussiness;
using wsApiSeat.Models;
using wsApiSeat.Services;

namespace wsApiSeat.Controllers
{
    public class AutosController : ApiController
    {
        string UserName = "zlvO7khPpVo=";
        string Password = "HS47sBOr7JQ=";
        byte SistemaId = 1;

        /*Servicio de seguridad SmartIT*/
        string UsrSmartIT = "ARamirez07";
        string PssSmartIT = "ARamirez77";
        //string servidorSeguridadSmartIT = "http://10.5.2.120/";   //Pruebas
        string servidorSeguridadSmartIT = "http://10.5.2.122/";     //Producción

        // POST api/Autos/RegistraAutoSeminuevoInteres
        [Route("api/Autos/RegistraAutoSeminuevoInteres", Name = "RegistraAutoSeminuevoInteres")]
        public Respuesta RegistraAutoSeminuevoInteres(long IdCuenta, AutoUsado autoJson)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            Respuesta respuesta = new Respuesta
            {
                Ok = "NO",
                Mensaje = "No es posible registrar su cuenta, intente más tarde.",
                Objeto = ""
            };

            string correo = "";

            List<Cuenta> cuentas = new List<Cuenta>();
            AutoUsado auto = new AutoUsado();
            try
            {
                auto = autoJson;
                auto.Descripcion = "";

                if (IdCuenta != 0)
                {
                    string strSql = "";
                    strSql = "SELECT FIAPIDCUEN IdCuenta, FIAPIDPERS IdPersona, TRIM(FSAPNOMBRE) Nombre, TRIM(FSAPAPEPAT) ApellidoPaterno, TRIM(FSAPAPEMAT) ApellidoMaterno, " +
                        "TRIM(FSAPCORREO) Correo  ";
                    strSql += "FROM PRODAPPS.APCCTAST WHERE FIAPSTATUS = 1 AND FIAPIDCUEN = " + IdCuenta.ToString();
                    DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                    string json = JsonConvert.SerializeObject(dt);
                    cuentas = JsonConvert.DeserializeObject<List<Cuenta>>(json);

                    if (cuentas.Count == 1)
                    {
                        correo = cuentas[0].Correo.Trim();
                    }

                    strSql = "";
                    strSql = "INSERT INTO " + constantes.Ambiente + "APPS.APDCSEMB ";
                    strSql += "(FIAPIDCONS, FIAPIDCUEN, FSAPAUTJSN, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT) VALUES ";
                    strSql += "((SELECT COALESCE(MAX(FIAPIDCONS), 0) + 1 " + "FROM " + constantes.Ambiente + "APPS.APDCSEMB)" + ", " + IdCuenta.ToString() + ", '" + JsonConvert.SerializeObject(auto) + "', 1, 1, 'APP', CURRENT DATE, CURRENT TIME, 'APP')";

                    try
                    {
                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        dbCnx.SetQuery(strSql);

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "En breve será contactado por un asesor.";
                    }
                    catch (Exception ex)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();

                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No se registró correctamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se registró correctamente.";
            }

            if (respuesta.Ok.Equals("SI"))
            {
                if (!string.IsNullOrEmpty(correo))
                {
                    string subject = "Nissan Imperio - Me interesa un auto";
                    HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, correo, CuentaService.ObtenerStrHtmlAutoMeInteresa(auto));
                    HiloEnvioCorreo hiloEnvioCorreoCasaTrust = new HiloEnvioCorreo(subject, "", CuentaService.ObtenerStrHtmlAutoMeInteresaMiAuto(cuentas[0], auto));
                    Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreo.EnvioCorreo));
                    Thread hiloCasaTrust = new Thread(new ThreadStart(hiloEnvioCorreoCasaTrust.EnvioCorreoCasaTrust));
                    hilo.Start();
                    hiloCasaTrust.Start();
                    //EnviaCorreoAutoMeInteresa(correo, autoJson);
                }
            }

            return respuesta;
        }

        // POST api/Autos/GetObtenerVehiculos
        [Route("api/Autos/GetObtenerVehiculos", Name = "GetObtenerVehiculos")]
        public List<Vehiculo> GetObtenerVehiculos(long IdCuenta)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {


                #region strql
                string strSql = "";
                strSql = "SELECT DISTINCT VEHIC.FNVEHICID,VEHIC.FSVEHISER,VEHIC.FSVEHIMOT,VEHIC.FSVEHIPLA,VEHIC.FIANODOMET,VERSI.FSVERSDES,VERSI.FNVERSANO,MODEL.FSMODECLA, MARCA.FSMARCDES, COLOR.FSCOLODES,CUENTA.FIAPIDCUEN, POLIZA.FSAPNUMPOL, SEGURO.FIAPIDASEG, SEGURO.FSAPASEGUR, POLIZA.FSAPTELASE, VEHIC.FNVEHICOE, VEHIC.FSVEHIPER, MODEL.FNMODEIDM, MARCA.FNMARCCLA, POLIZA.FFAPVENCIM, POLIZA.FIAPIDENGO, POLIZA.FFAPGTIAEX,  " +
                    "(SELECT  AGEN.FSGENOMLOG FROM PRODAUT.ANCMARCA M INNER JOIN PRODGRAL.GECCIAUN AGEN ON M.FNMARCASO = AGEN.FIGEIDMARC WHERE MARCA.FNMARCCLA = M.FNMARCCLA FETCH FIRST 1 ROWS ONLY) LOGO ";

                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CUENTA ";
                //strSql += "ON	VEHIC.FNVEHICLI = CUENTA.FIAPIDPERS ";
                strSql += "INNER JOIN	" + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
                strSql += "ON	VEHIC.FNVEHICLI = CUENTA.FIAPIDPERS ";
                strSql += "AND	VEHIC.FIANSTATU = 1 ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
                strSql += "ON	VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
                strSql += "ON	VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCFAMIL FAMIL ";
                strSql += "ON	MODEL.FNMODEFAM = FAMIL.FNFAMICLA ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMARCA MARCA ";
                strSql += "ON	FAMIL.FNFAMISUB = MARCA.FNMARCCLA ";
                strSql += "AND MARCA.FNMARCCLA <> 71 ";
                //strSql += "LEFT JOIN " + constantes.Ambiente + "GRAL.GECCIAUN AGEN ";
                //strSql += "ON	MARCA.FNMARCCLA = AGEN.FIGEIDMARC ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCCOLOR COLOR ";
                strSql += "ON	VEHIC.FNVEHICOE = COLOR.FNCOLOCLA ";
                strSql += "LEFT JOIN " + constantes.Ambiente + "APPS.APDVEHMB POLIZA ";
                strSql += "ON	VEHIC.FNVEHICID = POLIZA.FIAPIDVEHI ";
                strSql += "AND	CUENTA.FIAPIDCUEN = POLIZA.FIAPIDCUEN ";
                strSql += "LEFT JOIN " + constantes.Ambiente + "APPS.APCASEMB SEGURO ";
                strSql += "ON	POLIZA.FIAPIDASEG = SEGURO.FIAPIDASEG ";
                strSql += "WHERE	CUENTA.FIAPIDCUEN = " + IdCuenta.ToString();
                #endregion

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                Vehiculo vehiculo;
                List<Vehiculo> coleccionVehiculos = new List<Vehiculo>();
                foreach (DataRow dr in dt.Rows)
                {
                    vehiculo = new Vehiculo();
                    vehiculo.Id = dr["FNVEHICID"].ToString().Trim();
                    vehiculo.NumeroSerie = dr["FSVEHISER"].ToString().Trim();
                    vehiculo.NumeroMotor = dr["FSVEHIMOT"].ToString().Trim();
                    vehiculo.Placas = dr["FSVEHIPLA"].ToString().Trim();
                    vehiculo.Odometro = dr["FIANODOMET"].ToString().Trim();
                    vehiculo.Version = dr["FSVERSDES"].ToString().Trim();
                    vehiculo.Anio = dr["FNVERSANO"].ToString().Trim();
                    vehiculo.Modelo = dr["FSMODECLA"].ToString().Trim();
                    vehiculo.Marca = dr["FSMARCDES"].ToString().Trim();
                    vehiculo.Color = dr["FSCOLODES"].ToString().Trim();
                    vehiculo.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    vehiculo.NumeroPoliza = dr["FSAPNUMPOL"].ToString().Trim();
                    vehiculo.IdAseguradora = dr["FIAPIDASEG"].ToString().Trim();
                    vehiculo.Aseguradora = dr["FSAPASEGUR"].ToString().Trim();
                    vehiculo.TelAseguradora = dr["FSAPTELASE"].ToString().Trim();
                    vehiculo.IdColor = dr["FNVEHICOE"].ToString().Trim();
                    vehiculo.Permiso = dr["FSVEHIPER"].ToString().Trim();
                    vehiculo.IdModelo = dr["FNMODEIDM"].ToString().Trim();
                    vehiculo.IdMarca = dr["FNMARCCLA"].ToString().Trim();

                    string FechaVencimiento = dr["FFAPVENCIM"].ToString().Trim();

                    if (FechaVencimiento != "")
                    {
                        FechaVencimiento = DateTime.Parse(dr["FFAPVENCIM"].ToString().Trim()).ToString("dd-MM-yyyy");
                    }

                    vehiculo.FechaVencimiento = FechaVencimiento;
                    vehiculo.IdEngomado = dr["FIAPIDENGO"].ToString().Trim();

                    string FechaGarantiaExt = dr["FFAPGTIAEX"].ToString().Trim();

                    if (FechaGarantiaExt != "")
                    {
                        FechaGarantiaExt = DateTime.Parse(dr["FFAPGTIAEX"].ToString().Trim()).ToString("dd-MM-yyyy");
                    }

                    vehiculo.FechaGarantiaExt = FechaGarantiaExt;
                    vehiculo.NombreLogo = dr["LOGO"].ToString().Trim();

                    coleccionVehiculos.Add(vehiculo);
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();

                return coleccionVehiculos;

                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionVehiculos));
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<Vehiculo> coleccionVehiculos = new List<Vehiculo>();
                return coleccionVehiculos;
            }

        }

        // POST api/Autos/ObtenerDatosVehiculos
        [Route("api/Autos/GetObtenerDatosVehiculos", Name = "GetObtenerDatosVehiculos")]
        public List<Vehiculo> GetObtenerDatosVehiculos(long IdPersona)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT DISTINCT VEHIC.FNVEHICID,VEHIC.FSVEHISER,VEHIC.FSVEHIMOT,VEHIC.FSVEHIPLA,VEHIC.FIANODOMET,VERSI.FSVERSDES,VERSI.FNVERSANO,MODEL.FSMODECLA, MARCA.FSMARCDES, COLOR.FSCOLODES,CUENTA.FIAPIDCUEN, POLIZA.FSAPNUMPOL, SEGURO.FIAPIDASEG, SEGURO.FSAPASEGUR, POLIZA.FSAPTELASE, VEHIC.FNVEHICOE, VEHIC.FSVEHIPER, MODEL.FNMODEIDM, MARCA.FNMARCCLA, POLIZA.FFAPVENCIM, POLIZA.FIAPIDENGO, POLIZA.FFAPGTIAEX,  " +
                    "(SELECT  AGEN.FSGENOMLOG FROM PRODAUT.ANCMARCA M INNER JOIN PRODGRAL.GECCIAUN AGEN ON M.FNMARCASO = AGEN.FIGEIDMARC WHERE MARCA.FNMARCCLA = M.FNMARCCLA FETCH FIRST 1 ROWS ONLY) LOGO ";

                strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST CUENTA ";
                //strSql += "ON	VEHIC.FNVEHICLI = CUENTA.FIAPIDPERS ";
                strSql += "INNER JOIN	" + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
                strSql += "ON	VEHIC.FNVEHICLI = CUENTA.FIAPIDPERS ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
                strSql += "ON	VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMODEL MODEL ";
                strSql += "ON	VERSI.FNVERSIDM = MODEL.FNMODEIDM ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCFAMIL FAMIL ";
                strSql += "ON	MODEL.FNMODEFAM = FAMIL.FNFAMICLA ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCMARCA MARCA ";
                strSql += "ON	FAMIL.FNFAMISUB = MARCA.FNMARCCLA ";
                strSql += "AND MARCA.FNMARCCLA <> 71 ";
                //strSql += "LEFT JOIN " + constantes.Ambiente + "GRAL.GECCIAUN AGEN ";
                //strSql += "ON	MARCA.FNMARCCLA = AGEN.FIGEIDMARC ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCCOLOR COLOR ";
                strSql += "ON	VEHIC.FNVEHICOE = COLOR.FNCOLOCLA ";
                strSql += "LEFT JOIN " + constantes.Ambiente + "APPS.APDVEHMB POLIZA ";
                strSql += "ON	VEHIC.FNVEHICID = POLIZA.FIAPIDVEHI ";
                strSql += "AND	CUENTA.FIAPIDCUEN = POLIZA.FIAPIDCUEN ";
                strSql += "LEFT JOIN " + constantes.Ambiente + "APPS.APCASEMB SEGURO ";
                strSql += "ON	POLIZA.FIAPIDASEG = SEGURO.FIAPIDASEG ";
                strSql += "WHERE	CUENTA.FIAPIDPERS = " + IdPersona.ToString();


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                Vehiculo vehiculo;
                List<Vehiculo> coleccionVehiculos = new List<Vehiculo>();
                foreach (DataRow dr in dt.Rows)
                {
                    vehiculo = new Vehiculo();
                    vehiculo.Id = dr["FNVEHICID"].ToString().Trim();
                    vehiculo.NumeroSerie = dr["FSVEHISER"].ToString().Trim();
                    vehiculo.NumeroMotor = dr["FSVEHIMOT"].ToString().Trim();
                    vehiculo.Placas = dr["FSVEHIPLA"].ToString().Trim();
                    vehiculo.Odometro = dr["FIANODOMET"].ToString().Trim();
                    vehiculo.Version = dr["FSVERSDES"].ToString().Trim();
                    vehiculo.Anio = dr["FNVERSANO"].ToString().Trim();
                    vehiculo.Modelo = dr["FSMODECLA"].ToString().Trim();
                    vehiculo.Marca = dr["FSMARCDES"].ToString().Trim();
                    vehiculo.Color = dr["FSCOLODES"].ToString().Trim();
                    vehiculo.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    vehiculo.NumeroPoliza = dr["FSAPNUMPOL"].ToString().Trim();
                    vehiculo.IdAseguradora = dr["FIAPIDASEG"].ToString().Trim();
                    vehiculo.Aseguradora = dr["FSAPASEGUR"].ToString().Trim();
                    vehiculo.TelAseguradora = dr["FSAPTELASE"].ToString().Trim();
                    vehiculo.IdColor = dr["FNVEHICOE"].ToString().Trim();
                    vehiculo.Permiso = dr["FSVEHIPER"].ToString().Trim();
                    vehiculo.IdModelo = dr["FNMODEIDM"].ToString().Trim();
                    vehiculo.IdMarca = dr["FNMARCCLA"].ToString().Trim();

                    string FechaVencimiento = dr["FFAPVENCIM"].ToString().Trim();

                    if (FechaVencimiento != "")
                    {
                        FechaVencimiento = DateTime.Parse(dr["FFAPVENCIM"].ToString().Trim()).ToString("dd-MM-yyyy");
                    }

                    vehiculo.FechaVencimiento = FechaVencimiento;
                    vehiculo.IdEngomado = dr["FIAPIDENGO"].ToString().Trim();

                    string FechaGarantiaExt = dr["FFAPGTIAEX"].ToString().Trim();

                    if (FechaGarantiaExt != "")
                    {
                        FechaGarantiaExt = DateTime.Parse(dr["FFAPGTIAEX"].ToString().Trim()).ToString("dd-MM-yyyy");
                    }

                    vehiculo.FechaGarantiaExt = FechaGarantiaExt;
                    vehiculo.NombreLogo = dr["LOGO"].ToString().Trim();


                    coleccionVehiculos.Add(vehiculo);
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();
                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionVehiculos));
                return coleccionVehiculos;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<Vehiculo> coleccionVehiculos = new List<Vehiculo>();
                return coleccionVehiculos;
            }
        }

        [Route("api/Autos/GetObtenerMarcasConAgencia", Name = "GetObtenerMarcasConAgencia")]
        public List<Marca> GetObtenerMarcasConAgencia()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT    FNMARCCLA, TRIM(FSMARCDES) FSMARCDES ";
                strSql += "FROM	" + constantes.Ambiente + "AUT.ANCMARCA INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN ON FNMARCCLA = FIGEIDMARC AND FIGESTATUS = 1 ";
                strSql += "WHERE FIANSTATU = 1 GROUP BY FNMARCCLA, TRIM(FSMARCDES)";


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                DataView dtX = dt.DefaultView;
                dtX.Sort = "FSMARCDES";
                dt = dtX.ToTable();

                Marca marca;
                List<Marca> coleccionMarcas = new List<Marca>();
                foreach (DataRow dr in dt.Rows)
                {
                    marca = new Marca();
                    marca.IdMarca = dr["FNMARCCLA"].ToString().Trim();
                    marca.DescMarca = dr["FSMARCDES"].ToString().Trim();

                    coleccionMarcas.Add(marca);
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();
                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionMarcas));

                return coleccionMarcas;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<Marca> coleccionMarcas = new List<Marca>();
                return coleccionMarcas;
            }
        }

        // POST api/Autos/GetObtenerMarcasAuto
        [Route("api/Autos/GetObtenerMarcasAuto", Name = "GetObtenerMarcasAuto")]
        public List<Marca> GetObtenerMarcasAuto()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "AUT.ANCMARCA ";
                strSql += "WHERE FIANSTATU = 1 ";


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                DataView dtX = dt.DefaultView;
                dtX.Sort = "FSMARCDES";
                dt = dtX.ToTable();

                Marca marca;
                List<Marca> coleccionMarcas = new List<Marca>();
                foreach (DataRow dr in dt.Rows)
                {
                    marca = new Marca();
                    marca.IdMarca = dr["FNMARCCLA"].ToString().Trim();
                    marca.DescMarca = dr["FSMARCDES"].ToString().Trim();

                    coleccionMarcas.Add(marca);
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();
                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionMarcas));
                return coleccionMarcas;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<Marca> coleccionMarcas = new List<Marca>();
                return coleccionMarcas;
            }
        }

        // POST api/Autos/ObtenerListadoAseguradoras
        [Route("api/Autos/GetObtenerListadoAseguradoras", Name = "GetObtenerListadoAseguradoras")]
        public List<Aseguradora> GetObtenerListadoAseguradoras()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCASEMB ASEG ";
                strSql += "WHERE ASEG.FIAPSTATUS = 1";


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                Aseguradora aseguradora;
                List<Aseguradora> coleccionAseguradoras = new List<Aseguradora>();
                foreach (DataRow dr in dt.Rows)
                {
                    aseguradora = new Aseguradora();
                    aseguradora.IdAseguradora = dr["FIAPIDASEG"].ToString().Trim();
                    aseguradora.NombreAseguradora = dr["FSAPASEGUR"].ToString().Trim();
                    coleccionAseguradoras.Add(aseguradora);
                }

                JavaScriptSerializer jss = new JavaScriptSerializer();
                //Context.Response.ContentType = "application/json";
                //Context.Response.Output.Write(jss.Serialize(coleccionAseguradoras));
                return coleccionAseguradoras;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<Aseguradora> coleccionAseguradoras = new List<Aseguradora>();
                return coleccionAseguradoras;
            }
        }

        // POST api/Autos/ObtenerFotosAutoSeminuevo
        [Route("api/Autos/GetObtenerFotosAutoSeminuevo", Name = "GetObtenerFotosAutoSeminuevo")]
        public List<Foto> GetObtenerFotosAutoSeminuevo(long IdVehiculo)
        {
            try
            {
                string servicioSeminuevos = "http://ws-smartit.divisionautomotriz.com/wsApiCasaTrust/api/autos/";
                var cliente = new RestClient(servicioSeminuevos + "ObtenerFotosPorId" + "?IdVehiculo=" + IdVehiculo.ToString());
                cliente.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = cliente.Execute(request);
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                List<Foto> fotos = deserial.Deserialize<List<Foto>>(response);
                //var autos = cliente.Execute<List<AutoUsado>>(request);
                return fotos;
            }
            catch (Exception ex)
            {

                List<Foto> fotos = new List<Foto>();

                return fotos;

            }


        }

        // POST api/Autos/PostActualizarDatosSeguroVehiculo
        [Route("api/Autos/PostActualizarDatosSeguroVehiculo", Name = "PostActualizarDatosSeguroVehiculo")]
        public Respuesta PostActualizarDatosSeguroVehiculo([FromBody] ActualizarDatosSeguroVehiculo DatosSeguro)
        {
            Respuesta respuesta = new Respuesta();

            DVADB.DB2 dbcnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {

                dbcnx.AbrirConexion();
                dbcnx.BeginTransaccion();


                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APDVEHMB ";
                strSql += " WHERE FIAPIDCUEN = " + DatosSeguro.IdCuenta;
                strSql += " AND FIAPIDVEHI = " + DatosSeguro.IdVehiculo;

                DataTable dt = dbcnx.GetDataSet(strSql).Tables[0];
                DataView dtX = dt.DefaultView;
                dt = dtX.ToTable();


                if (dt.Rows.Count != 0)
                {

                    strSql = "";

                    if (DatosSeguro.NumeroPoliza != "")
                    {
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APDVEHMB ";
                        strSql += " SET FSAPNUMPOL = '" + DatosSeguro.NumeroPoliza + "' ";
                        strSql += " WHERE FIAPIDCUEN = " + DatosSeguro.IdCuenta;
                        strSql += " AND FIAPIDVEHI = " + DatosSeguro.IdVehiculo;
                        dbcnx.SetQuery(strSql);
                    }


                    if (DatosSeguro.IdAseguradora != "")
                    {

                        strSql = "";
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APDVEHMB ";
                        strSql += " SET FIAPIDASEG = " + DatosSeguro.IdAseguradora;
                        strSql += " WHERE FIAPIDCUEN = " + DatosSeguro.IdCuenta;
                        strSql += " AND FIAPIDVEHI = " + DatosSeguro.IdVehiculo;
                        dbcnx.SetQuery(strSql);

                    }


                    if (DatosSeguro.TelAseguradora != "")
                    {
                        strSql = "";
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APDVEHMB ";
                        strSql += " SET FSAPTELASE = '" + DatosSeguro.TelAseguradora + "' ";
                        strSql += " WHERE FIAPIDCUEN = " + DatosSeguro.IdCuenta;
                        strSql += " AND FIAPIDVEHI = " + DatosSeguro.IdVehiculo;
                        dbcnx.SetQuery(strSql);
                    }


                    if (DatosSeguro.FechaVencimiento != "")
                    {
                        strSql = "";
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APDVEHMB ";
                        strSql += " SET FFAPVENCIM = '" + DatosSeguro.FechaVencimiento + "' ";
                        strSql += " WHERE FIAPIDCUEN = " + DatosSeguro.IdCuenta;
                        strSql += " AND FIAPIDVEHI = " + DatosSeguro.IdVehiculo;
                        dbcnx.SetQuery(strSql);
                    }
                }
                else
                {


                    if (DatosSeguro.IdAseguradora == "")
                    {
                        DatosSeguro.IdAseguradora = "default";
                    }

                    if (DatosSeguro.TelAseguradora == "")
                    {
                        DatosSeguro.TelAseguradora = "default";
                    }

                    strSql = "";
                    strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDVEHMB ";
                    strSql += "(FIAPIDCUEN, FIAPIDVEHI, FSAPNUMPOL, FIAPIDASEG, FSAPTELASE, FFAPVENCIM, FIAPIDENGO, FFAPGTIAEX, FIAPSTATUS, USERCREAT,PROGCREAT) ";
                    strSql += "VALUES ";
                    strSql += "(";
                    strSql += DatosSeguro.IdCuenta + ",";
                    strSql += DatosSeguro.IdVehiculo + ",";
                    strSql += "'" + DatosSeguro.NumeroPoliza + "',";
                    strSql += DatosSeguro.IdAseguradora + ",";
                    strSql += DatosSeguro.TelAseguradora + ",";
                    if (DatosSeguro.FechaVencimiento != "")
                    {
                        strSql += "'" + DatosSeguro.FechaVencimiento + "',";
                    }
                    else
                    {
                        strSql += "default,";
                    }
                    strSql += "default,";
                    strSql += "default,";
                    strSql += "1,'CENEASM','CENEASM'";
                    strSql += ")";
                    dbcnx.SetQuery(strSql);

                }

                dbcnx.CommitTransaccion();
                dbcnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos actualizados correctamente.";
                respuesta.Objeto = JsonConvert.SerializeObject(DatosSeguro);


                return respuesta;
            }

            catch (Exception)
            {
                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar los datos.";
                respuesta.Objeto = "";

                return respuesta;

            }
        }

        // POST api/Autos/PostActualizarDatosVehiculo
        [Route("api/Autos/PostActualizarDatosVehiculo", Name = "PostActualizarDatosVehiculo")]
        //public List<RespuestaServidor> PostActualizarDatosVehiculo(string IdCuenta, string IdVehiculo, string Kilometraje, string Placas)
        public Respuesta PostActualizarDatosVehiculo([FromBody] ActualizarDatosVehiculo DatosVehiculo)
        {
            Respuesta respuesta = new Respuesta();

            DVADB.DB2 dbcnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            try
            {

                string strSql = "";
                strSql = "SELECT * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST CUENTA ";
                strSql += "WHERE CUENTA.FIAPSTATUS = 1 ";
                strSql += "AND CUENTA.FIAPIDCUEN = " + DatosVehiculo.IdCuenta;


                DataTable dt = dbcnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    dbcnx.AbrirConexion();
                    dbcnx.BeginTransaccion();

                    strSql = "";
                    strSql += "UPDATE " + constantes.Ambiente + "AUT.ANCVEHIC ";
                    strSql += " SET FSVEHIPLA = '" + DatosVehiculo.Placas + "'";
                    strSql += " WHERE FNVEHICID = " + DatosVehiculo.IdVehiculo;
                    dbcnx.SetQuery(strSql);

                    strSql = "";
                    strSql += "UPDATE " + constantes.Ambiente + "AUT.ANCVEHIC ";
                    strSql += " SET FIANODOMET = " + DatosVehiculo.Kilometraje;
                    strSql += " WHERE FNVEHICID = " + DatosVehiculo.IdVehiculo;
                    dbcnx.SetQuery(strSql);

                    dbcnx.CommitTransaccion();
                    dbcnx.CerrarConexion();

                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Datos actualizados correctamente.";
                    respuesta.Objeto = JsonConvert.SerializeObject(DatosVehiculo);

                    return respuesta;

                }
                else
                {

                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se encontró información para la cuenta indicada";
                    respuesta.Objeto = JsonConvert.SerializeObject(DatosVehiculo);

                    return respuesta;
                }

            }
            catch (Exception)
            {

                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "Ocurrió un error al tratar de actualizar los datos.";
                respuesta.Objeto = JsonConvert.SerializeObject(DatosVehiculo);

                return respuesta;

            }

        }

        // POST api/Autos/PostActualizarEstatusSeguimiento
        [Route("api/Autos/PostActualizarEstatusSeguimiento", Name = "PostActualizarEstatusSeguimiento")]
        public RespuestaTest<EstatusSeguimiento> PostActualizarEstatusSeguimiento([FromBody] EstatusSeguimiento Seguimiento)
        {
            var respuesta = new RespuestaTest<EstatusSeguimiento>();

            string Usr = UsrSmartIT;
            string Pwd = PssSmartIT;
            //string Pwd = "Sony";
            string method = "POST";
            string body = string.Empty;
            string address = "";
            string parameters = "";
            string cookieHeader = "";

            if (Seguimiento.IdsDetallesCotizacion.Count() > 0)
            {

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

                respuesta.Mensaje = output;

                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    address = "serv/cotizaciontotalpreorden/AutorizarPorIds/";
                    parameters = Seguimiento.IdAgencia + "/" + Seguimiento.IdPreOrden + "/" + Seguimiento.IdCotizacion + "/1/1" + "/" + Seguimiento.IdsDetallesCotizacion[0] + "/1";
                    method = "POST";

                    request = (HttpWebRequest)WebRequest.Create(servidorSeguridadSmartIT + address + parameters);
                    request.Method = method;
                    //request.Credentials = new NetworkCredential("demo@leankit.com", "demopassword");
                    CookieContainer cookiecontainer = new CookieContainer();
                    string[] cookies = cookieHeader.Split(';');

                    cookiecontainer.SetCookies(new Uri(servidorSeguridadSmartIT), cookies[0]);

                    request.CookieContainer = cookiecontainer;
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
                    respuesta.Mensaje = output;

                    //respuesta = respuesta.Replace('\"', '"');
                    respuesta.Mensaje = respuesta.Mensaje.Replace("\\\"", "");
                    //respuesta = "[" + respuesta + "]";

                    try
                    {
                        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(respuesta.Mensaje);
                    }
                    catch (Exception e)
                    {
                        respuesta.Mensaje = e.Message;
                    }
                }
            }

            DVADB.DB2 dbcnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();


            try
            {

                dbcnx.AbrirConexion();
                dbcnx.BeginTransaccion();

                string strSql = "";
                strSql += "UPDATE " + constantes.Ambiente + "APPS.APDSEGMB ";
                strSql += " SET FIAPAUTORI = " + Seguimiento.Autorizado;
                strSql += " WHERE FIAPIDCUEN = " + Seguimiento.IdCuenta;
                strSql += " AND FIAPIDCIAU = " + Seguimiento.IdAgencia;
                strSql += " AND FIAPIDPREO = " + Seguimiento.IdPreOrden;
                strSql += " AND FIAPIDSEGO = " + Seguimiento.IdSeguimientoOrden;
                dbcnx.SetQuery(strSql);

                strSql = "";
                strSql += "UPDATE " + constantes.Ambiente + "APPS.APDSEGMB ";
                strSql += " SET FIAPVISTO = 1";
                strSql += " WHERE FIAPIDCUEN = " + Seguimiento.IdCuenta;
                strSql += " AND FIAPIDCIAU = " + Seguimiento.IdAgencia;
                strSql += " AND FIAPIDPREO = " + Seguimiento.IdPreOrden;
                strSql += " AND FIAPIDSEGO = " + Seguimiento.IdSeguimientoOrden;
                dbcnx.SetQuery(strSql);

                if (Seguimiento.IdsDetallesCotizacion.Count > 0)
                {

                    //string idsDetallesCotizacion = Seguimiento.IdsDetallesCotizacion[0].ToString();
                    //idsDetallesCotizacion = idsDetallesCotizacion.Replace("[", "(");
                    //idsDetallesCotizacion = idsDetallesCotizacion.Replace("]", ")");


                    string idsDetallesCotizacion = "";

                    foreach (DetalleAutoCotizacion valor in Seguimiento.IdsDetallesCotizacion)
                    {

                        idsDetallesCotizacion += valor.IdDetalleCotizacion.ToString() + ",";

                    }

                    idsDetallesCotizacion = idsDetallesCotizacion.Substring(0, idsDetallesCotizacion.Length - 1);



                    strSql = "";
                    strSql += "UPDATE " + constantes.Ambiente + "APPS.APDSCOMB ";
                    strSql += " SET FIAPAUTORI = 1";
                    strSql += " WHERE FIAPIDSEGO = " + Seguimiento.IdSeguimientoOrden;
                    strSql += " AND FIAPIDDETA IN ( " + idsDetallesCotizacion + " )";
                    dbcnx.SetQuery(strSql);
                }


                dbcnx.CommitTransaccion();
                dbcnx.CerrarConexion();

                //Enviar Alerta Via Email

                strSql = "";
                strSql += "SELECT * FROM " + constantes.Ambiente + "SERV.SEEPREOR PREOR ";
                strSql += " INNER JOIN " + constantes.Ambiente + "SERV.SECASESO ASESO ";
                strSql += " ON PREOR.FISEIDCIAU = ASESO.FISEIDCIAU ";
                strSql += " AND PREOR.FISEIDASES = ASESO.FISEIDASES ";
                strSql += " INNER JOIN " + constantes.Ambiente + "SEGU.SGCUSUAR USUAR ";
                strSql += " ON ASESO.FISEIDUSUA = USUAR.FISGIDUSUA ";
                strSql += " INNER JOIN " + constantes.Ambiente + "SERV.SEEORDEN ORDEN ";
                strSql += " ON PREOR.FISEIDCIAU = ORDEN.FISEIDCIAU ";
                strSql += " AND PREOR.FISEIDPREO = ORDEN.FISEIDPREO ";
                strSql += " WHERE PREOR.FISEIDCIAU = " + Seguimiento.IdAgencia;
                strSql += " AND PREOR.FISEIDPREO = " + Seguimiento.IdPreOrden;

                DataTable dt = dbcnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    String Email = dt.Rows[0]["FCSGEMPRIM"].ToString().Trim();
                    String Folio = dt.Rows[0]["FISEFOLIO"].ToString().Trim();

                    DateTime Hoy = DateTime.Today;
                    DateTime Ahora = DateTime.Now;

                    string fechaActual = Hoy.ToString("dd-MM-yyyy");

                    string horaActual = Ahora.ToString("HH:mm:ss"); ;

                    String Mensaje = "Folio de orden: " + Folio + ".\n\n";
                    Mensaje += "Se autorizó cotización.\n\n";
                    Mensaje += "Fecha: " + fechaActual + ".\n\nHora: " + horaActual + ".";

                    string mailFrom = "notificaciones@grupoautofin.com";
                    string password = "RXPJPJJ2013llx";
                    string smtpServidor = "smtp.office365.com";
                    string mensaje = Mensaje;
                    string subject = "Cotización Aprobada";

                    SmtpClient client = new SmtpClient(smtpServidor, 587);
                    MailAddress from = new MailAddress(mailFrom);
                    MailAddress to = new MailAddress(Email);
                    MailMessage message = new MailMessage(from, to);

                    message.Body = mensaje;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = subject;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;

                    client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
                    client.EnableSsl = true;
                    client.Send(message);
                }


                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se actualizó el seguimiento de forma satisfactoria";
                respuesta.Objeto = Seguimiento;


                return respuesta;
            }

            catch (Exception e)
            {
                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "Ocurrió un error al actualizar el seguimiento";
                respuesta.Objeto = Seguimiento;

                return respuesta;

            }
        }

        [Route("api/Autos/PostActualizarOtrosDatosVehiculo", Name = "PostActualizarOtrosDatosVehiculo")]
        public Respuesta PostActualizarOtrosDatosVehiculo([FromBody] ActualizarOtrosDatosVehiculo DatosVehiculo)
        {
            Respuesta respuesta = new Respuesta();

            DVADB.DB2 dbcnx = new DVADB.DB2();

            try
            {
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APDVEHMB ";
                strSql += " WHERE FIAPIDCUEN = " + DatosVehiculo.IdCuenta;
                strSql += " AND FIAPIDVEHI = " + DatosVehiculo.IdVehiculo;


                DataTable dt = dbcnx.GetDataSet(strSql).Tables[0];
                DataView dtX = dt.DefaultView;
                dt = dtX.ToTable();

                dbcnx.AbrirConexion();
                dbcnx.BeginTransaccion();



                if (dt.Rows.Count != 0)
                {

                    strSql = "";

                    if (DatosVehiculo.IdEngomado != "")
                    {
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APDVEHMB ";
                        strSql += " SET FIAPIDENGO = " + DatosVehiculo.IdEngomado;
                        strSql += " WHERE FIAPIDCUEN = " + DatosVehiculo.IdCuenta;
                        strSql += " AND FIAPIDVEHI = " + DatosVehiculo.IdVehiculo;
                        dbcnx.SetQuery(strSql);
                    }

                    if (DatosVehiculo.FechaGarantiaExt != "")
                    {
                        strSql = "";
                        strSql += "UPDATE " + constantes.Ambiente + "APPS.APDVEHMB ";
                        strSql += " SET FFAPGTIAEX= '" + DatosVehiculo.FechaGarantiaExt + "' ";
                        strSql += " WHERE FIAPIDCUEN = " + DatosVehiculo.IdCuenta;
                        strSql += " AND FIAPIDVEHI = " + DatosVehiculo.IdVehiculo;
                        dbcnx.SetQuery(strSql);
                    }



                }
                else
                {
                    if (DatosVehiculo.IdEngomado == "")
                    {
                        DatosVehiculo.IdEngomado = "default";
                    }



                    strSql = "";
                    strSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDVEHMB ";
                    strSql += "(FIAPIDCUEN, FIAPIDVEHI, FSAPNUMPOL, FIAPIDASEG, FSAPTELASE, FFAPVENCIM, FIAPIDENGO, FFAPGTIAEX, FIAPSTATUS, USERCREAT,PROGCREAT) ";
                    strSql += "VALUES ";
                    strSql += "(";
                    strSql += DatosVehiculo.IdCuenta + ",";
                    strSql += DatosVehiculo.IdVehiculo + ",";
                    strSql += "default,";
                    strSql += DatosVehiculo.IdAseguradora + ",";
                    strSql += "default,";
                    strSql += "default,";
                    strSql += DatosVehiculo.IdEngomado + ",";
                    if (DatosVehiculo.FechaGarantiaExt != "")
                    {
                        strSql += "'" + DatosVehiculo.FechaGarantiaExt + "',";
                    }
                    else
                    {
                        strSql += "default,";
                    }
                    strSql += "1,'APP','APP'";
                    strSql += ")";
                    dbcnx.SetQuery(strSql);


                }

                dbcnx.CommitTransaccion();
                dbcnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos actualizados correctamente";
                respuesta.Objeto = JsonConvert.SerializeObject(DatosVehiculo);

                return respuesta;

            }
            catch (Exception e)
            {
                dbcnx.RollbackTransaccion();
                dbcnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "Ocurrió un error al tratar de actualizar los datos";
                respuesta.Objeto = JsonConvert.SerializeObject(DatosVehiculo);

                return respuesta;

            }
        }

        // POST api/Autos/GetObtenerAgenciasxIdMarca
        [Route("api/Autos/GetObtenerAgenciasxIdMarca", Name = "GetObtenerAgenciasxIdMarca")]
        public List<AgenciaCitaServicio> GetObtenerAgenciasxIdMarca(String IdMarca)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {
                DVAConstants.Constants constantes = new DVAConstants.Constants();


                string strSql = "";
                strSql = "SELECT    CIAUN.* ";
                strSql += "FROM	" + constantes.Ambiente + "GRAL.GECCIAUN CIAUN INNER JOIN PRODSERV.SECGPCIT GPCIT ON CIAUN.FIGEIDCIAU = GPCIT.FISEIDCIAU AND GPCIT.FISESTATUS = 1 AND GPCIT.FISEAPLAMA = 1 ";
                strSql += "WHERE CIAUN.FIGESTATUS = 1 ";
                strSql += "AND CIAUN.FIGEIDMARC = " + IdMarca;
                strSql += " ORDER BY TRIM(CIAUN.FSGERAZCOM)";


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                DataView dtX = dt.DefaultView;
                dtX.Sort = "FSGERAZCOM";
                dt = dtX.ToTable();

                AgenciaCitaServicio agencia;
                List<AgenciaCitaServicio> coleccionAgencias = new List<AgenciaCitaServicio>();
                foreach (DataRow dr in dt.Rows)
                {
                    agencia = new AgenciaCitaServicio();
                    agencia.IdMarca = dr["FIGEIDMARC"].ToString().Trim();
                    agencia.IdAgencia = dr["FIGEIDCIAU"].ToString().Trim();
                    agencia.Agencia = dr["FSGERAZCOM"].ToString().Trim();

                    coleccionAgencias.Add(agencia);
                }

                return coleccionAgencias;
            }
            catch (Exception ex)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();


                List<AgenciaCitaServicio> coleccionAgencias = new List<AgenciaCitaServicio>();
                return coleccionAgencias;

            }


        }

        // POST api/Autos/GetObtenerAgenciasxIdsMarcas
        [Route("api/Autos/GetObtenerAgenciasxIdsMarcas", Name = "GetObtenerAgenciasxIdsMarcas")]
        public List<AgenciaCitaServicio> GetObtenerAgenciasxIdsMarcas(string IdsMarcas)
        {
            string idsMarcas = IdsMarcas;

            //idsMarcas = idsMarcas.Replace("[", "(");
            //idsMarcas = idsMarcas.Replace("]", ")");

            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT    CIAUN.* ";
                strSql += "FROM	" + constantes.Ambiente + "GRAL.GECCIAUN CIAUN INNER JOIN PRODSERV.SECGPCIT GPCIT ON CIAUN.FIGEIDCIAU = GPCIT.FISEIDCIAU AND GPCIT.FISESTATUS = 1 AND GPCIT.FISEAPLAMA = 1 ";
                strSql += "WHERE CIAUN.FIGESTATUS = 1 ";
                strSql += "AND CIAUN.FIGEIDMARC IN (" + idsMarcas + " )";
                strSql += " ORDER BY TRIM(CIAUN.FSGERAZCOM)";


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                DataView dtX = dt.DefaultView;
                dtX.Sort = "FSGERAZCOM";
                dt = dtX.ToTable();

                AgenciaCitaServicio agencia;
                List<AgenciaCitaServicio> coleccionAgencias = new List<AgenciaCitaServicio>();
                foreach (DataRow dr in dt.Rows)
                {
                    agencia = new AgenciaCitaServicio();
                    agencia.IdMarca = dr["FIGEIDMARC"].ToString().Trim();
                    agencia.IdAgencia = dr["FIGEIDCIAU"].ToString().Trim();
                    agencia.Agencia = dr["FSGERAZCOM"].ToString().Trim();

                    coleccionAgencias.Add(agencia);
                }

                return coleccionAgencias;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<AgenciaCitaServicio> coleccionAgencias = new List<AgenciaCitaServicio>();
                return coleccionAgencias;
            }

        }

        // POST api/Autos/GetObtenerAgendaCitasCliente
        [Route("api/Autos/GetObtenerAgendaCitasCliente", Name = "GetObtenerAgendaCitasCliente")]
        public List<CitaServicioAgendada> GetObtenerAgendaCitasCliente(long IdPersona)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                DateTime Hoy = DateTime.Today;

                string FechaHoy = Hoy.ToString("yyyy-MM-dd");

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "SERV.SEECITAS CITAS ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.SEDCITAS DETALLE ";
                strSql += "ON	CITAS.FISEIDCIAU = DETALLE.FISEIDCIAU ";
                strSql += "AND 	CITAS.FISEIDCITA = DETALLE.FISEIDCITA ";
                strSql += "INNER JOIN " + constantes.Ambiente + "GRAL.GECCIAUN AGENCIA ";
                strSql += "ON	CITAS.FISEIDCIAU = AGENCIA.FIGEIDCIAU ";
                strSql += "INNER JOIN " + constantes.Ambiente + "AUT.ANCVEHIC VEHIC ";
                strSql += "ON	VEHIC.FNVEHICID = DETALLE.FISEIDVEHI ";
                strSql += "INNER JOIN " + constantes.Ambiente + "SERV.ANCVERSI VERSI ";
                strSql += "ON	VEHIC.FNVEHIVER = VERSI.FNVERSIDV ";
                strSql += "WHERE CITAS.FISESTATUS = 1 ";
                strSql += "AND CITAS.FISEIDESTA <> 4 "; // La constante está mal
                strSql += "AND CITAS.FFSEFECINC >= '" + FechaHoy + "' ";
                strSql += "AND CITAS.FISEIDPERS = " + IdPersona;


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                CitaServicioAgendada cita;
                List<CitaServicioAgendada> coleccionSeguimiento = new List<CitaServicioAgendada>();
                foreach (DataRow dr in dt.Rows)
                {
                    cita = new CitaServicioAgendada();
                    cita.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                    cita.Agencia = dr["FSGERAZSOC"].ToString().Trim();
                    cita.IdCita = dr["FISEIDCITA"].ToString().Trim();
                    cita.FolioCita = dr["FISEFOLIO"].ToString().Trim();
                    cita.Fecha = Convert.ToDateTime(dr["FFSEFECINC"]).ToString("dd-MM-yyyy");
                    cita.Hora = Convert.ToDateTime(dr["FHSEHRAINC"]).ToString("HH:mm:ss");
                    cita.Asunto = dr["FSSEASUNTO"].ToString().Trim();
                    cita.ModeloVersion = dr["FSSEMODELO"].ToString().Trim() + " - " + dr["FSVERSDES"].ToString().Trim();

                    coleccionSeguimiento.Add(cita);
                }

                return coleccionSeguimiento;

            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<CitaServicioAgendada> coleccionSeguimiento = new List<CitaServicioAgendada>();

                return coleccionSeguimiento;

            }

        }

        // POST api/Autos/PostObtenerAnios
        [Route("api/Autos/GetObtenerAnios", Name = "GetObtenerAnios")]
        public List<AnioVehiculo> GetObtenerAnios(int IdModelo)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                string strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "AUT.ANCVERSI VERSI ";
                strSql += "WHERE VERSI.FIANSTATU = 1 ";
                strSql += "AND VERSI.FNVERSIDM = " + IdModelo;


                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                AnioVehiculo anioVehiculo;
                List<AnioVehiculo> coleccionVersiones = new List<AnioVehiculo>();
                foreach (DataRow dr in dt.Rows)
                {
                    anioVehiculo = new AnioVehiculo();
                    anioVehiculo.IdModelo = dr["FNVERSIDM"].ToString().Trim();
                    anioVehiculo.Anio = dr["FNVERSANO"].ToString().Trim();

                    coleccionVersiones.Add(anioVehiculo);
                }

                coleccionVersiones = coleccionVersiones.OrderBy(o => o.Anio).ToList();

                for (int x = 0; x < coleccionVersiones.Count(); x++)
                {
                    if (x != 0)
                    {
                        if (coleccionVersiones[x].Anio == coleccionVersiones[x - 1].Anio)
                        {
                            coleccionVersiones.Remove(coleccionVersiones[x]);
                            x--;
                        }
                    }
                }

                return coleccionVersiones;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                List<AnioVehiculo> coleccionVersiones = new List<AnioVehiculo>();

                return coleccionVersiones;

            }
        }

        // POST api/Autos/PostObtenerAutoSeminuevo
        [Route("api/Autos/GetObtenerAutoSeminuevo", Name = "GetObtenerAutoSeminuevo")]
        public AutoUsado GetObtenerAutoSeminuevo(long IdVehiculo)
        {
            try
            {

                var cliente = new RestClient("http://ws-smartit.divisionautomotriz.com/wsApiCasaTrust/api/autos/" + "ObtenerAutoPorId" + "?IdVehiculo=" + IdVehiculo.ToString());
                cliente.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = cliente.Execute(request);
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                AutoUsado auto = deserial.Deserialize<AutoUsado>(response);
                if (auto == null)
                {
                    auto = new AutoUsado();
                }
                //var autos = cliente.Execute<List<AutoUsado>>(request);
                return auto;
            }
            catch (Exception ex)
            {
                AutoUsado auto = new AutoUsado();

                return auto;
            }

        }

        // POST api/Autos/PostObtenerAutosSeminuevos
        [Route("api/Autos/GetObtenerAutosSeminuevos", Name = "GetObtenerAutosSeminuevos")]
        public List<AutoUsado> GetObtenerAutosSeminuevos(int IdMarca)
        {
            DVADB.DB2 cnx = new DVADB.DB2();

            List<int> cias = new List<int>();
            try
            {
                string sql = "SELECT FIGEIDCIAU FROM PRODGRAL.GECCIAUN WHERE FIGEIDMARC = " + IdMarca.ToString();
                DataTable dt = cnx.GetDataSet(sql).Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    cias.Add(Convert.ToInt32(dr["FIGEIDCIAU"]));
                }

                var cliente = new RestClient("http://ws-smartit.divisionautomotriz.com/wsApiCasaTrust/api/autos/" + "ObtenerTodos");
                cliente.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = cliente.Execute(request);
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                List<AutoUsado> autos = deserial.Deserialize<List<AutoUsado>>(response);
                return autos.Where(x => cias.Contains(Convert.ToInt32(x.IdAgencia))).ToList();
            }
            catch (Exception)
            {
                cnx.RollbackTransaccion();
                cnx.CerrarConexion();

                List<AutoUsado> autos = new List<AutoUsado>();
                return autos;
            }

            //var autos = cliente.Execute<List<AutoUsado>>(request);
            //return autos.Where(x=> Convert.ToInt32(x.IdMarca) == IdMarca).ToList();
        }

        [Route("api/Autos/GetObtenerModelos", Name = "GetObtenerModelos")]
        public List<ModelosCarros> GetObtenerModelos()
        {
            BAutos bussinsess = new BAutos();
            return bussinsess.GetObtenerModelos();
        }

        [Route("api/Autos/GetObtenerMedia", Name = "GetObtenerMedia")]
        public List<MediaProc2> GetObtenerMedia()
        {
            BAutos bussinsess = new BAutos();
            return bussinsess.ObtenerMedia();
        }

    }
}
