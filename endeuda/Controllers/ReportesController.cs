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