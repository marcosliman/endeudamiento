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
    public class TipoPerdidaController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        // GET: Usuario
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("TipoPerdida", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var TipoPerdida =
                    (from TPerdida in db.TipoPerdida
                     select new RetornoGenerico
                     {
                         Id = TPerdida.IdTipoPerdida,
                         Nombre = TPerdida.NombreTipoPerdida
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaTPerdida = new SelectList(TipoPerdida.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTPerdida"] = listaTPerdida;
                return View();
            }
        }
        public JsonResult TipoPerdida_Read(bool? interno, string perdida, bool? activo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "TipoDocumento" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }

            var lista = (from TPerdida in db.TipoPerdida
                         where
                         ((perdida != null) ? (TPerdida.NombreTipoPerdida.Contains(perdida)) : 0 == 0)
                         select new TipoPerdidaViewModel
                         {
                             IdTipoPerdida = TPerdida.IdTipoPerdida,
                             NombreTipoPerdida = TPerdida.NombreTipoPerdida,
                             Activo = TPerdida.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreTipoPerdida).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "TipoDocumento" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            TipoPerdida registro = new TipoPerdida();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.TipoPerdida.Find(id);
                }
            }

            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTipoPerdida(TipoPerdida registro)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "TipoDocumento" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            TipoPerdida registroEdit = new TipoPerdida();
            if (registro.IdTipoPerdida > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Tipo Perdida Actualizado" };
                registroEdit = db.TipoPerdida.Find(registro.IdTipoPerdida);
                if (registroEdit != null)
                {
                    registroEdit.NombreTipoPerdida = registro.NombreTipoPerdida;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Tipo Perdida registrado correctamente" };
                db.TipoPerdida.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}