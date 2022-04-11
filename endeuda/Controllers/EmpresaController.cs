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
            else { 
                id = 0;
            }

            var empresas = (from emp in db.Empresa
                            join eu in db.EmpresaRelacionada.Where(c => c.IdEmpresa == id) on emp.IdEmpresa equals eu.IdEmpresa into t_eu
                            from l_eu in t_eu.DefaultIfEmpty()
                            where emp.Activo == true
                            select new RetornoGenerico { Id = emp.IdEmpresa, Nombre = emp.RazonSocial, Seleccionado = (l_eu != null) ? true : false }).OrderBy(c => c.Nombre).ToList();

            ViewData["listaEmpresas"] = empresas;
           
            return View(empresa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Empresa empresa, int[] empresas)
        {
            
            dynamic showMessageString = string.Empty;

            Empresa empresaEdit = new Empresa();
            if (empresa.IdEmpresa > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Empresa Actualizada" };
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
            var uEmpresas = db.EmpresaRelacionada.Where(c => c.IdEmpresa == empresaEdit.IdEmpresa);
            db.EmpresaRelacionada.RemoveRange(uEmpresas);
            db.SaveChanges();
            if (empresas != null)
            {
                foreach (var id in empresas)
                {
                    EmpresaRelacionada newEmpresa = new EmpresaRelacionada();
                    newEmpresa.IdEmpresa = empresa.IdEmpresa;
                    newEmpresa.IdEmpresaRelacionada = id ;
                    db.EmpresaRelacionada.Add(newEmpresa);
                }
            }
            db.SaveChanges();
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        
    }
}
