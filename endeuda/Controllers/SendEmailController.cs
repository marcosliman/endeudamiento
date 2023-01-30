using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using tesoreria.Helper;
namespace tesoreria.Controllers
{
    public class SendEmailController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        // GET: Usuario
        public bool NotificaActivacionContrato(int IdContrato)
        {
            //Notificar Asignación            
            var contrato = db.Contrato.Find(IdContrato);
            int[] perfiles = { (int)Helper.PerfilAcceso.Seguros, (int)Helper.PerfilAcceso.AdmContabilidad };
            //var parcelaTarea = db.ParcelaTarea.Where(c => c.IdParcelaReserva == parcelareserva.IdParcela && c.IdTarea == IdTarea).FirstOrDefault();
            List<UsuarioPerfil> usersPerfil = new List<UsuarioPerfil>();
            usersPerfil = (from us in db.Usuario.ToList()
                           join up in db.UsuarioPerfil.ToList() on us.IdUsuario equals up.IdUsuario
                           where perfiles.Any(y=>y==up.IdPerfil) && us.Activo == true
                           select new UsuarioPerfil { Usuario = us, Perfil = up.Perfil }).ToList();
            var correos = usersPerfil.GroupBy(c => new { c.Usuario.CorreoElectronico }).Select(c => new CachedUser { Email = c.Key.CorreoElectronico }).ToList();

            HelperFunciones funciones = new HelperFunciones();
            var mensaje = "<h3>Estimado(a).</h3> <br>El sistema de Financiamiento y Seguros alerta que se ha activado el contrato Nro. <b>" + contrato.NumeroContrato + "</b><br>";
            mensaje += "<b>Descripción</b>: " + contrato.Descripcion + "<br>";
            mensaje += "<b>Monto</b>: " + String.Format("{0:N0}", contrato.Monto) + "<br>";
            mensaje += "<br><b><i>Este email fue generado de forma automática, por favor no responder.</i></b>";
            bool res = funciones.envioCorreo(mensaje, "", "Alerta Activación de Contrato: " + contrato.NumeroContrato, correos);
            return res;
        }
        public bool NotificaModificacionContrato(int IdContrato,string nombreLog)
        {
            //Notificar Asignación            
            var contrato = db.Contrato.Find(IdContrato);
            int[] perfiles = { (int)Helper.PerfilAcceso.Finanzas, (int)Helper.PerfilAcceso.AdmContabilidad };
            //var parcelaTarea = db.ParcelaTarea.Where(c => c.IdParcelaReserva == parcelareserva.IdParcela && c.IdTarea == IdTarea).FirstOrDefault();
            List<UsuarioPerfil> usersPerfil = new List<UsuarioPerfil>();
            usersPerfil = (from us in db.Usuario.ToList()
                           join up in db.UsuarioPerfil.ToList() on us.IdUsuario equals up.IdUsuario
                           where perfiles.Any(y => y == up.IdPerfil) && us.Activo == true
                           select new UsuarioPerfil { Usuario = us, Perfil = up.Perfil }).ToList();
            var correos = usersPerfil.GroupBy(c => new { c.Usuario.CorreoElectronico }).Select(c => new CachedUser { Email = c.Key.CorreoElectronico }).ToList();

            HelperFunciones funciones = new HelperFunciones();
            var mensaje = "<h3>Estimado(a).</h3> <br>El sistema de Financiamiento y Seguros alerta que se ha Modificado el contrato Nro. <b>" + contrato.NumeroContrato + "</b><br>";
            mensaje += "<b>Descripción</b>: " + contrato.Descripcion + "<br>";
            mensaje += "<b>Modificación</b>: " + nombreLog + "<br>";
            mensaje += "<br><b><i>Este email fue generado de forma automática, por favor no responder.</i></b>";
            bool res = funciones.envioCorreo(mensaje, "", "Alerta Modificación de Contrato: " + contrato.NumeroContrato, correos);
            return res;
        }
        public bool NotificaNuevoActivo(int IdActivo)
        {
            //Notificar Asignación            
            var activo = db.Activo.Find(IdActivo);
            int[] perfiles = { (int)Helper.PerfilAcceso.Seguros, (int)Helper.PerfilAcceso.Finanzas, (int)Helper.PerfilAcceso.AdmContabilidad };
            //var parcelaTarea = db.ParcelaTarea.Where(c => c.IdParcelaReserva == parcelareserva.IdParcela && c.IdTarea == IdTarea).FirstOrDefault();
            List<UsuarioPerfil> usersPerfil = new List<UsuarioPerfil>();
            usersPerfil = (from us in db.Usuario.ToList()
                           join up in db.UsuarioPerfil.ToList() on us.IdUsuario equals up.IdUsuario
                           where perfiles.Any(y => y == up.IdPerfil) && us.Activo == true
                           select new UsuarioPerfil { Usuario = us, Perfil = up.Perfil }).ToList();
            var correos = usersPerfil.GroupBy(c => new { c.Usuario.CorreoElectronico }).Select(c => new CachedUser { Email = c.Key.CorreoElectronico }).ToList();

            HelperFunciones funciones = new HelperFunciones();
            var mensaje = "<h3>Estimado(a).</h3> <br>El sistema de Financiamiento y Seguros alerta que se ha Creado el Activo Nro. <b>" + activo.NumeroInterno + "</b><br>";
            mensaje += "<b>Empresa</b>: " + activo.Empresa.RazonSocial + "<br>";
            mensaje += "<b>Descripción</b>: " + activo.Descripcion + "<br>";
            mensaje += "<b>Familia</b>: " + activo.Familia.NombreFamilia + "<br>";
            mensaje += "<b>Clasificación de Cuenta</b>: " + activo.IdCuenta + "<br>";
            mensaje += "<br><b><i>Este email fue generado de forma automática, por favor no responder.</i></b>";
            bool res = funciones.envioCorreo(mensaje, "", "Alerta Creación de Activo: " + activo.NumeroInterno, correos);
            return res;
        }
    }
}