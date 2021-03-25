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

namespace wsApiSeat.Controllers
{
    public class PromocionesYPublicidadController : ApiController
    {
        // POST api/Usuario/ObtenerRefaccionesOrdenServicio
        [Route("api/PromocionesPublicidad/ObtenerPublicidadPorMarcas", Name = "ObtenerPublicidadPorMarcas")]
        public List<Publicidad> ObtenerPublicidadPorMarcas(string IdsMarcas)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";

            string idsMarcas = IdsMarcas.Trim();

            //try
            //{

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCPUBMB PROMO ";
            strSql += "WHERE PROMO.FIAPSTATUS = 1 ";
            strSql += "AND PROMO.FIAPIDMARC IN (" + idsMarcas + ") ";
            strSql += "AND (PROMO.FFAPINIPUB <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND PROMO.FFAPFINPUB >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";


            DataTable dtPublicidad = dbCnx.GetDataSet(strSql).Tables[0];

            Publicidad publicidad;
            List<Publicidad> coleccionPublicidad = new List<Publicidad>();

            foreach (DataRow dr in dtPublicidad.Rows)
            {
                publicidad = new Publicidad();
                publicidad.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                publicidad.IdPublicidad = dr["FIAPIDPUBL"].ToString().Trim();
                publicidad.NomPublicidad = dr["FSAPPUBLIC"].ToString().Trim();
                publicidad.DescPublicidad = dr["FSAPDESPUB"].ToString().Trim();
                publicidad.IniciaPublicidad = dr["FFAPINIPUB"].ToString().Trim();
                publicidad.TerminaPublicidad = dr["FFAPFINPUB"].ToString().Trim();
                publicidad.LinkPublicidad = dr["FSAPLINPUB"].ToString().Trim();                
                coleccionPublicidad.Add(publicidad);
            }

            return coleccionPublicidad;
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPublicidad));

            //}
            //catch (Exception e)
            //{
            //    respuesta = e.Message;
            //    Console.WriteLine(respuesta);
            //}
        }


        // POST api/PromocionesPublicidad/ObtenerRefaccionesOrdenServicio
        [Route("api/PromocionesPublicidad/ObtenerPublicidadPorAgencia", Name = "ObtenerPublicidadPorAgencia")]
        public List<Publicidad> ObtenerPublicidadPorAgencia(string[] IdsAgencias)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";

            string IdAgencia = IdsAgencias[0].Replace("[", "(").Replace("]", ")");

            //try
            //{

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCPUBMB PROMO ";
            strSql += "WHERE PROMO.FIAPSTATUS = 1 ";
            strSql += " AND PROMO.FIAPIDCIAU IN " + IdAgencia;


            DataTable dtPublicidad = dbCnx.GetDataSet(strSql).Tables[0];

            Publicidad publicidad;
            List<Publicidad> coleccionPublicidad = new List<Publicidad>();

            foreach (DataRow dr in dtPublicidad.Rows)
            {
                publicidad = new Publicidad();
                publicidad.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                publicidad.IdPublicidad = dr["FIAPIDPUBL"].ToString().Trim();
                publicidad.NomPublicidad = dr["FSAPPUBLIC"].ToString().Trim();
                publicidad.DescPublicidad = dr["FSAPDESPUB"].ToString().Trim();
                publicidad.IniciaPublicidad = dr["FFAPINIPUB"].ToString().Trim();
                publicidad.TerminaPublicidad = dr["FFAPFINPUB"].ToString().Trim();
                publicidad.LinkPublicidad = dr["FSAPLINPUB"].ToString().Trim();
                coleccionPublicidad.Add(publicidad);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPublicidad));

            return coleccionPublicidad;

            //}
            //catch (Exception e)
            //{
            //    respuesta = e.Message;
            //    Console.WriteLine(respuesta);
            //}
        }


        // POST api/PromocionesPublicidad/ObtenerRefaccionesOrdenServicio
        [Route("api/PromocionesPublicidad/ObtenerPublicidad", Name = "ObtenerPublicidad")]
        public List<Publicidad> ObtenerPublicidad()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";
            //try
            //{

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCPUBMB PROMO ";
            strSql += "WHERE PROMO.FIAPSTATUS = 1 ";
            strSql += "AND PROMO.FIAPRESTRI = 0 ";
            strSql += "AND (PROMO.FFAPINIPUB <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND PROMO.FFAPFINPUB >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";


            DataTable dtPublicidad = dbCnx.GetDataSet(strSql).Tables[0];

            Publicidad publicidad;
            List<Publicidad> coleccionPublicidad = new List<Publicidad>();

            foreach (DataRow dr in dtPublicidad.Rows)
            {
                publicidad = new Publicidad();
                publicidad.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                publicidad.IdPublicidad = dr["FIAPIDPUBL"].ToString().Trim();
                publicidad.NomPublicidad = dr["FSAPPUBLIC"].ToString().Trim();
                publicidad.DescPublicidad = dr["FSAPDESPUB"].ToString().Trim();
                publicidad.IniciaPublicidad = dr["FFAPINIPUB"].ToString().Trim();
                publicidad.TerminaPublicidad = dr["FFAPFINPUB"].ToString().Trim();
                publicidad.LinkPublicidad = dr["FSAPLINPUB"].ToString().Trim();
                coleccionPublicidad.Add(publicidad);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPublicidad));

            return coleccionPublicidad;

            //}
            //catch (Exception e)
            //{
            //    respuesta = e.Message;
            //    Console.WriteLine(respuesta);
            //}
        }


        // POST api/PromocionesPublicidad/ObtenerRefaccionesOrdenServicio
        [Route("api/PromocionesPublicidad/ObtenerPromocionesPorMarcas", Name = "ObtenerPromocionesPorMarcas")]
        public List<Promocion> ObtenerPromocionesPorMarcas(string IdsMarcas, string IdTipo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";

            string idsMarcas = IdsMarcas.Trim();

            //try
            //{

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCPROMB PROMO ";
            strSql += "WHERE PROMO.FIAPSTATUS = 1 ";
            strSql += "AND PROMO.FIAPIDTIPO = " + IdTipo + " ";
            strSql += "AND PROMO.FIAPIDMARC IN (" + idsMarcas + ") ";
            strSql += "AND (PROMO.FFAPINIPRO <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND PROMO.FFAPFINPRO >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";


            DataTable dtPromociones = dbCnx.GetDataSet(strSql).Tables[0];

            Promocion promocion;
            List<Promocion> coleccionPromociones = new List<Promocion>();

            foreach (DataRow dr in dtPromociones.Rows)
            {
                promocion = new Promocion();
                promocion.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                promocion.Id = dr["FIAPIDPROM"].ToString().Trim();
                promocion.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                promocion.Tipo = dr["FSAPTIPO"].ToString().Trim();
                promocion.NombrePromocion = dr["FSAPNOMPRO"].ToString().Trim();
                promocion.DescripcionPromocion = dr["FSAPDESPRO"].ToString().Trim();
                promocion.FechaInicio = DateTime.Parse(dr["FFAPINIPRO"].ToString().Trim()).ToString("dd-MM-yyyy");
                promocion.FechaFin = DateTime.Parse(dr["FFAPFINPRO"].ToString().Trim()).ToString("dd-MM-yyyy");
                promocion.RutaImagen = dr["FSAPLINPRO"].ToString().Trim();
                coleccionPromociones.Add(promocion);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPromociones));
            return coleccionPromociones;

            //}
            //catch (Exception e)
            //{
            //    respuesta = e.Message;
            //    Console.WriteLine(respuesta);
            //}
        }

        // POST api/PromocionesPublicidad/ObtenerRefaccionesOrdenServicio
        [Route("api/PromocionesPublicidad/ObtenerPromocionesPorAgencia", Name = "ObtenerPromocionesPorAgencia")]
        public List<Promocion> ObtenerPromocionesPorAgencia(string[] IdsAgencias, string IdTipo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";

            string IdAgencia = IdsAgencias[0].Replace("[", "(").Replace("]", ")");

            //try
            //{

            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCPROMB PROMO ";
            strSql += "WHERE PROMO.FIAPSTATUS = 1 ";
            strSql += "AND PROMO.FIAPIDTIPO = " + IdTipo + " ";
            strSql += "AND PROMO.FIAPIDCIAU IN " + IdAgencia + " ";
            strSql += "AND (PROMO.FFAPINIPRO <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND PROMO.FFAPFINPRO >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";


            DataTable dtPromociones = dbCnx.GetDataSet(strSql).Tables[0];

            Promocion promocion;
            List<Promocion> coleccionPromociones = new List<Promocion>();

            foreach (DataRow dr in dtPromociones.Rows)
            {
                promocion = new Promocion();
                promocion.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                promocion.Id = dr["FIAPIDPROM"].ToString().Trim();
                promocion.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                promocion.Tipo = dr["FSAPTIPO"].ToString().Trim();
                promocion.NombrePromocion = dr["FSAPNOMPRO"].ToString().Trim();
                promocion.DescripcionPromocion = dr["FSAPDESPRO"].ToString().Trim();
                promocion.FechaInicio = DateTime.Parse(dr["FFAPINIPRO"].ToString().Trim()).ToString("dd-MM-yyyy");
                promocion.FechaFin = DateTime.Parse(dr["FFAPFINPRO"].ToString().Trim()).ToString("dd-MM-yyyy");
                promocion.RutaImagen = dr["FSAPLINPRO"].ToString().Trim();
                coleccionPromociones.Add(promocion);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPromociones));

            return coleccionPromociones;

            //}
            //catch (Exception e)
            //{
            //    respuesta = e.Message;
            //    Console.WriteLine(respuesta);
            //}
        }

        // POST api/PromocionesPublicidad/ObtenerPromociones
        [Route("api/PromocionesPublicidad/ObtenerPromociones", Name = "ObtenerPromociones")]
        public List<Promocion> ObtenerPromociones(string IdTipo)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";

            //try{
            string strSql = "";
            strSql = "SELECT * ";
            strSql += "FROM	" + constantes.Ambiente + "APPS.APCPROMB PROMO ";
            strSql += "WHERE PROMO.FIAPSTATUS = 1 ";
            strSql += "AND PROMO.FIAPRESTRI = 0 ";
            strSql += "AND PROMO.FIAPIDTIPO = " + IdTipo + " ";
            strSql += "AND (PROMO.FFAPINIPRO <= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND PROMO.FFAPFINPRO >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "')";
            DataTable dtPromociones = dbCnx.GetDataSet(strSql).Tables[0];

            Promocion promocion;
            List<Promocion> coleccionPromociones = new List<Promocion>();

            foreach (DataRow dr in dtPromociones.Rows)
            {
                promocion = new Promocion();
                promocion.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                promocion.Id = dr["FIAPIDPROM"].ToString().Trim();
                promocion.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                promocion.Tipo = dr["FSAPTIPO"].ToString().Trim();
                promocion.NombrePromocion = dr["FSAPNOMPRO"].ToString().Trim();
                promocion.DescripcionPromocion = dr["FSAPDESPRO"].ToString().Trim();
                promocion.FechaInicio = DateTime.Parse(dr["FFAPINIPRO"].ToString().Trim()).ToString("dd-MM-yyyy");
                promocion.FechaFin = DateTime.Parse(dr["FFAPFINPRO"].ToString().Trim()).ToString("dd-MM-yyyy");
                promocion.RutaImagen = dr["FSAPLINPRO"].ToString().Trim();                
                coleccionPromociones.Add(promocion);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionPromociones));

            return coleccionPromociones;

            //}
            //catch (Exception e)
            //{
            //    respuesta = e.Message;
            //    Console.WriteLine(respuesta);
            //}
        }

    }
}
