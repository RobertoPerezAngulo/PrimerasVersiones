using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using RouteAttribute = System.Web.Http.RouteAttribute;
using wsApiSeat.Models;
using System.IO;
using Newtonsoft.Json;
using System.Data;

namespace wsApiSeat.Controllers
{
    public class FacturacionController : ApiController
    {

        // GET api/Facturacion/GetObtenerUsosCfdi
        [Route("api/Facturacion/GetObtenerUsosCfdi", Name = "Obtiene los usos cfdi")]
        public List<UsoCfdi> GetObtenerUsosCfdi()
        {

            List<UsoCfdi> lstUsos = new List<UsoCfdi>();
            
            try
            {
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\UsosCfdi.json");
                lstUsos = JsonConvert.DeserializeObject<List<UsoCfdi>>(strJSON);
            }
            catch (Exception _exc)
            {
                lstUsos = null;
            }

            return lstUsos;

        }


        // GET api/Facturacion/GetObtenerNacionalidad
        [Route("api/Facturacion/GetObtenerNacionalidad", Name = "Obtiene la nacionalidad")]
        public List<Nacionalidad> GetObtenerNacionalidad()
        {

            List<Nacionalidad> lstNacionalidad = new List<Nacionalidad>();

            try
            {
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Nacionalidad.json");
                lstNacionalidad = JsonConvert.DeserializeObject<List<Nacionalidad>>(strJSON);
            }

            catch (Exception _exc)
            {
                lstNacionalidad = null;
            }

            return lstNacionalidad;

        }



        // GET api/Facturacion/GetObtenerDocumentosIdentificacion
        [Route("api/Facturacion/GetObtenerDocumentosIdentificacion", Name = "Obtiene los documentos de identificación")]
        public List<DocumentoIdentificacion> GetObtenerDocumentosIdentificacion()
        {

            List<DocumentoIdentificacion> lstDocumentos = new List<DocumentoIdentificacion>();

            try
            {
                 string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\IdentificacionOficial.json");
                lstDocumentos = JsonConvert.DeserializeObject<List<DocumentoIdentificacion>>(strJSON);

            }

            catch (Exception)
            {
                lstDocumentos = null;
            }

            return lstDocumentos;

        }


        // GET api/Facturacion/GetObtenerSexo
        [Route("api/Facturacion/GetObtenerSexo", Name = "Obtiene catalogo de sexo")]
        public List<CatalogoSexo> GetObtenerSexo()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            List<CatalogoSexo> lstSexo = new List<CatalogoSexo>();
            CatalogoSexo sexo = new CatalogoSexo();
            
            try
            {
                //   string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\CatalogoSexo.json");
                // lstSexo = JsonConvert.DeserializeObject<List<CatalogoSexo>>(strJSON);

                string strSexo = "";
                strSexo += "select FDSXIDSEXO, FSSXDESCRI from PRODPERS.CTCSEXO";

                DataTable comp = dbCnx.GetDataSet(strSexo).Tables[0];

                if (comp.Rows.Count > 0) {

                    foreach (DataRow dr in comp.Rows) {

                        sexo = new CatalogoSexo();

                        sexo.IdSexo = dr["FDSXIDSEXO"].ToString().Trim();
                        sexo.DescripcionSexo = dr["FSSXDESCRI"].ToString().Trim();
                        lstSexo.Add(sexo);

                    }
                }
            }

            catch (Exception _exc)
            {
                lstSexo = null;
            }

            return lstSexo;

        }



        // GET api/Facturacion/GetObtenerOcupacion
        [Route("api/Facturacion/GetObtenerOcupacion", Name = "Obtiene catalogo de ocupaciones")]
        public List<Ocupacion> GetObtenerOcupacion()
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            List<Ocupacion> lstOcupacion = new List<Ocupacion>();
            Ocupacion ocupacion = new Ocupacion();

            try
            {
                 string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Ocupacion.json");
                 lstOcupacion = JsonConvert.DeserializeObject<List<Ocupacion>>(strJSON);

            }

            catch (Exception)
            {
                lstOcupacion = null;
            }

            return lstOcupacion;

        }



        // GET api/Facturacion/GetObtenerCatalogoSociedad
        [Route("api/Facturacion/GetObtenerCatalogoSociedad", Name = "Obtiene catalogo de sociedad para persona moral")]
        public List<Sociedad> GetObtenerCatalogoSociedad()
        {
            
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            List<Sociedad> lstSociedad = new List<Sociedad>();
            Sociedad sociedad = new Sociedad();


            try
            {
                
                string strSoc = "";
                strSoc += "select FIGEIDSOCI, FSGEDESSOC from PRODGRAL.GECSOCIE WHERE FIGESTATUS = 1";

                DataTable soc = dbCnx.GetDataSet(strSoc).Tables[0];

                if (soc.Rows.Count > 0)
                {

                    foreach (DataRow dr in soc.Rows)
                    {
                        sociedad = new Sociedad();

                        sociedad.IdSociedad = dr["FIGEIDSOCI"].ToString().Trim();
                        sociedad.DescripcionSociedad = dr["FSGEDESSOC"].ToString().Trim();
                        lstSociedad.Add(sociedad);

                    }
                }
                
            }

            catch (Exception _exc)
            {
                lstSociedad = null;
            }

            return lstSociedad;
            
        }


        // GET api/Facturacion/GetObtenerGiroComercial
        [Route("api/Facturacion/GetObtenerGiroComercial", Name = "Obtiene catalogo de giro comercial para personas morales")]
        public List<GiroComercial> GetObtenerGiroComercial()
        {
            
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            List<GiroComercial> lstGiro = new List<GiroComercial>();
            GiroComercial giro = new GiroComercial();


            try
            {
                
                string strGiro = "";
                strGiro += "select FICTIDGRCM, TRIM(FSCTDSGRCM) FSCTDSGRCM from PRODPERS.CTCGRCME WHERE  FICTSTATUS = 1 ";

                DataTable gir = dbCnx.GetDataSet(strGiro).Tables[0];

                if (gir.Rows.Count > 0)
                {

                    foreach (DataRow dr in gir.Rows)
                    {
                        giro = new GiroComercial();

                        giro.IdGiroComercial = dr["FICTIDGRCM"].ToString().Trim();
                        giro.DescripcionGiroComercial = dr["FSCTDSGRCM"].ToString().Trim();
                        lstGiro.Add(giro);

                    }

                    lstGiro = lstGiro.OrderBy(s=>s.DescripcionGiroComercial).ToList();
                }

            }

            catch (Exception _exc)
            {
                lstGiro = null;
            }

            return lstGiro;

        }


    }
}
