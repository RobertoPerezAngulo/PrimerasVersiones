using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using wsApiSeat.Models;
using wsApiSeat.Models.OrdenCompra;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace wsApiSeat.Controllers
{
    public class MedioPagoController : ApiController
    {

        // GET api/MedioPagoController/GetObtenerOpcionesDePagoProc2
        [Route("api/MedioPago/GetObtenerOpcionesDePago", Name = "Obtiene los medios de pago")]
        public List<OpcionesPago> GetObtenerOpcionesDePago()
        {
            List<OpcionesPago> lstOpcionPago = new List<OpcionesPago>();
            try
            {
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\Resources\OpcionesPago.json");
                lstOpcionPago = JsonConvert.DeserializeObject<List<OpcionesPago>>(strJSON);
            }
            catch (Exception _exc)
            {
                lstOpcionPago = null;
            }
            return lstOpcionPago;
        }

        // GET api/MedioPagoController/GetObtenerCuentasBancariasPorAgencia
        [Route("api/MedioPago/GetObtenerCuentasBancariasPorAgencia", Name = "Obtiene la configuracion de cuentas bancarias por agencia")]
        public CuentasBancariasPorAgencia GetObtenerCuentasBancariasPorAgencia(int IdAgencia)
        {
            CuentasBancariasPorAgencia lstCuentasBancarias = new CuentasBancariasPorAgencia();
            string strJSON = string.Empty;
            try
            {
                switch (IdAgencia)
                {
                    case 12: //monterrey 
                        strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\Resources\CuentasBancariasMonterrey.json");
                        lstCuentasBancarias = JsonConvert.DeserializeObject<CuentasBancariasPorAgencia>(strJSON);                    
                        break;
                    case 35: // acapulco
                        strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\Resources\CuentasBancariasAcapulco.json");                      
                        lstCuentasBancarias = JsonConvert.DeserializeObject<CuentasBancariasPorAgencia>(strJSON);
                        break;
                    case 36: //cdmx
                        strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\Resources\CuentasBancariasCdmx.json");
                        lstCuentasBancarias = JsonConvert.DeserializeObject<CuentasBancariasPorAgencia>(strJSON);
                        break;
                    default:
                        lstCuentasBancarias = new CuentasBancariasPorAgencia() ;
                        break;
               }
            }
            catch (Exception _exc)
            {
                lstCuentasBancarias = null;
            }
            return lstCuentasBancarias;
        }

        // GET api/MedioPago/GetObtenerCotizadorPlacasTenencia
        [Route("api/MedioPago/GetObtenerReferenciaDePago", Name = "GetObtenerReferenciaDePago")]
        public List<ReferenciaBancaria> GetObtenerReferenciaDePago(long IdCompra)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();

            List<ReferenciaBancaria> lstReferencias = new List<ReferenciaBancaria>();
            ReferenciaBancaria referencia = new ReferenciaBancaria();

            List<Banco> lstBancos = new List<Banco>();
            Banco banco = new Banco();

            List<ConceptoPago> lstConceptoPago = new List<ConceptoPago>();
            ConceptoPago concepto = new ConceptoPago();


            string RazonSocial = "";
            string NombreCompletoCliente = "";
            string nombre = "";
            string ApellidoPaterno = "";
            string ApellidoMaterno = "";


            try
            {

                string strNombreCli = "";
                strNombreCli += "SELECT FSAPRAZSOC, FSAPNMBFIS, FSAPAPTFIS, FSAPAMTFIS  FROM \t";
                strNombreCli += "PRODAPPS.APDDTFST \t";
                strNombreCli += "WHERE FIAPIDCOMP =" + IdCompra + "\t";
                strNombreCli += "AND FIAPSTATUS = 1";

                DataTable dtNC = dbCnx.GetDataSet(strNombreCli).Tables[0];

                if (dtNC.Rows.Count > 0)
                {
                    foreach (DataRow drNC in dtNC.Rows)
                    {

                        RazonSocial = drNC["FSAPRAZSOC"].ToString().Trim();

                        if (RazonSocial != "" && RazonSocial != null)
                        {
                            NombreCompletoCliente = RazonSocial;
                        }
                        else
                        {

                            nombre = drNC["FSAPNMBFIS"].ToString().Trim();
                            ApellidoPaterno = drNC["FSAPAPTFIS"].ToString().Trim();
                            ApellidoMaterno = drNC["FSAPAMTFIS"].ToString().Trim();

                            NombreCompletoCliente = nombre + " " + ApellidoPaterno + " " + ApellidoMaterno;

                        }
                    }
                }


                int idAgencia = 0;
                string strAgenc = "";

                strAgenc = "select FIAPIDCIAU, trim(FSAPNUMSER) NumeroSerie, FDAPTOTAL Total, FIAPIDINVE from prodapps.APEPANST where FIAPIDCOMP = " + IdCompra;
                DataTable dt = dbCnx.GetDataSet(strAgenc).Tables[0];

                if (dt.Rows.Count > 0)
                {

                    idAgencia = Convert.ToInt32(dt.Rows[0]["FIAPIDCIAU"].ToString().Trim());
                }


                string strConce = "";
                strConce += "select FICCIDCOPA, FSCCDESCOP  from \t";
                strConce += "PRODCXC.CCCCFMLC \t";
                strConce += "where ficcidagen = " + idAgencia + " ";
                strConce += "and FICCIDCOPA IN(38, 39) \t";
                strConce += "and ficcidmodu = 1";

                DataTable dtC = dbCnx.GetDataSet(strConce).Tables[0];

                concepto = new ConceptoPago();
                concepto.IdConceptoPago = "0";
                concepto.DescripcionConceptoPago = "PEDIDO";

                lstConceptoPago.Add(concepto);


                if (dtC.Rows.Count > 0)
                {
                    foreach (DataRow drC in dtC.Rows)
                    {
                        concepto = new ConceptoPago();

                        concepto.IdConceptoPago = drC["FICCIDCOPA"].ToString().Trim();
                        concepto.DescripcionConceptoPago = drC["FSCCDESCOP"].ToString().Trim();

                        lstConceptoPago.Add(concepto);

                    }
                }


                string strBanco = "";
                strBanco += "SELECT FSCCNMCORT, FSCCCONVEN, FSCCCLABE \t";
                strBanco += "FROM PRODCXC.CCCCFBAN \t";
                strBanco += "where ficcidagen = " + idAgencia + " ";
                strBanco += "AND FICCSTATUS = 1";


                DataTable dtB = dbCnx.GetDataSet(strBanco).Tables[0];

                if (dtB.Rows.Count > 0)
                {

                    foreach (DataRow drB in dtB.Rows)
                    {
                        banco = new Banco();

                        banco.Nombre = drB["FSCCNMCORT"].ToString().Trim();
                        banco.Convenio = drB["FSCCCONVEN"].ToString().Trim();
                        banco.ClabeInterbancaria = drB["FSCCCLABE"].ToString().Trim();

                        lstBancos.Add(banco);

                    }
                }

                string IdPedido = "";
                string IdPersona = "";

                string strPed = "";
                strPed += "select FIAPIDCOMP, FIAPIDCIAU, FIAPIDPEDI, FIAPIDPERS  from \t";
                strPed += "prodapps.APEPANST \t";
                strPed += "WHERE FIAPIDCOMP =" + IdCompra + " \t";
                strPed += "AND FIAPSTATUS = 1";

                DataTable dtPed = dbCnx.GetDataSet(strPed).Tables[0];

                if (dtPed.Rows.Count > 0)
                {
                    foreach (DataRow drPed in dtPed.Rows)
                    {

                        IdPedido = drPed["FIAPIDPEDI"].ToString().Trim();
                        IdPersona = drPed["FIAPIDPERS"].ToString().Trim();

                        break;
                        
                    }

                }


                string strRb = "";

                CuentasBancariasPorAgencia listRazonSocial = new CuentasBancariasPorAgencia();
                listRazonSocial = GetObtenerCuentasBancariasPorAgencia(idAgencia);

                strRb += "SELECT FICCIDPERS, FICCIDIDEN, FSCCLNCAPT, FICCIDCOPA  FROM PRODCXC.CCELNCAP \t";
                strRb += "where ficcidagen = " + idAgencia + " ";
                strRb += "and ficcidpers = " + IdPersona + "\t";
                strRb += "and ficcididen = " + IdPedido + "\t";
                strRb += "and ficcidcopa in (0,38,39) \t";
                strRb += "and ficcstatus = 1";

                DataTable dtR = dbCnx.GetDataSet(strRb).Tables[0];
                
                if (dtR.Rows.Count > 0)
                {
                    foreach (DataRow drR in dtR.Rows)
                    {
                        referencia = new ReferenciaBancaria();
                        referencia.RazonSocial = listRazonSocial.RazonSocial;
                        referencia.Ubicacion = listRazonSocial.Ubicacion;
                        referencia.IdReferencia = drR["FICCIDCOPA"].ToString().Trim();
                        referencia.Nombre = lstConceptoPago.Find(x => Convert.ToInt32(x.IdConceptoPago) == Convert.ToInt32(referencia.IdReferencia)).DescripcionConceptoPago;
                        referencia.NombreCliente = NombreCompletoCliente;
                        referencia.NumeroCliente = drR["FICCIDPERS"].ToString().Trim();
                        referencia.NumeroPedido = drR["FICCIDIDEN"].ToString().Trim();
                        referencia.LineaCaptura = drR["FSCCLNCAPT"].ToString().Trim();
                        referencia.Bancos = lstBancos;
                        lstReferencias.Add(referencia);
                    }
                }
            }
            catch (Exception ex)
            {
                lstReferencias = null;
                return lstReferencias;
            }
            return lstReferencias;
        }
    }
}
