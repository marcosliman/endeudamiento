using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.UI;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Text;
using DocumentFormat.OpenXml.Math;
namespace tesoreria.Controllers
{
    public class EmpresaController : Controller
    {
        // GET: Empresa
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Empresa", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public JsonResult Empresas_Read(bool? interno)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Empresa" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            //var lista = db.Empresa.ToList();
            var lista = (from emp in db.Empresa.ToList()
                         select new
                         {
                             emp.IdEmpresa,
                             emp.RazonSocial,
                             emp.AliasEmpresa,
                             emp.BaseSoftland,
                             emp.IdTributario,
                             emp.Activo
                         }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);          

        }
        public ActionResult Create(int? id)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Empresa" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return RedirectToAction(tieneAcceso.Vista, tieneAcceso.Controlador);
            }
            Empresa empresa = new Empresa();
            empresa.Activo = true;

            if (id != null)
            {
                if (id > 0)
                {
                    empresa = db.Empresa.Find(id);

                }
            }
            else { 
                id = 0;
            }

            var empresas = (from emp in db.Empresa
                            join eu in db.EmpresaRelacionada.Where(c => c.IdEmpresa == id) on emp.IdEmpresa equals eu.IdEmpresaRelacionada into t_eu
                            from l_eu in t_eu.DefaultIfEmpty()
                            where emp.Activo == true
                            select new RetornoGenerico { Id = emp.IdEmpresa, Nombre = emp.RazonSocial, Seleccionado = (l_eu != null) ? true : false }).OrderBy(c => c.Nombre).ToList();

            ViewData["listaEmpresas"] = empresas;
           
            return View(empresa);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Empresa empresa, int[] empresas)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Empresa" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            Empresa empresaEdit = new Empresa();
            if (empresa.IdEmpresa > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Empresa Actualizada" };
                empresaEdit = db.Empresa.Find(empresa.IdEmpresa);
                if (empresaEdit != null)
                {
                    empresaEdit.RazonSocial = empresa.RazonSocial;
                    empresaEdit.AliasEmpresa = empresa.AliasEmpresa;
                    empresaEdit.BaseSoftland = empresa.BaseSoftland;
                    empresaEdit.IdTributario = empresa.IdTributario;
                    empresaEdit.Activo = empresa.Activo;
                }
            }
            else
            {
                empresaEdit = empresa;
                showMessageString = new { Estado = 0, Mensaje = "Empresa Registrada" };
                db.Empresa.Add(empresa);
            }
            
            db.SaveChanges();
            var uEmpresas = db.EmpresaRelacionada.Where(c => c.IdEmpresa == empresaEdit.IdEmpresa);
            db.EmpresaRelacionada.RemoveRange(uEmpresas);
            db.SaveChanges();
            if (empresas != null)
            {
                foreach (var id in empresas)
                {
                    EmpresaRelacionada newEmpresa = new EmpresaRelacionada();
                    newEmpresa.IdEmpresa = empresa.IdEmpresa;
                    newEmpresa.IdEmpresaRelacionada = id ;
                    db.EmpresaRelacionada.Add(newEmpresa);
                }
            }
            db.SaveChanges();
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResumenDeudaTipoFinanciamiento_Read(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes,string valorUf)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Solicitante" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
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
                   "SP_DEUDA_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}", 
                   fechaInicio, fechaFin, (int)Helper.TipoContrato.Contrato, IdEmpresa, IdBanco, valorUfDouble).ToList();
            var empresas = deudas.GroupBy(c => new { c.IdEmpresa, c.Empresa }).Select(c => new { c.Key.IdEmpresa, c.Key.Empresa }).ToList();
            var tiposFinancia= deudas.Where(c=>c.SaldoInsoluto>0).GroupBy(c => new { c.IdTipoFinanciamiento, c.NombreTipoFinanciamiento})
                .Select(c => new { c.Key.IdTipoFinanciamiento, c.Key.NombreTipoFinanciamiento }).ToList();
            var sumDeudas = deudas.GroupBy(c => new { c.Empresa,c.IdTipoFinanciamiento })
                .Select(c =>
                new {
                    c.Key.IdTipoFinanciamiento,
                    c.Key.Empresa,
                    SaldoInsoluto = Math.Round(((double)c.Sum(x => x.SaldoInsoluto)/ 1000000),0)
                })
                .ToList();
            var serie = (from tf in tiposFinancia
                         select new
                         {
                             name = tf.NombreTipoFinanciamiento,
                             data = sumDeudas.Where(x => x.IdTipoFinanciamiento == tf.IdTipoFinanciamiento)
                             .Select(x => new { name = x.Empresa, y = x.SaldoInsoluto }).ToList()
                         }
                       ).ToList();
            return Json(new { categoria= empresas, serie }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConsolidadoDeudaEmpresa_Read(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes, string valorUf)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Solicitante" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var valorUfDouble=(valorUf!="")?Double.Parse(valorUf):1;
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
                   "SP_DEUDA_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}", fechaInicio, fechaFin, 0, IdEmpresa,IdBanco, valorUfDouble).ToList();
            //var total=deudas.Sum(c => c.SaldoInsoluto);
            //var deudaMqs = deudas.Where(c=>c.IdTipoContrato == (int)Helper.TipoContrato.Leasing && c.IdEmpresa == 1).Sum(c => c.SaldoInsoluto);
            var empresas = deudas.GroupBy(c => new { c.Empresa, c.IdEmpresa }).Select(c => new { c.Key.Empresa,c.Key.IdEmpresa}).ToList();
            var sumDeudas = ( from c in empresas
                select new {
                    c.Empresa,             
                    DeudaCredito = deudas.Where(x=>x.IdTipoContrato== (int)Helper.TipoContrato.Contrato && x.IdEmpresa==c.IdEmpresa)
                    .Sum(x => x.SaldoInsoluto),
                    DeudaLeasing = deudas.Where(x => x.IdTipoContrato == (int)Helper.TipoContrato.Leasing && x.IdEmpresa==c.IdEmpresa)
                    .Sum(x => x.SaldoInsoluto)
                }).ToList();
            var listaRetorno = sumDeudas.Select(c => new
            {
                c.Empresa,                
                c.DeudaCredito,
                c.DeudaLeasing,
                TotalFinal = c.DeudaCredito + c.DeudaLeasing
            }).ToList();

            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConsolidadoDeudaEmpresaAnios_Read(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes, string valorUf)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Solicitante" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return Json(new { tieneAcceso.Estado, tieneAcceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
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
                   "SP_DEUDA_CONTRATO_CONSOLIDADA @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}", fechaInicio, fechaFin, 
                   0, IdEmpresa,IdBanco, valorUfDouble).ToList();
            var listAnios = deudas.GroupBy(c => new { c.anio}).Select(c => new { c.Key.anio}).ToList();
            var tiposFinancia = deudas.GroupBy(c => new { c.IdTipoContrato})
                .Select(c => new { c.Key.IdTipoContrato}).ToList();
            var sumDeudas = deudas.GroupBy(c => new { c.IdTipoContrato,c.anio })
                .Select(c =>
                new {
                    c.Key.IdTipoContrato,
                    c.Key.anio,
                    DeudaCapital = Math.Round(((double)c.Sum(x => x.DeudaCapital) / 1000000), 0),
                    DeudaCuota = Math.Round(((double)c.Sum(x => x.DeudaCuota) / 1000000), 0)
                })
                .ToList();
            var serie = (from tf in tiposFinancia
                         select new
                         {
                             name = (tf.IdTipoContrato==(int)Helper.TipoContrato.Contrato)?"Deuda Estructurada (Capital)":"Deuda Leasing (Capital)",
                             data = sumDeudas.Where(x => x.IdTipoContrato == tf.IdTipoContrato)
                             .Select(x => new { name = x.anio, y = x.DeudaCapital }).OrderBy(x=>x.name).ToList()
                         }
                       ).ToList();
            return Json(new { categoria = listAnios, serie }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TipoDeudaResumen(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes, string valorUf)
        {
            var tieneAcceso = loginCtrl.ValidaAcceso(new string[] { "Solicitante" }, Helper.TipoAcceso.Acceder);
            if (tieneAcceso.AccesoValido == false)
            {
                return RedirectToAction(tieneAcceso.Vista, tieneAcceso.Controlador);
            }
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
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
                       "SP_DEUDA_CONTRATO_CONSOLIDADA @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}", 
                       fechaInicio, fechaFin, 0, IdEmpresa,IdBanco, valorUfDouble).ToList();
                var listAnios = deudas.GroupBy(c => new { c.anio }).Select(c => new { c.Key.anio }).ToList();
                var tiposFinancia = deudas.GroupBy(c => new { c.IdTipoContrato})
                    .Select(c => new { c.Key.IdTipoContrato}).ToList();
                var sumDeudas = deudas.GroupBy(c => new { c.IdTipoContrato, c.anio })
                    .Select(c =>
                    new {
                        c.Key.IdTipoContrato,
                        c.Key.anio,
                        DeudaCapital = Math.Round(((double)c.Sum(x => x.DeudaCapital) / 1000000), 0),
                        DeudaCuota = Math.Round(((double)c.Sum(x => x.DeudaCuota) / 1000000), 0)
                    })
                    .ToList();
                var valoresAnio = (from d in sumDeudas
                                   select new ReporteContratoViewModel
                                     {
                                        IdTipoContrato = d.IdTipoContrato,
                                        anio =d.anio,
                                       DeudaCapital = d.DeudaCapital,
                                       DeudaCuota=d.DeudaCuota
                                   }).ToList();
                ViewData["listAnios"] = listAnios.Select(c => new RetornoGenerico { Id = (int)c.anio, Nombre = c.anio.ToString() }).OrderBy(c => c.Id).ToList();
                ViewData["listTipoFinancia"] = tiposFinancia
                    .Select(c => new RetornoGenerico { Id = (int)c.IdTipoContrato,Nombre= (c.IdTipoContrato == (int)Helper.TipoContrato.Contrato) ? "Deuda Estructurada" : "Deuda Leasing" })
                    .OrderBy(c => c.Id).ToList();
                ViewData["listValoresAnio"] = valoresAnio;
                return View();
            }
        }
        public ActionResult DescargarBaseContratos(int? IdEmpresaBus, int? IdBancoBus, int? anioBus, int? IdMesBus, string valorUfBus)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Solicitante" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var valorUfDouble = (valorUfBus != "") ? Double.Parse(valorUfBus) : 1;
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
                   0, IdEmpresaBus, IdBancoBus, valorUfDouble).ToList();
            
            GridView gridviewResultado = new GridView();
            System.Data.DataTable table;
            System.Data.DataRow row;
            table = new System.Data.DataTable();
            //Agregar Encabezados
            table.Columns.Add("IdContrato", typeof(System.String));
            table.Columns.Add("IdEmpresa", typeof(System.String));
            table.Columns.Add("Empresa", typeof(System.String));
            table.Columns.Add("IdBanco", typeof(System.String));
            table.Columns.Add("NombreBanco", typeof(System.String));
            table.Columns.Add("NumeroContrato", typeof(System.String));
            table.Columns.Add("IdTipoContrato", typeof(System.String));
            table.Columns.Add("NombreTipoContrato", typeof(System.String));
            table.Columns.Add("Moneda", typeof(System.String));
            table.Columns.Add("NombreTipoFinanciamiento", typeof(System.String));            
            table.Columns.Add("TotalCP", typeof(System.String));
            table.Columns.Add("TotalLP", typeof(System.String));
            table.Columns.Add("TotalGeneral", typeof(System.String));
            table.Columns.Add("SaldoInsoluto", typeof(System.String));
            table.Columns.Add("TotalFinal", typeof(System.String));
            table.Columns.Add("IdFamilia", typeof(System.String)); 
            Session.Add("Tabla", table);
            var fechaActual = DateTime.Now.Date;
            foreach (var d in deudas)
            {                
                GridView gridview = new GridView();
                table = (System.Data.DataTable)(Session["Tabla"]);
                row = table.NewRow();
                row["IdContrato"] = d.IdContrato;
                row["IdEmpresa"] = d.IdEmpresa;
                row["Empresa"] = d.Empresa;
                row["IdBanco"] = d.IdBanco;
                row["NombreBanco"] = d.NombreBanco;
                row["NumeroContrato"] = d.NumeroContrato;
                row["IdTipoContrato"] = d.IdTipoContrato;
                row["NombreTipoContrato"] = d.NombreTipoContrato;
                row["Moneda"] = d.Moneda;
                row["NombreTipoFinanciamiento"] = d.NombreTipoFinanciamiento;
                row["TotalCP"] = d.TotalCP;
                row["TotalLP"] = d.TotalLP;
                row["TotalGeneral"] = d.TotalGeneral;
                row["SaldoInsoluto"] = d.SaldoInsoluto;
                row["TotalFinal"] = d.TotalFinal;
                row["IdFamilia"] = d.IdFamilia;
                table.Rows.Add(row);

                gridview.DataSource = table;
                gridview.DataBind();
                Session.Add("Tabla", table);

                gridviewResultado.DataSource = gridview.DataSource;
                gridviewResultado.DataBind();

            }
            
            var nombreFile = "Contratos_" + anioBus.ToString()+"_"+ IdMesBus.ToString();
            Response.ClearContent();
            Response.Buffer = true;
            // set the header
            Response.ContentType = "application/ms-excel";
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("content-disposition", "attachment; filename = " + nombreFile + ".xls");
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.Default;
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    gridviewResultado.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                }
            }
            return View();
        }
    }
}
