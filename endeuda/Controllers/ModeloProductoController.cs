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
    public class ModeloProductoController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("ModeloProducto", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var ModeloProducto =
                    (from MProducto in db.ModeloProducto
                     select new RetornoGenerico
                     {
                         Id = MProducto.IdModeloProducto,
                         Nombre = MProducto.DescModeloProducto
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaMProductos = new SelectList(ModeloProducto.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaMProductos"] = listaMProductos;
                return View();
            }
        }
        public JsonResult ModeloProducto_Read(string marpro, bool? activo)
        {

            var lista = (from MProducto in db.ModeloProducto
                         where
                         ((marpro != null) ? (MProducto.DescModeloProducto.Contains(marpro)) : 0 == 0)
                         select new ModeloProductoViewModel
                         {
                             IdModeloProducto = MProducto.IdModeloProducto,
                             DescModeloProducto = MProducto.DescModeloProducto,
                             Activo = MProducto.Activo
                         }).AsEnumerable().OrderBy(c => c.DescModeloProducto).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            ModeloProducto registro = new ModeloProducto();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.ModeloProducto.Find(id);
                }
            }

            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateModeloProducto(ModeloProducto registro)
        {
            dynamic showMessageString = string.Empty;

            ModeloProducto registroEdit = new ModeloProducto();
            if (registro.IdModeloProducto > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Modelo Producto actualizado" };
                registroEdit = db.ModeloProducto.Find(registro.IdModeloProducto);
                if (registroEdit != null)
                {
                    registroEdit.DescModeloProducto = registro.DescModeloProducto;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Modelo Producto registrado correctamente" };
                db.ModeloProducto.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}
