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
    public class MarcaProductoController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("MarcaProducto", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var MarcaProducto =
                    (from MProducto in db.MarcaProducto
                     select new RetornoGenerico
                     {
                         Id = MProducto.IdMarcaProducto,
                         Nombre = MProducto.DescMarcaProducto
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaMProductos = new SelectList(MarcaProducto.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaMProductos"] = listaMProductos;
                return View();
            }
        }
        public JsonResult MarcaProducto_Read(string marpro, bool? activo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "MarcaProducto" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var lista = (from MProducto in db.MarcaProducto
                         where
                         ((marpro != null) ? (MProducto.DescMarcaProducto.Contains(marpro)) : 0 == 0)
                         select new MarcaProductoViewModel
                         {
                             IdMarcaProducto = MProducto.IdMarcaProducto,
                             DescMarcaProducto = MProducto.DescMarcaProducto,
                             Activo = MProducto.Activo
                         }).AsEnumerable().OrderBy(c => c.DescMarcaProducto).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "MarcaProducto" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            MarcaProducto registro = new MarcaProducto();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.MarcaProducto.Find(id);
                }
            }

            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMarcaProducto(MarcaProducto registro)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "MarcaProducto" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            MarcaProducto registroEdit = new MarcaProducto();
            if (registro.IdMarcaProducto > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Marca Producto actualizada" };
                registroEdit = db.MarcaProducto.Find(registro.IdMarcaProducto);
                if (registroEdit != null)
                {
                    registroEdit.DescMarcaProducto = registro.DescMarcaProducto;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Marca Producto registrada correctamente" };
                db.MarcaProducto.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}
