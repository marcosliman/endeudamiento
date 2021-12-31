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
    public class CategoriaDocumentoController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("CategoriaDocumento", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var Categorias =
                    (from Catdoc in db.CategoriaDocumento
                     select new RetornoGenerico
                     {
                         Id = Catdoc.IdCategoriaDocumento,
                         Nombre = Catdoc.NombreCategoriaDocumento
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaCategorias = new SelectList(Categorias.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaCategorias"] = listaCategorias;
                return View();
            }
        }
        public JsonResult CategoriaDocumento_Read(bool? interno, string categoriadocumento, bool? activo)
        {

            var lista = (from CatDocto in db.CategoriaDocumento
                         where
                         ((categoriadocumento != "") ? (CatDocto.NombreCategoriaDocumento.Contains(categoriadocumento)) : 0 == 0) 
                         select new CatDoctoViewModel
                         {
                             IdCategoriaDocumento= CatDocto.IdCategoriaDocumento,
                             NombreCategoriaDocumento= CatDocto.NombreCategoriaDocumento,
                             Activo = CatDocto.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreCategoriaDocumento).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            CategoriaDocumento registro = new CategoriaDocumento();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.CategoriaDocumento.Find(id);
                }
            }
            var Categorias = (from t in db.CategoriaDocumento
                            select new RetornoGenerico { Id = t.IdCategoriaDocumento, Nombre = t.NombreCategoriaDocumento}).OrderBy(c => c.Nombre).ToList();
            SelectList listaCategoria = new SelectList(Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdCategoriaDocumento);
            ViewData["listaCategoria"] = listaCategoria;
            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateCategoriaDocumento(CategoriaDocumento registro)
        {
            dynamic showMessageString = string.Empty;

            CategoriaDocumento registroEdit = new CategoriaDocumento();
            if (registro.IdCategoriaDocumento > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Categoria Actualizada" };
                registroEdit = db.CategoriaDocumento.Find(registro.IdCategoriaDocumento);
                if (registroEdit != null)
                {
                    registroEdit.IdCategoriaDocumento = registro.IdCategoriaDocumento;
                    registroEdit.NombreCategoriaDocumento = registro.NombreCategoriaDocumento;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Categoria registrada correctamente" };
                db.CategoriaDocumento.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}