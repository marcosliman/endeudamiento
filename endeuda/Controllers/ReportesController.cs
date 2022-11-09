using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.Globalization;
using Microsoft.Graph.ExternalConnectors;
using System.Drawing;

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
                FiltroViewData();
                return View();
            }
        }
        public ActionResult ConsolidadoDeudaLeasing_Read(int? IdEmpresaBus, int? IdBancoBus, int? anioBus, int? IdMesBus, string valorUf)
        {
            var valorUfDouble = (valorUf != "") ? Double.Parse(valorUf) : 1;
            var inicioMes = "01-" + IdMesBus.ToString() + "-" + anioBus.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            IdEmpresaBus = (IdEmpresaBus == null) ? 0 : IdEmpresaBus;

            var deudas = db.Database.SqlQuery<ReporteContratoViewModel>(
                   "SP_DEUDA_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}", fechaInicio, fechaFin,
                   (int)Helper.TipoContrato.Leasing, IdEmpresaBus, IdBancoBus, valorUfDouble).ToList();

            var registro=(from d in deudas
                          join Con in db.Contrato on d.IdContrato equals Con.IdContrato
                          join Fam in db.Familia on d.IdFamilia equals Fam.IdFamilia into t_fam
                          from l_fam in t_fam.DefaultIfEmpty()
                          join Est in db.Estado on Con.IdEstado equals Est.IdEstado
                          where Con.FechaTermino >= fechaInicio
                          select new {
                              IdContrato = d.IdContrato,
                              RazonSocial = d.Empresa,
                              NombreBanco = d.NombreBanco,
                              NumeroContrato = d.NumeroContrato,
                              NombreFamilia = (l_fam != null) ? l_fam.NombreFamilia : "Familia No Asociada",
                              DescripcionActivo = Con.Descripcion,
                              Plazo = Con.Plazo,
                              FechaInicio = Con.FechaInicio,
                              FechaTermino = Con.FechaTermino,
                              Total = Math.Round(Con.Monto, 2),
                              TasaAnual = Con.TasaAnual,
                              PuedeEliminar = (Con.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                              NombreEstado = Est.NombreEstado,
                              d.DeudaCuota,
                              Con.TipoMoneda.NombreTipoMoneda,
                              d.IVAMes,
                              CuotaBruta= d.DeudaCuota+ d.IVAMes,
                              SaldoInsoluto=d.SaldoInsoluto
                          }).ToList();            
            return Json(registro, JsonRequestBehavior.AllowGet);
        }
        public void FiltroViewData()
        {
            var fechaActual = DateTime.Now;
            var empresa = (from e in db.Empresa
                           where e.Activo == true
                           select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
            SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
            ViewData["listaEmpresa"] = listaEmpresa;

            var bancos = (from e in db.Banco
                          where e.Activo == true
                          select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
            SelectList listaBancos = new SelectList(bancos.OrderBy(c => c.Nombre), "Id", "Nombre");
            ViewData["listaBancos"] = listaBancos;

            var meses = (from e in db.Mes
                         select new RetornoGenerico { Id = e.IdMes, Nombre = e.NombreMes }).OrderBy(c => c.Id).ToList();
            SelectList listaMes = new SelectList(meses.OrderBy(c => c.Id), "Id", "Nombre", fechaActual.Month);
            ViewData["listaMes"] = listaMes;
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
                FiltroViewData();
                return View();
            }
        }
        public ActionResult ConsolidadoDeudaCreditosCon_Read(int? IdEmpresaBus, int? IdBancoBus, int? anioBus, int? IdMesBus, string valorUf,int? tipoFinanciamiento,string ConSaldo)
        {
            var valorUfDouble = (valorUf != "") ? Double.Parse(valorUf) : 1;
            var inicioMes = "01-" + IdMesBus.ToString() + "-" + anioBus.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            IdEmpresaBus = (IdEmpresaBus == null) ? 0 : IdEmpresaBus;

            var deudas = db.Database.SqlQuery<ReporteContratoViewModel>(
                   "SP_DEUDA_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}", fechaInicio, fechaFin,
                   (int)Helper.TipoContrato.Contrato, IdEmpresaBus, IdBancoBus, valorUfDouble).ToList();

            var listaretorno = (from d in deudas
                            join Con in db.Contrato on d.IdContrato equals Con.IdContrato
                            join Fam in db.Familia on d.IdFamilia equals Fam.IdFamilia into t_fam
                            from l_fam in t_fam.DefaultIfEmpty()
                            join Est in db.Estado on Con.IdEstado equals Est.IdEstado
                            where Con.FechaTermino >= fechaInicio 
                            && ((tipoFinanciamiento!=null)?Con.IdTipoFinanciamiento== tipoFinanciamiento:true)
                            && ((ConSaldo != null && ConSaldo!="") ? 
                            ((ConSaldo=="S")?d.SaldoInsoluto >0: d.SaldoInsoluto ==0) : true)
                            select new
                            {
                                IdContrato = d.IdContrato,
                                RazonSocial = d.Empresa,
                                NombreBanco = d.NombreBanco,
                                NumeroContrato = d.NumeroContrato,
                                NombreFamilia = (l_fam != null) ? l_fam.NombreFamilia : "Familia No Asociada",
                                DescripcionActivo = Con.Descripcion,
                                Plazo = Con.Plazo,
                                FechaInicio = Con.FechaInicio,
                                FechaTermino = Con.FechaTermino,
                                DeudaInicial = Math.Round(Con.Monto, 2),
                                Con.TipoGarantia,
                                Total = Math.Round(Con.Monto, 2),
                                TasaAnual = Con.TasaAnual,
                                PuedeEliminar = (Con.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                                NombreEstado = Est.NombreEstado,
                                d.DeudaCuota,
                                CuotaMes = d.DeudaCuota,
                                Con.TipoMoneda.NombreTipoMoneda,
                                d.IVAMes,
                                CuotaBruta = d.DeudaCuota + d.IVAMes,
                                SaldoInsoluto = d.SaldoInsoluto,
                                d.CapitalPagado
                            }).ToList();
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
                FiltroViewData();
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
                FiltroViewData();
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
                FiltroViewData();
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

                return View();
            }
        }

        public ActionResult ListaActivosFinanciados_Read(int? idEmpresa, int? idBanco, string numeroActivo)
        {
            var registro = (from a in db.Activo.ToList()
                                join f in db.Familia.ToList() on a.IdFamilia equals f.IdFamilia into fw
                                from fv in fw.DefaultIfEmpty()
                                join Emp in db.Empresa.ToList() on a.IdEmpresa equals Emp.IdEmpresa
                                join ca in db.ContratoActivo.ToList() on a.IdActivo equals ca.IdActivo
                                join c in db.Contrato.ToList() on ca.IdContrato equals c.IdContrato
                                join m in db.TipoMoneda.ToList() on c.IdTipoMoneda equals m.IdTipoMoneda
                                join Bco in db.Banco.ToList() on c.IdBanco equals Bco.IdBanco
                                join Est in db.Estado.ToList() on a.IdEstado equals Est.IdEstado
                                join sa in db.PolizaActivo.ToList() on a.IdActivo equals sa.IdActivo into saw
                                from sav in saw.DefaultIfEmpty()
                                //join s in db.Poliza.ToList() on sav.IdPoliza equals s.IdPoliza into sw
                                //from sv in sw.DefaultIfEmpty()
                                where Emp.IdEmpresa == ((idEmpresa != null) ? idEmpresa : Emp.IdEmpresa)
                                && c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                                && a.NumeroInterno == ((numeroActivo != "") ? numeroActivo : a.NumeroInterno)
                                && c.IdEstado == (int)Helper.Estado.ConActivo
                                select new 
                                {
                                    IdActivo = a.IdActivo,
                                    NumeroActivo = a.NumeroInterno,
                                    Descripcion = a.Descripcion,
                                    Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                    RazonSocial = Emp.RazonSocial,
                                    NombreBanco = Bco.NombreBanco,
                                    NumeroContrato = c.NumeroContrato,
                                    Numerocuotas = c.Plazo,
                                    FechaInicio = c.FechaInicio,
                                    FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                    MontoContrato = c.Monto,
                                    TasaAnual = c.TasaAnual,
                                    Moneda = m.NombreTipoMoneda,
                                    //NumeroPoliza = (sv != null) ? sv.NumeroPoliza : string.Empty,
                                    NumeroPoliza = "",
                                    EstadoActivo = Est.NombreEstado,
                                    a.Valor,
                                    FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
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
                var fechaActual = DateTime.Now;
                var meses = (from e in db.Mes
                             select new RetornoGenerico { Id = e.IdMes, Nombre = e.NombreMes }).OrderBy(c => c.Id).ToList();
                SelectList listaMes = new SelectList(meses.OrderBy(c => c.Id), "Id", "Nombre", 1);
                ViewData["listaMesInicial"] = listaMes;
               
                SelectList listaMesFin = new SelectList(meses.OrderBy(c => c.Id), "Id", "Nombre", fechaActual.Month);
                ViewData["listaMes"] = listaMesFin;
                return View();
            }
        }
        public ActionResult CuotasPagadas_Read(int? anioInicial,int? IdMesInicial,int? anioFinal,int? IdMesFinal,int? estadoPago)
        {            
            var listado = (from c in db.Contrato
                             join b in db.Banco on c.IdBanco equals b.IdBanco
                             join e in db.Empresa on c.IdEmpresa equals e.IdEmpresa
                             join ca in db.Contrato_Amortizacion on c.IdContrato equals ca.IdContrato 
                            join cda in db.Contrato_DetAmortizacion on ca.IdContratoAmortizacion equals cda.IdContratoAmortizacion
                           where c.IdTipoContrato==(int)Helper.TipoContrato.Leasing
                           && ((anioInicial!=null)?cda.FechaPago.Year>= anioInicial:true )
                           && ((IdMesInicial != null) ? cda.FechaPago.Month >= IdMesInicial : true)
                           && ((anioFinal != null) ? cda.FechaPago.Year <= anioFinal : true)
                           && ((IdMesFinal != null) ? cda.FechaPago.Month <= IdMesFinal : true)
                           && ((estadoPago != null) ? 
                           ((estadoPago==1)? db.ComprobanteDetAmortizacion.Where(y => y.IdContratoDetAmortizacion == cda.IdContratoDetAmortizacion).Count() >0
                           : db.ComprobanteDetAmortizacion.Where(y => y.IdContratoDetAmortizacion == cda.IdContratoDetAmortizacion).Count() ==0)
                           : true)
                           //&& db.ComprobanteDetAmortizacion.Where(y => y.IdContratoDetAmortizacion == cda.IdContratoDetAmortizacion).Count() >= estadoPago
                           select new
                             {
                                 IdContrato = c.IdContrato,
                                 NroContrato = c.NumeroContrato,
                                 IdBanco = b.IdBanco,
                                 Banco=b.NombreBanco,
                                 IdEmpresa = c.IdEmpresa,
                                 Empresa=e.RazonSocial,
                                 Monto = c.Monto,
                                 FechaPago= cda.FechaPago,
                                 SaldoInsoluto = cda.Saldo_Insoluto,
                                 cda.Periodo,
                                 Cuota =cda.Cuota,
                                 cda.IvaDiferido,
                                 cda.Obligacion,
                                 Mes= cda.IdMes,
                                 CpbteT= db.ComprobanteDetAmortizacion.Where(y => y.IdContratoDetAmortizacion == cda.IdContratoDetAmortizacion && y.CpbTip=="T").FirstOrDefault(),
                                 CpbteE = db.ComprobanteDetAmortizacion.Where(y => y.IdContratoDetAmortizacion == cda.IdContratoDetAmortizacion && y.CpbTip == "E").FirstOrDefault()

                           }).ToList();
            
            return Json(listado, JsonRequestBehavior.AllowGet);
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

        public ActionResult PrestamosRelacionadas()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("PrestamosRelacionadas", Helper.TipoAcceso.Acceder))
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

        public ActionResult PrestamosRelacionadas_Read(int? idEmpresaFinancia, int? idEmpresaReceptora)
        {
            var consulta = "select mu.IdMutuo,emp.RazonSocial as EmiteDeuda,emp2.RazonSocial as RecibeDeuda, Sum(mu.MontoPrestamo) as MontoPrestamo,mu.FechaPrestamo, MUTUO.SumaAbono as MontoAbono, mu.InteresTotal, Sum(mu.MontoPrestamo) - Sum(MUTUO.SumaAbono) as Saldo " +
                          "from Empresa emp " +
                          "left " +
                          "join Mutuo mu on emp.IdEmpresa = mu.IdEmpresaFinancia " +
                          "Left " +
                          "join (select idmutuo,sum(muab.MontoAbono) as SumaAbono from mutuoabono muab group by IdMutuo) as MUTUO on MUTUO.IdMutuo = mu.IdMutuo " +
                          "left join Empresa Emp2 on mu.IdEmpresaReceptora = Emp2.IdEmpresa " +
                          "group by emp.RazonSocial,emp2.RazonSocial,mu.IdMutuo,MUTUO.SumaAbono,mu.FechaPrestamo, mu.InteresTotal";
            //select emp.RazonSocial,emp2.RazonSocial, Sum(mu.MontoPrestamo)--,Interes, montonfinal,Saldo
            //from Empresa emp
            //left join Mutuo mu on emp.IdEmpresa = mu.IdEmpresaFinancia
            //left join MutuoAbono muab on mu.IdMutuo=muab.IdMutuo
            //left join Empresa Emp2 on mu.IdEmpresaReceptora = Emp2.IdEmpresa
            //group by emp.RazonSocial,emp2.RazonSocial,mu.IdMutuo

            var contratos = db.Database.SqlQuery<ReporteRelacionadViewModel>(
                 consulta).ToList();
            return Json(contratos, JsonRequestBehavior.AllowGet);

            //var registros = (from emp in db.Empresa

            //                 join mu in db.Mutuo on emp.IdEmpresa equals mu.IdEmpresaFinancia into t_mu
            //                 from l_mu in t_mu.DefaultIfEmpty()

            //                 join muab in db.MutuoAbono on l_mu.IdMutuo equals muab.IdMutuo into t_muab
            //                 from l_muab in t_muab.DefaultIfEmpty()

            //                 join empb in db.Empresa on l_mu.IdEmpresaReceptora equals empb.IdEmpresa into t_empb
            //                 from empb in t_empb.DefaultIfEmpty()

            //                 where l_mu.IdEmpresaFinancia == ((idEmpresaFinancia != null) ? idEmpresaFinancia : l_mu.IdEmpresaFinancia)
            //                 && l_mu.IdEmpresaReceptora == ((idEmpresaReceptora != null) ? idEmpresaReceptora : l_mu.IdEmpresaReceptora)

            //                 select new
            //                 {
            //                     IdMutuo= (l_muab != null) ? l_muab.IdMutuo : 0,
            //                     MontoAbono= (l_muab != null) ? l_muab.MontoAbono : 0,
            //                     EmiteDeuda =emp.RazonSocial,
            //                     RecibeDeuda= (empb != null) ? empb.RazonSocial : null,
            //                     FechaPrestamo = (l_mu != null) ? l_mu.FechaPrestamo : (DateTime?)null,
            //                     MontoPrestamo = (l_mu != null) ? l_mu.MontoPrestamo : 0,
            //                     Interes= (l_mu != null) ? l_mu.InteresTotal : 0
            //                 }).Distinct().ToList();

            //var totales = registros.GroupBy(c => new { c.IdMutuo,c.EmiteDeuda, c.RecibeDeuda,c.FechaPrestamo})
            //    .Select(c => new {
            //        c.Key.IdMutuo,
            //        c.Key.EmiteDeuda,
            //        c.Key.RecibeDeuda,
            //        c.Key.FechaPrestamo,
            //        Monto = c.Sum(x => x.MontoPrestamo),
            //        PagosRealizados=c.Sum(x=>x.MontoAbono),
            //        Interes = c.Sum(x => x.Interes),
            //        Saldo="0",
            //    }).ToList();

            //return Json(totales, JsonRequestBehavior.AllowGet);
        }
    }
}