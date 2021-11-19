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
                            where  ac.NumeroInterno == ((numeroActivo != null) ? numeroActivo : ac.NumeroInterno)
                            && ac.CodSoftland == ((codigoActivo != "") ? codigoActivo : ac.CodSoftland)
                            select new 
                            {
                                ac.IdActivo,
                                ac.NumeroInterno,
                                ac.CodSoftland,
                                ac.Descripcion,
                                ac.Capacidad,
                                ac.Marca,
                                ac.Modelo,
                                ac.Motor,
                                ac.Serie,
                                ac.Anio,
                                ac.Valor
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
                                    NumeroInterno = ac.NumeroInterno,
                                    CodSoftland = ac.CodSoftland,
                                    Descripcion = ac.Descripcion,
                                    Capacidad = ac.Capacidad,
                                    Marca = ac.Marca,
                                    Modelo = ac.Modelo,
                                    Motor = ac.Motor,
                                    Serie = ac.Serie,
                                    Anio = ac.Anio,
                                    Valor = ac.Valor,
                                    TituloBoton = "Editar"
                                }).FirstOrDefault();

                if (registro == null) {
                    registro = new ActivoViewModel();
                    registro.IdActivo = 0;
                    registro.TituloBoton = "Grabar";
                }

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
                datos.Motor = validarDatos.ValidaStr(datos.Motor);
                datos.Serie = validarDatos.ValidaStr(datos.Serie);

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var mensaje = "";
                        var idActivo = 0;
                        var activo = db.Activo.Where(c => c.IdActivo == datos.IdActivo).FirstOrDefault();

                        if (activo != null)
                        {
                            idActivo = activo.IdActivo;
                            mensaje = "Registro Actualizado con exito";
                            activo.NumeroInterno = datos.NumeroInterno;
                            activo.CodSoftland = datos.CodSoftland;
                            activo.Descripcion = datos.Descripcion;
                            activo.Capacidad = datos.Capacidad;
                            activo.Marca = datos.Marca;
                            activo.Modelo = datos.Modelo;
                            activo.Motor = datos.Motor;
                            activo.Serie = datos.Serie;
                            activo.Anio = datos.Anio;
                            activo.Valor = datos.Valor;
                            db.SaveChanges();
                            showMessageString = new { Estado = 0, Mensaje = mensaje };
                        }
                        else
                        {
                            mensaje = "Registro Ingresado con exito";
                            var activoAdd = new Activo();
                            activoAdd.NumeroInterno = datos.NumeroInterno;
                            activoAdd.CodSoftland = datos.CodSoftland;
                            activoAdd.Descripcion = datos.Descripcion;
                            activoAdd.Capacidad = datos.Capacidad;
                            activoAdd.Marca = datos.Marca;
                            activoAdd.Modelo = datos.Modelo;
                            activoAdd.Motor = datos.Motor;
                            activoAdd.Serie = datos.Serie;
                            activoAdd.Anio = datos.Anio;
                            activoAdd.Valor = datos.Valor;
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