using DVAModelsReflection.Models.SERV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiSeat.Models.Cotizador
{

    public class RequerimientosCotizacion {
        public string advance_payment { get; set; }
        public string amount { get; set; }
        public string model { get; set; }
        public string token { get; set; }
    }

    public class CotizadorModel
    {
        public CotizadorModel()
        {
            quotation_data = new List<quotation_dataItem>();
            quotation_data_view = new quotation_dataItem();
            respuesta = new Respuesta()
            {
                Mensaje = "Error",
                Ok = "NO",
                Objeto = ""
            };
        }
        public List<quotation_dataItem> quotation_data { get; set; }
        public quotation_dataItem quotation_data_view { get; set; }

        public Respuesta respuesta { get; set; }
    }



    public class quotation_dataItem {

        public string first_pay_month { get; set; }
        public string total_amount { get; set; }
        public string administration_pay { get; set; }
        public string term { get; set; }
    }
}