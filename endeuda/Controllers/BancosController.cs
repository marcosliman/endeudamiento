using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using tesoreria.Helper;
namespace tesoreria.Controllers
{
    public class BancosController : Controller
    {
        private ErpContext db = new ErpContext();
       tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Usuario
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Bancos", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public JsonResult Bancos_Read(bool? interno)
        {
            var lista = (from bco in db.Banco.ToList()
                         select new
                         {
                             bco.IdBanco,
                             bco.NombreBanco,
                             bco.Activo,
                             bco.FechaRegistro,
                             bco.UrlLogo,
                             bco.CodBanco
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create(int? id)
        {
            Banco registro = new Banco();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Banco.Find(id);

                }
            }
            return View(registro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Banco registro)
        {

            dynamic showMessageString = string.Empty;

            Banco registroEdit = new Banco();
            if (registro.IdBanco > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Banco Actualizado" };
                registroEdit = db.Banco.Find(registro.IdBanco);
                if (registroEdit != null)
                {
                    registroEdit.NombreBanco = registro.NombreBanco;
                    registroEdit.Activo = registro.Activo;
                    registroEdit.UrlLogo = registro.UrlLogo;
                    registroEdit.CodBanco = registro.CodBanco;
                }
            }
            else
            {
                registro.FechaRegistro = DateTime.Now;
                registroEdit = registro;
                showMessageString = new { Estado = 0, Mensaje = "Banco Registrado" };
                db.Banco.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConsolidadoDeudaBanco_Read(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes)
        {
            var inicioMes = "01-" + IdMes.ToString() + "-" + anio.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            IdEmpresa = (IdEmpresa == null) ? 0 : IdEmpresa;
            var deudas = db.Database.SqlQuery<ReporteContratoViewModel>(
                   "SP_DEUDA_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4}", fechaInicio, fechaFin, (int)Helper.TipoContrato.Leasing, IdEmpresa, IdBanco).ToList();
            var sumDeudas = deudas.GroupBy(c => new { c.NombreBanco })
                .Select(c => 
                new { 
                    c.Key.NombreBanco, 
                    TotalCP = c.Sum(x => x.TotalCP), 
                    TotalLP = c.Sum(x => x.TotalLP), 
                    TotalGeneral=c.Sum(x=>x.TotalGeneral),
                    SaldoInsoluto = c.Sum(x => x.SaldoInsoluto),
                    TotalFinal=c.Sum(x => x.TotalFinal)
                }).ToList();
            var totalDeuda = sumDeudas.Sum(c => c.TotalFinal);
            var listaRetorno = sumDeudas.Select(c => new
            {
                c.NombreBanco,
                c.TotalCP,
                c.TotalLP,
                c.TotalGeneral,
                c.SaldoInsoluto,
                c.TotalFinal,
                porcEntidad=(totalDeuda!=null && c.TotalFinal>0)?Math.Round(
                    ((double)c.TotalFinal/ (double)totalDeuda)*100
                    ,2):0
            }).ToList();

            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
        }
    }
}