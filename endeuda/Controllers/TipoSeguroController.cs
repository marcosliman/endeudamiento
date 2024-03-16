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
    public class TipoSeguroController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("TipoSeguro", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var TipoSeguro =
                    (from TSeguro in db.TipoSeguro
                     select new RetornoGenerico
                     {
                         Id = TSeguro.IdTipoSeguro,
                         Nombre = TSeguro.NombreTipoSeguro
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaSeguros = new SelectList(TipoSeguro.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaSeguros"] = listaSeguros;
                return View();
            }
        }
        public JsonResult TipoSeguro_Read(bool? interno, string seguro, bool? activo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "TipoSeguro" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }

            var lista = (from TSeguro in db.TipoSeguro
                         where
                         ((seguro != null) ? (TSeguro.NombreTipoSeguro.Contains(seguro)) : 0 == 0) 
                         select new TipoSeguroViewModel
                         {
                             IdTipoSeguro= TSeguro.IdTipoSeguro,
                             NombreTipoSeguro= TSeguro.NombreTipoSeguro,
                             Activo = TSeguro.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreTipoSeguro).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "TipoSeguro" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            TipoSeguro registro = new TipoSeguro();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.TipoSeguro.Find(id);
                }
            }
          
            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateTipoSeguro(TipoSeguro registro)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "TipoSeguro" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            TipoSeguro registroEdit = new TipoSeguro();
            if (registro.IdTipoSeguro > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Tipo Seguro Actualizado" };
                registroEdit = db.TipoSeguro.Find(registro.IdTipoSeguro);
                if (registroEdit != null)
                {
                    registroEdit.NombreTipoSeguro = registro.NombreTipoSeguro;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Tipo Seguro registrado correctamente" };
                db.TipoSeguro.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}