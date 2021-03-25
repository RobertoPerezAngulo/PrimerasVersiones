using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

using wsApiSeat.Models;
using wsApiSeat.Models.Notificaciones;
using wsApiSeat.Models.OrdenCompra;
using RouteAttribute = System.Web.Http.RouteAttribute;
using wsApiSeat.Services;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Globalization;
using wsApiSeat.Models.ContactoAgencia;
using System.Web.Hosting;
using System.Net.Http;
using wsApiSeat.Bussiness;
using System.Threading.Tasks;

namespace wsApiSeat.Controllers
{
    public class OrdenCompraController : ApiController
    {
        [Route("api/OrdenCompra/UpdateEstadoCompraSmart", Name = "UpdateEstadoCompraSmart")]
        public async Task<Respuesta> UpdateEstadoCompraSmart(long aIdCompra, int aIdEstado)
        {
            BPedido _update = new BPedido();
            return await _update.UpdateEstadoCompraSmart(aIdCompra,aIdEstado);
        }

        [Route("api/OrdenCompra/PostSeleccionaSeguro", Name = "PostSeleccionaSeguro")]
        public async Task<Respuesta> PostSeleccionaSeguro([FromBody]SeleccionaSeguroCliente seguro)
        {
            BPedido servicio = new BPedido();       
            return await servicio.UpdateSeleccionaSeguro(seguro);
        }

        [Route("api/OrdenCompra/GetObtenerSeguros", Name = "GetObtenerSeguros")]
        public List<SegurosCliente> GetObtenerSeguros(long IdCompra, long Idcuenta)
        {
            BPedido servicio = new BPedido();
            return servicio.GetObtenerSeguros(IdCompra,Idcuenta);
        }

        [Route("api/OrdenCompra/GetObtenerSegurosSmartIT", Name = "GetObtenerSegurosSmartIT")]
        public List<SegurosClienteSmartIt> GetObtenerSegurosSmartIT(long IdCompra, long Idcuenta)
        {
            BPedido servicio = new BPedido();
            return servicio.GetObtenerSegurosSmartIT(IdCompra, Idcuenta);
        }

        [Route("api/OrdenCompra/PostSubirPolizasSmartIT", Name = "PostSubirPolizasSmartIT")]
        [System.Web.Http.HttpPost]
        public async Task<Respuesta> SubirPolizasSmartIT([FromBody]SubirArchivoSmartIT archivo)
        {
            Respuesta respuesta = new Respuesta();
            BPedido service = new BPedido();
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Resources/Polizas/");
            string salida = string.Empty;
            try
            {
                if (archivo.ExtensionArch.ToLower().Trim() == "pdf")
                {

                    //var name = archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.Tipo + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + archivo.ExtensionArch;
                    var name = archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.Tipo + "." + archivo.ExtensionArch;
                    string filePath = root.ToString() + name;
                    if (!File.Exists(filePath))
                    {
                        salida =  await service.creaPDF(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = service.PostSubirPolizaSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta),archivo.NombreAseguradora,filePath,Math.Round(Convert.ToDecimal(archivo.Total),2).ToString(), archivo.Cobertura);
                    }
                    else
                    {
                        File.Delete(filePath);
                        salida = await service.creaPDF(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = service.UpdateRegisterSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, filePath, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura);
                    }

                }
                else if (archivo.ExtensionArch.ToLower().Trim() == "png" || archivo.ExtensionArch.ToLower().Trim() == "jpeg" || archivo.ExtensionArch.ToLower().Trim() == "jpg")
                {

                    var name = archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.Tipo + "_" + DateTime.Now.ToString("yyyyMMdd") + ".Jpeg";
                    string filePath = root.ToString() + name;

                    if (!File.Exists(filePath))
                    {
                        salida = await service.creaImagen(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = service.PostSubirPolizaSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, filePath, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura);
                    }
                    else
                    {
                        File.Delete(filePath);
                         salida = await service.creaImagen(filePath, archivo.Base64);
                        if (salida == "SI")
                            respuesta = service.UpdateRegisterSmartIT(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.Tipo), archivo.NombreDocumento, Convert.ToInt64(archivo.IdCuenta), archivo.NombreAseguradora, filePath, Math.Round(Convert.ToDecimal(archivo.Total), 2).ToString(), archivo.Cobertura);
                    }
                }

            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo subir el archivo.";
            }
            return respuesta;
        }

        // GET api/OrdenCompra/GetEstadosOrdenCompra
        [Route("api/OrdenCompra/GetEstadosOrdenCompra", Name = "GetObtenerEstadosOrdenCompra")]
        public List<EstadoOrdenCompra> GetObtenerEstadosOrdenCompra()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();


            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCESCST ";
            strSql += "WHERE FIAPSTATUS = 1";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            EstadoOrdenCompra orden;
            List<EstadoOrdenCompra> coleccionOrdenes = new List<EstadoOrdenCompra>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    orden = new EstadoOrdenCompra();
                    orden.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    orden.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();

                    coleccionOrdenes.Add(orden);
                }

            }
            else
            {

                orden = new EstadoOrdenCompra();
                orden.IdEstado = null;
                orden.DescripcionEstado = null;

                coleccionOrdenes.Add(orden);

            }

            return coleccionOrdenes;
        }

        // GET api/OrdenCompra/GetEstados
        [Route("api/OrdenCompra/GetEstados", Name = "GetEstados")]
        public List<EstadoNotificaciones> GetEstados()
        {
            List<EstadoNotificaciones> lstEstado = new List<EstadoNotificaciones>();

            try
            {
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\MensajeNotificacion.json");
                //string strJSON = File.ReadAllText(@"C:\Users\desar\OneDrive\Desktop\Jsons\MensajeNotificacion.json");

                lstEstado = JsonConvert.DeserializeObject<List<EstadoNotificaciones>>(strJSON);

            }
            catch (Exception _exc)
            {
                lstEstado = null;
            }

            return lstEstado;
        }

        // GET api/OrdenCompra/GetEstadosOrdenCompra
        [Route("api/OrdenCompra/GetObtenerSeguimientoCompra", Name = "GetObtenerSeguimientoCompra")]
        public List<SeguimientoCompra> GetSeguimientoCompra(int IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT seg.FIAPIDCOMP, seg.FIAPIDSEGU, seg.FSAPTITSEG, seg.FIAPIDESTA, est.FSAPESTADO, seg.DATECREAT, seg.TIMECREAT ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDSGCST seg ";
            strSql += "inner join prodapps. APCESCST est ";
            strSql += "ON seg.FIAPIDESTA = est.FIAPIDESTA ";
            strSql += "WHERE seg.FIAPIDCOMP = " + IdCompra + " AND seg.FIAPSTATUS = 1";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            SeguimientoCompra seguimiento;
            List<SeguimientoCompra> coleccionSeguimiento = new List<SeguimientoCompra>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    seguimiento = new SeguimientoCompra();
                    seguimiento.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    seguimiento.IdSeguimiento = dr["FIAPIDSEGU"].ToString().Trim();
                    seguimiento.TituloSeguimiento = dr["FSAPTITSEG"].ToString().Trim();
                    seguimiento.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    seguimiento.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    seguimiento.Fecha = dr["DATECREAT"].ToString().Trim();
                    seguimiento.Hora = dr["TIMECREAT"].ToString().Trim();

                    coleccionSeguimiento.Add(seguimiento);
                }

            }
            else
            {

                seguimiento = new SeguimientoCompra();
                seguimiento.IdCompra = null;
                seguimiento.IdSeguimiento = null;
                seguimiento.TituloSeguimiento = null;
                seguimiento.IdEstado = null;
                seguimiento.DescripcionEstado = null;
                coleccionSeguimiento.Add(seguimiento);

            }

            return coleccionSeguimiento;
        }

        // GET api/OrdenCompra/GetDocumento
        [Route("api/OrdenCompra/GetDocumento", Name = "GetDocumento")]
        public List<Documento> GetDocumento(int IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT FIAPIDCOMP, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APDDCPST ";
            strSql += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            Documento documento;
            List<Documento> coleccionDocumento = new List<Documento>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    documento = new Documento();
                    documento.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    documento.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                    documento.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                    documento.RutaDocumento = dr["FSAPRUTDOC"].ToString().Trim(); //.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/").Replace('\\', '/');

                    coleccionDocumento.Add(documento);
                }

            }
            else
            {

                documento = new Documento();
                documento.IdCompra = null;
                documento.IdTipo = null;
                documento.NombreDocumento = null;
                documento.RutaDocumento = null;

                coleccionDocumento.Add(documento);

            }

            return coleccionDocumento;
        }

        [Route("api/OrdenCompra/GetOrdenesCompra", Name = "GetOrdenesCompra")]
        public List<Compra> GetOrdenesCompra(int IdCuenta)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Compra compra = new Compra();
            List<Compra> coleccionCompras = new List<Compra>();

            string strSql = "";
            strSql += "select ape.FIAPIDCOMP, ape.FIAPFOLCOM, ape.FIAPIDCUEN, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT, ape.FDAPDESCUE, ape.FDAPIVA, ape.FDAPTOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO ";
            strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
            strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
            strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
            strSql += "WHERE ape.FIAPSTATUS = 1 AND ape.FIAPIDESTA NOT IN(15,24) AND ape.FIAPIDCUEN = " + IdCuenta + " ORDER BY ape.FIAPIDCOMP ASC";
            
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataTable dtP;
            DataTable dtA;

            PedidoVehiculo pedidoV;
            List<Models.OrdenCompra.Accesorio> Coleccionaccesorios;

            Models.OrdenCompra.Accesorio accesorio;


            string strSqlPedido = "";
            string strSqlAccesorios = "";

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new Compra();
                    compra.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    compra.FolioCompra = dr["FIAPFOLCOM"].ToString().Trim();
                    compra.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    compra.FechaCompra = dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = dr["FHAPHORCOM"].ToString().Trim();
                    compra.Subtotal = dr["FDAPSUBTOT"].ToString().Trim();
                    compra.Descuento = dr["FDAPDESCUE"].ToString().Trim();
                    compra.IVA = dr["FDAPIVA"].ToString().Trim();
                    compra.Total = dr["FDAPTOTAL"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    compra.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    compra.IdPaso = dr["FIAPIDPASO"].ToString().Trim();


                    strSqlPedido = "";
                    strSqlPedido += "select dan.FIAPIDCOMP, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPTRANSM ,dan.FSAPCOLEXT, dan.FSAPNUMINV,dan.FSAPNUMSER, dan.FDAPSUBTOT, dan.FDAPDESCUE, dan.FDAPIVA, dan.FDAPTOTAL, dan.FIAPCOTSEG, dan.FSAPRUTFOT  ";
                    strSqlPedido += " FROM " + constantes.Ambiente + "apps.APEPANST dan ";
                    strSqlPedido += " WHERE dan.FIAPIDCOMP = " + compra.IdCompra;
                    strSqlPedido += " and dan.FIAPSTATUS = 1";
                    

                    dtP = dbCnx.GetDataSet(strSqlPedido).Tables[0];

                    if (dtP.Rows.Count > 0)
                    {

                        foreach (DataRow drP in dtP.Rows)
                        {

                            pedidoV = new PedidoVehiculo();
                            pedidoV.IdCompra = drP["FIAPIDCOMP"].ToString().Trim();
                            pedidoV.IdAgencia = drP["FIAPIDCIAU"].ToString().Trim();
                            pedidoV.IdPedido = drP["FIAPIDPEDI"].ToString().Trim();
                            pedidoV.FechaPedido = drP["FFAPFECPED"].ToString().Trim();
                            pedidoV.HoraPedido = drP["FHAPHORPED"].ToString().Trim();
                            pedidoV.IdPersona = drP["FIAPIDPERS"].ToString().Trim();
                            pedidoV.IdVehiculo = drP["FIAPIDVEHI"].ToString().Trim();
                            pedidoV.IdInventario = drP["FIAPIDINVE"].ToString().Trim();
                            pedidoV.NumeroDeSerie = drP["FSAPNUMSER"].ToString().Trim();
                            pedidoV.Subtotal = drP["FDAPSUBTOT"].ToString().Trim();
                            pedidoV.Descuento = drP["FDAPDESCUE"].ToString().Trim();
                            pedidoV.Iva = drP["FDAPIVA"].ToString().Trim();
                            pedidoV.Total = drP["FDAPTOTAL"].ToString().Trim();
                            pedidoV.CotizarSeguro = drP["FIAPCOTSEG"].ToString().Trim();
                            pedidoV.Modelo = drP["FSAPMODELO"].ToString().Trim();
                            pedidoV.Version = drP["FSAPVERSIO"].ToString().Trim();
                            pedidoV.ColorExterior = drP["FSAPCOLEXT"].ToString().Trim();
                            pedidoV.NumeroInventario = drP["FSAPNUMINV"].ToString().Trim();
                            pedidoV.RutaFoto = drP["FSAPRUTFOT"].ToString().Trim();
                            pedidoV.Transmision = drP["FSAPTRANSM"].ToString().Trim();
                            compra.Pedido = pedidoV;


                            strSqlAccesorios = "";
                            strSqlAccesorios += "select dedan.FIAPIDCIAU, dedan.FIAPIDPEDI, dedan.FIAPIDCONS, dedan.FSAPCONCEP, dedan.FDAPSUBTOT, dedan.FDAPDESCUE, dedan.FDAPIVA, dedan.FDAPTOTAL, dedan.FSAPRUTFOT ";
                            strSqlAccesorios += " FROM " + constantes.Ambiente + "apps.APDPANST dedan ";
                            strSqlAccesorios += "where dedan.FIAPSTATUS=1 ";
                            strSqlAccesorios += "and dedan.FIAPIDCIAU = " + pedidoV.IdAgencia;
                            strSqlAccesorios += " and dedan.FIAPIDCOMP = " + pedidoV.IdCompra;

                            dtA = dbCnx.GetDataSet(strSqlAccesorios).Tables[0];

                            if (dtA.Rows.Count > 0)
                            {
                                Coleccionaccesorios = new List<Models.OrdenCompra.Accesorio>();
                                foreach (DataRow drA in dtA.Rows)
                                {

                                    accesorio = new Models.OrdenCompra.Accesorio();
                                    accesorio.IdAgencia = drA["FIAPIDCIAU"].ToString().Trim();
                                    accesorio.IdPedido = drA["FIAPIDPEDI"].ToString().Trim();
                                    accesorio.IdConsecutivo = drA["FIAPIDCONS"].ToString().Trim();
                                    accesorio.Concepto = drA["FSAPCONCEP"].ToString().Trim();
                                    accesorio.Subtotal = drA["FDAPSUBTOT"].ToString().Trim();
                                    accesorio.Descuento = drA["FDAPDESCUE"].ToString().Trim();
                                    accesorio.Iva = drA["FDAPIVA"].ToString().Trim();
                                    accesorio.Total = drA["FDAPTOTAL"].ToString().Trim();
                                    accesorio.RutaFoto = drA["FSAPRUTFOT"].ToString().Trim();

                                    Coleccionaccesorios.Add(accesorio);

                                }

                                compra.Accesorios = Coleccionaccesorios;

                            }
                            else
                            {
                                List<Accesorio> _accesorio = new List<Accesorio>();
                                compra.Accesorios = _accesorio;

                            }
                        }

                    }
                    else
                    {

                        compra.Pedido = null;
                    }
  
                    coleccionCompras.Add(compra);
                }
            }
            return coleccionCompras;
        }
        
        [Route("api/OrdenCompra/GetOrdenCompra", Name = "GetOrdenCompra")]
        public Compra GetOrdenCompra(int IdCompra)
        {
            Bussiness.BPedido bussiness = new Bussiness.BPedido();
            return bussiness.OrdenCompra(IdCompra);
        }
        

        [Route("api/OrdenCompra/GetOrdenesDeCompraEnProcesoPorAgencia", Name = "GetOrdenesDeCompraEnProcesoPorAgencia")]
        public List<Compra> GetOrdenesDeCompraEnProcesoPorAgencia(int idAgencia)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Compra compra = new Compra();
            List<Compra> coleccionCompras = new List<Compra>();

            //Compra
            string strSql = "";
            strSql += "select ape.FIAPIDCOMP, ape.FIAPFOLCOM, ape.FIAPIDCUEN, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT, ape.FDAPDESCUE, ape.FDAPIVA, ape.FDAPTOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPASO ";
            strSql += "FROM prodapps.APECMPST ape ";
            strSql += "inner join PRODapps.APCESCST est ";
            strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
            strSql += "INNER JOIN prodapps.APEPANST pedan ";
            strSql += "on ape.FIAPIDCOMP = pedan.FIAPIDCOMP ";
            strSql += "WHERE ape.FIAPSTATUS = 1 ";
            strSql += "and pedan.FIAPIDCIAU = " + idAgencia;
                       

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new Compra();
                    compra.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    compra.FolioCompra = dr["FIAPFOLCOM"].ToString().Trim();
                    compra.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    compra.FechaCompra = dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = dr["FHAPHORCOM"].ToString().Trim();
                    compra.Subtotal = dr["FDAPSUBTOT"].ToString().Trim();
                    compra.Descuento = dr["FDAPDESCUE"].ToString().Trim();
                    compra.IVA = dr["FDAPIVA"].ToString().Trim();
                    compra.Total = dr["FDAPTOTAL"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    compra.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdPaso = dr["FIAPIDPASO"].ToString().Trim();


                    compra.Pedido = null;
                    compra.Accesorios = null;

                    coleccionCompras.Add(compra);
                }

            }
            else
            {

                compra = new Compra();
                compra.IdCompra = null;
                compra.FolioCompra = null;
                compra.IdCuenta = null;
                compra.FechaCompra = null;
                compra.HoraCompra = null;
                compra.Subtotal = null;
                compra.Descuento = null;
                compra.IVA = null;
                compra.Total = null;
                compra.IdEstado = null;
                compra.DescripcionEstado = null;
                compra.Pedido = null;
                compra.Accesorios = null;

                coleccionCompras.Add(compra);

            }

            return coleccionCompras;



        }

        // GET api/OrdenCompra/GetOrdenCompra
        [Route("api/OrdenCompra/GetOrdenesDeCompraEnProcesoSmartIt", Name = "GetOrdenesDeCompraEnProcesoSmartIt")]
        public List<Compra> GetOrdenesDeCompraEnProcesoSmartIt()
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Compra compra = new Compra();
            List<Compra> coleccionCompras = new List<Compra>();

            //Compra
            string strSql = "";
            strSql += "select ape.FIAPIDCOMP, ape.FIAPFOLCOM, ape.FIAPIDCUEN, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT, ape.FDAPDESCUE, ape.FDAPIVA, ape.FDAPTOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPASO ";
            strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
            strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
            strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
            strSql += "WHERE ape.FIAPSTATUS = 1";


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new Compra();
                    compra.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    compra.FolioCompra = dr["FIAPFOLCOM"].ToString().Trim();
                    compra.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    compra.FechaCompra = dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = dr["FHAPHORCOM"].ToString().Trim();
                    compra.Subtotal = dr["FDAPSUBTOT"].ToString().Trim();
                    compra.Descuento = dr["FDAPDESCUE"].ToString().Trim();
                    compra.IVA = dr["FDAPIVA"].ToString().Trim();
                    compra.Total = dr["FDAPTOTAL"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    compra.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdPaso = dr["FIAPIDPASO"].ToString().Trim();


                    compra.Pedido = null;
                    compra.Accesorios = null;

                    coleccionCompras.Add(compra);
                }

            }
            else
            {

                compra = new Compra();
                compra.IdCompra = null;
                compra.FolioCompra = null;
                compra.IdCuenta = null;
                compra.FechaCompra = null;
                compra.HoraCompra = null;
                compra.Subtotal = null;
                compra.Descuento = null;
                compra.IVA = null;
                compra.Total = null;
                compra.IdEstado = null;
                compra.DescripcionEstado = null;
                compra.Pedido = null;
                compra.Accesorios = null;

                coleccionCompras.Add(compra);

            }

            return coleccionCompras;



        }

        // GET api/OrdenCompra/GetOrdenCompra
        [Route("api/OrdenCompra/GetOrdenesDeCompraEnProceso", Name = "GetOrdenesDeCompraEnProceso")]
        public List<Compra> GetOrdenesDeCompraEnProceso()
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Compra compra = new Compra();
            List<Compra> coleccionCompras = new List<Compra>();

            //Compra
            string strSql = "";
            strSql += "select ape.FIAPIDCOMP, ape.FIAPFOLCOM, ape.FIAPIDCUEN, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT, ape.FDAPDESCUE, ape.FDAPIVA, ape.FDAPTOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO ";
            strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
            strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
            strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA AND ape.FIAPIDESTA <> 15 "; // 15 CANCELADO
            strSql += "WHERE ape.FIAPSTATUS = 1";



            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new Compra();
                    compra.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    compra.FolioCompra = dr["FIAPFOLCOM"].ToString().Trim();
                    compra.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    compra.FechaCompra = dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = dr["FHAPHORCOM"].ToString().Trim();
                    compra.Subtotal = dr["FDAPSUBTOT"].ToString().Trim();
                    compra.Descuento = dr["FDAPDESCUE"].ToString().Trim();
                    compra.IVA = dr["FDAPIVA"].ToString().Trim();
                    compra.Total = dr["FDAPTOTAL"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    compra.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    compra.IdPaso = dr["FIAPIDPASO"].ToString().Trim();



                    compra.Pedido = null;
                    compra.Accesorios = null;

                    coleccionCompras.Add(compra);
                }

            }
            else
            {

                compra = new Compra();
                compra.IdCompra = null;
                compra.FolioCompra = null;
                compra.IdCuenta = null;
                compra.FechaCompra = null;
                compra.HoraCompra = null;
                compra.Subtotal = null;
                compra.Descuento = null;
                compra.IVA = null;
                compra.Total = null;
                compra.IdEstado = null;
                compra.DescripcionEstado = null;
                compra.Pedido = null;
                compra.Accesorios = null;

                coleccionCompras.Add(compra);

            }

            return coleccionCompras;



        }


        [Route("api/OrdenCompra/GetOrdenesCompraSmartIt", Name = "GetOrdenesCompraSmartIt")]
        public List<Compra> GetOrdenesCompraSmartIt()
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Compra compra = new Compra();
            List<Compra> coleccionCompras = new List<Compra>();

            //Compra
            string strSql = "";
            strSql += "select ape.FIAPIDCOMP, ape.FIAPFOLCOM, ape.FIAPIDCUEN, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT, ape.FDAPDESCUE, ape.FDAPIVA, ape.FDAPTOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO ";
            strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
            strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
            strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
            strSql += "WHERE ape.FIAPSTATUS = 1";


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    compra = new Compra();
                    compra.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    compra.FolioCompra = dr["FIAPFOLCOM"].ToString().Trim();
                    compra.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                    compra.FechaCompra = dr["FFAPFECCOM"].ToString().Trim();
                    compra.HoraCompra = dr["FHAPHORCOM"].ToString().Trim();
                    compra.Subtotal = dr["FDAPSUBTOT"].ToString().Trim();
                    compra.Descuento = dr["FDAPDESCUE"].ToString().Trim();
                    compra.IVA = dr["FDAPIVA"].ToString().Trim();
                    compra.Total = dr["FDAPTOTAL"].ToString().Trim();
                    compra.IdEstado = dr["FIAPIDESTA"].ToString().Trim();
                    compra.DescripcionEstado = dr["FSAPESTADO"].ToString().Trim();
                    compra.RutaReferenciaBancaria = dr["FSAPRUTRFB"].ToString().Trim();
                    compra.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    compra.IdPaso = dr["FIAPIDPASO"].ToString().Trim();


                    compra.Pedido = null;
                    compra.Accesorios = null;

                    coleccionCompras.Add(compra);
                }

            }
            else
            {

                compra = new Compra();
                compra.IdCompra = null;
                compra.FolioCompra = null;
                compra.IdCuenta = null;
                compra.FechaCompra = null;
                compra.HoraCompra = null;
                compra.Subtotal = null;
                compra.Descuento = null;
                compra.IVA = null;
                compra.Total = null;
                compra.IdEstado = null;
                compra.DescripcionEstado = null;
                compra.Pedido = null;
                compra.Accesorios = null;

                coleccionCompras.Add(compra);

            }

            return coleccionCompras;
        }


        [Route("api/OrdenCompra/GetActualizaEstatusOrdenCompra", Name = "GetActualizaEstatusOrdenCompra")]
        public Respuesta GetActualizaEstatusOrdenCompra(int IdCompra, int PasoSiguiente)
        {
            string nuevoPaso = "";
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();
            string strSqlSeg = "";
            strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APECMPST "; //  SE ACTUALIZA EL CABECERO
            strSqlSeg += "SET FIAPIDPASO = " + PasoSiguiente + ", ";
            strSqlSeg += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
            strSqlSeg += "WHERE FIAPIDCOMP = " + IdCompra + " ";
            strSqlSeg += "AND FIAPSTATUS = 1";
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                dbCnx.SetQuery(strSqlSeg);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se actualizó el proceso de manera satisfactoria";
                respuesta.Objeto = null;
            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar el estado de la cuenta";
                respuesta.Objeto = null;
                return respuesta;
            }
            if (respuesta.Ok.Equals("SI"))
            {
                switch (PasoSiguiente)
                {
                    #region paso siguiente
                    case 1:
                        nuevoPaso = "<p> Se actualizo el paso a <strong>Apártalo</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 2:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Carga tus datos</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 3:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Liquida</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 4:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Asegura</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 5:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Emplaca</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 6:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Programa tu entrega</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 7:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Recibe tu auto</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 8:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Factura disponible</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 9:
                        nuevoPaso = "<p>Se actualizo el paso a <strong>Recibe tu factura</strong></p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    default:
                        nuevoPaso = "";
                        break;
                        #endregion
                }
            }
            return respuesta;
        }


        public void enviaCorreo(long IdCompra, string nuevoPaso) {
            string subject = "FURIA SEAT";
                HiloEnvioCorreoSoporte hiloEnvioCorreo = new HiloEnvioCorreoSoporte(subject, "jrperez@grupoautofin.com",nuevoPaso, IdCompra.ToString());
                Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreo.EnvioCorreoSoporte));
                hilo.Start();

        }


        // PUT api/OrdenCompra/GetActualizaCheckOrdenCompra

        [Route("api/OrdenCompra/GetActualizaCheckOrdenCompra", Name = "GetActualizaCheckOrdenCompra")]
        public async Task<Respuesta> GetActualizaCheckOrdenCompra(int IdCompra, int IdCheck)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            BPedido _updateEstado = new BPedido();
            Respuesta respuesta = new Respuesta();
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                string strSqlSeg = "";
                strSqlSeg += "UPDATE " + constantes.Ambiente + "APPS.APDCKLST ";
                strSqlSeg += "SET FIAPREALIZ = 1" + ", ";
                strSqlSeg += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                strSqlSeg += "WHERE FIAPIDCOMP = " + IdCompra + " ";
                strSqlSeg += "AND FIAPIDPCKL = " + IdCheck + " ";
                strSqlSeg += "AND FIAPSTATUS = 1";
                dbCnx.SetQuery(strSqlSeg);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se actualizó el proceso de manera satisfactoria";
                respuesta.Objeto = null;

                #region Actualiza estado
                switch (IdCheck)
                {
                    case 2:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 2);
                        break;
                    case 3:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 3);
                        break;
                    case 5:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 4);
                        break;
                    case 6:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 5);
                        break;
                    case 8:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 6);
                        break;
                    case 9:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 7);
                        break;
                    case 11:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 8);
                        break;
                    case 12:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 9);
                        break;
                    case 13:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 10);
                        break;
                    case 15:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 11);
                        break;
                    case 16:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 12);
                        break;
                    case 18:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 13);
                        break;
                    case 19:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 14);
                        break;
                    case 20:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 16);
                        break;
                    case 23:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 17);
                        break;
                    case 24:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 18);
                        break;
                    case 25:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 19);
                        break;
                    case 27:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 20);
                        break;
                    case 29:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 21);
                        break;
                    case 30:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 22);
                        break;
                    case 31:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 23);
                        break;
                    case 33:
                        await _updateEstado.UpdateEstadoCompraSmart(IdCompra, 24);
                        break;
                    default:
                        break;
                }
                #endregion

            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar el proceso";
                respuesta.Objeto = null;
                return respuesta;
            }

            if (respuesta.Ok.Equals("SI"))
            {
                string nuevoPaso = "";
                switch (IdCheck)
                {
                    #region check
                    case 2:
                        nuevoPaso = "<p><strong>EC: </strong> Carga del apartado</p><p><strong>PP: </strong>Apártalo</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 3:
                        nuevoPaso = "<p><strong>EC: </strong> Apartado validado</p><p><strong>PP: </strong>Apártalo</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 5:
                        nuevoPaso = "<p><strong>EC: </strong> Pedido</p><p><strong>PP: </strong>Carga tus datos</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 6:
                        nuevoPaso = "<p><strong>EC: </strong> Factura generada</p><p><strong>PP: </strong>Carga tus datos</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 8:
                        nuevoPaso = "<p><strong>EC: </strong> Carga de liquidación</p><p><strong>PP: </strong>Liquida</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 9:
                        nuevoPaso = "<p><strong>EC: </strong> Total validado</p><p><strong>PP: </strong>Liquida</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 11:
                        nuevoPaso = "<p><strong>EC: </strong> Carga de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 12:
                        nuevoPaso = "<p><strong>EC: </strong> Solicitud de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 13:
                        nuevoPaso = "<p><strong>EC: </strong> Cotización de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 15:
                        nuevoPaso = "<p><strong>EC: </strong> Pago de póliza</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 16:
                        nuevoPaso = "<p><strong>EC: </strong> Póliza emitida</p><p><strong>PP: </strong>Asegura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 18:
                        nuevoPaso = "<p><strong>EC: </strong> Placas si</p><p><strong>PP: </strong>Emplaca</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 19:
                        nuevoPaso = "<p><strong>EC: </strong> Placas no</p><p><strong>PP: </strong>Emplaca</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 20:
                        nuevoPaso = "<p><strong>EC: </strong> Llegada</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 23:
                        nuevoPaso = "<p><strong>EC: </strong> Cita agendada</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 24:
                        nuevoPaso = "<p><strong>EC: </strong> Cita fuera de zona</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 25:
                        nuevoPaso = "<p><strong>EC: </strong> Auto listo para entrega</p><p><strong>PP: </strong>Programa entrega</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 27:
                        nuevoPaso = "<p><strong>EC: </strong> Auto recibido</p><p><strong>PP: </strong>Recibe tu auto</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 29:
                        nuevoPaso = "<p><strong>EC: </strong> Cita factura agendada</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 30:
                        nuevoPaso = "<p><strong>EC: </strong> Factura fuera de zona</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 31:
                        nuevoPaso = "<p><strong>EC: </strong> Factura lista para entrega</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 33:
                        nuevoPaso = "<p><strong>EC: </strong> Factura recibida</p><p><strong>PP: </strong>Programa factura</p>";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    case 34:
                        nuevoPaso = "Prueba solicitada.";
                        enviaCorreo(IdCompra, nuevoPaso);
                        break;
                    default:
                        nuevoPaso = "";
                        break;
                        #endregion
                }
            }
            return respuesta;
        }


        //GET api/Usuario/GetCheckListOrden
        [Route("api/OrdenCompra/GetCheckListOrden", Name = "GetCheckListOrden")]
        public List<Checklist> GetCheckListOrden(int IdCompra)
        {
            List<Checklist> CheckList = new List<Checklist>();
            Checklist Check;
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
         
            string Query = "";
            Query = "SELECT FIAPIDCOMP, FIAPIDPROC, FIAPIDPCKL, FSAPDESCCK, FIAPSMARTI, FIAPAPPVIS, FIAPSISTEM, FIAPREALIZ ";
            Query += "FROM    " + constantes.Ambiente + "APPS.APDCKLST ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Check = new Checklist();
                    Check.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Check.IdCheck = dr["FIAPIDPCKL"].ToString().Trim();
                    Check.IdProceso = dr["FIAPIDPROC"].ToString().Trim();
                    Check.DescripcionCheck = dr["FSAPDESCCK"].ToString().Trim();
                    Check.SmartItControl = dr["FIAPSMARTI"].ToString().Trim();
                    Check.AppControl = dr["FIAPAPPVIS"].ToString().Trim();
                    Check.SistemaContol = dr["FIAPSISTEM"].ToString().Trim();
                    Check.Realizado = dr["FIAPREALIZ"].ToString().Trim();
                    CheckList.Add(Check);
                }
                    CheckList = CheckList.OrderBy(o => Convert.ToInt32(o.IdCheck)).ToList();
            }
            else
            {
                Check = new Checklist();
                Check.IdCompra = null;
                Check.IdCheck = null;
                Check.IdProceso = null;
                Check.DescripcionCheck = null;
                Check.SmartItControl = null;
                Check.AppControl = null;
                Check.SistemaContol = null;
                Check.Realizado = null;
                CheckList.Add(Check);
            }
            return CheckList;
        }


        //  GET api/Usuario/GetObtieneTipoDocumentos

        [Route("api/OrdenCompra/GetObtieneTipoDocumentos", Name = "GetObtieneTipoDocumentos")]
        public TipoDocumento GetObtieneTipoDocumentos()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();



            TipoDocumento documento = new TipoDocumento();

            ClasificacionDocumento documentoClasificado;
            List<ClasificacionDocumento> coleccionDocumentosPersonaFisica = new List<ClasificacionDocumento>();
            List<ClasificacionDocumento> coleccionDocumentosPersonaMoral = new List<ClasificacionDocumento>();

            string strSql = "";
            strSql += "SELECT  FIAPIDTPPR, FSAPTIPPER, FIAPIDTIPO, FSAPDOCUME  FROM " + constantes.Ambiente + "APPS.APCTPDST "; // TABLA DE TIPO DOCUMENTO
            strSql += "WHERE FIAPSTATUS = 1  AND FIAPIDTIPO not in (98,99) ";


            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {

                        if (dr["FIAPIDTPPR"].ToString().Trim() == "1")
                        {

                            documentoClasificado = new ClasificacionDocumento();
                            documentoClasificado.IdTipoPersona = dr["FIAPIDTPPR"].ToString().Trim();
                            documentoClasificado.TipoPersona = dr["FSAPTIPPER"].ToString().Trim();
                            documentoClasificado.IdTipoDocumento = dr["FIAPIDTIPO"].ToString().Trim();
                            documentoClasificado.Documento = dr["FSAPDOCUME"].ToString().Trim();

                            coleccionDocumentosPersonaFisica.Add(documentoClasificado);

                        }
                        else
                        {

                            documentoClasificado = new ClasificacionDocumento();
                            documentoClasificado.IdTipoPersona = dr["FIAPIDTPPR"].ToString().Trim();
                            documentoClasificado.TipoPersona = dr["FSAPTIPPER"].ToString().Trim();
                            documentoClasificado.IdTipoDocumento = dr["FIAPIDTIPO"].ToString().Trim();
                            documentoClasificado.Documento = dr["FSAPDOCUME"].ToString().Trim();
                            coleccionDocumentosPersonaMoral.Add(documentoClasificado);


                        }
                    }


                }
                else
                {
                    documento.PersonaFisica = null;
                    documento.PersonaMoral = null;

                }

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                documento.PersonaFisica = coleccionDocumentosPersonaFisica;
                documento.PersonaMoral = coleccionDocumentosPersonaMoral;


            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();


            }

            return documento;



        }


        // GET api/OrdenCompra/GetObtenerFactura
        [Route("api/OrdenCompra/GetObtenerFactura", Name = "GetObtenerFactura")]
        public Respuesta GetObtenerFactura(long IdCompra)

        {

            DVADB.DB2 dbCnx = new DVADB.DB2();

            Respuesta respuesta = new Respuesta();

            respuesta.Ok = "NO";
            respuesta.Mensaje = "Factura no encontrada, intentar nuevamente.";

            try

            {

                string strSql = "";

                strSql += "SELECT factu.FICAIDCIAU, factu.FICAIDCLIE, factu.FCCAPREIN, factu.FICAFOLIN FROM PRODAPPS.APEPANST pedan ";
                strSql += "INNER JOIN PRODCAJA.CAEFACAN facan ON pedan.FIAPIDCIAU = facan.FICAIDCIAU AND pedan.FIAPIDPEDI = facan.FICAIDPEDI ";
                strSql += "INNER JOIN PRODCAJA.CAEFACTU factu ON facan.FICAIDCIAU = factu.FICAIDCIAU AND facan.FICAIDFACT = factu.FICAIDFACT ";
                strSql += "WHERE pedan.FIAPSTATUS = 1  AND pedan.FIAPIDCOMP = " + IdCompra.ToString();

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)

                {

                    string idCia = "";
                    string idCliente = "";
                    string serie = "";
                    string folio = "";

                    foreach (DataRow dr in dt.Rows)

                    {

                        idCia = dr["FICAIDCIAU"].ToString().Trim();
                        idCliente = dr["FICAIDCLIE"].ToString().Trim();
                        serie = dr["FCCAPREIN"].ToString().Trim();
                        folio = dr["FICAFOLIN"].ToString().Trim();

                    }

                    DVAModelsReflection.DB2Database _db = new DVAModelsReflection.DB2Database();
                    List<String> documentoFiscal = new List<string>();
                    DVAFacturacion.FacturaCFD factura = new DVAFacturacion.FacturaCFD(_db, int.Parse(idCia), int.Parse(idCliente));
                    documentoFiscal = (factura.GetDocumentoFiscal(serie, int.Parse(folio)));


                    if (documentoFiscal.Count > 0)

                    {
                        string respuestaDocumentoFiscal = documentoFiscal[0];

                        var webClient = new WebClient();

                        byte[] binarydata = webClient.DownloadData(respuestaDocumentoFiscal);

                        string facturaBase64 = System.Convert.ToBase64String(binarydata, 0, binarydata.Length);

                        byte[] facturaBytes = Convert.FromBase64String(facturaBase64);

                        Stream facturaStream = new MemoryStream(facturaBytes);

                        string directorioFactura = @"C:\inetpub\wwwroot\wsApiSeat\Resources\Facturas\";

                        string facturaNombre = serie + "-" + folio + ".pdf";

                        File.WriteAllBytes(directorioFactura + facturaNombre,facturaBytes);

                        string rutaFactura = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Facturas/" + facturaNombre;

                        respuesta.Ok = "SI";

                        respuesta.Mensaje = rutaFactura;

                    }

                    else

                    {
                        throw new Exception();
                    }

                }

            }

            catch (Exception ex)

            {
                respuesta.Mensaje = "Factura no encontrada, intentar nuevamente.";
            }

            return respuesta;

        }


        // GET api/OrdenCompra/GetObtenerCotizadorPlacasTenencia
        [Route("api/OrdenCompra/GetObtenerCotizadorPlacasTenencia", Name = "GetObtenerCotizadorPlacasTenencia")]
        public List<CotizadorPlacasTenencia> GetObtenerCotizadorPlacasTenencia(long IdCompra)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();

            List<CotizadorPlacasTenencia> lstCotizador = new List<CotizadorPlacasTenencia>();
            CotizadorPlacasTenencia cotizador = new CotizadorPlacasTenencia();

            try

            {
                string strSql = "";

                strSql += "SELECT factu.FSCAIDEDO, factu.FFCAFECHA, factu.FDCASUBTOT, factu.FICAIDCIAU, factu.FICAIDCLIE, factu.FCCAPREIN, factu.FICAFOLIN FROM PRODAPPS.APEPANST pedan ";
                strSql += "INNER JOIN PRODCAJA.CAEFACAN facan ON pedan.FIAPIDCIAU = facan.FICAIDCIAU AND pedan.FIAPIDPEDI = facan.FICAIDPEDI ";
                strSql += "INNER JOIN PRODCAJA.CAEFACTU factu ON facan.FICAIDCIAU = factu.FICAIDCIAU AND facan.FICAIDFACT = factu.FICAIDFACT ";
                strSql += "WHERE pedan.FIAPSTATUS = 1  AND pedan.FIAPIDCOMP = " + IdCompra.ToString();

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];


                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {

                        cotizador = new CotizadorPlacasTenencia();

                        decimal porcentajeMensual = 0;
                        string IdEstado = "";


                        DateTime fechaFactura = Convert.ToDateTime(dr["FFCAFECHA"].ToString().Trim());

                        IdEstado = dr["FSCAIDEDO"].ToString().Trim();

                        int mes = fechaFactura.Month;
                                                
                        #region porcentajeMensual

                        switch (mes)
                        {
                            #region Mes
                            //enero
                            case 1:
                                porcentajeMensual = 1;
                                break;

                            //febrero
                            case 2:
                                porcentajeMensual = 0.92M;
                                break;


                            //marzo
                            case 3:
                                porcentajeMensual = 0.83M;
                                break;

                            //abril
                            case 4:
                                porcentajeMensual = 0.75M;
                                break;

                            //mayo
                            case 5:
                                porcentajeMensual = 0.67M;
                                break;

                            //junio
                            case 6:
                                porcentajeMensual = 0.58M;
                                break;

                            //julio
                            case 7:
                                porcentajeMensual = 0.50M;
                                break;

                            //agosto
                            case 8:
                                porcentajeMensual = 0.42M;
                                break;

                            //septiembre
                            case 9:
                                porcentajeMensual = 0.33M;
                                break;

                            //octubre
                            case 10:
                                porcentajeMensual = 0.25M;
                                break;

                            //noviembre
                            case 11:
                                porcentajeMensual = 0.17M;
                                break;

                            //diciembre
                            case 12:
                                porcentajeMensual = 0.08M;
                                break;


                            default:
                                porcentajeMensual = 0;
                                break;
                                #endregion

                        }

                        #endregion


                        #region Calculo de placas y tenencia

                        switch (IdEstado)
                        {

                            case "09":

                                #region PrecioCDMX

                                lstCotizador = new List<CotizadorPlacasTenencia>();
                                cotizador = new CotizadorPlacasTenencia();

                                cotizador.IdTramite = "1";
                                cotizador.DescripcionTramite = "Trámite en CDMX";
                                cotizador.PrecioPlacas = "2780";
                                cotizador.PorcentajeMensual = porcentajeMensual.ToString().Trim();

                                cotizador.PrecioFacturaSinIva = dr["FDCASUBTOT"].ToString().Trim();
                                decimal resCDM = Convert.ToDecimal(cotizador.PrecioFacturaSinIva) * 0.03M;
                                cotizador.PrecioTenencia = Convert.ToDecimal((resCDM * porcentajeMensual)).ToString();
                                cotizador.Total = (Convert.ToDecimal(cotizador.PrecioPlacas) + Convert.ToDecimal(cotizador.PrecioTenencia)).ToString();

                                lstCotizador.Add(cotizador);

                                #endregion
                                
                                break;

                            case "15":

                                #region PrecioEDOMEX

                                lstCotizador = new List<CotizadorPlacasTenencia>();
                                cotizador = new CotizadorPlacasTenencia();

                                cotizador.IdTramite = "2";
                                cotizador.DescripcionTramite = "Trámite en el estado de México";
                                cotizador.PrecioPlacas = "4600";
                                cotizador.PorcentajeMensual = porcentajeMensual.ToString().Trim();
                                cotizador.PrecioFacturaSinIva = dr["FDCASUBTOT"].ToString().Trim();

                                decimal resEM = Convert.ToDecimal(cotizador.PrecioFacturaSinIva) * 0.03M;

                                cotizador.PrecioTenencia = Convert.ToDecimal((resEM * porcentajeMensual)).ToString();

                                cotizador.Total = (Convert.ToDecimal(cotizador.PrecioPlacas) + Convert.ToDecimal(cotizador.PrecioTenencia)).ToString();

                                lstCotizador.Add(cotizador);

                                #endregion
                                break;


                            case "17":

                                #region EDOMorelos
                                lstCotizador = new List<CotizadorPlacasTenencia>();
                                cotizador = new CotizadorPlacasTenencia();

                                cotizador.IdTramite = "3";
                                cotizador.DescripcionTramite = "Trámite en el estado de Morelos";
                                cotizador.PrecioPlacas = "4600";
                                cotizador.PorcentajeMensual = porcentajeMensual.ToString().Trim();
                                cotizador.PrecioFacturaSinIva = dr["FDCASUBTOT"].ToString().Trim();

                                decimal resEMo = Convert.ToDecimal(cotizador.PrecioFacturaSinIva) * 0.03M;

                                cotizador.PrecioTenencia = Convert.ToDecimal((resEMo * porcentajeMensual)).ToString();

                                cotizador.Total = (Convert.ToDecimal(cotizador.PrecioPlacas) + Convert.ToDecimal(cotizador.PrecioTenencia)).ToString();

                                lstCotizador.Add(cotizador);

                                #endregion


                                break;

                            default:

                                lstCotizador = new List<CotizadorPlacasTenencia>();
                               
                                break;

                        }

                        #endregion

                        
                    }
                }
                else
                {
                    lstCotizador = new List<CotizadorPlacasTenencia>();
                }
            }
            catch (Exception ex)

            {
                lstCotizador = new List<CotizadorPlacasTenencia>();
                return lstCotizador;
            }

            return lstCotizador;

        }


        [Route("api/OrdenCompra/GetFiltroDeCitas", Name = "GetFiltroDeCitas")]
        public HorariosCitas GetFiltroDeCitas(string TipoCita, string aIdAgencia)
        {
            #region VARIABLES
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            List<Ubicaciones> _ubicaciones = new List<Ubicaciones>();
            List<Ubicaciones> ListaUbicaciones = new List<Ubicaciones>();
            List<CitasExistentes> Citas = new List<CitasExistentes>();
            List<CitaApp> CitasPorOrden = new List<CitaApp>();
            CitaApp Cita;
            List<CitaApp> CitasExistentes = new List<CitaApp>();
            TimeSpan HoraPasadas48Horas = new TimeSpan();
            DateTime FechaLimite = new DateTime();
            string FechaInicial = "";
            string FechaFinal = "";
            List<string> PeriodoDeFechas = new List<string>();
            List<string> Horarios = new List<string>();
            Rangos Horario = new Rangos();
            HorariosCitas Objeto = new HorariosCitas();
            List<Intervalos> ListaDeIntervalos = new List<Intervalos>();
            List<Intervalos> HorariosCitasSeguroFactura = new List<Intervalos>();
            List<Rangos> HorariosFinales = new List<Rangos>();
            List<Fechas> Fechas = new List<Models.OrdenCompra.Fechas>();
            List<TimeSpan> Horas = new List<TimeSpan>();
            #endregion
            #region LLENA CITAS EXISTENTES
            DateTime hoy = DateTime.Today;
            DateTime x = hoy.AddHours(48);
            HoraPasadas48Horas = x.TimeOfDay;
            FechaLimite = x.AddDays(10).Date;

            FechaInicial = x.ToString("yyyy-MM-dd");
            FechaFinal = FechaLimite.ToString("yyyy-MM-dd");

            string Query = "";
            Query = "SELECT FIAPIDTCIT, FIAPIDCOMP, FIAPIDTURN, FIAPDESCCI, FFAPFECHA, FHAPHORINI, FHAPHORFIN, FSAPUBICAC ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDDTCST ";
            Query += "WHERE FFAPFECHA >= " + "'" + FechaInicial + "'" + " AND FFAPFECHA <= " + "'" + FechaFinal + "'" + " AND FIAPSTATUS = 1";


            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Cita = new CitaApp();
                    Cita.DescripcionCita = dr["FIAPDESCCI"].ToString().Trim();
                    Cita.Fecha = dr["FFAPFECHA"].ToString().Trim();
                    Cita.HoraFinal = dr["FHAPHORFIN"].ToString().Trim();
                    Cita.HoraInicial = dr["FHAPHORINI"].ToString().Trim();
                    Cita.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Cita.IdTipoCita = dr["FIAPIDTCIT"].ToString().Trim();
                    Cita.IdTurno = dr["FIAPIDTURN"].ToString().Trim();
                    Cita.Ubicacion = dr["FSAPUBICAC"].ToString().Trim();
                    CitasExistentes.Add(Cita);
                }
            }
            else
            {
                Cita = new CitaApp();
                Cita.DescripcionCita = null;
                Cita.Fecha = null;
                Cita.HoraFinal = null;
                Cita.HoraInicial = null;
                Cita.IdCompra = null;
                Cita.IdTipoCita = null;
                Cita.Ubicacion = null;
                Cita.IdTurno = null;
                CitasExistentes.Add(Cita);
            }


            foreach (CitaApp cita in CitasExistentes)
            {

                string HoraInicia = cita.HoraInicial;
                string HoraFinal = cita.HoraFinal;

                if (HoraInicia != null)
                {
                    if (HoraInicia != "")
                        HoraInicia = HoraInicia.Substring(10, HoraInicia.Length - 19);

                }
                if (HoraFinal != null)
                {
                    if (HoraFinal != "")
                        HoraFinal = HoraFinal.Substring(10, HoraFinal.Length - 19);
                }


                string IntervaloCadena = "";
                if (HoraFinal != null)
                {
                    if (HoraFinal != "")
                    {
                        IntervaloCadena = HoraInicia + "-" + HoraFinal;

                    }
                    else
                    {
                        DateTime xs = Convert.ToDateTime(cita.HoraInicial);
                        IntervaloCadena = xs.ToString("HH:ss");
                    }

                }



                CitasExistentes nc = new CitasExistentes();

                if (cita.Fecha != null)
                {
                    nc.Fecha = cita.Fecha.Remove(cita.Fecha.Length - 14);
                    nc.Intervalo = IntervaloCadena;
                    nc.TipoCita = cita.IdTipoCita;
                    nc.Turno = cita.IdTurno;
                }


                Citas.Add(nc);
            }

            #endregion
            #region LLENA HORARIOS
            while (x <= FechaLimite)
            {
                Fechas fecha = new Fechas();
                string Fecha = x.ToString("dd/MM/yyyy");

                fecha.Fecha = Fecha;
                fecha.Dia = x.ToString("dddd", new CultureInfo("es-ES")).ToUpper();
                fecha.Horarios = null;
                fecha.Intervalos = new List<Intervalos>();
                Fechas.Add(fecha);

                x = x.AddDays(1);
            }


            string Query2 = "";
            Query2 = "SELECT FIAPDESCDI ,FIAPIDTURN , FHAPHORINI, FHAPHORFIN ";
            Query2 += "FROM	" + constantes.Ambiente + "APPS.APCHORST ";
            Query2 += "WHERE FIAPSTATUS = 1";


            DataTable DTRangos = dbCnx.GetDataSet(Query2).Tables[0];
            if (DTRangos.Rows.Count > 0)
            {
                foreach (DataRow dr in DTRangos.Rows)
                {
                    DateTime HoraInicial = Convert.ToDateTime(dr["FHAPHORINI"].ToString().Trim());
                    DateTime HoraFinald = Convert.ToDateTime(dr["FHAPHORFIN"].ToString().Trim());
                    string HoraFinal = HoraFinald.ToString("HH:mm:ss");
                    string HoraInicia = HoraInicial.ToString("HH:mm:ss");
                    string IntervaloCadena = HoraInicia + "-" + HoraFinal;


                    Intervalos Intervalo = new Intervalos();
                    Intervalo.Dia = dr["FIAPDESCDI"].ToString().Trim();
                    Intervalo.IdTurno = dr["FIAPIDTURN"].ToString().Trim();
                    Intervalo.Intervalo = IntervaloCadena;
                    ListaDeIntervalos.Add(Intervalo);
                }

            }
            else
            {
                Intervalos Intervalo = new Intervalos();
                Intervalo.Dia = null;
                Intervalo.IdTurno = null;
                Intervalo.Intervalo = null;
                ListaDeIntervalos.Add(Intervalo);

            }
            #endregion
            #region Obtiene Horarios
            HorariosCitasSeguroFactura = ListaDeIntervalos.FindAll(o => o.IdTurno == "0");
            #endregion
            #region Realiza filtro Horarios 
            foreach (Intervalos inter in ListaDeIntervalos)
            {
                foreach (Fechas f in Fechas)
                {
                    if (f.Dia == inter.Dia)
                    {
                        f.Intervalos.Add(inter);
                    }
                }

            }


            foreach (Fechas F in Fechas)
            {
                List<Intervalos> IntervaloFinal = new List<Intervalos>();

                foreach (Intervalos i in F.Intervalos)
                {
                    CitasExistentes ce = new CitasExistentes();

                    if (Citas[0].Fecha != null)
                    {
                        ce = Citas.Find(o => o.Fecha.Trim() == F.Fecha);
                    }

                    if (ce != null)
                    {
                        List<Intervalos> NoAgendados = new List<Intervalos>();
                        NoAgendados = F.Intervalos.FindAll(O => O.IdTurno != ce.Turno && O.IdTurno != "0");
                        List<int> aIds = new List<int>();
                        F.Intervalos = NoAgendados;
                        break;

                    }

                }
            }


            #endregion
            #region LLENA UBICACIONES
            string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Ubicacion.json");
            //string strJSON = File.ReadAllText(@"C:\Users\jcuevas\Documents\wsApiSeat\ResourcesJson\Ubicacion.json");
            _ubicaciones = JsonConvert.DeserializeObject<List<Ubicaciones>>(strJSON);
            ListaUbicaciones = _ubicaciones.FindAll(o => o.IdAgencia == aIdAgencia).ToList();
            #endregion
            #region LLENA OBJETO
            Objeto.TipoDeCita = TipoCita;
            Objeto.Ubicaciones = ListaUbicaciones;
            Objeto.Fechas = Fechas;
            #region Realiza filtro de tipo de cita 
            if (TipoCita != "1")
            {
                #region Llena Horarios 
                foreach (Fechas r in Objeto.Fechas)
                {
                    DateTime Fecha = Convert.ToDateTime(r.Fecha);

                    if (r.Dia == "SÁBADO")
                    {
                        string HoraInicial = "09:00";
                        string HoraFinal = "14:00";

                        DateTime Inicio = Convert.ToDateTime(HoraInicial);
                        DateTime Final = Convert.ToDateTime(HoraFinal);
                        List<DateTime> HorasSabado = new List<DateTime>();
                        List<Horarios> HorariosSabado = new List<Models.OrdenCompra.Horarios>();

                        while (Inicio <= Final)
                        {
                            HorasSabado.Add(Inicio);
                            Inicio = Inicio.AddHours(1);
                        }

                        foreach (DateTime w in HorasSabado)
                        {
                            Horarios hr = new Horarios();
                            string DiaName = Fecha.ToString("dddd", new CultureInfo("es-ES"));
                            string Hora = w.ToString("HH:mm:ss");

                            hr.Dia = DiaName.ToUpper();
                            hr.Hora = Hora;
                            HorariosSabado.Add(hr);
                        }

                        r.Horarios = HorariosSabado;
                    }
                    else
                    {

                        string HoraInicial = "09:00";
                        string HoraFinal = "18:00";

                        DateTime Inicio = Convert.ToDateTime(HoraInicial);
                        DateTime Final = Convert.ToDateTime(HoraFinal);
                        List<DateTime> HorasSemana = new List<DateTime>();
                        List<Horarios> HorariosSemana = new List<Models.OrdenCompra.Horarios>();

                        while (Inicio <= Final)
                        {
                            HorasSemana.Add(Inicio);
                            Inicio = Inicio.AddHours(1);
                        }

                        foreach (DateTime w in HorasSemana)
                        {
                            Horarios hr = new Horarios();
                            string DiaName = Fecha.ToString("dddd", new CultureInfo("es-ES"));
                            string Hora = w.ToString("HH:mm:ss");

                            hr.Dia = DiaName.ToUpper();
                            hr.Hora = Hora;
                            HorariosSemana.Add(hr);
                        }

                        r.Horarios = HorariosSemana;

                    }
                }

                #endregion

                foreach (Fechas Fech in Objeto.Fechas)
                {
                    Fech.Intervalos = null;
                }

            }
            else
            {
                #region Elimina intervalos para el caso de citas tipo 2 o 3
                foreach (Fechas F in Fechas)
                {
                    foreach (Intervalos I in F.Intervalos)
                    {
                        F.Intervalos = F.Intervalos.FindAll(O => O.IdTurno != "0");
                    }
                }
                #endregion

            }
            #endregion

            #region Elimina Horarios no disponibles
            if (TipoCita != "1")
            {
                Objeto.Fechas = Objeto.Fechas.FindAll(o => o.Dia != "DOMINGO");


                foreach (CitasExistentes C in Citas)
                {

                    if (C.Fecha != null)
                    {
                        string Fecha = C.Fecha.Trim();
                        string Hora = C.Intervalo.Trim();
                        List<Horarios> HorariosDisponibles = new List<Horarios>();

                        foreach (Fechas fecha in Objeto.Fechas)
                        {
                            HorariosDisponibles = new List<Models.OrdenCompra.Horarios>();
                            if (fecha.Fecha == Fecha)
                            {
                                foreach (Horarios hora in fecha.Horarios)
                                {
                                    string Hora2 = hora.Hora.Substring(0, hora.Hora.Length - 3);

                                    if (Hora2 != Hora)
                                    {
                                        HorariosDisponibles.Add(hora);
                                    }

                                }
                            }
                            else
                            {
                                HorariosDisponibles = fecha.Horarios;
                            }

                            fecha.Horarios = HorariosDisponibles;
                        }
                    }
                }
            }

            #endregion

            #endregion
            return Objeto;
        }

        [Route("api/OrdenCompra/PostRegistraCitas", Name = "PostRegistraCitas")]
        public Respuesta PostRegistraCitas([FromBody] CitaAppGenerica cita)
        {

            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            List<CatalogoCitas> lstCatCitas = new List<CatalogoCitas>();
            CatalogoCitas citas = new CatalogoCitas();

            string Pasoproceso = string.Empty;
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();


                string strCatCitas = "";

                strCatCitas += "SELECT FIAPIDTCIT, FSAPDESCIT FROM \t";
                strCatCitas += "PRODAPPS.APCTPCST \t";
                strCatCitas += "WHERE FIAPSTATUS = 1";

                DataTable dtCat = dbCnx.GetDataSet(strCatCitas).Tables[0];

                if (dtCat.Rows.Count > 0)
                {

                    foreach (DataRow dr in dtCat.Rows)
                    {
                        citas = new CatalogoCitas();

                        citas.IdTipoCita = dr["FIAPIDTCIT"].ToString().Trim();
                        citas.DescripcionCita = dr["FSAPDESCIT"].ToString().Trim();

                        lstCatCitas.Add(citas);
                    }
                }

                
                string descripcion = "";
                descripcion = (lstCatCitas.Find(x => Convert.ToInt32(x.IdTipoCita) == Convert.ToInt32(cita.IdTipoCita)).DescripcionCita);



                string strCita = "";

                strCita += "INSERT INTO PRODAPPS.APDDTCST \t";
                strCita += "( ";
                strCita += "FIAPIDCONS, FIAPIDTCIT, FIAPIDTURN,";
                strCita += "FIAPIDCOMP, FIAPIDHORA, FIAPDESCCI,";
                strCita += "FFAPFECHA,  FHAPHORINI, FHAPHORFIN,";
                strCita += "FSAPUBICAC, FIAPSTATUS, USERCREAT,";
                strCita += "DATECREAT,TIMECREAT, PROGCREAT)";
                strCita += "VALUES (";
                strCita += "(SELECT COALESCE(MAX(FIAPIDCONS),0) + 1 FROM prodapps.APDDTCST)," + cita.IdTipoCita.ToString().Trim() + "," + cita.IdTurnoMatVes.ToString().Trim();
                strCita += "," + cita.IdCompra.ToString().Trim() + "," + "0" + "," + "'" + descripcion.ToString().Trim() + "'";
                strCita += "," + "'" + cita.Fecha + "'" + "," + "'" + cita.HoraInicial + "'" + "," + (string.IsNullOrEmpty(cita.HoraFinal) ? "Default" : "'" + cita.HoraFinal.ToString().Trim() + "'");
                strCita += "," + "'" + cita.Ubicacion.ToString().Trim().ToUpper() + "'" + "," + "1" + "," + "'APP'";
                strCita += "," + "CURRENT DATE, CURRENT TIME, 'APP')";

                dbCnx.SetQuery(strCita);

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos registrados exitosamente";
                respuesta.Objeto = null;

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();


                // HugoBoss

                // SE ENVIA CORREO A GERENTES Y CLIENTE

                if (respuesta.Ok.Equals("SI"))
                {

                    string compraCliente = string.Empty;

                    compraCliente += "SELECT FIAPIDCUEN FROM ";
                    compraCliente += "PRODAPPS.APECMPST ";
                    compraCliente += "WHERE FIAPIDCOMP = " + cita.IdCompra.ToString().Trim();

                    DataTable cuen = dbCnx.GetDataSet(compraCliente).Tables[0];

                    string idCuenta = "";

                    if (cuen.Rows.Count > 0)
                    {

                        foreach (DataRow dr in cuen.Rows)
                        {

                            idCuenta = dr["FIAPIDCUEN"].ToString().Trim();

                        }
                    }


                    string strSqlCu = "";

                    strSqlCu = "SELECT  *  ";
                    strSqlCu += "FROM	PRODAPPS.APCCTAST ";
                    strSqlCu += "WHERE FIAPSTATUS = 1 ";
                    strSqlCu += "AND FIAPIDCUEN = " + idCuenta;

                    DataTable dtCu = dbCnx.GetDataSet(strSqlCu).Tables[0];

                    string correoCliente = string.Empty;


                    if (dtCu.Rows.Count > 0)
                    {
                        correoCliente = dtCu.Rows[0]["FSAPCORREO"].ToString().Trim();
                    }



                    ////////////////////////////////////////////////

                    if (!string.IsNullOrEmpty(correoCliente))
                    {                        
                        string strHtml = "";

                        try
                        {
                            
                            HttpRequest request = HttpContext.Current.Request;

                            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";                         

                            string rutaTipoCita = string.Empty;


                            switch (Convert.ToInt32(cita.IdTipoCita))
                            {

                                case 1:
                                    rutaTipoCita = baseUrl + "Resources/Emailing/TestDrive/index.html";
                                    break;

                                case 2:
                                    rutaTipoCita = baseUrl + "Resources/Emailing/EntregaAuto/index.html";
                                    Pasoproceso = "<strong>PP: </strong> Programa entrega";
                                    break;

                                case 3:
                                    rutaTipoCita = baseUrl + "Resources/Emailing/EntregaFactura/index.html";
                                    Pasoproceso = "<strong>PP: </strong> Programa factura";
                                    break;

                                default:
                                    break;

                            }

                            Uri uri = new Uri(rutaTipoCita);
                            HttpWebRequest requestweb = (HttpWebRequest)HttpWebRequest.Create(uri);
                            requestweb.Method = WebRequestMethods.Http.Get;
                            HttpWebResponse response = (HttpWebResponse)requestweb.GetResponse();
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            string output = reader.ReadToEnd();
                            response.Close();
                            strHtml = output;


                            string dia = string.Empty;                            
                            string mes = string.Empty;
                            string anio = string.Empty;
                            string fecha = string.Empty;
                                                
                            mes = cita.Fecha.Substring(5,2).ToString().Trim();

                            switch (mes) {

                                case "01":
                                    mes = "enero";
                                   break;
                                
                                case "02":
                                    mes = "febrero";
                                    break;

                                case "03":
                                    mes = "marzo";
                                    break;

                                case "04":
                                    mes = "abril";
                                    break;

                                case "05":
                                    mes = "mayo";
                                    break;

                                case "06":
                                    mes = "junio";
                                    break;

                                case "07":
                                    mes = "julio";
                                    break;

                                case "08":
                                    mes = "agosto";
                                    break;

                                case "09":
                                    mes = "septiembre";
                                    break;

                                case "10":
                                    mes = "octubre";
                                    break;

                                case "11":
                                    mes = "noviembre";
                                    break;

                                case "12":
                                    mes = "diciembre";
                                    break;

                            }
                                                        

                            dia = cita.Fecha.Substring(8, 2).ToString().Trim();
                            anio = cita.Fecha.Substring(0, 4).ToString().Trim();


                            fecha = dia + " de " + mes + " de " + anio;


                            strHtml = strHtml.Replace("[{Ubicacion}]", cita.Ubicacion.ToString().Trim().ToUpper());                         
                            strHtml = strHtml.Replace("[{Fecha}]", fecha);
                            strHtml = strHtml.Replace("[{Hora}]", cita.HoraInicial.ToString().Trim());

                        }
                        catch (Exception ex)
                        {

                        }
                        


                        string consulta = string.Empty;
                        int idAgencia;

                        string strSql = "select FIAPIDCIAU from prodapps.APEPANST where FIAPIDCOMP = " + cita.IdCompra.ToString().Trim();
                        DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            idAgencia = Convert.ToInt32(dt.Rows[0]["FIAPIDCIAU"].ToString().Trim());
                            string subjectCliente = "SEAT";
                            EnvioCorreoCliente hiloEnvioCorreoCliente = new EnvioCorreoCliente(subjectCliente, correoCliente, "Se agendó la cita " + descripcion + " con exito", strHtml, idAgencia);
                            Thread hiloCliente = new Thread(new ThreadStart(hiloEnvioCorreoCliente.EnviarCorreoCliente));
                            hiloCliente.Start();
                        }
                        else
                        {
                            throw new Exception();
                        }
                        
                    }

                }
            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar la cita";
                respuesta.Objeto = null;

                return respuesta;

            }

            return respuesta;

        }

      

        //POST api/OrdenCompra/PostEncuesta
        [Route("api/OrdenCompra/PostEncuesta", Name = "PostEncuesta")]
        public Respuesta PostEncuesta(string IdCompra, [FromBody] Encuesta Encuesta)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new
            DVAConstants.Constants();

            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "";

            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string JsonEncuesta = JsonConvert.SerializeObject(Encuesta);

                string strInsCabComp = "";
                strInsCabComp += "INSERT INTO " + constantes.Ambiente + "APPS.APDENCST ";
                strInsCabComp += "(FIAPIDCOMP, FIAPIDENCU, FSAPENCUES, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT)";
                strInsCabComp += "VALUES ";
                strInsCabComp += "(";
                strInsCabComp += IdCompra + ", ";
                strInsCabComp += "1, ";
                strInsCabComp += "'" + JsonEncuesta + "'" + ", ";
                strInsCabComp += "1" + ",";
                strInsCabComp += "1, 'APPS' ,CURRENT DATE, CURRENT TIME, 'APPS'";
                strInsCabComp += ")";


                dbCnx.SetQuery(strInsCabComp);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();


                respuesta.Objeto = JsonEncuesta;
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Envio exitoso.";

                return respuesta;

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "Fallo el envio.";
            }

            return respuesta;

        }


        //POST api/OrdenCompra/PostDatosGestoria
        [Route("api/OrdenCompra/PostRegistraDatosGestoria", Name = "PostRegistraDatosGestoria")]
        public Respuesta PostRegistraDatosGestoria([FromBody] DatosGestoria datosGestoria)
        {

            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string strG = "";

                strG += "INSERT INTO PRODAPPS.APDDTGST (";
                strG += "FIAPIDCOMP, FIAPIDTRSE, FIAPSELECT, FDAPVALORC,";
                strG += "FDAPPORCEN, FDAPTOTAL, FIAPSTATUS,";
                strG += "USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                strG += ") VALUES ( ";
                strG += (string.IsNullOrEmpty(datosGestoria.IdCompra) ? "Default" : datosGestoria.IdCompra.ToString().Trim()) + "," + (string.IsNullOrEmpty(datosGestoria.IdTramiteSeleccionado) ? "Default" : datosGestoria.IdTramiteSeleccionado.ToString().Trim()) + "," + (string.IsNullOrEmpty(datosGestoria.ElegioTramite) ? "Default" : datosGestoria.ElegioTramite.ToString().Trim()) + "," +"0.03";
                strG += "," + (string.IsNullOrEmpty(datosGestoria.PorcentajeMensual) ? "Default" : datosGestoria.PorcentajeMensual.ToString().Trim()) + "," + (string.IsNullOrEmpty(datosGestoria.Total) ? "Default" : datosGestoria.Total.ToString().Trim()) + "," + "1";
                strG += "," + "'APP', CURRENT DATE, CURRENT TIME, 'APP')";

                if (Convert.ToInt32(datosGestoria.ElegioTramite) == 0)
                {

                    string strSqlSeg = "";
                    strSqlSeg += "UPDATE PRODAPPS.APDCKLST ";
                    strSqlSeg += "SET FIAPREALIZ = 1 " + ", ";
                    strSqlSeg += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                    strSqlSeg += "WHERE FIAPIDCOMP = " + datosGestoria.IdCompra.ToString() + " ";
                    strSqlSeg += "AND FIAPIDPCKL = 12 ";
                    strSqlSeg += "AND FIAPSTATUS = 1";

                    dbCnx.SetQuery(strSqlSeg);

                }

                dbCnx.SetQuery(strG);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje  = "Datos registrados correctamente";
                respuesta.Objeto = null;

            }
            catch (Exception ex) {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo registrar los datos.";
                respuesta.Objeto = null;

                return respuesta;
            }

            return respuesta;
        }

        // POST api/OrdenCompra/PostRegistraSeguro
        [Route("api/OrdenCompra/PostRegistraSeguro", Name = "PostRegistraSeguro")]
        public Respuesta PostRegistraSeguro(string IdCompra, [FromBody] Seguro ObjSeguro)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new
            DVAConstants.Constants();
            Respuesta respuesta = new Respuesta();

            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string strInsCabComp = "";
                strInsCabComp += "INSERT INTO " + constantes.Ambiente + "APPS.APDDTSST ";
                strInsCabComp += "(FIAPIDCOMP, FIAPSELECT, FIAPIDASEG, FSAPDESASE, FSAPCOBERT, FFAPVIGINI, FFAPVIGFIN, FDAPPRIMAT, FDAPCOMISE, FDAPTOTAL, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT)";
                strInsCabComp += "VALUES ";
                strInsCabComp += "(";
                strInsCabComp += IdCompra + ", ";
                strInsCabComp += "'" + ObjSeguro.Seleccionado + "'" + ", ";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.IdAseguradora) ? "Default" : ObjSeguro.IdAseguradora.Trim().ToUpper()) + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.Aseguradora.ToUpper()) ? "Default" : "'" + ObjSeguro.Aseguradora.Trim().ToUpper() + "'") + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.Cobertura.ToUpper()) ? "Default" : "'" + ObjSeguro.Cobertura.Trim().ToUpper() + "'") + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.FechaInicial.ToUpper()) ? "Default" : "'" + ObjSeguro.FechaInicial.Trim().ToUpper() + "'") + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.FechaFinal.ToUpper()) ? "Default" : "'" + ObjSeguro.FechaFinal.Trim().ToUpper() + "'") + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.PrimaTotal.ToUpper()) ? "Default" : "'" + ObjSeguro.PrimaTotal.Trim().ToUpper() + "'") + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.Comision.ToUpper()) ? "Default" : "'" + ObjSeguro.Comision.Trim().ToUpper() + "'") + ",";
                strInsCabComp += (string.IsNullOrEmpty(ObjSeguro.TotalSeguro.ToUpper()) ? "Default" : "'" + ObjSeguro.TotalSeguro.Trim().ToUpper() + "'") + ",";
                strInsCabComp += "1, 'APP' ,CURRENT DATE, CURRENT TIME, 'APP'";
                strInsCabComp += ")";

                dbCnx.SetQuery(strInsCabComp);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Objeto = "";
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro de seguro exitoso.";                

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "Falló el registro del seguro.";
                respuesta.Objeto = null;

                return respuesta;
            }

            return respuesta;
        }



        [Route("api/OrdenCompra/GetObtieneComprobantesPago", Name = "GetObtieneComprobantesPago")]
        public List<Documento> GetObtieneComprobantesPago(int IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string Query = "";
            Query = "SELECT FIAPIDCONS, FIAPIDCOMP, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDCMPST ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

            Documento Comprobante;
            List<Documento> ComprobantesDePago = new List<Documento>();

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Comprobante = new Documento();
                    Comprobante.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Comprobante.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                    Comprobante.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                    Comprobante.RutaDocumento = dr["FSAPRUTDOC"].ToString().Trim(); //.Replace('\\', '/').Replace("C:/inetpub/wwwroot/wsApiSeat/Compras/ComprobantePagos/", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/ComprobantePagos/");
                    ComprobantesDePago.Add(Comprobante);
                }
            }
            else
            {
                Comprobante = new Documento();
                Comprobante.IdCompra = null;
                Comprobante.IdTipo = null;
                Comprobante.NombreDocumento = null;
                Comprobante.RutaDocumento = null;
                ComprobantesDePago.Add(Comprobante);

            }
            return ComprobantesDePago;
        }



        //GET api/OrdenCompra/GetDatosGestoria
        [Route("api/OrdenCompra/GetDatosGestoria", Name = "GetDatosGestoria")]
        public DatosGestoria GetDatosGestoria(int IdCompra)
        {
            DatosGestoria Gestoria = new DatosGestoria();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string Query = "";
            Query = "SELECT FIAPIDCOMP, FIAPIDTRSE, FIAPSELECT, FDAPVALORC, FDAPPORCEN, FDAPTOTAL ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDDTGST ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";
            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Gestoria.PorcentajeMensual = dr["FDAPPORCEN"].ToString().Trim();
                    Gestoria.ElegioTramite = dr["FIAPSELECT"].ToString().Trim();
                    Gestoria.Total = dr["FDAPTOTAL"].ToString().Trim();
                    Gestoria.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Gestoria.IdTramiteSeleccionado = dr["FIAPIDTRSE"].ToString().Trim();
                }
            }
            else
            {
                Gestoria.PorcentajeMensual = null;
                Gestoria.ElegioTramite = null;
                Gestoria.Total = null; 
            }
            return Gestoria;
        }


        //GET api/OrdenCompra/GetDatosSeguro
        [Route("api/OrdenCompra/GetDatosSeguro", Name = "GetDatosSeguro")]
        public Seguro GetDatosSeguro(int IdCompra)
        {
            Seguro Seguro = new Seguro();
            DVADB.DB2 dbCnx = new DVADB.DB2();

            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string Query = "";
            Query = "SELECT FIAPIDCOMP, FIAPSELECT, FIAPIDASEG, FSAPDESASE, FSAPCOBERT, FFAPVIGINI, FFAPVIGFIN, FDAPPRIMAT, FDAPCOMISE, FDAPTOTAL ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDDTSST ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Seguro.Aseguradora = dr["FSAPDESASE"].ToString().Trim();
                    Seguro.Cobertura = dr["FSAPCOBERT"].ToString().Trim();
                    Seguro.Comision = dr["FDAPCOMISE"].ToString().Trim();
                    Seguro.FechaFinal = dr["FFAPVIGFIN"].ToString().Trim();
                    Seguro.FechaInicial = dr["FFAPVIGINI"].ToString().Trim();
                    Seguro.PrimaTotal = dr["FDAPPRIMAT"].ToString().Trim();
                    Seguro.Seleccionado = dr["FIAPSELECT"].ToString().Trim();
                    Seguro.TotalSeguro = dr["FDAPTOTAL"].ToString().Trim();
                }
            }
            else
            {
                Seguro.Aseguradora = null;
                Seguro.Cobertura = null;
                Seguro.Comision = null;
                Seguro.FechaFinal = null;
                Seguro.FechaInicial = null;
                Seguro.PrimaTotal = null;
                Seguro.Seleccionado = null;
                Seguro.TotalSeguro = null;

            }
            return Seguro;
        }

        //GET api/OrdenCompra/GetCitas
        [Route("api/OrdenCompra/GetCitas", Name = "GetCitas")]
        public List<CitaApp> GetCitas(int IdCompra)
        {
            List<CitaApp> CitasPorOrden = new List<CitaApp>();
            CitaApp Cita;

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();


            string Query = "";
            Query = "SELECT FIAPIDTCIT, FIAPIDCOMP, FIAPDESCCI, FFAPFECHA, FHAPHORINI, FHAPHORFIN, FSAPUBICAC ";
            Query += "FROM    " + constantes.Ambiente + "APPS.APDDTCST ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Cita = new CitaApp();
                    Cita.DescripcionCita = dr["FIAPDESCCI"].ToString().Trim();
                    Cita.Fecha = dr["FFAPFECHA"].ToString().Trim();
                    Cita.HoraFinal = dr["FHAPHORFIN"].ToString().Trim();
                    Cita.HoraInicial = dr["FHAPHORINI"].ToString().Trim();
                    Cita.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Cita.IdTipoCita = dr["FIAPIDTCIT"].ToString().Trim();
                    Cita.Ubicacion = dr["FSAPUBICAC"].ToString().Trim();
                    CitasPorOrden.Add(Cita);
                }
            }
            else
            {
                Cita = new CitaApp();
                Cita.DescripcionCita = null;
                Cita.Fecha = null;
                Cita.HoraFinal = null;
                Cita.HoraInicial = null;
                Cita.IdCompra = null;
                Cita.IdTipoCita = null;
                Cita.Ubicacion = null;

                CitasPorOrden.Add(Cita);
            }
            
            foreach (CitaApp cita in CitasPorOrden)
            {
                DateTime Inicial = new DateTime();
                DateTime Final = new DateTime();
                
                if (cita.HoraInicial != "" && cita.HoraInicial != null)
                {
                    Inicial = Convert.ToDateTime(cita.HoraInicial);
                    cita.HoraInicial = Inicial.ToString("dd/MM/yyyy HH:mm:ss");
                }


                if (cita.HoraFinal != "" && cita.HoraFinal != null)
                {
                    Final = Convert.ToDateTime(cita.HoraFinal);
                    cita.HoraFinal = Final.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
            return CitasPorOrden;
        }

        //POST api/OrdenCompra/PostActualizaDatosCuenta
        [Route("api/OrdenCompra/PostActualizaDatosCuenta", Name = "PostActualizaDatosCuenta")]
        public RespuestaTest<Respuesta> PostActualizaDatosCuenta([FromBody] Cuenta datosPerfil)
        {
            RespuestaTest<Respuesta> respuesta = new RespuestaTest<Respuesta>();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            dbCnx.AbrirConexion();
            dbCnx.BeginTransaccion();
            try
            {
                string strSqlCuenta = "";

                strSqlCuenta += "UPDATE PRODAPPS.APCCTAST ";
                strSqlCuenta += "SET FSAPNOMBRE = " + "'" + datosPerfil.Nombre.ToString().Trim().ToUpper() + "'" + ",";
                strSqlCuenta += "FSAPAPEPAT = " + "'" + datosPerfil.ApellidoPaterno.ToString().Trim().ToUpper() + "'" + ",";
                strSqlCuenta += "FSAPAPEMAT = " + "'" + datosPerfil.ApellidoMaterno.ToString().Trim().ToUpper() + "'" + ",";
                strSqlCuenta += "FIAPNUMMOV = " + datosPerfil.TelefonoMovil.ToString().Trim() + ", ";
                strSqlCuenta += "USERUPDAT = 'APP' , DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT= 'APP' ";                
                strSqlCuenta += "WHERE FIAPIDCUEN = " + datosPerfil.IdCuenta + " ";
                strSqlCuenta += "AND lower(TRIM(FSAPCORREO)) =" + "'" +datosPerfil.Correo.ToString().Trim().ToLower() + "'";
                dbCnx.SetQuery(strSqlCuenta);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos actualizados correctamente.";
                respuesta.Objeto = null;
            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                respuesta = new RespuestaTest<Respuesta>();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar la información";
                respuesta.Objeto = null;
                return respuesta;
            }
       
            return respuesta;
        }


        //GET api/OrdenCompra/GetEncuesta
        [Route("api/OrdenCompra/GetEncuesta", Name = "GetEncuesta")]
        public EncuestaOrden GetEncuesta(int IdCompra)
        {
            EncuestaOrden Encuesta = new EncuestaOrden();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            try
            {
                string Query = "";
                Query = "SELECT FIAPIDCOMP, FIAPIDENCU, FSAPENCUES ";
                Query += "FROM    " + constantes.Ambiente + "APPS.APDENCST ";
                Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

                DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DT.Rows)
                    {
                        Encuesta = new EncuestaOrden();

                        Encuesta.IdEncuesta = dr["FIAPIDENCU"].ToString().Trim();
                        Encuesta.Encuesta = dr["FSAPENCUES"].ToString().Trim();
                    }
                }
                else
                {
                    Encuesta = new EncuestaOrden();

                    Encuesta.IdEncuesta = "0";
                    Encuesta.Encuesta = "No se encontró encuesta para la compra seleccionada.";
                }
            }
            catch (Exception ex) {


                Encuesta = new EncuestaOrden();

                Encuesta.IdEncuesta = "0";
                Encuesta.Encuesta = "Ocurrió un error al buscar la encuesta.";

                return Encuesta;

            }
            return Encuesta;
        }


        //GET api/OrdenCompra/GetCatalogoProcesoCompra
        [Route("api/OrdenCompra/GetCatalogoProcesoCompra", Name = "GetCatalogoProcesoCompra")]
        public List<ProcesoCompra> GetCatalogoProcesoCompra() {

            DVADB.DB2 dbCnx = new DVADB.DB2();

            List<ProcesoCompra> lstElementoProceso = new List<ProcesoCompra>();
            ProcesoCompra elementoProceso = new ProcesoCompra();


            try {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string srtCons = "";

                srtCons += "select FIAPIDPASO, FSAPDESCRI  from ";
                srtCons += "prodapps.APCPRCST ";
                srtCons += "WHERE FIAPIDPROC = 2 ";
                srtCons += "AND FIAPSTATUS = 1 ";
                srtCons += "AND FIAPIDPASO <> 10 ";


                DataTable dt = dbCnx.GetDataSet(srtCons).Tables[0];

                if (dt.Rows.Count > 0)

                {

                    foreach (DataRow dr in dt.Rows)
                    {

                        elementoProceso = new ProcesoCompra();

                        elementoProceso.Idpaso = dr["FIAPIDPASO"].ToString().Trim();
                        elementoProceso.Descripcion = dr["FSAPDESCRI"].ToString().Trim();

                        lstElementoProceso.Add(elementoProceso);

                    }

                    lstElementoProceso = lstElementoProceso.OrderBy(s => Convert.ToUInt32(s.Idpaso)).ToList();

                }
                else {

                    elementoProceso = new ProcesoCompra();
                    lstElementoProceso.Add(elementoProceso);
                }

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

            }
            catch (Exception ex) {


                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                elementoProceso = new ProcesoCompra();
                lstElementoProceso.Add(elementoProceso);

                return lstElementoProceso;
            }
            
                        
            return lstElementoProceso;

        }


        //POST api/OrdenCompra/PutCancelaOrdenCompra
        [Route("api/OrdenCompra/GetCancelaOrdenCompra", Name = "GetCancelaOrdenCompra")]
        public Respuesta GetCancelaOrdenCompra(long IdCompra)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                
                string strSql = "";

                strSql += "SELECT  pedan.FIAPIDINVE, pedan.FIAPIDPEDI, factu.FSCAIDEDO, factu.FFCAFECHA, factu.FDCASUBTOT, factu.FICAIDCIAU, ";
                strSql += "factu.FICAIDCLIE, factu.FCCAPREIN, factu.FICAFOLIN FROM PRODAPPS.APEPANST pedan ";
                strSql += "INNER JOIN PRODCAJA.CAEFACAN facan ON pedan.FIAPIDCIAU = facan.FICAIDCIAU AND pedan.FIAPIDPEDI = facan.FICAIDPEDI ";
                strSql += "INNER JOIN PRODCAJA.CAEFACTU factu ON facan.FICAIDCIAU = factu.FICAIDCIAU AND facan.FICAIDFACT = factu.FICAIDFACT ";
                strSql += "WHERE pedan.FIAPSTATUS = 1  AND pedan.FIAPIDCOMP = " + IdCompra.ToString();

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
                
                if (dt.Rows.Count > 0)
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se puede cancelar la orden de compra porque está facturada.";
                    respuesta.Objeto = "";
                }
                else
                {

                    string consultaPed = "";
                    consultaPed += "SELECT FIAPIDINVE, FIAPIDPEDI, FIAPIDCIAU ";
                    consultaPed += "FROM PRODAPPS.APEPANST ";
                    consultaPed += "WHERE FIAPSTATUS = 1 ";
                    consultaPed += "AND FIAPIDCOMP = " + IdCompra.ToString();
                    
                    DataTable dtPed = dbCnx.GetDataSet(consultaPed).Tables[0];


                    if (dtPed.Rows.Count > 0)
                    {

                        foreach (DataRow dr in dtPed.Rows)
                        {

                            string IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                            string IdPedido = dr["FIAPIDPEDI"].ToString().Trim();
                            string IdInventario = dr["FIAPIDINVE"].ToString().Trim();

                            strSql = "";
                            strSql = "UPDATE PRODAPPS.APECMPST SET FIAPIDESTA = 15, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP'   WHERE FIAPIDCOMP = " + IdCompra;
                            dbCnx.SetQuery(strSql);

                            if (Convert.ToUInt64(IdPedido) > 0)
                            {

                                strSql = "";
                                strSql += "UPDATE PRODAUT.ANEPEDAU SET FIANPASTP = 4, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FNANPAAGE = " + IdAgencia + " AND FNANPAIDE = " + IdPedido;
                                dbCnx.SetQuery(strSql);
                            }

                            strSql = "";
                            strSql += "UPDATE PRODAUT.ANCAUTOM SET FNAUTOEST = 10, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FNAUTOEST IN (11, 50) AND FNAUTOAGE = " + IdAgencia + " AND FIANIDINVE = " + IdInventario;
                            dbCnx.SetQuery(strSql);

                            strSql = ""; // se inserta el detalle
                            strSql += "INSERT INTO PRODAPPS.APDSGCST ";
                            strSql += "(FIAPIDCOMP, FIAPIDSEGU, FSAPTITSEG, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT)";
                            strSql += "VALUES ";
                            strSql += "(";
                            strSql += IdCompra + ", ";
                            strSql += "(SELECT coalesce(MAX(FIAPIDSEGU),0)+1 ID FROM PRODAPPS.APDSGCST WHERE FIAPIDCOMP = " + IdCompra + "),";
                            strSql += "'Movimiento generado en Servicios' ,";
                            strSql += "15,";
                            strSql += "1, 'APPS' ,CURRENT DATE, CURRENT TIME, 'APP'";
                            strSql += ")";
                            dbCnx.SetQuery(strSql);
                        }

                        respuesta.Ok = "SI";
                        respuesta.Mensaje = "Se canceló de forma satisfactoria la orden de compra.";
                        respuesta.Objeto = "";
                    }
                    else {

                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No se encontró la orden de compra.";
                        respuesta.Objeto = "";
                    }    
                }

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                
            }
            catch (Exception ex) {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo cancelar la orden de compra";
                respuesta.Objeto = "";   
            }
            return respuesta;
           
        }


        //  GET api/OrdenCompra/GetObtieneHistorialMovimientosConAgencia
        [Route("api/OrdenCompra/GetObtieneHistorialMovimientosConAgencia", Name = "GetObtieneHistorialMovimientosConAgencia")]
        public List<HistorialMovimientosContactoAgencia> GetObtieneHistorialMovimientosConAgencia(string IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            List<HistorialMovimientosContactoAgencia> Historial = new List<HistorialMovimientosContactoAgencia>();
            HistorialMovimientosContactoAgencia Reg = new HistorialMovimientosContactoAgencia();

            string strSql = "";
            strSql += "SELECT  FIAPIDCIAU, FSAPDESCCK, FFAPFECHA, FHAPHORA, FSAPDESEST, FSAPNOTIFI  FROM " + constantes.Ambiente + "APPS.APDHSMST "; // TABLA DE TIPO DOCUMENTO
            strSql += "WHERE FIAPSTATUS = 1 AND FIAPIDCOMP = " + IdCompra;

            try
            {



                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        Reg = new HistorialMovimientosContactoAgencia();
                        Reg.Agencia = dr["FIAPIDCIAU"].ToString().Trim();
                        Reg.Estado = dr["FSAPDESEST"].ToString().Trim();
                        Reg.Fecha = dr["FFAPFECHA"].ToString().Trim();
                        Reg.Hora = dr["FHAPHORA"].ToString().Trim();
                        Reg.Mensaje = dr["FSAPNOTIFI"].ToString().Trim();
                        Reg.Paso = dr["FSAPDESCCK"].ToString().Trim();

                        Historial.Add(Reg);
                    }
                }
                else
                {
                    Reg = new HistorialMovimientosContactoAgencia();
                    Reg.Agencia = null;
                    Reg.Estado = null;
                    Reg.Fecha = null;
                    Reg.Hora = null;
                    Reg.Mensaje = null;
                    Reg.Paso = null;
                    Historial.Add(Reg);

                }

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                
                return Historial;

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
            }

            return Historial;
        }
        
    }
}

