using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.IO;
namespace tesoreria.Controllers
{
    public class LicitacionController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        // GET: Contrato
        #region Licitacion
        public ActionResult RegistrarLicitacion()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarLicitacion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresa"] = listaEmpresa;

                var tipoFinancimiento = (from e in db.TipoFinanciamiento
                                         where e.Activo == true
                                         select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoFinancimiento = new SelectList(tipoFinancimiento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoFinancimiento"] = listaTipoFinancimiento;

                return View();
            }
        }

        public ActionResult ListaLicitacion_Read(int? idEmpresa, int? idTipoFinanciamiento)
        {
            var registro = (from l in db.Licitacion
                            join e in db.Estado on l.IdEstado equals e.IdEstado
                            where l.IdEmpresa == ((idEmpresa != null) ? idEmpresa : l.IdEmpresa)
                            && l.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : l.IdTipoFinanciamiento)
                            && l.IdEstado == (int)Helper.Estado.LicCreada
                            select new LicitacionViewModel
                            {
                                IdLicitacion = l.IdLicitacion,
                                Autogenerado = l.Autogenerado,
                                RazonSocial = l.Empresa.RazonSocial,
                                NombreTipoFinanciamiento = l.TipoFinanciamiento.NombreTipoFinanciamiento,
                                Monto = l.Monto,
                                NombreEstado = e.NombreEstado
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddLicitacion(int idLicitacion)
        {

            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarLicitacion", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from l in db.Licitacion where l.IdLicitacion == idLicitacion
                                  select new LicitacionViewModel {
                                      IdLicitacion = l.IdLicitacion,
                                      IdEmpresa = l.IdEmpresa,
                                      IdTipoContrato = l.TipoFinanciamiento.IdTipoContrato,
                                      IdTipoFinanciamiento = l.IdTipoFinanciamiento,
                                      Autogenerado = l.Autogenerado,
                                      Monto = l.Monto,
                                      ExisteLicitacion = "S"
                                  }
                                  ).FirstOrDefault();

                if (registro == null) {
                    registro = new LicitacionViewModel();
                    registro.IdEmpresa = 0;
                    registro.IdTipoContrato = 0;
                    registro.IdTipoFinanciamiento = 0;
                    registro.ExisteLicitacion = "N";
                }

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;
                
                var tipoFinancimiento = (from e in db.TipoFinanciamiento
                               where e.Activo == true
                               select new SelectListItem() { Value = e.IdTipoFinanciamiento.ToString(), Text = e.NombreTipoFinanciamiento,
                                   Group = new SelectListGroup(){ Name=e.TipoContrato.NombreTipoContrato,Disabled=false } }).OrderBy(c => c.Group.Name).ToList();
                SelectList listaTipoFinancimiento = new SelectList(tipoFinancimiento, "Value", "Text","Group.Name",0);                
                
                ViewData["listaTipoFinancimiento"] = listaTipoFinancimiento;

                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarLicitacion(Licitacion dato)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarLicitacion", Helper.TipoAcceso.Crear))
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
                            var idLicitacion = 0;

                            /*cambiar estado de reserva*/
                            var licitacion = db.Licitacion.Find(dato.IdLicitacion);

                            if (licitacion != null)
                            {
                                mensaje = "Licitación actualizada con éxito";
                                idLicitacion = dato.IdLicitacion;
                                licitacion.IdEmpresa = dato.IdEmpresa;
                                //licitacion.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                                licitacion.Monto = dato.Monto;
                                db.SaveChanges();
                            }
                            else {
                                mensaje = "Licitación creada con éxito";
                                var maxLicitacion = 0;
                                var existeLicitacion = db.Licitacion.Count();
                                if (existeLicitacion > 0) { 
                                    maxLicitacion = (from t in db.Licitacion
                                                         where t.IdLicitacion == t.IdLicitacion
                                                    select new { t.Correlativo }).Max(c => c.Correlativo);
                                }

                                maxLicitacion = maxLicitacion + 1;

                                var autogenerado = maxLicitacion.ToString("D7");

                                var addLicitacion = new Licitacion();
                                addLicitacion.Correlativo = maxLicitacion;
                                addLicitacion.Autogenerado = autogenerado;
                                addLicitacion.IdEmpresa = dato.IdEmpresa;
                                addLicitacion.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                                addLicitacion.Monto = dato.Monto;
                                addLicitacion.IdEstado = (int)Helper.Estado.LicCreada;
                                addLicitacion.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addLicitacion.FechaRegistro = DateTime.Now;
                                db.Licitacion.Add(addLicitacion);
                                db.SaveChanges();

                                idLicitacion = addLicitacion.IdLicitacion;
                            }

                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = mensaje, idLicitacion = idLicitacion };
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
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteLicitacion(int idLicitacion)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbLicitacion = db.Licitacion.Find(idLicitacion);

                    if (dbLicitacion != null)
                    {

                        var existeContrato = (from c in db.Contrato
                                              join o in db.LicitacionOferta on c.IdLicitacionOferta equals o.IdLicitacionOferta
                                              where o.IdLicitacion == dbLicitacion.IdLicitacion
                                              select c).FirstOrDefault();

                        if (existeContrato == null)
                        {
                            var dbArchivo = (from doc in db.LicitacionOfertaDocumento
                                             join of in db.LicitacionOferta on doc.IdLicitacionOferta equals of.IdLicitacionOferta
                                             where of.IdLicitacion == dbLicitacion.IdLicitacion
                                             select new
                                             {
                                                 doc.IdLicitacionOfertaDocumento,
                                                 of.IdLicitacion,
                                                 of.IdLicitacionOferta,
                                                 doc.UrlDocumento
                                             }).ToList();
                            foreach (var arc in dbArchivo)
                            {
                                var archivo = Server.MapPath(arc.UrlDocumento);
                                if (System.IO.File.Exists(archivo))
                                {
                                    System.IO.File.Delete(archivo);
                                }

                                db.Database.ExecuteSqlCommand("DELETE FROM LicitacionOfertaDocumento WHERE IdLicitacionOfertaDocumento = {0}", arc.IdLicitacionOfertaDocumento);
                                db.SaveChanges();
                            }

                            var dbActivo = db.LicitacionActivo.Where(c => c.IdLicitacion == idLicitacion);
                            foreach (var act in dbActivo)
                            {
                                db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", Helper.Estado.ActDisponible, act.IdActivo);
                                db.SaveChanges();
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM LicitacionActivo WHERE IdLicitacion = {0}", dbLicitacion.IdLicitacion);
                            db.SaveChanges();

                            db.Database.ExecuteSqlCommand("DELETE FROM LicitacionOferta WHERE IdLicitacion = {0}", dbLicitacion.IdLicitacion);
                            db.SaveChanges();

                            db.Licitacion.Remove(dbLicitacion);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = "Registro Eliminado con exito" };
                        }
                        else {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "No se puede eliminar licitación, tiene datos relacionados" };
                        }
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Se ha producido un error, intentelo nuevamente" };
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

        /*finaliza licitacion*/
        [HttpPost]
        public JsonResult FinalizarLicitacion(int idLicitacion) 
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbLicitacion = db.Licitacion.Find(idLicitacion);

                    if (dbLicitacion != null)
                    {
                        var verificaActivo = "S";
                        if (dbLicitacion.TipoFinanciamiento.IdTipoContrato == (int)Helper.TipoContrato.Contrato && dbLicitacion.IdTipoFinanciamiento != (int)Helper.TipoFinanciamiento.EstructuradoConGarantia) {
                            verificaActivo = "N";
                        }
                        var existeActivo = db.LicitacionActivo.Where(c => c.IdLicitacion == idLicitacion).FirstOrDefault();
                        var existeOferta = db.LicitacionOferta.Where(c => c.IdLicitacion == idLicitacion).FirstOrDefault();

                        if ((existeActivo != null || verificaActivo == "N") && existeOferta != null)
                        {
                            dbLicitacion.IdEstado = (int)Helper.Estado.LicFinalizada;
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = "Licitación finalizada exitosamente" };
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "No puede finalizar licitacion, tiene datos faltantes" };
                        }
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

        public ActionResult LicitacionBuscar()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("LicitacionBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var banco = (from e in db.Banco
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaBanco"] = listaBanco;

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresa"] = listaEmpresa;

                var tipoFinancimiento = (from e in db.TipoFinanciamiento
                                         where e.Activo == true
                                         select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoFinancimiento = new SelectList(tipoFinancimiento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoFinancimiento"] = listaTipoFinancimiento;

                return View();
            }
        }

        public ActionResult BuscarLicitacion_Read(int? idBanco, int? idEmpresa, int? idTipoFinanciamiento)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "LicitacionBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var registro = (from l in db.Licitacion
                            join e in db.Estado on l.IdEstado equals e.IdEstado
                            //join lo in db.LicitacionOferta on l.IdLicitacion equals lo.IdLicitacion into low
                            //from lov in low.DefaultIfEmpty()
                            //join c in db.Contrato on lov.IdLicitacionOferta equals c.IdLicitacionOferta into cw
                            //from cv in cw.DefaultIfEmpty()
                            where l.IdEmpresa == ((idEmpresa != null) ? idEmpresa : l.IdEmpresa)
                            && l.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : l.IdTipoFinanciamiento)
                            && l.IdEstado != (int)Helper.Estado.LicCreada
                            select new LicitacionViewModel
                            {
                                IdLicitacion = l.IdLicitacion,
                                Autogenerado = l.Autogenerado,
                                RazonSocial = l.Empresa.RazonSocial,
                                NombreTipoFinanciamiento = l.TipoFinanciamiento.NombreTipoFinanciamiento,
                                Monto = l.Monto,
                                NombreEstado = e.NombreEstado
                                //NumeroContrato = (cv != null) ? cv.NumeroContrato : string.Empty
                            }).AsEnumerable().ToList();

            foreach (var reg in registro) {
                var contrato = (from lo in db.LicitacionOferta
                                join c in db.Contrato on lo.IdLicitacionOferta equals c.IdLicitacionOferta
                                where lo.IdLicitacion == reg.IdLicitacion
                                select new { c.NumeroContrato }).FirstOrDefault();
                if (contrato != null) {
                    reg.NumeroContrato = contrato.NumeroContrato;
                }
            }

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalVerLicitacion(int idLicitacion)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "LicitacionBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var registro = (from l in db.Licitacion
                                where l.IdLicitacion == idLicitacion
                                select new LicitacionViewModel
                                {
                                    IdLicitacion = l.IdLicitacion,
                                    IdEmpresa = l.IdEmpresa,
                                    RazonSocial = l.Empresa.RazonSocial,
                                    IdTipoFinanciamiento = l.IdTipoFinanciamiento,
                                    NombreTipoFinanciamiento = l.TipoFinanciamiento.NombreTipoFinanciamiento,
                                    Autogenerado = l.Autogenerado,
                                    Monto = l.Monto,
                                    ExisteLicitacion = "S"
                                }
                  ).FirstOrDefault();
                return View(registro);
            }
        }
        #endregion

        #region Activo

        public ActionResult ListaActivoLicitacion_Read(int idLicitacion)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion", "LicitacionBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var idEmpresa = 0;
            var licitacion = db.Licitacion.Find(idLicitacion);
            if (licitacion != null)
            {
                idEmpresa = licitacion.IdEmpresa;
            }

            var registro = (from ac in db.Activo
                            join rel in db.LicitacionActivo on ac.IdActivo equals rel.IdActivo
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            //join pr in db.Proveedor on ac.IdProveedor equals pr.IdProveedor into prw
                            //from prv in prw.DefaultIfEmpty()
                            where rel.IdLicitacion == idLicitacion
                            select new ActivoViewModel
                            {
                                IdLicitacionActivo = rel.IdLicitacionActivo,
                                IdActivo = ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                NombreCuenta = "",
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                Motor = ac.Motor,
                                Chasis = ac.Chasis,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NombreProveedor = "",//(prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModalAsociarActivo(int idLicitacion)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var licitacionActivo = new LicitacionActivo();
                licitacionActivo.IdLicitacion = idLicitacion;

                return PartialView(licitacionActivo);
            }
        }

        public ActionResult ListaActivoAsociar_Read(int idLicitacion, string numeroActivo, string codigoActivo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }

            var idEmpresa = 0;
            var licitacion = db.Licitacion.Find(idLicitacion);
            if (licitacion != null) {
                idEmpresa = licitacion.IdEmpresa;
            }

            //var activoLicitacion = new LicitacionActivo();
            var activoLicitacion = db.LicitacionActivo.AsEnumerable().ToList();
            if (activoLicitacion.Count() == 0) {
                activoLicitacion = new List<LicitacionActivo>();
            }
            var activoContrato = db.ContratoActivo.AsEnumerable().ToList();
            if (activoContrato.Count() == 0)
            {
                activoContrato = new List<ContratoActivo>();
            }

            var registro = (from ac in db.Activo.ToList()
                            join em in db.Empresa.ToList() on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia.ToList() on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            //join pr in db.Proveedor on ac.IdProveedor equals pr.IdProveedor into prw
                            //from prv in prw.DefaultIfEmpty()
                            where ac.NumeroInterno == ((numeroActivo != "") ? numeroActivo : ac.NumeroInterno)
                                && ac.CodSoftland == ((codigoActivo != "") ? codigoActivo : ac.CodSoftland)
                                && ac.IdEmpresa == idEmpresa
                                && ac.IdEstado == (int)Helper.Estado.ActDisponible
                                && activoLicitacion.Where(x=>x.IdActivo ==ac.IdActivo).Count()==0
                                && activoContrato.Where(x => x.IdActivo == ac.IdActivo).Count() == 0
                            select new ActivoViewModel
                            {
                                IdActivo = ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                NombreCuenta = "",
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                Motor = ac.Motor,
                                Chasis = ac.Chasis,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NombreProveedor = "",//(prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult AsociarActivo(int idLicitacion, int[] activos)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarLicitacion", Helper.TipoAcceso.Crear))
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

                            foreach (int ac in activos) {
                                var existeActivo = db.Activo.Where(c => c.IdActivo == ac && c.IdEstado == (int)Helper.Estado.ActDisponible).FirstOrDefault();
                                if (existeActivo != null) {
                                    var addActivo = new LicitacionActivo();
                                    addActivo.IdLicitacion = idLicitacion;
                                    addActivo.IdActivo = ac;
                                    db.LicitacionActivo.Add(addActivo);
                                    db.SaveChanges();

                                    /*cambio el estado del activo*/
                                    /*existeActivo.IdEstado = (int)Helper.Estado.ActLicitacion;
                                    db.SaveChanges();*/
                                }
                            }

                            var mensaje = "";
                            mensaje = "Asociación realizada con éxito";

                            //CambiaEstadoLicitacion(idLicitacion);

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
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAsociacionActivo(int idLicitacionActivo)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbLicitacionActivo = db.LicitacionActivo.Find(idLicitacionActivo);

                    if (dbLicitacionActivo != null)
                    {

                        db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", (int)Helper.Estado.ActDisponible,dbLicitacionActivo.IdActivo);
                        db.SaveChanges();

                        var idLicitacion = dbLicitacionActivo.IdLicitacion;
                        db.LicitacionActivo.Remove(dbLicitacionActivo);
                        db.SaveChanges();

                        //CambiaEstadoLicitacion(idLicitacion);
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                        showMessageString = new { Estado = 0, Mensaje = "Asociaciín eliminada con exito" };
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Se ha producido un error, intentelo nuevamente" };
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

        #endregion

        #region Oferta

        public ActionResult ListaLicitacionOferta_Read(int? idLicitacion)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion", "LicitacionBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var registro = (from l in db.LicitacionOferta
                            join e in db.Estado on l.IdEstado equals e.IdEstado
                            where l.IdLicitacion == ((idLicitacion != null) ? idLicitacion : l.IdLicitacion)
                            select new LicitacionOfertaViewModel
                            {
                                IdLicitacionOferta = l.IdLicitacionOferta,
                                IdLicitacion = l.IdLicitacion,
                                TasaMensual = l.TasaMensual,
                                TasaAnual = l.TasaAnual,
                                NombreBanco = l.Banco.NombreBanco,
                                Plazo = l.Plazo,
                                NombreEstado = e.NombreEstado
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ModalRegistrarOferta(int idLicitacionOferta, int idLicitacion)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var registro = (from l in db.LicitacionOferta
                                where l.IdLicitacionOferta == idLicitacionOferta
                                select new LicitacionOfertaViewModel
                                {

                                    IdLicitacionOferta = l.IdLicitacionOferta,
                                    IdLicitacion = l.IdLicitacion,
                                    TasaMensual = l.TasaMensual,
                                    TasaAnual = l.TasaAnual,
                                    IdBanco = l.IdBanco,
                                    Plazo = l.Plazo,
                                    IdEstado = l.IdEstado,
                                    TituloBoton = "Actualizar Oferta"
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new LicitacionOfertaViewModel();
                    registro.IdLicitacion = idLicitacion;
                    registro.IdBanco = 0;
                    registro.TasaAnual = null;
                    registro.TasaMensual = null;
                    registro.Plazo = null;
                    registro.TituloBoton = "Grabar Oferta";
                }

                var banco = (from e in db.Banco
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdBanco);
                ViewData["listaBanco"] = listaBanco;

                var doc = db.LicitacionOfertaDocumento.Where(c => c.IdLicitacionOferta == idLicitacionOferta).ToList();
                registro.Archivos = doc;

                return PartialView(registro);
            }
        }


        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarOferta(LicitacionOferta dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            LicitacionOfertaDocumento archivoOferta = new LicitacionOfertaDocumento();
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
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
                            var idOferta = 0;


                            var existeOferta = db.LicitacionOferta.Where(c => c.IdLicitacion == dato.IdLicitacion && c.IdBanco == dato.IdBanco && c.IdLicitacionOferta != dato.IdLicitacionOferta).FirstOrDefault();

                            if (existeOferta == null)
                            {
                                var oferta = db.LicitacionOferta.Find(dato.IdLicitacionOferta);

                                if (oferta != null)
                                {
                                    mensaje = "Oferta Licitación actualizada con éxito";
                                    oferta.IdBanco = dato.IdBanco;
                                    oferta.TasaMensual = dato.TasaMensual;
                                    oferta.TasaAnual = dato.TasaAnual;
                                    oferta.Plazo = dato.Plazo;
                                    db.SaveChanges();

                                    idOferta = dato.IdLicitacionOferta;
                                }
                                else
                                {
                                    mensaje = "Oferta Licitación creada con éxito";

                                    var addOferta = new LicitacionOferta();
                                    addOferta.IdLicitacion = dato.IdLicitacion;
                                    addOferta.IdBanco = dato.IdBanco;
                                    addOferta.TasaMensual = dato.TasaMensual;
                                    addOferta.TasaAnual = dato.TasaAnual;
                                    addOferta.Plazo = dato.Plazo;
                                    addOferta.IdEstado = (int)Helper.Estado.OfeGenerada;
                                    addOferta.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    addOferta.FechaRegistro = DateTime.Now;
                                    db.LicitacionOferta.Add(addOferta);
                                    db.SaveChanges();

                                    idOferta = addOferta.IdLicitacionOferta;

                                }

                                if (archivo != null)
                                {

                                    var pathDocumento = "";
                                    var fileName = idOferta.ToString() + '_' + Path.GetFileName(archivo.FileName);
                                    var carpeta = "Licitacion";
                                    var pathData = "~/App_Data";
                                    var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta); ;
                                    if (!Directory.Exists(pathCarpeta))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                    }
                                    string carpetaSolicitud = dato.IdLicitacion.ToString();
                                    var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                    if (!Directory.Exists(pathCarpetaSolicitud))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                    }
                                    pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                    var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                    archivo.SaveAs(physicalPath);

                                    archivoOferta.IdLicitacionOferta = (int)idOferta;
                                    archivoOferta.FechaRegistro = DateTime.Now;
                                    archivoOferta.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    archivoOferta.NombreOriginal = fileName;
                                    archivoOferta.UrlDocumento = pathDocumento;
                                    db.LicitacionOfertaDocumento.Add(archivoOferta);
                                    db.SaveChanges();
                                    mensaje = "Archivo Cargado con exito";


                                }

                                //CambiaEstadoLicitacion(dato.IdLicitacion);
                                dbContextTransaction.Commit();
                                showMessageString = new { Estado = 0, Mensaje = mensaje };
                            }
                            else {
                                showMessageString = new { Estado = 105, Mensaje = "Oferta ya existe para la licitación, revise su información" };

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
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteLicitacionOferta(int idLicitacionOferta)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarLicitacion" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }

            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbLicitacionOferta = db.LicitacionOferta.Find(idLicitacionOferta);

                    if (dbLicitacionOferta != null)
                    {
                        var dbArchivo = db.LicitacionOfertaDocumento.Where(c=> c.IdLicitacionOferta == idLicitacionOferta).ToList();
                        foreach (var arc in dbArchivo) { 
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }
                        }


                        db.Database.ExecuteSqlCommand("DELETE FROM LicitacionOfertaDocumento WHERE IdLicitacionOferta = {0}", dbLicitacionOferta.IdLicitacionOferta);
                        db.SaveChanges();

                        var idLicitacion = dbLicitacionOferta.IdLicitacion;

                        db.LicitacionOferta.Remove(dbLicitacionOferta);
                        db.SaveChanges();

                        //CambiaEstadoLicitacion(idLicitacion);
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                        showMessageString = new { Estado = 0, Mensaje = "Registro Eliminado con exito" };
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Se ha producido un error, intentelo nuevamente" };
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

        public ActionResult LicitacionOfertaDocumentos_Read(int? idLicitacionOferta)
        {

            var registro = db.LicitacionOfertaDocumento.Where(c => c.IdLicitacionOferta == idLicitacionOferta).ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult DeleteArchivoOferta(int idLicitacionOfertaDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.LicitacionOfertaDocumento.Find(idLicitacionOfertaDocumento);
            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            db.Database.ExecuteSqlCommand("DELETE FROM LicitacionOfertaDocumento WHERE IdLicitacionOfertaDocumento = {0}", idLicitacionOfertaDocumento);
            db.SaveChanges();
            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        public void Descargar(string rutaArchivo)
        {
            if (seguridad == null)
            {
                Response.Write("<script>alert('Usuario no conectado');</script>");
            }
            else
            {
                string filename = rutaArchivo;

                if (!String.IsNullOrEmpty(rutaArchivo))
                {

                    String path = Server.MapPath(rutaArchivo);
                    System.IO.FileInfo toDownload = new System.IO.FileInfo(path);

                    if (toDownload.Exists)
                    {
                        Response.Clear();
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + toDownload.Name);
                        Response.AddHeader("Content-Length", toDownload.Length.ToString());
                        Response.ContentType = "application/octet-stream";
                        Response.WriteFile(rutaArchivo);
                        Response.End();
                    }
                    else
                    {
                        Response.Write("<script>alert('Archivo no existe');window.history.back();</script>");
                    }
                }
            }            

        }

        #endregion

        public ActionResult ModalEditarLicitacion()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult CargarDocOferta()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ModalEditarOferta()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ModalVerActivo()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                return View();
            }
        }

    }
}