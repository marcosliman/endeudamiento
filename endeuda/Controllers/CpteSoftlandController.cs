using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.Models.Softland;
using modelo.ViewModel;
using System.Globalization;
namespace tesoreria.Controllers
{
    public class CpteSoftlandController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        public ActionResult ComprobanteSoftland(string CpbNum, string CpbAno, string baseSoftland, bool? deModal)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            SoftLandContext dbSoft = new SoftLandContext(baseSoftland);
            var movCpbte = dbSoft.cwmovim.Where(c => c.CpbNum == CpbNum && c.CpbAno == CpbAno && c.CpbNum != "00000000").FirstOrDefault();
            var nroComprobante = "";
            if (movCpbte != null)
            {
                nroComprobante = movCpbte.CpbNum;
            }
            var comprobante = DatosComprobante(nroComprobante, CpbAno, baseSoftland);
            ViewBag.deModal = deModal;
            return View(comprobante);
        }
        public ComprobanteEgresoViewModel DatosComprobante(string nroComprobante, string CpbAno, string baseSoftland)
        {
            ComprobanteEgresoViewModel comprobante = new ComprobanteEgresoViewModel();
            if (baseSoftland != "")
            {
                SoftLandContext dbSoft = new SoftLandContext(baseSoftland);
                var empresaSoft = dbSoft.soempre.FirstOrDefault();
                comprobante.AliasEmpresa = empresaSoft.NomB;
                comprobante.soempre = empresaSoft;
                var cpbSoft = dbSoft.cwcpbte.Where(c => c.CpbNum == nroComprobante && c.CpbAno == CpbAno).FirstOrDefault();
                if (cpbSoft != null)
                {
                    comprobante.CpbAno = cpbSoft.CpbAno;
                    comprobante.CpbFec = cpbSoft.CpbFec;
                    comprobante.NroComprobanteEgreso = cpbSoft.CpbNum;
                    var cwArea = dbSoft.cwtaren.Where(c => c.CodArn == cpbSoft.AreaCod).FirstOrDefault();
                    comprobante.AreaNegocio = (cwArea == null) ? "" : cwArea.DesArn;
                    comprobante.cwcpbte = cpbSoft;
                    var usuarioSoft = dbSoft.wisusuarios.Where(c => c.Usuario == cpbSoft.Usuario).FirstOrDefault();
                    comprobante.wisusuarios = usuarioSoft;
                    var detalleComprobante = (from mov in dbSoft.cwmovim
                                              join cpb in dbSoft.cwcpbte on new { mov.CpbAno, mov.CpbNum } equals new { cpb.CpbAno, cpb.CpbNum }
                                              join aux in dbSoft.cwtauxi on mov.CodAux equals aux.CodAux into t_aux
                                              join cta in dbSoft.cwpctas on mov.PctCod equals cta.PCCODI
                                              from l_aux in t_aux.DefaultIfEmpty()
                                              where cpb.CpbNum == nroComprobante && cpb.CpbAno == comprobante.CpbAno
                                              select new cwmovimViewModels
                                              {
                                                  PctCod = mov.PctCod,
                                                  NomAux = (l_aux == null) ? string.Empty : l_aux.NomAux,
                                                  RutAux = (l_aux == null) ? string.Empty : l_aux.RutAux,
                                                  TtdCod = (mov.TtdCod == "00") ? string.Empty : mov.TtdCod,
                                                  NumDoc = (mov.NumDoc == 0) ? null : mov.NumDoc,
                                                  MovTipDocRef = (mov.MovTipDocRef == "00") ? string.Empty : mov.MovTipDocRef,
                                                  MovNumDocRef = (mov.MovNumDocRef == 0) ? null : mov.MovNumDocRef,
                                                  MovDebe = (mov.MovDebe == 0) ? null : mov.MovDebe,
                                                  MovHaber = (mov.MovHaber == 0) ? null : mov.MovHaber,
                                                  MovGlosa = mov.MovGlosa,
                                                  MovNum = mov.MovNum,
                                                  PCDESC = cta.PCDESC
                                              }).OrderBy(c => c.MovNum).ToList();

                    comprobante.detalleComprobante = detalleComprobante;

                }

            }

            return comprobante;
        }
      
    }
}