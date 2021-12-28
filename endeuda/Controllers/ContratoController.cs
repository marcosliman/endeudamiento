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
    public class ContratoController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato


        public ActionResult ListaContratoLeasing()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
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

        public ActionResult ListaContrato_Read(int tipoContrato, int? idBanco, int? idEmpresa, string numeroContrato)
        {
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado.ToList() on c.IdEstado equals e.IdEstado
                            where c.IdTipoContrato == tipoContrato
                            && c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            && c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            && c.NumeroContrato == ((numeroContrato != "") ? numeroContrato : c.NumeroContrato)
                            select new ContratoViewModel
                            {
                                IdContrato = c.IdContrato,
                                NumeroContrato = c.NumeroContrato,
                                RazonSocial = c.Empresa.RazonSocial,
                                NombreBanco = c.Banco.NombreBanco,
                                NombreTipoFinanciamiento = (c.TipoFinanciamiento != null) ? c.TipoFinanciamiento.NombreTipoFinanciamiento : string.Empty,
                                Plazo = c.Plazo,
                                Monto = c.Monto,
                                TasaAnual = c.TasaAnual,
                                TasaMensual = c.TasaMensual,
                                FechaInicio = c.FechaInicio,
                                FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                FechaTermino = c.FechaTermino,
                                FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (c.IdEstado != (int)Helper.Estado.ConCreado) ? false : true
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }



        public ActionResult RegistrarContratoLeasing(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                //var contrato = new ContratoViewModel();
                var contrato = (from c in db.Contrato where c.IdContrato == idContrato
                                select new ContratoViewModel {
                                    IdContrato = c.IdContrato,
                                    ExisteContrato = "S"
                                }).FirstOrDefault();
                if (contrato == null) {
                    contrato = new ContratoViewModel();
                    contrato.IdContrato = idContrato;
                    contrato.ExisteContrato = "N";
                }
                
                return View(contrato);
            }
        }

        public ActionResult AddContratoLeasing(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from c in db.Contrato.ToList()
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    IdLicitacionOferta = c.IdLicitacionOferta,
                                    EsLicitacion = (c.IdLicitacionOferta != null) ? "SI" : "NO",
                                    MotivoEleccion = c.MotivoEleccion,
                                    IdEmpresa = c.IdEmpresa,
                                    IdBanco = c.IdBanco,
                                    IdTipoImpuesto = c.IdTipoImpuesto,
                                    NumeroContrato = c.NumeroContrato,
                                    TasaMensual = c.TasaMensual,
                                    TasaAnual = c.TasaAnual,
                                    Plazo = c.Plazo,
                                    Monto = c.Monto,
                                    FechaInicio = c.FechaInicio,
                                    FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                    FechaTermino = c.FechaTermino,
                                    FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                    IdEstado = c.IdEstado,
                                    ExisteContrato = "S",
                                    TituloBoton = "Actualizar Contrato"
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new ContratoViewModel();
                    registro.IdLicitacionOferta = 0;
                    registro.EsLicitacion = "NA";
                    registro.IdEmpresa = 0;
                    registro.IdBanco = 0;
                    registro.IdTipoImpuesto = 0;
                    registro.Monto = null;
                    registro.TasaMensual = null;
                    registro.TasaAnual = null;
                    registro.Plazo = null;
                    registro.IdEstado = 0;
                    registro.ExisteContrato = "N";
                    registro.TituloBoton = "Grabar Contrato";
                }

                var licitacionOferta = db.LicitacionOferta.Where(c => c.IdLicitacionOferta == registro.IdLicitacionOferta).FirstOrDefault();
                var idLicitacion = 0;
                var idLicitacionOferta = 0;

                if (licitacionOferta != null)
                {
                    idLicitacion = licitacionOferta.IdLicitacion;
                    idLicitacionOferta = licitacionOferta.IdLicitacionOferta;

                    var licitacion = (from e in db.Licitacion
                                      where e.IdLicitacion == idLicitacion
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacionOferta == idLicitacionOferta
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }
                else {
                    var licitacion = (from e in db.Licitacion
                                      where e.IdEstado == (int)Helper.Estado.LicCompleta && e.IdTipoFinanciamiento == (int)Helper.TipoContrato.Leasing
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacion == idLicitacion
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }


                var banco = (from e in db.Banco
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre",registro.IdBanco);
                ViewData["listaBanco"] = listaBanco;

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre",registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var impuesto = (from e in db.TipoImpuesto
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdTipoImpuesto, Nombre = e.NombreTipoImpuesto }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoImpuesto = new SelectList(impuesto.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoImpuesto);
                ViewData["listaTipoImpuesto"] = listaTipoImpuesto;

                return View(registro);
            }
        }

        public JsonResult ListaOferta(int idLicitacion)
        {
            var oferta = (from e in db.LicitacionOferta
                          join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                          join b in db.Banco on e.IdBanco equals b.IdBanco
                          where e.IdLicitacion == idLicitacion
                          select new RetornoGenerico
                           {
                               Id = e.IdLicitacionOferta,
                               Nombre = b.NombreBanco,
                               Seleccionado = false,
                               Editable = true
                           }).OrderBy(c => c.Id).ToList();
            return Json(oferta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOferta(int idLicitacionOferta)
        {
            var oferta = (from e in db.LicitacionOferta
                          join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                          join b in db.Banco on e.IdBanco equals b.IdBanco
                          where e.IdLicitacionOferta == idLicitacionOferta
                          select new LicitacionOfertaViewModel
                          {
                              IdLicitacionOferta = e.IdLicitacionOferta,
                              IdBanco = e.IdBanco,
                              IdEmpresa = l.IdEmpresa,
                              TasaMensual = e.TasaMensual,
                              TasaAnual = e.TasaAnual,
                              Plazo = e.Plazo,
                              IdTipoFinanciamiento = l.IdTipoFinanciamiento

                          }).FirstOrDefault();
            return Json(oferta, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarContratoLeasing(Contrato dato)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    dato.MotivoEleccion = validarDatos.ValidaStr(dato.MotivoEleccion);
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";
                            var idContrato = 0;

                            var contrato = db.Contrato.Find(dato.IdContrato);

                            /*verifico si el origen es una licitacion*/
                            var existeOferta = "N";
                            var oferta = new LicitacionOfertaViewModel();
                            if (contrato != null){
                                dato.IdLicitacionOferta = contrato.IdLicitacionOferta;
                            }
                            if (dato.IdLicitacionOferta > 0) {
                                oferta = (from e in db.LicitacionOferta
                                              join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                              join b in db.Banco on e.IdBanco equals b.IdBanco
                                              where e.IdLicitacionOferta == dato.IdLicitacionOferta
                                              select new LicitacionOfertaViewModel
                                              {
                                                  IdLicitacionOferta = e.IdLicitacionOferta,
                                                  IdLicitacion = l.IdLicitacion,
                                                  IdBanco = e.IdBanco,
                                                  IdEmpresa = l.IdEmpresa,
                                                  TasaMensual = e.TasaMensual,
                                                  TasaAnual = e.TasaAnual,
                                                  Plazo = e.Plazo,
                                                  IdTipoFinanciamiento = l.IdTipoFinanciamiento


                                              }).FirstOrDefault();

                                if(oferta != null) {
                                    dato.TasaMensual = (int)oferta.TasaMensual;
                                    dato.TasaAnual = (int)oferta.TasaAnual;
                                    dato.Plazo = (int)oferta.Plazo;
                                    dato.IdEmpresa = (int)oferta.IdEmpresa;
                                    dato.IdTipoFinanciamiento = (int)oferta.IdTipoFinanciamiento;
                                    dato.IdBanco = oferta.IdBanco;
                                    existeOferta = "S";
                                }
                            }

                            dato.FechaTermino = dato.FechaInicio.AddMonths(dato.Plazo);
                            if (dato.IdTipoContrato == (int)Helper.TipoContrato.Leasing) {
                                dato.IdTipoFinanciamiento = (int)Helper.TipoFinanciamiento.Leasing;
                                dato.IdTipoContrato = (int)Helper.TipoContrato.Leasing;
                            }
                            else {
                                dato.IdTipoContrato = (int)Helper.TipoContrato.Contrato;
                            }

                            if (contrato != null)
                            {
                                mensaje = "Contrato actualizado con éxito";
                                idContrato = dato.IdContrato;
                                contrato.IdLicitacionOferta = dato.IdLicitacionOferta;
                                contrato.Monto = dato.Monto;
                                contrato.NumeroContrato = dato.NumeroContrato;
                                contrato.MotivoEleccion = dato.MotivoEleccion;
                                contrato.IdEmpresa = dato.IdEmpresa;
                                contrato.IdBanco = dato.IdBanco;
                                contrato.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                                contrato.IdTipoImpuesto = dato.IdTipoImpuesto;
                                contrato.TipoGarantia = dato.TipoGarantia;
                                contrato.TasaMensual = dato.TasaMensual;
                                contrato.TasaAnual = dato.TasaAnual;
                                contrato.Plazo = dato.Plazo;
                                contrato.FechaInicio = dato.FechaInicio;
                                contrato.FechaTermino = dato.FechaTermino;
                                db.SaveChanges();
                            }
                            else
                            {
                                mensaje = "Contrato creado con éxito";

                                var addContrato = new Contrato();
                                addContrato.IdTipoContrato = dato.IdTipoContrato;
                                addContrato.IdLicitacionOferta = dato.IdLicitacionOferta;
                                addContrato.Monto = dato.Monto;
                                addContrato.NumeroContrato = dato.NumeroContrato;
                                addContrato.MotivoEleccion = dato.MotivoEleccion;
                                addContrato.IdEmpresa = dato.IdEmpresa;
                                addContrato.IdBanco = dato.IdBanco;
                                addContrato.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                                addContrato.IdTipoImpuesto = dato.IdTipoImpuesto;
                                addContrato.TipoGarantia = dato.TipoGarantia;
                                addContrato.TasaMensual = dato.TasaMensual;
                                addContrato.TasaAnual = dato.TasaAnual;
                                addContrato.Plazo = dato.Plazo;
                                addContrato.FechaInicio = Convert.ToDateTime(dato.FechaInicio);
                                addContrato.FechaTermino = Convert.ToDateTime(dato.FechaTermino);
                                addContrato.IdEstado = (int)Helper.Estado.ConCreado;
                                addContrato.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addContrato.FechaRegistro = DateTime.Now;
                                db.Contrato.Add(addContrato);
                                db.SaveChanges();

                                idContrato = addContrato.IdContrato;
                            }

                            /*si es de oferta grabo los activos de la licitacion*/
                            if (existeOferta == "S") {
                                var activo = db.LicitacionActivo.Where(c => c.IdLicitacion == oferta.IdLicitacion).ToList();
                                foreach (var ac in activo) {
                                    var existeActivo = db.ContratoActivo.Where(c => c.IdContrato == idContrato && c.IdActivo == ac.IdActivo).FirstOrDefault();
                                    if (existeActivo == null) {
                                        var addActivo = new ContratoActivo();
                                        addActivo.IdContrato = idContrato;
                                        addActivo.IdActivo = ac.IdActivo;
                                        db.ContratoActivo.Add(addActivo);
                                        db.SaveChanges();

                                        db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", (int)Helper.Estado.ActEnContrato, ac.IdActivo);
                                        db.SaveChanges();
                                    }
                                }

                                db.Database.ExecuteSqlCommand("UPDATE Licitacion SET IdEstado = {0} WHERE IdLicitacion = {1}", (int)Helper.Estado.LicContrato, oferta.IdLicitacion);
                                db.SaveChanges();

                            }

                            /*registro log contrato*/
                            GrabaLogContrato(idContrato,1);

                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = mensaje, idContrato = idContrato };
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
        public JsonResult DeleteContrato(int idContrato)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbContrato = db.Contrato.Find(idContrato);

                    if (dbContrato != null)
                    {

                        var dbArchivo = (from doc in db.ContratoActivoDocumento
                                         join of in db.ContratoActivo on doc.IdContratoActivo equals of.IdContratoActivo
                                         where of.IdContrato == dbContrato.IdContrato
                                         select new
                                         {
                                             doc.IdContratoActivoDocumento,
                                             of.IdContrato,
                                             of.IdContratoActivo,
                                             doc.UrlDocumento
                                         }).ToList();
                        foreach (var arc in dbArchivo)
                        {
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivoDocumento WHERE IdContratoActivoDocumento = {0}", arc.IdContratoActivoDocumento);
                            db.SaveChanges();
                        }

                        /*si viene de licitacion devuelvo el activo a la licitación, caso contrario lo dejo disponible*/
                        var dbActivo = db.ContratoActivo.Where(c => c.IdContrato == idContrato);
                        if (dbContrato.IdLicitacionOferta != null)
                        {
                            var oferta = db.LicitacionOferta.Find(dbContrato.IdLicitacionOferta);
                            if (oferta != null)
                            {
                                db.Database.ExecuteSqlCommand("UPDATE Licitacion SET IdEstado = {0} WHERE IdLicitacion = {1}", (int)Helper.Estado.LicCompleta, oferta.IdLicitacion);
                                db.SaveChanges();

                                foreach (var act in dbActivo)
                                {
                                    db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", Helper.Estado.ActLicitacion, act.IdActivo);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else {
                            foreach (var act in dbActivo)
                            {
                                db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", Helper.Estado.ActDisponible, act.IdActivo);
                                db.SaveChanges();
                            }
                        }

                        


                        db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivo WHERE IdContrato = {0}", dbContrato.IdContrato);
                        db.SaveChanges();


                        db.Contrato.Remove(dbContrato);
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

        [HttpPost]
        public JsonResult ActivarContrato(int idContrato)
        {
            dynamic showMessageString = string.Empty;
            var contrato = db.Contrato.Find(idContrato);
            var activos = db.ContratoActivo.Where(c => c.IdContrato == idContrato).Count();
            if (activos > 0)
            {
                contrato.IdEstado = (int)Helper.Estado.ConActivo;
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Contrato Activado Exitosamente" };
            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = "Existen Datos Incompletos" };
            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        /*Guarda log contrato*/
        private bool GrabaLogContrato(int idContrato, int idTipoLog)
        {
            //tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            var dbTipoLog = db.TipoLog.Find(idTipoLog);
            var dbContrato = db.Contrato.Find(idContrato);

            if (dbContrato != null)
            {
                if(dbContrato.IdEstado > (int)Helper.Estado.ConCreado) { 
                    var nombreLog = "Cambios realizados en: " + dbTipoLog.NombreTipoLog;
                    var addLog = new ContratoLog();
                    addLog.IdContrato = dbContrato.IdContrato;
                    addLog.IdTipoLog = dbTipoLog.IdTipoLog;
                    addLog.NombreLog = nombreLog;
                    addLog.IdUsuarioResgistro = (int)seguridad.IdUsuario;
                    addLog.FechaRegistro = DateTime.Now;
                    db.ContratoLog.Add(addLog);
                    db.SaveChanges();
                }
            }


            return true;
        }

        #region Contrato Credito
        public ActionResult ListaContratoCredito()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
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

        public ActionResult RegistrarContratoCredito(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var contrato = (from c in db.Contrato
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    ExisteContrato = "S"
                                }).FirstOrDefault();
                if (contrato == null)
                {
                    contrato = new ContratoViewModel();
                    contrato.IdContrato = idContrato;
                    contrato.ExisteContrato = "N";
                }
                return View(contrato);
            }
        }


        public ActionResult AddContratoCredito(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from c in db.Contrato.ToList()
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    IdLicitacionOferta = c.IdLicitacionOferta,
                                    EsLicitacion = (c.IdLicitacionOferta != null) ? "SI" : "NO",
                                    MotivoEleccion = c.MotivoEleccion,
                                    TipoGarantia = c.TipoGarantia,
                                    IdEmpresa = c.IdEmpresa,
                                    IdBanco = c.IdBanco,
                                    IdTipoFinanciamiento = c.IdTipoFinanciamiento,
                                    NumeroContrato = c.NumeroContrato,
                                    TasaMensual = c.TasaMensual,
                                    TasaAnual = c.TasaAnual,
                                    Plazo = c.Plazo,
                                    Monto = c.Monto,
                                    FechaInicio = c.FechaInicio,
                                    FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                    FechaTermino = c.FechaTermino,
                                    FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                    IdEstado = c.IdEstado,
                                    ExisteContrato = "S",
                                    TituloBoton = "Actualizar Contrato"
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new ContratoViewModel();
                    registro.IdLicitacionOferta = 0;
                    registro.EsLicitacion = "NA";
                    registro.IdEmpresa = 0;
                    registro.IdBanco = 0;
                    registro.IdTipoFinanciamiento = 0;
                    registro.Monto = null;
                    registro.TasaMensual = null;
                    registro.TasaAnual = null;
                    registro.Plazo = null;
                    registro.IdEstado = 0;
                    registro.ExisteContrato = "N";
                    registro.TituloBoton = "Grabar Contrato";
                }

                var licitacionOferta = db.LicitacionOferta.Where(c => c.IdLicitacionOferta == registro.IdLicitacionOferta).FirstOrDefault();
                var idLicitacion = 0;
                var idLicitacionOferta = 0;

                if (licitacionOferta != null)
                {
                    idLicitacion = licitacionOferta.IdLicitacion;
                    idLicitacionOferta = licitacionOferta.IdLicitacionOferta;

                    var licitacion = (from e in db.Licitacion
                                      where e.IdLicitacion == idLicitacion
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacionOferta == idLicitacionOferta
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }
                else
                {
                    var licitacion = (from e in db.Licitacion
                                      where e.IdEstado == (int)Helper.Estado.LicCompleta && e.IdTipoFinanciamiento != (int)Helper.TipoContrato.Leasing
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacion == idLicitacion
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }


                var banco = (from e in db.Banco
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdBanco);
                ViewData["listaBanco"] = listaBanco;

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var tipoCredito = (from e in db.TipoFinanciamiento
                                where e.Activo == true && e.IdTipoContrato == (int)Helper.TipoContrato.Contrato
                                select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoCredito = new SelectList(tipoCredito.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoFinanciamiento);
                ViewData["listaTipoCredito"] = listaTipoCredito;

                return View(registro);
            }
        }

        #endregion

        #region Activo
        public ActionResult ListaActivoContrato_Read(int idContrato)
        {

            var registro = (from ac in db.Activo
                            join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
                            join con in db.Contrato on rel.IdContrato equals con.IdContrato
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join pr in db.Proveedor on ac.IdProveedor equals pr.IdProveedor into prw
                            from prv in prw.DefaultIfEmpty()
                            where rel.IdContrato == idContrato
                            select new ActivoViewModel
                            {
                                IdContratoActivo = rel.IdContratoActivo,
                                IdActivo = ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = ac.Familia,
                                NombreCuenta = "",
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                MotorChasis = ac.MotorChasis,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NombreProveedor = (prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente,
                                Editable = (con.IdLicitacionOferta != null) ? false : true
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalAsociarActivo(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var contratoActivo = new ContratoActivo();
                contratoActivo.IdContrato = idContrato;

                return PartialView(contratoActivo);
            }
        }

        public ActionResult ListaActivoAsociar_Read(int idContrato, int? numeroActivo, string codigoActivo)
        {
            var idEmpresa = 0;
            var contrato = db.Contrato.Find(idContrato);
            if (contrato != null)
            {
                idEmpresa = contrato.IdEmpresa;
            }

            var registro = (from ac in db.Activo
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join pr in db.Proveedor on ac.IdProveedor equals pr.IdProveedor into prw
                            from prv in prw.DefaultIfEmpty()
                            where ac.NumeroInterno == ((numeroActivo != null) ? numeroActivo : ac.NumeroInterno)
                                && ac.CodSoftland == ((codigoActivo != "") ? codigoActivo : ac.CodSoftland)
                                && ac.IdEmpresa == idEmpresa
                                && ac.IdEstado == (int)Helper.Estado.ActDisponible
                            select new ActivoViewModel
                            {
                                IdActivo = ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = ac.Familia,
                                NombreCuenta = "",
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                MotorChasis = ac.MotorChasis,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NombreProveedor = (prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult AsociarActivo(int idContrato, int[] activos)
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

                            foreach (int ac in activos)
                            {
                                var existeActivo = db.Activo.Where(c => c.IdActivo == ac && c.IdEstado == (int)Helper.Estado.ActDisponible).FirstOrDefault();
                                if (existeActivo != null)
                                {
                                    var addActivo = new ContratoActivo();
                                    addActivo.IdContrato = idContrato;
                                    addActivo.IdActivo = ac;
                                    db.ContratoActivo.Add(addActivo);
                                    db.SaveChanges();

                                    /*cambio el estado del activo*/
                                    existeActivo.IdEstado = (int)Helper.Estado.ActEnContrato;
                                    db.SaveChanges();
                                }
                            }

                            /*registro log contrato*/
                            GrabaLogContrato(idContrato, 4);

                            var mensaje = "";
                            mensaje = "Asociación realizada con éxito";

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
        public JsonResult DeleteAsociacionActivo(int idContratoActivo)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbContratoActivo = db.ContratoActivo.Find(idContratoActivo);

                    if (dbContratoActivo != null)
                    {

                        db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", (int)Helper.Estado.ActDisponible, dbContratoActivo.IdActivo);
                        db.SaveChanges();


                        /*registro log contrato*/
                        GrabaLogContrato(dbContratoActivo.IdContrato, 4);

                        var dbArchivo = (from doc in db.ContratoActivoDocumento
                                         join of in db.ContratoActivo on doc.IdContratoActivo equals of.IdContratoActivo
                                         where of.IdContratoActivo == dbContratoActivo.IdContratoActivo
                                         select new
                                         {
                                             doc.IdContratoActivoDocumento,
                                             of.IdContrato,
                                             of.IdContratoActivo,
                                             doc.UrlDocumento
                                         }).ToList();
                        foreach (var arc in dbArchivo)
                        {
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivoDocumento WHERE IdContratoActivoDocumento = {0}", arc.IdContratoActivoDocumento);
                            db.SaveChanges();
                        }

                        db.ContratoActivo.Remove(dbContratoActivo);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        showMessageString = new { Estado = 0, Mensaje = "Asociación eliminada con exito" };
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


        public ActionResult AddAmortizacion()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        #region Documento Leasing
        public ActionResult AddDocumentoLeasing(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var registro = (from ac in db.Activo
                                join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
                                join con in db.Contrato on rel.IdContrato equals con.IdContrato
                                where rel.IdContrato == idContrato
                                select new ContratoActivoViewModel
                                {
                                    IdContratoActivo = rel.IdContratoActivo,
                                    IdActivo = ac.IdActivo,
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    Familia = ac.Familia,
                                    Archivos = (from d in db.ContratoActivoDocumento
                                                join t in db.TipoDocumento on d.IdTipoDocumento equals t.IdTipoDocumento
                                                where d.IdContratoActivo == rel.IdContratoActivo
                                                select new ContratoDocumentoViewModel {
                                                    IdContratoActivoDocumento = d.IdContratoActivoDocumento,
                                                    NombreTipoDocumento = t.NombreTipoDocumento,
                                                    UrlDocumento = d.UrlDocumento,
                                                    NombreOriginal = d.NombreOriginal
                                                }).ToList()
                                }).AsEnumerable().ToList();

                var tipoDocumento = (from e in db.TipoDocumento
                             where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.ActivoContrato
                             select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;

                /*verificacion activar contrato*/
                ViewData["puedeActivar"] = "N";
                ViewData["urlRetorno"] = "/Contrato/RegistrarContratoCredito";
                var conActivo = db.Contrato.Where(c => c.IdContrato == idContrato).FirstOrDefault();
                if (conActivo != null) {
                    if(conActivo.IdEstado == (int)Helper.Estado.ConCreado) { 
                        ViewData["puedeActivar"] = "S";
                    }
                    if (conActivo.IdTipoContrato == (int)Helper.TipoContrato.Leasing) {
                        ViewData["urlRetorno"] = "/Contrato/RegistrarContratoLeasing";
                    }
                }

                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarContratoDocumento(ContratoActivoDocumento dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            ContratoActivoDocumento addDocumento = new ContratoActivoDocumento();
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
                                var contrato = db.ContratoActivo.Where(c => c.IdContratoActivo == dato.IdContratoActivo).FirstOrDefault();
                                var pathDocumento = "";
                                var fileName = dato.IdTipoDocumento.ToString()+'_'+Path.GetFileName(archivo.FileName);
                                var carpeta = "Contrato/"+ contrato.IdContrato;
                                var pathData = "~/App_Data";
                                var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta); ;
                                if (!Directory.Exists(pathCarpeta))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                }
                                string carpetaSolicitud = contrato.IdContratoActivo.ToString();
                                var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                if (!Directory.Exists(pathCarpetaSolicitud))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                }
                                pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                archivo.SaveAs(physicalPath);

                                addDocumento.IdContratoActivo = (int)contrato.IdContratoActivo;
                                addDocumento.IdTipoDocumento = dato.IdTipoDocumento;
                                addDocumento.FechaRegistro = DateTime.Now;
                                addDocumento.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addDocumento.NombreOriginal = fileName;
                                addDocumento.UrlDocumento = pathDocumento;
                                db.ContratoActivoDocumento.Add(addDocumento);
                                db.SaveChanges();
                                mensaje = "Archivo Cargado con exito";

                                /*registro log contrato*/
                                GrabaLogContrato(contrato.IdContrato, 3);
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

        public ActionResult ListaContratoDocumento_Read(int idContratoActivo)
        {
            var registro = (from c in db.ContratoActivoDocumento
                            join td in db.TipoDocumento on c.IdTipoDocumento equals td.IdTipoDocumento
                            where c.IdContratoActivo == idContratoActivo
                            select new ContratoDocumentoViewModel
                            {
                                IdContratoActivoDocumento = c.IdContratoActivoDocumento,
                                IdContratoActivo = idContratoActivo,
                                NombreTipoDocumento = td.NombreTipoDocumento,
                                NombreOriginal = c.NombreOriginal,
                                UrlDocumento = c.UrlDocumento
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteContratoDocumento(int idContratoActivoDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.ContratoActivoDocumento.Find(idContratoActivoDocumento);

            var contrato = (from c in db.ContratoActivoDocumento
                            join ca in db.ContratoActivo on c.IdContratoActivo equals ca.IdContratoActivo
                            where c.IdContratoActivoDocumento == idContratoActivoDocumento
                            select new { ca.IdContrato }).FirstOrDefault();

            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivoDocumento WHERE IdContratoActivoDocumento = {0}", idContratoActivoDocumento);
            db.SaveChanges();


            /*registro log contrato*/
            if(contrato != null) { 
                GrabaLogContrato(contrato.IdContrato, 3);
            }


            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public ActionResult ContratoBuscar()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Acceder))
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

        public ActionResult ListaBuscarContrato_Read(int? idBanco, int? idEmpresa, int? idTipoFinanciamiento, string numeroContrato)
        {
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado.ToList() on c.IdEstado equals e.IdEstado
                            where c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            && c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            && c.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : c.IdTipoFinanciamiento)
                            && c.NumeroContrato == ((numeroContrato != "") ? numeroContrato : c.NumeroContrato)
                            select new ContratoViewModel
                            {
                                IdContrato = c.IdContrato,
                                NumeroContrato = c.NumeroContrato,
                                RazonSocial = c.Empresa.RazonSocial,
                                NombreBanco = c.Banco.NombreBanco,
                                NombreTipoFinanciamiento = (c.TipoFinanciamiento != null) ? c.TipoFinanciamiento.NombreTipoFinanciamiento : string.Empty,
                                Plazo = c.Plazo,
                                Monto = c.Monto,
                                TasaAnual = c.TasaAnual,
                                TasaMensual = c.TasaMensual,
                                FechaInicio = c.FechaInicio,
                                FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                FechaTermino = c.FechaTermino,
                                FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (c.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                                Descripcion=""
                                /*Descripcion = (from ca in db.ContratoActivo 
                                                join a in db.Activo on ca.IdActivo equals a.IdActivo
                                                where ca.IdContrato == c.IdContrato
                                                select new {a.Familia } into x
                                                group x by new {x.Familia } into g
                                                select new {
                                                    g.Key.Familia.ToString()
                                                }).FirstOrDefault()*/
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalRegistrarActivo()
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
        public ActionResult ModalEditarContratoLeasing()
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
        public ActionResult ModalEditarContratoCredito()
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
        
        public ActionResult ModalContratoLeasingOrigen()
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