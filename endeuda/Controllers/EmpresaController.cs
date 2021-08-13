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
            else if (seguridad != null && !seguridad.TienePermiso("Empresas", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            return View();
        }

        public JsonResult Empresas_Read(bool? interno)
        {     
            var lista = db.Empresa.ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);          

        }
        public ActionResult Create(int? id)
        {
            Empresa empresa = new Empresa();
           

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
                    empresaEdit.IdTributario = empresa.IdTributario;
                    empresaEdit.CtaActivo = empresa.CtaActivo;
                    empresaEdit.CtaVentas = empresa.CtaVentas;
                    empresaEdit.CtaGastos = empresa.CtaGastos;
                    empresaEdit.CtaCosto = empresa.CtaCosto;
                    empresaEdit.CtaDevolucion = empresa.CtaDevolucion;
                    empresaEdit.CodSubGrNuevo = empresa.CodSubGrNuevo;
                    empresaEdit.IdTributario = empresa.IdTributario;
                    empresaEdit.Activo = empresa.Activo;
                    empresaEdit.TieneBodega = empresa.TieneBodega;
                    empresaEdit.BaseSoftland = empresa.BaseSoftland;
                }
            }
            else
            {
                empresaEdit = empresa;
                empresaEdit.FechaRegistro = DateTime.Now;
                showMessageString = new { Estado = 0, Mensaje = "Empresa Registrada" };
                db.Empresa.Add(empresa);
            }
            
            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        
    }
}
