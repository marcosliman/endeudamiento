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

        public ActionResult ListaActivo_Read(int? numeroActivo, string codigoActivo)
        {
            var registro = (from ac in db.Activo
                            join em in db.Empresa on ac.IdEmpresa equals em.IdEmpresa into emw
                            from emv in emw.DefaultIfEmpty()
                            where  ac.NumeroInterno == ((numeroActivo != null) ? numeroActivo : ac.NumeroInterno)
                            && ac.CodSoftland == ((codigoActivo != "") ? codigoActivo : ac.CodSoftland)
                            select new ActivoViewModel
                            {
                                IdActivo =  ac.IdActivo,
                                RazonSocial = (emv != null) ? emv.RazonSocial : string.Empty,
                                NumeroInterno = ac.NumeroInterno,
                                CodSoftland = ac.CodSoftland,
                                Familia = ac.Familia,
                                Descripcion = ac.Descripcion,
                                Capacidad = ac.Capacidad,
                                Marca = ac.Marca,
                                Modelo = ac.Modelo,
                                MotorChasis = ac.MotorChasis,
                                Serie = ac.Serie,
                                Anio = ac.Anio,
                                Valor = ac.Valor,
                                NumeroFactura = ac.NumeroFactura,
                                Patente = ac.Patente
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

                var registro = (from ac in db.Activo where ac.IdActivo == IdActivo

                                select new ActivoViewModel
                                {
                                    IdActivo = ac.IdActivo,
                                    IdEmpresa = ac.IdEmpresa,
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    Familia = ac.Familia,
                                    Descripcion = ac.Descripcion,
                                    Capacidad = ac.Capacidad,
                                    Marca = ac.Marca,
                                    Modelo = ac.Modelo,
                                    MotorChasis = ac.MotorChasis,
                                    Serie = ac.Serie,
                                    Anio = ac.Anio,
                                    Valor = ac.Valor,
                                    IdProveedor = ac.IdProveedor,
                                    IdCuenta = ac.IdCuenta,
                                    NumeroFactura = ac.NumeroFactura,
                                    Patente = ac.Patente,
                                    TituloBoton = "Editar"
                                }).FirstOrDefault();

                if (registro == null) {
                    registro = new ActivoViewModel();
                    registro.IdActivo = 0;
                    registro.IdProveedor = 0;
                    registro.IdEmpresa = 0;
                    registro.TituloBoton = "Grabar";
                }

                var empresa = (from e in db.Empresa
                                 where e.Activo == true
                                 select new RetornoGenerico { Id = e.IdEmpresa, Nombre = e.RazonSocial }).OrderBy(c => c.Id).ToList();
                SelectList listaEmpresa = new SelectList(empresa.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdEmpresa);
                ViewData["listaEmpresa"] = listaEmpresa;

                var proveedor = (from p in db.Proveedor
                                     where p.Activo == true
                                     select new RetornoGenerico { Id = p.IdProveedor, Nombre = p.NombreProveedor }).OrderBy(c => c.Id).ToList();
                SelectList listaProveedor = new SelectList(proveedor.OrderBy(c => c.Nombre), "Id", "Nombre", registro.IdProveedor);
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
                datos.Familia = validarDatos.ValidaStr(datos.Familia);
                datos.Descripcion = validarDatos.ValidaStr(datos.Descripcion);
                datos.Capacidad = validarDatos.ValidaStr(datos.Capacidad);
                datos.Marca = validarDatos.ValidaStr(datos.Marca);
                datos.Modelo = validarDatos.ValidaStr(datos.Modelo);
                datos.MotorChasis = validarDatos.ValidaStr(datos.MotorChasis);
                datos.Serie = validarDatos.ValidaStr(datos.Serie);
                datos.NumeroFactura = validarDatos.ValidaStr(datos.NumeroFactura);
                datos.Patente = validarDatos.ValidaStr(datos.Patente);


                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var mensaje = "";
                        var idActivo = 0;
                        var activo = db.Activo.Where(c => c.IdActivo == datos.IdActivo).FirstOrDefault();

                        var estado = (int)Helper.Estado.ActCreado;

                        if (datos.CodSoftland != "" && datos.Familia != "" && datos.NumeroInterno > 0) {
                            estado = (int)Helper.Estado.ActDisponible;
                        }

                        if (activo != null)
                        {
                            idActivo = activo.IdActivo;
                            mensaje = "Registro Actualizado con exito";
                            activo.IdEmpresa = datos.IdEmpresa;
                            activo.NumeroInterno = datos.NumeroInterno;
                            activo.CodSoftland = datos.CodSoftland;
                            activo.Familia = datos.Familia;
                            activo.Descripcion = datos.Descripcion;
                            activo.Capacidad = datos.Capacidad;
                            activo.Marca = datos.Marca;
                            activo.Modelo = datos.Modelo;
                            activo.MotorChasis = datos.MotorChasis;
                            activo.Serie = datos.Serie;
                            activo.Anio = datos.Anio;
                            activo.Valor = datos.Valor;
                            activo.IdProveedor = datos.IdProveedor;
                            activo.IdCuenta = datos.IdCuenta;
                            activo.NumeroFactura = datos.NumeroFactura;
                            activo.Patente = datos.Patente;
                            activo.IdEstado = estado;
                            db.SaveChanges();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };
                        }
                        else
                        {
                            mensaje = "Registro Ingresado con exito";
                            var activoAdd = new Activo();
                            activoAdd.IdEmpresa = datos.IdEmpresa;
                            activoAdd.NumeroInterno = datos.NumeroInterno;
                            activoAdd.CodSoftland = datos.CodSoftland;
                            activoAdd.Familia = datos.Familia;
                            activoAdd.Descripcion = datos.Descripcion;
                            activoAdd.Capacidad = datos.Capacidad;
                            activoAdd.Marca = datos.Marca;
                            activoAdd.Modelo = datos.Modelo;
                            activoAdd.MotorChasis = datos.MotorChasis;
                            activoAdd.Serie = datos.Serie;
                            activoAdd.Anio = datos.Anio;
                            activoAdd.Valor = datos.Valor;
                            activoAdd.IdProveedor = datos.IdProveedor;
                            activoAdd.IdCuenta = datos.IdCuenta;
                            activoAdd.NumeroFactura = datos.NumeroFactura;
                            activoAdd.Patente = datos.Patente;
                            activoAdd.IdEstado = estado;
                            activoAdd.FechaRegistro = DateTime.Now;
                            activoAdd.IdUsuarioRegistro = (int)seguridad.IdUsuario;
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
    }
}