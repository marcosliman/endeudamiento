using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tesoreria.Helper;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.Globalization;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using System.Security.Claims;
using tesoreria.Utils;
using Microsoft.Graph;
using Azure.Identity;
using gestor.Helpers;
namespace tesoreria.Controllers
{
    public class OutlookController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato
        public ActionResult Bandeja()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("BandejaOutlook", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {                
                return View();
            }
        }
        public async Task<JsonResult> Bandeja_Read()
        {
            string correosJson = "{}";
            IConfidentialClientApplication app = MsalAppBuilder.BuildConfidentialClientApplication();
            AuthenticationResult result = null;
            var account = await app.GetAccountAsync(seguridad.AccountId);
            string[] scopes = { "Mail.Read" };
            // Value from app registration
            var clientId = AuthenticationConfig.ClientId;

            // using Azure.Identity;
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            try
            {
                Seguridad seguridadOld = new Seguridad();
                seguridadOld = seguridad;
                //try to get token silently
                var accesToken = await GraphHelper.GetUserAccessTokenAsync();
                var userDetails = await GraphHelper.GetUserDetailsAsync(accesToken);
                //tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
                var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", accesToken);
                    }));
                var messages = await graphClient.Me.MailFolders.Inbox.Messages
                .Request()
                //.Select(m => new {
                //    m.From,
                //    m.Id,
                //    m.Subject,
                //    m.IsRead,
                //    m.BodyPreview,
                //    m.ReceivedDateTime,                    
                //    //m.From.EmailAddress.Address
                //})
                //.OrderBy($"Subject desc")
                //.OrderBy($"Subject")
                //.Filter($"startsWith(From/EmailAddress/Address,'sgi@mqs.cl')")
                .OrderBy($"ReceivedDateTime desc")
                .Top(50)
                .GetAsync();
                var listaRetorno = messages;//.Where(c => c.From.EmailAddress.Address == "sgi@mqs.cl");
                var json = Json(listaRetorno, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = 500000000;
                return json;
            }
            catch (MsalUiRequiredException)
            {
                ViewBag.Relogin = "true";
                List<ValueCorreo> retorno = new List<ValueCorreo>();
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            catch (Exception eee)
            {
                ViewBag.Error = "An error has occurred. Details: " + eee.Message;
                List<ValueCorreo> retorno = new List<ValueCorreo>();
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            //try
            //{
            //    Seguridad seguridadOld = new Seguridad();
            //    seguridadOld=seguridad;
            //    //try to get token silently
            //    result = await app.AcquireTokenSilent(scopes, account).ExecuteAsync().ConfigureAwait(false);
            //    //tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;

            //}
            //catch (MsalUiRequiredException)
            //{
            //    ViewBag.Relogin = "true";
            //}
            //catch (Exception eee)
            //{
            //    ViewBag.Error = "An error has occurred. Details: " + eee.Message;
            //    //return View();
            //}

            //if (result != null)
            //{
            //    // Use the token to read email
            //    HttpClient hc = new HttpClient();
            //    hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

            //    HttpResponseMessage hrm = await hc.GetAsync("https://graph.microsoft.com/v1.0/me/messages?$filter=mail eq 'sgi@mqs.cl'&top=50");
            //    var rez = hrm.Content.ReadAsStringAsync();
            //    correosJson = rez.Result;

            //}
            ////var respuesta2=JsonConvert.SerializeObject(correosJson);
            //var respuesta = JsonConvert.DeserializeObject<CorreoMicrosoftViewModels>(correosJson);            
            //if (respuesta.value != null)
            //{
            //    var listaRetorno = respuesta.value.Select(c => new { c.id, c.subject, c.bodyPreview, c.receivedDateTime, c.isRead, c.from }).ToList();
            //    return Json(listaRetorno, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    List<ValueCorreo> retorno = new List<ValueCorreo>();
            //    return Json(retorno, JsonRequestBehavior.AllowGet);
            //}
            //return rawResponse;

        }
    }
}