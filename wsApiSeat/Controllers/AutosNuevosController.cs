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
using wsApiSeat.Helpers;
using System.Threading.Tasks;
using DVAModelsReflection.Models.AUT;

namespace wsApiSeat.Controllers
{
    public class AutosNuevosController : ApiController
    {
        string servicioAutofin = "http://ws-smartit.divisionautomotriz.com";

        // GET api/AutosNuevos/GetObtenerExistencias
        [Route("api/AutosNuevos/GetObtenerExistencias", Name = "GetObtenerExistencias")]
        public async Task<Respuesta> GetObtenerExistencias()
        {
            Respuesta respuesta = new Respuesta();
            JavaScriptSerializer jss = new JavaScriptSerializer();      
            try
            {
                //List<AutoNuevo> autosNuevos = new List<AutoNuevo>();
                //String autosNuevosStr = await HttpHelper.GetStr<String>("/wsApiJato/api/Jato/GetMarca?NombreMarca=CUPRA"); JRPA
                String autosNuevosStr = await HttpHelper.GetStr<String>("/wsApiJato/api/Jato/GetMarca?NombreMarca=SEAT");

                respuesta.Ok = "SI";
                respuesta.Mensaje = "Respuesta Exitosa";
                //respuesta.Objeto = jss.Serialize(autosNuevosStr);
                respuesta.Objeto = autosNuevosStr;
                return respuesta;
            }
            catch (Exception ex)
            {
                //dbCnx.RollbackTransaccion();
                //dbCnx.CerrarConexion();
                respuesta.Ok = "NO";
                respuesta.Mensaje = "Error Intente de nuevo mas tarde";
                respuesta.Objeto = "";
                return respuesta;
            }
        }

        // GET api/AutosNuevos/GetObtenerMarcas
        [Route("api/AutosNuevos/GetObtenerMarcas", Name = "GetObtenerMarcas")]
        public List<AutoNuevo> GetObtenerMarcas()
        {
            List<AutoNuevo> autosNuevos = new List<AutoNuevo>();

            try
            {
                //var cliente = new RestClient(servicioAutofin + "/wsApiJato/api/Jato/GetMarca?NombreMarca=CUPRA"); JRPA
                var cliente = new RestClient(servicioAutofin + "/wsApiJato/api/Jato/GetMarca?NombreMarca=SEAT");
                cliente.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = cliente.Execute(request);
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                List<Dictionary<string, object>> promocionesAutofin = deserial.Deserialize<List<Dictionary<string, object>>>(response);
                foreach (Dictionary<string, object> promocionAutofin in promocionesAutofin)
                {
                    AutoNuevo autoNuevo = new AutoNuevo();
                    
                    #region CapturaMarca
                    autoNuevo.IdMarca = (promocionAutofin["IdMarca"] ?? String.Empty).ToString();
                    autoNuevo.Nombre = (promocionAutofin["Nombre"] ?? String.Empty).ToString();
                    autoNuevo.UrlMarca = (promocionAutofin["UrlMarca"] ?? String.Empty).ToString();
                    autoNuevo.Armadora = (promocionAutofin["Armadora"] ?? String.Empty).ToString();
                    autoNuevo.UrlArmadora = (promocionAutofin["UrlArmadora"] ?? String.Empty).ToString();

                    #endregion

                    List<Dictionary<string, object>> Modelos = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(promocionAutofin["Modelos"].ToString());

                    //var Modelos = promocionAutofin["Modelos"];
                    foreach (Dictionary<string, object> modelo in Modelos)
                    {
                        #region Modelos
                        Modelos modeloNuevo = new Modelos();
                        modeloNuevo.IdModelo = (modelo["IdModelo"] ?? String.Empty).ToString();
                        modeloNuevo.Nombre = (modelo["Nombre"] ?? String.Empty).ToString();
                        modeloNuevo.UrlModelo = (modelo["UrlModelo"] ?? String.Empty).ToString(); 
                        #endregion

                        List<Dictionary<string, object>> Anios = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(modelo["Anios"].ToString());
                        foreach (Dictionary<string, object> anio in Anios)
                        {
                            Anios anioNW = new Anios();
                            anioNW.Numero = (anio["Numero"] ?? string.Empty).ToString();

                            List<Dictionary<string, object>> Versiones = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(anio["Versiones"].ToString());
                           foreach (Dictionary<string, object> version in Versiones)
                           {
                                Versiones ver = new Versiones();
                                #region CapturaVersiones
                                ver.IdVehiculo = (version["IdVehiculo"] ?? string.Empty).ToString();
                                ver.NombreCorto = (version["NombreCorto"] ?? string.Empty).ToString();
                                ver.Nombre = (version["Nombre"] ?? string.Empty).ToString();
                                ver.NumeroPuertas = (version["NumeroPuertas"] ?? string.Empty).ToString();
                                ver.RuedasMotriz = (version["RuedasMotriz"] ?? string.Empty).ToString();
                                ver.Transmision = (version["Transmision"] ?? string.Empty).ToString();
                                ver.Precio = (version["Precio"] ?? string.Empty).ToString();
                                ver.RutaFoto = (version["RutaFoto"] ?? string.Empty).ToString();
                                ver.Estilo = (version["Estilo"] ?? string.Empty).ToString();
                                ver.Descripcion = (version["Descripcion"] ?? string.Empty).ToString();
                                ver.ConsumoGasolinaCiudad = (version["ConsumoGasolinaCiudad"] ?? string.Empty).ToString();
                                ver.CodigoManufactura = (version["CodigoManufactura"] ?? string.Empty).ToString();
                                #endregion

                                #region CapturaDetallesVersiones

                                var cliente2 = new RestClient(servicioAutofin + "/wsApiJato/api/Jato/GetVersion?IdVehiculo=" + ver.IdVehiculo);
                                cliente.Timeout = -1;
                                var request2 = new RestRequest(Method.GET);
                                IRestResponse response2 = cliente2.Execute(request2);
                                RestSharp.Deserializers.JsonDeserializer deserial2 = new RestSharp.Deserializers.JsonDeserializer();
                                Dictionary<string, object> versionItem = deserial2.Deserialize<Dictionary<string, object>>(response2);

                                #region Equipamiento

                                List<Dictionary<string, object>> EquipamientoDic = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(versionItem["Equipamiento"].ToString());
                                foreach (Dictionary<string, object> itemEquip in EquipamientoDic)
                                {
                                    Equipamiento equipamiento = new Equipamiento(); ;

                                    equipamiento.IdCategoria = (itemEquip["IdCategoria"] ?? String.Empty).ToString();
                                    equipamiento.NombreCategoria = (itemEquip["NombreCategoria"] ?? String.Empty).ToString();
                                    equipamiento.Nombre = (itemEquip["Nombre"] ?? String.Empty).ToString();
                                    equipamiento.Disponible = (itemEquip["Disponible"] ?? String.Empty).ToString();
                                    equipamiento.Valor = (itemEquip["Valor"] ?? String.Empty).ToString();

                                    #region Atributos
                                    List<Dictionary<string, object>> Atributos = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(itemEquip["Atributos"].ToString());

                                    foreach (Dictionary<string, object> itemAtributos in Atributos)
                                    {
                                        AtributoEquip atributo = new AtributoEquip();
                                        atributo.Nombre = (itemAtributos["Nombre"] ?? string.Empty).ToString();
                                        atributo.Valor = (itemAtributos["Valor"] ?? string.Empty).ToString();

                                        equipamiento.Atributos.Add(atributo);
                                    }
                                    #endregion

                                    ver.Equipamiento.Add(equipamiento);
                                }
                                #endregion

                                #region Caracteristicas

                                List<Dictionary<string, object>> CaracteristicasDic = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(versionItem["Caracteristicas"].ToString());
                                foreach (Dictionary<string, object> item in CaracteristicasDic)
                                {
                                    Caracteristicas caracteristicas = new Caracteristicas();

                                    caracteristicas.IdCategoria = (item["IdCategoria"] ?? String.Empty).ToString();
                                    caracteristicas.NombreCategoria = (item["NombreCategoria"] ?? String.Empty).ToString();
                                    caracteristicas.Nombre = (item["Nombre"] ?? String.Empty).ToString();
                                    caracteristicas.Descripcion = (item["Descripcion"] ?? String.Empty).ToString();
                                 
                                    ver.Caracteristicas.Add(caracteristicas);
                                }
                                #endregion

                                #region Generales

                                List<Dictionary<string, object>> GeneralesLic = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(versionItem["Generales"].ToString());
                                foreach (Dictionary<string, object> item in GeneralesLic)
                                {
                                    Generales generales = new Generales();

                                    generales.IdCategoria = (item["IdCategoria"] ?? String.Empty).ToString();
                                    generales.NombreCategoria = (item["NombreCategoria"] ?? String.Empty).ToString();
                                    generales.Articulo = (item["Articulo"] ?? String.Empty).ToString();
                                    generales.Descripcion = (item["Descripcion"] ?? String.Empty).ToString();

                                    ver.Generales.Add(generales);
                                }
                                #endregion

                                #region Fotos                                

                                var fotosObj = versionItem["Fotos"];
                                Dictionary<string, object> fotosDict = new Dictionary<string, object>();
                                fotosDict = fotosObj as Dictionary<string, object>;

                                Fotos fotos = new Fotos();

                                #region exteriores
                                List<Dictionary<string, object>> exteriores = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(fotosDict["Exteriores"].ToString());

                                foreach (Dictionary<string, object> item in exteriores)
                                {
                                    CaracteristicasFotos caracteristica = new CaracteristicasFotos();
                                    caracteristica.Vista = (item["Vista"] ?? string.Empty).ToString();

                                    List<Dictionary<string, object>> rutas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(item["Rutas"].ToString());

                                    foreach (Dictionary<string, object> ruta in rutas)
                                    {
                                        RutaP rutaAdd = new RutaP();
                                        rutaAdd.Ruta = (ruta["Ruta"] ?? string.Empty).ToString();

                                        caracteristica.Rutas.Add(rutaAdd);
                                    }
                                    fotos.Exteriores.Add(caracteristica);
                                }
                                #endregion


                                #region interiores
                                List<Dictionary<string, object>> interiores = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(fotosDict["Interiores"].ToString());

                                foreach (Dictionary<string, object> item in interiores)
                                {
                                    CaracteristicasFotos caracteristica = new CaracteristicasFotos();
                                    caracteristica.Vista = (item["Vista"] ?? string.Empty).ToString();

                                    List<Dictionary<string, object>> rutas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(item["Rutas"].ToString());

                                    foreach (Dictionary<string, object> ruta in rutas)
                                    {
                                        RutaP rutaAdd = new RutaP();
                                        rutaAdd.Ruta = (ruta["Ruta"] ?? string.Empty).ToString();

                                        caracteristica.Rutas.Add(rutaAdd);
                                    }
                                    fotos.Interiores.Add(caracteristica);
                                }
                                #endregion

                                ver.Fotos = fotos;

                                #endregion

                                #endregion

                                anioNW.Versiones.Add(ver);
                            }

                            modeloNuevo.Anios.Add(anioNW);
                        }
                        autoNuevo.Modelos.Add(modeloNuevo);
                    }

                    autosNuevos.Add(autoNuevo);
                }
            }
            catch (Exception)
            {
                AutoNuevo autoNuevo = new AutoNuevo();
                autosNuevos.Add(autoNuevo);
            }

            return autosNuevos;
        }
        

    }
}
