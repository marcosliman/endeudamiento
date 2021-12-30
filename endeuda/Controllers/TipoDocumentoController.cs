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
    public class TipoDocumentoController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("TipoDocumento", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var Cliente =
                    (from Catdoc in db.CategoriaDocumento
                     select new RetornoGenerico
                     {
                         Id = Catdoc.IdCategoriaDocumento,
                         Nombre = Catdoc.NombreCategoriaDocumento
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaCategorias = new SelectList(Cliente.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaCategorias"] = listaCategorias;
                return View();
            }
        }
        public JsonResult TipoDocumento_Read(bool? interno,int? categoria, string documento, bool? activo)
        {

            var lista = (from Tdocto in db.TipoDocumento
                         join Catdocto in db.CategoriaDocumento on Tdocto.IdCategoriaDocumento equals Catdocto.IdCategoriaDocumento into t_Catdocto
                         from l_Catdocto in t_Catdocto.DefaultIfEmpty()
                         where
                         ((categoria != null) ? Tdocto.IdCategoriaDocumento == categoria : true)  &&
                         ((documento != "") ? (Tdocto.NombreTipoDocumento.Contains(documento)) : 0 == 0) 
                         select new TipoDoctoViewModel
                         {
                             IdTipoDocumento= Tdocto.IdTipoDocumento,
                             IdCategoriaDocumento= Tdocto.IdCategoriaDocumento,
                             NombreTipoDocumento=Tdocto.NombreTipoDocumento,
                             NombreCategoriaDocumento = (l_Catdocto != null) ? l_Catdocto.NombreCategoriaDocumento : "Sin Categoria Asociada",
                             Activo =Tdocto.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreTipoDocumento).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            TipoDocumento registro = new TipoDocumento();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.TipoDocumento.Find(id);
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
        public JsonResult CreateTipoDocumento(TipoDocumento registro)
        {
            dynamic showMessageString = string.Empty;

            TipoDocumento registroEdit = new TipoDocumento();
            if (registro.IdTipoDocumento > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Tipo de Documento Actualizado" };
                registroEdit = db.TipoDocumento.Find(registro.IdTipoDocumento);
                if (registroEdit != null)
                {
                    registroEdit.IdCategoriaDocumento = registro.IdCategoriaDocumento;
                    registroEdit.NombreTipoDocumento = registro.NombreTipoDocumento;
                    registroEdit.IdCategoriaDocumento = registro.IdCategoriaDocumento;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Tipo de Documento Registrado" };
                db.TipoDocumento.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

    }
}