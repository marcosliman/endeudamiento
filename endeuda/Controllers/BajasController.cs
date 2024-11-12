using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.IO;
namespace tesoreria.Controllers
{
    public class BajasController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        #region Solicitud
 
        public ActionResult SolicitudBuscar()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("SolicitudBaja", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult AddSolicitud(int? id)
        {

            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("SolicitudBaja", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ModalAsociarActivo(int? id)
        {
            var licitacionActivo = new LicitacionActivo();
            licitacionActivo.IdLicitacion = 0;

            return PartialView(licitacionActivo);
        }

        public ActionResult ModalFormulario1(int? id)
        {
            var licitacionActivo = new LicitacionActivo();
            licitacionActivo.IdLicitacion = 1;

            return PartialView(licitacionActivo);
        }

        public ActionResult ModalFormulario2(int? id)
        {
            var licitacionActivo = new LicitacionActivo();
            licitacionActivo.IdLicitacion = 2;

            return PartialView(licitacionActivo);
        }

        public ActionResult ModalVerSolicitud(int? id)
        {
            var licitacionActivo = new LicitacionActivo();

            return PartialView(licitacionActivo);
        }
        #endregion

        public ActionResult Aprobacion()
        {
            return View();
        }

        public ActionResult SolicitudAprobar(int? id)
        {

            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("AprobacionBaja", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Autorizacion()
        {
            return View();
        }

        public ActionResult SolicitudAutorizar(int? id)
        {

            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("AutorizacionBaja", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Reporte()
        {
            return View();
        }

    }
}