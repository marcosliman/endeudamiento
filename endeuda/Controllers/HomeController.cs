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
using bodega.Helper;

namespace tesoreria.Controllers
{
    public class HomeController : Controller
    {
        private ErpContext db = new ErpContext();
        private readonly ILog _log = LogManager.GetLogger(typeof(HomeController));
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        string UrlSistemaBodega = System.Configuration.ConfigurationManager.AppSettings["UrlSistemaBodega"];
        public ActionResult Login()
        {            
            return View();
        }
        public ActionResult Index()
        {
            ViewBag.menuActivo = 0;          
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
                    
                    showMessageString = new { Estado = 0, Mensaje = "Acceso correcto", ToUrl = "/Home/Inicio" };
                    
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
                var empresas = (from t in db.Empresa
                                where t.Activo == true
                                select new RetornoGenerico { Id = t.IdEmpresa, Nombre = t.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresas.OrderBy(c => c.Nombre), "Id", "Nombre", 1);
                ViewData["listaEmpresa"] = listaEmpresa;
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

    
    }
}