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
    public class UsuarioController : Controller
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
            else if (seguridad != null && !seguridad.TienePermiso("Usuarios", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public JsonResult Usuarios_Read(bool? interno)
        {
            var lista = db.Usuario.ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create(int? id)
        {
            Usuario registro = new Usuario();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Usuario.Find(id);
                    registro.Clave = "";
                }
            }
            var perfiles = (from p in db.Perfil.ToList()
                            join up in db.UsuarioPerfil.Where(x => x.IdUsuario == id) on p.IdPerfil equals up.IdPerfil into t_up
                            from l_up in t_up.DefaultIfEmpty()                            
                            select new RetornoGenerico
                            {
                                Id = (int)p.IdPerfil,
                                Nombre = p.NombrePerfil,
                                Seleccionado = (l_up == null) ? false : true,
                                Editable = true
                            }).OrderBy(c => c.Nombre).ToList();

            //SelectList listaperfil = new SelectList(perfiles.OrderBy(c => c.Nombre), "Id", "Nombre");
            ViewData["listaPerfiles"] = perfiles;

           
            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Usuario registro, int[] perfiles)
        {
            dynamic showMessageString = string.Empty;
            var valido = true;
            Usuario registroEdit = new Usuario();
            if (registro.IdUsuario > 0)
            {
                showMessageString = new { Estado = 0, Mensaje = "Usuario Actualizado" };                              
                
                registroEdit = db.Usuario.Find(registro.IdUsuario);
                if (registroEdit != null)
                {
                    if (registro.Clave != "" && registro.Clave != null)
                    {
                        registroEdit.Clave = Crypto.Hash(registro.Clave);
                    }
                    registroEdit.RutUsuario = registro.RutUsuario.ToUpper();
                    registroEdit.NombreUsuario = registro.NombreUsuario;
                    registroEdit.ApellidoUsuario = registro.ApellidoUsuario;
                    registroEdit.CorreoElectronico = registro.CorreoElectronico;
                    registroEdit.Activo = registro.Activo;
                }
                
            }
            else
            {
                var mensajeRetorno = "";                
                var rutExiste = db.Usuario.Where(c => c.RutUsuario.Replace("-", "").Replace(".", "").ToUpper() == registro.RutUsuario.Replace("-", "").Replace(".", "").ToUpper()).FirstOrDefault();
                if (rutExiste != null)
                {
                    mensajeRetorno = "Rut ya existe en el sistema";
                    valido = false;
                }
                var emailExiste = db.Usuario.Where(c => c.CorreoElectronico.ToUpper() == registro.CorreoElectronico.ToUpper()).FirstOrDefault(); 
                if (emailExiste != null)
                {
                    mensajeRetorno = (mensajeRetorno == "") ? mensajeRetorno : mensajeRetorno + "<br>";
                    mensajeRetorno += "Correo ya existe en el sistema";
                    valido = false;
                }
                if (valido == true)
                {
                    registroEdit = registro;
                    registroEdit.RutUsuario = registro.RutUsuario.ToUpper();
                    registroEdit.Clave = Crypto.Hash(registro.Clave);
                    registroEdit.FechaRegistro = DateTime.Now;
                    showMessageString = new { Estado = 0, Mensaje = "Usuario Registrado" };
                    db.Usuario.Add(registroEdit);
                }
                else
                {
                    showMessageString = new { Estado = 100, Mensaje = mensajeRetorno };
                }
            }
            db.SaveChanges();
            if (valido == true)
            {
                //eliminar perfiles
                var uPerfil = db.UsuarioPerfil.Where(c => c.IdUsuario == registroEdit.IdUsuario);

                foreach (var c in uPerfil)
                {
                    db.UsuarioPerfil.Remove(c);
                }
                db.SaveChanges();
                //Asociar perfiles
                if (perfiles != null)
                {
                    foreach (var id in perfiles)
                    {
                        UsuarioPerfil newPerfil = new UsuarioPerfil();
                        newPerfil.IdPerfil = id;
                        newPerfil.IdUsuario = registroEdit.IdUsuario;
                        newPerfil.Activo = true;
                        db.UsuarioPerfil.Add(newPerfil);
                    }
                }
                db.SaveChanges();
            }
            
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
    }
}