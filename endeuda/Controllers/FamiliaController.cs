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
    public class FamiliaController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("Familia", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var Familia =
                    (from Fam in db.Familia
                     select new RetornoGenerico
                     {
                         Id = Fam.IdFamilia,
                         Nombre = Fam.NombreFamilia
                     }
                    ).OrderBy(c => c.Id).ToList();
                SelectList listaFamilias = new SelectList(Familia.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaFamilias"] = listaFamilias;
                return View();
            }
        }
        public JsonResult Familia_Read(bool? interno, string familia, bool? activo)
        {

            var lista = (from Fam in db.Familia
                         where
                         ((familia != "" && familia!=null) ? (Fam.NombreFamilia.Contains(familia)) : true) 
                         select new FamiliaViewModel
                         {
                             IdFamilia= Fam.IdFamilia,
                             NombreFamilia= Fam.NombreFamilia,
                             Activo = Fam.Activo
                         }).AsEnumerable().OrderBy(c => c.NombreFamilia).ToList();
            if (activo != null)
            {
                lista = lista.Where(c => c.Activo == activo).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create(int? id)
        {
            Familia registro = new Familia();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Familia.Find(id);
                }
            }
          
            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateFamilia(Familia registro)
        {
            dynamic showMessageString = string.Empty;

            Familia registroEdit = new Familia();
            if (registro.IdFamilia > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Familia Actualizada" };
                registroEdit = db.Familia.Find(registro.IdFamilia);
                if (registroEdit != null)
                {
                    registroEdit.NombreFamilia = registro.NombreFamilia;
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                showMessageString = new { Estado = 0, Mensaje = "Familia registrada correctamente" };
                db.Familia.Add(registro);
            }

            db.SaveChanges();

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TipoDeudaFamilia(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes, string valorUf,int? IdFamilia)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var familia = db.Familia.Find(IdFamilia);
                var valorUfDouble = (valorUf != "") ? Double.Parse(valorUf) : 1;
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
                       "SP_DEUDA_CONTRATO_FAMILIA @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5},@IdFamilia={6}",
                       fechaInicio, fechaFin, 0, IdEmpresa, IdBanco, valorUfDouble, IdFamilia).ToList();
                var listAnios = deudas.GroupBy(c => new { c.anio }).Select(c => new { c.Key.anio }).ToList();                
                var sumDeudas = deudas.GroupBy(c => new { c.anio })
                    .Select(c =>
                    new {
                        c.Key.anio,
                        DeudaCapital = Math.Round(((double)c.Sum(x => x.DeudaCapital) / 1000000), 0),
                        DeudaCuota = Math.Round(((double)c.Sum(x => x.DeudaCuota) / 1000000), 0),
                        DeudaInteres= Math.Round(((double)c.Sum(x => x.DeudaInteres) / 1000000), 0),
                    })
                    .ToList();
                var valoresAnio = (from d in sumDeudas
                                   select new ReporteContratoViewModel
                                   {
                                       anio = d.anio,
                                       DeudaCapital = d.DeudaCapital,
                                       DeudaCuota = d.DeudaCuota,
                                       DeudaInteres=d.DeudaInteres
                                   }).ToList();
                ViewData["listAnios"] = listAnios.Select(c => new RetornoGenerico { Id = (int)c.anio, Nombre = c.anio.ToString() }).OrderBy(c => c.Id).ToList();
                ViewData["listValoresAnio"] = valoresAnio;
                ViewData["InteresMes"] = (deudas.Where(c => c.anio == anio).Count()>0)?deudas.Where(c => c.anio == anio).Sum(c => c.DeudaInteresMes):0;
                ViewData["CuotaMes"] = (deudas.Where(c => c.anio == anio).Count() > 0) ? deudas.Where(c => c.anio == anio).Sum(c => c.DeudaCuotaMes) : 0;
                return View(familia);
            }
        }
    }
}