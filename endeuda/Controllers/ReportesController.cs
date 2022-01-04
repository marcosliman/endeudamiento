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
        public ActionResult ConsolidadoLeasing_Read()
        {
            var registro = (from Con in db.Contrato
                            join TCon in db.TipoContrato on Con.IdTipoContrato equals TCon.IdTipoContrato
                            join ConAct in db.ContratoActivo on Con.IdContrato equals ConAct.IdContrato
                            join Act in db.Activo on ConAct.IdActivo equals Act.IdActivo
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
                                IdBanco = Con.IdBanco,
                                DescActivo = Act.Descripcion,
                                Plazo = Con.Plazo,
                                Monto = Con.Monto,
                                TasaAnual = Con.TasaAnual,
                                TasaMensual = Con.TasaMensual,
                                FechaInicio = Con.FechaInicio,
                                FechaTermino = Con.FechaTermino,
                                PuedeEliminar = (Con.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                                NombreEstado = Est.NombreEstado
                            }).AsEnumerable().ToList();

            foreach (var reg in registro)
            {
                var activo = (from ca in db.ContratoActivo
                              join a in db.Activo on ca.IdActivo equals a.IdActivo
                              join f in db.Familia on a.IdFamilia equals f.IdFamilia into fw
                              from fv in fw.DefaultIfEmpty()
                              where ca.IdContrato == reg.IdContrato
                              select new { fv.NombreFamilia } into x
                              group x by new { x.NombreFamilia } into g
                              select new
                              {
                                  cont = g.Count(),
                                  g.Key.NombreFamilia
                              }).ToList();

                var desc = "";
                if (activo != null)
                {
                    foreach (var a in activo)
                    {
                        desc += a.cont.ToString() + " " + a.NombreFamilia + ", ";
                    }

                }

                reg.Descripcion = desc;
            }

            return Json(registro, JsonRequestBehavior.AllowGet);
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