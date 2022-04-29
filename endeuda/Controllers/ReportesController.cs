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
    public class ReportesController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato
        public ActionResult ConsolidadoDeudaLeasing()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ConsolidadoDeudaLeasing", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ConsolidadoDeudaLeasing_Read()
        {
           
            var registro = (from Con in db.Contrato
                            join TCon in db.TipoContrato on Con.IdTipoContrato equals TCon.IdTipoContrato
                            join ConAct in db.ContratoActivo on Con.IdContrato equals ConAct.IdContrato
                            join Act in db.Activo on ConAct.IdActivo equals Act.IdActivo
                            join Fam in db.Familia on Act.IdFamilia equals Fam.IdFamilia into t_fam
                            from l_fam in t_fam.DefaultIfEmpty()
                            join Emp in db.Empresa on Con.IdEmpresa equals Emp.IdEmpresa
                            join Bco in db.Banco on Con.IdBanco equals Bco.IdBanco
                            join Est in db.Estado on Con.IdEstado equals Est.IdEstado
                            //where c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            //&& c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            //&& c.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : c.IdTipoFinanciamiento)
                            //&& c.NumeroContrato == ((numeroContrato != "") ? numeroContrato : c.NumeroContrato)
                            select new ConsolidadoLeasingViewModel
                            {
                                IdContrato = Con.IdContrato,
                                RazonSocial = Emp.RazonSocial,
                                NombreBanco = Bco.NombreBanco,
                                NumeroContrato = Con.NumeroContrato,
                                NombreFamilia = (l_fam != null) ? l_fam.NombreFamilia : "Familia No Asociada",
                                DescripcionActivo = Act.Descripcion,
                                Plazo = Con.Plazo,
                                FechaInicio = Con.FechaInicio,
                                FechaTermino = Con.FechaTermino,
                                Total = Math.Round(Con.Monto, 2),
                                TasaAnual = Con.TasaAnual,
                                PuedeEliminar = (Con.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                                NombreEstado = Est.NombreEstado
                            }).AsEnumerable().ToList();
            
            var listaretorno = registro.GroupBy(c => new { c.IdContrato, c.RazonSocial, c.NombreBanco, c.NumeroContrato, c.NombreFamilia,c.DescripcionActivo,c.Plazo,c.FechaInicio,c.FechaTermino,c.Total })
                .Select(c => new { c.Key.IdContrato, c.Key.RazonSocial,c.Key.NombreBanco,c.Key.NumeroContrato,c.Key.NombreFamilia,c.Key.DescripcionActivo,c.Key.Plazo,c.Key.FechaInicio,c.Key.FechaTermino,Total = c.Sum(x => x.Monto)}).ToList();


            return Json(listaretorno, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConsolidadoDeudaCreditosCon()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ConsolidadoDeudaCreditosCon", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ConsolidadoDeudaCreditosCon_Read()
        {

            var listaretorno = (from Con in db.Contrato
                            join TCon in db.TipoContrato on Con.IdTipoContrato equals TCon.IdTipoContrato
                            join Emp in db.Empresa on Con.IdEmpresa equals Emp.IdEmpresa
                            join Bco in db.Banco on Con.IdBanco equals Bco.IdBanco
                            join Est in db.Estado on Con.IdEstado equals Est.IdEstado
                            where Con.IdTipoFinanciamiento==(int)Helper.TipoFinanciamiento.EstructuradoConGarantia
                             //&& c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            //&& c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            //&& c.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : c.IdTipoFinanciamiento)
                            //&& c.NumeroContrato == ((numeroContrato != "") ? numeroContrato : c.NumeroContrato)
                            select new 
                            {
                                IdContrato = Con.IdContrato,
                                RazonSocial = Emp.RazonSocial,
                                NombreBanco = Bco.NombreBanco,
                                DeudaInicial=Con.Monto,
                                Con.TipoGarantia,
                                NumeroContrato = Con.NumeroContrato,
                                TasaAnual = Con.TasaAnual,
                                CuotaMes=0,
                                CapitalPagado=0,
                                SaldoInsoluto=0,
                                Plazo = Con.Plazo,
                                FechaInicio = Con.FechaInicio,
                                FechaTermino = Con.FechaTermino
                            }).AsEnumerable().ToList();
           
            return Json(listaretorno, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConsolidadoDeudaCreditosSin()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ConsolidadoDeudaCreditosSin", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ConsolidadoDeudaCreditosCapital()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ConsolidadoDeudaCreditosCapital", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        
        public ActionResult ConsolidadoDeudaLeasingTipoActivo()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ConsolidadoDeudaCreditosCapital", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        
        public ActionResult ActivosFinanciados()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ActivosFinanciados", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ModalCalcularPrepago()
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
        public ActionResult CuotasPagadas()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("CuotasPagadas", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult CuotasPagadas_Read()
        {
            //select c.IdContrato,c.NumeroContrato, c.IdBanco,c.IdEmpresa, e.RazonSocial,cda.periodo, sum(c.Monto) as montocontrato,cda.FechaPago,sum(cda.Cuota) as cuota, sum(cda.Saldo_Insoluto) as saldo_insoluto
            //from Contrato c
            //join Banco b on c.IdBanco = b.IdBanco
            //join Empresa e on c.IdEmpresa = e.IdEmpresa
            //left join Contrato_Amortizacion ca on c.IdContrato = ca.IdContrato
            //left join Contrato_DetAmortizacion cda on ca.IdContratoAmortizacion = cda.IdContratoAmortizacion
            //group by c.IdContrato,c.NumeroContrato, c.IdBanco,c.IdEmpresa,e.RazonSocial, cda.periodo,cda.FechaPago
            //order by IdContrato, cda.Periodo,cda.FechaPago asc
            var registros = (from c in db.Contrato
                             join b in db.Banco on c.IdBanco equals b.IdBanco
                             join e in db.Empresa on c.IdEmpresa equals e.IdEmpresa
                             join ca in db.Contrato_Amortizacion on c.IdContrato equals ca.IdContrato into t_ca
                             from l_ca in t_ca.DefaultIfEmpty()
                             join cda in db.Contrato_DetAmortizacion on l_ca.IdContratoAmortizacion equals cda.IdContratoAmortizacion into t_cda
                             from l_cda in t_cda.DefaultIfEmpty()
                             select new
                             {
                                 Periodo = l_cda.Periodo,
                                 IdContrato = c.IdContrato,
                                 NroContrato = c.NumeroContrato,
                                 IdBanco = b.IdBanco,
                                 Banco=b.NombreBanco,
                                 IdEmpresa = c.IdEmpresa,
                                 Empresa=e.RazonSocial,
                                 Monto = c.Monto,
                                 FechaPago=l_cda.FechaPago,
                                 SaldoInsoluto = l_cda.Saldo_Insoluto,
                                 Cuota = l_cda.Cuota,
                                 Mes=l_cda.Mes,
                                 TipoPago="Tipo Pago",
                                 ComprobanteEgreso="Comprobante Egreso",
                             }).Distinct().ToList();

            var totales = registros.GroupBy(c => new { c.IdContrato,c.NroContrato, c.IdBanco,c.Banco, c.IdEmpresa,c.Empresa,c.Periodo,c.FechaPago})
                .Select(c => new {
                    c.Key.IdContrato,
                    c.Key.NroContrato,
                    c.Key.IdBanco,
                    c.Key.Banco,
                    c.Key.IdEmpresa,
                    c.Key.Empresa,
                    c.Key.Periodo,
                    c.Key.FechaPago,
                    Item="0",
                    TipoPago="Normal",
                    MontoContrato = c.Sum(x => x.Monto),
                    Cuota = c.Sum(x => x.Cuota),
                    SaldoInsoluto=c.Sum(x => x.SaldoInsoluto),
                    ComprobanteEgreso="00000000"
                }).ToList();

            return Json(totales, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModalEstadoActivo()
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
        
        public ActionResult ControlActivoCodigoInterno()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ControlActivoCodigoInterno", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
    }
}