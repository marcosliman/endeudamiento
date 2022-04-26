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
                SelectList listaMes = new SelectList(mes.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaMes"] = listaMes;

                var contrato = new ContratoViewModel();
                contrato.FechaRegistro = DateTime.Now;
                contrato.FechaInicioStr = contrato.FechaRegistro.ToString("dd-MM-yyyy");

                return View(contrato);
            }
        }

        public ActionResult ListaContrato_Read(int? idTipoContrato, int? idEmpresa, int? anio, int? idMes)
        {
            var registro = (from c in db.Contrato.ToList()
                            where c.IdTipoContrato == ((idTipoContrato != null) ? idTipoContrato : c.IdTipoContrato)
                             && c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            select new ReporteContratoViewModel
                            {
                                IdContrato = c.IdContrato,
                                Empresa = c.Empresa.RazonSocial,
                                Acreedor = c.Banco.NombreBanco,
                                NumeroContrato = c.NumeroContrato,
                                Moneda = c.TipoMoneda.NombreTipoMoneda,
                                TasaMensual = c.TasaMensual,
                                TasaAnual = c.TasaAnual,
                                Total = 0,
                                Total1 = 0,
                                Total2 = 0,
                                Total3 = 0,
                                TotalGeneral = 0
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
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
                SelectList listaMes = new SelectList(mes.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaMes"] = listaMes;

                var contrato = new ContratoViewModel();
                contrato.FechaRegistro = DateTime.Now;
                contrato.FechaInicioStr = contrato.FechaRegistro.ToString("dd-MM-yyyy");

                return View(contrato);
            }
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