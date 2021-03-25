using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using wsApiSeat.Models;
using wsApiSeat.Services;

namespace wsApiSeat.Controllers
{
    public class ValuesController : ApiController
    {
        [Route("api/ValuesController/GetEnvioCorreoTest", Name = "EnvioCorreoTest")]
        public string EnvioCorreoTest()
        {
            CorreoService _ob = new CorreoService();
            List<string> ob = new List<string>();
            ob.Add("jrperez@grupoautofin.com");
            //_ob.EnviarCorreoGerentesYBack("FURIA SEAT", "8", "Se ha registrado un folio nuevo", ob);

            string subject = "Furia SEAT";
            EnvioCorreoSoporteCupraYGerentes hiloEnvioCorreoAGerentesYBack = new EnvioCorreoSoporteCupraYGerentes(subject,"5", "<strong> EC: </strong> Se ha registrado un folio nuevo ", "<strong> PP: </strong> Apártalo", ob);
            Thread hilo = new Thread(new ThreadStart(hiloEnvioCorreoAGerentesYBack.EnvioCorreoGerentesYBack));
            hilo.Start();

            return "";
        }


        [Route("api/ValuesController/GetRecuperaversionColorTest", Name = "RecuperaversionColorTest")]
        public string RecuperaversionColor(string IdVersion, string IdColor)
        {
            List<Models.Color> _model = new List<Models.Color>();
            try
            {
                List<ModelosCarros> respuesta = new List<ModelosCarros>();
                string strJSON = File.ReadAllText(@"C:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Versiones.json");
                respuesta = JsonConvert.DeserializeObject<List<ModelosCarros>>(strJSON);
                #region Encuentra Color 
                foreach (var _respuesta in respuesta)
                {
                    foreach (var versiones in _respuesta.Versiones)
                    {
                        foreach (var _preciotransmision in versiones.PrecioTransmision)
                        {
                            if (_preciotransmision.IdVersion == IdVersion)
                            {
                                _model = versiones.Colores.Where(x => x.IdColor == IdColor).ToList();
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception)
            {
                return "Error en el color";
            }

            return _model.Select(x => x.Ruta).First().ToString();
        }

        [Route("api/ValuesController/GetRecuepraModelosTest", Name = "RecuepraModelosTest")]
        public List<ModelosCarros> RecuepraModelos()
        {
            List<ModelosCarros> respuesta = new List<ModelosCarros>();
            List<PrecioVenta> _precioventa = new List<PrecioVenta>(); ;
            List<Models.Version> _versionesList = new List<Models.Version>();
            List<ModelosCarros> _modelos = new List<ModelosCarros>();
            try
            {
                
                string strJSON = File.ReadAllText(@"C:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Versiones.json");
                respuesta = JsonConvert.DeserializeObject<List<ModelosCarros>>(strJSON);

                foreach (ModelosCarros item in respuesta)
                {
                    foreach (Models.Version itemversion in item.Versiones)
                    {
                        foreach (PrecioVenta itemprecioventa in itemversion.PrecioTransmision)
                        {
                            PrecioVenta _pre = new PrecioVenta()
                            {
                                IdVersion = itemprecioventa.IdVersion,
                                Transmision = itemprecioventa.Transmision,
                                Total = itemprecioventa.Total,
                                SubTotal = Math.Round(Convert.ToDecimal(itemprecioventa.Total) / Convert.ToDecimal(1.16),2).ToString(),
                                Iva = Math.Round((Convert.ToDecimal(itemprecioventa.Total) / Convert.ToDecimal(1.16)) * (Convert.ToDecimal(0.16)),2).ToString()
                            };
                            //Agregamos el precio transmision
                            _precioventa.Add(_pre);
                        }

                        Models.Version _version = new Models.Version()
                        {
                            Nombre = itemversion.Nombre,
                            Ruta360 = itemversion.Ruta360,
                            RutaVideo = itemversion.RutaVideo,
                            RutaFichaTecnica = itemversion.RutaFichaTecnica,
                            PrecioTransmision = itemversion.PrecioTransmision,
                            Colores = itemversion.Colores
                        };
                        //Agregamos las versiones de los precios de los carros
                        _versionesList.Add(_version);
                        _precioventa.Clear();
                    }
                    ModelosCarros _carros = new ModelosCarros()
                    {
                        IdModelo = item.IdModelo,
                        NombreModelo = item.NombreModelo,
                        AnioModelo = item.AnioModelo,
                        Motor = item.Motor,
                        Potencia = item.Potencia,
                        Aceleracion = item.Aceleracion,
                        VelocidadMax = item.VelocidadMax,
                        Versiones = item.Versiones,
                        GamaColores = item.GamaColores,
                        ConsumoGasolina = item.ConsumoGasolina,
                        Transmision = item.Transmision
                    };
                    //Agregamos los modelos de los carros
                    _modelos.Add(_carros);
                    _versionesList.Clear();
                }

            }
            catch (Exception)
            {

                throw;
            }

            
            return _modelos; //Newtonsoft.Json.JsonConvert.DeserializeObject<List<ModelosCarros>>(_modelos); ;
        }


    }
}
