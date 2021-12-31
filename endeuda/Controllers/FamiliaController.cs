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
    public class FamiliaController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("Familia", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var Familia =
                    (from Fam in db.Familia
                     select new RetornoGenerico
                     {
                         Id = Fam.IdFamilia,
                         Nombre = Fam.NombreFamilia
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaFamilias = new SelectList(Familia.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaFamilias"] = listaFamilias;
                return View();
            }
        }
        public JsonResult Familia_Read(bool? interno, string familia, bool? activo)
        {

            var lista = (from Fam in db.Familia
                         where
                         ((familia != "") ? (Fam.NombreFamilia.Contains(familia)) : 0 == 0) 
                         select new FamiliaViewModel
                         {
                             IdFamilia= Fam.IdFamilia,
                             NombreFamilia= Fam.NombreFamilia,
                             Activo = Fam.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreFamilia).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            Familia registro = new Familia();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Familia.Find(id);
                }
            }
          
            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateFamilia(Familia registro)
        {
            dynamic showMessageString = string.Empty;

            Familia registroEdit = new Familia();
            if (registro.IdFamilia > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Familia Actualizada" };
                registroEdit = db.Familia.Find(registro.IdFamilia);
                if (registroEdit != null)
                {
                    registroEdit.NombreFamilia = registro.NombreFamilia;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Familia registrada correctamente" };
                db.Familia.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}