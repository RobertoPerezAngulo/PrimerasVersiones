using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace wsApiSeat.Services
{
    public class CorreoService
    {
        public CorreoService()
        {

        }

        public void Enviar(string Subject,string folio,string Mensaje, string Correo)
        {
            EnviarCorreo(Subject, folio, Mensaje, Correo);
        }

        public void EnviarHtml(string Subject, string Correo, string Html)
        {
            EnviarCorreoHtml(Subject, Correo, Html);
        }

        private void EnviarCorreo(string subject ,string folio ,string mensaje, string correo)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "RXPJPJJ2013llx";
            string smtpServidor = "smtp.office365.com";
            //string subject = "ProcesoCorporativoAutofin";

            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(correo));
            message.From = from;

            message.Subject = subject;
            message.Body = "<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>FURIA</title> </head> <body style= 'padding: 10%'> <table align='center' border='1px' cellpadding='0' cellspacing='0' width='40%'> <tr> <td style='text-align: center;'> <p> <h1>Actualización de compra</h1> </p> <p> Folio número: <strong> " + folio + "</strong> </p> </td> </tr> <tr> <td style='padding: 20px 0 30px 0; text-align: center;'>" + mensaje + "</td> </tr> <tr> <td style='text-align: center;'> SEAT FURIA MOTORS </td> </tr> </table> </body></ html > ";
            message.IsBodyHtml = true;
          
            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //message.SubjectEncoding = System.Text.Encoding.UTF8;

            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;

            //client.SendCompleted += client_EnviarCorreoCompleado;

            client.Send(message);
            //client.SendAsync(message, "Enviado");
        }

        public void EnviarCorreoGerentesYBack(string subject, string folio, string EC, string PP, List<string> Destinatarios)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "RXPJPJJ2013llx";
            string smtpServidor = "smtp.office365.com";
            //string subject = "ProcesoCorporativoAutofin";

           // string filename = @"C:\correo\politicas.pdf";
           // Attachment data = new Attachment(filename, MediaTypeNames.Application.Octet);

            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailMessage message = new MailMessage();


            foreach (string correoDestino in Destinatarios)
            {
                message.To.Add(new MailAddress(correoDestino));
            }
            

            message.From = from;
            message.Subject = subject;
            message.Body = "<!DOCTYPE html> <html lang='en'> <head> <meta charset='UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <title>FURIA</title> </head> <body style= 'padding: 10%'> <table align='center' border='1px' cellpadding='0' cellspacing='0' width='40%'> <tr> <td style='text-align: center;'> <p> <h1>Folio nuevo</h1> </p> <p> Folio número: <strong> " + folio + "</strong> </p> </td> </tr> <tr> <td style='padding: 20px 0 30px 0; text-align: center;'> <p>" + EC + "</p> <p> " + PP + "</p> </td> </tr> <tr> <td style='text-align: center;'> SEAT FURIA MOTORS </td> </tr> </table> </body></ html > ";
            message.IsBodyHtml = true;
            //            message.Attachments.Add(data);

            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //message.SubjectEncoding = System.Text.Encoding.UTF8;

            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;

            //client.SendCompleted += client_EnviarCorreoCompleado;

            client.Send(message);
            //client.SendAsync(message, "Enviado");
        }

        //public void EnviarCorreoCliente(string subject, string correo, string mensaje, int IdTipoCita, string StrHtml, int IdAgencia)
        public void EnviarCorreoCliente(string subject, string correo, string mensaje, string StrHtml, int IdAgencia)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "RXPJPJJ2013llx";
            string smtpServidor = "smtp.office365.com";

            string filename = string.Empty;

            switch (IdAgencia)
            {
                case 12:
                    #region Monterrey
                    //switch (IdTipoCita)
                    //{
                    //    case 1:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Monterrey\politicaTestDriveMonterrey.pdf";
                    //        break;
                    //    case 2:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Monterrey\politicaEntregaMonterrey.pdf";
                    //        break;
                    //    case 3:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Monterrey\politicaFacturacionMonterrey.pdf";
                    //        break;
                    //    default:
                    //        break;
                    //}
                    #endregion
                    filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Monterrey\politicasMonterrey.pdf";
                    break;
                case 35:
                    #region Acapulco
                    //switch (IdTipoCita)
                    //{
                    //    case 1:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Acapulco\politicaTestDriveAcapulco.pdf";
                    //        break;
                    //    case 2:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Acapulco\politicaEntregaAcapulco.pdf";
                    //        break;
                    //    case 3:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Acapulco\politicaFacturacionAcapulco.pdf";
                    //        break;
                    //    default:
                    //        break;
                    //}
                    #endregion
                    filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\Acapulco\politicasAcapulco.pdf";
                    break;
                case 36:
                    #region CDMX
                    //switch (IdTipoCita)
                    //{
                    //    case 1:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\CDMX\politicaTestDriveCDMX.pdf";
                    //        break;
                    //    case 2:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\CDMX\politicaEntregaCDMX.pdf";
                    //        break;
                    //    case 3:
                    //        filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\CDMX\politicaFacturacionCDMX.pdf";
                    //        break;
                    //    default:
                    //        break;
                    //}
                    #endregion
                    filename = @"c:\inetpub\wwwroot\wsApiSeat\Resources\Politicas\CDMX\politicasCDMX.pdf";
                    break;
                default:
                    break;
            }
            
             Attachment data = new Attachment(filename, MediaTypeNames.Application.Octet);

            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailMessage message = new MailMessage();            

            message.To.Add(new MailAddress(correo));            


            message.From = from;

            message.Subject = subject;

            string mensajeC = "<!doctype html>" + StrHtml;
            message.Body = mensajeC;
            message.IsBodyHtml = true;


           // message.Body = mensaje;

            message.Attachments.Add(data);
                      

            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //message.SubjectEncoding = System.Text.Encoding.UTF8;

            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;

            //client.SendCompleted += client_EnviarCorreoCompleado;

            client.Send(message);
            //client.SendAsync(message, "Enviado");
        }




        public static void EnviarCorreoCotizacionSeguro(string correo, string msg)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "RXPJPJJ2013llx";
            string smtpServidor = "smtp.office365.com";
            string mensaje = "<!doctype html>" + msg;

            string subject = "Cotización de Seguro MiAuto";

            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailAddress to = new MailAddress(correo);
            MailMessage message = new MailMessage(from, to);

            message.Body = mensaje;
            //message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Subject = subject;
            // message.SubjectEncoding = System.Text.Encoding.UTF8;

            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;
            client.Send(message);
        }

        private void EnviarCorreoHtml(string subject, string correo, string html)
        {
            string mailFrom = "notificaciones@grupoautofin.com";
            string password = "RXPJPJJ2013llx";
            string smtpServidor = "smtp.office365.com";

            SmtpClient client = new SmtpClient(smtpServidor, 587);
            MailAddress from = new MailAddress(mailFrom);
            MailAddress to = new MailAddress(correo);
            MailMessage message = new MailMessage(from, to);

            message.Subject = subject;

            string mensaje = "<!doctype html>" + html;
            message.Body = mensaje;
            message.IsBodyHtml = true;

            client.Credentials = new System.Net.NetworkCredential(mailFrom, password);
            client.EnableSsl = true;

            //client.SendCompleted += client_EnviarCorreoCompleado;

            client.Send(message);
            //client.SendAsync(message, "Enviado");
        }

        void client_EnviarCorreoCompleado(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        public static string obtenerURLServidor()
        {
            HttpRequest request = HttpContext.Current.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";

            return baseUrl;
        }

        public static  string leerArchivoWeb(string url)
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
    }

    public class HiloEnvioCorreo
    {
        protected string subject;
        protected string correo;
        protected string strHtml;

        public HiloEnvioCorreo(string Subject, string Correo, string StrHtml)
        {
            this.subject = Subject;
            this.correo = Correo;
            this.strHtml = StrHtml;
        }

        public void EnvioCorreo()
        {
            try
            {
                CorreoService servicioCorreo = new CorreoService();
                servicioCorreo.EnviarHtml(subject, correo, strHtml);

            }
            catch (Exception e)
            {

            }
        }

        public void EnvioCorreoCasaTrust()
        {
            try
            {
                CorreoService servicioCorreo = new CorreoService();
                //servicioCorreo.EnviarHtml(subject, "centraldeusados_cpe@grupoautofin.com", strHtml);
                servicioCorreo.EnviarHtml(subject, "aramirez@grupoautofin.com", strHtml);

            }
            catch (Exception e)
            {

            }
        }
    }

        
    public class HiloEnvioCorreoSoporte
    {
        protected string subject;
        protected string correo;
        protected string mensaje;
        protected string folio;

        public HiloEnvioCorreoSoporte(string Subject, string Correo, string Mensaje ,string folio)
        {
            this.subject = Subject;
            this.correo = Correo;
            this.mensaje = Mensaje;
            this.folio = folio;
        }
        
        public void EnvioCorreoSoporte()
        {
            try
            {
                CorreoService servicioCorreo = new CorreoService();
                servicioCorreo.Enviar(subject, folio,mensaje, correo);

            }
            catch (Exception e)
            {

            }
        }
    }


    public class EnvioCorreoSoporteCupraYGerentes
    {
        protected string subject;       
        protected string folio;
        protected string EC;
        protected string PP;
        protected List<string> Destinatarios;

        public EnvioCorreoSoporteCupraYGerentes(string Subject, string folio, string EC, string PP, List<string> Destinatarios)
        {
            this.subject = Subject;           
            this.folio = folio;
            this.EC = EC;
            this.PP = PP;
            this.Destinatarios = Destinatarios;
        }

        public void EnvioCorreoGerentesYBack()
        {
            try
            {
                CorreoService servicioCorreo = new CorreoService();
                servicioCorreo.EnviarCorreoGerentesYBack(subject,folio,EC,PP, Destinatarios);

            }
            catch (Exception e)
            {

            }
        }
    }




    public class EnvioCorreoCliente
    {
        protected string subject;
        protected string Correo;
        protected string Mensaje;       
        //protected int IdTipoCita;
        protected string strHtml;
        protected int IdAgencia;

        public EnvioCorreoCliente(string Subject, string Correo, string Mensaje, string strHtml, int IdAgencia)
        {
            this.subject = Subject;
            this.Mensaje = Mensaje;
            this.Correo = Correo;
            this.strHtml = strHtml;
            this.IdAgencia = IdAgencia;
        }

        public void EnviarCorreoCliente()
        {
            try
            {
                CorreoService servicioCorreo = new CorreoService();
                servicioCorreo.EnviarCorreoCliente(subject, Correo, Mensaje, strHtml, IdAgencia);

            }
            catch (Exception e)
            {

            }
        }
    }



}