using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.Globalization;
namespace tesoreria.Controllers
{
    public class AmortizacionController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato
        public ActionResult AmortizacionLeasing()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("AmortizacionLeasing", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult OtrosCreditos()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("OtrosCreditos", Helper.TipoAcceso.Acceder))
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

                var mes = (from m in db.Mes
                           select new RetornoGenerico { Id = m.IdMes, Nombre = m.NombreMes }).OrderBy(c => c.Id).ToList();
                SelectList listaMes = new SelectList(mes.OrderBy(c => c.Id), "Id", "Nombre", DateTime.Now.Month);
                ViewData["listaMes"] = listaMes;

                var contrato = new ContratoViewModel();
                contrato.FechaRegistro = DateTime.Now;
                contrato.FechaInicioStr = contrato.FechaRegistro.ToString("dd-MM-yyyy");

                return View(contrato);
            }
        }

        public ActionResult ListaContrato_Read(int? idTipoContrato, int? IdEmpresa, int? Anio, int? IdMes)
        {
            var inicioMes="01-"+IdMes.ToString()+"-"+Anio.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            IdEmpresa = (IdEmpresa == null) ? 0 : IdEmpresa;
            var contratos = db.Database.SqlQuery<ReporteContratoViewModel>(
                   "SP_AMORTIZACION_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3}", fechaInicio, fechaFin, idTipoContrato, IdEmpresa).ToList();
                        
            return Json(contratos, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Leasing()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Leasing", Helper.TipoAcceso.Acceder))
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

                var mes = (from m in db.Mes
                           select new RetornoGenerico { Id = m.IdMes, Nombre = m.NombreMes }).OrderBy(c => c.Id).ToList();
                SelectList listaMes = new SelectList(mes.OrderBy(c => c.Id), "Id", "Nombre",DateTime.Now.Month);
                ViewData["listaMes"] = listaMes;

                var contrato = new ContratoViewModel();
                contrato.FechaRegistro = DateTime.Now;
                contrato.FechaInicioStr = contrato.FechaRegistro.ToString("dd-MM-yyyy");

                return View(contrato);
            }
        }
        public ActionResult AmortizacionContrato(int IdContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            //else if (seguridad != null && !seguridad.TienePermiso("Leasing", Helper.TipoAcceso.Acceder))
            //{
            //    return RedirectToAction("Inicio", "Home");
            //}
            else
            {
                var contrato = db.Contrato.Find(IdContrato);
                return View(contrato);
            }
        }
        public ActionResult AsociarCpbteEgreso(int IdContratoDetAmortizacion)
        {
            var detalleCuota = db.Contrato_DetAmortizacion.Find(IdContratoDetAmortizacion);
            var fecha = detalleCuota.FechaPago.ToString("dd-MM-yyyy");
            ViewBag.FechaPago = fecha;
            var comprobantes = db.ComprobanteDetAmortizacion.Where(c => c.IdContratoDetAmortizacion == IdContratoDetAmortizacion).ToList();
            ViewData["listaComprobantes"] = comprobantes;
            var inicioMes = "01-" + detalleCuota.FechaPago.Month.ToString() + "-" + detalleCuota.FechaPago.Year.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            ViewBag.fechaInicio = fechaInicio.ToString("dd/MM/yyyy");
            ViewBag.fechaFin = fechaFin.ToString("dd/MM/yyyy");
            return View(detalleCuota);
        }
        public JsonResult ComprobantesEgrBusqueda_Read(int IdContratoDetAmortizacion, string AnoCpbte, string CpbNum, string rangoFecha, string busGlosa,string CpbTip,string MontoCpbte,double? MovNumDocRef)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;
            if (rangoFecha != "")
            {
                var fechas = rangoFecha.Split('-');
                fechaInicio = Convert.ToDateTime(fechas[0]);
                fechaFin = Convert.ToDateTime(fechas[1]);
            }
            if(CpbNum!=null && CpbNum != "")
            {
                CpbNum = CpbNum.PadLeft(8, '0');
            }
            var MontoCpbteDouble = (MontoCpbte != "" && MontoCpbte!=null) ? Double.Parse(MontoCpbte) : (double?)null;
            var detalleCuota = db.Contrato_DetAmortizacion.Find(IdContratoDetAmortizacion);
            var contrato = detalleCuota.Contrato_Amortizacion.Contrato;
            var empresa = db.Empresa.Find(contrato.IdEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);

            var movFiltro= (from mov in dbSoft.cwmovim
                              join cpb in dbSoft.cwcpbte on new { mov.CpbAno, mov.CpbNum } equals new { cpb.CpbAno, cpb.CpbNum }
                              where cpb.CpbNum != "00000000" && cpb.CpbAno == AnoCpbte && cpb.CpbEst == "V"
                              && ((MovNumDocRef != null) ? mov.MovNumDocRef == MovNumDocRef : true)
                              && ((CpbNum == "") ? true : cpb.CpbNum.Contains(CpbNum))
                              && ((busGlosa == "") ? true : cpb.CpbGlo.Contains(busGlosa))
                              && ((CpbTip != "" && CpbTip != null) ? cpb.CpbTip == CpbTip : true)
                              && ((fechaInicio != null) ? cpb.CpbFec >= fechaInicio : true) &&
                                 ((fechaFin != null) ? cpb.CpbFec <= fechaFin : true)
                              select new { cpb.CpbNum, cpb.CpbAno } into x
                              group x by new { x.CpbNum, x.CpbAno } into g
                              select new
                              {
                                  g.Key.CpbNum,
                                  g.Key.CpbAno
                              }).ToList();

            var comprobantes = (from mov in dbSoft.cwmovim
                                join cpb in dbSoft.cwcpbte on new { mov.CpbAno, mov.CpbNum } equals new { cpb.CpbAno, cpb.CpbNum }
                                where cpb.CpbNum != "00000000" && cpb.CpbAno == AnoCpbte && cpb.CpbEst == "V"
                                && ((CpbNum == "") ? true : cpb.CpbNum.Contains(CpbNum))
                                && ((busGlosa == "") ? true : cpb.CpbGlo.Contains(busGlosa))
                                && ((CpbTip != "" && CpbTip!=null) ? cpb.CpbTip== CpbTip:true)
                                && ((fechaInicio != null) ? cpb.CpbFec >= fechaInicio : true) &&
                                   ((fechaFin != null) ? cpb.CpbFec <= fechaFin : true)
                                select new { cpb.CpbNum,cpb.CpbTip, cpb.CpbFec, cpb.CpbGlo, mov.MovHaber, cpb.CpbAno } into x
                                group x by new { x.CpbNum,x.CpbTip, x.CpbFec, x.CpbGlo, x.CpbAno } into g
                                select new
                                {
                                    g.Key.CpbNum,
                                    g.Key.CpbTip,
                                    g.Key.CpbFec,
                                    g.Key.CpbGlo,
                                    Monto = g.Sum(c => c.MovHaber),
                                    g.Key.CpbAno
                                }).ToList();
            var cpbteCuota = (from cd in db.ComprobanteDetAmortizacion
                              where cd.IdContratoDetAmortizacion == IdContratoDetAmortizacion
                              select new { cd.CpbNum, cd.CpbAno, cd.Monto }).ToList();

            var cpbteOtras = (from cd in db.ComprobanteDetAmortizacion
                              where cd.IdContratoDetAmortizacion != IdContratoDetAmortizacion
                              select new { cd.CpbNum, cd.CpbAno, cd.Monto }).ToList();
            var listaRetorno = (from c in comprobantes
                                join m in movFiltro on new { c.CpbNum,c.CpbAno} equals new { m.CpbNum,m.CpbAno}
                                where
                                //(c.Monto - cpbteOtras.Where(y => y.CpbNum == c.CpbNum && y.CpbAno == c.CpbAno).Sum(y => y.Monto)) > 0 &&
                                cpbteCuota.Where(y => y.CpbNum == c.CpbNum && y.CpbAno == c.CpbAno).Count() == 0
                                && ((MontoCpbteDouble != null)?c.Monto== MontoCpbteDouble:true)
                                select new
                                {
                                    c.CpbAno,
                                    c.CpbNum,
                                    c.CpbFec,
                                    c.CpbGlo,
                                    c.Monto,
                                    Tipo=((c.CpbTip=="E")?"Egreso":((c.CpbTip=="I")?"Ingreso":"Traspaso")),
                                    seleccionado = (cpbteCuota.Where(y => y.CpbNum == c.CpbNum && y.CpbAno == c.CpbAno).Count() > 0) ? true : false,
                                    ocupado = (cpbteOtras.Where(y => y.CpbNum == c.CpbNum && y.CpbAno == c.CpbAno).Count() > 0) ? true : false,
                                    BaseSoftland = empresa.BaseSoftland,
                                }).ToList();
            var json = Json(listaRetorno, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = 500000000;
            return json;
        }
        public JsonResult ComprobantesDetAmortizacion_Read(int IdContratoDetAmortizacion)
        {
            var listaRetorno = db.ComprobanteDetAmortizacion.Where(c => c.IdContratoDetAmortizacion == IdContratoDetAmortizacion).ToList();
            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AsociarComprobanteDet(string CpbNum, string CpbAno, int IdContratoDetAmortizacion)
        {
            dynamic showMessageString = string.Empty;
            try
            {
                var cuotaPago = db.Contrato_DetAmortizacion.Find(IdContratoDetAmortizacion);
                var contrato = cuotaPago.Contrato_Amortizacion.Contrato;
                var empresa = db.Empresa.Find(contrato.IdEmpresa);
                SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
                var comprobante = dbSoft.cwcpbte.Where(c => c.CpbNum == CpbNum && c.CpbAno == CpbAno).FirstOrDefault();
                var existeCpbteDetalle = db.ComprobanteDetAmortizacion.Where(c => c.CpbNum == CpbNum && c.CpbAno == CpbAno 
                && c.IdContratoDetAmortizacion == IdContratoDetAmortizacion && c.CpbTip== comprobante.CpbTip).FirstOrDefault();
                showMessageString = new
                {
                    Estado = 0,
                    Mensaje = "Comprobante asociado exitosamente"
                };
                if (existeCpbteDetalle != null)
                {
                    showMessageString = new
                    {
                        Estado = 100,
                        Mensaje = "Comprobante ya Asociado"
                    };                    
                }
                else
                {
                    var montoCpbte = dbSoft.cwmovim.Where(c => c.CpbAno == CpbAno && c.CpbNum == CpbNum).Sum(c => c.MovHaber);
                    /*if(movDetalle.Monto== montoCpbte)
                    {*/
                    var cpbteMOv = db.ComprobanteDetAmortizacion.Where(c => c.IdContratoDetAmortizacion == IdContratoDetAmortizacion && c.CpbTip== comprobante.CpbTip).ToList();
                    db.ComprobanteDetAmortizacion.RemoveRange(cpbteMOv);
                    db.SaveChanges();
                    ComprobanteDetAmortizacion regitro = new ComprobanteDetAmortizacion();
                    regitro.IdContratoDetAmortizacion = IdContratoDetAmortizacion;
                    regitro.FechaRegistro = DateTime.Now;
                    regitro.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                    regitro.CpbNum = CpbNum;
                    regitro.CpbAno = CpbAno;
                    regitro.CpbMes = comprobante.CpbMes;
                    regitro.CpbFec = comprobante.CpbFec;
                    regitro.Monto = (double)montoCpbte;
                    regitro.CpbGlo = comprobante.CpbGlo;
                    regitro.BaseSoftland=empresa.BaseSoftland;
                    regitro.EsCreado = false;
                    regitro.AsociadoManual = true;
                    regitro.CpbTip = comprobante.CpbTip;
                    db.ComprobanteDetAmortizacion.Add(regitro);
                    
                }
                db.SaveChanges();

            }
            catch (Exception e)
            {
                showMessageString = new
                {
                    Estado = 100,
                    Mensaje = "Error al Asociar el Cpbte. Contable"
                };
            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult QuitarCpbtePrograma(string CpbNum, string CpbAno, int IdContratoDetAmortizacion)
        {
            dynamic showMessageString = string.Empty;
            try
            {
                var existeCpbteDetalle = db.ComprobanteDetAmortizacion.Where(c => c.CpbNum == CpbNum && c.CpbAno == CpbAno && c.IdContratoDetAmortizacion == IdContratoDetAmortizacion).FirstOrDefault();
                if (existeCpbteDetalle != null)
                {
                    db.ComprobanteDetAmortizacion.Remove(existeCpbteDetalle);                    
                    db.SaveChanges();
                }
                showMessageString = new { Estado = 0, Mensaje = "Comprobante Eliminado exitosamente" };
            }
            catch
            {
                showMessageString = new { Estado = 100, Mensaje = "Error al quitar comprobante" };
            }
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModalContrato(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
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
                                    RazonSocial = c.Empresa.RazonSocial,
                                    IdBanco = c.IdBanco,
                                    NombreBanco = c.Banco.NombreBanco,
                                    NombreTipoFinanciamiento = c.TipoFinanciamiento.NombreTipoFinanciamiento,
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
                                    IdTipoMoneda = c.IdTipoMoneda,
                                    TituloBoton = (c.IdTipoContrato == (int)Helper.TipoContrato.Leasing) ? "Contrato Leasing Origen" : "Contrato origen"
                                }).FirstOrDefault();
                return View(registro);
            }
        }

        public ActionResult ListaDocumentoContrato_Read(int idContrato)
        {
            var registro = (from d in db.ContratoActivoDocumento
                            join t in db.TipoDocumento on d.IdTipoDocumento equals t.IdTipoDocumento
                            join c in db.ContratoActivo on d.IdContratoActivo equals c.IdContratoActivo
                            where c.IdContrato == idContrato
                            select new ContratoDocumentoViewModel
                            {
                                IdContratoActivoDocumento = d.IdContratoActivoDocumento,
                                NombreTipoDocumento = t.NombreTipoDocumento,
                                UrlDocumento = d.UrlDocumento,
                                NombreOriginal = d.NombreOriginal
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalAmortizacion(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var registro = (from c in db.Contrato.ToList()
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    TituloBoton = (c.IdTipoContrato == (int)Helper.TipoContrato.Leasing) ? "Detalle de Leasing" : "Detalle Contrato"
                                }).FirstOrDefault();
                return View(registro);
            }
        }

        public ActionResult ListaAmortizacion_Read(int idContrato)
        {
            var registro = (from ca in db.Contrato_Amortizacion.ToList()
                            join d in db.Contrato_DetAmortizacion.ToList() on ca.IdContratoAmortizacion equals d.IdContratoAmortizacion
                            where ca.IdContrato == idContrato
                            select new AmortizacionViewModel
                            {
                                Anio = 0,
                                NombreMes = (d.Mes != null) ? d.Mes.NombreMes : string.Empty,
                                CortoPlazo = d.CortoPlazo,
                                LargoPlazo = d.LargoPlazo,
                                FechaPago = d.FechaPago,
                                FechaPagoStr = d.FechaPago.ToString("dd-MM-yyyy"),
                                Periodo = d.Periodo,
                                Cuota = d.Cuota,
                                IvaDiferido = d.IvaDiferido,
                                Intereses = d.Intereses,
                                Amortizacion = d.Amortizacion,
                                Saldo_Insoluto = d.Saldo_Insoluto
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
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
        public ActionResult ModalContratoCreditoOrigen()
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
        public ActionResult ComprobanteEgreso()
        {
            return View();            
        }
        
    }
}