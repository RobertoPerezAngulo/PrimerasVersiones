using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using wsApiSeat.Models;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace wsApiSeat.Controllers
{
    public class URLsController : ApiController
    {

        // GET api/URLs/GetObtenerRutas360
        [Route("api/URLs/GetObtenerRutas360", Name = "Obtiene las rutas 360")]
        public Rutas360 GetObtenerRutas360()
        {

            Rutas360 rutas = new Rutas360();

            try
            {
                string strJSON = File.ReadAllText(@"c:\inetpub\wwwroot\wsApiSeat\ResourcesJson\Rutas360.json");
                rutas = JsonConvert.DeserializeObject<Rutas360>(strJSON);
            }
            catch (Exception _exc)
            {
                rutas = null;

            }

            return rutas;

        }


    }
}
