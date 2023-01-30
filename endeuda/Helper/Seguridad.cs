using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace tesoreria.Helper
{
    public class PermisoUsuario
    {
        public Nullable<int> IdPermisoPerfil { get; set; }

        public Nullable<int> IdMenu { get; set; }

        public Nullable<int> IdMenuPadre { get; set; }

        public Nullable<int> IdPerfil { get; set; }
        //public List<Perfil> IdPerfil { get; set; }

        public string NombreVista { get; set; }
        public string NombreMenu { get; set; }

        public bool Crear { get; set; }

        public bool Editar { get; set; }

        public bool Eliminar { get; set; }

        public bool Acceder { get; set; }

        public int NivelMenu { get; set; }

        public Nullable<int> IdEstadoVigencia { get; set; }
    }

    public class Menu
    {
        public int IdMenu { get; set; }

        public Nullable<int> OrdenMenu { get; set; }
        public string Nombre { get; set; }

        public string NombreMenu { get; set; }

        public string Icono { get; set; }

        public string UrlMenu { get; set; }

        public int? IdMenuPadre { get; set; }

        public List<Menu> Hijos { get; set; }
        public int NivelMenu { get; set; }

        //public bool EnMenu { get; set; }
    }

    public class Seguridad
    {
        public List<PermisoUsuario> Permisos { get; set; }

        public List<Menu> MenuUsuario { get; set; }

        //public List<RolUsuario> Roles { get; set; }

        public Nullable<int> IdUsuario { get; set; }

        //public int? IdEmpresa { get; set; }

        public string UserName { get; set; }

        public List<UserProg> UsuarioPrograma { get; set; }

        public bool TienePermiso(string accion)
        {
            return false;
        }

        public bool TienePermiso(string vista, TipoAcceso tipoAcceso)
        {
            var permiso = Permisos.FirstOrDefault(r => r.NombreVista == vista);
            if (permiso == null)
            {
                return false;
            }
            switch (tipoAcceso)
            {
                case TipoAcceso.Acceder:
                    return permiso.Acceder;
                case TipoAcceso.Crear:
                    return permiso.Crear;
                case TipoAcceso.Editar:
                    return permiso.Editar;
                case TipoAcceso.Eliminar:
                    return permiso.Eliminar;
                //case TipoAcceso.Nacional:
                //    return permiso.Nacional;
                default:
                    return false;
            }
        }
        public string Nombre { get; set; }
        public bool Interno { get; set; }
        public decimal? EMPR_ID { get; set; }
        public string RutEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public List<Perfil> IdPerfil { get; set; }
        public int IdTemporada { get; set; }
        public int? IdRegion { get; set; }
        public List<UserComuna> UsuarioComuna { get; set; }
        public List<Empresa> empresas { get; set; }
        public bool multiEmpresa { get; set; }
        public string origenAcceso { get; set; }
        public string UsuarioSAG { get; set; }
        public int? IdUsuarioSAG { get; set; }
        public List<Perfil> PerfilAll { get; set; }
        public string AccountId { get; set; }
    }

    public enum PerfilAcceso : int
    {
        Administrador = 1,
        Licitaciones = 2,
        AdmControlInterno = 3,
        AdmContabilidad = 4,
        AnalistaCreaEquipos = 5,
        Seguros = 6,
        Finanzas = 7
    }

    public enum TipoAcceso : int
    {
        Crear = 1,
        Editar = 2,
        Eliminar = 3,
        Acceder = 4//,
        //Nacional = 5
    }

    public class RolUsuario
    {
        public int IdRol { get; set; }

        public string Nombre { get; set; }
    }

    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int IdPerfil { get; set; }
        public string CorreoElectronico { get; set; }
    }

    public class Perfil
    {
        public int IdPerfil { get; set; }
        public decimal EMPR_ID { get; set; }
    }

    public class UserProg
    {
        public int IdPrograma { get; set; }
    }

    public class UserComuna
    {
        public decimal IdComuna { get; set; }
    }
    public class Empresa
    {
        public decimal EMPR_ID { get; set; }
        public string RazonSocial { get; set; }
        public string RutEmpresa { get; set; }
        public bool DeTrazabilidad { get; set; }
    }

}