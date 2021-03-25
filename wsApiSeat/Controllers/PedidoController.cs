using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using wsApiSeat.Bussiness;
using wsApiSeat.Models;
using wsApiSeat.Models.ContactoAgencia;
using wsApiSeat.Models.Pedido;

namespace wsApiSeat.Controllers
{
    public class PedidoController : ApiController
    {

        // POST api/Autos/GenerarReferenciaBancaria
        [Route("api/Pedido/GenerarReferenciaBancaria", Name = "GenerarReferenciaBancaria")]
        public Respuesta GenerarReferenciaBancaria(int IdAgencia, long IdCliente, long IdPedido)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "";

            PedidoModel pedido = new PedidoModel();
            try
            {
                string ruta = DVAReferenciaBancaria.Referencia.getLineaDeCaptura_App(IdAgencia,
                Convert.ToInt32(IdCliente),
                DVAReferenciaBancaria.Referencia.TipoModulo.AUTOS_NUEVOS,
                Convert.ToInt32(IdPedido), "C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "MiAuto", 1, 1);
                ruta = ruta.Replace("C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/");

                respuesta.Ok = "SI";
                respuesta.Mensaje = ruta;

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = "NO";
                //respuesta.Mensaje = "No se generó correctamente.";
                respuesta.Mensaje = ex.Message.ToString();
            }

            return respuesta;

        }

        // Get api/Pedido/GetObtenerInventario
        [Route("api/Pedido/GetObtenerInventario", Name = "GetObtenerInventario")]
        public List<InventarioModel> GetObtenerInventario()
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string respuesta = "SI";
            try
            {
                string strSql = "";
                strSql = "SELECT AUTOM.FNAUTOAGE, AUTOM.FIANIDINVE, AUTOM.FNAUTOMID, AUTOM.FNAUTOVAN, 'N' || '-' || FNAUTOANO || '-' || FNAUTONUM NumeroInventario, TRIM(AUTOM.FSAUTOSER) FSAUTOSER, AUTOM.FNAUTOSUV, AUTOM.FNAUTOIVV, AUTOM.FNAUTOTAV, TRIM(COLOR.FNCOLOCLA) IDCOLOR,TRIM(COLOR.FSCOLODES) FSCOLODES ";
                strSql += "FROM PRODAUT.ANCMARCA MARCA ";
                strSql += "INNER JOIN   PRODAUT.ANCFAMIL FAMIL ";
                strSql += "ON   MARCA.FNMARCCLA = FAMIL.FNFAMISUB ";
                strSql += "INNER JOIN   PRODAUT.ANCMODEL MODEL ";
                strSql += "ON   FAMIL.FNFAMICLA = MODEL.FNMODEFAM ";
                strSql += "INNER JOIN   PRODAUT.ANCVERSI VERSI ";
                strSql += "ON   MODEL.FNMODEIDM = VERSI.FNVERSIDM ";
                strSql += "AND  VERSI.FNVERSIDV IN (37199,37290,37180,37292,37181,37192,37237,37391,37178,37179,37301,37238,37239,37086,37093,37023,37043,37020) ";
                strSql += "INNER JOIN   PRODAUT.ANCAUTOM AUTOM ";
                strSql += "ON   VERSI.FNVERSIDV = AUTOM.FNAUTOVAN AND AUTOM.FNAUTOAGE IN (36,12,35) ";
                strSql += "AND  AUTOM.FNAUTOEST IN (6,10) ";
                strSql += "INNER JOIN   PRODAUT.ANCCOLOR COLOR ";
                strSql += "ON   AUTOM.FNAUTOCOE = COLOR.FNCOLOCLA ";
                strSql += "WHERE    MARCA.FNMARCCLA = 41";

                DataTable dtInventario = dbCnx.GetDataSet(strSql).Tables[0];


                InventarioModel inventario;
                List<InventarioModel> coleccionInventario = new List<InventarioModel>();

                if (dtInventario.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtInventario.Rows)
                    {
                        inventario = new InventarioModel();
                        inventario.Color = dr["FSCOLODES"].ToString().Trim();
                        inventario.IdAgencia = dr["FNAUTOAGE"].ToString().Trim();
                        inventario.IdInventario = dr["FIANIDINVE"].ToString().Trim();
                        inventario.IdVehiculo = dr["FNAUTOMID"].ToString().Trim();
                        inventario.IdVersion = dr["FNAUTOVAN"].ToString().Trim();
                        inventario.IdColor = dr["IDCOLOR"].ToString().Trim();
                        #region ColorUnidad
                        switch (inventario.IdVersion)
                        {
                            case "37290": //IBIZA REFERENCE TIPTRONIC
                                switch (inventario.IdColor)
                                {
                                    #region color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Negro-Medianoche.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "7449": //Naranja Eclipse 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "8913": //Rojo Deseo 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Rojo-Deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Azul-Misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37199": //IBIZA REFERENCE STD
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Negro-Medianoche.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "7449": //Naranja Eclipse 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "8913": //Rojo Deseo 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Rojo-Deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Azul-Misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37292": //IBIZA STYLE TIPTRONIC
                                switch (inventario.IdColor)
                                {
                                    #region color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-nevada.png";
                                        break;
                                    case "289": //Blanco Candy
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-candy.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-naranja-eclipse.png";
                                        break;
                                    case "6915": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-rojo-deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 7180
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37180": //IBIZA STYLE STD
                                switch (inventario.IdColor)
                                {
                                    #region color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-nevada.png";
                                        break;
                                    case "289": //Blanco Candy
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-candy.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-naranja-eclipse.png";
                                        break;
                                    case "6915": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-rojo-deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 7180
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37181": //IBIZA XCELLENCE STD
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-blanco-nevada.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-negro-medianoche.png";
                                        break;
                                    case "1129": //Rojo Emoción 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-rojo-emocion.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37192": //IBIZA XCELLENCE TIPTRONIC.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-blanco-nevada.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-negro-medianoche.png";
                                        break;
                                    case "1129": //Rojo Emoción 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-rojo-emocion.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37237": //IBIZA FR STD.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-naranja-eclipse.png";
                                        break;
                                    case "1129": //Rojo Emoción
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-rojo-emocion.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37237xxx": //IBIZA FR TIPTRONIC.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-naranja-eclipse.png";
                                        break;
                                    case "1129": //Rojo Emoción
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-rojo-emocion.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37391": //ARONA REFERNCE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "6966": //AZUL MISTERIOR 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-azul-misterio.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ARONA/REFERENCE/arona-reference-blanco-nevada.png";
                                        break;
                                    case "7705": //Gris Mágnetico 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-gris-magnetico.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-naranja-eclipse.png";
                                        break;
                                    case "8913": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/Arona-Reference-Rojo-Deseo.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37178": //ARONA XCELLENCE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7599": //AZUL MISTERIOR CON TECHO NEGRO
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-azul-misterio-con-techo-negro.png";
                                        break;
                                    case "8163": //Naranja Eclipse con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-naranja-eclipse-con-techo-gris.png";
                                        break;
                                    case "7612": //Blanco Nevada con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-blanco-nevada-con-techo-gris.png";
                                        break;
                                    case "8059": //Gris Mágnetico con Techo Naranja
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-gris-magnetico-con-techo-naranja.png";
                                        break;
                                    case "8684": //Blanco Nevada con Techo Naranja
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-blanco-nevada-con-techo-naranja.png";
                                        break;
                                    case "7841": //Blanco Nevada con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-blanco-nevada-con-techo-negro.png";
                                        break;
                                    case "7803": //Gris Mágnetico con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-gris-magnetico-con-techo-negro.png";
                                        break;
                                    case "7802": //Plata Urbano con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-plata-urbano-con-techo-negro.png";
                                        break;
                                    case "7856": //Negro Medianoche con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-negro-medianoche-con-techo-gris.png";
                                        break;
                                    case "7904": //Rojo Deseo con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-rojo-deseo-con-techo-gris.png";
                                        break;
                                    case "7796": //Rojo Deseo con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-rojo-deseo-con-techo-negro.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37179": //ARONA STYLE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "6966": //Azul misterio
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-azul-misterio.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-blanco-nevada.png";
                                        break;
                                    case "7180": //Plata Urbano
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-plata-urbano.png";
                                        break;
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-gris-magnetico.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-naranja-eclipse.png";
                                        break;
                                    case "8913": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-rojo-deseo.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37301": //ATECA FR AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7221": //Rojo Velvet
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-rojo-velvet.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-blanco-nevada.png";
                                        break;
                                    case "7190": //Gris Rodio
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-gris-rodio.png";
                                        break;
                                    case "8384": //Negro Cristal
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-negro-cristal.png";
                                        break;
                                    case "7474": //AZUL ENERGIA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-azul-energia.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37238": //ATECA STYLE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7221": //Rojo Velvet
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-rojo-velvet.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-blanco-nevada.png";
                                        break;
                                    case "5007": //PLATA BILLANTE
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-plata-brillante.png";
                                        break;
                                    case "7190": //GRIS RODIO
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-plata-brillante.png";
                                        break;
                                    case "8384": //Negro Cristal
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-negro-cristal.png";
                                        break;
                                    case "5010": //Azul Lava
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-azul-lava.png";
                                        break;
                                        #endregion

                                }
                                break;
                            case "37239": //ATECA XPERIENCE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7221": //Rojo Velvet
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-rojo-velvet.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-blanco-nevada.png";
                                        break;
                                    case "5007": //PLATA BILLANTE
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-plata-brillante.png";
                                        break;
                                    case "7190": //GRIS RODIO
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-gris-rodio.png";
                                        break;
                                    case "8384": //Negro Cristal
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-negro-cristal.png";
                                        break;
                                    case "5010": //Azul Lava
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-azul-lava.png";
                                        break;
                                    case "8190": //Camuflaje
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-camuflaje.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37086": //ASEAT FR TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-blanco-onyx.png";
                                        break;
                                    case "469": //Gris Urano
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-gris-urano.png";
                                        break;
                                    case "8179": //Negro Profundo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-negro-profundo.png";
                                        break;
                                    case "8190": //Camuflaje
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-camuflaje.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37093": //ASEAT FR TARRACO DSG 3ERA FILA.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-blanco-onyx.png";
                                        break;
                                    case "469": //Gris Urano
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-gris-urano.png";
                                        break;
                                    case "8179": //Negro Profundo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-negro-profundo.png";
                                        break;
                                    case "8190": //Camuflaje
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-camuflaje.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37023": //ASEAT STYLE TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style--blanco-onyx.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-azul-atlantico.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37043": //ASEAT STYLE TARRACO DSG. 3RA FILA.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style--blanco-onyx.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-azul-atlantico.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "36115XXXX": //ASEAT XCELLENCE TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-blanco-onyx.png";
                                        break;
                                    case "8261":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-beige-titanio.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-azul-atlantico.png";
                                        break;
                                    case "8190":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-camuflaje.png";
                                        break;
                                }
                                break;
                            case "37020": //ASEAT XCELLENCE TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-blanco-onyx.png";
                                        break;
                                    case "8261":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-beige-titanio.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-azul-atlantico.png";
                                        break;
                                    case "8190":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-camuflaje.png";
                                        break;
                                }
                                break;

                        }
                        #endregion
                        inventario.NumeroSerie = dr["FSAUTOSER"].ToString().Trim();
                        inventario.NumeroInventario = dr["NumeroInventario"].ToString().Trim();
                        coleccionInventario.Add(inventario);
                    }
                }

                return coleccionInventario;
            }
            catch (Exception e)
            {
                List<InventarioModel> coleccionInventario = new List<InventarioModel>();
                return coleccionInventario;
                respuesta = e.Message;
                Console.WriteLine(respuesta);
            }
        }

        // Get api/Pedido/GetObtenerInventarioPorAgencia
        [Route("api/Pedido/GetObtenerInventarioPorAgencia", Name = "GetObtenerInventarioPorAgencia")]
        public List<InventarioModel> GetObtenerInventarioPorAgencia(int IdAgencia, string IdVersion)
        {
            System.Globalization.CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string respuesta = "SI";
            try
            {
                List<InventarioZona> lstZonas = new List<InventarioZona>();
                InventarioZona zona = new InventarioZona();
                string strSql = "";
                strSql = "SELECT AUTOM.FNAUTOAGE, AUTOM.FIANIDINVE, AUTOM.FNAUTOMID, AUTOM.FNAUTOVAN, 'N' || '-' || FNAUTOANO || '-' || FNAUTONUM NumeroInventario, TRIM(AUTOM.FSAUTOSER) FSAUTOSER, AUTOM.FNAUTOSUV, AUTOM.FNAUTOIVV, AUTOM.FNAUTOTAV, TRIM(COLOR.FNCOLOCLA) IDCOLOR,TRIM(COLOR.FSCOLODES) FSCOLODES ";
                strSql += "FROM PRODAUT.ANCMARCA MARCA ";
                strSql += "INNER JOIN PRODAUT.ANCFAMIL FAMIL ";
                strSql += "ON MARCA.FNMARCCLA = FAMIL.FNFAMISUB ";
                strSql += "INNER JOIN PRODAUT.ANCMODEL MODEL ";
                strSql += "ON FAMIL.FNFAMICLA = MODEL.FNMODEFAM ";
                strSql += "INNER JOIN PRODAUT.ANCVERSI VERSI ";
                strSql += "ON MODEL.FNMODEIDM = VERSI.FNVERSIDM ";
                strSql += "AND VERSI.FNVERSIDV = " + IdVersion + " ";
                strSql += "INNER JOIN PRODAUT.ANCAUTOM AUTOM ";
                strSql += "ON VERSI.FNVERSIDV = AUTOM.FNAUTOVAN AND AUTOM.FNAUTOAGE = " + IdAgencia + " ";
                strSql += "AND AUTOM.FNAUTOEST IN (6,10) ";
                strSql += "INNER JOIN  PRODAUT.ANCCOLOR COLOR ";
                strSql += "ON AUTOM.FNAUTOCOE = COLOR.FNCOLOCLA ";
                strSql += "WHERE MARCA.FNMARCCLA = 41";

                DataTable dtInventario = dbCnx.GetDataSet(strSql).Tables[0];
                InventarioModel inventario;
                List<InventarioModel> coleccionInventario = new List<InventarioModel>();

                if (dtInventario.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtInventario.Rows)
                    {
                        inventario = new InventarioModel();
                        inventario.Color = textInfo.ToTitleCase(dr["FSCOLODES"].ToString().ToLower().Trim());
                        inventario.IdAgencia = dr["FNAUTOAGE"].ToString().Trim();
                        inventario.IdInventario = dr["FIANIDINVE"].ToString().Trim();
                        inventario.IdVehiculo = dr["FNAUTOMID"].ToString().Trim();
                        inventario.IdVersion = dr["FNAUTOVAN"].ToString().Trim();
                        inventario.IdColor = dr["IDCOLOR"].ToString().Trim();
                        #region ColorUnidad
                        switch (inventario.IdVersion)
                        {
                            case "28174": //IBIZA REFERENCE TIPTRONIC
                                switch (inventario.IdColor)
                                {
                                    #region color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Negro-Medianoche.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "7449": //Naranja Eclipse 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "8913": //Rojo Deseo 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Rojo-Deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Azul-Misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "27267": //IBIZA REFERENCE STD
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/ibiza-reference-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Negro-Medianoche.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "7449": //Naranja Eclipse 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Plata-Urbano.png";
                                        break;
                                    case "8913": //Rojo Deseo 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Rojo-Deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/REFERENCE/Ibiza-Reference-Azul-Misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37292": //IBIZA STYLE TIPTRONIC
                                switch (inventario.IdColor)
                                {
                                    #region color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-nevada.png";
                                        break;
                                    case "289": //Blanco Candy
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-candy.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-naranja-eclipse.png";
                                        break;
                                    case "6915": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-rojo-deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 7180
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37180": //IBIZA STYLE STD
                                switch (inventario.IdColor)
                                {
                                    #region color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-nevada.png";
                                        break;
                                    case "289": //Blanco Candy
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-blanco-candy.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-naranja-eclipse.png";
                                        break;
                                    case "6915": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-rojo-deseo.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 7180
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/STYLE/ibiza-style-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37181": //IBIZA XCELLENCE STD
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-blanco-nevada.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-negro-medianoche.png";
                                        break;
                                    case "1129": //Rojo Emoción 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-rojo-emocion.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37192": //IBIZA XCELLENCE TIPTRONIC.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-blanco-nevada.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-negro-medianoche.png";
                                        break;
                                    case "1129": //Rojo Emoción 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/XCELLENCE/ibiza-xcellence-rojo-emocion.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37237": //IBIZA FR STD.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-naranja-eclipse.png";
                                        break;
                                    case "1129": //Rojo Emoción
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-rojo-emocion.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37237xxx": //IBIZA FR TIPTRONIC.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-gris-magnetico.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-blanco-nevada.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-naranja-eclipse.png";
                                        break;
                                    case "1129": //Rojo Emoción
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-rojo-emocion.png";
                                        break;
                                    case "6966": //AZUL MISTERIOR 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/IBIZA/FR/ibiza-fr-azul-misterio.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37391": //ARONA REFERNCE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "6966": //AZUL MISTERIOR 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-azul-misterio.png";
                                        break;
                                    case "4354": //BLANCO NEVADA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ARONA/REFERENCE/arona-reference-blanco-nevada.png";
                                        break;
                                    case "7705": //Gris Mágnetico 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-gris-magnetico.png";
                                        break;
                                    case "7180": //PLATA URBANO 
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-plata-urbano.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/arona-reference-naranja-eclipse.png";
                                        break;
                                    case "8913": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/reference/Arona-Reference-Rojo-Deseo.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37178": //ARONA XCELLENCE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7599": //AZUL MISTERIOR CON TECHO NEGRO
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-azul-misterio-con-techo-negro.png";
                                        break;
                                    case "8163": //Naranja Eclipse con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-naranja-eclipse-con-techo-gris.png";
                                        break;
                                    case "7612": //Blanco Nevada con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-blanco-nevada-con-techo-gris.png";
                                        break;
                                    case "8059": //Gris Mágnetico con Techo Naranja
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-gris-magnetico-con-techo-naranja.png";
                                        break;
                                    case "8684": //Blanco Nevada con Techo Naranja
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-blanco-nevada-con-techo-naranja.png";
                                        break;
                                    case "7841": //Blanco Nevada con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-blanco-nevada-con-techo-negro.png";
                                        break;
                                    case "7803": //Gris Mágnetico con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-gris-magnetico-con-techo-negro.png";
                                        break;
                                    case "7802": //Plata Urbano con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-plata-urbano-con-techo-negro.png";
                                        break;
                                    case "7856": //Negro Medianoche con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-negro-medianoche-con-techo-gris.png";
                                        break;
                                    case "7904": //Rojo Deseo con Techo Gris
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-rojo-deseo-con-techo-gris.png";
                                        break;
                                    case "7796": //Rojo Deseo con Techo Negro
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/xcellence/arona-xcellence-rojo-deseo-con-techo-negro.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37179": //ARONA STYLE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "6966": //Azul misterio
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-azul-misterio.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-blanco-nevada.png";
                                        break;
                                    case "7180": //Plata Urbano
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-plata-urbano.png";
                                        break;
                                    case "7705": //Gris Mágnetico
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-gris-magnetico.png";
                                        break;
                                    case "7170": //Negro Medianoche
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-negro-medianoche.png";
                                        break;
                                    case "7449": //Naranja Eclipse
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-naranja-eclipse.png";
                                        break;
                                    case "8913": //Rojo Deseo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/arona/style/arona-style-rojo-deseo.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37301": //ATECA FR AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7221": //Rojo Velvet
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-rojo-velvet.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-blanco-nevada.png";
                                        break;
                                    case "7190": //Gris Rodio
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-gris-rodio.png";
                                        break;
                                    case "8384": //Negro Cristal
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-negro-cristal.png";
                                        break;
                                    case "7474": //AZUL ENERGIA
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/FR/ateca-fr-azul-energia.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37238": //ATECA STYLE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7221": //Rojo Velvet
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-rojo-velvet.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-blanco-nevada.png";
                                        break;
                                    case "5007": //PLATA BILLANTE
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-plata-brillante.png";
                                        break;
                                    case "7190": //GRIS RODIO
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-plata-brillante.png";
                                        break;
                                    case "8384": //Negro Cristal
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-negro-cristal.png";
                                        break;
                                    case "5010": //Azul Lava
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/STYLE/ateca-style-azul-lava.png";
                                        break;
                                        #endregion

                                }
                                break;
                            case "37239": //ATECA XPERIENCE AUT.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "7221": //Rojo Velvet
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-rojo-velvet.png";
                                        break;
                                    case "4354": //Blanco Nevada
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-blanco-nevada.png";
                                        break;
                                    case "5007": //PLATA BILLANTE
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-plata-brillante.png";
                                        break;
                                    case "7190": //GRIS RODIO
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-gris-rodio.png";
                                        break;
                                    case "8384": //Negro Cristal
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-negro-cristal.png";
                                        break;
                                    case "5010": //Azul Lava
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-azul-lava.png";
                                        break;
                                    case "8190": //Camuflaje
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/ATECA/XPERIENCE/ateca-xperience-camuflaje.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37086": //ASEAT FR TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-blanco-onyx.png";
                                        break;
                                    case "469": //Gris Urano
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-gris-urano.png";
                                        break;
                                    case "8179": //Negro Profundo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-negro-profundo.png";
                                        break;
                                    case "8190": //Camuflaje
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-camuflaje.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37093": //ASEAT FR TARRACO DSG 3ERA FILA.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-blanco-onyx.png";
                                        break;
                                    case "469": //Gris Urano
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-gris-urano.png";
                                        break;
                                    case "8179": //Negro Profundo
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-negro-profundo.png";
                                        break;
                                    case "8190": //Camuflaje
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/FR/tarraco-fr-camuflaje.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37023": //ASEAT STYLE TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style--blanco-onyx.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-azul-atlantico.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "37043": //ASEAT STYLE TARRACO DSG. 3RA FILA.
                                switch (inventario.IdColor)
                                {
                                    #region Color
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style--blanco-onyx.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/STYLE/tarraco-style-azul-atlantico.png";
                                        break;
                                        #endregion
                                }
                                break;
                            case "36115XXXX": //ASEAT XCELLENCE TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-blanco-onyx.png";
                                        break;
                                    case "8261":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-beige-titanio.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-azul-atlantico.png";
                                        break;
                                    case "8190":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-camuflaje.png";
                                        break;
                                }
                                break;
                            case "37020": //ASEAT XCELLENCE TARRACO DSG.
                                switch (inventario.IdColor)
                                {
                                    case "8478":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-blanco-onyx.png";
                                        break;
                                    case "8261":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-beige-titanio.png";
                                        break;
                                    case "8180":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-indio.png";
                                        break;
                                    case "469":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-gris-urano.png";
                                        break;
                                    case "5007":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-plata-brillante.png";
                                        break;
                                    case "8179":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-negro-profundo.png";
                                        break;
                                    case "8219":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-azul-atlantico.png";
                                        break;
                                    case "8190":
                                        inventario.Ruta = "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Resources/Autos/TARRACO/XCELLENCE/tarraco-xcellence-camuflaje.png";
                                        break;
                                }
                                break;

                        }
                        #endregion
                        inventario.NumeroSerie = dr["FSAUTOSER"].ToString().Trim();
                        inventario.NumeroInventario = dr["NumeroInventario"].ToString().Trim();
                        coleccionInventario.Add(inventario);
                    }
                }

                return coleccionInventario;
            }
            catch (Exception ex)
            {
                List<InventarioModel> coleccionInventario = new List<InventarioModel>();
                return coleccionInventario;
                respuesta = ex.Message;
                Console.WriteLine(respuesta);
            }
        }

        // POST api/Autos/RegistraClienteCuentaPedidoBorrar
        [Route("api/Pedido/RegistraClienteCuentaPedidoBorrar", Name = "RegistraClienteCuentaPedidoBorrar")]
        public RespuestaTest<PedidoModel> RegistraClienteCuentaPedidoBorrar([FromBody] PedidoModel Pedido)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new
            DVAConstants.Constants();

            RespuestaTest<PedidoModel> respuesta = new RespuestaTest<PedidoModel>();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "";

            PedidoModel pedidoModel = new PedidoModel();
            try
            {
                pedidoModel.IdPedido = "84852";
                pedidoModel.IdAgencia = "12";
                pedidoModel.Fecha = "2020-06-01";
                pedidoModel.IdCliente = "2270985";
                pedidoModel.RFC = "GUVM952006";
                pedidoModel.Nombre = "MAURICIO";
                pedidoModel.ApellidoPaterno = "GUERRERO";
                pedidoModel.ApellidoMaterno = "VEGA";
                pedidoModel.Telefono = "5565656565";
                pedidoModel.Correo = "mauro-gro@gmail.com";
                pedidoModel.IdInventario = "7363";
                pedidoModel.IdVehiculo = "1276156";
                pedidoModel.IdVersion = "36034";
                pedidoModel.NumeroSerie = "VSSAD75F4L6535234";
                pedidoModel.SubTotal = "472414.66";
                pedidoModel.Iva = "84852";
                pedidoModel.Total = "548001.00";

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro exitoso.";
                respuesta.Objeto = pedidoModel;

                return respuesta;

            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No fue posible registrar el pedido.";
            }

            return respuesta;

        }

        // POST api/Autos/GenerarReferenciaBancariaBorrar
        [Route("api/Pedido/GenerarReferenciaBancariaBorrar", Name = "GenerarReferenciaBancariaBorrar")]
        public Respuesta GenerarReferenciaBancariaBorrar(int IdAgencia, long IdCliente, long IdPedido)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "";

            PedidoModel pedido = new PedidoModel();
            try
            {
                IdAgencia = 12;
                IdCliente = 2270985;
                IdPedido = 84852;

                //string ruta = DVAReferenciaBancaria.Referencia.getLineaDeCaptura_App(IdAgencia,
                //Convert.ToInt32(IdCliente),
                //DVAReferenciaBancaria.Referencia.TipoModulo.AUTOS_NUEVOS,
                //Convert.ToInt32(IdPedido), "C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "MiAuto", 1, 1);
                //ruta = ruta.Replace("C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/");

                respuesta.Ok = "SI";
                respuesta.Mensaje = "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/LineaCaptura_20200802165216.pdf";

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = ex.Message.ToString();
            }

            return respuesta;

        }

        [Route("api/Pedido/PostSubirDocumentos", Name = "PostSubirDocumentos")]
        [HttpPost]
        public async Task<Respuesta> PostSubirDocumentos(long IdCompra, int TipoDocumento, string Documento)
        {

            //var root = ctx.Server.MapPath("~/Compras/Cupra/Documentacion/");            
            String paths = string.Empty;
            Respuesta respuesta = new Respuesta();
            try
            {
                var ctx = HttpContext.Current;
                //var root = HostingEnvironment.MapPath("~" + "/Compras/Cupra/Documentacion");
                var root = HostingEnvironment.MapPath("~" + "/Compras/Documentacion");
                paths = root;
                var provider = new MultipartFormDataStreamProvider(root);
                string path = string.Empty;
                int index = 0;

                BPedido service = new BPedido();

                //probar con este 

                //await Task.FromResult(Request.Content.ReadAsMultipartAsync(provider));

                await Request.Content.ReadAsMultipartAsync(provider); // esto si va la linea original


                //idSolicitud = servicio.PostRegistrarSolicitudGetId(IdCuenta, CvePlataforma);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                    // remove double quotes from string.
                    name = name.Trim('"').ToLower();
                    index = name.IndexOf(".jpg");
                    if (index == -1) { index = name.IndexOf(".pdf"); }
                    if (index == -1) { index = name.IndexOf(".png"); }
                    if (index == -1) { index = name.IndexOf(".jpeg"); }
                    //if (index == -1) { index = name.IndexOf(".mp4"); }                    
                    //name = IdTipoDocumento + "_" + idSolicitud + "_" + IdCuenta; //name.Trim('"');
                    var localFileName = file.LocalFileName;
                    name = IdCompra + "_" + TipoDocumento + "_" + DateTime.Now.ToString("yyyyMMdd") + "" + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;
                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        respuesta = service.PostSubirDocumentos(IdCompra, TipoDocumento, Documento, path);
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        respuesta = service.UpdateRegister(IdCompra, TipoDocumento, Documento, filePath);
                    }

                    // ruta = ruta.Replace("C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/");
                    //http://ws-smartit.divisionautomotriz.com/wsApiSeat

                }
            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }
            return respuesta;
        }

        [Route("api/Pedido/PostSubirFotoPerfil", Name = "PostSubirFotoPerfil")]
        [HttpPost]
        public async Task<Respuesta> PostSubirFotoPerfil(long IdCuenta, string Foto)
        {

            String paths = string.Empty;
            Respuesta respuesta = new Respuesta();
            try
            {
                var ctx = HttpContext.Current;
                var root = HostingEnvironment.MapPath("~" + "/Cuentas/FotoPerfil");
                paths = root;
                var provider = new MultipartFormDataStreamProvider(root);
                string path = string.Empty;
                int index = 0;

                BPedido service = new BPedido();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                    // remove double quotes from string.
                    name = name.Trim('"').ToLower();
                    index = name.IndexOf(".jpg");
                    //if (index == -1) { index = name.IndexOf(".pdf"); }
                    if (index == -1) { index = name.IndexOf(".png"); }
                    if (index == -1) { index = name.IndexOf(".jpeg"); }
                    //if (index == -1) { index = name.IndexOf(".mp4"); }                    
                    //name = IdTipoDocumento + "_" + idSolicitud + "_" + IdCuenta; //name.Trim('"');
                    var localFileName = file.LocalFileName;
                    name = IdCuenta /*+ "_"  + DateTime.Now.ToString("yyyyMMdd") + ""*/ + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;
                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        respuesta = service.PostSubirFoto(IdCuenta, path);
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        respuesta = service.UpdateRegisterFotoPerfil(IdCuenta, filePath);
                    }

                }

            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }
            return respuesta;
        }


        //GET api/Pedido/GetObtieneRutaFotoPerfil
        [Route("api/Pedido/GetObtieneRutaFotoPerfil", Name = "GetObtieneRutaFotoPerfil")]
        public RespuestaTest<Cuenta> GetObtieneRutaFotoPerfil(int IdCuenta)
        {
            RespuestaTest<Cuenta> respuesta = new RespuestaTest<Cuenta>();
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();
                string Query = "";
                Query = "SELECT FSAPRUTFOT ";
                Query += "FROM    " + constantes.Ambiente + "APPS.APCCTAST ";
                Query += "WHERE FIAPIDCUEN = " + IdCuenta + " AND FIAPSTATUS = 1";

                DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow dr in DT.Rows)
                    {
                        if (dr["FSAPRUTFOT"].ToString().Trim() == "" || dr["FSAPRUTFOT"].ToString().Trim() == null)
                        {

                            respuesta.Ok = "NO";
                            respuesta.Mensaje = "No se encontró foto de perfil para la cuenta indicada.";
                            respuesta.Objeto = null;
                        }
                        else
                        {
                            respuesta.Ok = "SI";
                            respuesta.Mensaje = dr["FSAPRUTFOT"].ToString().Trim();
                            respuesta.Objeto = null;
                        }
                    }
                }
                else
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No se encontró la cuenta indicada";
                    respuesta.Objeto = null;
                }
            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "ocurrió un error al tratar de obtener la foto de perfil";
                respuesta.Objeto = null;

                return respuesta;
            }

            return respuesta;
        }


        [Route("api/Pedido/PostSubirComprobantePago", Name = "PostSubirComprobantePago")]
        [HttpPost]
        public async Task<Respuesta> PostSubirComprobantePago(long IdCompra, int Consecutivo, int TipoDocumento, string Documento)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string sqlStr = "";
            sqlStr += "SELECT FIAPIDCONS, FIAPIDCOMP, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC ";
            sqlStr += "FROM PRODAPPS.APDCMPST ";
            sqlStr += "WHERE FIAPIDCONS = " + Consecutivo;
            sqlStr += " AND FIAPIDTIPO = " + TipoDocumento;
            sqlStr += " AND FIAPIDCOMP = " + IdCompra;


            DataTable comp = dbCnx.GetDataSet(sqlStr).Tables[0];

            if (comp.Rows.Count > 0)
            {

                string strSql = "";
                strSql += "UPDATE PRODAPPS.APDCMPST ";
                strSql += "SET FIAPSTATUS = 0 ";
                strSql += " WHERE FIAPIDTIPO = " + TipoDocumento;
                strSql += " AND FIAPIDCOMP = " + IdCompra;
                strSql += " AND FIAPIDCONS = " + Consecutivo;

                dbCnx.SetQuery(strSql);
            }

            //var root = ctx.Server.MapPath("~/Compras/Cupra/Documentacion/");            
            String paths = string.Empty;
            Respuesta respuesta = new Respuesta();
            try
            {
                var ctx = HttpContext.Current;
                //var root = obtenerURLServidor() + "//Compras//Cupra//Documentacion//";
                //var root = HttpContext.Current.Server.MapPath("~/Compras/Cupra/Documentacion");//ctx.Server.MapPath("~/Compras/Cupra/Documentacion");            
                //var root = HostingEnvironment.MapPath("~" + "/Compras/Cupra/ComprobantePagos");
                var root = HostingEnvironment.MapPath("~" + "/Compras/ComprobantePagos");
                paths = root;
                var provider = new MultipartFormDataStreamProvider(root);
                string path = string.Empty;
                int index = 0;

                BPedido service = new BPedido();

                await Request.Content.ReadAsMultipartAsync(provider);


                //idSolicitud = servicio.PostRegistrarSolicitudGetId(IdCuenta, CvePlataforma);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
                    // remove double quotes from string.
                    name = name.Trim('"').ToLower();
                    index = name.IndexOf(".jpg");
                    if (index == -1) { index = name.IndexOf(".pdf"); }
                    if (index == -1) { index = name.IndexOf(".png"); }
                    if (index == -1) { index = name.IndexOf(".jpeg"); }
                    //if (index == -1) { index = name.IndexOf(".mp4"); }                    
                    //name = IdTipoDocumento + "_" + idSolicitud + "_" + IdCuenta; //name.Trim('"');
                    var localFileName = file.LocalFileName;
                    name = IdCompra + "_" + TipoDocumento + "_" + Consecutivo + "_" + DateTime.Now.ToString("yyyyMMdd") + "" + name.Substring(index);
                    var filePath = Path.Combine(root, name);
                    path = filePath;
                    if (!File.Exists(filePath))
                    {
                        File.Move(localFileName, filePath);
                        respuesta = service.PostSubirComprobantePagos(IdCompra, Consecutivo, TipoDocumento, Documento, path);
                    }
                    else
                    {
                        File.Delete(filePath);
                        File.Move(localFileName, filePath);
                        respuesta = service.UpdateRegisterComprobantePagos(IdCompra, Consecutivo, TipoDocumento, Documento, filePath);
                    }

                    // ruta = ruta.Replace("C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/");
                    //http://ws-smartit.divisionautomotriz.com/wsApiSeat

                }
            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = e.Message + "ruta->:" + paths;
            }

            return respuesta;
        }


        private string obtenerURLServidor()
        {
            HttpRequest request = HttpContext.Current.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";

            return baseUrl;
        }

        [Route("api/Pedido/PostRegitraClienteCuentaPedido", Name = "PostRegitraClienteCuentaPedido")]
        public Respuesta PostRegistraClienteCuentaPedido([FromBody]CompraAutoNuevo compra)
        {
            //Compra compra = new Compra();
            BPedido bussiness = new BPedido();
            return bussiness.PostRegistraClienteCuentaPedido(compra);

        }


        [Route("api/Pedido/PostRegistraPedido", Name = "PostRegistraPedido")]
        public RespuestaTest<CompraAutoNuevo> PostRegistraPedido([FromBody]CompraAutoNuevo compra)
        {
            BPedido bussiness = new BPedido();
            return bussiness.PostRegistraPedido(compra);

        }

        [Route("api/Pedido/PostRegistraDatosFiscales", Name = "PostRegistraDatosFiscales")]
        public Respuesta PostRegistraDatosFiscales(DatosFiscalesProc2 datosFiscales, int IdTipoPersona)
        {
            BPedido servicio = new BPedido();
            return servicio.RegistraDatosFiscales(datosFiscales, IdTipoPersona);
        }

        [Route("api/Pedido/GetDatosFiscales", Name = "GetDatosFiscales")]
        public Models.Pedido.DatosFiscalesProc2 GetDatosFiscales(string IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            Models.Pedido.DatosFiscalesProc2 dato = new Models.Pedido.DatosFiscalesProc2();

            try
            {
                string strSql = "";
                strSql += "SELECT ";
                strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS,";
                strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS,";
                strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG,";
                strSql += "FSAPAMTRLG, FIAPLDTRLG, FSAPCRRRLG, FIAPNMTRLG, FSAPCUCFDI, FSAPDESCRI,";



                strSql += "FIAPIDNACP, FSAPDESNAP, FSAPCURPPE,";
                strSql += "FSAPCALLEP, FSAPNUMEXP, FSAPNUMINP, FSAPCOLONP, FSAPDELEGP, FSAPCIUDAP, FSAPESTADP, FSAPDIRPER,";
                strSql += "FIAPIDGENP, FSAPDESGEP, FIAPIDOCUP, FSAPDESOCP,";
                strSql += "FIAPIDTSOP, FSAPDESTSP, FFAPCREEMP, FIAPIDGIRO, FSAPDESCGI,";
                strSql += "FSAPCALLER, FSAPNUMEXR, FSAPNUMINR, FSAPCOLONR, FSAPDELEGR, FSAPCIUDAR, FSAPESTADR, FSAPDIRREP,";
                strSql += "FIAPIDNACR, FSAPDESNAR, FIAPIDGENR, FSAPDESGER, FIAPIDOCUR, FSAPDESOCR,";
                strSql += "FSAPAPTCON, FSAPAPMCON, FSAPNOMCON, FIAPLADCON, FIAPTELCON, FSAPCORCON ";



                strSql += "FROM PRODAPPS.APDDTFST WHERE FIAPSTATUS = 1 AND FIAPIDCOMP = " + IdCompra;
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];



                if (dt.Rows.Count > 0)
                {
                    dato = new Models.Pedido.DatosFiscalesProc2()
                    {
                        IdCompra = dt.Rows[0]["FIAPIDCOMP"].ToString().Trim()
                                ,
                        RfcFisica = dt.Rows[0]["FSAPRFCFIS"].ToString().Trim()
                                ,
                        NombreFisica = dt.Rows[0]["FSAPNMBFIS"].ToString().Trim()
                                ,
                        ApellidoPaternoFisica = dt.Rows[0]["FSAPAPTFIS"].ToString().Trim()
                                ,
                        ApellidoMaternoFisica = dt.Rows[0]["FSAPAMTFIS"].ToString().Trim()
                                ,
                        LadaFisica = dt.Rows[0]["FIAPLDTFIS"].ToString().Trim()
                                ,
                        NumeroTelefonoFisica = dt.Rows[0]["FIAPNMTFIS"].ToString().Trim()
                                ,
                        CorreoFisica = dt.Rows[0]["FSAPCRRFIS"].ToString().Trim()
                                ,
                        RfcRazonSocial = dt.Rows[0]["FSAPRFCRSC"].ToString().Trim()
                                ,
                        RazonSocial = dt.Rows[0]["FSAPRAZSOC"].ToString().Trim()
                                ,
                        RfcRepresentanteLegal = dt.Rows[0]["FSAPRFCRLG"].ToString().Trim()
                                ,
                        NombreRepresentanteLegal = dt.Rows[0]["FSAPNMBRLG"].ToString().Trim()
                                ,
                        ApellidoPaternoRepresentanteLegal = dt.Rows[0]["FSAPAPTRLG"].ToString().Trim()
                                ,
                        ApellidoMaternoRepresentantelegal = dt.Rows[0]["FSAPAMTRLG"].ToString().Trim()
                                ,
                        LadaRepresentantelegal = dt.Rows[0]["FIAPLDTRLG"].ToString().Trim()
                                ,
                        NumeroTelefonoRepresentanteLegal = dt.Rows[0]["FIAPNMTRLG"].ToString().Trim()
                                ,
                        CorreoRepresentantelegal = dt.Rows[0]["FSAPCRRRLG"].ToString().Trim()
                        ,
                        ClaveUsoCfdi = dt.Rows[0]["FSAPCUCFDI"].ToString().Trim()
                        ,
                        DescripcionUsoCfdi = dt.Rows[0]["FSAPDESCRI"].ToString().Trim()



                        ,
                        //Nuevos Campos 
                        IdNacionalidadC2 = dt.Rows[0]["FIAPIDNACP"].ToString().Trim(),
                        DescripcionNacionalidadC2 = dt.Rows[0]["FSAPDESNAP"].ToString().Trim(),
                        CurpC2 = dt.Rows[0]["FSAPCURPPE"].ToString().Trim(),



                        CallePersonaC2 = dt.Rows[0]["FSAPCALLEP"].ToString().Trim(),
                        NumeroExteriorPersonaC2 = dt.Rows[0]["FSAPNUMEXP"].ToString().Trim(),
                        NumeroInteriorPersonaC2 = dt.Rows[0]["FSAPNUMINP"].ToString().Trim(),
                        ColoniaPersonaC2 = dt.Rows[0]["FSAPCOLONP"].ToString().Trim(),
                        DelegacionPersonaC2 = dt.Rows[0]["FSAPDELEGP"].ToString().Trim(),
                        CiudadPersonaC2 = dt.Rows[0]["FSAPCIUDAP"].ToString().Trim(),
                        EstadoPersonaC2 = dt.Rows[0]["FSAPESTADP"].ToString().Trim(),
                        DireccionC2 = dt.Rows[0]["FSAPDIRPER"].ToString().Trim(),



                        IdGeneroPersonaPF = dt.Rows[0]["FIAPIDGENP"].ToString().Trim(),
                        DescripcionGeneroPF = dt.Rows[0]["FSAPDESGEP"].ToString().Trim(),
                        IdOcupacionPersonaPF = dt.Rows[0]["FIAPIDOCUP"].ToString().Trim(),
                        DescripcionOcupacionPF = dt.Rows[0]["FSAPDESOCP"].ToString().Trim(),



                        IdTipoSociedadPM = dt.Rows[0]["FIAPIDTSOP"].ToString().Trim(),
                        DescripcionTipoSociedadPM = dt.Rows[0]["FSAPDESTSP"].ToString().Trim(),
                        FechaConstitucionEmpresaPM = dt.Rows[0]["FFAPCREEMP"].ToString().Trim(),
                        IdGiroDeLaEmpresaPM = dt.Rows[0]["FIAPIDGIRO"].ToString().Trim(),
                        DescripcionGiroDeLaEmpresaPM = dt.Rows[0]["FSAPDESCGI"].ToString().Trim(),



                        CalleRepresentanteLegalVPF = dt.Rows[0]["FSAPCALLER"].ToString().Trim(),
                        NumeroExteriorRepresentanteLegalVPF = dt.Rows[0]["FSAPNUMEXR"].ToString().Trim(),
                        NumeroInteriorRepresentanteLegalVPF = dt.Rows[0]["FSAPNUMINR"].ToString().Trim(),
                        ColoniaRepresentanteLegalVPF = dt.Rows[0]["FSAPCOLONR"].ToString().Trim(),
                        DelegacionRepresentanteLegalVPF = dt.Rows[0]["FSAPDELEGR"].ToString().Trim(),
                        CiudadRepresentanteLegalVPF = dt.Rows[0]["FSAPCIUDAR"].ToString().Trim(),
                        EstadoRepresentanteLegalVPF = dt.Rows[0]["FSAPESTADR"].ToString().Trim(),
                        DireccionRepresentanteLegalVPF = dt.Rows[0]["FSAPDIRREP"].ToString().Trim(),



                        IdNacionalidadRepresentanteLegalVPF = dt.Rows[0]["FIAPIDNACR"].ToString().Trim(),
                        DescripcionNacionalidadRepresentanteLegalVPF = dt.Rows[0]["FSAPDESNAR"].ToString().Trim(),
                        IdGeneroRepresentanteLegalVPF = dt.Rows[0]["FIAPIDGENR"].ToString().Trim(),
                        DescripcionGeneroRepresentanteLegalVPF = dt.Rows[0]["FSAPDESGER"].ToString().Trim(),
                        IdOcupacionRepresentanteLegalVPF = dt.Rows[0]["FIAPIDOCUR"].ToString().Trim(),
                        DescripcionOcupacionRepresentanteLegalVPF = dt.Rows[0]["FSAPDESOCR"].ToString().Trim(),



                        ApellidoPaternoContactoVPF = dt.Rows[0]["FSAPAPTCON"].ToString().Trim(),
                        ApellidoMaternoContactoVPF = dt.Rows[0]["FSAPAPMCON"].ToString().Trim(),
                        NombresContactoVPF = dt.Rows[0]["FSAPNOMCON"].ToString().Trim(),
                        LadaContactoVPF = dt.Rows[0]["FIAPLADCON"].ToString().Trim(),
                        TelefonoContactoVPF = dt.Rows[0]["FIAPTELCON"].ToString().Trim(),
                        CorreoContactoVPF = dt.Rows[0]["FSAPCORCON"].ToString().Trim(),
                    };
                }
            }
            catch (Exception ex)
            {



            }

            return dato;

        }

        [Route("api/Pedido/PutCotizarSeguroEnPedido", Name = "PutCotizarSeguroEnPedido")]
        public Respuesta PutCotizarSeguroEnPedido(long IdCompra)
        {
            BPedido bussiness = new BPedido();
            return bussiness.ActualizarCotizarSeguroEnPedido(IdCompra);

        }


        [Route("api/Pedido/PostRegistraApartado", Name = "PostRegistraApartado")]
        public RespuestaTest<CompraAutoNuevo> PostRegistraApartado([FromBody] CompraAutoNuevo compra)
        {
            BPedido bussiness = new BPedido();
            return bussiness.RegistraApartado(compra);

        }


        [Route("api/Pedido/PostDeshacePasoRealizado", Name = "PostDeshacePasoRealizado")]
        public Respuesta PostDeshacePasoRealizado(long IdCompra, int IdPaso)
        {
            BPedido bussiness = new BPedido();
            return bussiness.DeshacerPaso(IdCompra, IdPaso);

        }


        [Route("api/Pedido/PostRegistraCheckContactoAgencia", Name = "PostRegistraCheckContactoAgencia")]
        public Respuesta PostRegistraCheckContactoAgencia([FromBody] CheckAgencia check)
        {
            BPedido bussiness = new BPedido();
            return bussiness.RegistraCheckContactoAgencia(check);

        }


        [Route("api/Pedido/PostActualizaCheckContactoAgencia", Name = "PostActualizaCheckContactoAgencia")]
        public Respuesta PostActualizaCheckContactoAgencia([FromBody] ActualizaCheckAgencia check)
        {
            BPedido bussiness = new BPedido();
            return bussiness.ActualizaCheckContactoAgencia(check);

        }

        [Route("api/Pedido/GetCheckContactoAgenciaPorIdCompra", Name = "GetCheckContactoAgenciaPorIdCompra")]
        public List<DatoCheckAgencia> GetCheckContactoAgenciaPorIdCompra(int IdCompra)
        {
            BPedido bussiness = new BPedido();
            return bussiness.ObtieneCheckContactoAgencia(IdCompra);

        }

        [Route("api/Pedido/PostRegistraHistoricoContactoAgencia", Name = "PostRegistraHistoricoContactoAgencia")]
        public Respuesta PostRegistraHistoricoContactoAgencia([FromBody] HistoricoComunicacionAgencia historico)
        {
            BPedido bussiness = new BPedido();
            return bussiness.RegistraHistorico(historico);

        }

        [Route("api/Pedido/PostSubirDocumentosContactoAgencia", Name = "PostSubirDocumentosContactoAgencia")]
        public async Task<Respuesta> PostSubirDocumentosContactoAgencia([FromBody] SubirArchivo archivo)
        {

            Respuesta respuesta = new Respuesta();

            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Compras/ComunicacionAgencia/");

            try
            {

                if (archivo.ExtensionArch.ToLower().Trim() == "pdf")
                {


                    BPedido service = new BPedido();

                    var name = archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.TipoDocumento + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + archivo.ExtensionArch;

                    string filePath = root.ToString() + name;


                    if (!File.Exists(filePath))
                    {

                        string salida = await service.creaPDF(filePath, archivo.Base64);

                        if (salida == "SI")
                        {
                            respuesta = service.PostSubirDocumentoComunicacionAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, filePath);
                        }

                    }
                    else
                    {
                        File.Delete(filePath);

                        string salida = await service.creaPDF(filePath, archivo.Base64);
                        if (salida == "SI")
                        {
                            respuesta = service.UpdateDocContactoAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, filePath);
                        }

                    }

                }
                else if (archivo.ExtensionArch.ToLower().Trim() == "png" || archivo.ExtensionArch.ToLower().Trim() == "jpeg" || archivo.ExtensionArch.ToLower().Trim() == "jpg")
                {

                    BPedido service = new BPedido();
                    var name = archivo.IdCompra + "_" + archivo.IdConsecutivo + "_" + archivo.TipoDocumento + "_" + DateTime.Now.ToString("yyyyMMdd") + ".Jpeg";
                    string filePath = root.ToString() + name;

                    if (!File.Exists(filePath))
                    {
                        string salida = await service.creaImagen(filePath, archivo.Base64);

                        if (salida == "SI")
                        {
                            respuesta = service.PostSubirDocumentoComunicacionAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, filePath);
                        }

                    }
                    else
                    {

                        File.Delete(filePath);
                        string salida = await service.creaImagen(filePath, archivo.Base64);
                        if (salida == "SI")
                        {
                            respuesta = service.UpdateDocContactoAgencia(Convert.ToInt64(archivo.IdCompra), Convert.ToInt32(archivo.IdConsecutivo), Convert.ToInt32(archivo.TipoDocumento), archivo.NombreDocumento, filePath);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo subir el archivo.";
            }
            return respuesta;
        }

        [Route("api/Pedido/GetObtenerDocumentoComunicacion", Name = "GetObtenerDocumentoComunicacion")]
        public List<DocumentoComunicacion> GetObtenerDocumentoComunicacion(int IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            string Query = "";
            Query = "SELECT FIAPIDCOMP, FIAPIDCONS, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC ";
            Query += "FROM	" + constantes.Ambiente + "APPS.APDDCSST ";
            Query += "WHERE FIAPIDCOMP = " + IdCompra + " AND FIAPSTATUS = 1";

            DataTable DT = dbCnx.GetDataSet(Query).Tables[0];

            DocumentoComunicacion Comprobante;
            List<DocumentoComunicacion> ComprobantesDePago = new List<DocumentoComunicacion>();

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow dr in DT.Rows)
                {
                    Comprobante = new DocumentoComunicacion();
                    Comprobante.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                    Comprobante.IdConsecutivo = dr["FIAPIDCONS"].ToString().Trim();
                    Comprobante.IdTipo = dr["FIAPIDTIPO"].ToString().Trim();
                    Comprobante.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                    Comprobante.RutaDocumento = dr["FSAPRUTDOC"].ToString().Trim();
                    ComprobantesDePago.Add(Comprobante);
                }
            }
            else
            {
                Comprobante = new DocumentoComunicacion();
                Comprobante.IdCompra = null;
                Comprobante.IdConsecutivo = null;
                Comprobante.IdTipo = null;
                Comprobante.NombreDocumento = null;
                Comprobante.RutaDocumento = null;
                ComprobantesDePago.Add(Comprobante);

            }
            return ComprobantesDePago;
        }

        public class datosTestReClien
        {
            public string idAgencia { get; set; }
            public string TipoPersona { get; set; }
            public string RfcFisica { get; set; }
            public string NumeroTelefonoFisica { get; set; }
            public string CorreoFisica { get; set; }
            public string NombreFisica { get; set; }
            public string ApellidoPaternoFisica { get; set; }
            public string ApellidoMaternoFisica { get; set; }
        }

    }
}
