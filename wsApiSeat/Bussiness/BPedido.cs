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
using wsApiSeat.Services;
using System.Threading;
using RestSharp;
using RestSharp.Deserializers;
using Microsoft.Ajax.Utilities;
using wsApiSeat.Models.Pedido;
using System.Security.Authentication;
using wsApiSeat.Models.ContactoAgencia;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace wsApiSeat.Bussiness
{
    public class BPedido
    {
        #region Constantes de la clase
        const string llaveAutorizacionNotificaciones = "Key=AAAAE2lxxJI:APA91bH_HW8bSZLgp4JIzVZhGoR6ziMZb-uPAYO6e0zXTt34AuxAp7W9bhdvAY5H03GwPggVNl3AmYPTy0KbMz4wQWyPIPsV7o-awXmhjdrx1NxQJK0b_xLatKpqeBqKBv7HZeZKFVlM";
        #endregion

        internal Task<Respuesta> UpdateEstadoCompraSmart(long aIdCompra, int aIdEstado)
        {
            return Task.Run(() =>
            {
                Respuesta respuesta = new Respuesta();
                DVADB.DB2 dbCnx = new DVADB.DB2();
                respuesta.Ok = "No";
                try
                {
                    string strSql = string.Empty;
                    strSql = @"UPDATE PRODAPPS.APECMPST SET FIAPIDESTA = " + aIdEstado +
                                " WHERE FIAPIDCOMP =" + aIdCompra;
                    dbCnx.SetQuery(strSql);
                    respuesta.Ok = "Si";
                    respuesta.Mensaje = "Se actualizo exitosamente";
                    respuesta.Objeto = "";
                }
                catch (Exception)
                {
                    respuesta.Mensaje = "Hubo un error al actualizar el estado";
                    respuesta.Objeto = "";
                }

                return respuesta;
            });
            
        }
        internal List<SegurosCliente> GetObtenerSeguros(long IdCompra, long Idcuenta)
        {
            
            DVADB.DB2 dbCnx = new DVADB.DB2();
            string strSql = string.Empty;
            strSql = @"
                SELECT C.FIAPIDCOMP, C.FIAPIDSPOL, C.FIAPIDTIPO, D.* FROM

                    (SELECT B.FIAPIDCOMP, B.FIAPIDSPOL, A.FIAPIDTIPO, B.FIAPIDCUEN FROM  PRODAPPS.APECTPSE A
                    CROSS JOIN(
                    SELECT DISTINCT FIAPIDSPOL, FIAPIDCOMP, FIAPIDCUEN FROM PRODAPPS.APEDSPOL
                            ) B) C
                            LEFT JOIN(SELECT * FROM PRODAPPS.APEDSPOL) D
                            ON C.FIAPIDCOMP = D.FIAPIDCOMP AND C.FIAPIDTIPO = D.FIAPIDTIPO AND C.FIAPIDSPOL = D.FIAPIDSPOL
                            WHERE C.FIAPIDCOMP =" + IdCompra + " AND C.FIAPIDCUEN = " + Idcuenta + " ORDER BY C.FIAPIDCOMP, C.FIAPIDSPOL, C.FIAPIDTIPO";

            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            SegurosCliente _ob;
            List<SegurosCliente> colecciones = new List<SegurosCliente>();

            foreach (DataRow dr in dt.Rows)
            {
                _ob = new SegurosCliente();
                _ob.IdConsecutivo = dr["FIAPIDSPOL"].ToString().Trim();
                _ob.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                _ob.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                _ob.Nombre = dr["FSAPNOASEG"].ToString().Trim();
                _ob.Cobertura = dr["FSAPCOBERT"].ToString().Trim();
                _ob.Cantidad = dr["FDAPCANTID"].ToString().Trim();
                _ob.Tipo = dr["FIAPIDTIPO"].ToString().Trim();
                _ob.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                _ob.Ruta = dr["FSAPRUTDOC"].ToString().Trim();
                colecciones.Add(_ob);
            }

            return colecciones;
        }

        internal List<SegurosClienteSmartIt> GetObtenerSegurosSmartIT(long IdCompra, long Idcuenta)
        {

            DVADB.DB2 dbCnx = new DVADB.DB2();
            string strSql = string.Empty;
            strSql = @"SELECT FIAPIDSPOL ,FIAPIDCOMP ,FIAPIDCUEN, FSAPNOASEG, FSAPCOBERT, FDAPCANTID, FIAPIDTIPO, FSAPDOCUME, FSAPRUTDOC ,FIAPSELECT FROM PRODAPPS.APEDSPOL " + "\n" +
                        "WHERE 1=1" + "\n" +
                        "AND FIAPIDCOMP= " + IdCompra + "\n" +
                        "AND FIAPIDCUEN= " + Idcuenta;
            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            SegurosClienteSmartIt _ob;
            List<SegurosClienteSmartIt> colecciones = new List<SegurosClienteSmartIt>();

            foreach (DataRow dr in dt.Rows)
            {
                _ob = new SegurosClienteSmartIt();
                _ob.IdConsecutivo = dr["FIAPIDSPOL"].ToString().Trim();
                _ob.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                _ob.IdCuenta = dr["FIAPIDCUEN"].ToString().Trim();
                _ob.Nombre = dr["FSAPNOASEG"].ToString().Trim();
                _ob.Cobertura = dr["FSAPCOBERT"].ToString().Trim();
                _ob.Cantidad = dr["FDAPCANTID"].ToString().Trim();
                _ob.Tipo = dr["FIAPIDTIPO"].ToString().Trim();
                _ob.NombreDocumento = dr["FSAPDOCUME"].ToString().Trim();
                _ob.Ruta = dr["FSAPRUTDOC"].ToString().Trim();
                _ob.selecciono = dr["FIAPSELECT"].ToString().Trim();
                colecciones.Add(_ob);
            }

            return colecciones;
        }
        internal Task<Respuesta> UpdateSeleccionaSeguro([FromBody] SeleccionaSeguroCliente seguro)
        {
            return Task.Run(()=>
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                Respuesta respuesta = new Respuesta();
                try
                {
                    string strSql = string.Empty;
                    strSql = @"SELECT FIAPSELECT FROM PRODAPPS.APEDSPOL" + "\n" +
                                "WHERE 1=1" + "\n" +
                                    "AND FIAPIDCOMP= " + seguro.IdCompra + "\n" +
                                    "AND FIAPIDCUEN= " + seguro.IdCuenta + "\n" +
                                    "AND FIAPSELECT= 1";
                    DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        strSql = string.Empty;
                        strSql = @"UPDATE PRODAPPS.APEDSPOL SET FIAPSELECT=" + 0 + "\n" +
                               "WHERE 1=1" + "\n" +
                                   "AND FIAPIDCOMP= " + seguro.IdCompra + "\n" +
                                   "AND FIAPIDCUEN= " + seguro.IdCuenta;
                        dbCnx.SetQuery(strSql);
                    }

                    strSql = string.Empty;
                    strSql = @"UPDATE PRODAPPS.APEDSPOL SET FIAPSELECT=" + 1 + "\n" +
                                "WHERE 1=1" + "\n" +
                                    "AND FIAPIDCOMP= " + seguro.IdCompra + "\n" +
                                    "AND FIAPIDCUEN= " + seguro.IdCuenta + "\n" +
                                    "AND FIAPIDTIPO= " + seguro.Tipo + "\n" +
                                    "AND FIAPIDSPOL= " + seguro.IdConsecutivo;
                    dbCnx.SetQuery(strSql);
                    respuesta = new Respuesta() { Ok = "SI", Mensaje = $"Se ha selecionado la poliza de la compra {seguro.IdCompra}" };
                }
                catch (Exception)
                {
                    respuesta = new Respuesta() { Ok = "NO", Mensaje = "No se puedo seleccionar seguro.", Objeto = null };
                }
                return respuesta;
            });  
        }
        internal Respuesta PostSubirDocumentos(long idCompra, int idTipoDocumento, string nombreDoc, string path)
        {
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                //path = path.Replace("C:\\CLON_APPS\\Apps\\Apps\\servicios\\wsApiSeat\\Compras\\Cupra\\Documentacion\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/Cupra/Documentacion/");
                //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\Documentacion\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/Documentacion/");
                path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Compras\\Documentacion\\", "http://10.5.16.17/wsApiSeat/Compras/Documentacion/");

                string strSql = "";
                strSql = @"INSERT INTO PRODAPPS.APDDCPST
                    (
                    FIAPIDCOMP,      /*ID COMPRA   */
                    FIAPIDTIPO,      /*ID TIPO DOC*/
                    FSAPDOCUME,      /*NOM. DOCUMENTO*/	
                    FSAPRUTDOC,      /*RUTA DOCUMENTO*/
                    FIAPSTATUS,      /*ESTATUS       */  
                    USERCREAT,       /*USUARIO CREACION*/
                    DATECREAT,       /*FECHA CREACION   */
                    TIMECREAT,       /*HORA CREACION    */
                    PROGCREAT       /*PROGRAMA CREACION*/

                   ) VALUES(" +
                   idCompra + " , " +
                   idTipoDocumento + " , " +
                   "'" + nombreDoc + "'" + " , " +
                   "'" + path + "'" + " , " +
                     1 + " , " +
                "'APP' , " +
                "CURRENT DATE , " +
                "CURRENT TIME , " +
                "'APP'" + ")";
                dbCnx.SetQuery(strSql);



                return new Respuesta() { Ok = "SI", Mensaje = "Su documentación se subió correctamente." };
            }
            catch (Exception _exc)
            {
                throw _exc;
            }
        }

        internal Respuesta PostSubirFoto(long idCuenta, string path)
        {
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                //path = path.Replace("C:\\CLON_APPS\\Apps\\Apps\\servicios\\wsApiSeat\\Compras\\Cupra\\Documentacion\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/Cupra/Documentacion/");
                //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Cuentas\\FotoPerfil\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Cuentas/FotoPerfil/");
                path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Cuentas\\FotoPerfil\\", "http://10.5.16.17/wsApiSeat/Cuentas/FotoPerfil/");

                string strSql = "";
                strSql = @"UPDATE PRODAPPS.APCCTAST " +
                            "SET FSAPRUTFOT = " + "'" + path + "'" +
                            ",PROGCREAT = 'APP' " +
                            ",USERUPDAT='APP' " +
                            ",DATEUPDAT = CURRENT DATE " +
                            ",TIMEUPDAT = CURRENT TIME " +
                            ",PROGUPDAT = 'APP' " +
                            " WHERE FIAPIDCUEN = " + idCuenta +
                            " AND FIAPSTATUS = 1";

                dbCnx.SetQuery(strSql);

                return new Respuesta() { Ok = "SI", Mensaje = "Se guardó correctamente la foto de perfil." };
            }
            catch (Exception _exc)
            {
                throw _exc;
            }
        }

        internal Respuesta PostSubirPolizaSmartIT(long IdCompra, int consecutivo, int TipoPoliza, string NombreArchivo, long cuenta, string NombreAseguradora, string path, string Total, string Cobertura)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string strSql = "";
            path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Resources\\Polizas\\", "http://10.5.16.17/wsApiSeat/Resources/Polizas/");
            strSql = @"INSERT INTO PRODAPPS.APEDSPOL
                    (
                       FIAPIDSPOL   /*CONSECUTIVO*/
                      ,FIAPIDCOMP   /*ID COMPRA*/
                      ,FIAPIDCUEN   /*ID CUENTA*/
                      ,FSAPNOASEG   /*NOMBRE DE ASEGURADORA*/
                      ,FSAPCOBERT   /*COBERTURA*/
                      ,FDAPCANTID   /*TOTAL*/
                      ,FIAPIDTIPO   /*TIPO*/
                      ,FSAPDOCUME   /*NOMBRE DOCUMENTO*/
                      ,FSAPRUTDOC   /*RUTA*/
                      ,FIAPSELECT   /*SELECCIONADO*/
                      ,FIAPSTATUS   /*ESTATUS*/
                      ,USERCREAT    /*USUARIO DE CREACION*/
                      ,DATECREAT    /*FECHA DE CREACION*/
                      ,TIMECREAT    /*HORA DE CREACION*/
                      ,PROGCREAT    /*PROGRAMA DE CREACION*/

                   ) VALUES(+ " + consecutivo +", " +
                   IdCompra + " , " +
                   cuenta + ",'" +
                   NombreAseguradora + "','" +
                   Cobertura + "'," +
                   Math.Round(Convert.ToDecimal(Total),2)  + "," +
                   "" + TipoPoliza + " , " +
                   "'" + NombreArchivo + " '," + 
                   "'" + path + "'" + " , " +
                     0 + " , " +
                     1 + " , " +
                "'APP' , " +
                "CURRENT DATE , " +
                "CURRENT TIME , " +
                "'APP'" + ")";
            dbCnx.SetQuery(strSql);
            return new Respuesta() { Ok = "SI", Mensaje = "Su poliza subió con éxito." };
            
        }

        internal Respuesta UpdateRegisterSmartIT(long IdCompra, int consecutivo, int TipoPoliza, string NombreArchivo, long cuenta, string NombreAseguradora, string path, string Total, string Cobertura)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string strSql = "";
            path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Resources\\Polizas\\", "http://10.5.16.17/wsApiSeat/Resources/Polizas/");
            strSql = @"UPDATE PRODAPPS.APEDSPOL SET FSAPNOASEG= '"+ NombreAseguradora + "',FSAPCOBERT= '"+ Cobertura + "',FDAPCANTID= " + Math.Round(Convert.ToDecimal(Total),2) + ",FSAPRUTDOC= '"+ path + "',FIAPSTATUS= "+ 1 + ", USERUPDAT= " + "'APPS'" + ",DATEUPDAT= " + "CURRENT_DATE" + ",TIMEUPDAT=" + "CURRENT_TIME"
                    + " WHERE FIAPIDCOMP= " + IdCompra + " AND FIAPIDCUEN= " + cuenta + " AND FIAPIDTIPO =" + TipoPoliza + " AND  FIAPIDSPOL=" + consecutivo;
            dbCnx.SetQuery(strSql);
            return new Respuesta() { Ok = "SI", Mensaje = "La poliza se reemplazó correctamente" };
        }

        internal Respuesta PostSubirComprobantePagos(long idCompra, int consecutivo, int idTipoDocumento, string nombreDoc, string path)
        {
            try
            {
                DVADB.DB2 dbCnx = new DVADB.DB2();
                DVAConstants.Constants constantes = new DVAConstants.Constants();

                path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Compras\\ComprobantePagos\\", "http://10.5.16.17/wsApiSeat/Compras/ComprobantePagos/");
                //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\ComprobantePagos\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/ComprobantePagos/");
            

                string strSql = "";
                strSql = @"INSERT INTO PRODAPPS.APDCMPST
                    (
                       FIAPIDCONS   /*CONSECUTIVO*/
                      ,FIAPIDCOMP   /*ID COMPRA*/
                      ,FIAPIDTIPO   /*ID TIPO*/
                      ,FSAPDOCUME   /*DOCUMENTO*/
                      ,FSAPRUTDOC   /*RUTA DOCUMENTO*/
                      ,FIAPSTATUS   /*ESTATUS*/
                      ,USERCREAT    /*USUARIO DE CREACION*/
                      ,DATECREAT    /*FECHA DE CREACION*/
                      ,TIMECREAT    /*HORA DE CREACION*/
                      ,PROGCREAT    /*PROGRAMA DE CREACION*/

                   ) VALUES(" +
                   consecutivo + " , " +
                   idCompra + " , " +
                   idTipoDocumento + " , " +
                   "'" + nombreDoc + "'" + " , " +
                   "'" + path + "'" + " , " +
                     1 + " , " +
                "'APP' , " +
                "CURRENT DATE , " +
                "CURRENT TIME , " +
                "'APP'" + ")";
                dbCnx.SetQuery(strSql);

                return new Respuesta() { Ok = "SI", Mensaje = "Su pago se subió correctamente." };
            }
            catch (Exception _exc)
            {
                throw _exc;
            }
        }

        internal Respuesta PostSubirDocumentoComunicacionAgencia(long idCompra, int idConsecutivo, int idTipoDocumento, string nombreDoc, string path)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();

                //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\ComunicacionAgencia\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/ComunicacionAgencia/");
                path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Compras\\ComunicacionAgencia\\", "http://10.5.16.17/wsApiSeat/Compras/ComunicacionAgencia/");
                // C:\inetpub\wwwroot\wsApiSeat\Compras\Cupra\Documentacion\33333_3_20201030.pdf

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string strSql = "";
                strSql = @"INSERT INTO PRODAPPS.APDDCSST
                    (
                       
                       FIAPIDCOMP   /*ID COMPRA*/
                      ,FIAPIDCONS  /*ID CONSECUTIVO*/
                      ,FIAPIDTIPO   /*ID TIPO*/
                      ,FSAPDOCUME   /*DOCUMENTO*/
                      ,FSAPRUTDOC   /*RUTA DOCUMENTO*/
                      ,FIAPSTATUS   /*ESTATUS*/
                      ,USERCREAT    /*USUARIO DE CREACION*/
                      ,DATECREAT    /*FECHA DE CREACION*/
                      ,TIMECREAT    /*HORA DE CREACION*/
                      ,PROGCREAT    /*PROGRAMA DE CREACION*/

                   ) VALUES(" +

                   idCompra + " , " +
                   idConsecutivo + " , " +
                   idTipoDocumento + " , " +
                   "'" + nombreDoc + "'" + " , " +
                   "'" + path + "'" + " , " +
                     1 + " , " +
                "'APP' , " +
                "CURRENT DATE , " +
                "CURRENT TIME , " +
                "'APP'" + ")";
                dbCnx.SetQuery(strSql);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                return new Respuesta() { Ok = "SI", Mensaje = "Se subió su documento de forma satisfactoria." };
            }
            catch (Exception _exc)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                return new Respuesta() { Ok = "No", Mensaje = _exc.Message };
            }
        }

        internal Respuesta UpdateRegister(long idCompra, int idTipoDocumento, string Documento, string path)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            //Path = Path.Replace("C:\\CLON_APPS\\Apps\\Apps\\servicios\\wsApiSeat\\Compras\\Cupra\\Documentacion\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/Cupra/Documentacion/");
            path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Compras\\Documentacion\\", "http://10.5.16.17/wsApiSeat/Compras/Documentacion/");
            //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\Documentacion\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/Documentacion/");

            string strSql = "";
            strSql = @"UPDATE PRODAPPS.APDDCPST SET FSAPDOCUME = '" + Documento.Trim() + "', FSAPRUTDOC ='" + path + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCOMP = " + idCompra + " AND FIAPIDTIPO = " + idTipoDocumento;
            dbCnx.SetQuery(strSql);

            return new Respuesta() { Ok = "SI", Mensaje = "Los documentos se remplazaron correctamente" };

        }

        internal Respuesta UpdateDocContactoAgencia(long idCompra, int idConsecutivo, int idTipoDocumento, string Documento, string path)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                DVAConstants.Constants constantes = new DVAConstants.Constants();
                path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Compras\\ComunicacionAgencia\\", "http://10.5.16.17/wsApiSeat/Compras/ComunicacionAgencia/");
                //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\ComunicacionAgencia\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/ComunicacionAgencia/");

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();


                string strSql = "";
                strSql = @"UPDATE PRODAPPS.APDDCSST SET FSAPDOCUME = '" + Documento.Trim() + "', FSAPRUTDOC ='" + path + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCOMP = " + idCompra + " AND FIAPIDTIPO = " + idTipoDocumento + " AND FIAPIDCONS = " + idConsecutivo + " AND FIAPSTATUS = 1";
                dbCnx.SetQuery(strSql);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();


                return new Respuesta() { Ok = "SI", Mensaje = "Los documentos se remplazaron correctamente." };
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                return new Respuesta() { Ok = "NO", Mensaje = "No se pudieron subir los archivo." };
            }

        }

        internal Respuesta UpdateRegisterFotoPerfil(long idCuenta, string path)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Cuentas\\FotoPerfil\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Cuentas/FotoPerfil/");
            //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Cuentas\\FotoPerfil\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Cuentas/FotoPerfil/");

            string strSql = "";
            strSql = @"UPDATE PRODAPPS.APCCTAST " +
                        "SET FSAPRUTFOT = " + "'" + path + "'" +
                        ",PROGCREAT = 'APP' " +
                        ",USERUPDAT='APP' " +
                        ",DATEUPDAT = CURRENT DATE " +
                        ",TIMEUPDAT = CURRENT TIME " +
                        ",PROGUPDAT = 'APP' " +
                        " WHERE FIAPIDCUEN = " + idCuenta +
                        " AND FIAPSTATUS = 1";

            dbCnx.SetQuery(strSql);

            return new Respuesta() { Ok = "SI", Mensaje = "Se remplazó la foto de perfil correctamente." };

        }

        internal Respuesta UpdateRegisterComprobantePagos(long idCompra, int consecutivo, int idTipoDocumento, string Documento, string path)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            path = path.Replace("C:\\Compartida\\Roberto Perez\\Proyecto seat\\Servicio Seat\\wsApiSeat\\Compras\\ComprobantePagos\\", "http://10.5.16.17/wsApiSeat/Compras/ComprobantePagos/");
            //path = path.Replace("C:\\inetpub\\wwwroot\\wsApiSeat\\Compras\\ComprobantePagos\\", "http://ws-smartit.divisionautomotriz.com/wsApiSeat/Compras/ComprobantePagos/");

            string strSql = "";
            strSql = @"UPDATE PRODAPPS.APDCMPST SET FSAPDOCUME = '" + Documento.Trim() + "', FSAPRUTDOC ='" + path + "', FIAPSTATUS = 1, USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE , TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCOMP = " + idCompra + " AND FIAPIDTIPO = " + idTipoDocumento + " AND  FIAPIDCONS=" + consecutivo;
            dbCnx.SetQuery(strSql);

            return new Respuesta() { Ok = "SI", Mensaje = "El comprobante de pago se reemplazó correctamente" };

        }

        public Respuesta PostRegistraClienteCuentaPedido(CompraAutoNuevo compra)
        {
            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            string jsonPedido = string.Empty;
            #region varialbles globales
            long idCliente = 0;
            long idCuenta = 0;
            long idPedido = 0;
            #endregion

            try
            {

                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                #region INSERT CLIENTE
                DVARegistraPersona.RegistraPersona persona = new DVARegistraPersona.RegistraPersona();
                persona.Nombre = compra.CuentaUsuario.Nombre.Trim();//pedido.Cuenta.Nombre;
                persona.ApellidoPaterno = compra.CuentaUsuario.ApellidoPaterno.Trim(); //pedido.CuentaUsuario.ApellidoPaterno;
                persona.ApellidoMaterno = compra.CuentaUsuario.ApellidoMaterno.Trim();//pedido.CuentaUsuario.ApellidoMaterno;                
                persona.NumeroCelular = compra.CuentaUsuario.TelefonoMovil.Trim();//pedido.CuentaUsuario.NumeroMobil;
                persona.Email = compra.CuentaUsuario.Correo.Trim();//pedido.CuentaUsuario.Correo;

                string json = JsonConvert.SerializeObject(persona);
                string valor = "36|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                DVAAutosystServerClasses.Seguridad.Seguridad seg = new DVAAutosystServerClasses.Seguridad.Seguridad();
                string token = seg.EncriptarCadena(valor);

                string url = "http://10.5.2.21:7070/wsRegistraPersona/api/Persona/registrarApps/valor/36/7244/1";
                url = url.Replace("valor", token);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result_cli = getResponse(streamReader.ReadToEnd());

                    bool isRegister = long.TryParse(result_cli, out idCliente);
                    if (isRegister)
                    {
                        compra.CuentaUsuario.IdPersona = idCliente.ToString();
                        compra.Pedido.IdPersona = idCliente.ToString();

                    }
                    else
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: ";
                        return respuesta;
                    }

                }

                #endregion

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                #region REGISTRA CUENTA

                string strSql = "";
                strSql = "SELECT FIAPIDCUEN Id FROM PRODAPPS.APCCTAST WHERE lower(TRIM(FSAPCORREO)) = '" + compra.CuentaUsuario.Correo.Trim().ToLower() + "'";
                DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];

                if (dtValidaCuenta.Rows.Count > 0)
                {
                    idCuenta = long.Parse(dtValidaCuenta.Rows[0]["Id"].ToString());
                    compra.IdCuenta = idCuenta.ToString();
                    compra.CuentaUsuario.IdCuenta = idCuenta.ToString();



                    strSql = "";
                    strSql = "UPDATE " + constantes.Ambiente + "APPS.APCCTAST ";
                    strSql += " SET FSAPTOKEN = '" + (!string.IsNullOrEmpty(compra.CuentaUsuario.Token.Trim()) ? compra.CuentaUsuario.Token.Trim() : "") + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCUEN =  " + idCuenta.ToString();
                    dbCnx.SetQuery(strSql);


                }
                else
                {

                    strSql = "";
                    strSql = "SELECT MAX(FIAPIDCUEN) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST ";

                    DataTable dt_cuenta = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt_cuenta.Rows.Count == 1)
                    {
                        idCuenta = long.Parse(dt_cuenta.Rows[0]["Id"].ToString());
                        compra.IdCuenta = idCuenta.ToString();
                        compra.CuentaUsuario.IdCuenta = idCuenta.ToString();

                        strSql = "";
                        strSql = "INSERT INTO " + constantes.Ambiente + "APPS.APCCTAST ";
                        strSql += "(FIAPIDCUEN,FIAPIDPERS , FSAPNOMBRE, FSAPAPEPAT, FSAPCORREO, FSAPTOKEN, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT) VALUES ";
                        strSql += "(" + idCuenta + "," + idCliente + ",'" + persona.Nombre.Trim().ToUpper() + "', '" + persona.ApellidoPaterno.Trim().ToUpper() + "', '" + persona.Email.Trim().ToLower() + "','" + (!string.IsNullOrEmpty(compra.CuentaUsuario.Token.Trim()) ? compra.CuentaUsuario.Token.Trim() : "") +
                            "', 1, 1, 'APP', CURRENT DATE, CURRENT TIME, 'APP')";
                        dbCnx.SetQuery(strSql);


                    }
                }

                #endregion

                #region  REGISTRO PEDIDO

                SolicitudPedido solicitudPedido = new SolicitudPedido();
                solicitudPedido.IdCliente = long.Parse(compra.Pedido.IdPersona);
                solicitudPedido.Serie = compra.Pedido.NumeroDeSerie;
                solicitudPedido.IdAgente = 999996;
                solicitudPedido.IdContacto = 0;
                solicitudPedido.IdTipoDeVenta = 52;
                solicitudPedido.Total = decimal.Parse(compra.Pedido.Total);
                jsonPedido = JsonConvert.SerializeObject(solicitudPedido);

                string valor_ped = "36|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                DVAAutosystServerClasses.Seguridad.Seguridad seg_ped = new DVAAutosystServerClasses.Seguridad.Seguridad();
                string token_ped = seg_ped.EncriptarCadena(valor_ped);


                string url_ped = "http://10.5.2.21:7070/wsRegistraPersona/api/Pedido/registrar/valor/36/7244/1";
                url_ped = url_ped.Replace("valor", token_ped);



                var httpWebRequest_ped = (HttpWebRequest)WebRequest.Create(url_ped);
                httpWebRequest_ped.ContentType = "application/json";
                httpWebRequest_ped.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest_ped.GetRequestStream()))
                {
                    streamWriter.Write(jsonPedido);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse_ped = (HttpWebResponse)httpWebRequest_ped.GetResponse();
                using (var streamReader = new StreamReader(httpResponse_ped.GetResponseStream()))
                {
                    var result_ped = getResponse(streamReader.ReadToEnd());
                    bool isRegisterPedido = long.TryParse(result_ped, out idPedido);
                    if (isRegisterPedido)
                    {
                        compra.Pedido.IdPedido = idPedido.ToString();
                    }
                    else
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: ";
                        respuesta.Objeto = null;
                        return respuesta;

                    }
                }

                #endregion

                #region REFERENCIA PAGO
                var client = new RestClient("http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/MiAuto.asmx/GenerarReferenciaBancaria?IdAgencia=" + compra.Pedido.IdAgencia + "&IdCliente=" + compra.Pedido.IdPersona + "&IdPedido=" + compra.Pedido.IdPedido);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                XmlAttributeDeserializer deserializer = new XmlAttributeDeserializer();
                var result = deserializer.Deserialize<RespuestReferencia>(response);


                if (result.Ok.Equals("SI"))
                {
                    compra.RutaReferenciaBancaria = result.Mensaje.Trim();
                }



                /*var data = JsonConvert.DeserializeObject(response.Content);
                var result = data["Ok"].Value<String>();
                if (result.Equals("SI"))
                {
                    compra.RutaReferenciaBancaria = data["Mensaje"].Value<String>().Trim();
                }*/
                #endregion

                #region REGISTRO COMPRA

                strSql = "";
                strSql = "SELECT COALESCE(MAX(FIAPIDCOMP),0) + 1 Id ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APECMPST ";

                DataTable dtc = dbCnx.GetDataSet(strSql).Tables[0];
                if (dtc.Rows.Count == 1)
                {
                    int idCompra = 0;
                    bool isCorrect = int.TryParse(dtc.Rows[0]["Id"].ToString(), out idCompra);
                    if (isCorrect)
                    {
                        compra.IdCompra = idCompra.ToString();
                        compra.Folio = idCompra.ToString();
                    }
                    else
                    {
                        respuesta.Ok = "NO";
                        respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde: ";
                        respuesta.Objeto = null;
                        return respuesta;

                    }


                    strSql = "";
                    strSql = @"INSERT INTO PRODAPPS.APECMPST
                            (
                            FIAPIDCOMP,      /*ID COMPRA   */
                            FIAPFOLCOM,     /* FOLIO COMPRA*/
                            FIAPIDCUEN,      /*ID CUENTA   */
                            FFAPFECCOM,      /*FECHA COMPRA*/
                            FHAPHORCOM,      /*HORA COMPRA */
                            FDAPSUBTOT,      /*SUBTOTAL */
                            FDAPDESCUE,      /*DESCUENTO*/
                            FDAPIVA,         /*IVA      */
                            FDAPTOTAL,      /* TOTAL*/    
                            FIAPIDESTA,      /*ID ESTADO*/
                            FIAPSTATUS,      /*ESTATUS     */     
                            USERCREAT,       /*USUARIO CREACION */
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/) VALUES(" +
                    compra.IdCompra + " , " +
                    compra.Folio + " , " +
                    compra.IdCuenta + " , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    compra.Subtotal + " , " +
                    0 + " , " + //DESCUENTO
                    compra.Iva + " , " +
                    compra.Total + " , " +
                    1 + " , " + //ID ESTADO
                    1 + " , " +
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";
                    dbCnx.SetQuery(strSql);

                }

                #endregion

                #region REGISTRO PEDIDOP APP AUTO              

                strSql = "";
                strSql = @"INSERT INTO PRODAPPS.APEPANST
                            (
                            FIAPIDCOMP,     /* ID COMPRA    */
                            FIAPIDCIAU,      /*ID CIA. UNICA*/
                            FIAPIDPEDI,      /*ID PEDIDO    */
                            FFAPFECPED,      /*FECHA PEDIDO */
                            FHAPHORPED,      /*HORA PEDIDO  */
                            FIAPIDPERS,      /*ID PERSONA   */
                            FIAPIDVEHI,     /*ID VEHICULO  */
                            FIAPIDINVE,      /*ID INVENTARIO*/


                            FSAPMODELO,
                            FSAPVERSIO,
                            FSAPTRANSM,      /*TRANSMISION*/ 
                            FSAPCOLEXT,
                            FSAPNUMINV,

                            FSAPNUMSER,      /*NUMERO SERIE*/ 
                            FDAPSUBTOT,      /*SUBTOTAL     */
                            FDAPDESCUE,      /*DESCUENTO      */ 
                            FDAPIVA,         /*IVA             */
                            FDAPTOTAL,       /*TOTAL      */     
                            FIAPCOTSEG,    /*COTIZAR SEGURO*/
                            FIAPSTATUS,      /*ESTATUS       */  
                            USERCREAT,       /*USUARIO CREACION*/
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/
                            ) VALUES(" +
                compra.IdCompra + "," +
                36 + "," +
                compra.Pedido.IdPedido + "," +
                "CURRENT DATE," +
                "CURRENT TIME," +
                compra.Pedido.IdPersona + "," +
                compra.Pedido.IdVehiculo + "," +
                compra.Pedido.IdInventario + ",'" +
                compra.Pedido.Modelo + "','" +
                compra.Pedido.Version + "','" +
                compra.Pedido.Transmision.ToString().Trim() + "','" +
                compra.Pedido.ColorExterior + "'," +
                "'" + compra.Pedido.NumeroInventario + "'" + "," +
                "'" + compra.Pedido.NumeroDeSerie + "'" + "," +
                compra.Pedido.Subtotal + ",0," +
                compra.Pedido.Iva + "," +
                compra.Pedido.Total + "," +
                compra.Pedido.CotizarSeguro + "," +
                1 + " , " +
                "'APP' , " +
                "CURRENT DATE , " +
                "CURRENT TIME , " +
                "'APP'" + ")";

                dbCnx.SetQuery(strSql);


                #endregion

                #region REGISTRO DETALLE PEDIDO ACCESORIOS

                List<AccesoriosUOtros> lstAccesorios = new List<AccesoriosUOtros>();
                if (compra.AccesoriosOtros != null)
                {
                    foreach (var accesorios in compra.AccesoriosOtros)
                    {
                        strSql = "";
                        strSql = "SELECT COALESCE(MAX(FIAPIDCONS),0) + 1 Id ";
                        strSql += "FROM " + constantes.Ambiente + "APPS.APDPANST WHERE FIAPIDCIAU = 36 AND FIAPIDPEDI = " + compra.Pedido.IdPedido;

                        DataTable dtdp = dbCnx.GetDataSet(strSql).Tables[0];

                        if (dtdp.Rows.Count == 1)
                        {

                            int idOut = 0;
                            bool isCorrect = int.TryParse(dtdp.Rows[0]["Id"].ToString(), out idOut);
                            if (isCorrect)
                            {
                                accesorios.Id = idOut.ToString();
                            }

                            strSql = "";
                            strSql = @"INSERT INTO PRODAPPS.APDPANST
                                (
                                FIAPIDCIAU,      /*ID CIA. UNICA */
                                FIAPIDPEDI,      /*ID PEDIDO     */
                                FIAPIDCONS,      /*ID CONSECUTIVO*/
                                FSAPCONCEP,      /*CONCEPTO      */
                                FDAPSUBTOT,      /*SUBTOTAL      */
                                FDAPDESCUE,      /*DESCUENTO       */
                                FDAPIVA,         /*IVA             */
                                FDAPTOTAL,       /*TOTAL      */     
                                FIAPSTATUS,     /*ESTATUS       */  
                                USERCREAT,       /*USUARIO CREACION*/
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES 
                                (" +
                                    36 + " , " +
                                    compra.Pedido.IdPedido + " , " +
                                    accesorios.Id + " , " +
                                    "'" + accesorios.Concepto.Replace("'", "") + "'" + " , " +
                                    accesorios.Subtotal + " , " +
                                    0 + " , " + // DESCUENTO
                                    accesorios.Iva + " , " +
                                    accesorios.Total + " , " +
                                    1 + " , " +
                                    "'APP' , " +
                                    "CURRENT DATE , " +
                                    "CURRENT TIME , " +
                                    "'APP'" + ")";

                            dbCnx.SetQuery(strSql);

                        }

                    }

                }

                #endregion

                #region REGISTRO SEGUIMIENTO PEDIDO
                strSql = "";
                strSql = "SELECT COALESCE(MAX(FIAPIDSEGU),0) + 1 Id ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APDSGCST WHERE  FIAPIDCOMP = " + compra.IdCompra;

                DataTable dtdt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dtdt.Rows.Count == 1)
                {


                    int idOut = 0;
                    bool isCorrect = int.TryParse(dtdt.Rows[0]["Id"].ToString(), out idOut);

                    strSql = "";
                    strSql = @"INSERT INTO PRODAPPS.APDSGCST
                            (
                                FIAPIDCOMP,      /*ID COMPRA     */
                                FIAPIDSEGU,      /*ID SEGUIMIENTO*/
                                FSAPTITSEG,     /*TITULO SEGUIM */
                                FIAPIDESTA,      /*ID ESTADO     */
                                FIAPSTATUS,      /*ESTATUS       */
                                USERCREAT,       /*USUARIO CREACION */
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES
                                (" +
                        compra.IdCompra + " , " +
                        idOut + " , " +
                        "'Regsitro pedido " + compra.Pedido.IdPedido + "' , " +
                        1 + " , " +
                        1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'" + ")";

                    dbCnx.SetQuery(strSql);
                }

                #endregion

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro con éxito.";
                respuesta.Objeto = JsonConvert.SerializeObject(compra);

            }
            catch (Exception _exc)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No fue posible registrar la solicitud, por favor intente de nuevo más tarde :" + _exc.Message;
                respuesta.Objeto = null;

            }
            if (respuesta.Ok.Equals("SI"))
            {
                if (!string.IsNullOrEmpty(compra.CuentaUsuario.Correo.Trim()))
                {
                    string subject = "Furia SEAT";
                    HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, compra.CuentaUsuario.Correo.Trim(), ObtenerStrHtmlCompra(compra));
                    Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreo.EnvioCorreo));
                    hilo.Start();
                }
            }
            return respuesta;
        }

        private string ObtenerStrHtmlCompra(CompraAutoNuevo compra)
        {
            string strHtml = "";

            try
            {
                string rutaArchivo = obtenerURLServidor() + "Resources/Email/" + "Compra.html"; // lee archivo web                
                                                                                                // strHtml = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\Resources\Cupra\compraCupraHtml.html");

                strHtml = leerArchivoWeb(rutaArchivo);

                strHtml = strHtml.Replace("[{FolioCompra}]", compra.Folio);
                strHtml = strHtml.Replace("[{FechaPedido}]", compra.Pedido.FechaPedido);
                strHtml = strHtml.Replace("[{NumeroPedido}]", compra.Pedido.IdPedido);
                strHtml = strHtml.Replace("[{HoraPedido}]", compra.Pedido.HoraPedido);

                strHtml = strHtml.Replace("[{Modelo}]", compra.Pedido.Modelo);
                strHtml = strHtml.Replace("[{Version}]", compra.Pedido.Version);
                strHtml = strHtml.Replace("[{Color}]", compra.Pedido.ColorExterior);
                strHtml = strHtml.Replace("[{NumeroInventario}]", compra.Pedido.NumeroInventario);
                strHtml = strHtml.Replace("[{SubTotalPedido}]", compra.Pedido.Subtotal);

                if (compra.AccesoriosOtros == null)
                {
                    compra.AccesoriosOtros = new List<AccesoriosUOtros>();
                }
                strHtml = strHtml.Replace("[{Cantidad}]", compra.AccesoriosOtros.Count > 0 ? "1" : "");
                strHtml = strHtml.Replace("[{Concepto}]", compra.AccesoriosOtros.Count > 0 ? compra.AccesoriosOtros.FirstOrDefault().Concepto : "");
                //strHtml = strHtml.Replace("[{SubTotalAccesorio}]", compra.AccesoriosOtros.Count > 0 ? compra.AccesoriosOtros.FirstOrDefault().Subtotal : "");
                strHtml = strHtml.Replace("[{SubTotalAccesorio}]", compra.AccesoriosOtros.Count > 0 ? compra.AccesoriosOtros.FirstOrDefault().Subtotal : "");

                //strHtml = strHtml.Replace("[{Cantidad}]", "");
                //strHtml = strHtml.Replace("[{Concepto}]", "");
                //strHtml = strHtml.Replace("[{SubTotalAccesorio}]", "");

                strHtml = strHtml.Replace("[{SubTotalCompra}]", compra.Subtotal);
                strHtml = strHtml.Replace("[{IvaCompra}]", compra.Iva);
                strHtml = strHtml.Replace("[{TotalCompra}]", compra.Total);
            }
            catch (Exception ex)
            {

            }

            return strHtml;
        }

        public string obtenerURLServidor()
        {

            HttpRequest request = HttpContext.Current.Request;

            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";

            return baseUrl;
        }

        public string leerArchivoWeb(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string output = reader.ReadToEnd();
            response.Close();

            return output;
        }

        public Respuesta PostRegistraClienteCuenta(Cuenta cuenta)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "No fue posible registrar la solicitud.";

            try
            {
                #region Registra CLIENTE
                DVARegistraPersona.RegistraPersona persona = new DVARegistraPersona.RegistraPersona();
                persona.Nombre = cuenta.Nombre.Trim();//pedido.Cuenta.Nombre;
                persona.ApellidoPaterno = cuenta.ApellidoPaterno.Trim(); //pedido.Cuenta.ApellidoPaterno;
                persona.ApellidoMaterno = cuenta.ApellidoMaterno.Trim();//pedido.Cuenta.ApellidoMaterno;                
                persona.NumeroCelular = cuenta.TelefonoMovil.Trim();//pedido.Cuenta.NumeroMobil;
                persona.Email = cuenta.Correo.Trim();//pedido.Cuenta.Correo;

                string json = JsonConvert.SerializeObject(persona);
                string valor = "36|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                DVAAutosystServerClasses.Seguridad.Seguridad seg = new DVAAutosystServerClasses.Seguridad.Seguridad();
                string token = seg.EncriptarCadena(valor);

                string url = "http://10.5.2.21:7070/wsRegistraPersona/api/Persona/registrarApps/valor/36/7244/1";
                url = url.Replace("valor", token);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result_cli = getResponse(streamReader.ReadToEnd());

                    long idCliente = 0;
                    bool isRegister = long.TryParse(result_cli, out idCliente);
                    if (isRegister)
                    {
                        cuenta.IdPersona = idCliente.ToString();
                    }
                }
                #endregion

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                #region REGISTRA CUENTA

                string strSql = "";
                strSql = "SELECT FIAPIDCUEN Id FROM PRODAPPS.APCCTAST WHERE lower(TRIM(FSAPCORREO)) = '" + cuenta.Correo.Trim().ToLower() + "'";
                DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];

                if (dtValidaCuenta.Rows.Count > 0)
                {
                    long idCuenta = 0;
                    idCuenta = long.Parse(dtValidaCuenta.Rows[0]["Id"].ToString());
                    cuenta.IdCuenta = idCuenta.ToString();

                    strSql = "";
                    strSql = "UPDATE " + constantes.Ambiente + "APPS.APCCTAST ";
                    strSql += " SET FSAPTOKEN = '" + (!string.IsNullOrEmpty(cuenta.Token.Trim()) ? cuenta.Token.Trim() : "") + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCUEN =  " + idCuenta.ToString();
                    dbCnx.SetQuery(strSql);
                }
                else
                {

                    strSql = "";
                    strSql = "SELECT MAX(FIAPIDCUEN) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST ";

                    DataTable dt_cuenta = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt_cuenta.Rows.Count == 1)
                    {
                        long idCuenta = 0;
                        idCuenta = long.Parse(dt_cuenta.Rows[0]["Id"].ToString());
                        cuenta.IdCuenta = idCuenta.ToString();

                        strSql = "";
                        strSql = "INSERT INTO " + constantes.Ambiente + "APPS.APCCTAST ";
                        strSql += "(FIAPIDCUEN,FIAPIDPERS , FSAPNOMBRE, FSAPAPEPAT, FSAPCORREO, FSAPTOKEN, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT) VALUES ";
                        strSql += "(" + idCuenta + "," + cuenta.IdPersona + ",'" + persona.Nombre.Trim().ToUpper() + "', '" + persona.ApellidoPaterno.Trim().ToUpper() + "', '" + persona.Email.Trim().ToLower() + "','" + (!string.IsNullOrEmpty(cuenta.Token.Trim()) ? cuenta.Token.Trim() : "") +
                            "', 1, 1, 'APP', CURRENT DATE, CURRENT TIME, 'APP')";
                        dbCnx.SetQuery(strSql);

                    }
                }

                #endregion

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se registró correctamente.";
                respuesta.Objeto = JsonConvert.SerializeObject(cuenta);
            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No fue posible registrar la solicitud.";
            }

            return respuesta;
        }

        internal Respuesta ActualizarCotizarSeguroEnPedido(long idCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            Respuesta respuesta = new Respuesta();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "No fue posible registrar la solicitud.";
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                string strSql = "";
                strSql = @"UPDATE PRODAPPS.APEPANST SET
                            FIAPCOTSEG = 1,     
                            USERUPDAT = 'APP',
                            DATEUPDAT = CURRENT DATE,
                            TIMEUPDAT = CURRENT TIME,
                            PROGUPDAT = 'APP'
                            WHERE FIAPIDCOMP = " + idCompra;

                dbCnx.SetQuery(strSql);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se registró correctamente.";
            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
            }
            return respuesta;
        }

        public RespuestaTest<CompraAutoNuevo> PostRegistraPedido(CompraAutoNuevo compra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            RespuestaTest<CompraAutoNuevo> respuesta = new RespuestaTest<CompraAutoNuevo>();
            respuesta.Ok = "NO";
            //respuesta.Mensaje = "No fue posible registrar la solicitud debido a que existe una compra activa.";
            respuesta.Mensaje = "";

            //#region Compra activa 
            //string strSql = "";
            //strSql += "select COUNT(*) Cuenta ";
            //strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
            //strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
            //strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
            //strSql += "WHERE ape.FIAPSTATUS = 1 AND ape.FIAPIDESTA IN (1,2,3,4,5,6,7,8,9,10,11,12,13) AND ape.FIAPIDCUEN = " + compra.IdCuenta;

            //DataTable dtCount = dbCnx.GetDataSet(strSql).Tables[0];
            //#endregion

            //if (dtCount.Rows.Count == 0)
            //{
            //RespuestaTest<CompraAutoNuevo> resp = new RespuestaTest<CompraAutoNuevo>();

            string jsonPedido = string.Empty;
            string strSql = "";

            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                #region APARTADO

                strSql = "";
                strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 10, \t";
                strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                strSql += "WHERE FNAUTOAGE = " + compra.Pedido.IdAgencia.ToString().Trim() + " \t";
                strSql += "AND FIANIDINVE=" + compra.Pedido.IdInventario + "\t";
                strSql += "AND FIANSTATU = 1 \t";
                strSql += "AND FNAUTOEST = 50";

                dbCnx.SetQuery(strSql);

                #endregion


                #region  REGISTRO PEDIDO

                SolicitudPedido solicitudPedido = new SolicitudPedido();
                solicitudPedido.IdCliente = long.Parse(compra.Pedido.IdPersona);
                solicitudPedido.Serie = compra.Pedido.NumeroDeSerie;
                solicitudPedido.IdAgente = 999996;
                solicitudPedido.IdContacto = 0;
                solicitudPedido.IdTipoDeVenta = 52;
                solicitudPedido.Total = decimal.Parse(compra.Pedido.Total);
                jsonPedido = JsonConvert.SerializeObject(solicitudPedido);

                string valor_ped = compra.Pedido.IdAgencia.ToString().Trim() + "|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                DVAAutosystServerClasses.Seguridad.Seguridad seg_ped = new DVAAutosystServerClasses.Seguridad.Seguridad();
                string token_ped = seg_ped.EncriptarCadena(valor_ped);

                //string url_ped = "http://10.5.2.21:7070/wsRegistraPersona/api/Pedido/registrarconreferencia/valor/" + compra.Pedido.IdAgencia.ToString().Trim() + "/7244/1";  /// modificar
                string url_ped = "http://10.5.2.21:7071/wsRegistraPersonaTest/api/Pedido/registrarconreferencia/valor/" + compra.Pedido.IdAgencia.ToString().Trim() + "/7244/1";  /// modificar
                url_ped = url_ped.Replace("valor", token_ped);

                var httpWebRequest_ped = (HttpWebRequest)WebRequest.Create(url_ped);
                httpWebRequest_ped.ContentType = "application/json";
                httpWebRequest_ped.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest_ped.GetRequestStream()))
                {
                    streamWriter.Write(jsonPedido);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse_ped = (HttpWebResponse)httpWebRequest_ped.GetResponse();
                using (var streamReader = new StreamReader(httpResponse_ped.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    RespuestaPedido respuestaPedido = new RespuestaPedido();
                    string error = string.Empty;
                    if (result.Contains("IdPedido") && result.Contains("Ruta"))
                        respuestaPedido = JsonConvert.DeserializeObject<RespuestaPedido>(result);
                    else
                        error = JsonConvert.DeserializeObject<string>(result);

                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception();
                    }
                    else
                    {
                        compra.Pedido.IdPedido = respuestaPedido.IdPedido;
                        string ruta = respuestaPedido.Ruta.Replace("C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/");
                        compra.RutaReferenciaBancaria = ruta;
                    }
                }

                #endregion


                #region REGISTRO COMPRA

                strSql = "";
                strSql = "SELECT COALESCE(MAX(FIAPIDCOMP),0) + 1 Id ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APECMPST ";

                DataTable dtc = dbCnx.GetDataSet(strSql).Tables[0];
                if (dtc.Rows.Count == 1)
                {
                    int idCompra = 0;
                    bool isCorrect = int.TryParse(dtc.Rows[0]["Id"].ToString(), out idCompra);
                    if (isCorrect)
                    {
                        compra.IdCompra = idCompra.ToString();
                        compra.Folio = idCompra.ToString();
                    }
                    else
                    {
                        return respuesta;
                    }


                    strSql = "";
                    strSql = @"INSERT INTO PRODAPPS.APECMPST
                            (
                            FIAPIDCOMP,      /*ID COMPRA   */
                            FIAPFOLCOM,     /* FOLIO COMPRA*/
                            FIAPIDCUEN,      /*ID CUENTA   */
                            FFAPFECCOM,      /*FECHA COMPRA*/
                            FHAPHORCOM,      /*HORA COMPRA */
                            FDAPSUBTOT,      /*SUBTOTAL */
                            FDAPDESCUE,      /*DESCUENTO*/
                            FDAPIVA,         /*IVA      */
                            FDAPTOTAL,      /* TOTAL*/    
                            FSAPRUTRFB,     /*RUTA REFERENCIA BANCARIA*/
                            FIAPIDESTA,      /*ID ESTADO*/
                            FIAPIDPROC,     /*ID PROCESO*/ 
                            FIAPIDPASO,     /*ID PASO*/
                            FIAPSTATUS,      /*ESTATUS     */     
                            USERCREAT,       /*USUARIO CREACION */
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/) VALUES(" +
                    compra.IdCompra + " , " +
                    compra.Folio + " , " +
                    compra.IdCuenta + " , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    compra.Subtotal + " , " +
                    0 + " , " + //DESCUENTO
                    compra.Iva + " , " +
                    compra.Total + " , '" +
                    compra.RutaReferenciaBancaria + "' , " +
                    1 + " , " + //ID ESTADO
                    2 + " , " + //ID PROCESO
                    1 + " , " + //ID PASO
                    1 + " , " +  // ESTATUS
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";
                    dbCnx.SetQuery(strSql);

                }

                #endregion

                #region REGISTRO PEDIDOP APP AUTO              

                strSql = "";
                strSql = @"INSERT INTO PRODAPPS.APEPANST
                            (
                            FIAPIDCOMP,     /* ID COMPRA    */
                            FIAPIDCIAU,      /*ID CIA. UNICA*/
                            FIAPIDPEDI,      /*ID PEDIDO    */
                            FFAPFECPED,      /*FECHA PEDIDO */
                            FHAPHORPED,      /*HORA PEDIDO  */
                            FIAPIDPERS,      /*ID PERSONA   */
                            FIAPIDVEHI,     /*ID VEHICULO  */
                            FIAPIDINVE,      /*ID INVENTARIO*/


                            FSAPMODELO,
                            FSAPVERSIO,
                            FSAPTRANSM,      /*TRANSMISION*/ 
                            FSAPCOLEXT,
                            FSAPNUMINV,

                            FSAPNUMSER,      /*NUMERO SERIE*/ 
                            FDAPSUBTOT,      /*SUBTOTAL     */
                            FDAPDESCUE,      /*DESCUENTO      */ 
                            FDAPIVA,         /*IVA             */
                            FDAPTOTAL,       /*TOTAL      */     
                            FIAPCOTSEG,    /*COTIZAR SEGURO*/
                            FSAPRUTFOT,    /*RUTA FOTO*/
                            FIAPSTATUS,      /*ESTATUS       */  
                            USERCREAT,       /*USUARIO CREACION*/
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/
                            ) VALUES(" +
                compra.IdCompra + "," +
                compra.Pedido.IdAgencia.ToString().Trim() + "," +
                compra.Pedido.IdPedido + "," +
                "CURRENT DATE," +
                "CURRENT TIME," +
                compra.Pedido.IdPersona + "," +
                compra.Pedido.IdVehiculo + "," +
                compra.Pedido.IdInventario + ",'" +
                compra.Pedido.Modelo + "','" +
                compra.Pedido.Version + "','" +
                compra.Pedido.Transmision + "','" +
                compra.Pedido.ColorExterior + "'," +
                "'" + compra.Pedido.NumeroInventario + "'" + "," +
                "'" + compra.Pedido.NumeroDeSerie + "'" + "," +
                compra.Pedido.Subtotal + ",0," +
                compra.Pedido.Iva + "," +
                compra.Pedido.Total + "," +
                compra.Pedido.CotizarSeguro + "," +
                "'" + compra.Pedido.RutaFoto + "'" +
               "," + 1 + " , " +
                "'APP' , " +
                "CURRENT DATE , " +
                "CURRENT TIME , " +
                "'APP'" + ")";

                dbCnx.SetQuery(strSql);

                #endregion

                #region REGISTRO DETALLE PEDIDO ACCESORIOS

                List<AccesoriosUOtros> lstAccesorios = new List<AccesoriosUOtros>();
                if (compra.AccesoriosOtros != null)
                {
                    foreach (var accesorios in compra.AccesoriosOtros)
                    {
                        strSql = "";
                        strSql = "SELECT COALESCE(MAX(FIAPIDCONS),0) + 1 Id ";
                        strSql += "FROM " + constantes.Ambiente + "APPS.APDPANST WHERE FIAPIDCIAU = " + compra.Pedido.IdAgencia.ToString().Trim() + " AND FIAPIDPEDI = " + compra.Pedido.IdPedido;

                        DataTable dtdp = dbCnx.GetDataSet(strSql).Tables[0];

                        if (dtdp.Rows.Count == 1)
                        {

                            int idOut = 0;
                            bool isCorrect = int.TryParse(dtdp.Rows[0]["Id"].ToString(), out idOut);
                            if (isCorrect)
                            {
                                accesorios.Id = idOut.ToString();
                            }

                            strSql = "";
                            strSql = @"INSERT INTO PRODAPPS.APDPANST
                                (
                                FIAPIDCIAU,      /*ID CIA. UNICA */
                                FIAPIDPEDI,      /*ID PEDIDO     */
                                FIAPIDCONS,      /*ID CONSECUTIVO*/
                                FSAPCONCEP,      /*CONCEPTO      */
                                FDAPSUBTOT,      /*SUBTOTAL      */
                                FDAPDESCUE,      /*DESCUENTO       */
                                FDAPIVA,         /*IVA             */
                                FDAPTOTAL,       /*TOTAL      */    
                                FSAPRUTFOT,     /*RUTA FOTO*/
                                FIAPSTATUS,     /*ESTATUS       */  
                                USERCREAT,       /*USUARIO CREACION*/
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES 
                                (" +
                                    compra.Pedido.IdAgencia.ToString().Trim() + " , " +
                                    compra.Pedido.IdPedido + " , " +
                                    accesorios.Id + " , " +
                                    "'" + accesorios.Concepto.Replace("'", "") + "'" + " , " +
                                    accesorios.Subtotal + " , " +
                                    0 + " , " + // DESCUENTO
                                    accesorios.Iva + " , " +
                                    accesorios.Total + " , " +
                                    "'" + accesorios.RutaFoto + "'" + "," +
                                    1 + " , " +
                                    "'APP' , " +
                                    "CURRENT DATE , " +
                                    "CURRENT TIME , " +
                                    "'APP'" + ")";

                            dbCnx.SetQuery(strSql);

                        }

                    }

                }

                #endregion

                #region REGISTRO SEGUIMIENTO PEDIDO
                strSql = "";
                strSql = "SELECT COALESCE(MAX(FIAPIDSEGU),0) + 1 Id ";
                strSql += "FROM " + constantes.Ambiente + "APPS.APDSGCST WHERE  FIAPIDCOMP = " + compra.IdCompra;

                DataTable dtdt = dbCnx.GetDataSet(strSql).Tables[0];
                if (dtdt.Rows.Count == 1)
                {


                    int idOut = 0;
                    bool isCorrect = int.TryParse(dtdt.Rows[0]["Id"].ToString(), out idOut);

                    strSql = "";
                    strSql = @"INSERT INTO PRODAPPS.APDSGCST
                            (
                                FIAPIDCOMP,      /*ID COMPRA     */
                                FIAPIDSEGU,      /*ID SEGUIMIENTO*/
                                FSAPTITSEG,     /*TITULO SEGUIM */
                                FIAPIDESTA,      /*ID ESTADO     */
                                FIAPSTATUS,      /*ESTATUS       */
                                USERCREAT,       /*USUARIO CREACION */
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES
                                (" +
                        compra.IdCompra + " , " +
                        idOut + " , " +
                        "'Registro pedido " + compra.Pedido.IdPedido + "' , " +
                        1 + " , " +
                        1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'" + ")";

                    dbCnx.SetQuery(strSql);
                }

                #endregion

                #region Inserta en notificaciones

                string instruccion = "{\"Vista\":\"miCupra\",\"Parametros\":{\"IdCompra\":\"" + compra.IdCompra + "\"}}";

                string strINSql = ""; // se inserta en tabka de notficaciones
                strINSql += "INSERT INTO " + constantes.Ambiente + "APPS.APDNOTST ";
                strINSql += "(FIAPIDCUEN, FIAPIDNOTI, FFAPNOTIFI, FHAPNOTIFI, FSAPASUNTO, FSAPNOTIFI, FIAPAPLSEG, FIAPIDPREO, FIAPAPLENC, FIAPIDENPE, FSAPINSTRU, FIAPSTATUS, USERCREAT, PROGCREAT) ";
                strINSql += "VALUES ";
                strINSql += "(";
                strINSql += compra.IdCuenta + ",";
                strINSql += "(SELECT coalesce(MAX(FIAPIDNOTI),0)+1 ID FROM PRODAPPS.APDNOTST),";
                strINSql += "CURRENT DATE" + ",";
                strINSql += "CURRENT TIME" + ",";
                strINSql += "'" + "PEDIDO RECIBIDO" + "',";
                strINSql += "'" + "Reservamos tu CUPRA durante 1 hr. para que puedas realizar tu pago del apartado." + "',";
                strINSql += "0,";
                strINSql += "default,";
                strINSql += "1,";
                strINSql += "default,";
                strINSql += "'" + instruccion + "'" + ",";
                strINSql += "1,'APPS','APPS'";
                strINSql += ")";

                dbCnx.SetQuery(strINSql);

                #endregion

                #region Notificacion fija
                string token = "";

                strSql = "";
                strSql = "SELECT    * ";
                strSql += "FROM	" + constantes.Ambiente + "APPS.APCCTAST ";
                strSql += "WHERE FIAPSTATUS = 1 ";
                strSql += "AND FIAPIDCUEN = " + compra.IdCuenta;
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
                notification.title = "PEDIDO RECIBIDO";
                notification.body = "Reservamos tu CUPRA durante 1 hr. para que puedas realizar tu pago del apartado.";
                alerta.notification = notification;
                data data = new data();
                // data.CUPRA = "Prueba";
                data.CUPRA = "";
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

                #region CheckList

                string strSqlCh = "";

                strSqlCh += "INSERT INTO PRODAPPS.APDCKLST( ";
                strSqlCh += " FIAPIDCOMP, FIAPIDPROC, FIAPIDPCKL, FSAPDESCCK, FIAPSMARTI, ";
                strSqlCh += " FIAPAPPVIS, FIAPSISTEM, FIAPREALIZ, FIAPSTATUS, USERCREAT, ";
                strSqlCh += " DATECREAT, TIMECREAT, PROGCREAT)";
                strSqlCh += " SELECT " + compra.IdCompra + ", FIAPIDPROC, FIAPIDPCKL,  FSAPDESCCK, FIAPSMARTI, ";
                strSqlCh += " FIAPAPPVIS, FIAPSISTEM,  0, FIAPSTATUS, 'APP', ";
                strSqlCh += " CURRENT DATE, CURRENT TIME, 'APP' ";
                strSqlCh += " FROM PRODAPPS.APCCKLIS  WHERE FIAPSTATUS = 1";

                dbCnx.SetQuery(strSqlCh);


                string strActualizaCh = "";

                strActualizaCh += "UPDATE PRODAPPS.APDCKLST ";
                strActualizaCh += "SET FIAPREALIZ = 1, ";
                strActualizaCh += "PROGUPDAT = 'APP', USERUPDAT = 'APP', TIMEUPDAT = CURRENT TIME, DATEUPDAT = CURRENT DATE ";
                strActualizaCh += "WHERE FIAPIDCOMP = " + compra.IdCompra;
                strActualizaCh += " AND FIAPIDPCKL = 1";

                dbCnx.SetQuery(strActualizaCh);

                #endregion



                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Se registró correctamente.";
                respuesta.Objeto = compra;
            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No fue posible registrar la solicitud.";
                respuesta.Objeto = null;

                return respuesta;

            }
            //}


            if (respuesta.Ok.Equals("SI"))
            {
                if (!string.IsNullOrEmpty(compra.CuentaUsuario.Correo.Trim()))
                {
                    string subject = "SEAT";
                    HiloEnvioCorreo hiloEnvioCorreo = new HiloEnvioCorreo(subject, compra.CuentaUsuario.Correo.Trim(), ObtenerStrHtmlCompra(compra));
                    Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreo.EnvioCorreo));
                    hilo.Start();

                    subject = "Pedido Recibido";
                    HiloEnvioCorreoSoporte hiloEnvioCorreoBack = new HiloEnvioCorreoSoporte(subject, "jrperez@grupoautofin.com", "Se actualizó el proceso de compra con folio: " + compra.IdCompra + " al estado Pedido recibido", compra.IdCompra);
                    Thread hiloBack = new Thread(new ThreadStart(hiloEnvioCorreoBack.EnvioCorreoSoporte));
                    hiloBack.Start();
                }
                else
                {

                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "No se pudo enviar el correo, la cuenta no tiene correo asignado.";
                    respuesta.Objeto = null;
                }
            }

            return respuesta;
        }

        public string getResponse(string response)
        {
            var data = (JObject)JsonConvert.DeserializeObject(response);
            var result = data["Respuesta"].Value<String>();
            return result;
        }

        public string getRespuesta(string response)
        {
            var data = (JObject)JsonConvert.DeserializeObject(response);
            var result = data["Ok"].Value<String>();
            return result;
        }

        public RespuestaTest<CompraAutoNuevo> RegistraApartado(CompraAutoNuevo compra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();

            RespuestaTest<CompraAutoNuevo> respuesta = new RespuestaTest<CompraAutoNuevo>();
            respuesta.Ok = "NO";
            respuesta.Mensaje = "";

            string strSql = "";
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                long idCuenta = 0;
                #region validacionInventarioDisponible

                strSql = "";
                strSql += "SELECT FNAUTOEST FROM PRODAUT.ANCAUTOM ";
                strSql += "WHERE FNAUTOAGE = " + compra.Pedido.IdAgencia.ToString().Trim() + " ";
                strSql += "AND FIANIDINVE=" + compra.Pedido.IdInventario + "\t";
                strSql += "AND FIANSTATU = 1 \t";
                strSql += "AND FNAUTOEST in (6,10)";

                DataTable dtVa = dbCnx.GetDataSet(strSql).Tables[0];

                if (dtVa.Rows.Count == 0)
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "Inventario no disponible.";
                    respuesta.Objeto = null;

                    throw new Exception();
                }
                #endregion

                #region REGISTRA CUENTA

                strSql = "";
                strSql = "SELECT FIAPIDCUEN Id, FSAPNOMBRE Nombre, FSAPAPEPAT ApellidoPaterno, FSAPAPEMAT ApellidoMaterno, FIAPLADMOV Lada, FIAPNUMMOV numero FROM PRODAPPS.APCCTAST WHERE lower(TRIM(FSAPCORREO)) = '" + compra.CuentaUsuario.Correo.Trim().ToLower() + "'";
                DataTable dtValidaCuenta = dbCnx.GetDataSet(strSql).Tables[0];
                if (dtValidaCuenta.Rows.Count > 0)
                {
                    idCuenta = long.Parse(dtValidaCuenta.Rows[0]["Id"].ToString());
                    compra.IdCuenta = idCuenta.ToString();
                    compra.CuentaUsuario.IdCuenta = idCuenta.ToString();
                    compra.CuentaUsuario.Nombre = dtValidaCuenta.Rows[0]["Nombre"].ToString().ToUpper().Trim();
                    compra.CuentaUsuario.ApellidoPaterno = dtValidaCuenta.Rows[0]["ApellidoPaterno"].ToString().ToUpper().Trim();
                    compra.CuentaUsuario.ApellidoMaterno = dtValidaCuenta.Rows[0]["ApellidoMaterno"].ToString().ToUpper().Trim();
                    compra.CuentaUsuario.LadaMovil = dtValidaCuenta.Rows[0]["Lada"].ToString().Trim();
                    compra.CuentaUsuario.TelefonoMovil = dtValidaCuenta.Rows[0]["numero"].ToString().Trim();

                    strSql = "";
                    strSql = "UPDATE " + constantes.Ambiente + "APPS.APCCTAST ";
                    strSql += " SET FIAPSTATUS = 1, FSAPTOKEN = '" + (!string.IsNullOrEmpty(compra.CuentaUsuario.Token.Trim()) ? compra.CuentaUsuario.Token.Trim() : "") + "', USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' WHERE FIAPIDCUEN =  " + idCuenta.ToString();
                    dbCnx.SetQuery(strSql);

                }
                else
                {
                    strSql = "";
                    strSql = "SELECT MAX(FIAPIDCUEN) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APCCTAST ";

                    DataTable dt_cuenta = dbCnx.GetDataSet(strSql).Tables[0];

                    if (dt_cuenta.Rows.Count == 1)
                    {
                        idCuenta = long.Parse(dt_cuenta.Rows[0]["Id"].ToString());
                        compra.IdCuenta = idCuenta.ToString();
                        compra.CuentaUsuario.IdCuenta = idCuenta.ToString();

                        strSql = "";
                        strSql = "INSERT INTO " + constantes.Ambiente + "APPS.APCCTAST ";
                        strSql += "(FIAPIDCUEN, FSAPNOMBRE, FSAPAPEPAT, FSAPCORREO,";
                        strSql += "FSAPAPEMAT,FIAPLADMOV, FIAPNUMMOV,";
                        strSql += "FSAPTOKEN, FIAPIDESTA, FIAPSTATUS, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT) VALUES ";
                        strSql += "(" + idCuenta + "," + "'" + compra.CuentaUsuario.Nombre.Trim().ToUpper() + "'" + "," + "'" + compra.CuentaUsuario.ApellidoPaterno.Trim().ToUpper() + "'" + ",";
                        strSql += "'" + compra.CuentaUsuario.Correo.Trim().ToLower() + "'" + "," + "'" + (!string.IsNullOrEmpty(compra.CuentaUsuario.ApellidoMaterno.Trim().ToUpper()) ? compra.CuentaUsuario.ApellidoMaterno.Trim().ToUpper() : "") + "'" + "," + "52" + "," + (!string.IsNullOrEmpty(compra.CuentaUsuario.TelefonoMovil.Trim()) ? compra.CuentaUsuario.TelefonoMovil.Trim() : "") + "," + "'" + (!string.IsNullOrEmpty(compra.CuentaUsuario.Token.Trim()) ? compra.CuentaUsuario.Token.Trim() : "") + "'" + "," + "1, 1, 'APP', CURRENT DATE, CURRENT TIME, 'APP')";
                        dbCnx.SetQuery(strSql);
                    }
                }

                #endregion

                #region COMPRA ACTIVA 
                strSql = "";
                strSql += "select FIAPIDCOMP Cuenta ";
                strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
                strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
                strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
                strSql += "WHERE ape.FIAPSTATUS = 1 AND ape.FIAPIDESTA IN (1,2,3,4,5,6,7,8,9,10,11,12,13,14,16,17,18,19,20,21,22,23) AND ape.FIAPIDCUEN = " + compra.IdCuenta;

                DataTable dtCount = dbCnx.GetDataSet(strSql).Tables[0];

                #endregion

                if (dtCount.Rows.Count == 0)
                {
                    #region REGISTRO COMPRA

                    strSql = "";
                    strSql = "SELECT COALESCE(MAX(FIAPIDCOMP),0) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APECMPST ";

                    DataTable dtc = dbCnx.GetDataSet(strSql).Tables[0];
                    if (dtc.Rows.Count == 1)
                    {
                        int idCompra = 0;
                        bool isCorrect = int.TryParse(dtc.Rows[0]["Id"].ToString(), out idCompra);
                        if (isCorrect)
                        {
                            compra.IdCompra = idCompra.ToString();
                            compra.Folio = idCompra.ToString();
                        }
                        else
                        {
                            respuesta.Mensaje = "No fue posible registrar la compra.";
                            return respuesta;
                        }

                        strSql = "";
                        strSql = @"INSERT INTO PRODAPPS.APECMPST
                            (
                            FIAPIDCOMP,      /*ID COMPRA   */
                            FIAPFOLCOM,     /* FOLIO COMPRA*/
                            FIAPIDCUEN,      /*ID CUENTA   */
                            FFAPFECCOM,      /*FECHA COMPRA*/
                            FHAPHORCOM,      /*HORA COMPRA */
                            FDAPSUBTOT,      /*SUBTOTAL */
                            FDAPDESCUE,      /*DESCUENTO*/
                            FDAPIVA,         /*IVA      */
                            FDAPTOTAL,      /* TOTAL*/    
                            FIAPIDESTA,      /*ID ESTADO*/
                            FIAPIDPROC,     /*ID PROCESO*/ 
                            FIAPIDPASO,     /*ID PASO*/
                            FIAPSTATUS,      /*ESTATUS     */     
                            USERCREAT,       /*USUARIO CREACION */
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/) VALUES(" +
                        compra.IdCompra + " , " +
                        compra.Folio + " , " +
                        compra.IdCuenta + " , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        compra.Subtotal + " , " +
                        0 + " , " + //DESCUENTO
                        compra.Iva + " , " +
                        //compra.Total + " , '" +
                        //compra.RutaReferenciaBancaria + "' , " +
                        compra.Total + " , " +
                        1 + " , " + //ID ESTADO
                        2 + " , " + //ID PROCESO
                        1 + " , " + //ID PASO
                        1 + " , " +
                        "'APP' , " +
                        "CURRENT DATE , " +
                        "CURRENT TIME , " +
                        "'APP'" + ")";
                        dbCnx.SetQuery(strSql);

                    }

                    #endregion

                    #region REGISTRO PEDIDOP APP AUTO              

                    strSql = "";
                    strSql = @"INSERT INTO PRODAPPS.APEPANST
                            (
                            FIAPIDCOMP,     /* ID COMPRA    */
                            FIAPIDCIAU,      /*ID CIA. UNICA*/
                            FIAPIDVEHI,     /*ID VEHICULO  */
                            FIAPIDINVE,      /*ID INVENTARIO*/


                            FSAPMODELO,
                            FSAPVERSIO,
                            FSAPTRANSM,      /*TRANSMISION*/ 
                            FSAPCOLEXT,
                            FSAPNUMINV,

                            FSAPNUMSER,      /*NUMERO SERIE*/ 
                            FDAPSUBTOT,      /*SUBTOTAL     */
                            FDAPDESCUE,      /*DESCUENTO      */ 
                            FDAPIVA,         /*IVA             */
                            FDAPTOTAL,       /*TOTAL      */     
                            FSAPRUTFOT,    /*RUTA FOTO*/
                            FIAPSTATUS,      /*ESTATUS       */  
                            USERCREAT,       /*USUARIO CREACION*/
                            DATECREAT,       /*FECHA CREACION   */
                            TIMECREAT,       /*HORA CREACION    */
                            PROGCREAT       /*PROGRAMA CREACION*/
                            ) VALUES(" +
                    compra.IdCompra + "," +
                    compra.Pedido.IdAgencia.ToString().Trim() + "," +
                    compra.Pedido.IdVehiculo + "," +
                    compra.Pedido.IdInventario + ",'" +
                    compra.Pedido.Modelo + "','" +
                    compra.Pedido.Version + "','" +
                    compra.Pedido.Transmision.ToString().Trim() + "','" +
                    compra.Pedido.ColorExterior + "'," +
                    "'" + compra.Pedido.NumeroInventario + "'" + "," +
                    "'" + compra.Pedido.NumeroDeSerie + "'" + "," +
                    compra.Pedido.Subtotal + ",0," +
                    compra.Pedido.Iva + "," +
                    compra.Pedido.Total + "," +
                    "'" + compra.Pedido.RutaFoto + "'" +
                   "," + 1 + " , " +
                    "'APP' , " +
                    "CURRENT DATE , " +
                    "CURRENT TIME , " +
                    "'APP'" + ")";

                    dbCnx.SetQuery(strSql);

                    #endregion

                    #region APARTADO

                    strSql = "";

                    strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 50, \t";
                    strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                    strSql += "WHERE FNAUTOAGE = " + compra.Pedido.IdAgencia.ToString().Trim() + " ";
                    strSql += "AND FIANIDINVE=" + compra.Pedido.IdInventario + "\t";
                    strSql += "AND FIANSTATU = 1 \t";
                    strSql += "AND FNAUTOEST in (6,10)";

                    dbCnx.SetQuery(strSql);

                    #endregion

                    #region REGISTRO DETALLE PEDIDO ACCESORIOS

                    List<AccesoriosUOtros> lstAccesorios = new List<AccesoriosUOtros>();
                    if (compra.AccesoriosOtros != null)
                    {
                        foreach (var accesorios in compra.AccesoriosOtros)
                        {
                            strSql = "";
                            strSql = "SELECT COALESCE(MAX(FIAPIDCONS),0) + 1 Id ";
                            strSql += "FROM " + constantes.Ambiente + "APPS.APDPANST WHERE FIAPIDCOMP = " + compra.IdCompra;

                            DataTable dtdp = dbCnx.GetDataSet(strSql).Tables[0];

                            if (dtdp.Rows.Count == 1)
                            {
                                int idOut = 0;
                                bool isCorrect = int.TryParse(dtdp.Rows[0]["Id"].ToString(), out idOut);
                                if (isCorrect)
                                {
                                    accesorios.Id = idOut.ToString();
                                }

                                strSql = "";
                                strSql = @"INSERT INTO PRODAPPS.APDPANST
                                (
                                FIAPIDCOMP,      /*Id Compra*/
                                FIAPIDCIAU,      /*ID CIA. UNICA */
                                FIAPIDCONS,      /*ID CONSECUTIVO*/
                                FSAPCONCEP,      /*CONCEPTO      */
                                FDAPSUBTOT,      /*SUBTOTAL      */
                                FDAPDESCUE,      /*DESCUENTO       */
                                FDAPIVA,         /*IVA             */
                                FDAPTOTAL,       /*TOTAL      */    
                                FSAPRUTFOT,     /*RUTA FOTO*/
                                FIAPSTATUS,     /*ESTATUS       */  
                                USERCREAT,       /*USUARIO CREACION*/
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES 
                                (" +
                                        compra.IdCompra + " , " +
                                        compra.Pedido.IdAgencia.ToString().Trim() + " , " +
                                        accesorios.Id + " , " +
                                        "'" + accesorios.Concepto.Replace("'", "") + "'" + " , " +
                                        accesorios.Subtotal.Replace("", "'").Replace(null, "'") + "'" + " , " +
                                        0 + " , " + // DESCUENTO
                                        accesorios.Iva.Replace("'", "").Replace(null, "'") + "'" + " , " +
                                        accesorios.Total.Replace("'", "").Replace(null, "'") + "'" + " , " +
                                        "'" + accesorios.RutaFoto + "'" + "," +
                                        1 + " , " +
                                        "'APP' , " +
                                        "CURRENT DATE , " +
                                        "CURRENT TIME , " +
                                        "'APP'" + ")";

                                dbCnx.SetQuery(strSql);

                            }

                        }

                    }

                    #endregion

                    #region REGISTRO SEGUIMIENTO PEDIDO
                    strSql = "";
                    strSql = "SELECT COALESCE(MAX(FIAPIDSEGU),0) + 1 Id ";
                    strSql += "FROM " + constantes.Ambiente + "APPS.APDSGCST WHERE  FIAPIDCOMP = " + compra.IdCompra;

                    DataTable dtdt = dbCnx.GetDataSet(strSql).Tables[0];
                    if (dtdt.Rows.Count == 1)
                    {
                        int idOut = 0;
                        bool isCorrect = int.TryParse(dtdt.Rows[0]["Id"].ToString(), out idOut);

                        strSql = "";
                        strSql = @"INSERT INTO PRODAPPS.APDSGCST
                            (
                                FIAPIDCOMP,      /*ID COMPRA     */
                                FIAPIDSEGU,      /*ID SEGUIMIENTO*/
                                FSAPTITSEG,     /*TITULO SEGUIM */
                                FIAPIDESTA,      /*ID ESTADO     */
                                FIAPSTATUS,      /*ESTATUS       */
                                USERCREAT,       /*USUARIO CREACION */
                                DATECREAT,       /*FECHA CREACION   */
                                TIMECREAT,       /*HORA CREACION    */
                                PROGCREAT       /*PROGRAMA CREACION*/
                                ) VALUES
                                (" +
                            compra.IdCompra + " , " +
                            idOut + " , " +
                            "'Registro de orden de compra' , " +
                            1 + " , " +
                            1 + " , " +
                            "'APP' , " +
                            "CURRENT DATE , " +
                            "CURRENT TIME , " +
                            "'APP'" + ")";

                        dbCnx.SetQuery(strSql);
                    }

                    #endregion

                    #region CheckList

                    string strSqlCh = "";

                    strSqlCh += "INSERT INTO PRODAPPS.APDCKLST( ";
                    strSqlCh += " FIAPIDCOMP, FIAPIDPROC, FIAPIDPCKL, FSAPDESCCK, FIAPSMARTI, ";
                    strSqlCh += " FIAPAPPVIS, FIAPSISTEM, FIAPREALIZ, FIAPSTATUS, USERCREAT, ";
                    strSqlCh += " DATECREAT, TIMECREAT, PROGCREAT)";
                    strSqlCh += " SELECT " + compra.IdCompra + ", FIAPIDPROC, FIAPIDPCKL,  FSAPDESCCK, FIAPSMARTI, ";
                    strSqlCh += " FIAPAPPVIS, FIAPSISTEM,  0, FIAPSTATUS, 'APP', ";
                    strSqlCh += " CURRENT DATE, CURRENT TIME, 'APP' ";
                    strSqlCh += " FROM PRODAPPS.APCCKLIS  WHERE FIAPSTATUS = 1";

                    dbCnx.SetQuery(strSqlCh);
                    string strActualizaCh = "";

                    strActualizaCh += "UPDATE PRODAPPS.APDCKLST ";
                    strActualizaCh += "SET FIAPREALIZ = 1, ";
                    strActualizaCh += "PROGUPDAT = 'APP', USERUPDAT = 'APP', TIMEUPDAT = CURRENT TIME, DATEUPDAT = CURRENT DATE ";
                    strActualizaCh += "WHERE FIAPIDCOMP = " + compra.IdCompra;
                    strActualizaCh += " AND FIAPIDPCKL = 1";
                    dbCnx.SetQuery(strActualizaCh);

                    #endregion

                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Se registró correctamente.";
                }
                else
                {

                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();
                    respuesta.Mensaje = "No fue posible registrar la solicitud debido a que existe una compra activa.";

                    Models.OrdenCompra.Compra _compraAuto = new Models.OrdenCompra.Compra();
                    _compraAuto = OrdenCompra(int.Parse(dtCount.Rows[0]["Cuenta"].ToString()));
                    compra.IdCompra = _compraAuto.IdCompra;
                    compra.Folio = _compraAuto.FolioCompra;
                    compra.IdCuenta = _compraAuto.IdCuenta;
                    compra.Fecha = _compraAuto.FechaCompra;
                    compra.Hora = _compraAuto.HoraCompra;
                    compra.Subtotal = _compraAuto.Subtotal;
                    compra.Iva = _compraAuto.IVA;
                    compra.Total = _compraAuto.Total;
                    compra.IdEstado = _compraAuto.IdEstado;
                    compra.RutaReferenciaBancaria = _compraAuto.RutaReferenciaBancaria;
                    compra.CuentaUsuario = _compraAuto.CuentaUsuario;
                    compra.AccesoriosOtros = null;
                    compra.IdPaso = _compraAuto.IdPaso;
                }

                respuesta.Objeto = compra;

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                if (respuesta.Mensaje == "")
                {
                    respuesta.Ok = "NO";
                    respuesta.Mensaje = "No fue posible registrar la solicitud.";
                }
            }



            #region envia Correo
            if (respuesta.Ok.Equals("SI"))
            {

                string consultaCorreos = string.Empty;
                List<string> lstCorreos = new List<string>();

                consultaCorreos += "SELECT FIAPIDCIAU, FSAPCORREO FROM ";
                consultaCorreos += "PRODAPPS.APDCRSST ";
                consultaCorreos += "WHERE FIAPSTATUS = 1 ";
                consultaCorreos += "AND FIAPIDCIAU = " + compra.Pedido.IdAgencia.ToString().Trim();

                DataTable dtCorreos = dbCnx.GetDataSet(consultaCorreos).Tables[0];

                if (dtCorreos.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCorreos.Rows)
                    {
                        string correo = string.Empty;
                        correo = dr["FSAPCORREO"].ToString().Trim();
                        lstCorreos.Add(correo);
                    }
                }

                string subject = "Furia SEAT";
                EnvioCorreoSoporteCupraYGerentes hiloEnvioCorreoAGerentesYBack = new EnvioCorreoSoporteCupraYGerentes(subject, compra.Folio.ToString().Trim(), "<strong> EC: </strong> Se ha registrado un folio nuevo ", "<strong> PP: </strong> Apártalo", lstCorreos);
                Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreoAGerentesYBack.EnvioCorreoGerentesYBack));
                hilo.Start();

            }




            #endregion

            return respuesta;
        }

        public Models.OrdenCompra.Compra OrdenCompra(int IdCompra)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            DVAConstants.Constants constantes = new DVAConstants.Constants();
            Models.OrdenCompra.Compra compra = new Models.OrdenCompra.Compra();


            //Compra
            string strSql = "";
            strSql += "select ape.FIAPIDCOMP, ape.FIAPFOLCOM, ape.FIAPIDCUEN, ape.FFAPFECCOM, ape.FHAPHORCOM, ape.FDAPSUBTOT, ape.FDAPDESCUE, ape.FDAPIVA, ape.FDAPTOTAL, ape.FSAPRUTRFB, ape.FIAPIDESTA,  est.FSAPESTADO, ape.FIAPIDPROC, ape.FIAPIDPASO ";
            strSql += "FROM " + constantes.Ambiente + "apps.APECMPST ape ";
            strSql += "inner join " + constantes.Ambiente + "apps.APCESCST est ";
            strSql += "ON ape.FIAPIDESTA = est.FIAPIDESTA ";
            strSql += "WHERE ape.FIAPSTATUS = 1 and ape.FIAPIDCOMP = " + IdCompra;


            DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

            DataTable dtP;
            DataTable dtA;
            DataTable dtC;


            Models.OrdenCompra.PedidoVehiculo pedidoV;
            List<Models.OrdenCompra.Accesorio> Coleccionaccesorios;
            Cuenta cuentaUsuario;

            Models.OrdenCompra.Accesorio accesorio;


            string strSqlPedido = "";
            string strSqlAccesorios = "";

            if (dt.Rows.Count > 0)
            {


                foreach (DataRow dr in dt.Rows)
                {
                    compra = new Models.OrdenCompra.Compra();
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



                    // obtiene datos de la cuenta 
                    string strSqlCuenta = "";
                    strSqlCuenta += "SELECT FIAPIDCUEN, FIAPIDPERS, FSAPNOMBRE, FSAPAPEPAT, FSAPAPEMAT, FSAPCORREO, FSAPCVEACT, FIAPLADMOV, FIAPNUMMOV, FSAPTOKEN, FSAPSO, FIAPIDESTA FROM ";
                    strSqlCuenta += "PRODAPPS.APCCTAST ";
                    strSqlCuenta += "WHERE FIAPIDCUEN = " + compra.IdCuenta + " ";
                    strSqlCuenta += " AND FIAPIDESTA = 1";

                    dtC = dbCnx.GetDataSet(strSqlCuenta).Tables[0];


                    strSqlPedido += "select dan.FIAPIDCOMP, dan.FIAPIDCIAU, dan.FIAPIDPEDI, dan.FFAPFECPED, dan.FHAPHORPED, dan.FIAPIDPERS, dan.FIAPIDVEHI, dan.FIAPIDINVE, dan.FSAPMODELO, dan.FSAPVERSIO, dan.FSAPCOLEXT, dan.FSAPNUMINV ,dan.FSAPNUMSER, dan.FDAPSUBTOT, dan.FDAPDESCUE, dan.FDAPIVA, dan.FDAPTOTAL, dan.FIAPCOTSEG, dan.FSAPRUTFOT  ";
                    strSqlPedido += " FROM " + constantes.Ambiente + "apps.APEPANST dan ";
                    strSqlPedido += " WHERE dan.FIAPIDCOMP = " + IdCompra;
                    strSqlPedido += " and dan.FIAPSTATUS = 1";
                    strSqlPedido += " and dan.FIAPIDCOMP = " + compra.IdCompra;

                    dtP = dbCnx.GetDataSet(strSqlPedido).Tables[0];

                    if (dtP.Rows.Count > 0)
                    {

                        foreach (DataRow drP in dtP.Rows)
                        {

                            pedidoV = new Models.OrdenCompra.PedidoVehiculo();
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
                            pedidoV.RutaFoto = drP["FSAPRUTFOT"].ToString().Trim();
                            pedidoV.NumeroInventario = drP["FSAPNUMINV"].ToString().Trim();
                            pedidoV.RutaFoto = drP["FSAPRUTFOT"].ToString().Trim();

                            compra.Pedido = pedidoV;


                            if (dtC.Rows.Count > 0)
                            {
                                foreach (DataRow drC in dtC.Rows)
                                {
                                    cuentaUsuario = new Cuenta();

                                    cuentaUsuario.IdCuenta = drC["FIAPIDCUEN"].ToString().Trim();
                                    cuentaUsuario.IdPersona = drC["FIAPIDPERS"].ToString().Trim();
                                    cuentaUsuario.Nombre = drC["FSAPNOMBRE"].ToString().Trim();
                                    cuentaUsuario.ApellidoPaterno = drC["FSAPAPEPAT"].ToString().Trim();
                                    cuentaUsuario.ApellidoMaterno = drC["FSAPAPEMAT"].ToString().Trim();
                                    cuentaUsuario.Correo = drC["FSAPCORREO"].ToString().Trim();
                                    cuentaUsuario.Clave = drC["FSAPCVEACT"].ToString().Trim();
                                    cuentaUsuario.Token = drC["FSAPTOKEN"].ToString().Trim();
                                    cuentaUsuario.LadaMovil = drC["FIAPLADMOV"].ToString().Trim();
                                    cuentaUsuario.TelefonoMovil = drC["FIAPNUMMOV"].ToString().Trim();

                                    compra.CuentaUsuario = cuentaUsuario;

                                }
                            }
                            else
                            {
                                compra.CuentaUsuario = null;
                            }





                            strSqlAccesorios += "select dedan.FIAPIDCIAU, dedan.FIAPIDPEDI, dedan.FIAPIDCONS, dedan.FSAPCONCEP, dedan.FDAPSUBTOT, dedan.FDAPDESCUE, dedan.FDAPIVA, dedan.FDAPTOTAL, dedan.FSAPRUTFOT ";
                            strSqlAccesorios += " FROM " + constantes.Ambiente + "apps.APDPANST dedan ";
                            strSqlAccesorios += "where dedan.FIAPSTATUS=1 ";
                            strSqlAccesorios += "and dedan.FIAPIDCIAU = " + pedidoV.IdAgencia;
                            strSqlAccesorios += " and dedan.FIAPIDPEDI = " + pedidoV.IdPedido;
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
                                compra.Accesorios = null;

                            }



                        }

                    }
                    else
                    {

                        compra.Pedido = null;
                    }


                }


            }
            else
            {

                compra = new Models.OrdenCompra.Compra();
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

            }

            return compra;
        }


        public Respuesta RegistraDatosFiscales(DatosFiscalesProc2 datosFiscales, int IdTipoPersona)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            Respuesta respuesta = new Respuesta();
            Models.Pedido.DatosFiscalesProc2 dato = new Models.Pedido.DatosFiscalesProc2();

            string jsonPedido = string.Empty;
            string strSql = "";

            long lngIdPersona = 0;
            long lngIdPedido = 0;
            int idAgencia = 0;
            string strRutaReferenciaBancaria = "";
            string strNumeroSerie = "";
            decimal dcmTotalPedido = 0;

            try
            {
                dato = datosFiscales;

                #region RFC
                if (dato.RfcFisica != null && dato.RfcFisica.Trim() != "")
                {
                    dato.RfcFisica = BPedido.FormatoRfc(dato.RfcFisica.Trim());
                }

                if (dato.RfcRazonSocial != null && dato.RfcRazonSocial.Trim() != "")
                {
                    dato.RfcRazonSocial = BPedido.FormatoRfc(dato.RfcRazonSocial.Trim());
                }

                if (dato.RfcRepresentanteLegal != null && dato.RfcRepresentanteLegal.Trim() != "")
                {
                    dato.RfcRepresentanteLegal = BPedido.FormatoRfc(dato.RfcRepresentanteLegal.Trim());
                }
                #endregion

                string idInventario = "";
                strSql = "select FIAPIDCIAU, trim(FSAPNUMSER) NumeroSerie, FDAPTOTAL Total, FIAPIDINVE from prodapps.APEPANST where FIAPIDCOMP = " + datosFiscales.IdCompra.ToString();
                DataTable dt = dbCnx.GetDataSet(strSql).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    strNumeroSerie = dt.Rows[0]["NumeroSerie"].ToString().Trim();
                    decimal.TryParse(dt.Rows[0]["Total"].ToString(), out dcmTotalPedido);
                    idInventario = dt.Rows[0]["FIAPIDINVE"].ToString().Trim();
                    idAgencia = Convert.ToInt32(dt.Rows[0]["FIAPIDCIAU"].ToString().Trim());
                }
                else
                {
                    throw new Exception();
                }

                #region APARTADO

                try
                {
                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();

                    strSql = "";
                    strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 10, \t";
                    strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                    strSql += "WHERE FNAUTOAGE = " + idAgencia + " \t";
                    strSql += "AND FIANIDINVE=" + idInventario + "\t";
                    strSql += "AND FIANSTATU = 1 \t";
                    strSql += "AND FNAUTOEST = 50";

                    dbCnx.SetQuery(strSql);

                    #region Registra CLIENTE

                    DVARegistraPersona.RegistraPersona persona = new DVARegistraPersona.RegistraPersona();

                    if (!string.IsNullOrEmpty(datosFiscales.RazonSocial.Trim()))
                    {
                        persona.RFC = datosFiscales.RfcRazonSocial.Trim();
                        persona.RazonSocial = datosFiscales.RazonSocial.Trim();
                        persona.NumeroCelular = datosFiscales.NumeroTelefonoFisica.Trim();

                        if (IdTipoPersona == 1)
                        {
                            persona.Email = datosFiscales.CorreoFisica.Trim();
                        }
                        else if (IdTipoPersona == 2)
                        {
                            persona.Email = datosFiscales.CorreoRepresentantelegal.ToString().Trim();
                        }


                    }
                    else
                    {
                        persona.Nombre = datosFiscales.NombreFisica.Trim();
                        persona.ApellidoPaterno = datosFiscales.ApellidoPaternoFisica.Trim();
                        persona.ApellidoMaterno = datosFiscales.ApellidoMaternoFisica.Trim();
                        persona.NumeroCelular = datosFiscales.NumeroTelefonoFisica.Trim();
                        persona.Email = datosFiscales.CorreoFisica.Trim();

                        if (datosFiscales.RfcFisica.Trim() != "" && datosFiscales.RfcFisica != null)
                        {

                            persona.RFC = datosFiscales.RfcFisica.Trim().ToUpper();

                        }
                    }

                    string idPersona = ObtieneORegistraPersonaApps(ref dbCnx, idAgencia, "APP", "APP", persona);

                    bool isRegister = long.TryParse(idPersona, out lngIdPersona);

                    if (isRegister == false)
                        throw new Exception();

                    #endregion

                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();

                }
                catch (Exception ex)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();
                    throw new Exception();
                }

                #endregion

                try
                {
                    #region  REGISTRO PEDIDO

                    SolicitudPedido solicitudPedido = new SolicitudPedido();
                    solicitudPedido.IdCliente = lngIdPersona;
                    solicitudPedido.Serie = strNumeroSerie;
                    solicitudPedido.IdAgente = 999996;
                    solicitudPedido.IdContacto = 0;
                    solicitudPedido.IdTipoDeVenta = 52;
                    solicitudPedido.Total = dcmTotalPedido;
                    jsonPedido = JsonConvert.SerializeObject(solicitudPedido);

                    string valor = idAgencia + "|7244|1|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    DVAAutosystServerClasses.Seguridad.Seguridad seg = new DVAAutosystServerClasses.Seguridad.Seguridad();
                    string token = seg.EncriptarCadena(valor);

                    //string url_ped = "http://10.5.2.21:7070/wsRegistraPersona/api/Pedido/registrarconreferencia/valor/" + idAgencia + "/7244/1";  /// modificar
                    string url_ped = "http://10.5.2.21:7071/wsRegistraPersonaTest/api/Pedido/registrarconreferencia/valor/" + idAgencia + "/7244/1";  /// modificar
                
                    url_ped = url_ped.Replace("valor", token);

                    var httpWebRequest_ped = (HttpWebRequest)WebRequest.Create(url_ped);

                    httpWebRequest_ped.Timeout = 40000;

                    httpWebRequest_ped.ContentType = "application/json";
                    httpWebRequest_ped.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest_ped.GetRequestStream()))
                    {
                        streamWriter.Write(jsonPedido);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse_ped = (HttpWebResponse)httpWebRequest_ped.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse_ped.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        RespuestaPedido respuestaPedido = new RespuestaPedido();
                        string error = string.Empty;
                        if (result.Contains("IdPedido") && result.Contains("Ruta"))
                            respuestaPedido = JsonConvert.DeserializeObject<RespuestaPedido>(result);
                        else
                            error = JsonConvert.DeserializeObject<string>(result);

                        if (!string.IsNullOrEmpty(error))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            long.TryParse(respuestaPedido.IdPedido, out lngIdPedido);
                            strRutaReferenciaBancaria = respuestaPedido.Ruta.Replace("C:\\inetpub\\wwwroot\\wsAppMiAuto\\Recursos\\Adjuntos\\", "http://ws-smartit.divisionautomotriz.com/wsAppMiAuto/Recursos/Adjuntos/");
                        }
                    }

                    #endregion
                }
                catch (Exception ex)
                {

                    #region APARTADO

                    try
                    {
                        dbCnx.AbrirConexion();
                        dbCnx.BeginTransaccion();

                        strSql = "";
                        strSql += "UPDATE PRODAUT.ANCAUTOM  SET FNAUTOEST = 50, \t";
                        strSql += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' \t";
                        strSql += "WHERE FNAUTOAGE = " + idAgencia + " \t";
                        strSql += "AND FIANIDINVE=" + idInventario + "\t";
                        strSql += "AND FIANSTATU = 1 \t";
                        strSql += "AND FNAUTOEST = 10";

                        dbCnx.SetQuery(strSql);

                        dbCnx.CommitTransaccion();
                        dbCnx.CerrarConexion();

                    }
                    catch (Exception)
                    {
                        dbCnx.RollbackTransaccion();
                        dbCnx.CerrarConexion();
                    }

                    #endregion

                    throw new Exception();
                }

                #region Tablas Apps

                try
                {

                    dbCnx.AbrirConexion();
                    dbCnx.BeginTransaccion();

                    string existe = "";
                    existe += "SELECT COUNT(*) COUNT FROM PRODAPPS.APDDTFST ";
                    existe += "WHERE FIAPIDCOMP = " + dato.IdCompra + " ";
                    existe += "AND FIAPSTATUS = 1";

                    DataTable exist = dbCnx.GetDataSet(existe).Tables[0];

                    foreach (DataRow dr in exist.Rows)
                    {
                        int cantidad = 0;

                        cantidad = Convert.ToInt32(dr["COUNT"].ToString().Trim());

                        #region DatosFiscales
                        if (cantidad > 0)
                        {
                            #region Actualizar
                            if (IdTipoPersona == 1)
                            { /*PERSONA FISICA*/

                                strSql = "";
                                strSql += "UPDATE PRODAPPS.APDDTFST ";
                                strSql += "SET FSAPRFCFIS = " + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + ",";
                                strSql += "FSAPNMBFIS = " + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + ",FSAPAPTFIS = " + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'";
                                strSql += ",FSAPAMTFIS= " + "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + ",FIAPLDTFIS = " + 52 + ",FIAPNMTFIS=" + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + ",FSAPCRRFIS=" + "'" + dato.CorreoFisica.Trim() + "'";
                                strSql += ",FSAPRFCRSC =" + "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + ",FSAPRAZSOC=" + "'" + dato.RazonSocial.Trim().ToUpper() +
                                    "',FSAPRFCRLG=" + "'" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                    "'" + ",FSAPNMBRLG=" + "'" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "'" + ",FSAPAPTRLG = " + "'" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "'";
                                strSql += ",FSAPAMTRLG = " + "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "'" + ",FIAPLDTRLG=" + 52 + ",FSAPCRRRLG=" + "'" + dato.CorreoRepresentantelegal.Trim() + "'" +
                                    ",FIAPNMTRLG=" + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper());
                                strSql += " ,FSAPCUCFDI = " + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + ", FSAPDESCRI = " + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                strSql += ",FIAPIDNACP = " + dato.IdNacionalidadC2.ToString().Trim() + ", FSAPDESNAP = " + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + ", FSAPCURPPE = " + "'" + dato.CurpC2.ToString().Trim() + "'";
                                strSql += ",FSAPDIRPER = " + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + ",FIAPIDGENP =" + dato.IdGeneroPersonaPF.ToString().Trim() + ",FSAPDESGEP =" + "'" + dato.DescripcionGeneroPF.ToString().Trim() + "'";
                                strSql += ",FIAPIDOCUP=" + dato.IdOcupacionPersonaPF.ToString().Trim() + ",FSAPDESOCP=" + "'" + dato.DescripcionOcupacionPF.ToString().Trim() + "'";

                                strSql += " ,PROGCREAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, USERUPDAT = 'APP', PROGUPDAT = 'APP' ";
                                strSql += "WHERE FIAPIDCOMP = " + dato.IdCompra;
                                dbCnx.SetQuery(strSql);

                            }
                            else if (IdTipoPersona == 2)
                            {

                                strSql = "";
                                strSql += "UPDATE PRODAPPS.APDDTFST ";
                                strSql += "SET FSAPRFCFIS = " + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + ",";
                                strSql += "FSAPNMBFIS = " + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + ",FSAPAPTFIS = " + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'";
                                strSql += ",FSAPAMTFIS= " + "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + ",FIAPLDTFIS = " + 52 + ",FIAPNMTFIS=" + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + ",FSAPCRRFIS=" + "'" + dato.CorreoFisica.Trim() + "'";
                                strSql += ",FSAPRFCRSC =" + "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + ",FSAPRAZSOC=" + "'" + dato.RazonSocial.Trim().ToUpper() +
                                    "',FSAPRFCRLG=" + "'" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                    "'" + ",FSAPNMBRLG=" + "'" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "'" + ",FSAPAPTRLG = " + "'" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "'";
                                strSql += ",FSAPAMTRLG = " + "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "'" + ",FIAPLDTRLG=" + 52 + ",FSAPCRRRLG=" + "'" + dato.CorreoRepresentantelegal.Trim() + "'" +
                                    ",FIAPNMTRLG=" + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper());
                                strSql += " ,FSAPCUCFDI = " + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + ", FSAPDESCRI = " + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                strSql += ",FIAPIDNACP = " + dato.IdNacionalidadC2.ToString().Trim() + ", FSAPDESNAP = " + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + ", FSAPCURPPE = " + "'" + dato.CurpC2.ToString().Trim() + "'";
                                strSql += ",FSAPDIRPER =" + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + ",FIAPIDTSOP = " + dato.IdTipoSociedadPM.ToString().Trim().ToUpper() + ",FSAPDESTSP=" + "'" + dato.DescripcionTipoSociedadPM.ToString().Trim().ToUpper() + "'";
                                strSql += ",FFAPCREEMP= " + "'" + dato.FechaConstitucionEmpresaPM.ToString().Trim().ToUpper() + "'" + ",FIAPIDGIRO=" + dato.IdGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + ",FSAPDESCGI =" + "'" + dato.DescripcionGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "'";
                                strSql += ",FSAPDIRREP =" + "'" + dato.DireccionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FIAPIDNACR=" + dato.IdNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + ",FSAPDESNAR=" + "'" + dato.DescripcionNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'";
                                strSql += ",FIAPIDGENR=" + dato.IdGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + ",FSAPDESGER=" + "'" + dato.DescripcionGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FIAPIDOCUR=" + dato.IdOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper();
                                strSql += ",FSAPDESOCR=" + "'" + dato.DescripcionOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + ",FSAPAPTCON=" + "'" + dato.ApellidoPaternoContactoVPF.ToString().Trim().ToUpper() + "'" + ",FSAPAPMCON=" + "'" + dato.ApellidoMaternoContactoVPF.ToString().Trim().ToUpper() + "'";
                                strSql += ",FSAPNOMCON=" + "'" + dato.NombresContactoVPF.ToString().Trim().ToUpper() + "'" + ",FIAPLADCON=" + 52 + ", FIAPTELCON=" + dato.TelefonoContactoVPF.ToString().Trim().ToUpper();
                                strSql += ",FSAPCORCON=" + "'" + dato.CorreoContactoVPF.ToString().Trim().ToUpper() + "'";

                                strSql += " ,PROGCREAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, USERUPDAT = 'APP', PROGUPDAT = 'APP' ";
                                strSql += "WHERE FIAPIDCOMP = " + dato.IdCompra;
                                dbCnx.SetQuery(strSql);

                            }
                            #endregion
                        }
                        else
                        {
                            #region Agregar
                            if (IdTipoPersona == 1)
                            { /*PERSONA FISICA*/
                                strSql = "";
                                strSql += "INSERT INTO PRODAPPS.APDDTFST (";
                                strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS, ";
                                strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS, ";
                                strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG, ";
                                strSql += "FSAPAMTRLG, FIAPLDTRLG, FIAPNMTRLG, FSAPCRRRLG, FSAPCUCFDI, FSAPDESCRI, ";

                                strSql += "FIAPIDNACP, FSAPDESNAP, FSAPCURPPE, ";
                                strSql += "FSAPDIRPER, FIAPIDGENP, FSAPDESGEP, ";
                                strSql += "FIAPIDOCUP, FSAPDESOCP, ";

                                strSql += "FIAPSTATUS, USERCREAT, PROGCREAT)";
                                strSql += "VALUES (";
                                strSql += dato.IdCompra + "," + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + "," + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'" + ",";
                                strSql += "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + "," + 52 + "," + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + "," + "'" + dato.CorreoFisica.Trim() + "'" + ",";
                                strSql += "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + "," + "'" + dato.RazonSocial.Trim().ToUpper() +
                                    "','" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                    "','" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "','" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "',";
                                strSql += "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "',52" +
                                    "," + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper()) +
                                    ",'" + dato.CorreoRepresentantelegal.Trim() + "'" + "," + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + "," + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                strSql += "," + dato.IdNacionalidadC2.ToString().Trim() + "," + "'" + dato.DescripcionNacionalidadC2.ToString().Trim() + "'" + "," + "'" + dato.CurpC2.ToString().Trim() + "'";
                                strSql += "," + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + "," + dato.IdGeneroPersonaPF.ToString().Trim() + "," + "'" + dato.DescripcionGeneroPF.ToString().Trim() + "'";
                                strSql += "," + dato.IdOcupacionPersonaPF.ToString().Trim() + "," + "'" + dato.DescripcionOcupacionPF.ToString().Trim() + "'";

                                strSql += ",1,'APP','APP')";

                                dbCnx.SetQuery(strSql);
                            }
                            else if (IdTipoPersona == 2)
                            { /*PERSONA MORAL*/

                                strSql = "";
                                strSql += "INSERT INTO PRODAPPS.APDDTFST (";
                                strSql += "FIAPIDCOMP, FSAPRFCFIS, FSAPNMBFIS, FSAPAPTFIS, ";
                                strSql += "FSAPAMTFIS, FIAPLDTFIS, FIAPNMTFIS, FSAPCRRFIS, ";
                                strSql += "FSAPRFCRSC, FSAPRAZSOC, FSAPRFCRLG, FSAPNMBRLG, FSAPAPTRLG, ";
                                strSql += "FSAPAMTRLG, FIAPLDTRLG, FIAPNMTRLG, FSAPCRRRLG, FSAPCUCFDI, FSAPDESCRI, ";
                                // aqui inician los cambios del proceso 2
                                strSql += "FIAPIDNACP,FSAPDESNAP,FSAPCURPPE,";
                                strSql += "FSAPDIRPER,FIAPIDTSOP,FSAPDESTSP,";
                                strSql += "FFAPCREEMP,FIAPIDGIRO,FSAPDESCGI,";
                                strSql += "FSAPDIRREP,FIAPIDNACR,FSAPDESNAR,";
                                strSql += "FIAPIDGENR,FSAPDESGER,FIAPIDOCUR,";
                                strSql += "FSAPDESOCR,FSAPAPTCON,FSAPAPMCON,";
                                strSql += "FSAPNOMCON,FIAPLADCON,FIAPTELCON,";
                                strSql += "FSAPCORCON,";

                                strSql += "FIAPSTATUS, USERCREAT, PROGCREAT)";
                                strSql += "VALUES (";
                                strSql += dato.IdCompra + "," + "'" + dato.RfcFisica.Trim().ToUpper() + "'" + "," + "'" + dato.NombreFisica.Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoFisica.Trim().ToUpper() + "'" + ",";
                                strSql += "'" + dato.ApellidoMaternoFisica.Trim().ToUpper() + "'" + "," + 52 + "," + (string.IsNullOrEmpty(dato.NumeroTelefonoFisica) ? "Default" : dato.NumeroTelefonoFisica.Trim().ToUpper()) + "," + "'" + dato.CorreoFisica.Trim() + "'" + ",";
                                strSql += "'" + dato.RfcRazonSocial.Trim().ToUpper() + "'" + "," + "'" + dato.RazonSocial.Trim().ToUpper() +
                                    "','" + dato.RfcRepresentanteLegal.Trim().ToUpper() +
                                    "','" + dato.NombreRepresentanteLegal.Trim().ToUpper() + "','" + dato.ApellidoPaternoRepresentanteLegal.Trim().ToUpper() + "',";
                                strSql += "'" + dato.ApellidoMaternoRepresentantelegal.Trim().ToUpper() + "',52" +
                                    "," + (string.IsNullOrEmpty(dato.NumeroTelefonoRepresentanteLegal.Trim()) ? "Default" : dato.NumeroTelefonoRepresentanteLegal.Trim().ToUpper()) +
                                    ",'" + dato.CorreoRepresentantelegal.Trim() + "'" + "," + (string.IsNullOrEmpty(dato.ClaveUsoCfdi.Trim()) ? "Default" : "'" + dato.ClaveUsoCfdi.Trim().ToUpper() + "'") + "," + (string.IsNullOrEmpty(dato.DescripcionUsoCfdi.Trim()) ? "Default" : "'" + dato.DescripcionUsoCfdi.Trim().ToUpper() + "'");

                                strSql += "," + dato.IdNacionalidadC2.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionNacionalidadC2.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.CurpC2.ToString().Trim().ToUpper() + "'";
                                strSql += "," + "'" + dato.DireccionC2.ToString().Trim().ToUpper() + "'" + "," + dato.IdTipoSociedadPM.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionTipoSociedadPM.ToString().Trim().ToUpper() + "'";
                                strSql += "," + "'" + dato.FechaConstitucionEmpresaPM.ToString().Trim().ToUpper() + "'" + "," + dato.IdGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionGiroDeLaEmpresaPM.ToString().Trim().ToUpper() + "'";
                                strSql += "," + "'" + dato.DireccionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + dato.IdNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionNacionalidadRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'";
                                strSql += "," + dato.IdGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "," + "'" + dato.DescripcionGeneroRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + dato.IdOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper();
                                strSql += "," + "'" + dato.DescripcionOcupacionRepresentanteLegalVPF.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoPaternoContactoVPF.ToString().Trim().ToUpper() + "'" + "," + "'" + dato.ApellidoMaternoContactoVPF.ToString().Trim().ToUpper() + "'";
                                strSql += "," + "'" + dato.NombresContactoVPF.ToString().Trim().ToUpper() + "'" + "," + 52 + "," + dato.TelefonoContactoVPF.ToString().Trim().ToUpper();
                                strSql += "," + "'" + dato.CorreoContactoVPF.ToString().Trim().ToUpper() + "'";

                                strSql += ",1,'APP','APP')";

                                dbCnx.SetQuery(strSql);

                            }
                            #endregion
                        }
                        #endregion
                    }

                    #region REGISTRO COMPRA

                    /*strSql = "";
                    strSql = @"UPDATE PRODAPPS.APECOMPR SET FSAPRUTRFB = '" +
                        strRutaReferenciaBancaria + "' WHERE FIAPIDCOMP = " + datosFiscales.IdCompra.ToString();
                    dbCnx.SetQuery(strSql);*/

                    #endregion

                    #region REGISTRO PEDIDOP APP AUTO              

                    strSql = "";
                    strSql = @"UPDATE PRODAPPS.APEPANST SET FIAPIDPEDI = " + lngIdPedido.ToString() +
                        ", FFAPFECPED = CURRENT DATE, FHAPHORPED = CURRENT TIME, FIAPIDPERS = " + lngIdPersona.ToString() +
                        " WHERE FIAPIDCOMP = " + datosFiscales.IdCompra.ToString();
                    dbCnx.SetQuery(strSql);

                    #endregion

                    #region REGISTRO DETALLE PEDIDO ACCESORIOS

                    //strSql = "";
                    //strSql = @"UPDATE PRODAPPS.APDPEDAN SET FIAPIDCIAU = " + idAgencia + " , FIAPIDPEDI = " + lngIdPedido.ToString() +
                    //    " WHERE FIAPIDCOMP = " + datosFiscales.IdCompra.ToString();
                    //dbCnx.SetQuery(strSql);

                    #endregion

                    dbCnx.CommitTransaccion();
                    dbCnx.CerrarConexion();

                    respuesta.Ok = "SI";
                    respuesta.Mensaje = "Registro exitoso.";
                    respuesta.Objeto = null;
                    return respuesta;

                }
                catch (Exception ex)
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();

                    throw new Exception();
                }
                #endregion

            }
            catch (Exception ex)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No fue posible registrar los datos fiscales.";
                respuesta.Objeto = null;
                return respuesta;
            }
        }

        public Respuesta DeshacerPaso(long Idcompra, int IdPaso)
        {

            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string strActualiza = "";

                strActualiza += "UPDATE PRODAPPS.APDCKLST ";
                strActualiza += "SET FIAPREALIZ = 0, ";
                strActualiza += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                strActualiza += "where FIAPIDCOMP = " + Idcompra.ToString() + " ";
                strActualiza += "and FIAPIDPCKL = " + IdPaso.ToString() + " ";
                strActualiza += "AND FIAPSTATUS = 1";

                dbCnx.SetQuery(strActualiza);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Paso actualizado correctamente";
                respuesta.Objeto = "";

            }
            catch (Exception)
            {
                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo actualizar el paso";
                respuesta.Objeto = "";

                return respuesta;
            }


            return respuesta;

        }

        public static string FormatoRfc(string rfc)
        {

            string rfcG = "";

            for (int c = 0; c < rfc.Length; c++)
            {

                if (rfcG.Length <= 5)
                {
                    if (Char.IsDigit(rfc[c]))
                    {
                        rfcG += rfc[c];
                    }
                }
            }

            return rfc.Replace(rfcG, "-*-").Replace("*", rfcG);
        }

        public Respuesta RegistraCheckContactoAgencia(CheckAgencia checkAgencia)
        {

            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {



                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();


                string descEstado = string.Empty;

                string strCCatEsta = "";

                strCCatEsta += "select FIAPIDESTA, FSAPDESEST FROM ";
                strCCatEsta += "PRODAPPS.APCESSST ";
                strCCatEsta += "where FIAPSTATUS = 1 ";
                strCCatEsta += "AND FIAPIDESTA = " + checkAgencia.IdEstado;

                DataTable dtCaE = dbCnx.GetDataSet(strCCatEsta).Tables[0];



                if (dtCaE.Rows.Count > 0)
                {

                    foreach (DataRow dr in dtCaE.Rows)
                    {

                        descEstado = dr["FSAPDESEST"].ToString().Trim();
                    }

                }



                string strRCh = "";

                strRCh += "INSERT INTO PRODAPPS.APDCLSST ( ";
                strRCh += "FIAPIDCIAU, FIAPIDCOMP, FIAPIDPCKL, ";
                strRCh += "FSAPDESCCK, FSAPDESEST, FIAPIDESTA, ";
                strRCh += "FIAPSTATUS, USERCREAT, PROGCREAT)";
                strRCh += "VALUES (";
                strRCh += (string.IsNullOrEmpty(checkAgencia.IdAgencia) ? "Default" : checkAgencia.IdAgencia.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(checkAgencia.IdCompra) ? "Default" : checkAgencia.IdCompra.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(checkAgencia.IdCheck) ? "Default" : checkAgencia.IdCheck.Trim().ToUpper()) + ",";
                strRCh += "'" + (string.IsNullOrEmpty(checkAgencia.DescripcionCheck) ? "Default" : checkAgencia.DescripcionCheck.Trim().ToUpper()) + "'" + "," + "'" + (string.IsNullOrEmpty(descEstado) ? "Default" : descEstado.Trim().ToUpper()) + "'" + "," + (string.IsNullOrEmpty(checkAgencia.IdEstado) ? "Default" : checkAgencia.IdEstado.Trim());
                strRCh += "," + "1,'APP', 'APP'";
                strRCh += ")";


                dbCnx.SetQuery(strRCh);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro guardado correctamente.";
                respuesta.Objeto = "";


            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar el registro.";
                respuesta.Objeto = "";

                return respuesta;
            }




            return respuesta;

        }

        public Respuesta ActualizaCheckContactoAgencia(ActualizaCheckAgencia checkAgencia)
        {

            Respuesta respuesta = new Respuesta();
            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();


                string descEstado = string.Empty;

                string strCCatEsta = "";

                strCCatEsta += "select FIAPIDESTA, FSAPDESEST FROM ";
                strCCatEsta += "PRODAPPS.APCESSST ";
                strCCatEsta += "where FIAPSTATUS = 1 ";
                strCCatEsta += "AND FIAPIDESTA = " + checkAgencia.IdEstado;

                DataTable dtCaE = dbCnx.GetDataSet(strCCatEsta).Tables[0];



                if (dtCaE.Rows.Count > 0)
                {

                    foreach (DataRow dr in dtCaE.Rows)
                    {

                        descEstado = dr["FSAPDESEST"].ToString().Trim();
                    }

                }



                string strUpCh = "";

                strUpCh += "UPDATE PRODAPPS.APDCLSST ";
                strUpCh += "SET FSAPDESEST = " + "'" + descEstado + "'" + "," + "FIAPIDESTA = " + checkAgencia.IdEstado + ",";
                strUpCh += "USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                strUpCh += "WHERE ";
                strUpCh += "FIAPIDCOMP = " + checkAgencia.IdCompra;
                strUpCh += " AND FIAPIDPCKL = " + checkAgencia.IdCheck;
                strUpCh += " AND FIAPSTATUS = 1";

                dbCnx.SetQuery(strUpCh);


                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Registro actualizado correctamente.";
                respuesta.Objeto = "";


            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar el registro.";
                respuesta.Objeto = "";

                return respuesta;
            }




            return respuesta;

        }

        public List<DatoCheckAgencia> ObtieneCheckContactoAgencia(int IdCompra)
        {


            DVADB.DB2 dbCnx = new DVADB.DB2();

            DatoCheckAgencia dato = new DatoCheckAgencia();
            List<DatoCheckAgencia> lstDatos = new List<DatoCheckAgencia>();

            try
            {

                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();


                string strCC = "";
                strCC += " select FIAPIDCIAU, FIAPIDCOMP, FIAPIDPCKL, FSAPDESCCK, FSAPDESEST, FIAPIDESTA ";
                strCC += "from PRODAPPS.APDCLSST ";
                strCC += "WHERE ";
                strCC += " FIAPIDCOMP = " + IdCompra;
                strCC += " AND FIAPSTATUS = 1";


                DataTable dtE = dbCnx.GetDataSet(strCC).Tables[0];

                if (dtE.Rows.Count > 0)
                {

                    foreach (DataRow dr in dtE.Rows)
                    {
                        dato = new DatoCheckAgencia();

                        dato.IdAgencia = dr["FIAPIDCIAU"].ToString().Trim();
                        dato.IdCompra = dr["FIAPIDCOMP"].ToString().Trim();
                        dato.IdCheck = dr["FIAPIDPCKL"].ToString().Trim();
                        dato.DescripcionCheck = dr["FSAPDESCCK"].ToString().Trim();
                        dato.DescripcionEstado = dr["FSAPDESEST"].ToString().Trim();
                        dato.IdEstado = dr["FIAPIDESTA"].ToString().Trim();

                        lstDatos.Add(dato);
                    }

                }


                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();




            }
            catch (Exception)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                lstDatos = new List<DatoCheckAgencia>();

                return lstDatos;
            }




            return lstDatos;

        }

        public Respuesta RegistraHistorico(HistoricoComunicacionAgencia historico)
        {

            Respuesta respuesta = new Respuesta();

            DVADB.DB2 dbCnx = new DVADB.DB2();

            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();

                string strHist = "";

                strHist += "INSERT INTO PRODAPPS.APDHSMST (";
                strHist += "FIAPIDCIAU, FIAPIDCOMP,";
                strHist += "FFAPFECHA, FHAPHORA,";
                strHist += "FIAPIDESTA, FSAPDESEST,";
                strHist += "FIAPIDPCKL, FSAPDESCCK,";
                strHist += "FSAPNOTIFI, FIAPSTATUS,";
                strHist += "USERCREAT, PROGCREAT)";
                strHist += "VALUES (";
                strHist += (string.IsNullOrEmpty(historico.IdAgencia) ? "Default" : historico.IdAgencia.Trim().ToUpper()) + "," + (string.IsNullOrEmpty(historico.IdCompra) ? "Default" : historico.IdCompra.Trim().ToUpper()) + ",";
                strHist += "CURRENT DATE, CURRENT TIME,";
                strHist += (string.IsNullOrEmpty(historico.IdEstadoMovimiento) ? "Default" : historico.IdEstadoMovimiento.Trim().ToUpper()) + " ," + "'" + (string.IsNullOrEmpty(historico.DescripcionEstadoMovimiento) ? "Default" : historico.DescripcionEstadoMovimiento.Trim().ToUpper()) + "'" + ",";
                strHist += (string.IsNullOrEmpty(historico.IdCheck) ? "Default" : historico.IdCheck.Trim().ToUpper()) + " ," + "'" + (string.IsNullOrEmpty(historico.DescripcionCheck) ? "Default" : historico.DescripcionCheck.Trim().ToUpper()) + "'" + ",";
                strHist += "'" + (string.IsNullOrEmpty(historico.Mensaje) ? "Default" : historico.Mensaje.Trim().ToUpper()) + "'" + "," + "1" + ",";
                strHist += "'APP', 'APP'";
                strHist += ")";





                dbCnx.SetQuery(strHist);

                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Datos registrados satisfactoriamente.";
                respuesta.Objeto = "";

            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                respuesta = new Respuesta();

                respuesta.Ok = "NO";
                respuesta.Mensaje = "No se pudo guardar el histórico.";
                respuesta.Objeto = "";

                return respuesta;
            }


            return respuesta;
        }

        public Task<string> creaImagen(string filePath, string objBase64)
        {
            return Task.Run(()=> 
            {
                string cadena = string.Empty;

                try
                {
                    Bitmap bmpReturn = null;
                    byte[] byteBuffer = Convert.FromBase64String(objBase64.Split(',')[1]);
                    MemoryStream memoryStream = new MemoryStream(byteBuffer);
                    memoryStream.Position = 0;
                    bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
                    memoryStream.Close();
                    memoryStream = null;
                    byteBuffer = null;

                    using (Bitmap bmp = new Bitmap(bmpReturn))
                    {
                        using (Bitmap newImage = new Bitmap(bmp))
                        {
                            newImage.Save(filePath, ImageFormat.Jpeg);
                        }
                    }

                    cadena = "SI";

                    return cadena;

                }
                catch (Exception)
                {

                    cadena = "NO";
                    return cadena;
                }
            });
        }

        public  Task<string> creaPDF(string filePath, string objBase64)
        {
            return Task.Run(() =>
            {
                string cadena = "";

                byte[] bytes = Convert.FromBase64String(objBase64.Split(',')[1]);

                System.IO.FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                System.IO.BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();

                cadena = "SI";

                return cadena;
            });

        }

        public static string ObtieneORegistraPersonaApps(ref DVADB.DB2 conexion, int IdAgencia, string IdUsuario, string IdPrograma, DVARegistraPersona.RegistraPersona registraPersona)
        {

            DVADB.DB2 dbCnx = conexion;

            try
            {

                if (registraPersona == null)
                    throw new Exception("Se debe recibir una objeto persona");

                foreach (PropertyInfo prop in registraPersona.GetType().GetProperties())
                {
                    if (prop.GetValue(registraPersona) == null)
                    {
                        prop.SetValue(registraPersona, "");
                    }
                }


                #region LIMPIADO DE ACENTOS Y PUNTO, GENERACION DE FORMATO DEL RFC
                // QUITAMOS ACENTOS 

                registraPersona.Nombre = quitaAcentos(registraPersona.Nombre.ToString().Trim().ToUpper());
                registraPersona.ApellidoPaterno = quitaAcentos(registraPersona.ApellidoPaterno.ToString().Trim().ToUpper());
                registraPersona.ApellidoMaterno = quitaAcentos(registraPersona.ApellidoMaterno.ToString().Trim().ToUpper());
                registraPersona.RazonSocial = quitaAcentos(registraPersona.RazonSocial.ToString().Trim().ToUpper());

                // QUITAMOS CARACTERES ESPECIALES

                if (registraPersona.RazonSocial.Contains("."))
                {
                    registraPersona.RazonSocial = registraPersona.RazonSocial.Replace(".", "");
                }


                // FORMATEAMOS EL RFC

                if (registraPersona.RFC.Contains("-"))
                {

                    string rfcSinAcentos = registraPersona.RFC.Replace("-", "");
                    registraPersona.RFC = FormatoRfc(rfcSinAcentos.ToString().Trim().ToUpper());

                }
                else
                {

                    if (registraPersona.RFC.Length > 0)
                    {
                        registraPersona.RFC = FormatoRfc(registraPersona.RFC.ToString().Trim().ToUpper());
                    }

                }


                #endregion



                if (registraPersona.RazonSocial == "")
                {

                    // Persona Fisica

                    if (registraPersona.RFC != "")
                    {

                        List<PersonaCliente> lstCliente = new List<PersonaCliente>();
                        PersonaCliente cliente = new PersonaCliente();

                        string strRfcB = "";
                        strRfcB += "SELECT FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, FSPERFC, FDPEIDTIPO, ";
                        strRfcB += "FDPEIDTCTE, FDPEIDCLAS, FFPEALTA, FHPEALTA, FFPEESTATU, FHPEESTATU FROM PRODPERS.CTEPERSO ";
                        strRfcB += "WHERE FSPERFC = " + "'" + registraPersona.RFC.ToString().ToUpper().Trim() + "'";
                        strRfcB += " AND FDPEESTATU = 1";

                        DataTable DtRFC = dbCnx.GetDataSet(strRfcB).Tables[0];

                        if (DtRFC.Rows.Count > 0)
                        {

                            foreach (DataRow dr in DtRFC.Rows)
                            {
                                cliente = new PersonaCliente();

                                cliente.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                                cliente.IdLegacy = dr["FSPECTEUNI"].ToString().Trim();
                                cliente.IdAgencia = dr["FIPEIDCIAU"].ToString().Trim();
                                cliente.Rfc = dr["FSPERFC"].ToString().Trim();
                                cliente.IdTipo = dr["FDPEIDTIPO"].ToString().Trim();
                                cliente.TipoCliente = dr["FDPEIDTCTE"].ToString().Trim();
                                cliente.ClasePersona = dr["FDPEIDCLAS"].ToString().Trim();
                                cliente.FechaAlta = dr["FFPEALTA"].ToString().Trim();
                                cliente.HoraAlta = dr["FHPEALTA"].ToString().Trim();
                                cliente.FechaEstatus = dr["FFPEESTATU"].ToString().Trim();
                                cliente.HoraEstatus = dr["FHPEESTATU"].ToString().Trim();

                                lstCliente.Add(cliente);
                            }

                        }


                        if (lstCliente.Count == 0)
                        {

                            long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                            return IdPersona.ToString();

                        }


                        if (lstCliente.Count == 1)
                        {
                            return cliente.IdPersona;
                        }


                        if (lstCliente.Count >= 2)
                        {


                            RegistroPersonaFisica personaFisica = new RegistroPersonaFisica();
                            List<RegistroPersonaFisica> lstPersonaFisica = new List<RegistroPersonaFisica>();


                            string idsRfcs = string.Empty;

                            foreach (PersonaCliente dato in lstCliente)
                            {

                                idsRfcs += dato.IdPersona + ",";
                            }

                            string strBPF = string.Empty;

                            strBPF += "SELECT CPERFI.*, CTINOB.FSTNDESCRI, CPAISE.FSGECVEPAI, CPAISE.FSGEDEPAIS, COCUPA.FSOCCVEOCU ";
                            strBPF += "FROM PRODPERS.CTCPERFI CPERFI ";
                            strBPF += "LEFT JOIN PRODPERS.CTCTINOB CTINOB ON CTINOB.FDTNIDTINO = CPERFI.FDPFIDTINO ";
                            strBPF += "LEFT JOIN PRODGRAL.GECPAISE CPAISE ON CPAISE.FIGEIDPAIS = CPERFI.FIPEIDPAIS ";
                            strBPF += "LEFT JOIN PRODPERS.CTCOCUPA COCUPA ON CPERFI.FIPEIDOCUP = COCUPA.FDOCIDOCUP ";
                            strBPF += "WHERE CPERFI.FDPFIDPERS IN ( " + idsRfcs.Substring(0, idsRfcs.Length - 1) + " ) ";
                            strBPF += "AND CPERFI.FDPFESTATU = 1";

                            DataTable dtBPF = dbCnx.GetDataSet(strBPF).Tables[0];


                            if (dtBPF.Rows.Count > 0)
                            {

                                foreach (DataRow dr in dtBPF.Rows)
                                {

                                    personaFisica = new RegistroPersonaFisica();

                                    personaFisica.IdPersona = dr["FDPFIDPERS"].ToString().Trim();
                                    personaFisica.Nombre = dr["FSPFNOMBRE"].ToString().Trim();
                                    personaFisica.ApellidoPaterno = dr["FSPFAPATER"].ToString().Trim();
                                    personaFisica.ApellidoMaterno = dr["FSPFAMATER"].ToString().Trim();
                                    personaFisica.IdSexo = dr["FDPFIDSEXO"].ToString().Trim();
                                    personaFisica.IdEstadoCIvil = dr["FDPFIDEDCI"].ToString().Trim();
                                    personaFisica.FechaAlta = Convert.ToDateTime(dr["DATECREAT"].ToString().Trim());

                                    lstPersonaFisica.Add(personaFisica);
                                }

                                List<RegistroPersonaFisica> lstListaPersonasConMismoNombre = new List<RegistroPersonaFisica>();

                                lstListaPersonasConMismoNombre = lstPersonaFisica.FindAll(x => x.Nombre == registraPersona.Nombre.ToString().Trim() && x.ApellidoPaterno == registraPersona.ApellidoPaterno.ToString().Trim() && x.ApellidoMaterno == registraPersona.ApellidoMaterno.ToString().Trim());


                                if (lstListaPersonasConMismoNombre.Count == 0)
                                {

                                    long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                    return IdPersona.ToString();
                                }


                                if (lstListaPersonasConMismoNombre.Count == 1)
                                {

                                    return lstListaPersonasConMismoNombre[0].IdPersona;

                                }

                                if (lstListaPersonasConMismoNombre.Count >= 2)
                                {
                                    if (registraPersona.RFC.ToString().Trim() == "" || registraPersona.RFC.ToString().Trim() == null)
                                    {

                                        PersonaTelefono NumTelefonosPer = new PersonaTelefono();
                                        List<PersonaTelefono> lstNumTelefonosPer = new List<PersonaTelefono>();

                                        string idsPMN = string.Empty;
                                        foreach (RegistroPersonaFisica perMN in lstListaPersonasConMismoNombre)
                                        {
                                            idsPMN += perMN.IdPersona + ",";
                                        }

                                        string strTel = string.Empty;

                                        strTel += "SELECT * FROM ";
                                        strTel += "PRODPERS.CTDRPTEL ";
                                        strTel += "WHERE  FDPTIDPERS IN ( " + idsPMN.Substring(0, idsPMN.Length - 1) + ")";
                                        strTel += "AND FDPTESTATU = 1 ";

                                        DataTable dtTele = dbCnx.GetDataSet(strTel).Tables[0];

                                        foreach (DataRow dr in dtTele.Rows)
                                        {

                                            NumTelefonosPer = new PersonaTelefono();
                                            NumTelefonosPer.IdPersona = dr["FDPTIDPERS"].ToString().Trim();
                                            NumTelefonosPer.Lada = dr["FDPTPLADA"].ToString().Trim();
                                            NumTelefonosPer.NumeroTelefono = NumTelefonosPer.Lada + dr["FDPTNUMTEL"].ToString().Trim();

                                            lstNumTelefonosPer.Add(NumTelefonosPer);
                                        }

                                        List<PersonaTelefono> lstTelefonoParaMismaPersona = lstNumTelefonosPer.FindAll(p => p.NumeroTelefono == registraPersona.NumeroCelular);

                                        if (lstTelefonoParaMismaPersona.Count == 0)
                                        {

                                            long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                            return IdPersona.ToString();
                                        }

                                        if (lstTelefonoParaMismaPersona.Count == 1 || lstTelefonoParaMismaPersona.Count >= 2)
                                        {

                                            return lstTelefonoParaMismaPersona[0].IdPersona;
                                        }

                                    }
                                    else
                                    {

                                        lstListaPersonasConMismoNombre = lstListaPersonasConMismoNombre.OrderBy(i => i.FechaAlta).ToList();

                                        return lstListaPersonasConMismoNombre[0].IdPersona;

                                    }
                                }

                            }
                            else
                            {

                                long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                return IdPersona.ToString();

                            }
                        }

                    }
                    else
                    {

                        // CUANDO EL RFC ESTA VACIO 

                        // se busca la coincidencia por nombre

                        string IdsCoincidencias = string.Empty;
                        string strFisica = "";

                        strFisica += "SELECT CPERFI.*, CTINOB.FSTNDESCRI, CPAISE.FSGECVEPAI, CPAISE.FSGEDEPAIS, COCUPA.FSOCCVEOCU ";
                        strFisica += "FROM PRODPERS.CTCPERFI CPERFI ";
                        strFisica += "LEFT JOIN PRODPERS.CTCTINOB CTINOB ON CTINOB.FDTNIDTINO = CPERFI.FDPFIDTINO ";
                        strFisica += "LEFT JOIN PRODGRAL.GECPAISE CPAISE ON CPAISE.FIGEIDPAIS = CPERFI.FIPEIDPAIS ";
                        strFisica += "LEFT JOIN PRODPERS.CTCOCUPA COCUPA ON CPERFI.FIPEIDOCUP = COCUPA.FDOCIDOCUP ";
                        strFisica += "WHERE CPERFI.FSPFNOMBRE = " + "'" + registraPersona.Nombre.ToString().ToUpper().Trim() + "' ";
                        strFisica += "AND FSPFAPATER = " + "'" + registraPersona.ApellidoPaterno.ToString().ToUpper().Trim() + "' ";
                        strFisica += "AND FSPFAMATER = " + "'" + registraPersona.ApellidoMaterno.ToString().ToUpper().Trim() + "' ";
                        strFisica += "AND FDPFESTATU = 1 ";


                        DataTable dtPF = dbCnx.GetDataSet(strFisica).Tables[0];

                        if (dtPF.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtPF.Rows)
                            {
                                IdsCoincidencias += dr["FDPFIDPERS"].ToString().Trim() + ",";
                            }


                            //mediante el IdPersona recuperados hay que buscar las personas que satisfacen 
                            // la regla que cumplen con el nombre para posteriormente eliminar aquellos que tienen RFC


                            List<PersonaCliente> lstPersonas = new List<PersonaCliente>();
                            PersonaCliente persona = new PersonaCliente();
                            PersonaTelefono NumTelefonosPer = new PersonaTelefono();
                            List<PersonaTelefono> lstNumTelefonosPer = new List<PersonaTelefono>();




                            string strRfcB = "";
                            strRfcB += "SELECT FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, FSPERFC, FDPEIDTIPO, ";
                            strRfcB += "FDPEIDTCTE, FDPEIDCLAS, FFPEALTA, FHPEALTA, FFPEESTATU, FHPEESTATU FROM PRODPERS.CTEPERSO ";
                            strRfcB += "WHERE FDPEIDPERS IN ( " + IdsCoincidencias.Substring(0, IdsCoincidencias.Length - 1) + ") ";
                            strRfcB += " AND FDPEESTATU = 1";

                            DataTable DtRFC = dbCnx.GetDataSet(strRfcB).Tables[0];

                            if (DtRFC.Rows.Count > 0)
                            {

                                foreach (DataRow dr in DtRFC.Rows)
                                {
                                    persona = new PersonaCliente();

                                    persona.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                                    persona.IdLegacy = dr["FSPECTEUNI"].ToString().Trim();
                                    persona.IdAgencia = dr["FIPEIDCIAU"].ToString().Trim();
                                    persona.Rfc = dr["FSPERFC"].ToString().Trim();
                                    persona.IdTipo = dr["FDPEIDTIPO"].ToString().Trim();
                                    persona.TipoCliente = dr["FDPEIDTCTE"].ToString().Trim();
                                    persona.ClasePersona = dr["FDPEIDCLAS"].ToString().Trim();
                                    persona.FechaAlta = dr["FFPEALTA"].ToString().Trim();
                                    persona.HoraAlta = dr["FHPEALTA"].ToString().Trim();
                                    persona.FechaEstatus = dr["FFPEESTATU"].ToString().Trim();
                                    persona.HoraEstatus = dr["FHPEESTATU"].ToString().Trim();

                                    lstPersonas.Add(persona);
                                }

                            }

                            // quitamos aquellas personas que tienen RFC

                            string IdsPersonasSinRFC = string.Empty;

                            lstPersonas = lstPersonas.FindAll(c => c.Rfc == "");

                            foreach (PersonaCliente dato in lstPersonas)
                            {

                                IdsPersonasSinRFC += dato.IdPersona + ",";

                            }

                            // buscamos los telefonos por los ids que no tienen RFC                            

                            string strTel = string.Empty;

                            strTel += "SELECT * FROM ";
                            strTel += "PRODPERS.CTDRPTEL ";
                            strTel += "WHERE  FDPTIDPERS IN ( " + IdsPersonasSinRFC.Substring(0, IdsPersonasSinRFC.Length - 1) + ")";
                            strTel += "AND FDPTESTATU = 1 ";

                            DataTable dtTele = dbCnx.GetDataSet(strTel).Tables[0];

                            foreach (DataRow dr in dtTele.Rows)
                            {

                                NumTelefonosPer = new PersonaTelefono();
                                NumTelefonosPer.IdPersona = dr["FDPTIDPERS"].ToString().Trim();
                                NumTelefonosPer.Lada = dr["FDPTPLADA"].ToString().Trim();
                                NumTelefonosPer.NumeroTelefono = NumTelefonosPer.Lada + dr["FDPTNUMTEL"].ToString().Trim();

                                lstNumTelefonosPer.Add(NumTelefonosPer);
                            }

                            List<PersonaTelefono> lstTelefonoParaMismaPersona = lstNumTelefonosPer.FindAll(p => p.NumeroTelefono == registraPersona.NumeroCelular);

                            if (lstTelefonoParaMismaPersona.Count == 0)
                            {

                                long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                                return IdPersona.ToString();
                            }

                            if (lstTelefonoParaMismaPersona.Count == 1 || lstTelefonoParaMismaPersona.Count >= 2)
                            {

                                return lstTelefonoParaMismaPersona[0].IdPersona;
                            }

                            ///////               

                        }
                        else
                        {

                            // si no hay coincidencia por nombres, entonces se registra el cliente 
                            long IdPersona = RegistraPersonaFisica(ref conexion, IdAgencia, registraPersona);
                            return IdPersona.ToString();
                        }

                    }
                }
                else
                {
                    //Persona Moral

                    // Buscamos el Id de persona mediante el rfc

                    if (registraPersona.RFC == "" && registraPersona.RFC == null)
                    {
                        throw new Exception();
                    }


                    List<PersonaCliente> lstClienteMoral = new List<PersonaCliente>();
                    PersonaCliente clienteMoral = new PersonaCliente();

                    string rfcsCoincidentes = string.Empty;

                    string strRfcM = "";
                    strRfcM += "SELECT FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, FSPERFC, FDPEIDTIPO, ";
                    strRfcM += "FDPEIDTCTE, FDPEIDCLAS, FFPEALTA, FHPEALTA, FFPEESTATU, FHPEESTATU FROM PRODPERS.CTEPERSO ";
                    strRfcM += "WHERE FSPERFC = " + "'" + registraPersona.RFC.ToString().ToUpper().Trim() + "'";
                    strRfcM += " AND FDPEESTATU = 1";

                    DataTable DtRFM = dbCnx.GetDataSet(strRfcM).Tables[0];

                    if (DtRFM.Rows.Count > 0)
                    {

                        foreach (DataRow dr in DtRFM.Rows)
                        {
                            clienteMoral = new PersonaCliente();

                            clienteMoral.IdPersona = dr["FDPEIDPERS"].ToString().Trim();
                            clienteMoral.IdLegacy = dr["FSPECTEUNI"].ToString().Trim();
                            clienteMoral.IdAgencia = dr["FIPEIDCIAU"].ToString().Trim();
                            clienteMoral.Rfc = dr["FSPERFC"].ToString().Trim();
                            clienteMoral.IdTipo = dr["FDPEIDTIPO"].ToString().Trim();
                            clienteMoral.TipoCliente = dr["FDPEIDTCTE"].ToString().Trim();
                            clienteMoral.ClasePersona = dr["FDPEIDCLAS"].ToString().Trim();
                            clienteMoral.FechaAlta = dr["FFPEALTA"].ToString().Trim();
                            clienteMoral.HoraAlta = dr["FHPEALTA"].ToString().Trim();
                            clienteMoral.FechaEstatus = dr["FFPEESTATU"].ToString().Trim();
                            clienteMoral.HoraEstatus = dr["FHPEESTATU"].ToString().Trim();

                            lstClienteMoral.Add(clienteMoral);
                            rfcsCoincidentes += dr["FDPEIDPERS"].ToString().Trim() + ",";

                        }

                    }


                    if (lstClienteMoral.Count == 0)
                    {
                        long IdPersona = RegistraPersonaMoral(ref conexion, IdAgencia, registraPersona);
                        return IdPersona.ToString();
                    }


                    if (lstClienteMoral.Count == 1)
                    {
                        return lstClienteMoral[0].IdPersona;
                    }

                    if (lstClienteMoral.Count >= 2)
                    {

                        List<PersonaMoral> lstPersonaMoral = new List<PersonaMoral>();
                        PersonaMoral personaMoral = new PersonaMoral();

                        string strBPM = string.Empty;
                        strBPM += "SELECT DPERMO.*, CGRCME.FSCTCVEGRC ";
                        strBPM += "FROM PRODPERS.CTDPERMO DPERMO ";
                        strBPM += "LEFT JOIN PRODPERS.CTCGRCME CGRCME ON CGRCME.FICTIDGRCM = DPERMO.FICTIDGRCM ";
                        strBPM += "WHERE ";
                        strBPM += "FSPMRAZON = " + "'" + registraPersona.RazonSocial.ToString().ToUpper().Trim() + "' ";
                        strBPM += "AND FDPMIDPERS IN (" + rfcsCoincidentes.Substring(0, rfcsCoincidentes.Length - 1) + ") ";
                        strBPM += "AND FDPMESTATU = 1 ";


                        /*  string strBPM = string.Empty;
                          strBPM += "SELECT DPERMO.*, CGRCME.FSCTCVEGRC ";
                          strBPM += "FROM PRODPERS.CTDPERMO DPERMO ";
                          strBPM += "LEFT JOIN PRODPERS.CTCGRCME CGRCME ON CGRCME.FICTIDGRCM = DPERMO.FICTIDGRCM ";
                          strBPM += "WHERE ";
                          strBPM += "FDPMIDPERS IN (" + rfcsCoincidentes.Substring(0, rfcsCoincidentes.Length-1) + ")";
                          strBPM += "AND FDPMESTATU = 1";
                          */


                        DataTable dtPM = dbCnx.GetDataSet(strBPM).Tables[0];

                        if (dtPM.Rows.Count > 0)
                        {

                            foreach (DataRow dr in dtPM.Rows)
                            {

                                personaMoral = new PersonaMoral();
                                personaMoral.IdPersona = dr["FDPMIDPERS"].ToString().Trim();
                                personaMoral.RazonSocial = dr["FSPMRAZON"].ToString().Trim();
                                personaMoral.IdRepresentanteLegal = dr["FICTIDRPLG"].ToString().Trim();
                                personaMoral.IdGiroComercial = dr["FICTIDGRCM"].ToString().Trim();

                                lstPersonaMoral.Add(personaMoral);
                            }

                        }

                        if (lstPersonaMoral.Count == 0)
                        {
                            long IdPersona = RegistraPersonaMoral(ref conexion, IdAgencia, registraPersona);
                            return IdPersona.ToString();
                        }

                        if (lstPersonaMoral.Count == 1)
                        {

                            return lstPersonaMoral[0].IdPersona;
                        }

                        if (lstPersonaMoral.Count >= 2)
                        {

                            return lstPersonaMoral[0].IdPersona;

                        }



                    }

                }
                return ""; // quitar
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;

            }
        }

        public static string quitaAcentos(string cadena)
        {

            string cadenaA = "[á|à|ä|â]".ToString().ToUpper();
            string cadenaE = "[é|è|ë|ê]".ToString().ToUpper();
            string cadenaI = "[í|ì|ï|î]".ToString().ToUpper();
            string cadenaO = "[ó|ò|ö|ô]".ToString().ToUpper();
            string cadenaU = "[ú|ù|ü|û]".ToString().ToUpper();

            Regex reemplazaAcento_a = new Regex(cadenaA, RegexOptions.Compiled);
            Regex reemplazaAcento_e = new Regex(cadenaE, RegexOptions.Compiled);
            Regex reemplazaAcento_i = new Regex(cadenaI, RegexOptions.Compiled);
            Regex reemplazaAcento_o = new Regex(cadenaO, RegexOptions.Compiled);
            Regex reemplazaAcento_u = new Regex(cadenaU, RegexOptions.Compiled);

            cadena = reemplazaAcento_a.Replace(cadena, "A");
            cadena = reemplazaAcento_e.Replace(cadena, "E");
            cadena = reemplazaAcento_i.Replace(cadena, "I");
            cadena = reemplazaAcento_o.Replace(cadena, "O");
            cadena = reemplazaAcento_u.Replace(cadena, "U");
            return cadena;
        }

        private static long RegistraPersonaFisica(ref DVADB.DB2 conexion, int IdAgencia, DVARegistraPersona.RegistraPersona persona)
        {
            // EL RFC DEBE IR FORMATEADO CON GUIONES

            DVADB.DB2 dbCnx = conexion;

            string insertaPersona = string.Empty;
            long IdPersona = 0;
            PERSControlDeFolio folio = new PERSControlDeFolio();
            string obtieneFoliadorPersona = string.Empty;
            int max = 1;

            try
            {
                #region FOLIADOR

                obtieneFoliadorPersona += "SELECT FDFOIDFOLI, FSFODESCRI, FDFOULTFOL, FBFOESTATU FROM ";
                obtieneFoliadorPersona += "PRODPERS.CTDFOLIO ";
                obtieneFoliadorPersona += "WHERE FBFOESTATU = 1 ";
                obtieneFoliadorPersona += "AND FDFOIDFOLI = 1";

                DataTable dtFoliador = dbCnx.GetDataSet(obtieneFoliadorPersona).Tables[0];
                if (dtFoliador.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtFoliador.Rows)
                    {
                        folio = new PERSControlDeFolio();
                        folio.IdFolio = Convert.ToInt32(dr["FDFOIDFOLI"].ToString().Trim());
                        folio.Descripcion = dr["FSFODESCRI"].ToString().Trim();
                        folio.FolioActual = Convert.ToInt32(dr["FDFOULTFOL"].ToString().Trim());
                        folio.Estatus = dr["FBFOESTATU"].ToString().Trim();
                    }
                }

                PERSControlDeFolio control = folio;

                int lastIdPersona = folio.FolioActual;

                if (control != null)
                    max = control.FolioActual + 1;

                if (max <= lastIdPersona)
                    max = lastIdPersona + 1;

                if (control == null)
                {
                    control = new PERSControlDeFolio();
                    control.FolioActual = max;
                }
                else
                {
                    control.FolioActual = max;
                    // aDB.Update(aIdUser, aIdProgram, control);

                    string actualizaFoliador = "";

                    actualizaFoliador += "UPDATE PRODPERS.CTDFOLIO ";
                    actualizaFoliador += "SET FDFOULTFOL = " + max;
                    actualizaFoliador += ", USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                    actualizaFoliador += "WHERE FDFOIDFOLI = 1 ";
                    actualizaFoliador += "AND FBFOESTATU = 1";

                    dbCnx.SetQuery(actualizaFoliador);
                }
                #endregion

                insertaPersona += "select FDPEIDPERS from NEW TABLE ( ";

                insertaPersona += "INSERT INTO PRODPERS.CTEPERSO ( ";
                insertaPersona += "FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, ";
                insertaPersona += "FSPERFC, FDPEIDTIPO, FDPEIDTCTE,";
                insertaPersona += "FDPEIDCLAS, FFPEALTA, FHPEALTA, ";
                insertaPersona += "FFPEESTATU, FHPEESTATU, FDPEESTATU, ";
                insertaPersona += "USERCREAT, DATECREAT, TIMECREAT, PROGCREAT ";
                insertaPersona += ") VALUES ( ";
                insertaPersona += max + ", " + max + ", " + IdAgencia + " ";
                insertaPersona += "," + "'" + persona.RFC.ToString().ToUpper().Trim() + "'" + "," + 1 + "," + 1 + " ";
                insertaPersona += "," + 1 + "," + "CURRENT DATE" + "," + "CURRENT TIME" + ",";
                insertaPersona += "CURRENT DATE" + "," + "CURRENT TIME" + "," + 1;
                insertaPersona += "," + "'APP', CURRENT DATE, CURRENT TIME, 'APP'";
                insertaPersona += ")";

                insertaPersona += ")";


                DataTable dtPeM = dbCnx.GetDataSet(insertaPersona).Tables[0];

                if (dtPeM.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPeM.Rows)
                    {
                        IdPersona = Convert.ToInt32(dr["FDPEIDPERS"].ToString().Trim());
                    }
                }
                else
                {
                    throw new Exception();
                }


                string insertaPersFis = string.Empty;

                insertaPersFis += "INSERT INTO PRODPERS.CTCPERFI (";
                insertaPersFis += "FDPFIDPERS, FSPFNOMBRE, FSPFAPATER, ";
                insertaPersFis += "FSPFAMATER, FDPFIDSEXO, FDPFIDEDCI, ";
                insertaPersFis += "FDPFESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                insertaPersFis += ") VALUES ( ";
                insertaPersFis += IdPersona + "," + "'" + persona.Nombre.ToString().Trim().ToUpper() + "'" + "," + "'" + persona.ApellidoPaterno.ToString().Trim().ToUpper() + "'";
                insertaPersFis += "," + "'" + persona.ApellidoMaterno.ToString().Trim().ToUpper() + "'" + "," + 1 + "," + 2 + " ";
                insertaPersFis += "," + "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP' ";
                insertaPersFis += ")";

                dbCnx.SetQuery(insertaPersFis);


                if (persona.NumeroCelular != "" && persona.NumeroCelular.Length == 10)
                {

                    string Lada = string.Empty;
                    string Telefono = string.Empty;
                    if (persona.NumeroCelular.StartsWith("55") || persona.NumeroCelular.StartsWith("81") || persona.NumeroCelular.StartsWith("33"))
                    {
                        Lada = persona.NumeroCelular.Substring(0, 2);
                        Telefono = persona.NumeroCelular.Substring(2, 8);
                    }
                    else
                    {
                        Lada = persona.NumeroCelular.Substring(0, 3);
                        Telefono = persona.NumeroCelular.Substring(3, 7);
                    }



                    string insertaTelefono = string.Empty;

                    insertaTelefono += "INSERT INTO PRODPERS.CTDRPTEL (";
                    insertaTelefono += "FDPTIDPERS, FDPTCONTEL, FDPTIDTELE,";
                    insertaTelefono += "FDPTPLADA, FDPTNUMTEL, FBCTDEFAUL,";
                    insertaTelefono += "FDPTESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                    insertaTelefono += ") VALUES (";
                    insertaTelefono += IdPersona + "," + 1 + "," + 2 + ",";
                    insertaTelefono += Lada + "," + Telefono + "," + 1 + ",";
                    insertaTelefono += "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP' ";
                    insertaTelefono += ")";

                    dbCnx.SetQuery(insertaTelefono);

                }

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();

                throw new Exception();
            }


            return IdPersona;

        }

        private static long RegistraPersonaMoral(ref DVADB.DB2 conexion, int IdAgencia, DVARegistraPersona.RegistraPersona persona)
        {

            // EL RFC DEBE IR FORMATEADO CON GUIONES

            DVADB.DB2 dbCnx = conexion;

            string insertaPersona = string.Empty;
            long IdPersona = 0;
            PERSControlDeFolio folio = new PERSControlDeFolio();
            string obtieneFoliadorPersona = string.Empty;
            int max = 1;
            try
            {
                #region FOLIADOR



                obtieneFoliadorPersona += "SELECT FDFOIDFOLI, FSFODESCRI, FDFOULTFOL, FBFOESTATU FROM ";
                obtieneFoliadorPersona += "PRODPERS.CTDFOLIO ";
                obtieneFoliadorPersona += "WHERE FBFOESTATU = 1 ";
                obtieneFoliadorPersona += "AND FDFOIDFOLI = 1";




                DataTable dtFoliador = dbCnx.GetDataSet(obtieneFoliadorPersona).Tables[0];



                if (dtFoliador.Rows.Count > 0)
                {



                    foreach (DataRow dr in dtFoliador.Rows)
                    {
                        folio = new PERSControlDeFolio();



                        folio.IdFolio = Convert.ToInt32(dr["FDFOIDFOLI"].ToString().Trim());
                        folio.Descripcion = dr["FSFODESCRI"].ToString().Trim();
                        folio.FolioActual = Convert.ToInt32(dr["FDFOULTFOL"].ToString().Trim());
                        folio.Estatus = dr["FBFOESTATU"].ToString().Trim();



                    }
                }




                PERSControlDeFolio control = folio;

                int lastIdPersona = folio.FolioActual;



                if (control != null)
                    max = control.FolioActual + 1;



                if (max <= lastIdPersona)
                    max = lastIdPersona + 1;



                if (control == null)
                {
                    control = new PERSControlDeFolio();
                    control.FolioActual = max;



                }
                else
                {
                    control.FolioActual = max;
                    // aDB.Update(aIdUser, aIdProgram, control);



                    string actualizaFoliador = "";



                    actualizaFoliador += "UPDATE PRODPERS.CTDFOLIO ";
                    actualizaFoliador += "SET FDFOULTFOL = " + max;
                    actualizaFoliador += ", USERUPDAT = 'APP', DATEUPDAT = CURRENT DATE, TIMEUPDAT = CURRENT TIME, PROGUPDAT = 'APP' ";
                    actualizaFoliador += "WHERE FDFOIDFOLI = 1 ";
                    actualizaFoliador += "AND FBFOESTATU = 1";



                    dbCnx.SetQuery(actualizaFoliador);



                }




                #endregion

                insertaPersona += "select FDPEIDPERS from NEW TABLE ( ";

                insertaPersona += "INSERT INTO PRODPERS.CTEPERSO ( ";
                insertaPersona += "FDPEIDPERS, FSPECTEUNI, FIPEIDCIAU, ";
                insertaPersona += "FSPERFC, FDPEIDTIPO, FDPEIDTCTE,";
                insertaPersona += "FDPEIDCLAS, FFPEALTA, FHPEALTA, ";
                insertaPersona += "FFPEESTATU, FHPEESTATU, FDPEESTATU, ";
                insertaPersona += "USERCREAT, DATECREAT, TIMECREAT, PROGCREAT ";
                insertaPersona += ") VALUES ( ";
                insertaPersona += max + ", " + max + ", " + IdAgencia + " , ";
                insertaPersona += "'" + persona.RFC.ToString().ToUpper().Trim() + "'" + "," + 2 + "," + 1 + " ";
                insertaPersona += "," + 1 + "," + "CURRENT DATE" + "," + "CURRENT TIME" + ",";
                insertaPersona += "CURRENT DATE" + "," + "CURRENT TIME" + "," + 1;
                insertaPersona += "," + "'APP', CURRENT DATE, CURRENT TIME, 'APP'";
                insertaPersona += ")";

                insertaPersona += ")";


                DataTable dtPeM = dbCnx.GetDataSet(insertaPersona).Tables[0];

                if (dtPeM.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPeM.Rows)
                    {
                        IdPersona = Convert.ToInt32(dr["FDPEIDPERS"].ToString().Trim());
                    }
                }
                else
                {
                    dbCnx.RollbackTransaccion();
                    dbCnx.CerrarConexion();

                    throw new Exception();
                }


                string insertaPersMoral = string.Empty;

                insertaPersMoral += "INSERT INTO PRODPERS.CTDPERMO (";
                insertaPersMoral += "FDPMIDPERS, FSPMRAZON, FICTIDRPLG,FICTIDGRCM,";
                insertaPersMoral += "FDPMESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                insertaPersMoral += ") VALUES ( ";
                insertaPersMoral += IdPersona + "," + "'" + persona.RazonSocial.ToString().ToUpper().Trim() + "'" + "," + 0 + "," + 0 + ",";
                insertaPersMoral += "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP'";
                insertaPersMoral += ")";

                dbCnx.SetQuery(insertaPersMoral);

                if (persona.NumeroCelular != "" && persona.NumeroCelular.Length == 10)
                {

                    string Lada = string.Empty;
                    string Telefono = string.Empty;
                    if (persona.NumeroCelular.StartsWith("55") || persona.NumeroCelular.StartsWith("81") || persona.NumeroCelular.StartsWith("33"))
                    {
                        Lada = persona.NumeroCelular.Substring(0, 2);
                        Telefono = persona.NumeroCelular.Substring(2, 8);
                    }
                    else
                    {
                        Lada = persona.NumeroCelular.Substring(0, 3);
                        Telefono = persona.NumeroCelular.Substring(3, 7);
                    }


                    string insertaTelefono = string.Empty;

                    insertaTelefono += "INSERT INTO PRODPERS.CTDRPTEL (";
                    insertaTelefono += "FDPTIDPERS, FDPTCONTEL, FDPTIDTELE,";
                    insertaTelefono += "FDPTPLADA, FDPTNUMTEL, FBCTDEFAUL,";
                    insertaTelefono += "FDPTESTATU, USERCREAT, DATECREAT, TIMECREAT, PROGCREAT";
                    insertaTelefono += ") VALUES (";
                    insertaTelefono += IdPersona + "," + 1 + "," + 2 + ",";
                    insertaTelefono += Lada + "," + Telefono + "," + 1 + ",";
                    insertaTelefono += "1, 'APP', CURRENT DATE, CURRENT TIME, 'APP' ";
                    insertaTelefono += ")";

                    dbCnx.SetQuery(insertaTelefono);


                }

            }
            catch (Exception ex)
            {
                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                throw new Exception();
            }

            return IdPersona;

        }

    }


    public class RespuestReferencia
    {
        public string Ok { get; set; }
        public string Mensaje { get; set; }
    }
}