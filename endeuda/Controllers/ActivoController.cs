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
using Microsoft.Win32;
using tesoreria.Helper;

namespace tesoreria.Controllers
{
    public class ActivoController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var contratoNull = new Contrato();
            var registro = (from ac in db.Activo
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            join f in db.Familia on ac.IdFamilia equals f.IdFamilia into fw
                            from fv in fw.DefaultIfEmpty()
                            where ac.NumeroInterno == ((numeroInterno != "") ? numeroInterno : ac.NumeroInterno)
                            && ac.CodSoftland == ((codigoSoftland != "") ? codigoSoftland : ac.CodSoftland)
                            select new
                            {
                                IdActivo = ac.IdActivo,
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
                                ac.NumeroContrato,
                                ac.SincronizadoSoftland,
                                ac.CampoGD,
                                ac.MesAnio,
                                ac.DescCC_Mqs,
                                ac.DescCC_MqsSur,
                                ac.ValorFactura,
                                EnContrato = (db.ContratoActivo.Where(x => x.IdActivo == ac.IdActivo && x.Contrato.IdEstado!=(int)Helper.Estado.ConFinalizado).Count() > 0) ? true : false,
                                ContratoActivo = db.ContratoActivo.Where(x => x.IdActivo == ac.IdActivo && x.Contrato.IdEstado != (int)Helper.Estado.ConFinalizado).FirstOrDefault(),
                                ac.FechaBaja,
                                ac.FecIngBaja,
                                ac.Glosa,
                                EnPoliza=(db.PolizaActivo.Where(x=>x.IdActivo==ac.IdActivo).Count()>0)?true:false,
                                PolizaActivo= db.PolizaActivo.Where(x => x.IdActivo == ac.IdActivo).FirstOrDefault()
                            }).AsEnumerable().ToList();
            var listaRetorno = (from reg in registro
                                select new { 
                                reg.IdActivo,
                                reg.RazonSocial,
                                reg.NumeroInterno,
                                reg.CodSoftland,
                                reg.Familia,
                                reg.Descripcion,
                                reg.Capacidad,
                                reg.Marca,
                                reg.Modelo,
                                reg.Motor,
                                reg.Chasis,
                                reg.Serie,
                                reg.Anio,
                                reg.Valor,
                                reg.NumeroFactura,
                                reg.Patente,
                                reg.IdEstado,
                                reg.NumeroContrato,
                                reg.SincronizadoSoftland,
                                reg.CampoGD,
                                reg.MesAnio,
                                Cc_Mqs=reg.DescCC_Mqs,
                                Cc_MqsSur=reg.DescCC_MqsSur,
                                reg.ValorFactura,
                                reg.FechaBaja,
                                reg.FecIngBaja,
                                reg.Glosa,
                                reg.EnContrato,
                                Leasing=(reg.ContratoActivo!=null)?((reg.ContratoActivo.Contrato.IdTipoContrato==1)?reg.ContratoActivo.Contrato.NumeroContrato:""):"",
                                Banco = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.Banco.NombreBanco : "") : "",
                                DescripcionLeasing = (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdTipoContrato == 1) ? reg.ContratoActivo.Contrato.Descripcion : "") : "",
                                TipoPropiedad= (reg.ContratoActivo != null) ? ((reg.ContratoActivo.Contrato.IdEstado == (int)Helper.Estado.ConActivo) ? "Leasing" : "Propio") : "Propio",
                                    NumeroPoliza = (reg.EnPoliza==true)?reg.PolizaActivo.Poliza.NumeroPoliza:""
                                    //(Propio (parte siendo propio) o leasing vigente (al finalizar leasing pasa a ser propio))
                                }).ToList();
            return Json(listaRetorno, JsonRequestBehavior.AllowGet);
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
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
                                    IdEstado=ac.IdEstado,
                                    Activo=ac,
                                    IdMarcaProducto=ac.IdMarcaProducto,
                                    IdModeloProducto=ac.IdModeloProducto,
                                    Glosa=ac.Glosa
                                }).FirstOrDefault();
                var codiCCLtda = "";
                var codiCCMaquinariasa = "";
                if (registro == null) {
                    registro = new ActivoViewModel();
                    registro.IdActivo = 0;
                    registro.IdProveedor = "";
                    registro.IdEmpresa = 0;
                    registro.TituloBoton = "Grabar";
                    registro.IdCuenta = "";
                    registro.IdMarcaProducto = 0;
                    registro.IdModeloProducto = 0;
                    registro.Activo = new Activo();
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
                    codiCCLtda = registro.Activo.CodiCC_MqsSur;
                    codiCCMaquinariasa = registro.Activo.CodiCC_Mqs;
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

                //marca y modelo
                var marcas = (from t in db.MarcaProducto
                           select new RetornoGenerico { Id = t.IdMarcaProducto, Nombre = t.DescMarcaProducto }).OrderBy(c => c.Nombre).ToList();
                SelectList listaMarcas = new SelectList(marcas.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdMarcaProducto);
                ViewData["listaMarcas"] = listaMarcas;
                var modelos = (from t in db.ModeloProducto
                              select new RetornoGenerico { Id = t.IdModeloProducto, Nombre = t.DescModeloProducto }).OrderBy(c => c.Nombre).ToList();
                SelectList listaModelos = new SelectList(modelos.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdModeloProducto);
                ViewData["listaModelos"] = listaModelos;
                //Centro de costo Ltda
                var empresaLtda = db.Empresa.Find((int)Helper.Constantes.IdEmpresaPrincipal);
                SoftLandContext dbSoftLtda = new SoftLandContext(empresaLtda.BaseSoftland);
                var cCostoLtda = (from t in dbSoftLtda.cwtccos
                             where t.Activo=="S"
                             select new RetornoGenerico { Id = 0, Nombre =t.CodiCC+": "+ t.DescCC, ValorString = t.CodiCC }).OrderBy(c => c.Nombre).ToList();
                SelectList listaCCostoLtda = new SelectList(cCostoLtda.OrderBy(c => c.Nombre), "ValorString", "Nombre", codiCCLtda);
                ViewData["listaCCostoLtda"] = listaCCostoLtda;
                //Centro de costo Maquinariasa
                var empresaMaquinariasa = db.Empresa.Find((int)Helper.Constantes.IdEmpresaMaquinariasa);
                SoftLandContext dbSoftMaquinariasa = new SoftLandContext(empresaMaquinariasa.BaseSoftland);
                var cCostoMaquinariasa = (from t in dbSoftMaquinariasa.cwtccos
                                  where t.Activo == "S"
                                  select new RetornoGenerico { Id = 0, Nombre = t.CodiCC + ": " + t.DescCC, ValorString = t.CodiCC }).OrderBy(c => c.Nombre).ToList();
                SelectList listaCCostoMaquinariasa = new SelectList(cCostoMaquinariasa.OrderBy(c => c.Nombre), "ValorString", "Nombre", codiCCMaquinariasa);
                ViewData["listaCCostoMaquinariasa"] = listaCCostoMaquinariasa;

                //estados
                var estados = (from t in db.Estado where t.IdEstado==(int)Helper.Estado.ActDisponible || t.IdEstado == (int)Helper.Estado.ActBaja
                              select new RetornoGenerico { Id = t.IdEstado, Nombre = t.NombreEstado }).OrderBy(c => c.Nombre).ToList();
                SelectList listaEstados = new SelectList(estados.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEstado);
                ViewData["listaEstados"] = listaEstados;
                //Grupo Tarifario
                var grupotarifas = (from t in db.GrupoTarifario
                               where t.Activo == true
                               select new RetornoGenerico { Id = t.IdGrupoTarifario, Nombre = t.GrupoFamilia }).OrderBy(c => c.Nombre).ToList();
                SelectList listaGrupoTarifario = new SelectList(grupotarifas.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEstado);
                ViewData["listaGrupoTarifario"] = listaGrupoTarifario;
                return View(registro);
            }
        }
        public ActivoViewModel SaveActivoBD(Activo datos)
        {
            
            ActivoViewModel retorno = new ActivoViewModel();
            var validarDatos = DependencyResolver.Current.GetService<FuncionesGeneralesController>();
            
            var marca = db.MarcaProducto.Find(datos.IdMarcaProducto);
            var modelo = db.ModeloProducto.Find(datos.IdModeloProducto);
            datos.CodSoftland = validarDatos.ValidaStr(datos.CodSoftland);
            datos.Descripcion = validarDatos.ValidaStr(datos.Descripcion);
            datos.Capacidad = validarDatos.ValidaStr(datos.Capacidad);
            datos.Marca = validarDatos.ValidaStr((marca != null) ? marca.DescMarcaProducto : "");
            datos.Modelo = validarDatos.ValidaStr((modelo != null) ? modelo.DescModeloProducto : "");
            datos.Grupo = validarDatos.ValidaStr(datos.Grupo);
            datos.SubGrupo = validarDatos.ValidaStr(datos.SubGrupo);
            datos.Motor = validarDatos.ValidaStr(datos.Motor);
            datos.Chasis = validarDatos.ValidaStr(datos.Chasis);
            datos.Serie = validarDatos.ValidaStr(datos.Serie);
            datos.NumeroFactura = validarDatos.ValidaStr(datos.NumeroFactura);
            datos.Patente = validarDatos.ValidaStr(datos.Patente);
            datos.Glosa = validarDatos.ValidaStr(datos.Glosa);
            datos.CampoGD = validarDatos.ValidaStr(datos.CampoGD);
            datos.MesAnio = validarDatos.ValidaStr(datos.MesAnio);
            datos.CodiCC_Mqs = validarDatos.ValidaStr(datos.CodiCC_Mqs);
            datos.CodiCC_MqsSur = validarDatos.ValidaStr(datos.CodiCC_MqsSur);

            //centro de costo
            var empresaLtda = db.Empresa.Find((int)Helper.Constantes.IdEmpresaPrincipal);
            SoftLandContext dbSoftLtda = new SoftLandContext(empresaLtda.BaseSoftland);
            var cCostoLtda = dbSoftLtda.cwtccos.Find(datos.CodiCC_MqsSur);
            datos.DescCC_MqsSur = (cCostoLtda != null) ? cCostoLtda.DescCC : "";

            //Centro de costo Maquinariasa
            var empresaMaquinariasa = db.Empresa.Find((int)Helper.Constantes.IdEmpresaMaquinariasa);
            SoftLandContext dbSoftMaquinariasa = new SoftLandContext(empresaMaquinariasa.BaseSoftland);
            var cCostoMaquinariasa = dbSoftMaquinariasa.cwtccos.Find(datos.CodiCC_Mqs);
            datos.DescCC_Mqs = (cCostoMaquinariasa != null) ? cCostoMaquinariasa.DescCC : "";
            //Grupo Activo
            var empresaSel = db.Empresa.Find(datos.IdEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresaSel.BaseSoftland);
            var grupos = dbSoft.awtgrup.ToList();
            datos.Grupo = (datos.Grupo!=null)?datos.Grupo.TrimStart('0'): datos.Grupo;
            var grupo = grupos.Where(c => c.CodGru.TrimStart('0') == datos.Grupo).FirstOrDefault();
            var descGrupo = (grupo != null) ? grupo.DesGru : "";
            //SubGrupo Activo                
            var subgrupos = dbSoft.awtsubgr.ToList();
            datos.SubGrupo = (datos.SubGrupo!=null) ? datos.SubGrupo.TrimStart('0') : datos.SubGrupo;
            var subgrupo = subgrupos.Where(c => c.CodSGru.TrimStart('0') == datos.SubGrupo && c.CodGru.TrimStart('0') == datos.Grupo).FirstOrDefault();
            var descSubGrupo = (subgrupo != null) ? subgrupo.DesSGru : "";
            //Activo en softland
            var activoSoftland = dbSoft.awfichaac.Find(datos.CodSoftland);
            var esCreado = false;
            var sinError = true;
            var idActivo = 0;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var mensaje = "";
                    
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
                        activo.Grupo = (grupo!=null)? grupo.CodGru:datos.Grupo;
                        activo.SubGrupo = (subgrupo!=null)? subgrupo.CodSGru:datos.SubGrupo;
                        activo.Anio = datos.Anio;
                        activo.Valor = datos.Valor;
                        activo.IdProveedor = datos.IdProveedor;
                        var auxiliar = dbSoft.cwtauxi.Find(datos.IdProveedor);
                        activo.NombreProveedor = (auxiliar != null) ? auxiliar.NomAux : "";
                        activo.IdCuenta = datos.IdCuenta;
                        activo.NumeroFactura = datos.NumeroFactura;
                        activo.Patente = datos.Patente;
                        activo.Glosa = datos.Glosa;
                        activo.IdMarcaProducto = datos.IdMarcaProducto;
                        activo.IdModeloProducto = datos.IdModeloProducto;
                        activo.IdGrupoTarifario = datos.IdGrupoTarifario;
                        if (activo.IdEstado == 3)
                        {
                            activo.IdEstado = estado;
                        }
                        activo.CampoGD = datos.CampoGD;
                        activo.MesAnio = datos.MesAnio;
                        activo.CodiCC_Mqs = datos.CodiCC_Mqs;
                        activo.DescCC_Mqs = datos.DescCC_Mqs;
                        activo.CodiCC_MqsSur = datos.CodiCC_MqsSur;
                        activo.DescCC_MqsSur = datos.DescCC_MqsSur;
                        activo.ValorFactura = datos.ValorFactura;
                        activo.DesGrupo = descGrupo;
                        activo.DesSGru = descSubGrupo;
                        if (datos.IdEstado == (int)Helper.Estado.ActDisponible)
                        {
                            activo.FechaBaja = null;
                            activo.Glosa ="";
                            activo.FecIngBaja = null;
                            activo.IdEstado = datos.IdEstado;
                        }
                        //bajada
                        if (activoSoftland != null)
                        {
                            activo.FechaBaja = activoSoftland.FecBaja;
                            activo.Glosa = activoSoftland.GloBaja;
                            activo.FecIngBaja = activoSoftland.FecIng;
                        }
                        if (datos.IdEstado == (int)Helper.Estado.ActBaja)
                        {
                            activo.FechaBaja = datos.FechaBaja;
                            activo.Glosa = datos.Glosa;
                            activo.FecIngBaja = DateTime.Now;
                            activo.IdEstado = datos.IdEstado;
                        }
                        
                        db.SaveChanges();
                        retorno.IdEstado = 0;
                        retorno.NombreEstado = mensaje;
                    }
                    else
                    {
                        esCreado = true;
                        var activoAdd = new Activo();
                        if (datos.CodSoftland != "" && datos.IdFamilia != null && datos.NumeroInterno != "")
                        {
                            estado = (int)Helper.Estado.ActDisponible;
                        }
                        mensaje = "Registro Ingresado con exito";
                        
                        activoAdd.IdEmpresa = datos.IdEmpresa;

                        activoAdd.NumeroInterno = datos.NumeroInterno;
                        activoAdd.CodSoftland = datos.CodSoftland;
                        activoAdd.IdFamilia = datos.IdFamilia;
                        activoAdd.Descripcion = datos.Descripcion;
                        activoAdd.Capacidad = datos.Capacidad;
                        activoAdd.IdMarcaProducto = datos.IdMarcaProducto;
                        activoAdd.IdModeloProducto = datos.IdModeloProducto;
                        activoAdd.Marca = datos.Marca;
                        activoAdd.Modelo = datos.Modelo;
                        activoAdd.Motor = datos.Motor;
                        activoAdd.Chasis = datos.Chasis;
                        activoAdd.Serie = datos.Serie;
                        activoAdd.Anio = datos.Anio;
                        activoAdd.Grupo = (grupo != null) ? grupo.CodGru : datos.Grupo;
                        activoAdd.SubGrupo = (subgrupo != null) ? subgrupo.CodSGru : datos.SubGrupo;
                        activoAdd.Valor = datos.Valor;
                        activoAdd.IdProveedor = datos.IdProveedor;

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
                        activoAdd.CampoGD = datos.CampoGD;
                        activoAdd.MesAnio = datos.MesAnio;
                        activoAdd.CodiCC_Mqs = datos.CodiCC_Mqs;
                        activoAdd.DescCC_Mqs = datos.DescCC_Mqs;
                        activoAdd.CodiCC_MqsSur = datos.CodiCC_MqsSur;
                        activoAdd.DescCC_MqsSur = datos.DescCC_MqsSur;
                        activoAdd.ValorFactura = datos.ValorFactura;
                        activoAdd.DesGrupo = descGrupo;
                        activoAdd.DesSGru = descSubGrupo;
                        activoAdd.IdGrupoTarifario = datos.IdGrupoTarifario;
                        //bajada
                        if (activoSoftland != null)
                        {
                            activoAdd.FechaBaja = activoSoftland.FecBaja;
                            activoAdd.Glosa = activoSoftland.GloBaja;
                            activoAdd.FecIngBaja = activoSoftland.FecIng;
                        }
                        db.Activo.Add(activoAdd);
                        db.SaveChanges();

                        idActivo = activoAdd.IdActivo;
                        retorno.IdEstado = 0;
                        retorno.NombreEstado = mensaje;
                    }
                    sinError = true;
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    retorno.IdEstado = 500;
                    retorno.NombreEstado = "Error: " + ex.Message;
                }
            }
            if(sinError==true && esCreado == true)
            {
                SendEmailController send = new SendEmailController();
                var ret = send.NotificaNuevoActivo(idActivo);
            }
            return retorno;
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GrabarActivo(Activo datos)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Crear);
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Editar);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;            

            tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
            if (seguridad == null || !seguridad.TienePermiso("ControlInterno", Helper.TipoAcceso.Acceder))
            {
                showMessageString = new { Estado = 1000, Mensaje = "Se finalizó la sesión" };
            }
            else
            {
                var valido = true;
                var estado = 0;
                var mensaje="";
                if (datos.IdActivo>0)
                {
                    var existNumInterno = db.Activo.Where(c => c.NumeroInterno == datos.NumeroInterno && c.IdEmpresa == datos.IdEmpresa && c.IdActivo!=datos.IdActivo).FirstOrDefault();
                    valido = (existNumInterno != null) ? false : true;
                }
                else
                {
                    var existNumInterno = db.Activo.Where(c => c.NumeroInterno == datos.NumeroInterno && c.IdEmpresa == datos.IdEmpresa).FirstOrDefault();
                    valido = (existNumInterno != null) ? false : true;
                }
                if (valido == true)
                {
                    var retornoSave = SaveActivoBD(datos);
                    showMessageString = new { Estado = retornoSave.IdEstado, Mensaje = retornoSave.NombreEstado };
                }
                else
                {
                    mensaje = "¡Número Interno " + datos.NumeroInterno.ToString() + " ya existe!";
                    estado = 100;
                    showMessageString = new { Estado = estado, Mensaje = mensaje };
                }
            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EliminarActivo(int idActivo)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Crear);
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Editar);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var activo = db.Activo.Find(idActivo);
                    //solo elimina cuando no hay registros asociados
                    if (activo != null)
                    {
                        var contratoAsociado = db.ContratoActivo.Where(c => c.IdActivo == idActivo).Count();
                        if (contratoAsociado > 0)
                        {
                            dbContextTransaction.Rollback();
                            showMessageString = new { Estado = 100, Mensaje = "Activo no puede ser Eliminado" };
                        }
                        else
                        {
                            db.Activo.Remove(activo);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            showMessageString = new { Estado = 0, Mensaje = "Registro eliminado con exito" };
                        }                        
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
        public ActionResult ListaImportaActivo_Read(int idEmpresa, string CodGru,string CodSGru,string CodActBus)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
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
                            && ((CodActBus != "") ? ac.CodAct == CodActBus : true)
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var grupos = (from t in dbSoft.awtgrup
                             select new RetornoGenerico { Id = 0, Nombre = t.DesGru, ValorString = t.CodGru }).OrderBy(c => c.Nombre).ToList();
            return Json(grupos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerSubGrupoSoftland(int idEmpresa, string CodGru)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var subgrupos = (from t in dbSoft.awtsubgr where ((CodGru!="")? t.CodGru==CodGru :true)
                             select new RetornoGenerico { Id = 0, Nombre = t.DesSGru, ValorString = t.CodSGru }).OrderBy(c => c.Nombre).ToList();
            return Json(subgrupos, JsonRequestBehavior.AllowGet);
        }
        public Activo ImportaActivoFromSoftland(int idEmpresa, string CodAct)
        {
            var empresa = db.Empresa.Find(idEmpresa);
            SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
            var activoSoftland = dbSoft.awfichaac.Find(CodAct);
            //solo elimina cuando no hay registros asociados
            var activoAdd = new Activo();
            if (activoSoftland != null)
            {
                var existeActivo = db.Activo.Where(c => c.CodSoftland == CodAct && c.NumeroInterno==CodAct).FirstOrDefault();
                if (existeActivo == null)
                {
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
                    activoAdd.IdEstado = (int)Helper.Estado.ActCreado;
                    activoAdd.FechaRegistro = DateTime.Now;
                    activoAdd.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                    activoAdd.SincronizadoSoftland = true;
                    activoAdd.NumeroFactura = activoSoftland.NumFac;
                    //Bajada
                    activoAdd.FechaBaja = activoSoftland.FecBaja;
                    activoAdd.Glosa = activoSoftland.GloBaja;
                    activoAdd.FecIngBaja = activoSoftland.FecIng;
                    db.Activo.Add(activoAdd);
                    db.SaveChanges();
                }                
            }
            return activoAdd;
        }
        [HttpPost]
        public ActionResult ImportaActivoSoftland(int idEmpresa,string CodAct)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
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
                        var activoAdd = ImportaActivoFromSoftland(idEmpresa, CodAct);                        
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
        [HttpPost]
        public ActionResult ImportaActivoSoftland_Masivo(int idEmpresa, string[] activos)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var empresa = db.Empresa.Find(idEmpresa);
                    SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
                    foreach(var CodAct in activos)
                    {
                        var activoSoftland = dbSoft.awfichaac.Find(CodAct);
                        //solo elimina cuando no hay registros asociados
                        if (activoSoftland != null)
                        {
                            var activoAdd = ImportaActivoFromSoftland(idEmpresa, CodAct);                            
                        }
                        
                    }
                    dbContextTransaction.Commit();
                    showMessageString = new { Estado = 0, Mensaje = "Activos Importados exitosamente" };
                    return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }
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
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);

            if (acceso.AccesoValido == false)

            {
                return RedirectToAction(acceso.Vista, acceso.Controlador);
            }

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
                    var empresaSel = db.Empresa.Find(IdEmpresa);
                    SoftLandContext dbSoft = new SoftLandContext(empresaSel.BaseSoftland);
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.NumeroInterno !="")
                            {
                                Activo activoAdd = new Activo();
                                var existeActivo = db.Activo.Where(c => c.IdEmpresa == IdEmpresa && c.NumeroInterno == a.NumeroInterno).FirstOrDefault();
                                //familia
                                var familia = db.Familia.Where(c => c.NombreFamilia == a.Familia).FirstOrDefault();
                                if (existeActivo != null)
                                {                                 
                                    activoAdd = existeActivo;
                                }
                                activoAdd.IdEmpresa = IdEmpresa;
                                activoAdd.NumeroInterno = a.NumeroInterno;
                                activoAdd.CodSoftland = a.CodSoftland;
                                activoAdd.IdFamilia = (familia != null) ? (int?)familia.IdFamilia : null;
                                activoAdd.Descripcion = a.Descripcion;
                                activoAdd.IdCuenta = a.ClasificacionCuenta;
                                activoAdd.Valor = a.Valor;
                                var marca = db.MarcaProducto.Where(c=>c.DescMarcaProducto.ToUpper()==a.Marca.ToUpper()).FirstOrDefault();
                                var modelo = db.ModeloProducto.Where(c=>c.DescModeloProducto.ToUpper() == a.Modelo.ToUpper()).FirstOrDefault();
                                activoAdd.IdMarcaProducto = (marca!=null)? marca.IdMarcaProducto:0;
                                activoAdd.IdModeloProducto = (modelo != null)? modelo.IdModeloProducto:0;
                                activoAdd.Anio = a.Anio;
                                activoAdd.Grupo = a.Grupo;
                                activoAdd.SubGrupo = a.SubGrupo;
                                var auxiliar = dbSoft.cwtauxi.Where(c=>c.RutAux.Replace(".","").Replace("-","").ToUpper()==a.RutProveedor.Replace(".", "").Replace("-", "").ToUpper()).FirstOrDefault();
                                activoAdd.IdProveedor =(auxiliar!=null)? auxiliar.CodAux:"";
                                activoAdd.NumeroFactura = a.NumeroFactura;
                                activoAdd.Motor = a.Motor;
                                activoAdd.Chasis = a.Chasis;
                                activoAdd.Patente = a.Patente;
                                activoAdd.Glosa = a.Glosa;
                                activoAdd.CampoGD = a.CampoGD;
                                //convertir mes anio a string
                                try
                                {
                                    var dateMesAnio = Convert.ToDateTime(a.MesAnio);
                                    activoAdd.MesAnio = dateMesAnio.Month.ToString() + "/" + dateMesAnio.Year.ToString();
                                }
                                catch
                                {
                                    activoAdd.MesAnio = "";
                                }                                
                                activoAdd.CodiCC_Mqs = a.CodiCC_Mqs;
                                activoAdd.CodiCC_MqsSur = a.CodiCC_MqsSur;
                                activoAdd.IdEstado = (int)Helper.Estado.ActCreado;
                                activoAdd.FechaRegistro = DateTime.Now;
                                activoAdd.IdUsuarioRegistro = (int)seguridad.IdUsuario;
                                activoAdd.SincronizadoSoftland = false;
                                var retornoSave = SaveActivoBD(activoAdd);
                                showMessageString = new { Estado = retornoSave.IdEstado, Mensaje = retornoSave.NombreEstado };
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
        public ActionResult SincronizarActivo(int? IdActivo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("ControlInterno", Helper.TipoAcceso.Acceder) || IdActivo==null)
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var activo=db.Activo.Find(IdActivo);
                return View(activo);
            }
        }

        [HttpPost]
        public ActionResult SincronizarActivoConSoftland(int? IdActivo, string CodAct)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "ControlInterno" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var activo = db.Activo.Find(IdActivo);
                    var empresa = db.Empresa.Find(activo.IdEmpresa);
                    SoftLandContext dbSoft = new SoftLandContext(empresa.BaseSoftland);
                    var activoSoftland = dbSoft.awfichaac.Find(CodAct);
                    //solo elimina cuando no hay registros asociados
                    if (activoSoftland != null)
                    {                        
                        activo.CodSoftland = CodAct;
                        activo.SincronizadoSoftland = true;                        
                        db.SaveChanges();
                        dbContextTransaction.Commit();

                        showMessageString = new { Estado = 0, Mensaje = "Activo Sincronizado con exito" };
                        return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        dbContextTransaction.Rollback();
                        showMessageString = new { Estado = 500, Mensaje = "Error: Activo no existe en Softland" };
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

        public ActionResult VistaActivo(int IdActivo)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                List<RetornoGenerico> cuentas = new List<RetornoGenerico>();
                List<RetornoGenerico> proveedores = new List<RetornoGenerico>();
                List<RetornoGenerico> grupos = new List<RetornoGenerico>();
                List<RetornoGenerico> subgrupos = new List<RetornoGenerico>();

                var registro = (from ac in db.Activo
                                where ac.IdActivo == IdActivo

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
                                    Grupo = ac.Grupo,
                                    SubGrupo = ac.SubGrupo,
                                    IdEstado = ac.IdEstado,
                                    Activo = ac,
                                    IdMarcaProducto = ac.IdMarcaProducto,
                                    IdModeloProducto = ac.IdModeloProducto
                                }).FirstOrDefault();

                if (registro == null)
                {
                    registro = new ActivoViewModel();
                    registro.IdActivo = 0;
                    registro.IdProveedor = "";
                    registro.IdEmpresa = 0;
                    registro.TituloBoton = "Grabar";
                    registro.IdCuenta = "";
                    registro.IdMarcaProducto = 0;
                    registro.IdModeloProducto = 0;
                }
                else
                {
                    var empresaSel = db.Empresa.Find(registro.IdEmpresa);
                    SoftLandContext dbSoft = new SoftLandContext(empresaSel.BaseSoftland);
                    cuentas = (from t in dbSoft.cwpctas
                               select new RetornoGenerico { Id = 0, Nombre = t.PCCODI + ": " + t.PCDESC, ValorString = t.PCCODI }).OrderBy(c => c.Nombre).ToList();
                    var auxiliar = dbSoft.cwtauxi.Find(registro.IdProveedor);
                    proveedores = (from aux in dbSoft.cwtauxi
                                   where aux.CodAux == registro.IdProveedor
                                   select new RetornoGenerico { Id = 0, Nombre = aux.RutAux + " : " + aux.NomAux, ValorString = aux.CodAux }).OrderBy(c => c.Id).ToList();

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

                //marca y modelo
                var marcas = (from t in db.MarcaProducto
                              select new RetornoGenerico { Id = t.IdMarcaProducto, Nombre = t.DescMarcaProducto }).OrderBy(c => c.Nombre).ToList();
                SelectList listaMarcas = new SelectList(marcas.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdMarcaProducto);
                ViewData["listaMarcas"] = listaMarcas;
                var modelos = (from t in db.ModeloProducto
                               select new RetornoGenerico { Id = t.IdModeloProducto, Nombre = t.DescModeloProducto }).OrderBy(c => c.Nombre).ToList();
                SelectList listaModelos = new SelectList(modelos.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdModeloProducto);
                ViewData["listaModelos"] = listaModelos;
                return View(registro);
            }
        }
        public ActionResult CobroArriendo()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("CobroArriendo", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                var mesActual = DateTime.Now.Month;
                var empresaSoftland = db.Empresa.Find(Helper.Constantes.IdEmpresaMaquinariasa);
                SoftLandContext dbSoft = new SoftLandContext(empresaSoftland.BaseSoftland);
                var empresa = (from e in db.Empresa
                               where e.Activo == true
                               select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre");
                ViewData["listaEmpresa"] = listaEmpresa;

                var areas = (from area in dbSoft.cwtaren
                             select new RetornoGenerico { Id = 0, ValorString = area.CodArn, Nombre = area.DesArn }).OrderBy(c => c.Id).ToList();
                SelectList listaArea = new SelectList(areas.OrderBy(c => c.Nombre), "ValorString", "Nombre");
                ViewData["listaArea"] = listaArea;

                var meses = (from mes in db.Mes
                             select new RetornoGenerico { Id = mes.IdMes, Nombre = mes.NombreMes }).OrderBy(c => c.Id).ToList();
                SelectList listaMes = new SelectList(meses.OrderBy(c => c.Id), "Id", "Nombre");
                ViewData["listaMes"] = listaMes;

                return View();
            }
        }
        public ActionResult ObtenerTotalDiasMes(int idMes)
        {
            var totalDias = DateTime.DaysInMonth(DateTime.Now.Year, idMes);
            return Json(totalDias, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxReporteArriendo(HttpPostedFileBase ArchivoCaptura, string valorUf, FormCollection formCollection)
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                List<string> data = new List<string>();
                List<CobroArriendoViewModel> listCobroArriendo= new List<CobroArriendoViewModel>();
                if (ArchivoCaptura != null)
                {
                    // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                    if (ArchivoCaptura.ContentType == "application/vnd.ms-excel" || ArchivoCaptura.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        var valorUfDouble = (valorUf != "") ? Double.Parse(valorUf) : 1;
                        var familias = db.Familia.ToList();
                        var grupoTarifario=db.GrupoTarifario.ToList();
                        var estados = db.Estado.ToList();
                        var empresaSoftland = db.Empresa.Find(Helper.Constantes.IdEmpresaMaquinariasa);
                        SoftLandContext dbSoft = new SoftLandContext(empresaSoftland.BaseSoftland);
                        var areas = dbSoft.cwtaren.ToList();
                        List<CobroArriendoViewModel> resumenZona = new List<CobroArriendoViewModel>();
                        foreach (var area in areas)
                        {
                            var montoStr = formCollection["valor_" + area.CodArn];
                            var montoDouble = (montoStr != "") ? Double.Parse(montoStr) : 0;
                            CobroArriendoViewModel resumen = new CobroArriendoViewModel();
                            resumen.CodArn = area.CodArn;
                            resumen.DesArn= area.DesArn;
                            resumen.MontoVenta = montoDouble;
                            resumenZona.Add(resumen);
                        }
                        var totalVentaZona = resumenZona.Sum(c => c.MontoVenta);
                        var zonaRetorno = resumenZona.Select(c => new CobroArriendoViewModel {
                            CodArn=c.CodArn,
                            DesArn=c.DesArn,
                            MontoVenta=c.MontoVenta,
                            PorcentajeVenta= (c.MontoVenta>0)?Math.Round( (((double)c.MontoVenta/ (double)totalVentaZona)*100),4):0
                        }).ToList();
                        ViewData["resumenZona"] = zonaRetorno;
                        string filename = ArchivoCaptura.FileName;
                        string targetpath = Server.MapPath("~/App_Data/");
                        ArchivoCaptura.SaveAs(targetpath + filename);
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
                        var artistAlbums = from a in excelFile.Worksheet<CobroArriendoViewModel>(sheetName) select a;                        
                        foreach (var a in artistAlbums)
                        {
                            try
                            {
                                if (a.NroEquipo != "")
                                {                                    
                                    var areaint = Int32.Parse(a.Sucursal);
                                    var areaStr = string.Format("{0:000}", areaint);
                                    var area = dbSoft.cwtaren.Find(areaStr);
                                    a.DesArn = (area!=null)? area.DesArn:a.Sucursal;
                                    a.CodArn= (area != null) ? area.CodArn :"";
                                    //datos del activo
                                    var activo = db.Activo.Where(c => c.NumeroInterno == a.NroEquipo).FirstOrDefault();
                                    if (activo != null)
                                    {
                                        a.DescCC_MqsSur = activo.DescCC_MqsSur;
                                        a.DescCC_Mqs = activo.DescCC_Mqs;
                                        a.Anio = activo.Anio;
                                        var famActivo = familias.Where(c => c.IdFamilia == activo.IdFamilia).FirstOrDefault();
                                        a.DescripcionFamilia = (famActivo != null) ? famActivo.NombreFamilia : "";
                                        var tarifaActivo = grupoTarifario.Where(c => c.IdGrupoTarifario == activo.IdGrupoTarifario).FirstOrDefault();
                                        a.GrupoTarifario = (tarifaActivo != null) ? tarifaActivo.DescripcionGrupoTarifario : "";
                                        a.TarifaUF = (tarifaActivo != null) ? tarifaActivo.UF : 0;
                                        a.TarifaCLP = (a.TarifaUF * valorUfDouble);
                                        a.TarifaCLP = (a.TarifaCLP != null) ? Math.Round((double)a.TarifaCLP, 0) : 0;

                                        var DepreciacionDouble = (a.Depreciacion != "") ? Double.Parse(a.Depreciacion) : 0;
                                        DepreciacionDouble = (DepreciacionDouble != null) ? Math.Round((double)DepreciacionDouble, 0) : 0;
                                        a.DepreciacionDouble = DepreciacionDouble;

                                        a.TarifaCLP_Aplicar = a.TarifaCLP - DepreciacionDouble;
                                        a.TarifaCLP_Aplicar = (a.TarifaCLP_Aplicar != null) ? Math.Round((double)a.TarifaCLP_Aplicar, 0) : 0;

                                        var DiasArriendoDouble = (a.DiasArriendo != "") ? Double.Parse(a.DiasArriendo) : 0;
                                        a.Cobro_DiasArriendo = (DiasArriendoDouble * a.TarifaCLP_Aplicar) / 30;
                                        a.Cobro_DiasArriendo = (a.Cobro_DiasArriendo != null) ? Math.Round((double)a.Cobro_DiasArriendo, 0) : 0;

                                        var DiasDisponibleDouble = (a.DiasDisponible != "") ? Double.Parse(a.DiasDisponible) : 0;
                                        a.Cobro_DiasDisponible = (DiasDisponibleDouble * a.TarifaCLP_Aplicar) / 30;
                                        a.Cobro_DiasDisponible = (a.Cobro_DiasDisponible != null) ? Math.Round((double)a.Cobro_DiasDisponible, 0) : 0;

                                        var DiasTallerDouble = (a.DiasTaller != "") ? Double.Parse(a.DiasTaller) : 0;
                                        a.Cobro_DiasTaller = (DiasTallerDouble * a.TarifaCLP_Aplicar) / 30;
                                        a.Cobro_DiasTaller = (a.Cobro_DiasTaller != null) ? Math.Round((double)a.Cobro_DiasTaller, 0) : 0;
                                        var estAct = estados.Where(c => c.IdEstado == activo.IdEstado).FirstOrDefault();
                                        a.EstadoActivo = (estAct != null) ? estAct.NombreEstado : "";
                                        listCobroArriendo.Add(a);
                                    }
                                    
                                }
                                else
                                {
                                    data.Add("<ul>");
                                    if (a.NroEquipo == "") data.Add("<li> Nro es obligatorio</li>");
                                    data.Add("</ul>");
                                    data.ToArray();
                                    break;
                                }
                                
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
                            }
                        }
                        ViewData["listCobroArriendo"] = listCobroArriendo;
                        //var saldoInsoluto=
                        //deleting excel file from folder
                        if ((System.IO.File.Exists(pathToExcelFile)))
                        {
                            System.IO.File.Delete(pathToExcelFile);
                        }
                        List<CobroArriendoViewModel> cobroDistribuidoZona = new List<CobroArriendoViewModel>();
                        foreach (var area in areas)
                        {
                            var montoStr = formCollection["valor_" + area.CodArn];
                            var montoDouble = (montoStr != "") ? Double.Parse(montoStr) : 0;
                            CobroArriendoViewModel resumen = new CobroArriendoViewModel();
                            resumen.CodArn = area.CodArn;
                            resumen.DesArn = area.DesArn;
                            resumen.Cobro_DiasArriendo = listCobroArriendo.Where(c=>c.CodArn==area.CodArn).Sum(c=>c.Cobro_DiasArriendo);
                            resumen.Cobro_DiasDisponible = listCobroArriendo.Where(c => c.CodArn == area.CodArn).Sum(c => c.Cobro_DiasDisponible);
                            resumen.Cobro_DiasTaller = listCobroArriendo.Where(c => c.CodArn == area.CodArn).Sum(c => c.Cobro_DiasTaller);
                            cobroDistribuidoZona.Add(resumen);
                        }
                        ViewData["cobroDistribuidoZona"] = cobroDistribuidoZona;
                    }
                    else
                    {
                        //alert message for invalid file format
                        data.Add("<ul>");
                        data.Add("<li>Only Excel file format is allowed</li>");
                        data.Add("</ul>");
                        data.ToArray();
                    }
                }
                else
                {
                    data.Add("<ul>");
                    if (ArchivoCaptura == null) data.Add("<li>Please choose Excel file</li>");
                    data.Add("</ul>");
                    data.ToArray();
                }
                return View();
            }
        }

    }
}