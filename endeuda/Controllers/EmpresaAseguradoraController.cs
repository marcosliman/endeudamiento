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
    public class EmpresaAseguradoraController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("EmpresaAseguradora", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var EmpresaAseguradora =
                    (from EAseguradora in db.EmpresaAseguradora
                     select new RetornoGenerico
                     {
                         Id = EAseguradora.IdEmpresaAseguradora,
                         Nombre = EAseguradora.NombreEmpresaAseguradora
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaEAseguradoras = new SelectList(EmpresaAseguradora.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEAseguradoras"] = listaEAseguradoras;
                return View();
            }
        }
        public JsonResult EmpresaAseguradora_Read(string empase, bool? activo)
        {

            var lista = (from EAseguradora in db.EmpresaAseguradora
                         where
                         ((empase != null) ? (EAseguradora.NombreEmpresaAseguradora.Contains(empase)) : 0 == 0)
                         select new EmpresaAseguradoraViewModel
                         {
                             IdEmpresaAseguradora = EAseguradora.IdEmpresaAseguradora,
                             NombreEmpresaAseguradora = EAseguradora.NombreEmpresaAseguradora,
                             Activo = EAseguradora.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreEmpresaAseguradora).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            EmpresaAseguradora registro = new EmpresaAseguradora();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.EmpresaAseguradora.Find(id);
                }
            }

            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateEmpresaAseguradora(EmpresaAseguradora registro)
        {
            dynamic showMessageString = string.Empty;

            EmpresaAseguradora registroEdit = new EmpresaAseguradora();
            if (registro.IdEmpresaAseguradora > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Empresa Aseguradora Actualizada" };
                registroEdit = db.EmpresaAseguradora.Find(registro.IdEmpresaAseguradora);
                if (registroEdit != null)
                {
                    registroEdit.NombreEmpresaAseguradora = registro.NombreEmpresaAseguradora;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Empresa Aseguradora registrada correctamente" };
                db.EmpresaAseguradora.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}