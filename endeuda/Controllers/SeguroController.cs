using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.IO;
using System.Text;
using System.Data;
using LinqToExcel;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using DocumentFormat.OpenXml.Math;

namespace tesoreria.Controllers
{
    public class SeguroController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        #region Poliza
        public ActionResult ListaSeguro()
        {
            
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro", "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
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

        public ActionResult ListaSeguro_Read(int? idEmpresa, int? idEmpresaAseguradora, int? idTipoSeguro, string numeroPoliza,int? IdEstado)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var registro = (from p in db.Poliza.ToList()
                            join e in db.Estado.ToList() on p.IdEstado equals e.IdEstado
                            where p.IdEmpresa == ((idEmpresa != null) ? idEmpresa : p.IdEmpresa)
                            && p.IdEmpresaAseguradora == ((idEmpresaAseguradora != null) ? idEmpresaAseguradora : p.IdEmpresaAseguradora)
                            && p.IdTipoSeguro == ((idTipoSeguro != null) ? idTipoSeguro : p.IdTipoSeguro)
                            && p.NumeroPoliza == ((numeroPoliza != "") ? numeroPoliza : p.NumeroPoliza)
                            && p.IdEstado == ((IdEstado != null) ? IdEstado : p.IdEstado)
                            select new PolizaViewModel
                            {
                                IdPoliza = p.IdPoliza,
                                NumeroPoliza = p.NumeroPoliza,
                                RazonSocial = p.Empresa.RazonSocial,
                                NombreTipoSeguro = p.TipoSeguro.NombreTipoSeguro,
                                NombreEmpresaAseguradora = p.EmpresaAseguradora.NombreEmpresaAseguradora,
                                MontoAsegurado = p.MontoAsegurado,
                                FechaVencimiento = p.FechaVencimiento,
                                FechaVencimientoStr = p.FechaVencimiento.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (p.IdEstado != (int)Helper.Estado.PolCreado) ? false : true
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegistrarSeguro(int idPoliza)
        {
            
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro", "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
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
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro", "ListaSeguro" }, Helper.TipoAcceso.Crear);

            if (tieneCrear.AccesoValido == false)

            {
                return RedirectToAction(tieneCrear.Vista, tieneCrear.Controlador);
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
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro" }, Helper.TipoAcceso.Crear);
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            if (tieneCrear.AccesoValido == false)

            {
                return Json(new { tieneCrear.Estado, tieneCrear.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //dato.Beneficiario = validarDatos.ValidaStr(dato.Beneficiario);
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
                        var dbActivo = db.PolizaActivo.Where(c => c.IdPoliza == idPoliza).ToList();
                        db.PolizaActivo.RemoveRange(dbActivo);

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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro", "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var registro = (from ac in db.Activo
                            join rel in db.PolizaActivo on ac.IdActivo equals rel.IdActivo
                            join p in db.Poliza on rel.IdPoliza equals p.IdPoliza
                            join e in db.Estado on ac.IdEstado equals e.IdEstado
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            //join pr in db.Proveedor.ToList() on ac.IdProveedor equals pr.IdProveedor into prw
                            //from prv in prw.DefaultIfEmpty()
                            where rel.IdPoliza == idPoliza
                            select new 
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
                                Grupo = ac.DesGrupo,
                                SubGrupo = ac.DesSGru,
                                Valor = ac.Valor,
                                NombreProveedor = "",//(prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente,
                                Glosa = ac.Glosa,
                                FechaRegistroStr = "",
                                FechaBajaStr = "",
                                NombreEstado = e.NombreEstado,
                                NumeroLeasing = "",
                                rel.RutBeneficiario,
                                rel.Beneficiario,
                                EnContrato = (db.ContratoActivo.Where(x => x.IdActivo == ac.IdActivo).Count() > 0) ? true : false,
                                ContratoActivo = db.ContratoActivo.Where(x => x.IdActivo == ac.IdActivo).FirstOrDefault(),
                                rel.ValorPrima,
                                ac.FecIngBaja,
                                ac.FechaBaja
                            }).AsEnumerable().ToList();
            var listaRetorno = (from reg in registro
                                select new
                                {
                                    reg.IdPolizaActivo,
                                    reg.IdActivo,
                                    reg.RazonSocial,
                                    reg.NumeroInterno,
                                    reg.CodSoftland,
                                    reg.Familia,
                                    reg.NombreCuenta,
                                    reg.Descripcion,
                                    reg.Marca,
                                    reg.Modelo,
                                    reg.Motor,
                                    reg.Chasis,
                                    reg.Anio,
                                    reg.Grupo,
                                    reg.SubGrupo,
                                    reg.Valor,
                                    reg.NombreProveedor,
                                    reg.NumeroFactura,
                                    reg.Patente,
                                    reg.Glosa,
                                    reg.FechaRegistroStr,
                                    reg.FechaBajaStr,
                                    reg.NombreEstado,
                                    reg.NumeroLeasing,
                                    reg.RutBeneficiario,
                                    reg.Beneficiario,
                                    reg.FecIngBaja,
                                    reg.FechaBaja,
                                    reg.EnContrato,
                                    Leasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.NumeroContrato : "") : "",
                                    TerminoLeasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.FechaTermino : (DateTime?)null) : (DateTime?)null,
                                    Banco = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.Banco.NombreBanco : "") : "",
                                    DescripcionLeasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.Descripcion : "") : "",
                                    TipoPropiedad = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdEstado == (int)Helper.Estado.ConActivo) ? "Leasing" : "Propio") : "Propio",
                                    reg.ValorPrima
                                    //(Propio (parte siendo propio) o leasing vigente (al finalizar leasing pasa a ser propio))
                                }).ToList();
            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalAsociarActivo(int idPoliza)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var polizaActivo = new PolizaActivo();
                polizaActivo.IdPoliza = idPoliza;

                return PartialView(polizaActivo);
            }
        }

        public ActionResult ListaActivoAsociar_Read(int idPoliza, string numeroActivo, string codigoActivo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var idEmpresa = 0;
            var poliza = db.Poliza.Find(idPoliza);
            if (poliza != null)
            {
                idEmpresa = poliza.IdEmpresa;
            }

            var activoPoliza = db.PolizaActivo.Where(c => c.IdPoliza == idPoliza).AsEnumerable().ToList();
            if (activoPoliza.Count() == 0)
            {
                activoPoliza = new List<PolizaActivo>();
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
                                && activoPoliza.Where(x => x.IdActivo == ac.IdActivo).Count() == 0
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

        public ActionResult AsociarActivo(int idPoliza, int[] activos)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro", "RegistrarSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
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
                                    addActivo.FechaRegistro=DateTime.Now;
                                    db.PolizaActivo.Add(addActivo);
                                    db.SaveChanges();

                                    /*cambio el estado del activo*/
                                    /*existeActivo.IdEstado = (int)Helper.Estado.ActEnPoliza;
                                    db.SaveChanges();*/
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Editar);
            if (acceso.AccesoValido == false || tieneEditar.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }

            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbPolizaActivo = db.PolizaActivo.Find(idPolizaActivo);
                    var sinistros = db.Siniestro.Where(c => c.IdPolizaActivo == idPolizaActivo).ToList();
                    if (sinistros.Count() > 0)
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 100, Mensaje = "Activo cuenta con Siniestro asociado" };                        
                    }
                    else
                    {
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
                    
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                }

                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ModalEditarBeneficiario(int idActivoPoliza)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var polizaActivo = db.PolizaActivo.Find(idActivoPoliza);
                return PartialView(polizaActivo);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult EditarBeneficiario(PolizaActivo dato)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Editar);
            if (acceso.AccesoValido == false || tieneEditar.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
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

                            var p = db.PolizaActivo.Find(dato.IdPolizaActivo);
                            mensaje = "Beneficiario póliza de seguro actualizado con éxito";
                            var empresa = db.Empresa.Find(p.Poliza.IdEmpresa);
                            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
                            var uxiliar = dbSoft.cwtauxi.Find(dato.Beneficiario);
                            if (p != null) {

                                //validar datos
                                dato.Beneficiario = validarDatos.ValidaStr(dato.Beneficiario);

                                p.RutBeneficiario = uxiliar.RutAux;
                                p.Beneficiario = uxiliar.NomAux;
                                p.CodAux=dato.Beneficiario;
                                p.PaginaInicial = dato.PaginaInicial;
                                p.PaginaTermino = dato.PaginaTermino;
                                p.ValorPrima = dato.ValorPrima;
                                p.NumeroEndoso = dato.NumeroEndoso;
                                p.FechaEndoso = dato.FechaEndoso;
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                            }
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

        #endregion

        #region Documento Poliza
        public ActionResult AddDocumento(int idPoliza)
        {
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro" }, Helper.TipoAcceso.Crear);
            if (tieneCrear.AccesoValido == false)

            {
                return RedirectToAction(tieneCrear.Vista, tieneCrear.Controlador);
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }

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

        #region Buscar Seguro

        public ActionResult SeguroBuscar()
        {
            
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
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
        public JsonResult Auxiliares_Read(string q)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var activos = PolizasActivas(null, null, null, "", "", "");
            var auxiliares = activos.Where(c=>c.RutBeneficiario!=null).GroupBy(c => new { c.RutBeneficiario, c.Beneficiario }).Select(c => new { c.Key.RutBeneficiario, c.Key.Beneficiario }).ToList();
            var listaProd = (from aux in auxiliares
                             where (aux.RutBeneficiario.Replace(".", "").Replace("-", "").Contains(q.Replace(".", "").Replace("-", "")) || aux.Beneficiario.Contains(q))
                             select new
                             {
                                 id = aux.RutBeneficiario,
                                 text = aux.RutBeneficiario + " : " + aux.Beneficiario
                             }).ToList();

            return Json(listaProd, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportarTxtBanco(string RutBeneficiario)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            FuncionesGeneralesController funcion = new FuncionesGeneralesController();

            var activos = PolizasActivas(null, null, null, "", RutBeneficiario,"");            
            var delimeter = ";";
            var lineEndDelimeter = "\n";
            StringBuilder sb = new StringBuilder();
            string Columns = string.Empty;
            
            foreach (var act in activos)
            {
                //OPER.LEASING,	ITEM,	RUT BENEFICIARIO, N° DE EQUIPO, FAMILIA DESCRIPCIÓN MARCA, MODELO, N° MOTOR,	N° CHASIS,	AÑO FABRICACIÓN, No PAG.INC.,	No PAG. TERM.,VALOR(CLP) //
                string row = string.Empty;
                row = act.Leasing + delimeter + act.NumeroInterno + delimeter + act.RutBeneficiario + delimeter + "nro equipo" + delimeter + act.Familia + delimeter + act.Descripcion + delimeter;
                row += act.Marca + delimeter + act.Modelo + delimeter + act.Motor + delimeter + act.Chasis + delimeter + act.Anio + delimeter + act.PaginaInicial + delimeter;
                row += act.PaginaTermino + delimeter + act.Valor + delimeter;

                // row += tmp.prog.NombreCuenta.ToString() + delimeter;               
                sb.Append(row.Remove(row.Length - 1, 1) + lineEndDelimeter);
            }
            RutBeneficiario = RutBeneficiario.Replace(".", "").Replace("-", "");
            var nombreArchivo = "TXT " + RutBeneficiario.Trim();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentType = "application/Text";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + nombreArchivo + ".txt;");
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public List<PolizaActivoViewModel> PolizasActivas(int? idEmpresa, int? idEmpresaAseguradora, int? idTipoSeguro, string numeroPoliza,string RutBeneficiario,string numeroActivo)
        {
            RutBeneficiario = (RutBeneficiario!=null)?RutBeneficiario.Replace(".", "").Replace("-", ""):"";
            var registro = (from p in db.Poliza.ToList()
                            join e in db.Estado.ToList() on p.IdEstado equals e.IdEstado
                            join rel in db.PolizaActivo.ToList() on p.IdPoliza equals rel.IdPoliza
                            join a in db.Activo.ToList() on rel.IdActivo equals a.IdActivo
                            join s in db.Siniestro on rel.IdPolizaActivo equals s.IdPolizaActivo into sw
                            from sv in sw.DefaultIfEmpty()
                            where p.IdEmpresa == ((idEmpresa != null) ? idEmpresa : p.IdEmpresa)
                            && p.IdEmpresaAseguradora == ((idEmpresaAseguradora != null) ? idEmpresaAseguradora : p.IdEmpresaAseguradora)
                            && p.IdTipoSeguro == ((idTipoSeguro != null) ? idTipoSeguro : p.IdTipoSeguro)
                            && p.NumeroPoliza == ((numeroPoliza != "") ? numeroPoliza : p.NumeroPoliza)
                            && a.NumeroInterno == ((numeroActivo != "" && numeroActivo != null) ? numeroActivo : a.NumeroInterno)
                            && ((RutBeneficiario != "" && RutBeneficiario != null)? ((rel.RutBeneficiario != null) ? rel.RutBeneficiario : "").Replace(".", "").Replace("-", "")== RutBeneficiario : true)
                            select new
                            {
                                p.IdPoliza,
                                IdSiniestro = (sv != null) ? sv.IdSiniestro : 0,
                                p.NumeroPoliza,
                                NombreTipoSeguro = (p.TipoSeguro.NombreTipoSeguro != null) ? p.TipoSeguro.NombreTipoSeguro : string.Empty,
                                NombreEmpresaAseguradora = (p.EmpresaAseguradora.NombreEmpresaAseguradora != null) ? p.EmpresaAseguradora.NombreEmpresaAseguradora : string.Empty,
                                p.MontoAsegurado,
                                p.PrimaMensual,
                                p.NumeroPagos,
                                NombreTipoMoneda = (p.TipoMoneda.NombreTipoMoneda != null) ? p.TipoMoneda.NombreTipoMoneda : string.Empty,
                                p.FechaVencimiento,
                                FechaVencimientoStr = p.FechaVencimiento.ToString("dd-MM-yyyy"),
                                rel.Beneficiario,
                                rel.RutBeneficiario,
                                p.FechaEnvioBanco,
                                FechaEnvioBancoStr = p.FechaEnvioBanco.ToString("dd-MM-yyyy"),
                                p.Empresa.RazonSocial,
                                rel.IdPolizaActivo,
                                a.NumeroInterno,
                                Familia = (a.Familia != null) ? a.Familia.NombreFamilia : string.Empty,
                                NombreEstadoActivo = (a.Estado.NombreEstado != null) ? a.Estado.NombreEstado : string.Empty,
                                a.Descripcion,
                                a.Patente,
                                a.Marca,
                                a.Modelo,
                                a.Motor,
                                a.Chasis,
                                a.Anio,
                                Grupo = a.DesGrupo,
                                SubGrupo = a.DesSGru,
                                a.Valor,
                                FechaRegistroActivo = a.FechaRegistro,
                                FechaRegistroActivoStr = "",
                                a.Glosa,
                                a.FechaBaja,
                                a.FecIngBaja,
                                TieneSiniestro = (db.Siniestro.Where(x => x.IdPolizaActivo == rel.IdPolizaActivo).Count() > 0) ? "SI" : "NO",
                                EnContrato = (db.ContratoActivo.Where(x => x.IdActivo == a.IdActivo).Count() > 0) ? true : false,
                                ContratoActivo = db.ContratoActivo.Where(x => x.IdActivo == a.IdActivo).FirstOrDefault(),
                                rel.ValorPrima,
                                rel.PaginaInicial,
                                rel.PaginaTermino,
                                rel.NumeroEndoso,
                                rel.FechaEndoso
                            }).AsEnumerable().ToList();
            var listaRetorno = (from reg in registro
                                select new PolizaActivoViewModel
                                {
                                    IdPoliza=reg.IdPoliza,
                                    IdSiniestro=reg.IdSiniestro,
                                    NumeroPoliza=reg.NumeroPoliza,
                                    NombreTipoSeguro=reg.NombreTipoSeguro,
                                    NombreEmpresaAseguradora=reg.NombreEmpresaAseguradora,
                                    MontoAsegurado=reg.MontoAsegurado,
                                    PrimaMensual=reg.PrimaMensual,
                                    NumeroPagos=reg.NumeroPagos,
                                    NombreTipoMoneda=reg.NombreTipoMoneda,
                                    FechaVencimiento=reg.FechaVencimiento,
                                    FechaVencimientoStr=reg.FechaVencimientoStr,
                                    Beneficiario=reg.Beneficiario,
                                    RutBeneficiario=reg.RutBeneficiario,
                                    FechaEnvioBanco=reg.FechaEnvioBanco,
                                    FechaEnvioBancoStr=reg.FechaEnvioBancoStr,
                                    RazonSocial=reg.RazonSocial,
                                    IdPolizaActivo=reg.IdPolizaActivo,
                                    NumeroInterno=reg.NumeroInterno,
                                    Familia=reg.Familia,
                                    NombreEstadoActivo=reg.NombreEstadoActivo,
                                    Descripcion=reg.Descripcion,
                                    Patente=reg.Patente,
                                    Marca=reg.Marca,
                                    Modelo=reg.Modelo,
                                    Motor=reg.Motor,
                                    Chasis=reg.Chasis,
                                    Anio=reg.Anio,
                                    Grupo=reg.Grupo,
                                    SubGrupo=reg.SubGrupo,
                                    Valor=reg.Valor,
                                    FechaRegistroActivo=reg.FechaRegistroActivo,
                                    FechaRegistroActivoStr=reg.FechaRegistroActivoStr,
                                    Glosa=reg.Glosa,
                                    FechaBaja=reg.FechaBaja,
                                    FecIngBaja=reg.FecIngBaja,
                                    TieneSiniestro=reg.TieneSiniestro,
                                    ValorPrima=reg.ValorPrima,
                                    PaginaInicial=reg.PaginaInicial,
                                    PaginaTermino=reg.PaginaTermino,
                                    EnContrato =reg.EnContrato,
                                    Leasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.NumeroContrato : "") : "",
                                    Banco = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.Banco.NombreBanco : "") : "",
                                    DescripcionLeasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.Descripcion : "") : "",
                                    TipoPropiedad = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdEstado == (int)Helper.Estado.ConActivo) ? "Leasing" : "Propio") : "Propio",
                                    TerminoLeasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.FechaTermino : (DateTime?)null) : (DateTime?)null,
                                    NumeroEndoso=reg.NumeroEndoso,
                                    FechaEndoso=reg.FechaEndoso
                                    //(Propio (parte siendo propio) o leasing vigente (al finalizar leasing pasa a ser propio))
                                }).ToList();
            return listaRetorno;
        }
        public ActionResult ListaSeguroBuscar_Read(int? idEmpresa, int? idEmpresaAseguradora, int? idTipoSeguro, string numeroPoliza, string numeroActivo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var listaRetorno = PolizasActivas(idEmpresa, idEmpresaAseguradora, idTipoSeguro, numeroPoliza,"", numeroActivo);
            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalVistaSeguro(int idPoliza)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var contrato = new ContratoViewModel();
                var poliza = (from c in db.Poliza
                                where c.IdPoliza == idPoliza
                                select new PolizaViewModel
                                {
                                    IdPoliza = c.IdPoliza,
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

        public ActionResult VistaSeguro(int idPoliza)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
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
                                    FechaEnvioBanco = p.FechaEnvioBanco,
                                    FechaEnvioBancoStr = p.FechaEnvioBanco.ToString("dd-MM-yyyy"),
                                    IdEstado = p.IdEstado,
                                    ExistePoliza = "S"
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

                return PartialView(registro);
            }
        }

        public ActionResult VistaDocumentoSeguro(int idPoliza)
        {
            
            var acceso = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro", "SeguroBuscar" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {


                var registro = new PolizaDocumento();
                registro.IdPoliza = idPoliza;

                return PartialView(registro);
            }
        }
        #endregion

        #region Siniestro
        public ActionResult ModalAgregarSiniestro(int idPolizaActivo,int idSiniestro)
        {
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Crear);

            if (tieneCrear.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var siniestro = (from s in db.Siniestro
                                 where s.IdSiniestro == idSiniestro
                                 select new SiniestroViewModel
                                 {
                                     IdSiniestro = s.IdSiniestro,
                                     IdPolizaActivo = s.IdPolizaActivo,
                                     ExisteSiniestro = "S"
                                 }).FirstOrDefault();

                if (siniestro == null) {
                    siniestro = new SiniestroViewModel();
                    siniestro.IdSiniestro = 0;
                    siniestro.IdPolizaActivo = idPolizaActivo;
                    siniestro.ExisteSiniestro = "N";
                }

                return View(siniestro);
            }
        }

        public ActionResult AddSiniestro(int IdPolizaActivo,int idSiniestro)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Acceder);
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar" }, Helper.TipoAcceso.Crear);
            if (tieneCrear.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var siniestro = (from s in db.Siniestro.ToList()
                                 where s.IdSiniestro == idSiniestro
                                 select new SiniestroViewModel
                                 {
                                     IdSiniestro = s.IdSiniestro,
                                     IdPolizaActivo = s.IdPolizaActivo,
                                     NumeroLiquidacion = s.NumeroLiquidacion,
                                     MontoLiquidacion = s.MontoLiquidacion,
                                     FechaDeclaracion = s.FechaDeclaracion,
                                     NumeroSiniestro = s.NumeroSiniestro,
                                     FechaSiniestro = s.FechaSiniestro,
                                     DetalleSiniestro = s.DetalleSiniestro,
                                     //IdPerdidaReclamada = s.IdPerdidaReclamada,
                                     MontoReclamado = s.MontoReclamado,
                                     IdPerdidaDeterminada = s.IdPerdidaDeterminada,
                                     IdEstado = s.IdEstado,
                                     Liquidador = s.Liquidador,
                                     Infraseguro = s.Infraseguro,
                                     Deducible = s.Deducible,
                                     Indemnizacion = s.Indemnizacion,
                                     ExisteSiniestro = "S",
                                     TituloBoton = "Actualizar Siniestro"
                                 }).FirstOrDefault();

                if (siniestro == null)
                {
                    siniestro = new SiniestroViewModel();
                    siniestro.IdSiniestro = 0;
                    siniestro.IdPolizaActivo = IdPolizaActivo;
                    siniestro.MontoReclamado = null;
                    siniestro.IdPerdidaReclamada = 0;
                    siniestro.IdPerdidaDeterminada = 0;
                    siniestro.IdEstado = 0;
                    siniestro.MontoLiquidacion = null;
                    siniestro.Deducible = null;
                    siniestro.Indemnizacion = null;
                    siniestro.FechaDeclaracion = null;
                    siniestro.FechaSiniestro = null;
                    siniestro.TituloBoton = "Grabar Siniestro";
                }

                var activo = (from p in db.PolizaActivo
                              join a in db.Activo on p.IdActivo equals a.IdActivo
                              where p.IdPolizaActivo == siniestro.IdPolizaActivo
                              select new RetornoGenerico { Id = p.IdPolizaActivo, Nombre = "Activo " + a.NumeroInterno.ToString() }).OrderBy(c => c.Id).ToList();
                SelectList listaActivo = new SelectList(activo.OrderBy(c => c.Nombre), "Id", "Nombre", siniestro.IdPolizaActivo);
                ViewData["listaActivo"] = listaActivo;

                var reclamado = (from e in db.TipoPerdida
                                 where e.Activo == true
                                 select new RetornoGenerico { Id = e.IdTipoPerdida, Nombre = e.NombreTipoPerdida }).OrderBy(c => c.Id).ToList();
                SelectList listaReclamado = new SelectList(reclamado.OrderBy(c => c.Nombre), "Id", "Nombre", siniestro.IdPerdidaReclamada);
                ViewData["listaReclamado"] = listaReclamado;

                var determinado = (from e in db.TipoPerdida
                                   where e.Activo == true
                                   select new RetornoGenerico { Id = e.IdTipoPerdida, Nombre = e.NombreTipoPerdida }).OrderBy(c => c.Id).ToList();
                SelectList listaDeterminado = new SelectList(determinado.OrderBy(c => c.Nombre), "Id", "Nombre", siniestro.IdPerdidaDeterminada);
                ViewData["listaDeterminado"] = listaDeterminado;

                var estado = (from e in db.Estado
                              where e.IdTipoEstado == (int)Helper.TipoEstado.Siniestro
                              select new RetornoGenerico { Id = e.IdEstado, Nombre = e.NombreEstado }).OrderBy(c => c.Id).ToList();
                SelectList listaEstado = new SelectList(estado.OrderBy(c => c.Nombre), "Id", "Nombre", siniestro.IdEstado);
                ViewData["listaEstado"] = listaEstado;

                return View(siniestro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarSiniestro(Siniestro dato)
        {
            
            var acceso = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar", "RegistrarSeguro" }, Helper.TipoAcceso.Acceder);
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar", "RegistrarSeguro" }, Helper.TipoAcceso.Crear);
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "SeguroBuscar", "RegistrarSeguro" }, Helper.TipoAcceso.Editar);
            if (tieneCrear.AccesoValido == false || tieneEditar.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
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
                    dato.DetalleSiniestro = validarDatos.ValidaStr(dato.DetalleSiniestro);
                    dato.Liquidador = validarDatos.ValidaStr(dato.Liquidador);
                    dato.NumeroSiniestro = validarDatos.ValidaStr(dato.NumeroSiniestro);
                    dato.Infraseguro = validarDatos.ValidaStr(dato.Infraseguro);
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";
                            var idSiniestro = 0;
                            var idPolizaActivo = 0;
                            var swTran = true;

                            var siniestro = db.Siniestro.Find(dato.IdSiniestro);


                            if (siniestro != null)
                            {
                                mensaje = "Siniestro actualizado con éxito";
                                var existeSiniestro = db.Siniestro.Where(c => c.NumeroSiniestro == dato.NumeroSiniestro && c.IdSiniestro != siniestro.IdSiniestro).FirstOrDefault();
                                if (existeSiniestro == null)
                                {
                                    idSiniestro = dato.IdSiniestro;
                                    idPolizaActivo = dato.IdPolizaActivo;

                                    siniestro.NumeroLiquidacion = dato.NumeroLiquidacion;
                                    siniestro.MontoLiquidacion = dato.MontoLiquidacion;
                                    siniestro.FechaDeclaracion = dato.FechaDeclaracion;
                                    siniestro.NumeroSiniestro = dato.NumeroSiniestro;
                                    siniestro.FechaSiniestro = dato.FechaSiniestro;
                                    siniestro.DetalleSiniestro = dato.DetalleSiniestro;
                                    siniestro.MontoReclamado = dato.MontoReclamado;
                                    //siniestro.IdPerdidaReclamada = dato.IdPerdidaReclamada;
                                    siniestro.IdPerdidaDeterminada = dato.IdPerdidaDeterminada;
                                    siniestro.IdEstado = dato.IdEstado;
                                    siniestro.Liquidador = dato.Liquidador;
                                    siniestro.Infraseguro = dato.Infraseguro;
                                    siniestro.Deducible = dato.Deducible;
                                    siniestro.Indemnizacion = dato.Indemnizacion;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    swTran = false;
                                    mensaje = "Numero Siniestro ya existe, revise sus datos";
                                }
                            }
                            else
                            {
                                mensaje = "Poliza de seguro creada con éxito";

                                var existeSiniestro = db.Siniestro.Where(c => c.NumeroSiniestro == dato.NumeroSiniestro).FirstOrDefault();
                                if (existeSiniestro == null)
                                {
                                    var addSiniestro = new Siniestro();
                                    addSiniestro.IdPolizaActivo = dato.IdPolizaActivo;
                                    addSiniestro.NumeroLiquidacion = dato.NumeroLiquidacion;
                                    addSiniestro.MontoLiquidacion = dato.MontoLiquidacion;
                                    addSiniestro.FechaDeclaracion = dato.FechaDeclaracion;
                                    addSiniestro.NumeroSiniestro = dato.NumeroSiniestro;
                                    addSiniestro.FechaSiniestro = dato.FechaSiniestro;
                                    addSiniestro.DetalleSiniestro = dato.DetalleSiniestro;
                                    addSiniestro.MontoReclamado = dato.MontoReclamado;
                                    //addSiniestro.IdPerdidaReclamada = dato.IdPerdidaReclamada;
                                    addSiniestro.IdPerdidaDeterminada = dato.IdPerdidaDeterminada;
                                    addSiniestro.IdEstado = dato.IdEstado;
                                    addSiniestro.Liquidador = dato.Liquidador;
                                    addSiniestro.Infraseguro = dato.Infraseguro;
                                    addSiniestro.Deducible = dato.Deducible;
                                    addSiniestro.Indemnizacion = dato.Indemnizacion;
                                    addSiniestro.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    addSiniestro.FechaRegistro = DateTime.Now;
                                    db.Siniestro.Add(addSiniestro);
                                    db.SaveChanges();

                                    idSiniestro = addSiniestro.IdSiniestro;
                                    idPolizaActivo = addSiniestro.IdPolizaActivo;
                                }
                                else
                                {
                                    swTran = false;
                                    mensaje = "Numero Siniestro ya existe, revise sus datos";
                                }
                            }
                            /*solo ejecuta la transaccion si todo esta correcto*/
                            if (swTran)
                            {
                                dbContextTransaction.Commit();
                                showMessageString = new { Estado = 0, Mensaje = mensaje, idSiniestro = idSiniestro, idPolizaActivo = idPolizaActivo };
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
        public JsonResult DeleteSiniestro(int idSiniestro)
        {
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "NuevosProyectos" }, Helper.TipoAcceso.Editar);

            if (tieneEditar.AccesoValido == false)

            {
                return Json(new { tieneEditar.Estado, tieneEditar.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbSiniestro = db.Siniestro.Find(idSiniestro);

                    if (dbSiniestro != null)
                    {

                        var dbArchivo = (from doc in db.SiniestroDocumento
                                         where doc.IdSiniestro == dbSiniestro.IdSiniestro
                                         select new
                                         {
                                             doc.IdSiniestroDocumento,
                                             doc.UrlDocumento
                                         }).ToList();
                        foreach (var arc in dbArchivo)
                        {
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM SiniestroDocumento WHERE IdSiniestroDocumento = {0}", arc.IdSiniestroDocumento);
                            db.SaveChanges();
                        }


                        db.Siniestro.Remove(dbSiniestro);
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

        #endregion

        #region Documento Siniestro
        public ActionResult AddDocumentoSiniestro(int idSiniestro)
        {
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "RegistrarSeguro" }, Helper.TipoAcceso.Crear);
            if (tieneCrear.AccesoValido == false)

            {
                return RedirectToAction(tieneCrear.Vista, tieneCrear.Controlador);
            }

            else
            {

                var tipoDocumento = (from e in db.TipoDocumento
                                     where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.Siniestro
                                     select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;

                var registro = new SiniestroDocumento();
                registro.IdSiniestro = idSiniestro;

                return View(registro);
            }
        }

        public ActionResult ListaSiniestroDocumento_Read(int idSiniestro)
        {
            var registro = (from c in db.SiniestroDocumento
                            join td in db.TipoDocumento on c.IdTipoDocumento equals td.IdTipoDocumento
                            where c.IdSiniestro == idSiniestro
                            select new SiniestroDocumentoViewModel
                            {
                                IdSiniestroDocumento = c.IdSiniestroDocumento,
                                IdSiniestro = c.IdSiniestro,
                                NombreTipoDocumento = td.NombreTipoDocumento,
                                NombreOriginal = c.NombreOriginal,
                                UrlDocumento = c.UrlDocumento
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarSiniestroDocumento(SiniestroDocumento dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            SiniestroDocumento addDocumento = new SiniestroDocumento();
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
                                var siniestro = (from s in db.Siniestro
                                                 join a in db.PolizaActivo on s.IdPolizaActivo equals a.IdPolizaActivo
                                                 where s.IdSiniestro == dato.IdSiniestro
                                                 select new {
                                                     a.IdPoliza,
                                                     s.IdSiniestro
                                                 }).FirstOrDefault();
                                var pathDocumento = "";
                                var fileName = dato.IdTipoDocumento.ToString() + '_' + Path.GetFileName(archivo.FileName);
                                var carpeta = "Poliza/"+siniestro.IdPoliza.ToString()+"/Siniestro";
                                var pathData = "~/App_Data";
                                var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta);
                                if (!Directory.Exists(pathCarpeta))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                }
                                string carpetaSolicitud = siniestro.IdSiniestro.ToString();
                                var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                if (!Directory.Exists(pathCarpetaSolicitud))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                }
                                pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                archivo.SaveAs(physicalPath);

                                addDocumento.IdSiniestro = (int)siniestro.IdSiniestro;
                                addDocumento.IdTipoDocumento = dato.IdTipoDocumento;
                                addDocumento.FechaRegistro = DateTime.Now;
                                addDocumento.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addDocumento.NombreOriginal = fileName;
                                addDocumento.UrlDocumento = pathDocumento;
                                db.SiniestroDocumento.Add(addDocumento);
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
        public JsonResult DeleteSiniestroDocumento(int idSiniestroDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.SiniestroDocumento.Find(idSiniestroDocumento);

            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            db.Database.ExecuteSqlCommand("DELETE FROM SiniestroDocumento WHERE IdSiniestroDocumento = {0}", idSiniestroDocumento);
            db.SaveChanges();


            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        #endregion
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

                return View();
            }
        }
        public ActionResult ImportActivos(int? IdPoliza)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
            else
            {
                var contrato = db.Poliza.Find(IdPoliza);
                return View(contrato);
            }
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportaPlanillaActivos(int IdPoliza, HttpPostedFileBase archivo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ListaSeguro" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            List<string> data = new List<string>();
            if (archivo != null)
            {
                var poliza = db.Poliza.Find(IdPoliza);
                var activosEmpresa = db.Activo.Where(c => c.IdEmpresa == poliza.IdEmpresa).ToList();
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                if (archivo.ContentType == "application/vnd.ms-excel" || archivo.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = archivo.FileName;
                    string targetpath = Server.MapPath("~/App_Data/");
                    archivo.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();
                    adapter.Fill(ds, "ExcelTable");
                    DataTable dtable = ds.Tables["ExcelTable"];
                    string sheetName = "Sheet1";
                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<ActivoViewModel>(sheetName) select a;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.NumeroInterno !=null && a.NumeroInterno!="")
                            {
                                var nroInnn = a.NumeroInterno.TrimStart('0');
                                var activo = activosEmpresa.Where(c => ((c.NumeroInterno!=null && c.NumeroInterno!="")?c.NumeroInterno.TrimStart('0'):"") == a.NumeroInterno.TrimStart('0') && c.IdEmpresa==poliza.IdEmpresa).FirstOrDefault();
                                if (activo != null)
                                {
                                    var empresa = db.Empresa.Find(poliza.IdEmpresa);
                                    SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
                                    var auxiliar = dbSoft.cwtauxi.Where(c=>c.RutAux.Replace(".", "").Replace("-", "").ToUpper() == a.RutBeneficiario.Replace(".","").Replace("-","").ToUpper()).FirstOrDefault();
                                    if (auxiliar != null)
                                    {
                                        var activoP = db.PolizaActivo.Where(c => c.IdPoliza == IdPoliza && c.Activo.NumeroInterno == activo.NumeroInterno).FirstOrDefault();
                                        if (activoP != null)
                                        {
                                            activoP.RutBeneficiario = a.RutBeneficiario;
                                            activoP.PaginaInicial = a.PaginaInicial;
                                            activoP.PaginaTermino = a.PaginaTermino;
                                            activoP.ValorPrima = a.ValorPrima;
                                            activoP.RutBeneficiario = auxiliar.RutAux;
                                            activoP.Beneficiario = auxiliar.NomAux;
                                            activoP.CodAux = auxiliar.CodAux;
                                            activoP.NumeroEndoso = a.NumeroEndoso;
                                            activoP.FechaEndoso = a.FechaEndoso;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            PolizaActivo newActivoP = new PolizaActivo();
                                            newActivoP.IdPoliza = IdPoliza;
                                            newActivoP.RutBeneficiario = a.RutBeneficiario;
                                            newActivoP.PaginaInicial = a.PaginaInicial;
                                            newActivoP.PaginaTermino = a.PaginaTermino;
                                            newActivoP.ValorPrima = a.ValorPrima;
                                            newActivoP.RutBeneficiario = auxiliar.RutAux;
                                            newActivoP.Beneficiario = auxiliar.NomAux;
                                            newActivoP.CodAux = auxiliar.CodAux;
                                            newActivoP.IdActivo = activo.IdActivo;
                                            newActivoP.FechaRegistro = DateTime.Now;
                                            newActivoP.NumeroEndoso = a.NumeroEndoso;
                                            newActivoP.FechaEndoso = a.FechaEndoso;
                                            db.PolizaActivo.Add(newActivoP);
                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        data.Add("<ul>");
                                        data.Add("<li>Auxiliar no exitse</li>");
                                        data.Add("</ul>");
                                        data.ToArray();

                                    }
                                }
                                else
                                {
                                    data.Add("<ul>");
                                    data.Add("<li>Activo no existe</li>");
                                    data.Add("</ul>");
                                    data.ToArray();

                                }

                            }
                            else
                            {
                                data.Add("<ul>");
                                data.Add("<li> Nro es obligatorio</li>");
                                data.Add("</ul>");
                                data.ToArray();
                                showMessageString = new { Estado = 100, Mensaje = "Error en los registros" };
                            }
                            
                            var tabla = data;
                            showMessageString = new { Estado = 0, Mensaje = "Registros Importados Exitosamente" };
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                            showMessageString = new { Estado = 100, Mensaje = "Error en los registros" };
                        }
                    }
                    //deleting excel file from folder
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }

                }
                else
                {
                    //alert message for invalid file format
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();

                    showMessageString = new { Estado = 100, Mensaje = "Formato no permitido" };
                }
            }
            else
            {
                data.Add("<ul>");
                if (archivo == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                showMessageString = new { Estado = 100, Mensaje = "Please choose Excel file" };
            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }
    }
}