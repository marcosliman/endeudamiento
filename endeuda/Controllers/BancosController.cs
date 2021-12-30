using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using tesoreria.Helper;
namespace tesoreria.Controllers
{
    public class BancosController : Controller
    {
        private ErpContext db = new ErpContext();
       tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Usuario
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Bancos", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public JsonResult Bancos_Read(bool? interno)
        {
            var lista = (from bco in db.Banco.ToList()
                         select new
                         {
                             bco.IdBanco,
                             bco.NombreBanco,
                             bco.Activo,
                             bco.FechaRegistro,
                             bco.UrlLogo,
                             bco.CodBanco
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create(int? id)
        {
            Banco registro = new Banco();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Banco.Find(id);

                }
            }
            return View(registro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Banco registro)
        {

            dynamic showMessageString = string.Empty;

            Banco registroEdit = new Banco();
            if (registro.IdBanco > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Banco Actualizado" };
                registroEdit = db.Banco.Find(registro.IdBanco);
                if (registroEdit != null)
                {
                    registroEdit.NombreBanco = registro.NombreBanco;
                    registroEdit.Activo = registro.Activo;
                    registroEdit.UrlLogo = registro.UrlLogo;
                    registroEdit.CodBanco = registro.CodBanco;
                }
            }
            else
            {
                registro.FechaRegistro = DateTime.Now;
                registroEdit = registro;
                showMessageString = new { Estado = 0, Mensaje = "Banco Registrado" };
                db.Banco.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}