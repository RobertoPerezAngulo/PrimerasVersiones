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
using wsApiSeat.WS_Security;
using wsApiSeat.Models.Cotizador;

namespace wsApiSeat.Controllers
{
    public class CotizadorController : ApiController
    {
        string servicioContizador = "http://ws-smartit.divisionautomotriz.com";

        // GET api/Cotizador/GetToken
        [Route("api/Cotizador/GetToken", Name = "GetToken")]
        public TokenCotizador GetToken()
        {
            TokenCotizador tokenCotizador = new TokenCotizador();
            
            try
            {
                var client = new RestClient("http://201.147.99.235:4290/cotizadorgam/public/api/v1/login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "laravel_session=qahQB9vVy3wQ2TPBrTgkw60d6ZX9BLHYQ0Z6YaQY");
                request.AddParameter("application/json", "{\n    \"username\" : \"gam\",\n    \"password\" : \"Gam7h8j9k!!\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                Console.WriteLine(response.Content);

                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                List<Dictionary<string, object>> promocionesAutofin = deserial.Deserialize<List<Dictionary<string, object>>>(response);

                foreach (Dictionary<string, object> promocionAutofin in promocionesAutofin)
                {
                    tokenCotizador = new TokenCotizador();

                   tokenCotizador.token = (promocionAutofin["token"] ?? String.Empty).ToString();


                }
            }
            catch (Exception)
            {
                
            }
            return tokenCotizador;
        }

        // POST api/Cotizador/PostCotizador
        [Route("api/Cotizador/PostCotizador", Name = "PostCotizador")]
        public CotizadorModel PostCotizador([FromBody] RequerimientosCotizacion requem)
        {
            CotizadorModel cotizadorModel = new CotizadorModel();
            try
            {     
                string authorizationStr = "Bearer " + requem.token;

                var client = new RestClient("http://201.147.99.235:4290/cotizadorgam/public/api/v1/cotizadorsimpleauto");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                //request.AddHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjQwN2U5ODgzNDFmNTg0MGIzMWIwODBhNTkxZDQ4OGZiNWI3NTY2NmNiZmEyM2Q2MzVmNzk0OTU2M2YwMmMwZjVhZjkwYWQ1ZjVhMjI4YWE5In0.eyJhdWQiOiIxIiwianRpIjoiNDA3ZTk4ODM0MWY1ODQwYjMxYjA4MGE1OTFkNDg4ZmI1Yjc1NjY2Y2JmYTIzZDYzNWY3OTQ5NTYzZjAyYzBmNWFmOTBhZDVmNWEyMjhhYTkiLCJpYXQiOjE1OTYxMzIwMDcsIm5iZiI6MTU5NjEzMjAwNywiZXhwIjoxNjI3NjY4MDA3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.uv-nP9FekDbPG2NSiLRv1O-2Fa90EPOZI0NCrRYodmvGLLr7TQrGFXKQ97AfZFrLVaVm0jzYibdcOrHzE4gtBPe0RJIcCj1eBbM59Fdr9jtoHHcprnWwv-wdS7I0D5EAMauX4rqizqrTkHJd9x8-NQgvFplmcjC8ZmidNAUYYYbO-KjfvEaZ1oiacm6qa741cJHbMHDDM44Ei91jZPOxySWFCtqWEW31x6lVVztuxN2HkgDb3ql9oixrSFpk0THAO2j_7YhvDRxN_AAJLAWBRYjPPALgiMAGmf0L0MNG-tMfmdSZpMO_eyrjXpMBVHpVm5LzL-3MVm5ewfyVS3IPINhkVZwwGvXJitYiiJ1AsCNDjC5859jP4tK9ca8uNyF2pgb7ZC5di2wjiP2Gc-Gj066CBQJ9HHlOTAIxWin0myCPvopJX2jl0PsfAIk-cPQGJubXbWaL_8wNewc-ot0kZ0kfayj4LzQ89DZQveZPCl80jFurUlPAc8o8OOGi3vr9a591Bt57zLyr7JAwam4No9vXaz9Ps3Qh2S5PnagIXi8Lk38TwdqaplSo29kZzKNRsiIfSxxrsQxxHJgLRhHgJywRNGI8NGbhzp8AuejcQkYLp-pUz8lRIE4ZfWQKfYAX6u2lgMXyuN4pLR2B0fx4gtL5peSkDBgeV-WEr5zcF8g");
                request.AddHeader("Authorization", authorizationStr);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "laravel_session=qahQB9vVy3wQ2TPBrTgkw60d6ZX9BLHYQ0Z6YaQY");
                request.AddParameter("application/json", "{\n    \"model\" : \"" + requem.model + "\" ,\n    \"amount\" : \""+ requem.amount + "\" ,\n    \"advance_payment\" : \"" + requem.advance_payment + "\" \n}\n", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                //Console.WriteLine(response.Content);

         
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

                List<Dictionary<string, object>> reponseModel = deserial.Deserialize<List<Dictionary<string, object>>>(response);

                foreach (Dictionary<string, object> item in reponseModel)
                {
                    cotizadorModel = new CotizadorModel();

                    List<Dictionary<string, object>> quotation_datas = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(item["quotation_data"].ToString());

                    foreach (Dictionary<string, object> quotation_data in quotation_datas)
                    {
                        quotation_dataItem respuesta = new quotation_dataItem();

                        respuesta.first_pay_month = (quotation_data["first_pay_month"] ?? string.Empty).ToString();
                        respuesta.total_amount = (quotation_data["total_amount"] ?? string.Empty).ToString();
                        respuesta.administration_pay = (quotation_data["administration_pay"] ?? string.Empty).ToString();
                        respuesta.term = (quotation_data["term"] ?? string.Empty).ToString();

                        cotizadorModel.quotation_data.Add(respuesta);
                    }

                    var quotation_data_view = item["quotation_data_view"];
                    Dictionary<string, object> quotation_data_viewDict = new Dictionary<string, object>();
                    quotation_data_viewDict = quotation_data_view as Dictionary<string, object>;

                    cotizadorModel.quotation_data_view.first_pay_month = (quotation_data_viewDict["first_pay_month"] ?? string.Empty).ToString();
                    cotizadorModel.quotation_data_view.total_amount = (quotation_data_viewDict["total_amount"] ?? string.Empty).ToString();
                    cotizadorModel.quotation_data_view.administration_pay = (quotation_data_viewDict["administration_pay"] ?? string.Empty).ToString();
                    cotizadorModel.quotation_data_view.term = (quotation_data_viewDict["term"] ?? string.Empty).ToString();

                    cotizadorModel.respuesta.Ok = "SI";
                    cotizadorModel.respuesta.Mensaje = "";
                }

            }
            catch (Exception)
            {

            }
            return cotizadorModel;
        }

    }
}


