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
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using tesoreria.Utils;
using gestor.Helpers;
using gestor.TokenStorage;

namespace tesoreria.Controllers
{
    public class HomeController : BaseController
    {
        private ErpContext db = new ErpContext();
        private readonly ILog _log = LogManager.GetLogger(typeof(HomeController));
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        string UrlSistemaBodega = System.Configuration.ConfigurationManager.AppSettings["UrlSistemaBodega"];
        string TokenMicrosoft = System.Configuration.ConfigurationManager.AppSettings["TokenMicrosoft"];
        string UrlApiMicrosoft = System.Configuration.ConfigurationManager.AppSettings["UrlApiMicrosoft"];
        public ActionResult Error(string message, string debug)
        {
            Flash(message, debug);
            Session.Abandon();
            /*HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = "/Home/Index/" },
                               OpenIdConnectAuthenticationDefaults.AuthenticationType,
                               CookieAuthenticationDefaults.AuthenticationType);*/
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> ReadMail()
        {
            IConfidentialClientApplication app = MsalAppBuilder.BuildConfidentialClientApplication();
            AuthenticationResult result = null;
            var account = await app.GetAccountAsync(ClaimsPrincipal.Current.GetMsalAccountId());
            string[] scopes = { "Mail.Read" };

            try
            {
                // try to get token silently
                result = await app.AcquireTokenSilent(scopes, account).ExecuteAsync().ConfigureAwait(false);
            }
            catch (MsalUiRequiredException)
            {
                ViewBag.Relogin = "true";
                return View();
            }

            // More code here
            return View();
        }
        public void ActualizaSesionByMicrosoft()
        {
            var respuesta = Request;
            ViewBag.Estado = 0;
            ViewBag.Mensaje = "";
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
            var usuarioCuenta = userClaims?.FindFirst("preferred_username")?.Value;
            if (usuarioCuenta != "" && usuarioCuenta != null)
            {
                if (seguridad.UserName == usuarioCuenta)
                {
                    var arrayPerfil = new List<Helper.Perfil>();
                    var usuario = db.Usuario.Where(c => c.CorreoElectronico == usuarioCuenta).FirstOrDefault();
                    if (usuario != null)
                    {
                        Seguridad seguridad = new Seguridad();
                        LoginController loginControl = new LoginController();
                        seguridad = loginControl.PerfilesUsuario(1, usuario.IdUsuario);
                        seguridad.Interno = true;
                        seguridad = loginControl.AgregarPermisos(seguridad);
                        Session["userID"] = seguridad.IdUsuario;
                        Session["userNA"] = seguridad.Nombre;
                        Session["empID"] = seguridad.EMPR_ID;
                        Session["user_EmpresaNA"] = seguridad.Nombre;
                        if (seguridad.MenuUsuario == null)
                        {
                            Session.Abandon();
                            ViewBag.Mensaje = "Perfil no cuenta con opciones asociadas";
                            ViewBag.Estado = 100;
                        }
                        else
                        {
                            if (seguridad.MenuUsuario.Count() == 0)
                            {
                                Session.Abandon();
                                ViewBag.Mensaje = "Perfil no cuenta con opciones asociadas";
                                ViewBag.Estado = 100;
                            }
                            else
                            {
                                System.Web.HttpContext.Current.Session["Seguridad"] = seguridad;
                            }
                        }
                    }
                }

            }

        }
        public ActionResult Login()
        {            
            return View();
        }
        public async Task<ActionResult> Index(string message)
        {
            var tokenStore = new SessionTokenStore(null,
                       System.Web.HttpContext.Current, ClaimsPrincipal.Current);
            var sessionMicrosoft = tokenStore.GetSession();
            var userMicrosoft = tokenStore.GetUserDetails();
            if (seguridad != null)
            {

                if (message != "" && message != null)
                {
                    Response.Redirect("/Home/SignOut");

                }
            }
            if (sessionMicrosoft != null && userMicrosoft != null)
            {
                var accesToken = await GraphHelper.GetUserAccessTokenAsync();
                var userDetails = await GraphHelper.GetUserDetailsAsync(accesToken);

                if (userDetails != null)
                {
                    var usuarioCuenta = userDetails.Email;
                    ViewBag.usuarioCuenta = usuarioCuenta;
                }
            }
            return View();

        }
        public ActionResult Inicio()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Solicitante", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresa"] = listaEmpresa;

                var bancos = (from e in db.Banco
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBancos = new SelectList(bancos.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaBancos"] = listaBancos;
                return View();
            }
            
        }
        
        public ActionResult Aprobador()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Solicitud()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult AprobarSol()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Bodeguero()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult EntregarSol()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Autherize(string usuarioCuenta, string claveCuenta, int tipoAcceso)
        {
            dynamic showMessageString = string.Empty;
            dynamic nombreEmpresa = string.Empty;
            dynamic nombreUsuario = string.Empty;
            var arrayPerfil = new List<Helper.Perfil>();
            //usuarioCuenta = "marcosliman20@gmail.com";
            //claveCuenta = Crypto.Hash("123456");
            claveCuenta = Crypto.Hash(claveCuenta);
            var usuario = db.Usuario.Where(c => c.CorreoElectronico == usuarioCuenta && c.Clave == claveCuenta).FirstOrDefault();
            if (usuario != null)
            {
                Seguridad seguridad = new Seguridad();
                LoginController loginControl = new LoginController();
                seguridad = loginControl.PerfilesUsuario(tipoAcceso, usuario.IdUsuario);
                seguridad.Interno = true;
                seguridad = loginControl.AgregarPermisos(seguridad);
                Session["userID"] = seguridad.IdUsuario;
                Session["userNA"] = seguridad.Nombre;
                Session["empID"] = seguridad.EMPR_ID;
                Session["user_EmpresaNA"] = seguridad.Nombre;
                if (seguridad.MenuUsuario == null)
                {
                    Session.Abandon();
                    showMessageString = new { Estado = 100, Mensaje = "Perfil no cuenta con opciones asociadas", ToUrl = "" };
                }
                else
                {
                    System.Web.HttpContext.Current.Session["Seguridad"] = seguridad;
                    if (usuario.CambiarClave == true)
                    {
                        showMessageString = new { Estado = 0, Mensaje = "Acceso correcto", ToUrl = "/Usuario/CambiarClave" };
                    }
                    else
                    {
                        showMessageString = new { Estado = 0, Mensaje = "Acceso correcto", ToUrl = "/Home/Inicio" };
                    }
                    
                }

            }
            else
            {
                Session.Abandon();
                showMessageString = new { Estado = 100, Mensaje = "Usuario o Clave Incorrectos", ToUrl = "" };

            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexG()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                //var empresas = (from t in db.Empresa
                //                where t.Activo == true
                //                select new RetornoGenerico { Id = t.IdEmpresa, Nombre = t.RazonSocial }).OrderBy(c => c.Id).ToList();
                //SelectList listaEmpresa = new SelectList(empresas.OrderBy(c => c.Nombre), "Id", "Nombre", 1);
                //ViewData["listaEmpresa"] = listaEmpresa;
                return View();
            }

        }
        public ActionResult RotacionProducto()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {                
                return View();
            }

        }
        public void SignIn()
        {

            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = "/Home/SignInMicrosoft/" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);

        }
        /// <summary>
        /// Send an OpenID Connect sign-out request.
        /// </summary>
        public async Task SignOut()
        {
            IConfidentialClientApplication app = MsalAppBuilder.BuildConfidentialClientApplication();
            //var accounId = (seguridad != null) ? seguridad.AccountId : "";
            //IAccount account=null;
            var tokenStore = new SessionTokenStore(null,
                       System.Web.HttpContext.Current, ClaimsPrincipal.Current);
            var sessionMicrosoft = tokenStore.GetSession();
            //if (accounId != "")
            //{
            //    account = await app.GetAccountAsync(accounId);
            //}
            //else
            //{
            //    var idVigente = ClaimsPrincipal.Current.GetMsalAccountId();
            //    account = await app.GetAccountAsync(idVigente);
            //}
            //if (account!=null)
            if (sessionMicrosoft != null)
            {
                if (seguridad == null)
                {
                    var userMicrosoft = tokenStore.GetUserDetails();
                    if (userMicrosoft != null)
                    {
                        // Send an OpenID Connect sign-out request.
                        tokenStore.Clear();
                        // Send an OpenID Connect sign-out request.                   
                        HttpContext.GetOwinContext().Authentication.SignOut(
                                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                                CookieAuthenticationDefaults.AuthenticationType);
                    }
                    else
                    {
                        Response.Redirect("/Home/Index");
                    }
                }
                else
                {
                    var userMicrosoft = tokenStore.GetUserDetails();
                    if (userMicrosoft != null)
                    {
                        if (seguridad.UserName == userMicrosoft.Email)
                        {
                            Session.Abandon();

                            tokenStore.Clear();
                            // Send an OpenID Connect sign-out request.                   
                            HttpContext.GetOwinContext().Authentication.SignOut(
                                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                                    CookieAuthenticationDefaults.AuthenticationType);
                        }
                        else
                        {
                            Session.Abandon();
                            Response.Redirect("/Home/Index");
                        }
                    }
                    else
                    {
                        Session.Abandon();
                        Response.Redirect("/Home/Index");
                    }
                }

            }
            else
            {

                Session.Abandon();
                Response.Redirect("/Home/Index");
            }
        }
        /*public void SignOut()
        {
            Session.Abandon();
            var algo=HttpContext.GetOwinContext().Authentication;
            HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);

            var owinContext = System.Web.HttpContext.Current.Request.GetOwinContext();

            var authenticationTypes = owinContext.Authentication.GetAuthenticationTypes();
            var algo =authenticationTypes.Select(c=>c.AuthenticationType).ToArray();
            owinContext.Authentication.SignOut(authenticationTypes.Select(o => o.AuthenticationType).ToArray());

        }*/
        public ActionResult AccesoMicrosoft()
        {
            Response.Redirect("/Home/Index");
            return View("Index");
        }

        public async Task<ActionResult> SignInMicrosoft()
        {
            var respuesta = Request;
            ViewBag.Estado = 0;
            ViewBag.Mensaje = "";
            var accesToken = await GraphHelper.GetUserAccessTokenAsync();
            var userDetails = await GraphHelper.GetUserDetailsAsync(accesToken);

            //IConfidentialClientApplication app = MsalAppBuilder.BuildConfidentialClientApplication();
            ////AuthenticationResult userAccount = null;
            //var current = ClaimsPrincipal.Current;
            //var allgo = ClaimsPrincipal.Current.GetMsalAccountId();
            //var account = await app.GetAccountAsync(Session["AccountId"].ToString());
            //string[] scopes = { "Mail.Read" };
            //try
            //{
            //    if (account != null)
            //    {

            //        // try to get token silently
            //        //userAccount = await app.AcquireTokenSilent(scopes, account).ExecuteAsync().ConfigureAwait(false);
            //    }               

            //}
            //catch (MsalUiRequiredException)
            //{
            //    ViewBag.Estado = 100;
            //    ViewBag.Mensaje = (Request.QueryString["errormessage"] != null) ? Request.QueryString["errormessage"] : "Error de Conexión";
            //}
            //if (account != null)
            if (userDetails != null)
            {
                //var usuarioCuenta = account.Username;
                ViewBag.usuarioCuenta = userDetails.Email;
                dynamic showMessageString = string.Empty;
                dynamic nombreEmpresa = string.Empty;
                dynamic nombreUsuario = string.Empty;
                var arrayPerfil = new List<Helper.Perfil>();
                //var usuario = db.Usuario.Where(c => c.CorreoElectronico == account.Username).FirstOrDefault();
                var usuario = db.Usuario.Where(c => c.CorreoElectronico == userDetails.Email).FirstOrDefault();
                if (usuario != null)
                {
                    Seguridad seguridad = new Seguridad();
                    LoginController loginControl = new LoginController();
                    seguridad = loginControl.PerfilesUsuario(1, usuario.IdUsuario);
                    seguridad.Interno = true;
                    seguridad = loginControl.AgregarPermisos(seguridad);
                    Session["userID"] = seguridad.IdUsuario;
                    Session["userNA"] = seguridad.Nombre;
                    Session["empID"] = seguridad.EMPR_ID;
                    Session["user_EmpresaNA"] = seguridad.Nombre;
                    //seguridad.AccountId = Session["AccountId"].ToString();
                    if (seguridad.MenuUsuario == null)
                    {
                        //Session.Abandon();
                        ViewBag.Mensaje = "Perfil no cuenta con opciones asociadas";
                        ViewBag.Estado = 100;
                    }
                    else
                    {
                        if (seguridad.MenuUsuario.Count() == 0)
                        {
                            // Session.Abandon();
                            ViewBag.Mensaje = "Perfil no cuenta con opciones asociadas";
                            ViewBag.Estado = 100;
                        }
                        else
                        {
                            var sessiones = HttpContext.Session;
                            System.Web.HttpContext.Current.Session["Seguridad"] = seguridad;
                            return RedirectToAction("Inicio", "Home");
                        }
                    }
                }
                else
                {
                    // Session.Abandon();
                    ViewBag.Mensaje = "Usuario no Existe en el Sistema, favor comunicarse con el administrador";
                    ViewBag.Estado = 100;
                }
            }
            else
            {
                ViewBag.Estado = 100;
                ViewBag.Mensaje = (Request.QueryString["errormessage"] != null) ? Request.QueryString["errormessage"] : "Error de Conexión";
            }
            return View("Index");
        }

    }
}