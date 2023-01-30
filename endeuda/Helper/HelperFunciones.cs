using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using modelo.ViewModel;
namespace tesoreria.Helper
{
    public class HelperFunciones
    {
        public bool envioCorreo(string mensaje, string to, string asunto, List<CachedUser> correos)
        {
            MailMessage correo = new MailMessage();
            var MailProduccion = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["MailProduccion"].ToString());
            var MailTest = System.Configuration.ConfigurationManager.AppSettings["MailTest"].ToString();
            correo.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["SmtpUserName"]);
            if (to != "" && to != null)
            {
                if (MailProduccion == true)
                {
                    correo.To.Add(to);
                }
                else
                {
                    correo.To.Add(MailTest);
                }
            }
            if (correos != null)
            {
                foreach (var email in correos)
                {
                    if (MailProduccion == true)
                    {
                        correo.To.Add(email.Email);
                        var algo = to;
                    }
                    else
                    {
                        correo.To.Add(MailTest);
                    }
                }
            }
            //correo.CC.Add("marcosliman20@gmail.com "); // correo de prueba
            //correo.To.Add("marcos.liman@solucionesreloncavi.cl");
            correo.Subject = asunto;
            correo.Body = mensaje;
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;

            // configuracion del servidor
            SmtpClient smtp = new SmtpClient();
            smtp.Host = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            smtp.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"].ToString());
            smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SmtpSsl"].ToString()); ;
            smtp.UseDefaultCredentials = true;
            string sCuentaCorreo = System.Configuration.ConfigurationManager.AppSettings["SmtpUserName"];
            string sPassword = System.Configuration.ConfigurationManager.AppSettings["SmtpPassword"];
            smtp.Credentials = new System.Net.NetworkCredential(sCuentaCorreo, sPassword);
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                smtp.Send(correo);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }    
}