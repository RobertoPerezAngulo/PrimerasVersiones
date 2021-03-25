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
    public class CitasController : ApiController
    {

        // POST api/Citas/ObtenerTiposCitasxIdAgencia
        [Route("api/Citas/ObtenerTiposCitasxIdAgencia", Name = "ObtenerTiposCitasxIdAgencia")]
        public List<TipoCitaServicio> ObtenerTiposCitasxIdAgencia(String IdAgencia)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string strSql = "";
            strSql = "SELECT    * ";
            strSql += "FROM	" + constantes.Ambiente + "SERV.SECGPCIT ";
            strSql += "WHERE FISESTATUS = 1 ";
            strSql += "AND FISEAPLAMA = 1 ";
            strSql += "AND FISEIDCIAU = " + IdAgencia;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];
            //DataView dtX = dt.DefaultView;
            //dtX.Sort = "FSGERAZSOC";
            //dt = dtX.ToTable();

            TipoCitaServicio tipoCita;
            List<TipoCitaServicio> coleccionTiposCitaServicio = new List<TipoCitaServicio>();
            foreach (DataRow dr in dt.Rows)
            {
                tipoCita = new TipoCitaServicio();
                tipoCita.IdAgencia = dr["FISEIDCIAU"].ToString().Trim();
                tipoCita.IdTipoCita = dr["FISEIDGCIT"].ToString().Trim();
                tipoCita.TipoCita = dr["FSSEDESGCI"].ToString().Trim();

                coleccionTiposCitaServicio.Add(tipoCita);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();

            return coleccionTiposCitaServicio;

            //Context.Response.ContentType = "application/json";
            //Context.Response.Output.Write(jss.Serialize(coleccionTiposCitaServicio))
        }

    }
}
