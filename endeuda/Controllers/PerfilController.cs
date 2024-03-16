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
    public class PerfilController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        // GET: Perfil
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Perfiles", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
            
        }
        public JsonResult Perfiles_Read(bool? interno)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Perfiles" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var lista = db.Perfil.ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Perfiles" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            Perfil registro = new Perfil();

            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Perfil.Find(id);

                }
            }
            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Perfil registro)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Perfiles" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            Perfil registroEdit = new Perfil();
            if (registro.IdPerfil > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Perfil Actualizado" };
                registroEdit = db.Perfil.Find(registro.IdPerfil);
                if (registroEdit != null)
                {
                    registroEdit.NombrePerfil = registro.NombrePerfil;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                registroEdit.Editable = true;
                registroEdit = registro;                
                showMessageString = new { Estado = 0, Mensaje = "Perfil Registrado" };
                db.Perfil.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Permisos()
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Permisos" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            ViewBag.Perfil = new SelectList(db.Perfil.Where(c => c.Activo == true).OrderBy(c => c.NombrePerfil), "IdPerfil", "NombrePerfil");
                        
            var perfiles = (from p in db.Perfil
                            where p.Activo == true
                            select new RetornoGenerico
                            {
                                Id = p.IdPerfil,
                                Nombre = p.NombrePerfil,
                                Seleccionado = false
                            }).OrderBy(c => c.Nombre).ToList();

            ViewData["listaPerfil"] = perfiles;
            ViewData["Menu"] = db.Menu.OrderBy(c => c.NombreMenu).Select(c => new { Id = c.IdMenu, Nombre = c.NombreMenu });
            return View();
        }
        public ActionResult Permisos_Read(int idPerfil)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Permisos" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var registros = (from v in db.Menu
                             join a in db.PermisoPerfil on v.IdMenu equals a.IdMenu into resp
                             from j in resp.DefaultIfEmpty()
                             where ((j == null) ? idPerfil : j.IdPerfil) == idPerfil
                             && v.IdEstadoVigencia==1
                             select new
                             {
                                 Acceder = (j != null) ? j.Acceder : false,
                                 Crear = (j != null) ? j.Crear : false,
                                 Editar = (j != null) ? j.Editar : false,
                                 Eliminar = (j != null) ? j.Eliminar : false,
                                 IdPerfil = idPerfil,
                                 IdMenu = v.IdMenu,
                                 NivelMenu = v.NivelMenu
                             }).Distinct().ToList();

            var lista = (from m in db.Menu
                         join mp in db.Menu on m.IdMenuPadre equals mp.IdMenu
                         where m.NivelMenu>1
                         && m.IdEstadoVigencia == 1
                         select new PermisoViewModel
                         {
                             NombreGrupoPerfil = mp.NombreMenu,
                             NombreMenu = m.NombreMenu,
                             IdMenu = m.IdMenu,
                             IdPerfil = idPerfil,
                             TieneCrear = m.TieneCrear,
                             TieneEditar = m.TieneEditar,
                             TieneEliminar = m.TieneEliminar,
                             TieneAcceder = m.TieneAcceder,
                             NivelMenu = m.NivelMenu
                         }
                       ).OrderBy(c => c.NombreMenu).ToList();


            for (int i = 0; i < lista.Count; i++)
            {
                var acceso = registros.Where(c => c.IdMenu == lista[i].IdMenu).FirstOrDefault();
                if (acceso != null)
                {
                    lista[i].Acceder = acceso.Acceder;
                    lista[i].Crear = acceso.Crear;
                    lista[i].Editar = acceso.Editar;
                    lista[i].Eliminar = acceso.Eliminar;
                    lista[i].Id = acceso.IdMenu;
                }
            }
            var result = lista.Where(c => c.NivelMenu != 1).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void asignarPermisos(int tipoPermiso, int idPerfil, string grupoCheck)
        {
            dynamic nuevoPermiso = string.Empty;

            if (grupoCheck != null)
            {
                foreach (string permisoMenu in grupoCheck.Split(",".ToCharArray()))
                {
                    var idMenu = Int32.Parse(permisoMenu);
                    var acceso = db.PermisoPerfil.Where(c => c.IdPerfil == idPerfil && c.IdMenu == idMenu).FirstOrDefault();
                    if (acceso != null)
                    {
                        switch (tipoPermiso)
                        {
                            case 1:
                                acceso.Acceder = true;
                                break;
                            case 2:
                                acceso.Crear = true;
                                break;
                            case 3:
                                acceso.Editar = true;
                                break;
                            case 4:
                                acceso.Eliminar = true;
                                break;
                        }
                        /*si no existe permisos al menu padre sea crea*/
                        var consultaMenuPadre = db.Menu.Where(c => c.IdMenu == idMenu).FirstOrDefault();
                        if (consultaMenuPadre != null)
                        {
                            var existeMenuPadre = db.PermisoPerfil.Where(c => c.IdMenu == consultaMenuPadre.IdMenuPadre && c.IdPerfil == idPerfil).FirstOrDefault();
                            if (existeMenuPadre == null)
                            {
                                var nuevoPermisopadre = new PermisoPerfil
                                {
                                    IdMenu = consultaMenuPadre.IdMenuPadre.Value,
                                    IdPerfil = idPerfil,
                                    Acceder = true,
                                    Crear = false,
                                    Editar = false,
                                    Eliminar = false,
                                    IdEstadoVigencia = 1
                                };
                                db.PermisoPerfil.Add(nuevoPermisopadre);
                            }
                        }

                    }
                    else
                    {
                        /*crea nuevo permiso dependiendo del tipo (acceder, crear, editar, eliminar)*/
                        switch (tipoPermiso)
                        {
                            case 1:
                                nuevoPermiso = new PermisoPerfil
                                {
                                    IdMenu = idMenu,
                                    IdPerfil = idPerfil,
                                    Acceder = true,
                                    Crear = false,
                                    Editar = false,
                                    Eliminar = false,
                                    IdEstadoVigencia = 1
                                };
                                break;
                            case 2:
                                nuevoPermiso = new PermisoPerfil
                                {
                                    IdMenu = idMenu,
                                    IdPerfil = idPerfil,
                                    Acceder = false,
                                    Crear = true,
                                    Editar = false,
                                    Eliminar = false,
                                    IdEstadoVigencia = 1
                                };
                                break;
                            case 3:
                                nuevoPermiso = new PermisoPerfil
                                {
                                    IdMenu = idMenu,
                                    IdPerfil = idPerfil,
                                    Acceder = false,
                                    Crear = false,
                                    Editar = true,
                                    Eliminar = false,
                                    IdEstadoVigencia = 1
                                };
                                break;
                            case 4:
                                nuevoPermiso = new PermisoPerfil
                                {
                                    IdMenu = idMenu,
                                    IdPerfil = idPerfil,
                                    Acceder = false,
                                    Crear = false,
                                    Editar = false,
                                    Eliminar = true,
                                    IdEstadoVigencia = 1
                                };
                                break;
                        }

                        /*si no existe permisos al menu padre sea crea*/
                        var consultaMenuPadre = db.Menu.Where(c => c.IdMenu == idMenu).FirstOrDefault();
                        if (consultaMenuPadre != null)
                        {
                            var existeMenuPadre = db.PermisoPerfil.Where(c => c.IdMenu == consultaMenuPadre.IdMenuPadre && c.IdPerfil == idPerfil).FirstOrDefault();
                            if (existeMenuPadre == null)
                            {
                                var nuevoPermisopadre = new PermisoPerfil
                                {
                                    IdMenu = consultaMenuPadre.IdMenuPadre.Value,
                                    IdPerfil = idPerfil,
                                    Acceder = true,
                                    Crear = false,
                                    Editar = false,
                                    Eliminar = false,
                                    IdEstadoVigencia = 1
                                };
                                db.PermisoPerfil.Add(nuevoPermisopadre);
                            }
                        }

                        db.PermisoPerfil.Add(nuevoPermiso);
                    }
                    db.SaveChanges();
                }
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Permisos_Update(int IdPerfil)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Permisos" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            /*se usa transacciones*/
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var idPerfil = Int32.Parse(HttpContext.Request.Params["IdPerfil"]);
                    /*al iniciar elimino todos los menus asociados al perfil*/
                    var menuGrilla = (from pp in db.PermisoPerfil
                                      join m in db.Menu on pp.IdMenu equals m.IdMenu
                                      where pp.IdPerfil == IdPerfil && m.NivelMenu != 1
                                      select new { m.IdMenu }).ToList();

                    int[] arrayMenuPadre = null;
                    foreach (var mg in menuGrilla)
                    {
                        /*obtengo los menus padres de los hijos que se estan eliminando*/

                        var menuPadre = (from m in db.Menu
                                         where m.IdMenu == mg.IdMenu
                                         select new
                                         {
                                             idMenuPadre = (m.IdMenuPadre != null) ? m.IdMenuPadre : 0
                                         }).FirstOrDefault();

                        arrayMenuPadre = new int[] { (int)menuPadre.idMenuPadre };

                        db.Database.ExecuteSqlCommand("DELETE FROM PermisoPerfil WHERE IdPerfil = {0} ", IdPerfil, mg.IdMenu);
                    }

                    if (arrayMenuPadre != null)
                    {

                        foreach (var idMenuPadre in arrayMenuPadre)
                        {
                            var tieneMenuHijos = (from p in db.PermisoPerfil
                                                  join m in db.Menu on p.IdMenu equals m.IdMenu
                                                  where p.IdPerfil == IdPerfil && m.IdMenuPadre == idMenuPadre && m.NivelMenu != 1
                                                  select p).FirstOrDefault();

                            if (tieneMenuHijos == null)
                            {
                                db.Database.ExecuteSqlCommand("DELETE FROM PermisoPerfil WHERE IdPerfil = {0} and IdMenu = {1}", IdPerfil, idMenuPadre);
                            }
                        }
                    }

                    var chkAcceder = HttpContext.Request.Params["chkAcceder"];
                    asignarPermisos(1, IdPerfil, chkAcceder);

                    var chkCrear = HttpContext.Request.Params["chkCrear"];
                    asignarPermisos(2, IdPerfil, chkCrear);

                    var chkEditar = HttpContext.Request.Params["chkEditar"];
                    asignarPermisos(3, IdPerfil, chkEditar);

                    var chkEliminar = HttpContext.Request.Params["chkEliminar"];
                    asignarPermisos(4, IdPerfil, chkEliminar);

                    dbContextTransaction.Commit();

                    showMessageString = new
                    {
                        Status = 0,
                        Message = "Permisos Perfil actualizados exitosamente"
                    };

                    return Json(showMessageString, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    showMessageString = new
                    {
                        Status = 500,
                        Message = "Error: " + ex.Message
                    };

                    dbContextTransaction.Rollback();

                    return Json(showMessageString, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}