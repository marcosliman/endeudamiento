using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace bodega.Helper
{
    public class HelperFunciones
    {
        
        public bool envioCorreo(string mensaje,string to,string asunto)
        {
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["SmtpUserName"]);
            correo.To.Add(to);
            correo.To.Add("marcosliman20@gmail.com "); // correo de pruea
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

            try
            {
                smtp.Send(correo);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }

    
}