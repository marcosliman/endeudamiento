using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.Data;
using LinqToExcel;
using System.Data.OleDb;
using System.Data.Entity.Validation;
namespace tesoreria.Controllers
{
    public class ActivoController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato
        public ActionResult ControlInterno()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ControlInterno", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ListaActivo_Read(string numeroInterno, string codigoSoftland)
        {
            var registro = (from ac in db.Activo
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            where  ac.NumeroInterno == ((numeroInterno != "") ? numeroInterno : ac.NumeroInterno)
                            && ac.CodSoftland == ((codigoSoftland != "") ? codigoSoftland : ac.CodSoftland)
                            select new 
                            {
                                IdActivo =  ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                Descripcion = ac.Descripcion,
                                Capacidad = ac.Capacidad,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                Motor = ac.Motor,
                                Chasis = ac.Chasis,
                                Serie = ac.Serie,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente,
                                ac.IdEstado,
                                ac.NumeroContrato
            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ControlInternoBuscar()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ControlInternoBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ModalCargaMasivaA()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresa"] = listaEmpresa;
                return View();
            }
        }

        public ActionResult ModalVerActivo(int IdActivo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                if (IdActivo == 0) {

                }
                List<RetornoGenerico> cuentas = new List<RetornoGenerico>();
                List<RetornoGenerico> proveedores = new List<RetornoGenerico>();
                List<RetornoGenerico> grupos = new List<RetornoGenerico>();
                List<RetornoGenerico> subgrupos = new List<RetornoGenerico>();
                var registro = (from ac in db.Activo where ac.IdActivo == IdActivo

                                select new ActivoViewModel
                                {
                                    IdActivo = ac.IdActivo,
                                    IdEmpresa = ac.IdEmpresa,
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    IdFamilia = ac.IdFamilia,
                                    Familia = (ac.Familia.NombreFamilia != null) ? ac.Familia.NombreFamilia : string.Empty,
                                    Descripcion = ac.Descripcion,
                                    Capacidad = ac.Capacidad,
                                    Marca = ac.Marca,
                                    Modelo = ac.Modelo,
                                    Motor = ac.Motor,
                                    Chasis = ac.Chasis,
                                    Serie = ac.Serie,
                                    Anio = ac.Anio,
                                    Valor = ac.Valor,
                                    IdProveedor = ac.IdProveedor,
                                    IdCuenta = ac.IdCuenta,
                                    NumeroFactura = ac.NumeroFactura,
                                    Patente = ac.Patente,
                                    TituloBoton = "Editar",
                                    Grupo=ac.Grupo,
                                    SubGrupo=ac.SubGrupo,
                                    Activo=ac
                                }).FirstOrDefault();

                if (registro == null) {
                    registro = new ActivoViewModel();
                    registro.IdActivo = 0;
                    registro.IdProveedor = "";
                    registro.IdEmpresa = 0;
                    registro.TituloBoton = "Grabar";
                    registro.IdCuenta = "";
                }
                else
                {
                    var empresaSel = db.Empresa.Find(registro.IdEmpresa);
                    SoftLandContext dbSoft = new SoftLandContext(empresaSel.BaseSoftland);
                    cuentas = (from t in dbSoft.cwpctas
                               select new RetornoGenerico { Id = 0, Nombre = t.PCCODI + ": " + t.PCDESC, ValorString = t.PCCODI }).OrderBy(c => c.Nombre).ToList();
                    var auxiliar = dbSoft.cwtauxi.Find(registro.IdProveedor);
                    proveedores = (from aux in dbSoft.cwtauxi
                                 where aux.CodAux==registro.IdProveedor
                                 select new RetornoGenerico { Id =0, Nombre = aux.RutAux + " : " + aux.NomAux,ValorString= aux.CodAux }).OrderBy(c => c.Id).ToList();

                    grupos = (from t in dbSoft.awtgrup
                                     select new RetornoGenerico { Id = 0, Nombre = t.DesGru, ValorString = t.CodGru }).OrderBy(c => c.Nombre).ToList();
                    subgrupos = (from t in dbSoft.awtsubgr
                                     where ((registro.Grupo != "") ? t.CodGru == registro.Grupo : true)
                                     select new RetornoGenerico { Id = 0, Nombre = t.DesSGru, ValorString = t.CodSGru }).OrderBy(c => c.Nombre).ToList();
                }
                SelectList listaGrupos = new SelectList(grupos.OrderBy(c => c.Nombre), "ValorString", "Nombre", registro.Grupo);
                ViewData["listaGrupos"] = listaGrupos;
                
                SelectList listaSubGrupos = new SelectList(subgrupos.OrderBy(c => c.Nombre), "ValorString", "Nombre", registro.SubGrupo);
                ViewData["listaSubGrupos"] = listaSubGrupos;

                SelectList listaCuentas = new SelectList(cuentas.OrderBy(c => c.Nombre), "ValorString", "Nombre", registro.IdCuenta);
                ViewData["listaCuentas"] = listaCuentas;

                var empresa = (from e in db.Empresa
                                 where e.Activo == true
                                 select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var familia = (from e in db.Familia
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdFamilia, Nombre = e.NombreFamilia }).OrderBy(c => c.Id).ToList();
                SelectList listaFamilia = new SelectList(familia.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdFamilia);
                ViewData["listaFamilia"] = listaFamilia;

                SelectList listaProveedor = new SelectList(proveedores.OrderBy(c => c.Nombre), "ValorString", "Nombre");
                ViewData["listaProveedor"] = listaProveedor;
              
                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarActivo(Activo datos)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();

            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null || !seguridad.TienePermiso("ControlInterno", Helper.TipoAcceso.Acceder))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {

                datos.CodSoftland = validarDatos.ValidaStr(datos.CodSoftland);
                datos.Descripcion = validarDatos.ValidaStr(datos.Descripcion);
                datos.Capacidad = validarDatos.ValidaStr(datos.Capacidad);
                datos.Marca = validarDatos.ValidaStr(datos.Marca);
                datos.Modelo = validarDatos.ValidaStr(datos.Modelo);
                datos.Grupo = validarDatos.ValidaStr(datos.Grupo);
                datos.SubGrupo = validarDatos.ValidaStr(datos.SubGrupo);
                datos.Motor = validarDatos.ValidaStr(datos.Motor);
                datos.Chasis = validarDatos.ValidaStr(datos.Chasis);
                datos.Serie = validarDatos.ValidaStr(datos.Serie);
                datos.NumeroFactura = validarDatos.ValidaStr(datos.NumeroFactura);
                datos.Patente = validarDatos.ValidaStr(datos.Patente);
                datos.Glosa = validarDatos.ValidaStr(datos.Glosa);


                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var mensaje = "";
                        var idActivo = 0;
                        var activo = db.Activo.Where(c => c.IdActivo == datos.IdActivo).FirstOrDefault();

                        var estado = (int)Helper.Estado.ActCreado;



                        if (activo != null)
                        {

                            if (datos.CodSoftland != "" && datos.IdFamilia != null && datos.NumeroInterno != "" && activo.IdEstado == (int)Helper.Estado.ActCreado)
                            {
                                estado = (int)Helper.Estado.ActDisponible;
                            }

                            idActivo = activo.IdActivo;
                            mensaje = "Registro Actualizado con exito";
                            activo.IdEmpresa = datos.IdEmpresa;
                            activo.NumeroInterno = datos.NumeroInterno;
                            activo.CodSoftland = datos.CodSoftland;
                            activo.IdFamilia = datos.IdFamilia;
                            activo.Descripcion = datos.Descripcion;
                            activo.Capacidad = datos.Capacidad;
                            activo.Marca = datos.Marca;
                            activo.Modelo = datos.Modelo;
                            activo.Motor = datos.Motor;
                            activo.Chasis = datos.Chasis;
                            activo.Serie = datos.Serie;
                            activo.Grupo = datos.Grupo;
                            activo.SubGrupo = datos.SubGrupo;
                            activo.Anio = datos.Anio;
                            activo.Valor = datos.Valor;
                            activo.IdProveedor = datos.IdProveedor;
                            activo.IdCuenta = datos.IdCuenta;
                            activo.NumeroFactura = datos.NumeroFactura;
                            activo.Patente = datos.Patente;
                            activo.Glosa = datos.Glosa;
                            activo.IdEstado = estado;
                            db.SaveChanges();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };
                        }
                        else
                        {
                            if (datos.CodSoftland != "" && datos.IdFamilia != null && datos.NumeroInterno != "")
                            {
                                estado = (int)Helper.Estado.ActDisponible;
                            }
                            mensaje = "Registro Ingresado con exito";
                            var activoAdd = new Activo();
                            activoAdd.IdEmpresa = datos.IdEmpresa;
                            
                            activoAdd.NumeroInterno = datos.NumeroInterno;
                            activoAdd.CodSoftland = datos.CodSoftland;
                            activoAdd.IdFamilia = datos.IdFamilia;
                            activoAdd.Descripcion = datos.Descripcion;
                            activoAdd.Capacidad = datos.Capacidad;
                            activoAdd.Marca = datos.Marca;
                            activoAdd.Modelo = datos.Modelo;
                            activoAdd.Motor = datos.Motor;
                            activoAdd.Chasis = datos.Chasis;
                            activoAdd.Serie = datos.Serie;
                            activoAdd.Anio = datos.Anio;
                            activoAdd.Grupo = datos.Grupo;
                            activoAdd.SubGrupo = datos.SubGrupo;
                            activoAdd.Valor = datos.Valor;
                            activoAdd.IdProveedor = datos.IdProveedor;
                            var empresaSoft = db.Empresa.Find(datos.IdEmpresa);
                            SoftLandContext dbSoft = new SoftLandContext(empresaSoft.BaseSoftland);
                            var auxiliar = dbSoft.cwtauxi.Find(datos.IdProveedor);
                            activoAdd.NombreProveedor = (auxiliar != null) ? auxiliar.NomAux : "";
                            activoAdd.IdCuenta = datos.IdCuenta;
                            activoAdd.NumeroFactura = datos.NumeroFactura;
                            activoAdd.Patente = datos.Patente;
                            activoAdd.Glosa = datos.Glosa;
                            activoAdd.IdEstado = estado;
                            activoAdd.FechaRegistro = DateTime.Now;
                            activoAdd.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                            activoAdd.SincronizadoSoftland = false;
                            db.Activo.Add(activoAdd);
                            db.SaveChanges();

                            idActivo = activoAdd.IdActivo;
                            showMessageString = new { Estado = 0, Mensaje = mensaje };
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                    }
                }

            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EliminarActivo(int idActivo)
        {
            dynamic showMessageString = string.Empty;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var activo = db.Activo.Find(idActivo);
                    //solo elimina cuando no hay registros asociados
                    if (activo != null)
                    {
                        db.Activo.Remove(activo);
                        db.SaveChanges();

                        dbContextTransaction.Commit();

                        showMessageString = new { Estado = 0, Mensaje = "Registro eliminado con exito" };
                        return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        showMessageString = new { Estado = 500, Mensaje = "Error: No se pudo eliminar el registro" };
                        return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                    dbContextTransaction.Rollback();
                    return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult ObtenerCuentasSoftland(int idEmpresa)
        {
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var subgrupos = (from t in dbSoft.cwpctas
                             select new RetornoGenerico { Id = 0, Nombre = t.PCCODI + ": " + t.PCDESC, ValorString = t.PCCODI }).OrderBy(c => c.Nombre).ToList();
            return Json(subgrupos, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListaImportaActivo_Read(int idEmpresa, string CodGru,string CodSGru)
        {
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var activosEmpresa = db.Activo.Where(c => c.IdEmpresa == idEmpresa).ToList();
            var activosSoftland = (from ac in dbSoft.awfichaac
                            join gr in dbSoft.awtgrup on ac.CodGru equals gr.CodGru
                            join sgr in dbSoft.awtsubgr on new { ac.CodGru, ac.CodSGru } equals new {sgr.CodGru,sgr.CodSGru}
                            join prov in dbSoft.cwtauxi on ac.CodAux equals prov.CodAux into t_prov
                            from l_prov in t_prov.DefaultIfEmpty()
                            where ac.Estado=="V" && ((CodGru != "") ? ac.CodGru == CodGru : true)
                            && ((CodSGru != "") ? ac.CodSGru == CodSGru : true)
                            select new
                            {
                                ac.CodAct,
                                ac.DescAct,
                                gr.DesGru,
                                sgr.DesSGru,
                                ac.ValCom,
                                ac.FecIng,
                                ac.GloBaja,
                                ac.FecBaja,
                                ac.CtaCom,
                                ac.CodAux,
                                NomAux=(l_prov!=null)?l_prov.NomAux:"",
                                ac.NumFac,
                                ac.Leasing
                            }).AsEnumerable().ToList();

            var listaRetorno=(from ac in activosSoftland
                              join ae in activosEmpresa on ac.CodAct equals ae.CodSoftland into t_ae
                              from l_ae in t_ae.DefaultIfEmpty()
                              where l_ae==null 
                              select new
                              {
                                  DT_RowId = "row_" +ac.CodAct,
                                  ac.CodAct,
                                  ac.DescAct,
                                  ac.DesGru,
                                  ac.DesSGru,
                                  ac.ValCom,
                                  ac.FecIng,
                                  ac.GloBaja,
                                  ac.FecBaja,
                                  ac.CtaCom,
                                  ac.CodAux,
                                  ac.NomAux,
                                  ac.NumFac,
                                  ac.Leasing
                              }
                              ).ToList();
            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportaActivosSoftland()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ControlInterno", Helper.TipoAcceso.Acceder))
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
                return View();
            }
        }
        public JsonResult ObtenerGrupoSoftland(int idEmpresa)
        {
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var grupos = (from t in dbSoft.awtgrup
                             select new RetornoGenerico { Id = 0, Nombre = t.DesGru, ValorString = t.CodGru }).OrderBy(c => c.Nombre).ToList();
            return Json(grupos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerSubGrupoSoftland(int idEmpresa, string CodGru)
        {
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var subgrupos = (from t in dbSoft.awtsubgr where ((CodGru!="")? t.CodGru==CodGru :true)
                             select new RetornoGenerico { Id = 0, Nombre = t.DesSGru, ValorString = t.CodSGru }).OrderBy(c => c.Nombre).ToList();
            return Json(subgrupos, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ImportaActivoSoftland(int idEmpresa,string CodAct)
        {
            dynamic showMessageString = string.Empty;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var empresa = db.Empresa.Find(idEmpresa);
                    SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
                    var activoSoftland=dbSoft.awfichaac.Find(CodAct);
                    //solo elimina cuando no hay registros asociados
                    if (activoSoftland != null)
                    {
                        var activoAdd = new Activo();
                        activoAdd.IdEmpresa = idEmpresa;

                        activoAdd.NumeroInterno = CodAct;
                        activoAdd.CodSoftland = CodAct;
                        activoAdd.IdFamilia = null;
                        activoAdd.Descripcion = activoSoftland.DescAct;
                        activoAdd.Capacidad = "";
                        activoAdd.Marca = "";
                        activoAdd.Modelo = "";
                        activoAdd.Motor = "";
                        activoAdd.Chasis = "";
                        activoAdd.Serie = "";
                        activoAdd.Anio = ((DateTime)activoSoftland.FecIng).Year;
                        activoAdd.Grupo = activoSoftland.CodGru;
                        activoAdd.SubGrupo = activoSoftland.CodSGru;
                        activoAdd.Valor = activoSoftland.ValCom;
                        activoAdd.IdProveedor = activoSoftland.CodAux;                        
                        var auxiliar = dbSoft.cwtauxi.Find(activoSoftland.CodAux);
                        activoAdd.NombreProveedor = (auxiliar != null) ? auxiliar.NomAux : "";
                        activoAdd.IdCuenta = activoSoftland.CtaCom;
                        activoAdd.NumeroFactura = "";
                        activoAdd.Patente = "";
                        activoAdd.Glosa = "";
                        activoAdd.IdEstado = (int)Helper.Estado.ActCreado;
                        activoAdd.FechaRegistro = DateTime.Now;
                        activoAdd.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                        activoAdd.SincronizadoSoftland = true;
                        activoAdd.NumeroFactura = activoSoftland.NumFac;
                        db.Activo.Add(activoAdd);
                        db.SaveChanges();

                        dbContextTransaction.Commit();

                        showMessageString = new { Estado = 0, Mensaje = "Activo Importado con exito" };
                        return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        showMessageString = new { Estado = 500, Mensaje = "Error: No se pudo importar el activo" };
                        return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                    dbContextTransaction.Rollback();
                    return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult ImportActivosInterno(int? IdContrato)
        {
            var empresa = (from e in db.Empresa
                           where e.Activo == true
                           select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
            SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
            ViewData["listaEmpresa"] = listaEmpresa;
            var contrato = db.Contrato.Find(IdContrato);
            return View(contrato);
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportPlanillaActivoInterno(int IdEmpresa, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            List<string> data = new List<string>();
            if (archivo != null)
            {                
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                if (archivo.ContentType == "application/vnd.ms-excel" || archivo.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = archivo.FileName;
                    string targetpath = Server.MapPath("~/App_Data/");
                    archivo.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }
                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();
                    adapter.Fill(ds, "ExcelTable");
                    DataTable dtable = ds.Tables["ExcelTable"];
                    string sheetName = "Sheet1";
                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<ActivoViewModel>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.NumeroInterno !="")
                            {
                                var existeActivo = db.Activo.Where(c => c.IdEmpresa == IdEmpresa && c.NumeroInterno == a.NumeroInterno).FirstOrDefault();
                                if (existeActivo == null)
                                {
                                    Activo activoAdd = new Activo();
                                    activoAdd.IdEmpresa = IdEmpresa;
                                    activoAdd.NumeroInterno = a.NumeroInterno;
                                    activoAdd.CodSoftland = a.CodSoftland;
                                    activoAdd.IdFamilia = null;
                                    activoAdd.Descripcion = a.Descripcion;
                                    activoAdd.Capacidad = a.Capacidad;
                                    activoAdd.Marca = a.Marca;
                                    activoAdd.Modelo =a.Modelo;
                                    activoAdd.Motor = a.Motor;
                                    activoAdd.Chasis = a.Chasis;
                                    activoAdd.Serie = a.Serie;
                                    activoAdd.Anio =a.Anio;
                                    activoAdd.Valor = a.Valor;
                                    activoAdd.NumeroFactura = a.NumeroFactura;
                                    activoAdd.NumeroContrato = a.NumeroContrato;
                                    activoAdd.Patente = a.Patente;
                                    activoAdd.Glosa = "";
                                    activoAdd.IdEstado = (int)Helper.Estado.ActCreado;
                                    activoAdd.FechaRegistro = DateTime.Now;
                                    activoAdd.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    activoAdd.SincronizadoSoftland = false;
                                    db.Activo.Add(activoAdd);
                                    db.SaveChanges();
                                }

                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.NumeroInterno == "") data.Add("<li> Nro es obligatorio</li>");
                                data.Add("</ul>");
                                data.ToArray();
                                showMessageString = new { Estado = 100, Mensaje = "Error en los registros" };
                                break;
                            }
                            showMessageString = new { Estado = 0, Mensaje = "Registros Importados Exitosamente" };
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                            showMessageString = new { Estado = 100, Mensaje = "Error en los registros" };
                        }
                    }
                    
                    //var saldoInsoluto=
                    //deleting excel file from folder
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }

                }
                else
                {
                    //alert message for invalid file format
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();

                    showMessageString = new { Estado = 100, Mensaje = "Formato no permitido" };
                }
            }
            else
            {
                data.Add("<ul>");
                if (archivo == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                showMessageString = new { Estado = 100, Mensaje = "Please choose Excel file" };
            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }
    }
}