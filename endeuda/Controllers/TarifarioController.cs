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