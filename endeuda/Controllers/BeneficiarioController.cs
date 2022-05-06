using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tesoreria.Helper;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.Globalization;
namespace tesoreria.Controllers
{
    public class BeneficiarioController : Controller
    {
        private ErpContext db = new ErpContext();
        private readonly ILog _log = LogManager.GetLogger(typeof(HomeController));
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        public JsonResult Clientes_Read(string q)
        {
            var empresaSoft = db.Empresa.Find(Helper.Constantes.IdEmpresaPrincipal);
            SoftLandContext dbSoft = new SoftLandContext(empresaSoft.BaseSoftland);

            var listaProd = (from aux in dbSoft.cwtauxi
                             where aux.ClaCli == "S" && aux.ActAux == "S"
                             && (aux.RutAux.Replace(".", "").Replace("-", "").Contains(q.Replace(".", "").Replace("-", "")) || aux.NomAux.Contains(q))
                             select new
                             {
                                 id = aux.CodAux,
                                 text = aux.RutAux + " : " + aux.NomAux
                             }).ToList();

            return Json(listaProd, JsonRequestBehavior.AllowGet);
        }        
        public JsonResult Proveedores_Read(string q,int IdEmpresa)
        {
            var empresaSoft = db.Empresa.Find(IdEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresaSoft.BaseSoftland);

            var listaProd = (from aux in dbSoft.cwtauxi
                             where aux.ClaPro == "S" && aux.ActAux == "S"
                             && (aux.RutAux.Replace(".", "").Replace("-", "").Contains(q.Replace(".", "").Replace("-", "")) || aux.NomAux.Contains(q))
                             select new
                             {
                                 id = aux.CodAux,
                                 text = aux.RutAux + " : " + aux.NomAux
                             }).ToList();

            return Json(listaProd, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Auxiliares_Read(string q, int IdEmpresa)
        {
            var empresaSoft = db.Empresa.Find(IdEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresaSoft.BaseSoftland);

            var listaProd = (from aux in dbSoft.cwtauxi
                             where aux.ClaCli == "S" && aux.ActAux == "S"
                             && (aux.RutAux.Replace(".", "").Replace("-", "").Contains(q.Replace(".", "").Replace("-", "")) || aux.NomAux.Contains(q))
                             select new
                             {
                                 id = aux.CodAux,
                                 text = aux.RutAux + " : " + aux.NomAux
                             }).ToList();

            return Json(listaProd, JsonRequestBehavior.AllowGet);
        }
    }
}