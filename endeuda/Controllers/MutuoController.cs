using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
namespace tesoreria.Controllers
{
    public class MutuoController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato
        public ActionResult MutuoInicio()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoInicio", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        #region Mutuo
        public ActionResult MutuoGestion()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var empresaF = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaF = new SelectList(empresaF.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresaF"] = listaEmpresaF;

                var empresaR = (from e in db.Empresa
                                where e.Activo == true
                                select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaR = new SelectList(empresaR.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresaR"] = listaEmpresaR;

                return View();
            }
        }

        public ActionResult ListaMutuo_Read(int? idEmpresaFinancia, int? idEmpresaReceptora)
        {
            var registro = (from m in db.Mutuo.ToList()
                            join em in db.Empresa.ToList() on m.IdEmpresaFinancia equals em.IdEmpresa
                            join emr in db.Empresa.ToList() on m.IdEmpresaReceptora equals emr.IdEmpresa
                            join es in db.Estado.ToList() on m.IdEstado equals es.IdEstado
                            where m.IdEmpresaFinancia == ((idEmpresaFinancia != null) ? idEmpresaFinancia : m.IdEmpresaFinancia)
                            && m.IdEmpresaReceptora == ((idEmpresaReceptora != null) ? idEmpresaReceptora : m.IdEmpresaReceptora)
                            select new MutuoViewModel
                            {
                                IdMutuo = m.IdMutuo,
                                EmpresaFinancia = em.RazonSocial,
                                EmpresaReceptora = emr.RazonSocial,
                                MontoPrestamo = m.MontoPrestamo,
                                TasaMensual = m.TasaMensual,
                                TasaDiaria = m.TasaDiaria,
                                FechaPrestamo = m.FechaPrestamo,
                                FechaPrestamoStr = m.FechaPrestamo.ToString("dd-MM-yyyy"),
                                NombreEstado = es.NombreEstado,
                                PuedeProcesar = (m.IdEstado != (int)Helper.Estado.MutuoVigente) ? false : true,
                                PuedeEliminar = (m.IdEstado != (int)Helper.Estado.MutuoCreado) ? false : true
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EmpresaRelacionada(int idEmpresa)
        {
            var registro = (from c in db.Empresa
                            join r in db.EmpresaRelacionada on c.IdEmpresa equals r.IdEmpresaRelacionada
                            where r.IdEmpresa == idEmpresa
                            select new RetornoGenerico
                            {
                                Id = r.IdEmpresaRelacionada,
                                Nombre = c.RazonSocial
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrestamoRelacionadas()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("PrestamoRelacionadas", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ModalAddMutuo(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                //var contrato = new ContratoViewModel();
                var mutuo = (from c in db.Mutuo
                                where c.IdMutuo == idMutuo
                                select new MutuoViewModel
                                {
                                    IdMutuo = c.IdMutuo,
                                    ExisteMutuo = "S"
                                }).FirstOrDefault();
                if (mutuo == null)
                {
                    mutuo = new MutuoViewModel();
                    mutuo.IdMutuo = idMutuo;
                    mutuo.ExisteMutuo = "N";
                }

                return View(mutuo);
            }
        }

        public ActionResult AddMutuo(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from c in db.Mutuo.ToList()
                                where c.IdMutuo == idMutuo
                                select new MutuoViewModel
                                {
                                    IdMutuo = c.IdMutuo,
                                    IdEmpresaFinancia = c.IdEmpresaFinancia,
                                    IdEmpresaReceptora = c.IdEmpresaReceptora,
                                    TasaMensual = c.TasaMensual,
                                    TasaDiaria = c.TasaDiaria,
                                    MontoPrestamo = c.MontoPrestamo,
                                    FechaPrestamoStr = c.FechaPrestamo.ToString("dd-MM-yyyy"),
                                    IdEstado = c.IdEstado,
                                    ExisteMutuo = "S",
                                    TituloBoton = "Actualizar Mutuo",
                                    PuedeEditar = (c.IdEstado != (int)Helper.Estado.MutuoCreado) ? false : true
                                }).FirstOrDefault();

                if (registro == null) {
                    registro = new MutuoViewModel();
                    registro.IdMutuo = 0;
                    registro.IdEmpresaFinancia = 0;
                    registro.IdEmpresaReceptora = 0;
                    registro.TituloBoton = "Grabar Mutuo";
                    registro.PuedeEditar = true;
                }

                var empresaF = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaF = new SelectList(empresaF.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresaFinancia);
                ViewData["listaEmpresaF"] = listaEmpresaF;

                var empresaR = (from e in db.Empresa
                                join r in db.EmpresaRelacionada on e.IdEmpresa equals r.IdEmpresaRelacionada
                                where r.IdEmpresa == registro.IdEmpresaFinancia
                                select new RetornoGenerico { Id = r.IdEmpresaRelacionada, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaR = new SelectList(empresaR.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresaReceptora);
                ViewData["listaEmpresaR"] = listaEmpresaR;

                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GrabarMutuo(Mutuo dato)
        {

            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var dbMutuo = db.Mutuo.Find(dato.IdMutuo);

                            if (dbMutuo == null)
                            {
                                //var existeMutuo = db.Mutuo.Where(c => c.IdEmpresaFinancia == dato.IdEmpresaFinancia && c.IdEmpresaReceptora == dato.IdEmpresaReceptora
                                //                                && c.IdEstado != (int)Helper.Estado.MutuoFinalizado).FirstOrDefault();

                                //if (existeMutuo == null)
                                //{
                                    var addMutuo = new Mutuo();
                                    addMutuo.IdEmpresaFinancia = dato.IdEmpresaFinancia;
                                    addMutuo.IdEmpresaReceptora = dato.IdEmpresaReceptora;
                                    addMutuo.MontoPrestamo = dato.MontoPrestamo;
                                    addMutuo.FechaPrestamo = dato.FechaPrestamo;
                                    addMutuo.TasaMensual = dato.TasaMensual;
                                    addMutuo.TasaDiaria = dato.TasaDiaria;
                                    addMutuo.IdEstado = (int)Helper.Estado.MutuoCreado;
                                    addMutuo.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    addMutuo.FechaRegistro = DateTime.Now;
                                    db.Mutuo.Add(addMutuo);
                                    db.SaveChanges();

                                    dbContextTransaction.Commit();
                                    showMessageString = new { Estado = 0, Mensaje = "Prestamo registrado exitosamente", idMutuo = addMutuo.IdMutuo };
                                //}
                                //else {
                                //    showMessageString = new { Estado = 105, Mensaje = "Ya existe un prestamo vigente para la empresa que financia y la receptora"};
                                //}
                            }
                            else {
                                var existePrestamo = db.MutuoPrestamo.Where(c => c.IdMutuo == dbMutuo.IdMutuo).ToList();
                                var existeAbono = db.MutuoAbono.Where(c => c.IdMutuo == dbMutuo.IdMutuo).ToList();

                                if (existePrestamo.Count() > 0 || existeAbono.Count() > 0)
                                {
                                    showMessageString = new { Estado = 102, Mensaje = "no puede editar datos del prestamo, ya tiene procesos relacionados" };
                                }
                                else {
                                    dbMutuo.IdEmpresaFinancia = dato.IdEmpresaFinancia;
                                    dbMutuo.IdEmpresaReceptora = dato.IdEmpresaReceptora;
                                    dbMutuo.MontoPrestamo = dato.MontoPrestamo;
                                    dbMutuo.FechaPrestamo = dato.FechaPrestamo;
                                    dbMutuo.TasaMensual = dato.TasaMensual;
                                    dbMutuo.TasaDiaria = dato.TasaDiaria;
                                    db.SaveChanges();

                                    dbContextTransaction.Commit();
                                    showMessageString = new { Estado = 0, Mensaje = "Prestamo actualizado exitosamente", idMutuo = dbMutuo.IdMutuo };
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMutuo(int idMutuo)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbMutuo = db.Mutuo.Where(c=> c.IdMutuo == idMutuo && c.IdEstado == (int)Helper.Estado.MutuoCreado).FirstOrDefault();

                    if (dbMutuo != null)
                    {

                        var dbArchivo = (from doc in db.MutuoDocumento
                                         where doc.IdMutuo == dbMutuo.IdMutuo
                                         select new
                                         {
                                             doc.IdMutuoDocumento,
                                             doc.UrlDocumento
                                         }).ToList();
                        foreach (var arc in dbArchivo)
                        {
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM MutuoDocumento WHERE IdMutuoDocumento = {0}", arc.IdMutuoDocumento);
                            db.SaveChanges();
                        }

                        db.Mutuo.Remove(dbMutuo);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        showMessageString = new { Estado = 0, Mensaje = "Registro Eliminado con exito" };
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Mutuo se encuentra vigente, no puede ser eliminado" };
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                }

                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ConfirmarPrestamo(int idMutuo)
        {
            dynamic showMessageString = string.Empty;
            var mutuo = db.Mutuo.Find(idMutuo);

            //var documentos = db.MutuoDocumento.Where(c => c.IdMutuo == idMutuo).Count();
            //if (documentos > 0)
            //{
                mutuo.IdEstado = (int)Helper.Estado.MutuoVigente;
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Mutuo ha sido habilitado Exitosamente" };
            //}
            //else
            //{
            //    showMessageString = new { Estado = 100, Mensaje = "Existen Datos Incompletos" };
            //}

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Documento Mutuo
        public ActionResult AddDocumento(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var tipoDocumento = (from e in db.TipoDocumento
                                     where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.Mutuo
                                     select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;


                var mutuo = db.Mutuo.Where(c => c.IdMutuo == idMutuo).FirstOrDefault();
                ViewData["puedeActivar"] = "N";
                if (mutuo != null) {
                    if (mutuo.IdEstado == (int)Helper.Estado.MutuoCreado) {
                        ViewData["puedeActivar"] = "S";
                    }
                }


                var registro = new MutuoDocumento();
                registro.IdMutuo = idMutuo;

                return View(registro);
            }
        }

        public ActionResult ListaMutuoDocumento_Read(int idMutuo)
        {
            var registro = (from c in db.MutuoDocumento
                            join td in db.TipoDocumento on c.IdTipoDocumento equals td.IdTipoDocumento
                            where c.IdMutuo == idMutuo
                            select new MutuoDocumentoViewModel
                            {
                                IdMutuoDocumento = c.IdMutuoDocumento,
                                IdMutuo = c.IdMutuo,
                                NombreTipoDocumento = td.NombreTipoDocumento,
                                NombreOriginal = c.NombreOriginal,
                                UrlDocumento = c.UrlDocumento
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarMutuoDocumento(MutuoDocumento dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            MutuoDocumento addDocumento = new MutuoDocumento();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null)
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";

                            if (archivo != null)
                            {
                                var mutuo = db.Mutuo.Where(c => c.IdMutuo == dato.IdMutuo).FirstOrDefault();
                                var pathDocumento = "";
                                var fileName = dato.IdTipoDocumento.ToString() + '_' + Path.GetFileName(archivo.FileName);
                                var carpeta = "Mutuo/";
                                var pathData = "~/App_Data";
                                var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta);
                                if (!Directory.Exists(pathCarpeta))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                }
                                string carpetaSolicitud = mutuo.IdMutuo.ToString();
                                var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                if (!Directory.Exists(pathCarpetaSolicitud))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                }
                                pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                archivo.SaveAs(physicalPath);

                                addDocumento.IdMutuo = (int)mutuo.IdMutuo;
                                addDocumento.IdTipoDocumento = dato.IdTipoDocumento;
                                addDocumento.FechaRegistro = DateTime.Now;
                                addDocumento.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addDocumento.NombreOriginal = fileName;
                                addDocumento.UrlDocumento = pathDocumento;
                                db.MutuoDocumento.Add(addDocumento);
                                db.SaveChanges();
                                mensaje = "Archivo Cargado con exito";
                            }

                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            //return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMutuoDocumento(int idMutuoDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.MutuoDocumento.Find(idMutuoDocumento);

            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            db.Database.ExecuteSqlCommand("DELETE FROM MutuoDocumento WHERE IdMutuoDocumento = {0}", idMutuoDocumento);
            db.SaveChanges();


            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Abono Mutuo
        public ActionResult ModalRegistrarAbono(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from m in db.Mutuo.ToList()
                                join em in db.Empresa.ToList() on m.IdEmpresaFinancia equals em.IdEmpresa
                                join emr in db.Empresa.ToList() on m.IdEmpresaReceptora equals emr.IdEmpresa
                                where m.IdMutuo == idMutuo
                                select new MutuoViewModel
                                {
                                    IdMutuo = m.IdMutuo,
                                    EmpresaFinancia = em.RazonSocial,
                                    EmpresaReceptora = emr.RazonSocial
                                }).FirstOrDefault();


                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GrabarAbonoMutuo(MutuoAbono dato)
        {

            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var dbMutuo = db.Mutuo.Find(dato.IdMutuo);

                            if (dbMutuo != null)
                            {
                                var addAbono = new MutuoAbono();
                                addAbono.IdMutuo = dato.IdMutuo;
                                addAbono.MontoAbono = dato.MontoAbono;
                                addAbono.FechaAbono = dato.FechaAbono;
                                addAbono.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addAbono.FechaRegistro = DateTime.Now;
                                db.MutuoAbono.Add(addAbono);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                showMessageString = new { Estado = 0, Mensaje = "Abono registrado exitosamente" };
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Prestamo Mutuo

        public ActionResult ModalNuevoPrestamo(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from m in db.Mutuo.ToList()
                                join em in db.Empresa.ToList() on m.IdEmpresaFinancia equals em.IdEmpresa
                                join emr in db.Empresa.ToList() on m.IdEmpresaReceptora equals emr.IdEmpresa
                                where m.IdMutuo == idMutuo
                                select new MutuoViewModel
                                {
                                    IdMutuo = m.IdMutuo,
                                    EmpresaFinancia = em.RazonSocial,
                                    EmpresaReceptora = emr.RazonSocial
                                }).FirstOrDefault();


                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GrabarNuevoPrestamo(MutuoPrestamo dato)
        {

            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("MutuoGestion", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var dbMutuo = db.Mutuo.Find(dato.IdMutuo);

                            if (dbMutuo != null)
                            {
                                var addPrestamo = new MutuoPrestamo();
                                addPrestamo.IdMutuo = dato.IdMutuo;
                                addPrestamo.MontoPrestamo = dato.MontoPrestamo;
                                addPrestamo.FechaPrestamo = dato.FechaPrestamo;
                                addPrestamo.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addPrestamo.FechaRegistro = DateTime.Now;
                                db.MutuoPrestamo.Add(addPrestamo);
                                db.SaveChanges();

                                dbContextTransaction.Commit();
                                showMessageString = new { Estado = 0, Mensaje = "Abono registrado exitosamente" };
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public ActionResult ModalDetalleDeuda(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var mutuo = new MutuoViewModel();
                mutuo.IdMutuo = idMutuo;
                mutuo.FechaPrestamo = DateTime.Now;
                mutuo.FechaPrestamoStr = mutuo.FechaPrestamo.ToString("dd-MM-yyyy");

                return PartialView(mutuo);
            }
        }
        public ActionResult ModalProyectarAbono(int idMutuo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var mutuo = new Mutuo();
                mutuo.IdMutuo = idMutuo;

                return PartialView(mutuo);
            }
        }

        public ActionResult ListProyectar_Read(int? idMutuo,DateTime fechaProyeccion)
        {
            var registro = (from m in db.Mutuo.ToList()
                            where m.IdMutuo == ((idMutuo != null) ? idMutuo : m.IdMutuo)
                            select new
                            {
                                IdMutuo = m.IdMutuo,
                                Monto = m.MontoPrestamo,
                                TasaMensual = m.TasaMensual,
                                TasaDiaria = m.TasaDiaria,
                                FechaPrestamo = m.FechaPrestamo,
                                FechaPrestamoStr = m.FechaPrestamo.ToString("dd-MM-yyyy")
                            }).FirstOrDefault();


            List<ProyeccionMutuoViewModel> arrayProyeccion = new List<ProyeccionMutuoViewModel>();
            if (registro != null)
            {
                var monthDiff = Math.Abs((fechaProyeccion.Year * 12 + (fechaProyeccion.Month - 1)) - (registro.FechaPrestamo.Year * 12 + (registro.FechaPrestamo.Month - 1)))+1;

                double montoInicial = registro.Monto;
                double tasaDiaria = registro.TasaDiaria;
                double interes = 0;
                double interesNuevo = 0;
                double montoTotal = 0;
                double interesTotal = 0;
                double montoAmortizacion = 0;
                double montoPrestamo = 0;
                DateTime? fechaAmortizacion;
                for (int i = 0; i < monthDiff; i++)
                {
                    ProyeccionMutuoViewModel proy = new ProyeccionMutuoViewModel();
                    DateTime fecha = registro.FechaPrestamo.AddMonths(i);

                    int days;
                    int daysN = 0;
                    DateTime oPrimerDiaDelMes;
                    DateTime oUltimoDiaDelMes;
                    interesNuevo = 0;
                    fechaAmortizacion = null;
                    montoAmortizacion = 0;
                    montoPrestamo = 0;
                    if (i == 0)
                    {
                        //DateTime oPrimerDiaDelMes = new DateTime(fecha.Year, fecha.Month, fecha.Day);
                        oPrimerDiaDelMes = new DateTime(fecha.Year, fecha.Month, 1);

                        //agregamos 1 mes al objeto anterior y restamos 1 día.
                        oUltimoDiaDelMes = oPrimerDiaDelMes.AddMonths(1).AddDays(-1);

                        oPrimerDiaDelMes = new DateTime(fecha.Year, fecha.Month, fecha.Day);
                        TimeSpan difFechas = oUltimoDiaDelMes - oPrimerDiaDelMes;
                        days = (int)difFechas.TotalDays;
                    }
                    else
                    {
                        //Asi obtenemos el primer dia del mes actual
                        oPrimerDiaDelMes = new DateTime(fecha.Year, fecha.Month, 1);

                        //agregamos 1 mes al objeto anterior y restamos 1 día.
                        oUltimoDiaDelMes = oPrimerDiaDelMes.AddMonths(1).AddDays(-1);

                        TimeSpan difFechas = oUltimoDiaDelMes - oPrimerDiaDelMes;
                        days = (int)difFechas.TotalDays + 1;

                        string dias = Convert.ToString(days);
                    }

                    var auxAbono = 0;
                    var amortizacion = db.MutuoAbono.Where(c => c.IdMutuo == registro.IdMutuo && (c.FechaAbono >= oPrimerDiaDelMes && c.FechaAbono <= oUltimoDiaDelMes)).ToList();
                    if (amortizacion.Count() > 0) {
                        foreach(var abono in amortizacion) {
                            montoInicial = montoTotal;
                            auxAbono++;
                            proy = new ProyeccionMutuoViewModel();
                            montoAmortizacion = abono.MontoAbono;
                            fechaAmortizacion = abono.FechaAbono;

                            TimeSpan difFechasN = oUltimoDiaDelMes - (DateTime)fechaAmortizacion;
                            daysN = (int)difFechasN.TotalDays + 1;

                            if (auxAbono == 1)
                            {
                                interes = Math.Round(montoInicial * (tasaDiaria * days) / 100);
                            }
                            else {
                                interes = 0;
                            }

                            interesNuevo = -(Math.Round(montoAmortizacion * (tasaDiaria * daysN) / 100));

                            montoPrestamo = 0;

                            interesTotal = interes + interesNuevo;

                            montoTotal = montoInicial + montoPrestamo + interesTotal - montoAmortizacion;

                            proy.item = i;
                            proy.FechaInicio = oPrimerDiaDelMes;
                            proy.FechaInicioStr = oPrimerDiaDelMes.ToString("dd-MM-yyyy");
                            proy.FechaTermino = oUltimoDiaDelMes;
                            proy.FechaTerminoStr = oUltimoDiaDelMes.ToString("dd-MM-yyyy");
                            proy.CantidadDias = days;
                            proy.Monto = montoInicial;
                            proy.Interes = interes;
                            proy.MontoTotal = montoTotal;
                            proy.InteresTotal = interesTotal;
                            proy.MontoAmortizacion = montoAmortizacion;
                            proy.MontoPrestamo = montoPrestamo;
                            proy.FechaNuevo = (fechaAmortizacion != null) ? (DateTime)fechaAmortizacion : new DateTime();
                            proy.FechaNuevoStr = (fechaAmortizacion != null) ? fechaAmortizacion.Value.ToString("dd-MM-yyyy") : string.Empty;
                            proy.CantidadDiasNuevo = daysN;
                            proy.InteresNuevo = interesNuevo;
                            arrayProyeccion.Add(proy);

                            montoInicial = montoTotal;

                        }

                    }

                    var auxCredito = 0;
                    var nuevoCredito = db.MutuoPrestamo.Where(c => c.IdMutuo == registro.IdMutuo && (c.FechaPrestamo >= oPrimerDiaDelMes && c.FechaPrestamo <= oUltimoDiaDelMes)).ToList();
                    if (nuevoCredito.Count() > 0)
                    {
                        montoInicial = montoTotal;
                        foreach (var cred in nuevoCredito) {
                            auxCredito++;
                            proy = new ProyeccionMutuoViewModel();
                            montoPrestamo = cred.MontoPrestamo;
                            fechaAmortizacion = cred.FechaPrestamo;

                            TimeSpan difFechasN = oUltimoDiaDelMes - (DateTime)fechaAmortizacion;
                            daysN = (int)difFechasN.TotalDays + 1;

                            if (auxCredito == 1 && auxAbono == 0)
                            {
                                interes = Math.Round(montoInicial * (tasaDiaria * days) / 100);
                            }
                            else
                            {
                                interes = 0;
                            }

                            interesNuevo = (Math.Round(montoPrestamo * (tasaDiaria * daysN) / 100));

                            montoAmortizacion = 0;

                            interesTotal = interes + interesNuevo;

                            montoTotal = montoInicial + montoPrestamo + interesTotal - montoAmortizacion;

                            proy.item = i;
                            proy.FechaInicio = oPrimerDiaDelMes;
                            proy.FechaInicioStr = oPrimerDiaDelMes.ToString("dd-MM-yyyy");
                            proy.FechaTermino = oUltimoDiaDelMes;
                            proy.FechaTerminoStr = oUltimoDiaDelMes.ToString("dd-MM-yyyy");
                            proy.CantidadDias = days;
                            proy.Monto = montoInicial;
                            proy.Interes = interes;
                            proy.MontoTotal = montoTotal;
                            proy.InteresTotal = interesTotal;
                            proy.MontoAmortizacion = montoAmortizacion;
                            proy.MontoPrestamo = montoPrestamo;
                            proy.FechaNuevo = (fechaAmortizacion != null) ? (DateTime)fechaAmortizacion : new DateTime();
                            proy.FechaNuevoStr = (fechaAmortizacion != null) ? fechaAmortizacion.Value.ToString("dd-MM-yyyy") : string.Empty;
                            proy.CantidadDiasNuevo = daysN;
                            proy.InteresNuevo = interesNuevo;
                            arrayProyeccion.Add(proy);

                            montoInicial = montoTotal;

                        }

                    }

                    if (auxAbono > 0 || auxCredito > 0)
                    {
                        montoInicial = montoTotal;
                    }

                    var existeDato = arrayProyeccion.Where(c => c.FechaInicio == oPrimerDiaDelMes && c.FechaTermino == oUltimoDiaDelMes).Count();
                    if(existeDato == 0) {
                        interes = Math.Round(montoInicial * (tasaDiaria * days) / 100);
                        interesTotal = interes + interesNuevo;
                        montoTotal = montoInicial + montoPrestamo + interesTotal - montoAmortizacion;
                        proy.item = i;
                        proy.FechaInicio = oPrimerDiaDelMes;
                        proy.FechaInicioStr = oPrimerDiaDelMes.ToString("dd-MM-yyyy");
                        proy.FechaTermino = oUltimoDiaDelMes;
                        proy.FechaTerminoStr = oUltimoDiaDelMes.ToString("dd-MM-yyyy");
                        proy.CantidadDias = days;
                        proy.Monto = montoInicial;
                        proy.Interes = interes;
                        proy.MontoTotal = montoTotal;
                        proy.InteresTotal = interesTotal;
                        proy.MontoAmortizacion = montoAmortizacion;
                        proy.MontoPrestamo = montoPrestamo;
                        proy.FechaNuevo = (fechaAmortizacion != null) ? (DateTime)fechaAmortizacion : new DateTime();
                        proy.FechaNuevoStr = (fechaAmortizacion != null) ? fechaAmortizacion.Value.ToString("dd-MM-yyyy") : string.Empty;
                        proy.CantidadDiasNuevo = daysN;
                        proy.InteresNuevo = interesNuevo;
                        arrayProyeccion.Add(proy);
                        montoInicial = montoTotal;
                    }

                }
            }

            return Json(arrayProyeccion, JsonRequestBehavior.AllowGet);
        }
    }
}