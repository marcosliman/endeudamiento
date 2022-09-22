using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using LinqToExcel;
using System.Data.OleDb;
using System.Data.Entity.Validation;
namespace tesoreria.Controllers
{
    public class ContratoController : Controller
    {
        private ErpContext db = new ErpContext();
        private InmobContext dbInmob = new InmobContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Contrato

        #region Contrato leasing
        public ActionResult ListaContratoLeasing()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
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

                var tipoFinancimiento = (from e in db.TipoFinanciamiento
                                         where e.Activo == true
                                         select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoFinancimiento = new SelectList(tipoFinancimiento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoFinancimiento"] = listaTipoFinancimiento;

                return View();
            }
        }

        public ActionResult ListaContrato_Read(int tipoContrato, int? idBanco, int? idEmpresa, string numeroContrato,int? IdEstado)
        {
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado.ToList() on c.IdEstado equals e.IdEstado
                            where c.IdTipoContrato == tipoContrato
                            && c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            && c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            && c.NumeroContrato == ((numeroContrato != "") ? numeroContrato : c.NumeroContrato)
                            && ((IdEstado!=null)?c.IdEstado==IdEstado:true)
                            select new ContratoViewModel
                            {
                                IdContrato = c.IdContrato,
                                NumeroContrato = c.NumeroContrato,
                                RazonSocial = c.Empresa.RazonSocial,
                                NombreBanco = c.Banco.NombreBanco,
                                NombreTipoFinanciamiento = (c.TipoFinanciamiento != null) ? c.TipoFinanciamiento.NombreTipoFinanciamiento : string.Empty,
                                Plazo = c.Plazo,
                                Monto = c.Monto,
                                TasaAnual = c.TasaAnual,
                                TasaMensual = c.TasaMensual,
                                FechaInicio = c.FechaInicio,
                                FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                FechaTermino = c.FechaTermino,
                                FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (c.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                                NombreEstado = e.NombreEstado
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegistrarContratoLeasing(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                //var contrato = new ContratoViewModel();
                var contrato = (from c in db.Contrato where c.IdContrato == idContrato
                                select new ContratoViewModel {
                                    IdContrato = c.IdContrato,
                                    ExisteContrato = "S"
                                }).FirstOrDefault();
                if (contrato == null) {
                    contrato = new ContratoViewModel();
                    contrato.IdContrato = idContrato;
                    contrato.ExisteContrato = "N";
                }
                
                return View(contrato);
            }
        }

        public ActionResult AddContratoLeasing(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from c in db.Contrato.ToList()
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    IdLicitacionOferta = c.IdLicitacionOferta,
                                    EsLicitacion = (c.IdLicitacionOferta != null) ? "SI" : "NO",
                                    MotivoEleccion = c.MotivoEleccion,
                                    IdEmpresa = c.IdEmpresa,
                                    IdBanco = c.IdBanco,
                                    IdTipoImpuesto = c.IdTipoImpuesto,
                                    NumeroContrato = c.NumeroContrato,
                                    TasaMensual = c.TasaMensual,
                                    TasaAnual = c.TasaAnual,
                                    Plazo = c.Plazo,
                                    Monto = c.Monto,
                                    FechaInicio = c.FechaInicio,
                                    FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                    FechaTermino = c.FechaTermino,
                                    FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                    IdEstado = c.IdEstado,
                                    ExisteContrato = "S",
                                    TituloBoton = "Actualizar Contrato",
                                    IdTipoMoneda=c.IdTipoMoneda,
                                    Descripcion = (c.Descripcion != null) ? c.Descripcion : string.Empty
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new ContratoViewModel();
                    registro.IdLicitacionOferta = 0;
                    registro.EsLicitacion = "NA";
                    registro.IdEmpresa = 0;
                    registro.IdBanco = 0;
                    registro.IdTipoImpuesto = 0;
                    registro.Monto = null;
                    registro.TasaMensual = null;
                    registro.TasaAnual = null;
                    registro.Plazo = null;
                    registro.IdEstado = 0;
                    registro.ExisteContrato = "N";
                    registro.TituloBoton = "Grabar Contrato";
                    registro.IdTipoMoneda = 1;
                }
                //else {
                //    if (registro.Descripcion.Length == 0)
                //    {
                //        var activo = (from ca in db.ContratoActivo
                //                      join a in db.Activo on ca.IdActivo equals a.IdActivo
                //                      join f in db.Familia on a.IdFamilia equals f.IdFamilia into fw
                //                      from fv in fw.DefaultIfEmpty()
                //                      where ca.IdContrato == idContrato
                //                      select new { fv.NombreFamilia } into x
                //                      group x by new { x.NombreFamilia } into g
                //                      select new
                //                      {
                //                          cont = g.Count(),
                //                          g.Key.NombreFamilia
                //                      }).ToList();

                //        var desc = "";
                //        if (activo != null)
                //        {
                //            foreach (var a in activo)
                //            {
                //                desc += a.cont.ToString() + " " + ((a.NombreFamilia != null) ? a.NombreFamilia : "Familia No asociada") + ", ";



                //            }

                //        }

                //        registro.Descripcion = desc;
                //    }
                //}

                var licitacionOferta = db.LicitacionOferta.Where(c => c.IdLicitacionOferta == registro.IdLicitacionOferta).FirstOrDefault();
                var idLicitacion = 0;
                var idLicitacionOferta = 0;

                if (licitacionOferta != null)
                {
                    idLicitacion = licitacionOferta.IdLicitacion;
                    idLicitacionOferta = licitacionOferta.IdLicitacionOferta;

                    var licitacion = (from e in db.Licitacion
                                      where e.IdLicitacion == idLicitacion
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacionOferta == idLicitacionOferta
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }
                else {
                    var licitacion = (from e in db.Licitacion
                                      where e.IdEstado == (int)Helper.Estado.LicFinalizada && e.IdTipoFinanciamiento == (int)Helper.TipoContrato.Leasing
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacion == idLicitacion
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }

                var tiposMoneda = (from e in db.TipoMoneda
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdTipoMoneda, Nombre = e.NombreTipoMoneda }).OrderBy(c => c.Id).ToList();
                SelectList listaMonedas = new SelectList(tiposMoneda.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoMoneda);
                ViewData["listaMonedas"] = listaMonedas;

                var banco = (from e in db.Banco
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre",registro.IdBanco);
                ViewData["listaBanco"] = listaBanco;

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre",registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var impuesto = (from e in db.TipoImpuesto
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdTipoImpuesto, Nombre = e.NombreTipoImpuesto }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoImpuesto = new SelectList(impuesto.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoImpuesto);
                ViewData["listaTipoImpuesto"] = listaTipoImpuesto;

                return View(registro);
            }
        }

        public JsonResult ListaOferta(int idLicitacion)
        {
            var oferta = (from e in db.LicitacionOferta
                          join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                          join b in db.Banco on e.IdBanco equals b.IdBanco
                          where e.IdLicitacion == idLicitacion
                          select new RetornoGenerico
                           {
                               Id = e.IdLicitacionOferta,
                               Nombre = b.NombreBanco,
                               Seleccionado = false,
                               Editable = true
                           }).OrderBy(c => c.Id).ToList();
            return Json(oferta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOferta(int idLicitacionOferta)
        {
            var oferta = (from e in db.LicitacionOferta
                          join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                          join b in db.Banco on e.IdBanco equals b.IdBanco
                          where e.IdLicitacionOferta == idLicitacionOferta
                          select new LicitacionOfertaViewModel
                          {
                              IdLicitacionOferta = e.IdLicitacionOferta,
                              IdBanco = e.IdBanco,
                              IdEmpresa = l.IdEmpresa,
                              TasaMensual = e.TasaMensual,
                              TasaAnual = e.TasaAnual,
                              Plazo = e.Plazo,
                              IdTipoFinanciamiento = l.IdTipoFinanciamiento

                          }).FirstOrDefault();
            return Json(oferta, JsonRequestBehavior.AllowGet);
        }

        public string TextoLogContrato(Contrato contratoEdit, Contrato dato)
        {
            var textoLog = "";
            /*log de cambios*/
            if (contratoEdit.Monto != dato.Monto)
            {
                textoLog += "Monto Contrato: " + contratoEdit.Monto + " por " + dato.Monto;
            }
            if (contratoEdit.NumeroContrato != dato.NumeroContrato)
            {
                textoLog += ", Número Contrato: " + contratoEdit.NumeroContrato + " por " + dato.NumeroContrato;
            }
            if (contratoEdit.MotivoEleccion != dato.MotivoEleccion)
            {
                textoLog += ", Motivo Elección: " + contratoEdit.MotivoEleccion + " por " + dato.MotivoEleccion;
            }
            if (contratoEdit.IdBanco != dato.IdBanco)
            {
                var banco = db.Banco.Find(dato.IdBanco);
                textoLog += ", Banco: " + contratoEdit.Banco.NombreBanco + " por " + banco.NombreBanco;
            }
            if (contratoEdit.IdTipoFinanciamiento != dato.IdTipoFinanciamiento)
            {
                var tipoF = db.TipoFinanciamiento.Find(dato.IdTipoFinanciamiento);
                textoLog += ", Tipo Financiamiento: " + contratoEdit.TipoFinanciamiento.NombreTipoFinanciamiento + " por " + tipoF.NombreTipoFinanciamiento;
            }
            if (contratoEdit.IdTipoImpuesto != dato.IdTipoImpuesto)
            {
                var tipoI = db.TipoImpuesto.Find(dato.IdTipoImpuesto);
                textoLog += ", Tipo Impuesto: " + contratoEdit.TipoImpuesto.NombreTipoImpuesto + " por " + tipoI.NombreTipoImpuesto;
            }
            if (contratoEdit.TipoGarantia != dato.TipoGarantia)
            {
                textoLog += ", Tipo Garantía: " + contratoEdit.TipoGarantia + " por " + dato.TipoGarantia;
            }
            if (contratoEdit.TasaMensual != dato.TasaMensual)
            {
                textoLog += ", Tasa Mensual: " + contratoEdit.TasaMensual + " por " + dato.TasaMensual;
            }
            if (contratoEdit.TasaAnual != dato.TasaAnual)
            {
                textoLog += ", Tasa Anual: " + contratoEdit.TasaAnual + " por " + dato.TasaAnual;
            }
            if (contratoEdit.Plazo != dato.Plazo)
            {
                textoLog += ", Plazo Contrato: " + contratoEdit.Plazo + " por " + dato.Plazo;
            }
            if (contratoEdit.FechaInicio != dato.FechaInicio)
            {
                textoLog += ", Fecha Inicio Contrato: " + contratoEdit.FechaInicio + " por " + dato.FechaInicio;
            }
            
            return textoLog;
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualizarContratoVista(Contrato dato)
        {
            dynamic showMessageString = string.Empty;
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            if(seguridad == null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Editar))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión o no cuenta con permisos" };
            }
            else
            {
                var contrato = db.Contrato.Find(dato.IdContrato);
                var contratoEdit = db.Contrato.Find(dato.IdContrato);
                var mensaje = "";
                var idContrato = 0;
                //validar datos
                dato.Descripcion = validarDatos.ValidaStr(dato.Descripcion);
                dato.TipoGarantia = validarDatos.ValidaStr(dato.TipoGarantia);
                if (contrato != null)
                {
                    dato.IdTipoFinanciamiento = (contrato.IdTipoContrato == (int)Helper.TipoContrato.Leasing) ? contrato.IdTipoFinanciamiento : dato.IdTipoFinanciamiento;
                    var textoLog = TextoLogContrato(contratoEdit, dato);
                    if (contratoEdit.Descripcion != dato.Descripcion)
                    {
                        textoLog += ", Descripción: " + contratoEdit.Descripcion + " por " + dato.Descripcion;
                    }
                    mensaje = "Contrato actualizado con éxito";
                    dato.FechaTermino = dato.FechaInicio.AddMonths(dato.Plazo - 1);
                    idContrato = dato.IdContrato;
                    contrato.Monto = dato.Monto;
                    contrato.MotivoEleccion = dato.MotivoEleccion;
                    contrato.IdEmpresa = dato.IdEmpresa;
                    contrato.IdBanco = dato.IdBanco;
                    contrato.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                    contrato.IdTipoImpuesto = dato.IdTipoImpuesto;
                    contrato.TipoGarantia = dato.TipoGarantia;
                    contrato.TasaMensual = dato.TasaMensual;
                    contrato.TasaAnual = dato.TasaAnual;
                    contrato.Plazo = dato.Plazo;
                    contrato.FechaInicio = dato.FechaInicio;
                    contrato.FechaTermino = dato.FechaTermino;
                    contrato.IdTipoMoneda = dato.IdTipoMoneda;
                    contrato.Descripcion = dato.Descripcion;
                    contrato.IdFamilia = dato.IdFamilia;
                    db.SaveChanges();                   
                    /*registro log contrato*/
                    GrabaLogContrato(idContrato, 1, textoLog);

                    showMessageString = new { Estado = 0, Mensaje = mensaje, idContrato = idContrato };
                }
            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarContratoLeasing(Contrato dato)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    dato.MotivoEleccion = validarDatos.ValidaStr(dato.MotivoEleccion);
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";
                            var idContrato = 0;
                            var swTran = true;

                            var contrato = db.Contrato.Find(dato.IdContrato);
                            var contratoEdit = db.Contrato.Find(dato.IdContrato);

                            //validar datos
                            dato.Descripcion = validarDatos.ValidaStr(dato.Descripcion);
                            dato.TipoGarantia = validarDatos.ValidaStr(dato.TipoGarantia);

                            /*verifico si el origen es una licitacion*/
                            var existeOferta = "N";
                            var oferta = new LicitacionOfertaViewModel();
                            if (contrato != null){
                                dato.IdLicitacionOferta = contrato.IdLicitacionOferta;
                            }
                            if (dato.IdLicitacionOferta > 0) {
                                oferta = (from e in db.LicitacionOferta
                                              join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                              join b in db.Banco on e.IdBanco equals b.IdBanco
                                              where e.IdLicitacionOferta == dato.IdLicitacionOferta
                                              select new LicitacionOfertaViewModel
                                              {
                                                  IdLicitacionOferta = e.IdLicitacionOferta,
                                                  IdLicitacion = l.IdLicitacion,
                                                  IdBanco = e.IdBanco,
                                                  IdEmpresa = l.IdEmpresa,
                                                  TasaMensual = e.TasaMensual,
                                                  TasaAnual = e.TasaAnual,
                                                  Plazo = e.Plazo,
                                                  IdTipoFinanciamiento = l.IdTipoFinanciamiento


                                              }).FirstOrDefault();

                                if(oferta != null) {
                                    dato.TasaMensual = (double)oferta.TasaMensual;
                                    dato.TasaAnual = (double)oferta.TasaAnual;
                                    dato.Plazo = (int)oferta.Plazo;
                                    dato.IdEmpresa = (int)oferta.IdEmpresa;
                                    dato.IdTipoFinanciamiento = (int)oferta.IdTipoFinanciamiento;
                                    dato.IdBanco = oferta.IdBanco;
                                    existeOferta = "S";
                                }
                            }

                            dato.FechaTermino = dato.FechaInicio.AddMonths(dato.Plazo-1);
                            if (dato.IdTipoContrato == (int)Helper.TipoContrato.Leasing) {
                                dato.IdTipoFinanciamiento = (int)Helper.TipoFinanciamiento.Leasing;
                                dato.IdTipoContrato = (int)Helper.TipoContrato.Leasing;
                            }
                            else {
                                dato.IdTipoContrato = (int)Helper.TipoContrato.Contrato;
                            }

                            var textoLog = "";
                            if (contrato != null)
                            {

                                textoLog = TextoLogContrato(contratoEdit, dato);

                                mensaje = "Contrato actualizado con éxito";
                                var existeContrato = db.Contrato.Where(c => c.NumeroContrato == dato.NumeroContrato && c.IdContrato != contrato.IdContrato).FirstOrDefault();
                                if (existeContrato == null)
                                {
                                    idContrato = dato.IdContrato;
                                    contrato.IdLicitacionOferta = dato.IdLicitacionOferta;
                                    contrato.Monto = dato.Monto;
                                    contrato.NumeroContrato = dato.NumeroContrato;
                                    contrato.MotivoEleccion = dato.MotivoEleccion;
                                    contrato.IdEmpresa = dato.IdEmpresa;
                                    contrato.IdBanco = dato.IdBanco;
                                    contrato.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                                    contrato.IdTipoImpuesto = dato.IdTipoImpuesto;
                                    contrato.TipoGarantia = dato.TipoGarantia;
                                    contrato.TasaMensual = dato.TasaMensual;
                                    contrato.TasaAnual = dato.TasaAnual;
                                    contrato.Plazo = dato.Plazo;
                                    contrato.FechaInicio = dato.FechaInicio;
                                    contrato.FechaTermino = dato.FechaTermino;
                                    contrato.IdTipoMoneda=dato.IdTipoMoneda;
                                    //contrato.Descripcion = dato.Descripcion;
                                    db.SaveChanges();
                                    //Actualizar descripcion
                                    //UpdateDescripcion(idContrato);
                                }
                                else
                                {
                                    swTran = false;
                                    mensaje = "Numero Contrato ya existe, revise sus datos";
                                }
                            }
                            else
                            {
                                mensaje = "Contrato creado con éxito";
                                var existeContrato = db.Contrato.Where(c => c.NumeroContrato == dato.NumeroContrato).FirstOrDefault();
                                if (existeContrato == null)
                                {
                                    var addContrato = new Contrato();
                                    addContrato.IdTipoContrato = dato.IdTipoContrato;
                                    addContrato.IdLicitacionOferta = dato.IdLicitacionOferta;
                                    addContrato.Monto = dato.Monto;
                                    addContrato.NumeroContrato = dato.NumeroContrato;
                                    addContrato.MotivoEleccion = dato.MotivoEleccion;
                                    addContrato.IdEmpresa = dato.IdEmpresa;
                                    addContrato.IdBanco = dato.IdBanco;
                                    addContrato.IdTipoFinanciamiento = dato.IdTipoFinanciamiento;
                                    addContrato.IdTipoImpuesto = dato.IdTipoImpuesto;
                                    addContrato.TipoGarantia = dato.TipoGarantia;
                                    addContrato.TasaMensual = dato.TasaMensual;
                                    addContrato.TasaAnual = dato.TasaAnual;
                                    addContrato.Plazo = dato.Plazo;
                                    addContrato.FechaInicio = Convert.ToDateTime(dato.FechaInicio);
                                    addContrato.FechaTermino = Convert.ToDateTime(dato.FechaTermino);
                                    addContrato.IdEstado = (int)Helper.Estado.ConCreado;
                                    addContrato.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                    addContrato.FechaRegistro = DateTime.Now;
                                    addContrato.IdTipoMoneda = dato.IdTipoMoneda;
                                    addContrato.Descripcion = dato.Descripcion;
                                    db.Contrato.Add(addContrato);
                                    db.SaveChanges();

                                    idContrato = addContrato.IdContrato;
                                }
                                else
                                {
                                    swTran = false;
                                    mensaje = "Numero Contrato ya existe, revise sus datos";
                                }
                            }

                            

                            if (swTran)
                            {
                                /*si es de oferta grabo los activos de la licitacion*/
                                if (existeOferta == "S")
                                {
                                    var activo = db.LicitacionActivo.Where(c => c.IdLicitacion == oferta.IdLicitacion).ToList();
                                    foreach (var ac in activo)
                                    {
                                        var existeActivo = db.ContratoActivo.Where(c => c.IdContrato == idContrato && c.IdActivo == ac.IdActivo).FirstOrDefault();
                                        if (existeActivo == null)
                                        {
                                            var addActivo = new ContratoActivo();
                                            addActivo.IdContrato = idContrato;
                                            addActivo.IdActivo = ac.IdActivo;
                                            db.ContratoActivo.Add(addActivo);
                                            db.SaveChanges();

                                            /*db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", (int)Helper.Estado.ActEnContrato, ac.IdActivo);
                                            db.SaveChanges();*/
                                        }
                                    }

                                    db.Database.ExecuteSqlCommand("UPDATE Licitacion SET IdEstado = {0} WHERE IdLicitacion = {1}", (int)Helper.Estado.LicContrato, oferta.IdLicitacion);
                                    db.SaveChanges();

                                }

                                /*registro log contrato*/
                                GrabaLogContrato(idContrato, 1, textoLog);
                                dbContextTransaction.Commit();
                                showMessageString = new { Estado = 0, Mensaje = mensaje, idContrato = idContrato };
                            }
                            else {
                                dbContextTransaction.Rollback();
                                showMessageString = new { Estado = 100, Mensaje = mensaje };
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteContrato(int idContrato)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbContrato = db.Contrato.Find(idContrato);

                    if (dbContrato != null)
                    {
                        if (dbContrato.IdEstado == 30)
                        {
                            var amortizacion = db.Contrato_Amortizacion.Where(c => c.IdContrato == idContrato).FirstOrDefault();
                            if (amortizacion != null)
                            {
                                var detAmortizacion = db.Contrato_DetAmortizacion.Where(c => c.IdContratoAmortizacion == amortizacion.IdContratoAmortizacion).ToList();
                                db.Contrato_DetAmortizacion.RemoveRange(detAmortizacion);
                                db.Contrato_Amortizacion.Remove(amortizacion);
                                db.SaveChanges();
                            }                            
                            var dbArchivo = (from doc in db.ContratoActivoDocumento
                                             join of in db.ContratoActivo on doc.IdContratoActivo equals of.IdContratoActivo
                                             where of.IdContrato == dbContrato.IdContrato
                                             select new
                                             {
                                                 doc.IdContratoActivoDocumento,
                                                 of.IdContrato,
                                                 of.IdContratoActivo,
                                                 doc.UrlDocumento
                                             }).ToList();
                            foreach (var arc in dbArchivo)
                            {
                                var archivo = Server.MapPath(arc.UrlDocumento);
                                if (System.IO.File.Exists(archivo))
                                {
                                    System.IO.File.Delete(archivo);
                                }
                                db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivoDocumento WHERE IdContratoActivoDocumento = {0}", arc.IdContratoActivoDocumento);
                                db.SaveChanges();
                            }
                            /*si viene de licitacion devuelvo el activo a la licitación, caso contrario lo dejo disponible*/
                            var dbActivo = db.ContratoActivo.Where(c => c.IdContrato == idContrato);
                            if (dbContrato.IdLicitacionOferta != null)
                            {
                                var oferta = db.LicitacionOferta.Find(dbContrato.IdLicitacionOferta);
                                if (oferta != null)
                                {
                                    db.Database.ExecuteSqlCommand("UPDATE Licitacion SET IdEstado = {0} WHERE IdLicitacion = {1}", (int)Helper.Estado.LicFinalizada, oferta.IdLicitacion);
                                    db.SaveChanges();
                                    /*foreach (var act in dbActivo)
                                    {
                                        db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", Helper.Estado.ActLicitacion, act.IdActivo);
                                        db.SaveChanges();
                                    }*/
                                }
                            }
                            else
                            {
                                /*foreach (var act in dbActivo)
                                {
                                    db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", Helper.Estado.ActDisponible, act.IdActivo);
                                    db.SaveChanges();
                                }*/
                            }
                            db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivo WHERE IdContrato = {0}", dbContrato.IdContrato);
                            db.SaveChanges();

                            db.Contrato.Remove(dbContrato);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = "Registro Eliminado con exito" };
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "No exposible eliminar el contrato seleccionado" };
                        }
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Se ha producido un error, intentelo nuevamente" };
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                }

                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ActivarContrato(int idContrato,int IdFamilia, string descripcion)
        {
            dynamic showMessageString = string.Empty;
            var mensajeError = "Existen Datos Incompletos";
            var contrato = db.Contrato.Find(idContrato);
            var activos = db.ContratoActivo.Where(c => c.IdContrato == idContrato).Count();
            var valido = true;
            if (contrato.TipoFinanciamiento.IdTipoContrato == (int)Helper.TipoContrato.Contrato && contrato.IdTipoFinanciamiento != (int)Helper.TipoFinanciamiento.EstructuradoConGarantia) {
                activos = 1;
                valido = true;
            }
            else
            {
                if (contrato.TipoGarantia== null || contrato.TipoGarantia=="")
                {
                    valido = false;
                    mensajeError = "Debe Indicar el Tipo de Garantía";
                }
            }
            if (valido ==true)
            {
                contrato.IdEstado = (int)Helper.Estado.ConActivo;
                contrato.Descripcion = descripcion;
                contrato.IdFamilia = IdFamilia;
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Contrato Activado Exitosamente" };
            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = mensajeError };
            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        /*Guarda log contrato*/
        private bool GrabaLogContrato(int idContrato, int idTipoLog, string textoLog)
        {
            //tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            var dbTipoLog = db.TipoLog.Find(idTipoLog);
            var dbContrato = db.Contrato.Find(idContrato);

            if (dbContrato != null && textoLog!=null && textoLog!="")
            {
                if(dbContrato.IdEstado > (int)Helper.Estado.ConCreado) { 
                    var nombreLog = "Cambios realizados en: " + dbTipoLog.NombreTipoLog+" ("+ textoLog+" )";
                    var addLog = new ContratoLog();
                    addLog.IdContrato = dbContrato.IdContrato;
                    addLog.IdTipoLog = dbTipoLog.IdTipoLog;
                    addLog.NombreLog = nombreLog;
                    addLog.IdUsuarioResgistro = (int)seguridad.IdUsuario;
                    addLog.FechaRegistro = DateTime.Now;
                    db.ContratoLog.Add(addLog);
                    db.SaveChanges();
                }
            }


            return true;
        }
        #endregion

        #region Contrato Credito
        public ActionResult ListaContratoCredito()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
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

                var tipoFinancimiento = (from e in db.TipoFinanciamiento
                                         where e.Activo == true
                                         select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoFinancimiento = new SelectList(tipoFinancimiento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoFinancimiento"] = listaTipoFinancimiento;

                return View();
            }
        }

        public ActionResult RegistrarContratoCredito(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var contrato = (from c in db.Contrato
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    ExisteContrato = "S"
                                }).FirstOrDefault();
                if (contrato == null)
                {
                    contrato = new ContratoViewModel();
                    contrato.IdContrato = idContrato;
                    contrato.ExisteContrato = "N";
                }
                return View(contrato);
            }
        }


        public ActionResult AddContratoCredito(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from c in db.Contrato.ToList()
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    IdLicitacionOferta = c.IdLicitacionOferta,
                                    EsLicitacion = (c.IdLicitacionOferta != null) ? "SI" : "NO",
                                    MotivoEleccion = c.MotivoEleccion,
                                    TipoGarantia = c.TipoGarantia,
                                    IdEmpresa = c.IdEmpresa,
                                    IdBanco = c.IdBanco,
                                    IdTipoFinanciamiento = c.IdTipoFinanciamiento,
                                    NumeroContrato = c.NumeroContrato,
                                    TasaMensual = c.TasaMensual,
                                    TasaAnual = c.TasaAnual,
                                    Plazo = c.Plazo,
                                    Monto = c.Monto,
                                    FechaInicio = c.FechaInicio,
                                    FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                    FechaTermino = c.FechaTermino,
                                    FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                    IdEstado = c.IdEstado,
                                    ExisteContrato = "S",
                                    TituloBoton = "Actualizar Contrato",
                                    IdTipoMoneda = c.IdTipoMoneda,
                                    Descripcion = (c.Descripcion != null) ? c.Descripcion : string.Empty
                                }
                                  ).FirstOrDefault();

                if (registro == null)
                {
                    registro = new ContratoViewModel();
                    registro.IdLicitacionOferta = 0;
                    registro.EsLicitacion = "NA";
                    registro.IdEmpresa = 0;
                    registro.IdBanco = 0;
                    registro.IdTipoFinanciamiento = 0;
                    registro.Monto = null;
                    registro.TasaMensual = null;
                    registro.TasaAnual = null;
                    registro.Plazo = null;
                    registro.IdEstado = 0;
                    registro.ExisteContrato = "N";
                    registro.TituloBoton = "Grabar Contrato";
                    registro.IdTipoMoneda = 1;
                }
                //else {
                //    if (registro.Descripcion.Length == 0)
                //    {
                //        var activo = (from ca in db.ContratoActivo
                //                      join a in db.Activo on ca.IdActivo equals a.IdActivo
                //                      join f in db.Familia on a.IdFamilia equals f.IdFamilia into fw
                //                      from fv in fw.DefaultIfEmpty()
                //                      where ca.IdContrato == idContrato
                //                      select new { fv.NombreFamilia } into x
                //                      group x by new { x.NombreFamilia } into g
                //                      select new
                //                      {
                //                          cont = g.Count(),
                //                          g.Key.NombreFamilia
                //                      }).ToList();

                //        var desc = "";
                //        if (activo != null)
                //        {
                //            foreach (var a in activo)
                //            {
                //                desc += a.cont.ToString() + " " + ((a.NombreFamilia != null) ? a.NombreFamilia : "Familia No asociada") + ", ";



                //            }

                //        }

                //        registro.Descripcion = desc;
                //    }
                //}

                var licitacionOferta = db.LicitacionOferta.Where(c => c.IdLicitacionOferta == registro.IdLicitacionOferta).FirstOrDefault();
                var idLicitacion = 0;
                var idLicitacionOferta = 0;

                if (licitacionOferta != null)
                {
                    idLicitacion = licitacionOferta.IdLicitacion;
                    idLicitacionOferta = licitacionOferta.IdLicitacionOferta;

                    var licitacion = (from e in db.Licitacion
                                      where e.IdLicitacion == idLicitacion
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacionOferta == idLicitacionOferta
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }
                else
                {
                    var licitacion = (from e in db.Licitacion
                                      where e.IdEstado == (int)Helper.Estado.LicFinalizada && e.IdTipoFinanciamiento != (int)Helper.TipoContrato.Leasing
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacion == idLicitacion
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }


                var banco = (from e in db.Banco
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdBanco);
                ViewData["listaBanco"] = listaBanco;

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var tiposMoneda = (from e in db.TipoMoneda
                                   where e.Activo == true
                                   select new RetornoGenerico { Id = e.IdTipoMoneda, Nombre = e.NombreTipoMoneda }).OrderBy(c => c.Id).ToList();
                SelectList listaMonedas = new SelectList(tiposMoneda.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoMoneda);
                ViewData["listaMonedas"] = listaMonedas;

                var tipoCredito = (from e in db.TipoFinanciamiento
                                where e.Activo == true && e.IdTipoContrato == (int)Helper.TipoContrato.Contrato
                                select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoCredito = new SelectList(tipoCredito.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoFinanciamiento);
                ViewData["listaTipoCredito"] = listaTipoCredito;

                return View(registro);
            }
        }

        #endregion

        #region Activo
        public ActionResult ListaActivoContrato_Read(int idContrato)
        {

            var registro = (from ac in db.Activo
                            join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
                            join con in db.Contrato on rel.IdContrato equals con.IdContrato
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            //join pr in db.Proveedor on ac.IdProveedor equals pr.IdProveedor into prw
                            //from prv in prw.DefaultIfEmpty()
                            where rel.IdContrato == idContrato
                            select new ActivoViewModel
                            {
                                IdContratoActivo = rel.IdContratoActivo,
                                IdActivo = ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                NombreCuenta = ac.IdCuenta,
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                Motor = ac.Motor,
                                Chasis = ac.Chasis,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NombreProveedor = ac.NombreProveedor,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente,
                                Editable = (con.IdLicitacionOferta != null) ? false : true
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalAsociarActivo(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var contratoActivo = new ContratoActivo();
                contratoActivo.IdContrato = idContrato;

                return PartialView(contratoActivo);
            }
        }

        public ActionResult ListaActivoAsociar_Read(int idContrato, string numeroActivo, string codigoActivo)
        {
            var idEmpresa = 0;
            var contrato = db.Contrato.Find(idContrato);
            if (contrato != null)
            {
                idEmpresa = contrato.IdEmpresa;
            }

            var activoContrato = db.ContratoActivo.Where(c=>c.Contrato.IdEstado!=(int)Helper.Estado.ConFinalizado).AsEnumerable().ToList();
            if (activoContrato.Count() == 0)
            {
                activoContrato = new List<ContratoActivo>();
            }

            var registro = (from ac in db.Activo.ToList()
                            join em in db.Empresa.ToList() on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia.ToList() on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            //join pr in db.Proveedor on ac.IdProveedor equals pr.IdProveedor into prw
                            //from prv in prw.DefaultIfEmpty()
                            where ac.NumeroInterno == ((numeroActivo != "") ? numeroActivo : ac.NumeroInterno)
                                && ac.CodSoftland == ((codigoActivo != "") ? codigoActivo : ac.CodSoftland)
                                && ac.IdEmpresa == idEmpresa
                                && ac.IdEstado == (int)Helper.Estado.ActDisponible
                                && activoContrato.Where(x => x.IdActivo == ac.IdActivo).Count() == 0
                            select new ActivoViewModel
                            {
                                IdActivo = ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = (fv != null) ? fv.NombreFamilia : string.Empty,
                                NombreCuenta = "",
                                Descripcion = ac.Descripcion,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                Motor = ac.Motor,
                                Chasis = ac.Chasis,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NombreProveedor = "",//(prv != null) ? prv.NombreProveedor : string.Empty,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult AsociarActivo(int idContrato, int[] activos)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null && !seguridad.TienePermiso("RegistrarLicitacion", Helper.TipoAcceso.Crear))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            var textoLog = "";
                            foreach (int ac in activos)
                            {
                                var existeActivo = db.Activo.Where(c => c.IdActivo == ac && c.IdEstado == (int)Helper.Estado.ActDisponible).FirstOrDefault();
                                if (existeActivo != null)
                                {
                                    var addActivo = new ContratoActivo();
                                    addActivo.IdContrato = idContrato;
                                    addActivo.IdActivo = ac;
                                    db.ContratoActivo.Add(addActivo);
                                    db.SaveChanges();

                                    /*cambio el estado del activo*/
                                    /*existeActivo.IdEstado = (int)Helper.Estado.ActEnContrato;
                                    db.SaveChanges();*/

                                    /*log de cambios*/
                                    textoLog += " Nuevo activo: "+addActivo.Activo.NumeroInterno + "; ";
                                }
                            }

                            /*registro log contrato*/
                            GrabaLogContrato(idContrato, 4, textoLog);
                            //Actualizar descripcion
                            UpdateDescripcion(idContrato);
                            var mensaje = "";
                            mensaje = "Asociación realizada con éxito";

                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }
        public void UpdateDescripcion(int IdContrato)
        {
            var contrato = db.Contrato.Find(IdContrato);
            if (contrato.IdEstado == 30)
            {
                var activo = (from ca in db.ContratoActivo
                              join a in db.Activo on ca.IdActivo equals a.IdActivo
                              
                              where ca.IdContrato == IdContrato
                              select new { a.DesGrupo } into x
                              group x by new { x.DesGrupo } into g
                              select new
                              {
                                  cont = g.Count(),
                                  g.Key.DesGrupo
                              }).ToList();

                var desc = "";
                if (activo != null)
                {
                    foreach (var a in activo)
                    {
                        desc += a.cont.ToString() + " " + ((a.DesGrupo != null) ? a.DesGrupo : "") + ", ";
                    }
                }
                contrato.Descripcion = (desc.Length>0)?desc.Substring(0, desc.Length - 2) :desc;
                db.SaveChanges();
            }
        }
        [HttpPost]
        public JsonResult DeleteAsociacionActivo(int idContratoActivo)
        {
            dynamic showMessageString = string.Empty;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dbContratoActivo = db.ContratoActivo.Find(idContratoActivo);
                    var IdContrato = dbContratoActivo.IdContrato;
                    if (dbContratoActivo != null)
                    {

                        db.Database.ExecuteSqlCommand("UPDATE Activo SET IdEstado = {0} WHERE IdActivo = {1}", (int)Helper.Estado.ActDisponible, dbContratoActivo.IdActivo);
                        db.SaveChanges();


                        /*registro log contrato*/
                        var textoLog = "";
                        textoLog += " Elimina activo: " + dbContratoActivo.Activo.NumeroInterno;
                        GrabaLogContrato(dbContratoActivo.IdContrato, 4, textoLog);

                        var dbArchivo = (from doc in db.ContratoActivoDocumento
                                         join of in db.ContratoActivo on doc.IdContratoActivo equals of.IdContratoActivo
                                         where of.IdContratoActivo == dbContratoActivo.IdContratoActivo
                                         select new
                                         {
                                             doc.IdContratoActivoDocumento,
                                             of.IdContrato,
                                             of.IdContratoActivo,
                                             doc.UrlDocumento
                                         }).ToList();
                        foreach (var arc in dbArchivo)
                        {
                            var archivo = Server.MapPath(arc.UrlDocumento);
                            if (System.IO.File.Exists(archivo))
                            {
                                System.IO.File.Delete(archivo);
                            }

                            db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivoDocumento WHERE IdContratoActivoDocumento = {0}", arc.IdContratoActivoDocumento);
                            db.SaveChanges();
                        }

                        db.ContratoActivo.Remove(dbContratoActivo);
                        db.SaveChanges();
                        //actualizar descripción
                        UpdateDescripcion(IdContrato);

                        dbContextTransaction.Commit();
                        showMessageString = new { Estado = 0, Mensaje = "Asociación eliminada con exito" };
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Se ha producido un error, intentelo nuevamente" };
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                }

                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Amortizacion
        public ActionResult AddAmortizacion(int idContrato,string soloVer)
        {            
            ContratoActivo contratoActivo=new ContratoActivo();
            var contrato = db.ContratoActivo.Where(c=>c.IdContrato==idContrato).FirstOrDefault();
            if (contrato != null)
            {
                contratoActivo = contrato;
            }
            else
            {
                contratoActivo.IdContrato = idContrato;
            }
            ViewBag.soloVer = soloVer;
            return View(contratoActivo);
            
        }
        [HttpPost]
        public JsonResult DeleteAmortizacion(int IdContrato)
        {
            dynamic showMessageString = string.Empty;
            var contrato = db.Contrato.Find(IdContrato);
            var existeAmortizacion = db.Contrato_Amortizacion.Where(c => c.IdContrato == IdContrato).FirstOrDefault();
            if (existeAmortizacion != null)
            {
                var detAmortizacion = db.Contrato_DetAmortizacion.Where(c => c.IdContratoAmortizacion == existeAmortizacion.IdContratoAmortizacion).ToList();
                db.Contrato_DetAmortizacion.RemoveRange(detAmortizacion);               
                db.Contrato_Amortizacion.Remove(existeAmortizacion);
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Detalle Eliminado" };

            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = "No Existe amortización" };
            }
            

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetAmortizacion_Read(int idContrato)
        {
            var registro = (from ca in db.Contrato_Amortizacion.ToList()
                            join d in db.Contrato_DetAmortizacion.ToList() on ca.IdContratoAmortizacion equals d.IdContratoAmortizacion
                            where ca.IdContrato == idContrato
                            select new
                            {
                                d.IdContratoDetAmortizacion,
                                Anio = 0,
                                NombreMes = (d.Mes != null) ? d.Mes.NombreMes : string.Empty,
                                CortoPlazo = d.CortoPlazo,
                                LargoPlazo = d.LargoPlazo,
                                FechaPago = d.FechaPago,
                                FechaPagoStr = d.FechaPago.ToString("dd-MM-yyyy"),
                                Periodo = d.Periodo,
                                Cuota = d.Cuota,
                                IvaDiferido = d.IvaDiferido,
                                Intereses = d.Intereses,
                                Amortizacion = d.Amortizacion,
                                Saldo_Insoluto = d.Saldo_Insoluto,
                                Cpbtes = db.ComprobanteDetAmortizacion.Where(y=>y.IdContratoDetAmortizacion==d.IdContratoDetAmortizacion).ToList()
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportAmortizacion(int? IdContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var contrato = db.Contrato.Find(IdContrato);
                return View(contrato);
            }
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportaPlanillaAmortizacion(int IdContrato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            List<string> data = new List<string>();
            if (archivo != null)
            {
                //guardar amortizacion
                Contrato_Amortizacion contratoAmortizacion = new Contrato_Amortizacion();
                var existeAmortizacion = db.Contrato_Amortizacion.Where(c => c.IdContrato == IdContrato).FirstOrDefault();
                if (existeAmortizacion != null)
                {
                    contratoAmortizacion = existeAmortizacion;
                }
                else
                {
                    contratoAmortizacion.IdContrato = IdContrato;
                    contratoAmortizacion.FechaRegistro = DateTime.Now;
                    contratoAmortizacion.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                    db.Contrato_Amortizacion.Add(contratoAmortizacion);
                    db.SaveChanges();
                }
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
                    var artistAlbums = from a in excelFile.Worksheet<DetAmortizacionViewModel>(sheetName) select a;

                    var ivaObligacion = 1.19;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.Periodo >= 0)
                            {
                                var periodo = db.Contrato_DetAmortizacion.Where(c => c.IdContratoAmortizacion == contratoAmortizacion.IdContratoAmortizacion && c.Periodo == a.Periodo).FirstOrDefault();
                                if (periodo != null)
                                {
                                    periodo.CortoPlazo = a.CortoPlazo;
                                    periodo.LargoPlazo = a.LargoPlazo;
                                    periodo.FechaPago = a.FechaPago;
                                    periodo.Cuota = a.Cuota;
                                    periodo.IvaDiferido = a.IvaDiferido;
                                    periodo.Intereses = a.Intereses;
                                    periodo.Amortizacion = a.Amortizacion;
                                    periodo.Saldo_Insoluto = a.Saldo_Insoluto;
                                    periodo.Obligacion = a.Cuota * ivaObligacion;
                                    periodo.EsMes = 0;
                                    db.SaveChanges();                                    
                                }
                                else
                                {
                                    Contrato_DetAmortizacion detPeriodo= new Contrato_DetAmortizacion();
                                    detPeriodo.IdContratoAmortizacion = contratoAmortizacion.IdContratoAmortizacion;
                                    detPeriodo.Periodo = a.Periodo;
                                    detPeriodo.CortoPlazo = a.CortoPlazo;
                                    detPeriodo.LargoPlazo = a.LargoPlazo;
                                    detPeriodo.FechaPago = a.FechaPago;
                                    detPeriodo.Cuota = a.Cuota;
                                    detPeriodo.IvaDiferido = a.IvaDiferido;
                                    detPeriodo.Intereses = a.Intereses;
                                    detPeriodo.Amortizacion = a.Amortizacion;
                                    detPeriodo.Saldo_Insoluto = a.Saldo_Insoluto;
                                    detPeriodo.Obligacion = a.Cuota * ivaObligacion;
                                    detPeriodo.EsMes = 0;
                                    db.Contrato_DetAmortizacion.Add(detPeriodo);
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.Periodo == 0) data.Add("<li> Nro es obligatorio</li>");
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
                    //calcular tir
                    var valTir = computeIRR(IdContrato, 0);
                    var amortizacion = db.Contrato_Amortizacion.Where(c => c.IdContrato == IdContrato).FirstOrDefault();
                    amortizacion.TasaMensual = valTir;
                    amortizacion.TasaAnual=Math.Pow(1+valTir, 12)-1;
                    db.SaveChanges();
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
        
        
        public double computeIRR(int IdContrato, int numOfFlows)
        {
            var amortizacion = db.Contrato_Amortizacion.Where(c => c.IdContrato == IdContrato).FirstOrDefault();
            var detAmortizacion = db.Contrato_DetAmortizacion.Where(c => c.IdContratoAmortizacion == amortizacion.IdContratoAmortizacion).OrderBy(c=>c.Periodo).ToList();
            numOfFlows = detAmortizacion.Max(c => c.Periodo);
            var detFlow = detAmortizacion.Where(c => c.Periodo < numOfFlows).OrderBy(c => c.Periodo).ToList();
            double LOW_RATE = 0.01;
            double HIGH_RATE = 0.5;
            var MAX_ITERATION = 1000;
            double PRECISION_REQ = 0.00000001;
            int i = 0, j = 0;
            double m = 0.0;
            double old = 0.00;
            double nuevo = 0.00;
            double oldguessRate = LOW_RATE;
            double newguessRate = LOW_RATE;
            double guessRate = LOW_RATE;
            double lowGuessRate = LOW_RATE;
            double highGuessRate = HIGH_RATE;
            double npv = 0.0;
            double denom = 0.0;
            for (i = 0; i < MAX_ITERATION; i++)
            {
                npv = 0.00;
                var sumDenom = 0.00;
                j = 0;
                foreach(var cf in detAmortizacion)
                {
                    denom = Math.Pow((1 + guessRate), cf.Periodo);
                    sumDenom = sumDenom + denom;
                    var k = (cf.Cuota / denom);
                    npv = npv + k;
                    j++;
                }
                /* Stop checking once the required precision is achieved */
                if ((npv > 0) && (npv < PRECISION_REQ))
                    break;
                if (old == 0)
                    old = npv;
                else
                    old = nuevo;
                nuevo = npv;
                if (i > 0)
                {
                    if (old < nuevo)
                    {
                        if (old < 0 && nuevo < 0)
                            highGuessRate = newguessRate;
                        else
                            lowGuessRate = newguessRate;
                    }
                    else
                    {
                        if (old > 0 && nuevo > 0)
                            lowGuessRate = newguessRate;
                        else
                            highGuessRate = newguessRate;
                    }
                }
                oldguessRate = guessRate;
                guessRate = (lowGuessRate + highGuessRate) / 2;
                newguessRate = guessRate;
            }
            return guessRate;
        }
        #endregion

        #region Documento Contrato
        public ActionResult AddDocumentoContrato(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from ac in db.Activo
                                join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
                                join con in db.Contrato on rel.IdContrato equals con.IdContrato
                                where rel.IdContrato == idContrato
                                select new ContratoActivoViewModel
                                {
                                    IdContratoActivo = rel.IdContratoActivo,
                                    IdActivo = ac.IdActivo,
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    Valor = ac.Valor,
                                    Familia = (ac.Familia.NombreFamilia != null) ? ac.Familia.NombreFamilia : string.Empty,
                                    IdTipoContrato = con.IdTipoContrato
                                }).AsEnumerable().ToList();

                var tipoDocumento = (from e in db.TipoDocumento
                             where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.ActivoContrato
                             select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;

                //verificacion activar contrato
                double sumaActivo = 0;
                double sumaAmortizacion = 0;
                if (registro != null)
                {
                    sumaActivo = Convert.ToDouble(registro.Sum(c => c.Valor));
                }
                ViewData["puedeActivar"] = "N";
                ViewData["urlRetorno"] = "/Contrato/ListaContratoCredito";
                var conActivo = db.Contrato.Where(c => c.IdContrato == idContrato).FirstOrDefault();
                ViewBag.IdTipoContrato = 0;
                if (conActivo != null) {
                    var amortizacion = (from a in db.Contrato_Amortizacion
                                        join de in db.Contrato_DetAmortizacion on a.IdContratoAmortizacion equals de.IdContratoAmortizacion
                                        where a.IdContrato == idContrato
                                        select new { de.Amortizacion }).ToList();
                    if (amortizacion != null)
                    {
                        sumaAmortizacion = Convert.ToDouble(amortizacion.Sum(c => c.Amortizacion));
                    }
                    sumaAmortizacion = Math.Round(sumaAmortizacion, 0);
                    ViewBag.SumaActivo = sumaActivo;
                    if (conActivo.TipoFinanciamiento.IdTipoContrato == (int)Helper.TipoContrato.Contrato && conActivo.IdTipoFinanciamiento != (int)Helper.TipoFinanciamiento.EstructuradoConGarantia) {
                        ViewBag.SumaActivo = conActivo.Monto;
                    }
                    ViewBag.SumaAmortizacion = sumaAmortizacion;
                    ViewBag.MontoContrato = conActivo.Monto;

                    ViewBag.IdTipoContrato = conActivo.IdTipoContrato;
                    if (conActivo.IdEstado == (int)Helper.Estado.ConCreado) { 
                        ViewData["puedeActivar"] = "S";
                    }
                    if (conActivo.IdTipoContrato == (int)Helper.TipoContrato.Leasing) {
                        ViewData["urlRetorno"] = "/Contrato/ListaContratoLeasing";
                    }
                }

                var contrato = db.Contrato.Find(idContrato);
                var famContrato = (from ca in db.ContratoActivo
                                   join ac in db.Activo on ca.IdActivo equals ac.IdActivo
                                   join f in db.Familia on ac.IdFamilia equals f.IdFamilia
                                  where ca.IdContrato==contrato.IdContrato
                                  select new { f.IdFamilia,f.NombreFamilia})
                                  .GroupBy(c => new { c.IdFamilia,c.NombreFamilia})
                                  .Select(c => new RetornoGenerico{ Id=c.Key.IdFamilia,Nombre=c.Key.NombreFamilia}).ToList();
                SelectList listaFamilia = new SelectList(famContrato.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaFamilia"] = listaFamilia;

                return View(contrato);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarContratoDocumento(ContratoDocumento dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            ContratoDocumento addDocumento = new ContratoDocumento();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null)
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";

                            if (archivo != null)
                            {
                                var contrato = db.Contrato.Where(c => c.IdContrato == dato.IdContrato).FirstOrDefault();
                                var pathDocumento = "";
                                var fileName = dato.IdTipoDocumento.ToString()+'_'+Path.GetFileName(archivo.FileName);
                                var carpeta = "Contrato/"+ contrato.IdContrato;
                                var pathData = "~/App_Data";
                                var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta); ;
                                if (!Directory.Exists(pathCarpeta))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                }
                                string carpetaSolicitud = contrato.IdContrato.ToString();
                                var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                if (!Directory.Exists(pathCarpetaSolicitud))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                }
                                pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                archivo.SaveAs(physicalPath);

                                addDocumento.IdContrato = (int)contrato.IdContrato;
                                addDocumento.IdTipoDocumento = dato.IdTipoDocumento;
                                addDocumento.FechaRegistro = DateTime.Now;
                                addDocumento.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addDocumento.NombreOriginal = fileName;
                                addDocumento.UrlDocumento = pathDocumento;
                                db.ContratoDocumento.Add(addDocumento);
                                db.SaveChanges();
                                mensaje = "Archivo Cargado con exito";

                                //registro log contrato
                                var textoLog = "";
                                textoLog += " Agrega Documento: " + addDocumento.NombreOriginal;
                                GrabaLogContrato(contrato.IdContrato, 3, textoLog);
                            }

                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            //return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaContratoDocumento_Read(int idContrato)
        {
            var registro = (from c in db.ContratoDocumento
                            join td in db.TipoDocumento on c.IdTipoDocumento equals td.IdTipoDocumento
                            where c.IdContrato == idContrato
                            select new 
                            {
                                IdContratoDocumento = c.IdContratoDocumento,
                                IdContrato = idContrato,
                                NombreTipoDocumento = td.NombreTipoDocumento,
                                NombreOriginal = c.NombreOriginal,
                                UrlDocumento = c.UrlDocumento,
                                TieneEliminar=((int)seguridad.IdUsuario==c.IdUsuarioRegistro)?true:false
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteContratoDocumento(int idContratoDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.ContratoDocumento.Find(idContratoDocumento);

            var contrato = (from c in db.ContratoDocumento
                            where c.IdContratoDocumento == idContratoDocumento
                            select new { c.IdContrato,c.NombreOriginal }).FirstOrDefault();

            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            var textoLog = "";
            textoLog += " Elimina Documento: " + contrato.NombreOriginal;

            db.Database.ExecuteSqlCommand("DELETE FROM ContratoDocumento WHERE IdContratoDocumento = {0}", idContratoDocumento);
            db.SaveChanges();


            //registro log contrato
            if(contrato != null) { 
                GrabaLogContrato(contrato.IdContrato, 3, textoLog);
            }


            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        } 

        #endregion

        #region Documento Activo
        /*public ActionResult AddDocumentoLeasing(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("RegistrarContrato", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var registro = (from ac in db.Activo
                                join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
                                join con in db.Contrato on rel.IdContrato equals con.IdContrato
                                where rel.IdContrato == idContrato
                                select new ContratoActivoViewModel
                                {
                                    IdContratoActivo = rel.IdContratoActivo,
                                    IdActivo = ac.IdActivo,
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    Valor = ac.Valor,
                                    Familia = (ac.Familia.NombreFamilia != null) ? ac.Familia.NombreFamilia : string.Empty,
                                    Archivos = (from d in db.ContratoActivoDocumento
                                                join t in db.TipoDocumento on d.IdTipoDocumento equals t.IdTipoDocumento
                                                where d.IdContratoActivo == rel.IdContratoActivo
                                                select new ContratoDocumentoViewModel {
                                                    IdContratoActivoDocumento = d.IdContratoActivoDocumento,
                                                    NombreTipoDocumento = t.NombreTipoDocumento,
                                                    UrlDocumento = d.UrlDocumento,
                                                    NombreOriginal = d.NombreOriginal
                                                }).ToList(),
                                    IdTipoContrato=con.IdTipoContrato
                                }).AsEnumerable().ToList();
                
                var tipoDocumento = (from e in db.TipoDocumento
                             where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.ActivoContrato
                             select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;

                //verificacion activar contrato
                double sumaActivo = 0;
                double sumaAmortizacion = 0;
                if (registro != null)
                {
                    sumaActivo = Convert.ToDouble(registro.Sum(c => c.Valor));
                }
                ViewData["puedeActivar"] = "N";
                ViewData["urlRetorno"] = "/Contrato/ListaContratoCredito";
                var conActivo = db.Contrato.Where(c => c.IdContrato == idContrato).FirstOrDefault();
                ViewBag.IdTipoContrato = 0;
                if (conActivo != null) {
                    var amortizacion = (from a in db.Contrato_Amortizacion
                                        join de in db.Contrato_DetAmortizacion on a.IdContratoAmortizacion equals de.IdContratoAmortizacion
                                        where a.IdContrato == idContrato
                                        select new { de.Amortizacion }).ToList();
                    if (amortizacion != null)
                    {
                        sumaAmortizacion = Convert.ToDouble(amortizacion.Sum(c => c.Amortizacion));
                    }
                    sumaAmortizacion = Math.Round(sumaAmortizacion, 0);
                    ViewBag.SumaActivo = sumaActivo;
                    if (conActivo.TipoFinanciamiento.IdTipoContrato == (int)Helper.TipoContrato.Contrato && conActivo.IdTipoFinanciamiento != (int)Helper.TipoFinanciamiento.EstructuradoConGarantia) {
                        ViewBag.SumaActivo = conActivo.Monto;
                    }
                    ViewBag.SumaAmortizacion = sumaAmortizacion;
                    ViewBag.MontoContrato = conActivo.Monto;

                    ViewBag.IdTipoContrato = conActivo.IdTipoContrato;
                    if (conActivo.IdEstado == (int)Helper.Estado.ConCreado) { 
                        ViewData["puedeActivar"] = "S";
                    }
                    if (conActivo.IdTipoContrato == (int)Helper.TipoContrato.Leasing) {
                        ViewData["urlRetorno"] = "/Contrato/ListaContratoLeasing";
                    }
                }

                return View(registro);
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult GrabarContratoDocumento(ContratoActivoDocumento dato, HttpPostedFileBase archivo)
        {
            dynamic showMessageString = string.Empty;
            //validar que los datos ingresados sean correctos
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            ContratoActivoDocumento addDocumento = new ContratoActivoDocumento();
            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null)
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                if (ModelState.IsValid)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var mensaje = "";

                            if (archivo != null)
                            {
                                var contrato = db.ContratoActivo.Where(c => c.IdContratoActivo == dato.IdContratoActivo).FirstOrDefault();
                                var pathDocumento = "";
                                var fileName = dato.IdTipoDocumento.ToString()+'_'+Path.GetFileName(archivo.FileName);
                                var carpeta = "Contrato/"+ contrato.IdContrato;
                                var pathData = "~/App_Data";
                                var pathCarpeta = Path.Combine(Server.MapPath(pathData), carpeta); ;
                                if (!Directory.Exists(pathCarpeta))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpeta);
                                }
                                string carpetaSolicitud = contrato.IdContratoActivo.ToString();
                                var pathCarpetaSolicitud = Path.Combine(pathCarpeta, carpetaSolicitud);
                                if (!Directory.Exists(pathCarpetaSolicitud))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(pathCarpetaSolicitud);
                                }
                                pathDocumento = pathData + "/" + carpeta + "/" + carpetaSolicitud + "/" + fileName;
                                var physicalPath = Path.Combine(pathCarpetaSolicitud, fileName);
                                archivo.SaveAs(physicalPath);

                                addDocumento.IdContratoActivo = (int)contrato.IdContratoActivo;
                                addDocumento.IdTipoDocumento = dato.IdTipoDocumento;
                                addDocumento.FechaRegistro = DateTime.Now;
                                addDocumento.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                addDocumento.NombreOriginal = fileName;
                                addDocumento.UrlDocumento = pathDocumento;
                                db.ContratoActivoDocumento.Add(addDocumento);
                                db.SaveChanges();
                                mensaje = "Archivo Cargado con exito";

                                //registro log contrato
                                var textoLog = "";
                                textoLog += " Agrega Documento: " + addDocumento.NombreOriginal;
                                GrabaLogContrato(contrato.IdContrato, 3, textoLog);
                            }

                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 500, Mensaje = "Error: " + ex.Message };
                        }
                    }
                }
                else
                {
                    showMessageString = new { Estado = 103, Mensaje = "Se ha producido un error" };
                }
            }
            //return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaContratoDocumento_Read(int idContratoActivo)
        {
            var registro = (from c in db.ContratoActivoDocumento
                            join td in db.TipoDocumento on c.IdTipoDocumento equals td.IdTipoDocumento
                            where c.IdContratoActivo == idContratoActivo
                            select new ContratoDocumentoViewModel
                            {
                                IdContratoActivoDocumento = c.IdContratoActivoDocumento,
                                IdContratoActivo = idContratoActivo,
                                NombreTipoDocumento = td.NombreTipoDocumento,
                                NombreOriginal = c.NombreOriginal,
                                UrlDocumento = c.UrlDocumento
                            }).AsEnumerable().ToList();

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteContratoDocumento(int idContratoActivoDocumento)
        {
            dynamic showMessageString = string.Empty;
            var dbArchivo = db.ContratoActivoDocumento.Find(idContratoActivoDocumento);

            var contrato = (from c in db.ContratoActivoDocumento
                            join ca in db.ContratoActivo on c.IdContratoActivo equals ca.IdContratoActivo
                            where c.IdContratoActivoDocumento == idContratoActivoDocumento
                            select new { ca.IdContrato,c.NombreOriginal }).FirstOrDefault();

            var archivo = Server.MapPath(dbArchivo.UrlDocumento);
            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }
            var textoLog = "";
            textoLog += " Elimina Documento: " + contrato.NombreOriginal;

            db.Database.ExecuteSqlCommand("DELETE FROM ContratoActivoDocumento WHERE IdContratoActivoDocumento = {0}", idContratoActivoDocumento);
            db.SaveChanges();


            //registro log contrato
            if(contrato != null) { 
                GrabaLogContrato(contrato.IdContrato, 3, textoLog);
            }


            showMessageString = new { Estado = 0, Mensaje = "Archivo Eliminado" };

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        } */

        #endregion

        #region Vista contrato
        public ActionResult ContratoBuscar()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Acceder))
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

                var tipoFinancimiento = (from e in db.TipoFinanciamiento
                                         where e.Activo == true
                                         select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoFinancimiento = new SelectList(tipoFinancimiento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoFinancimiento"] = listaTipoFinancimiento;
                return View();
            }
        }

        public ActionResult ListaBuscarContrato_Read(int? idBanco, int? idEmpresa, int? idTipoFinanciamiento, string numeroContrato)
        {
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado.ToList() on c.IdEstado equals e.IdEstado
                            where c.IdEmpresa == ((idEmpresa != null) ? idEmpresa : c.IdEmpresa)
                            && c.IdBanco == ((idBanco != null) ? idBanco : c.IdBanco)
                            && c.IdTipoFinanciamiento == ((idTipoFinanciamiento != null) ? idTipoFinanciamiento : c.IdTipoFinanciamiento)
                            && c.NumeroContrato == ((numeroContrato != "") ? numeroContrato : c.NumeroContrato)
                            select new ContratoViewModel
                            {
                                IdContrato = c.IdContrato,
                                NumeroContrato = c.NumeroContrato,
                                RazonSocial = c.Empresa.RazonSocial,
                                NombreBanco = c.Banco.NombreBanco,
                                NombreTipoFinanciamiento = (c.TipoFinanciamiento != null) ? c.TipoFinanciamiento.NombreTipoFinanciamiento : string.Empty,
                                Plazo = c.Plazo,
                                Monto = c.Monto,
                                TasaAnual = c.TasaAnual,
                                TasaMensual = c.TasaMensual,
                                FechaInicio = c.FechaInicio,
                                FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                FechaTermino = c.FechaTermino,
                                FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                PuedeEliminar = (c.IdEstado != (int)Helper.Estado.ConCreado) ? false : true,
                                NombreEstado = e.NombreEstado,
                                Descripcion=c.Descripcion
                            }).AsEnumerable().ToList();

            //foreach (var reg in registro) {
            //    var activo = (from ca in db.ContratoActivo
            //                       join a in db.Activo on ca.IdActivo equals a.IdActivo
            //                      join f in db.Familia on a.IdFamilia equals f.IdFamilia into fw
            //                      from fv in fw.DefaultIfEmpty()
            //                  where ca.IdContrato == reg.IdContrato
            //                       select new { fv.NombreFamilia } into x
            //                       group x by new { x.NombreFamilia } into g
            //                       select new
            //                       {
            //                           cont = g.Count(),
            //                           g.Key.NombreFamilia
            //                       }).ToList();

            //    var desc = "";
            //    if (activo != null) {
            //        foreach (var a in activo) {
            //            desc += a.cont.ToString() + " " + ((a.NombreFamilia!=null)? a.NombreFamilia:"Familia No asociada") + ", ";                       

            //        }
                    
            //    }

            //    reg.Descripcion = desc;
            //}

            return Json(registro, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalVistaContrato(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                //var contrato = new ContratoViewModel();
                var contrato = (from c in db.Contrato
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    IdTipoContrato = c.IdTipoContrato,
                                    ExisteContrato = "S"
                                }).FirstOrDefault();
                if (contrato == null)
                {
                    contrato = new ContratoViewModel();
                    contrato.IdContrato = idContrato;
                    contrato.ExisteContrato = "N";
                }

                return View(contrato);
            }
        }

        public ActionResult VistaContrato(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var registro = (from c in db.Contrato.ToList()
                                where c.IdContrato == idContrato
                                select new ContratoViewModel
                                {
                                    IdContrato = c.IdContrato,
                                    IdTipoContrato = c.IdTipoContrato,
                                    IdLicitacionOferta = c.IdLicitacionOferta,
                                    EsLicitacion = (c.IdLicitacionOferta != null) ? "SI" : "NO",
                                    MotivoEleccion = c.MotivoEleccion,
                                    IdEmpresa = c.IdEmpresa,
                                    IdBanco = c.IdBanco,
                                    IdTipoImpuesto = c.IdTipoImpuesto,
                                    IdTipoFinanciamiento = c.IdTipoFinanciamiento,
                                    NumeroContrato = c.NumeroContrato,
                                    TasaMensual = c.TasaMensual,
                                    TasaAnual = c.TasaAnual,
                                    Plazo = c.Plazo,
                                    Monto = c.Monto,
                                    FechaInicio = c.FechaInicio,
                                    FechaInicioStr = c.FechaInicio.ToString("dd-MM-yyyy"),
                                    FechaTermino = c.FechaTermino,
                                    FechaTerminoStr = c.FechaTermino.ToString("dd-MM-yyyy"),
                                    IdEstado = c.IdEstado,
                                    ExisteContrato = "S",
                                    TituloBoton = "Actualizar Contrato",
                                    IdTipoMoneda=c.IdTipoMoneda,
                                    Descripcion=c.Descripcion,
                                    IdFamilia=c.IdFamilia
                                }
                                  ).FirstOrDefault();


                var licitacionOferta = db.LicitacionOferta.Where(c => c.IdLicitacionOferta == registro.IdLicitacionOferta).FirstOrDefault();
                var idLicitacion = 0;
                var idLicitacionOferta = 0;

                if (licitacionOferta != null)
                {
                    idLicitacion = licitacionOferta.IdLicitacion;
                    idLicitacionOferta = licitacionOferta.IdLicitacionOferta;

                    var licitacion = (from e in db.Licitacion
                                      where e.IdLicitacion == idLicitacion
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacionOferta == idLicitacionOferta
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }
                else
                {
                    var licitacion = (from e in db.Licitacion
                                      where e.IdEstado == (int)Helper.Estado.LicFinalizada && e.IdTipoFinanciamiento == (int)Helper.TipoContrato.Leasing
                                      select new RetornoGenerico { Id = e.IdLicitacion, Nombre = e.Autogenerado }).OrderBy(c => c.Id).ToList();
                    SelectList listaLicitacion = new SelectList(licitacion.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacion);
                    ViewData["listaLicitacion"] = listaLicitacion;

                    var oferta = (from e in db.LicitacionOferta
                                  join l in db.Licitacion on e.IdLicitacion equals l.IdLicitacion
                                  join b in db.Banco on e.IdBanco equals b.IdBanco
                                  where e.IdLicitacion == idLicitacion
                                  select new RetornoGenerico { Id = e.IdLicitacionOferta, Nombre = b.NombreBanco }).OrderBy(c => c.Id).ToList();
                    SelectList listaOferta = new SelectList(oferta.OrderBy(c => c.Nombre), "Id", "Nombre", idLicitacionOferta);
                    ViewData["listaOferta"] = listaOferta;
                }

                var tiposMoneda = (from e in db.TipoMoneda
                                   where e.Activo == true
                                   select new RetornoGenerico { Id = e.IdTipoMoneda, Nombre = e.NombreTipoMoneda }).OrderBy(c => c.Id).ToList();
                SelectList listaMonedas = new SelectList(tiposMoneda.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoMoneda);
                ViewData["listaMonedas"] = listaMonedas;

                var banco = (from e in db.Banco
                             where e.Activo == true
                             select new RetornoGenerico { Id = e.IdBanco, Nombre = e.NombreBanco }).OrderBy(c => c.Id).ToList();
                SelectList listaBanco = new SelectList(banco.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdBanco);
                ViewData["listaBanco"] = listaBanco;

                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var impuesto = (from e in db.TipoImpuesto
                                where e.Activo == true
                                select new RetornoGenerico { Id = e.IdTipoImpuesto, Nombre = e.NombreTipoImpuesto }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoImpuesto = new SelectList(impuesto.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoImpuesto);
                ViewData["listaTipoImpuesto"] = listaTipoImpuesto;

                var tipoCredito = (from e in db.TipoFinanciamiento
                                   where e.Activo == true && e.IdTipoContrato == (int)Helper.TipoContrato.Contrato
                                   select new RetornoGenerico { Id = e.IdTipoFinanciamiento, Nombre = e.NombreTipoFinanciamiento }).OrderBy(c => c.Id).ToList();
                SelectList listaTipoCredito = new SelectList(tipoCredito.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdTipoFinanciamiento);
                ViewData["listaTipoCredito"] = listaTipoCredito;

                var famContrato = (from ca in db.ContratoActivo
                                   join ac in db.Activo on ca.IdActivo equals ac.IdActivo
                                   join f in db.Familia on ac.IdFamilia equals f.IdFamilia
                                   where ca.IdContrato == registro.IdContrato
                                   select new { f.IdFamilia, f.NombreFamilia })
                                  .GroupBy(c => new { c.IdFamilia, c.NombreFamilia })
                                  .Select(c => new RetornoGenerico { Id = c.Key.IdFamilia, Nombre = c.Key.NombreFamilia }).ToList();
                SelectList listaFamilia = new SelectList(famContrato.OrderBy(c => c.Nombre), "Id", "Nombre",registro.IdFamilia);
                ViewData["listaFamilia"] = listaFamilia;

                return View(registro);
            }
        }

        public ActionResult VistaDocumento(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var tipoDocumento = (from e in db.TipoDocumento
                                     where e.Activo == true && e.IdCategoriaDocumento == (int)Helper.CategoriaDocumento.ActivoContrato
                                     select new RetornoGenerico { Id = e.IdTipoDocumento, Nombre = e.NombreTipoDocumento }).OrderBy(c => c.Id).ToList();
                //SelectList listaTipoDocumento = new SelectList(tipoDocumento.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaTipoDocumento"] = tipoDocumento;

                /*var registro = (from ac in db.Activo
                                join rel in db.ContratoActivo on ac.IdActivo equals rel.IdActivo
                                join con in db.Contrato on rel.IdContrato equals con.IdContrato
                                where rel.IdContrato == idContrato
                                select new ContratoActivoViewModel
                                {
                                    IdContratoActivo = rel.IdContratoActivo,
                                    IdActivo = ac.IdActivo,
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    Familia = (ac.Familia.NombreFamilia != null) ? ac.Familia.NombreFamilia : string.Empty,
                                    Archivos = (from d in db.ContratoActivoDocumento
                                                join t in db.TipoDocumento on d.IdTipoDocumento equals t.IdTipoDocumento
                                                where d.IdContratoActivo == rel.IdContratoActivo
                                                select new ContratoDocumentoViewModel
                                                {
                                                    IdContratoActivoDocumento = d.IdContratoActivoDocumento,
                                                    NombreTipoDocumento = t.NombreTipoDocumento,
                                                    UrlDocumento = d.UrlDocumento,
                                                    NombreOriginal = d.NombreOriginal
                                                }).ToList()
                                }).AsEnumerable().ToList();*/
                var registro = db.Contrato.Find(idContrato);

                return View(registro);
            }
        }

        public ActionResult VistaLog(int idContrato)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ContratoBuscar", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {

                var registro = (from l in db.ContratoLog.ToList()
                                join u in db.Usuario.ToList() on l.IdUsuarioResgistro equals u.IdUsuario
                                where l.IdContrato == idContrato
                                select new ContratoLogViewModel
                                {
                                    IdContratoLog = l.IdContratoLog,
                                    IdContrato = l.IdContrato,
                                    NombreLog = l.NombreLog,
                                    NombreUsuarioRegistro = u.NombreUsuario,
                                    FechaRegistroStr = l.FechaRegistro.ToString("dd-MM-yyyy HH:mm")
                                }).AsEnumerable().ToList();

                return View(registro);
            }
        }
        #endregion       
        public ActionResult Consolidado_Read(int? IdEmpresa,int? IdBanco,int? anio,int? IdMes, double? valorUf)
        {
            var inicioMes = "01-" + IdMes.ToString() + "-" + anio.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            var fecha = fechaFin.ToString();
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado on c.IdEstado equals e.IdEstado
                            join em in db.Empresa on c.IdEmpresa equals em.IdEmpresa
                            join tc in db.TipoContrato on c.IdTipoContrato equals tc.IdTipoContrato
                            where c.IdTipoContrato == 1 && c.FechaTermino>= fechaInicio
                            && ((IdEmpresa!=null)?em.IdEmpresa== IdEmpresa : true)
                            && ((IdBanco != null) ? c.IdBanco == IdBanco : true)
                            //&& ((anio != null) ? c.FechaTermino.Year == anio : true)
                            //&& ((IdMes != null) ? c.FechaTermino.Month == IdMes : true)
                            select new
                            {
                                em.IdEmpresa,
                                IdContrato = c.IdContrato,
                                IdTipoContrato = c.IdTipoContrato,
                                RazonSocial = em.RazonSocial,
                                Monto = (c.IdTipoMoneda==(int)Helper.TipoMoneda.UF)?(c.Monto*valorUf):c.Monto,
                                TasaMensual = c.TasaMensual,
                                c.TasaAnual,
                                Plazo = c.Plazo,
                                MontoTasaMensual = c.Monto * c.TasaMensual,
                                NombreTipoFinanciamiento = (c.TipoFinanciamiento != null) ? c.TipoFinanciamiento.NombreTipoFinanciamiento : string.Empty,
                                c.IdTipoMoneda
                            }).AsEnumerable().ToList();

            var tasaPorRegistro = (from c in registro
                                   select new
                                   {
                                       c.IdEmpresa,
                                       PorcMonto = c.Monto / registro.Where(x => x.IdEmpresa == c.IdEmpresa && x.IdTipoMoneda == c.IdTipoMoneda).Sum(x => x.Monto),
                                       c.RazonSocial,
                                       c.IdTipoMoneda,
                                       c.TasaAnual
                                   }).ToList();

            var totales = tasaPorRegistro.GroupBy(c => new { c.RazonSocial, c.IdEmpresa })
                .Select(c => new {
                    c.Key.RazonSocial,
                    c.Key.IdEmpresa,
                    TasaPromedio = c.Where(x => x.IdTipoMoneda == 1).Sum(x => x.PorcMonto * x.TasaAnual),
                    TasaPromedioUF = c.Where(x => x.IdTipoMoneda == 2).Sum(x => x.PorcMonto * x.TasaAnual),
                    CantidadReg = c.Count()
                });

            var listTasaPromedio = (from total in totales
                                    select new
                                    {
                                        total.RazonSocial,
                                        TasaPromedio = Math.Round((double)total.TasaPromedio, 2),
                                        total.CantidadReg,
                                        TasaPromedioUF = Math.Round((double)total.TasaPromedioUF, 2)
                                    }
                          ).ToList();
            return Json(listTasaPromedio, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Consolidado2_Read(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes, double? valorUf)
        {
            var inicioMes = "01-" + IdMes.ToString() + "-" + anio.ToString();
            DateTime fechaInicio = DateTime.Now.Date;
            if (inicioMes != "")
            {
                fechaInicio = Convert.ToDateTime(inicioMes);
            }
            var fechaMesSgte = fechaInicio.AddMonths(1);
            var fechaFin = fechaMesSgte.AddDays(-1);
            var fecha = fechaFin.ToString();
            var registro = (from c in db.Contrato.ToList()
                            join e in db.Estado on c.IdEstado equals e.IdEstado
                            join em in db.Empresa on c.IdEmpresa equals em.IdEmpresa
                            join tc in db.TipoContrato on c.IdTipoContrato equals tc.IdTipoContrato
                            where c.IdTipoContrato == 1 && c.FechaTermino >= fechaInicio
                            && ((IdEmpresa != null) ? em.IdEmpresa == IdEmpresa : true)
                            && ((IdBanco != null) ? c.IdBanco == IdBanco : true)
                            //&& ((anio != null) ? c.FechaTermino.Year == anio : true)
                            //&& ((IdMes != null) ? c.FechaTermino.Month == IdMes : true)
                            select new
                            {
                                em.IdEmpresa,
                                IdContrato = c.IdContrato,
                                IdTipoContrato = c.IdTipoContrato,
                                RazonSocial = em.RazonSocial,
                                Monto = c.Monto,
                                TasaMensual = c.TasaMensual,
                                c.TasaAnual,
                                Plazo = c.Plazo,
                                MontoTasaMensual = c.Monto * c.TasaMensual,
                                NombreTipoFinanciamiento = (c.TipoFinanciamiento != null) ? c.TipoFinanciamiento.NombreTipoFinanciamiento : string.Empty,
                                c.IdTipoMoneda
                            }).AsEnumerable().ToList();

            var totales = registro.GroupBy(c => new { c.RazonSocial, c.IdEmpresa })
                .Select(c => new {
                    c.Key.RazonSocial,
                    c.Key.IdEmpresa,
                    TotalCLP = c.Where(x => x.IdTipoMoneda == 1).Sum(x => x.Monto),
                    TotalUF = c.Where(x => x.IdTipoMoneda == 2).Sum(x => x.Monto),
                    TotalUSD = c.Where(x => x.IdTipoMoneda == 3).Sum(x => x.Monto),
                }).ToList();

            
            return Json(totales, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Consolidado3_Read(int? IdEmpresa, int? IdBanco, int? anio, int? IdMes, string valorUf)
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
                   "SP_DEUDA_CONTRATO @fechaInicio={0},@fechaFin={1},@idTipoContrato={2},@IdEmpresa={3},@IdBanco={4},@valorUf={5}",
                   fechaInicio, fechaFin, (int)Helper.TipoContrato.Leasing, IdEmpresa, IdBanco, valorUfDouble).ToList();

            var totales = deudas.Where(c=>c.IdEstado==(int)Helper.Estado.ConActivo).GroupBy(c => new { c.IdFamilia})
                .Select(c => new { 
                    c.Key.IdFamilia,
                    TotalCLP = c.Where(x => x.IdTipoMoneda == 1).Distinct().Sum(x => x.MontoContrato),
                    TotalUF = c.Where(x => x.IdTipoMoneda == 2).Distinct().Sum(x => x.MontoContrato),
                    CantidadReg = c.Count() ,
                    SaldoInsoluto=c.Sum(x=>x.SaldoInsoluto)
                });

            var listTasaPromedio = (from total in totales
                                    join fam in db.Familia on total.IdFamilia equals fam.IdFamilia
                                    select new
                                    {                                        
                                        total.IdFamilia,
                                        Familia=fam.NombreFamilia,
                                        total.CantidadReg,
                                        total.TotalCLP,
                                        TotalUF=Math.Round((double)total.TotalUF,0),
                                        DeudaVigente = total.SaldoInsoluto

                                    }
                          ).ToList();
            return Json(listTasaPromedio, JsonRequestBehavior.AllowGet);
        }
    }
}