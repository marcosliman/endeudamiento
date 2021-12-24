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
    public class ContratoController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato


        public ActionResult RegistrarContrato()
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

        public ActionResult ListaContrato_Read(int? idBanco, int? idEmpresa, int? idTipoFinanciamiento)
        {
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado.ToList() on c.IdEstado equals e.IdEstado
                            where c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            && c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            && c.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : c.IdTipoFinanciamiento)
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
                                FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy")
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
                var contrato = new ContratoViewModel();
                contrato.IdContrato = idContrato;
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
                                    ExisteContrato = "S",
                                    TituloBoton = "Actualizar Contrato"
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new ContratoViewModel();
                    registro.IdLicitacionOferta = 0;
                    registro.IdEmpresa = 0;
                    registro.IdBanco = 0;
                    registro.IdTipoImpuesto = 0;
                    registro.Monto = null;
                    registro.TasaMensual = null;
                    registro.TasaAnual = null;
                    registro.Plazo = null;
                    registro.ExisteContrato = "N";
                    registro.TituloBoton = "Grabar Contrato";
                }

                var licitacionOferta = db.LicitacionOferta.Where(c => c.IdLicitacionOferta == registro.IdLicitacionOferta).FirstOrDefault();
                var idLicitacion = 0;
                if (licitacionOferta != null) {
                    idLicitacion = licitacionOferta.IdLicitacion;
                }

                var licitacion = (from e in db.Licitacion
                             where e.IdEstado == (int)Helper.Estado.LicCompleta
                             select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                ViewData["listaLicitacion"] = listaLicitacion;

                var oferta = (from e in db.LicitacionOferta 
                                        join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                        join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacion == idLicitacion
                                        select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                ViewData["listaOferta"] = listaOferta;

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
                              Plazo = e.Plazo

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

                            dato.FechaTermino = dato.FechaInicio.AddMonths(dato.Plazo);
                            dato.IdTipoFinanciamiento = (int)Helper.TipoFinanciamiento.Leasing;

                            /*verifico si el origen es una licitacion*/
                            var existeOferta = "N";
                            var oferta = new LicitacionOfertaViewModel();
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
                                                  Plazo = e.Plazo

                                              }).FirstOrDefault();

                                if(oferta != null) {
                                    dato.TasaMensual = oferta.TasaMensual;
                                    dato.TasaAnual = oferta.TasaAnual;
                                    dato.Plazo = oferta.Plazo;
                                    dato.IdEmpresa = (int)oferta.IdEmpresa;
                                    dato.IdBanco = oferta.IdBanco;
                                    existeOferta = "S";
                                }
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
                                addContrato.IdTipoContrato = (int)Helper.TipoContrato.Leasing;
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

                        /*var dbArchivo = (from doc in db.LicitacionOfertaDocumento
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
                        }*/

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

        #region Activo
        public ActionResult ListaActivoContrato_Read(int idContrato)
        {

            var registro = (from ac in db.Activo
                            join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
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
                                Patente = ac.Patente
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

        public ActionResult AddDocumentoLeasing()
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

        public ActionResult RegistrarContratoCredito()
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
                return View();
            }
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