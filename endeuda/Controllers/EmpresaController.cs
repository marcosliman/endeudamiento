using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
namespace tesoreria.Controllers
{
    public class EmpresaController : Controller
    {
        // GET: Empresa
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Empresa", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public JsonResult Empresas_Read(bool? interno)
        {
            //var lista = db.Empresa.ToList();
            var lista = (from emp in db.Empresa.ToList()
                         select new
                         {
                             emp.IdEmpresa,
                             emp.RazonSocial,
                             emp.AliasEmpresa,
                             emp.BaseSoftland,
                             emp.IdTributario,
                             emp.Activo
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);          

        }
        public ActionResult Create(int? id)
        {
            Empresa empresa = new Empresa();
            empresa.Activo = true;

            if (id != null)
            {
                if (id > 0)
                {
                    empresa = db.Empresa.Find(id);
                  
                }
            }           
            return View(empresa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Empresa empresa)
        {
            
            dynamic showMessageString = string.Empty;

            Empresa empresaEdit = new Empresa();
            if (empresa.IdEmpresa > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Emrpesa Actualizada" };
                empresaEdit = db.Empresa.Find(empresa.IdEmpresa);
                if (empresaEdit != null)
                {
                    empresaEdit.RazonSocial = empresa.RazonSocial;
                    empresaEdit.AliasEmpresa = empresa.AliasEmpresa;
                    empresaEdit.BaseSoftland = empresa.BaseSoftland;
                    empresaEdit.IdTributario = empresa.IdTributario;
                    empresaEdit.Activo = empresa.Activo;
                }
            }
            else
            {
                empresaEdit = empresa;
                showMessageString = new { Estado = 0, Mensaje = "Empresa Registrada" };
                db.Empresa.Add(empresa);
            }
            
            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        
    }
}
