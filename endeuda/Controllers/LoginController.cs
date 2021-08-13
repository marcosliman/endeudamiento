using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tesoreria.Helper;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
namespace tesoreria.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private ErpContext db = new ErpContext();
        public Seguridad PerfilesUsuario(int tipoAcceso, int idUsuario)
        {                
            int usuarioId = 0;
            decimal? empresaId = 0;
            dynamic nombreEmpresa = string.Empty;
            dynamic nombreUsuario = string.Empty;
            var arrayPerfil = new List<Helper.Perfil>();                
            /*verificar usuarios empresas*/
            var existeUsuario = (from us in db.Usuario
                                 where us.IdUsuario== idUsuario
                                 select new
                                 {
                                     us.NombreUsuario,
                                     us.ApellidoUsuario,
                                     us.IdUsuario
                                 }
                                 ).FirstOrDefault();
            if (existeUsuario != null)
            {               
                var perfilesExternos = db.UsuarioPerfil.Where(c => c.IdUsuario == existeUsuario.IdUsuario).ToList();                  

                foreach (var pe in perfilesExternos)
                {
                    var perf = new Helper.Perfil
                    {
                        IdPerfil = pe.IdPerfil,
                        EMPR_ID = 0
                    };
                    arrayPerfil.Add(perf);
                }                
                usuarioId = existeUsuario.IdUsuario;
                nombreUsuario = existeUsuario.NombreUsuario + ' ' + existeUsuario.ApellidoUsuario;
            }

            Seguridad seguridadPerfil = new Seguridad
            {
                IdUsuario = usuarioId,
                Nombre = nombreUsuario,
                IdPerfil = arrayPerfil,
                EMPR_ID = empresaId,
                UserName = ""
            };
           
            seguridadPerfil.RazonSocial = seguridadPerfil.Nombre;
            seguridadPerfil.RutEmpresa = "";
            seguridadPerfil.multiEmpresa = false;
            return seguridadPerfil;
        }
        public Seguridad AgregarPermisos(Seguridad seguridad)
        {

            /*si no tiene acceso se devuelve al inicio*/
            if (seguridad.IdUsuario != null)
            {
                var perfiles = seguridad.IdPerfil.Select(c => c.IdPerfil).ToList();
                seguridad.MenuUsuario = new List<Helper.Menu>();
                var permisos = (from a in db.PermisoPerfil
                                join m in db.Menu on a.IdMenu equals m.IdMenu
                                join p in db.Perfil on a.IdPerfil equals p.IdPerfil
                                where m.IdEstadoVigencia == 1 && perfiles.Any(y => y == a.IdPerfil)
                                select new PermisoUsuario
                                {
                                    Acceder = (bool)a.Acceder,
                                    Crear = (bool)a.Crear,
                                    Editar = (bool)a.Editar,
                                    Eliminar = (bool)a.Eliminar,
                                    IdPermisoPerfil = a.IdPermisoPerfil,
                                    IdPerfil = a.IdPerfil,
                                    IdMenu = m.IdMenu,
                                    IdMenuPadre = m.IdMenuPadre,
                                    NombreMenu = m.NombreMenu,
                                    NombreVista = m.Nombre,
                                    NivelMenu = m.NivelMenu
                                }).ToList();
                var bajonivel = permisos.Where(c => c.NivelMenu == 3).ToList();
                if (seguridad.Permisos == null)
                {
                    seguridad.Permisos = permisos;
                }
                else
                {

                    seguridad.Permisos.AddRange(permisos);
                }
                var listMenuHeader = db.Menu.Where(c => c.NivelMenu == 0).ToList();
                if (permisos != null && permisos.Count() > 0)
                {
                    var menuPermiso = permisos.Select(c => c.IdMenu).ToList();
                    var Menus = db.Menu.Where(c => menuPermiso.Any(y => y == c.IdMenu)).OrderBy(c => c.NivelMenu);
                    //Nivel 1
                    foreach (var opcion in Menus)
                    {
                        var tmp = permisos.FirstOrDefault(c => c.IdMenu == opcion.IdMenu);
                        if (tmp != null && tmp.Acceder == true)
                        {
                            var menuPadre = new Helper.Menu
                            {
                                Icono = opcion.Icono,
                                OrdenMenu = opcion.OrdenMenu,
                                NombreMenu = opcion.NombreMenu,
                                UrlMenu = opcion.UrlMenu,
                                IdMenu = opcion.IdMenu,
                                IdMenuPadre = opcion.IdMenuPadre,
                                NivelMenu = opcion.NivelMenu,
                                Nombre = opcion.Nombre
                            };
                            menuPadre.Hijos = new List<Helper.Menu>();                            
                            seguridad.MenuUsuario.Add(menuPadre);
                            var existeHeader = listMenuHeader.Where(c => c.IdMenuPadre == menuPadre.IdMenu).FirstOrDefault();
                            if (existeHeader != null)
                            {
                                var menuHeader = new Helper.Menu
                                {                                   
                                    NombreMenu = existeHeader.NombreMenu,                                    
                                    IdMenuPadre = existeHeader.IdMenuPadre,
                                    NivelMenu=existeHeader.NivelMenu
                                };
                                seguridad.MenuUsuario.Add(menuHeader);
                            }
                        }
                    }

                    var subMenu = db.Menu.Where(c => menuPermiso.Any(y => y == c.IdMenu) && c.NivelMenu != 1).OrderBy(c => c.NivelMenu);
                    //Nivel 2
                    foreach (var opcion in subMenu)
                    {
                        var tmp = permisos.FirstOrDefault(c => c.IdMenu == opcion.IdMenu);                       
                        if (tmp != null && tmp.Acceder == true)
                        {
                            var menu = new Helper.Menu
                            {
                                Icono = opcion.Icono,
                                OrdenMenu = opcion.OrdenMenu,
                                NombreMenu = opcion.NombreMenu,
                                UrlMenu = opcion.UrlMenu,
                                IdMenu = opcion.IdMenu,
                                IdMenuPadre = opcion.IdMenuPadre,
                                NivelMenu = opcion.NivelMenu,
                                Nombre = opcion.Nombre
                            };
                            var aux = seguridad.MenuUsuario.Where(c => c.IdMenu == menu.IdMenuPadre).FirstOrDefault();
                            if (aux != null)
                            {
                                aux.Hijos.Add(menu);
                            }
                        }
                    }
                    
                }

            }
            return seguridad;
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");

        }
        public void SinPermios()
        {
            RedirectToAction("Index", "Home");
        }

    }
}