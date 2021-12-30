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
    public class SeguroController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;

        #region Poliza
        public ActionResult ListaSeguro()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarSeguro", Helper.TipoAcceso.Acceder))
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

                var empresaAseguradora = (from e in db.EmpresaAseguradora
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdEmpresaAseguradora, Nombre = e.NombreEmpresaAseguradora }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaAseguradora = new SelectList(empresaAseguradora.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresaAseguradora"] = listaEmpresaAseguradora;

                var tipoSeguro = (from e in db.TipoSeguro
                                         where e.Activo == true
                                         select new RetornoGenerico { Id = e.IdTipoSeguro, Nombre = e.NombreTipoSeguro }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoSeguro = new SelectList(tipoSeguro.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoSeguro"] = listaTipoSeguro;

                return View();
            }
        }

        public ActionResult ListaSeguro_Read(int? idEmpresa, int? idEmpresaAseguradora, int? idTipoSeguro, string numeroPoliza)
        {
            var registro = (from p in db.Poliza.ToList()
                            join e in db.Estado.ToList() on p.IdEstado equals e.IdEstado
                            where p.IdEmpresa == ((idEmpresa != null) ? idEmpresa : p.IdEmpresa)
                            && p.IdEmpresaAseguradora == ((idEmpresaAseguradora != null) ? idEmpresaAseguradora : p.IdEmpresaAseguradora)
                            && p.IdTipoSeguro == ((idTipoSeguro != null) ? idTipoSeguro : p.IdTipoSeguro)
                            && p.NumeroPoliza == ((numeroPoliza != "") ? numeroPoliza : p.NumeroPoliza)
                            select new PolizaViewModel
                            {
                                IdPoliza = p.IdPoliza,
                                NumeroPoliza = p.NumeroPoliza,
                                RazonSocial = p.Empresa.RazonSocial,
                                NombreTipoSeguro = p.TipoSeguro.NombreTipoSeguro,
                                NombreEmpresaAseguradora = p.EmpresaAseguradora.NombreEmpresaAseguradora,
                                Beneficiario = p.Beneficiario,
                                RutBeneficiario = p.RutBeneficiario,
                                MontoAsegurado = p.MontoAsegurado,
                                FechaVencimiento = p.FechaVencimiento,
                                FechaVencimientoStr = p.FechaVencimiento.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (p.IdEstado != (int)Helper.Estado.PolCreado) ? false : true
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegistrarSeguro(int idPoliza)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarSeguro", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var poliza = (from p in db.Poliza
                                where p.IdPoliza == idPoliza
                              select new PolizaViewModel
                                {
                                    IdPoliza = p.IdPoliza,
                                    ExistePoliza = "S"
                                }).FirstOrDefault();
                if (poliza == null)
                {
                    poliza = new PolizaViewModel();
                    poliza.IdPoliza = idPoliza;
                    poliza.ExistePoliza = "N";
                }

                return View(poliza);
            }
        }

        public ActionResult AddSeguro(int idPoliza)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarSeguro", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from p in db.Poliza.ToList()
                                where p.IdPoliza == idPoliza
                                select new PolizaViewModel
                                {
                                    IdPoliza = p.IdPoliza,
                                    NumeroPoliza = p.NumeroPoliza,
                                    IdEmpresa = p.IdEmpresa,
                                    IdEmpresaAseguradora = p.IdEmpresaAseguradora,
                                    IdTipoSeguro = p.IdTipoSeguro,
                                    IdTipoMoneda = p.IdTipoMoneda,
                                    MontoAsegurado = p.MontoAsegurado,
                                    PrimaMensual = p.PrimaMensual,
                                    NumeroPagos = p.NumeroPagos,
                                    FechaVencimientoStr = p.FechaVencimiento.ToString("dd-MM-yyyy"),
                                    Beneficiario = p.Beneficiario,
                                    RutBeneficiario = p.RutBeneficiario,
                                    FechaEnvioBanco = p.FechaEnvioBanco,
                                    FechaEnvioBancoStr = p.FechaEnvioBanco.ToString("dd-MM-yyyy"),
                                    IdEstado = p.IdEstado,
                                    ExistePoliza = "S",
                                    TituloBoton = "Actualizar Seguro"
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new PolizaViewModel();
                    registro.IdEmpresa = 0;
                    registro.IdEmpresaAseguradora = 0;
                    registro.IdTipoSeguro = 0;
                    registro.IdTipoMoneda = 0;
                    registro.MontoAsegurado = null;
                    registro.PrimaMensual = null;
                    registro.NumeroPagos = null;
                    registro.IdEstado = 0;
                    registro.ExistePoliza = "N";
                    registro.TituloBoton = "Grabar Seguro";
                }

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var empresaAseguradora = (from e in db.EmpresaAseguradora
                                          where e.Activo == true
                                          select new RetornoGenerico { Id = e.IdEmpresaAseguradora, Nombre = e.NombreEmpresaAseguradora }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaAseguradora = new SelectList(empresaAseguradora.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresaAseguradora);
                ViewData["listaEmpresaAseguradora"] = listaEmpresaAseguradora;

                var tipoSeguro = (from e in db.TipoSeguro
                                  where e.Activo == true
                                  select new RetornoGenerico { Id = e.IdTipoSeguro, Nombre = e.NombreTipoSeguro }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoSeguro = new SelectList(tipoSeguro.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoSeguro);
                ViewData["listaTipoSeguro"] = listaTipoSeguro;

                var tipoMoneda = (from e in db.TipoMoneda
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdTipoMoneda, Nombre = e.NombreTipoMoneda }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoMoneda = new SelectList(tipoMoneda.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoMoneda);
                ViewData["listaTipoMoneda"] = listaTipoMoneda;

                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarSeguro(Poliza dato)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarSeguro", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    dato.Beneficiario = validarDatos.ValidaStr(dato.Beneficiario);
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";
                            var idPoliza = 0;
                            var swTran = true;

                            var poliza = db.Poliza.Find(dato.IdPoliza);


                            if (poliza != null)
                            {
                                mensaje = "Poliza de seguro actualizada con éxito";
                                var existePoliza = db.Poliza.Where(c => c.NumeroPoliza == dato.NumeroPoliza && c.IdPoliza != poliza.IdPoliza).FirstOrDefault();
                                if (existePoliza == null)
                                {
                                    idPoliza = dato.IdPoliza;
                                    poliza.IdEmpresa = dato.IdEmpresa;
                                    poliza.IdEmpresaAseguradora = dato.IdEmpresaAseguradora;
                                    poliza.IdTipoSeguro = dato.IdTipoSeguro;
                                    poliza.IdTipoMoneda = dato.IdTipoMoneda;
                                    poliza.NumeroPoliza = dato.NumeroPoliza;
                                    poliza.MontoAsegurado = dato.MontoAsegurado;
                                    poliza.NumeroPagos = dato.NumeroPagos;
                                    poliza.PrimaMensual = dato.PrimaMensual;
                                    poliza.Beneficiario = dato.Beneficiario;
                                    poliza.RutBeneficiario = dato.RutBeneficiario;
                                    poliza.FechaVencimiento = dato.FechaVencimiento;
                                    poliza.FechaEnvioBanco = dato.FechaEnvioBanco;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    swTran = false;
                                    mensaje = "Numero Poliza ya existe, revise sus datos";
                                }
                            }
                            else
                            {
                                mensaje = "Poliza de seguro creada con éxito";

                                var existePoliza = db.Poliza.Where(c => c.NumeroPoliza == dato.NumeroPoliza).FirstOrDefault();
                                if (existePoliza == null)
                                {
                                    var addPoliza = new Poliza();
                                    addPoliza.IdEmpresa = dato.IdEmpresa;
                                    addPoliza.IdEmpresaAseguradora = dato.IdEmpresaAseguradora;
                                    addPoliza.IdTipoSeguro = dato.IdTipoSeguro;
                                    addPoliza.IdTipoMoneda = dato.IdTipoMoneda;
                                    addPoliza.NumeroPoliza = dato.NumeroPoliza;
                                    addPoliza.MontoAsegurado = dato.MontoAsegurado;
                                    addPoliza.NumeroPagos = dato.NumeroPagos;
                                    addPoliza.PrimaMensual = dato.PrimaMensual;
                                    addPoliza.Beneficiario = dato.Beneficiario;
                                    addPoliza.RutBeneficiario = dato.RutBeneficiario;
                                    addPoliza.FechaVencimiento = dato.FechaVencimiento;
                                    addPoliza.FechaEnvioBanco = dato.FechaEnvioBanco;
                                    addPoliza.IdEstado = (int)Helper.Estado.PolCreado;
                                    addPoliza.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    addPoliza.FechaRegistro = DateTime.Now;
                                    db.Poliza.Add(addPoliza);
                                    db.SaveChanges();

                                    idPoliza = addPoliza.IdPoliza;
                                }
                                else {
                                    swTran = false;
                                    mensaje = "Numero Poliza ya existe, revise sus datos";
                                }
                            }
                            /*solo ejecuta la transaccion si todo esta correcto*/
                            if (swTran)
                            {
                                dbContextTransaction.Commit();
                                showMessageString = new { Estado = 0, Mensaje = mensaje, idPoliza = idPoliza };
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                showMessageString = new { Estado = 100, Mensaje = mensaje };
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
        public JsonResult DeletePoliza(int idPoliza)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbPoliza = db.Poliza.Find(idPoliza);

                    if (dbPoliza != null)
                    {

                        var dbArchivo = (from doc in db.PolizaDocumento
                                         where doc.IdPoliza == dbPoliza.IdPoliza
                                         select new
                                         {
                                             doc.IdPolizaDocumento,
                                             doc.UrlDocumento
                                         }).ToList();
                        foreach (var arc in dbArchivo)
                        {
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM PolizaDocumento WHERE IdPolizaDocumento = {0}", arc.IdPolizaDocumento);
                            db.SaveChanges();
                        }

                        /*si viene de licitacion devuelvo el activo a la licitación, caso contrario lo dejo disponible*/
                        var dbActivo = db.PolizaActivo.Where(c => c.IdPoliza == idPoliza);
                        foreach (var act in dbActivo)
                        {
                            db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", Helper.Estado.ActDisponible, act.IdActivo);
                            db.SaveChanges();
                        }


                        db.Database.ExecuteSqlCommand("DELETE FROM PolizaActivo WHERE IdContrato = {0}", dbPoliza.IdPoliza);
                        db.SaveChanges();


                        db.Poliza.Remove(dbPoliza);
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
        public JsonResult ActivarPoliza(int idPoliza)
        {
            dynamic showMessageString = string.Empty;
            var poliza = db.Poliza.Find(idPoliza);
            var activos = db.PolizaActivo.Where(c => c.IdPoliza == idPoliza).Count();
            if (activos > 0)
            {
                poliza.IdEstado = (int)Helper.Estado.PolActivo;
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Poliza de seguro Activada Exitosamente" };
            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = "Existen Datos Incompletos" };
            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Activo
        public ActionResult ListaActivoPoliza_Read(int idPoliza)
        {

            var registro = (from ac in db.Activo.ToList()
                            join rel in db.PolizaActivo.ToList() on ac.IdActivo equals rel.IdActivo
                            join p in db.Poliza.ToList() on rel.IdPoliza equals p.IdPoliza
                            join e in db.Estado.ToList() on ac.IdEstado equals e.IdEstado
                            join em in db.Empresa.ToList() on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia.ToList() on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            join pr in db.Proveedor.ToList() on ac.IdProveedor equals pr.IdProveedor into prw
                            from prv in prw.DefaultIfEmpty()
                            where rel.IdPoliza == idPoliza
                            select new ActivoViewModel
                            {
                                IdPolizaActivo = rel.IdPolizaActivo,
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
                                Grupo = ac.Grupo,
                                SubGrupo = ac.SubGrupo,
                                Valor = ac.Valor,
                                NombreProveedor = (prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente,
                                Glosa = ac.Glosa,
                                FechaRegistroStr = "",
                                FechaBajaStr = "",
                                NombreEstado = e.NombreEstado,
                                NumeroLeasing = ""
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalAsociarActivo(int idPoliza)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var polizaActivo = new PolizaActivo();
                polizaActivo.IdPoliza = idPoliza;

                return PartialView(polizaActivo);
            }
        }

        public ActionResult ListaActivoAsociar_Read(int idPoliza, int? numeroActivo, string codigoActivo)
        {
            var idEmpresa = 0;
            var poliza = db.Poliza.Find(idPoliza);
            if (poliza != null)
            {
                idEmpresa = poliza.IdEmpresa;
            }

            var registro = (from ac in db.Activo
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
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
                                Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                NombreCuenta = "",
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                Motor = ac.Motor,
                                Chasis = ac.Chasis,
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

        public ActionResult AsociarActivo(int idPoliza, int[] activos)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarSeguro", Helper.TipoAcceso.Crear))
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
                                    var addActivo = new PolizaActivo();
                                    addActivo.IdPoliza = idPoliza;
                                    addActivo.IdActivo = ac;
                                    db.PolizaActivo.Add(addActivo);
                                    db.SaveChanges();

                                    /*cambio el estado del activo*/
                                    existeActivo.IdEstado = (int)Helper.Estado.ActEnPoliza;
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
        public JsonResult DeleteAsociacionActivo(int idPolizaActivo)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbPolizaActivo = db.PolizaActivo.Find(idPolizaActivo);

                    if (dbPolizaActivo != null)
                    {

                        db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", (int)Helper.Estado.ActDisponible, dbPolizaActivo.IdActivo);
                        db.SaveChanges();


                        db.PolizaActivo.Remove(dbPolizaActivo);
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

        #region Documento Poliza
        public ActionResult AddDocumento(int idPoliza)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarSeguro", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var tipoDocumento = (from e in db.TipoDocumento
                                     where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.Poliza
                                     select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;

                /*verificacion activar contrato*/
                ViewData["puedeActivar"] = "N";
                var conActivo = db.Poliza.Where(c => c.IdPoliza == idPoliza).FirstOrDefault();
                if (conActivo != null)
                {
                    if (conActivo.IdEstado == (int)Helper.Estado.PolCreado)
                    {
                        ViewData["puedeActivar"] = "S";
                    }
                }

                var registro = new PolizaDocumento();
                registro.IdPoliza = idPoliza;

                return View(registro);
            }
        }

        public ActionResult ListaPolizaDocumento_Read(int idPoliza)
        {
            var registro = (from c in db.PolizaDocumento
                            join td in db.TipoDocumento on c.IdTipoDocumento equals td.IdTipoDocumento
                            where c.IdPoliza == idPoliza
                            select new PolizaDocumentoViewModel
                            {
                                IdPolizaDocumento = c.IdPolizaDocumento,
                                IdPoliza = c.IdPoliza,
                                NombreTipoDocumento = td.NombreTipoDocumento,
                                NombreOriginal = c.NombreOriginal,
                                UrlDocumento = c.UrlDocumento
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarPolizaDocumento(PolizaDocumento dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            PolizaDocumento addDocumento = new PolizaDocumento();
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
                                var poliza = db.Poliza.Where(c => c.IdPoliza == dato.IdPoliza).FirstOrDefault();
                                var pathDocumento = "";
                                var fileName = dato.IdTipoDocumento.ToString() + '_' + Path.GetFileName(archivo.FileName);
                                var carpeta = "Poliza/";
                                var pathData = "~/App_Data";
                                var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta);
                                if (!Directory.Exists(pathCarpeta))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                }
                                string carpetaSolicitud = poliza.IdPoliza.ToString();
                                var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                if (!Directory.Exists(pathCarpetaSolicitud))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                }
                                pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                archivo.SaveAs(physicalPath);

                                addDocumento.IdPoliza = (int)poliza.IdPoliza;
                                addDocumento.IdTipoDocumento = dato.IdTipoDocumento;
                                addDocumento.FechaRegistro = DateTime.Now;
                                addDocumento.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addDocumento.NombreOriginal = fileName;
                                addDocumento.UrlDocumento = pathDocumento;
                                db.PolizaDocumento.Add(addDocumento);
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
        public JsonResult DeletePolizaDocumento(int idPolizaDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.PolizaDocumento.Find(idPolizaDocumento);

            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            db.Database.ExecuteSqlCommand("DELETE FROM PolizaDocumento WHERE IdPolizaDocumento = {0}", idPolizaDocumento);
            db.SaveChanges();


            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult SeguroBuscar()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("SeguroBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ModalBuscarActivo()
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
        public ActionResult ModalCargaMasiva()
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
        public ActionResult ModalEditarSeguro()
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

        public ActionResult ModalVerSeguro()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresa"] = listaEmpresa;

                var empresaAseguradora = (from e in db.EmpresaAseguradora
                                          where e.Activo == true
                                          select new RetornoGenerico { Id = e.IdEmpresaAseguradora, Nombre = e.NombreEmpresaAseguradora }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresaAseguradora = new SelectList(empresaAseguradora.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresaAseguradora"] = listaEmpresaAseguradora;

                var tipoSeguro = (from e in db.TipoSeguro
                                  where e.Activo == true
                                  select new RetornoGenerico { Id = e.IdTipoSeguro, Nombre = e.NombreTipoSeguro }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoSeguro = new SelectList(tipoSeguro.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoSeguro"] = listaTipoSeguro;
                return View();
            }
        }

        /*public ActionResult ListaSeguroBuscar_Read(int? idEmpresa, int? idEmpresaAseguradora, int? idTipoSeguro, string numeroPoliza, string numeroActivo)
        {
            var registro = (from p in db.Poliza.ToList()
                            join e in db.Estado.ToList() on p.IdEstado equals e.IdEstado
                            join rel in db.PolizaActivo.ToList() on p.IdPoliza equals rel.IdPoliza
                            join a in db.Activo.ToList() on rel.IdActivo equals a.IdActivo
                            where p.IdEmpresa == ((idEmpresa != null) ? idEmpresa : p.IdEmpresa)
                            && p.IdEmpresaAseguradora == ((idEmpresaAseguradora != null) ? idEmpresaAseguradora : p.IdEmpresaAseguradora)
                            && p.IdTipoSeguro == ((idTipoSeguro != null) ? idTipoSeguro : p.IdTipoSeguro)
                            && p.NumeroPoliza == ((numeroPoliza != "") ? numeroPoliza : p.NumeroPoliza)
                            select new PolizaViewModel
                            {
                                IdPoliza = p.IdPoliza,
                                NumeroPoliza = p.NumeroPoliza,
                                RazonSocial = p.Empresa.RazonSocial,
                                NombreTipoSeguro = p.TipoSeguro.NombreTipoSeguro,
                                NombreEmpresaAseguradora = p.EmpresaAseguradora.NombreEmpresaAseguradora,
                                Beneficiario = p.Beneficiario,
                                RutBeneficiario = p.RutBeneficiario,
                                MontoAsegurado = p.MontoAsegurado,
                                FechaVencimiento = p.FechaVencimiento,
                                FechaVencimientoStr = p.FechaVencimiento.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (p.IdEstado != (int)Helper.Estado.PolCreado) ? false : true,
                                ActivoViewModel = a.Familia
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult ModalAgregarSiniestro()
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