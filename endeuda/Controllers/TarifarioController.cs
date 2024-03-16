using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using tesoreria.Helper;
namespace tesoreria.Controllers
{
    public class TarifarioController : Controller
    {
        private ErpContext db = new ErpContext();
        private InmobContext dbInm = new InmobContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        // GET: Usuario
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Tarifario", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public JsonResult Tarifario_Read(bool? interno)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Tarifario" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var lista = (from gtari in db.GrupoTarifario.ToList()
                         select new
                         {
                             gtari.IdGrupoTarifario,
                             gtari.GrupoFamilia,
                             gtari.DescripcionGrupoTarifario,
                             gtari.UF,
                             gtari.Activo
                             
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create(int? id)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Tarifario" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            GrupoTarifario registro = new GrupoTarifario();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.GrupoTarifario.Find(id);

                }
            }
            return View(registro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(GrupoTarifario registro)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Tarifario" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            GrupoTarifario registroEdit = new GrupoTarifario();
            if (registro.IdGrupoTarifario > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Grupo Tarifa actualizado" };
                registroEdit = db.GrupoTarifario.Find(registro.IdGrupoTarifario);
                if (registroEdit != null)
                {
                    registroEdit.GrupoFamilia = registro.GrupoFamilia;
                    registroEdit.DescripcionGrupoTarifario = registro.DescripcionGrupoTarifario;
                    registroEdit.UF= registro.UF;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                registroEdit = registro;
                showMessageString = new { Estado = 0, Mensaje = "Grupo Tarifa Registrado" };
                db.GrupoTarifario.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}