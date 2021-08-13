using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace tesoreria.Helper
{
    internal static class Global
    {
        public static string stringConexion = ConfigurationManager.ConnectionStrings["ErpContext"].ConnectionString;
        public static string codChile = "CL";        
    }
    public class Constantes
    {
        public const string classBotonActivo = "btn btn-primary btn-sm";
        public const string classBotonInactivo = "btn btn-danger btn-sm";
        public const string iconoBotonBuscar = "fa fa-search";
        public const string iconoBotonNuevo = "fa fa-plus";
        public const string iconoBotonGrabar = "fa fa-floppy-o";
        public const string iconoBotonEditar = "fa fa-floppy-o";
        public const string iconoBotonEliminar = "fa fa-remove";
        public const string iconoBotonVolver = "fa fa-repeat";
        public const int IdEmpresaPrincipal = 1;
    }

    public enum Estado : int
    {
        //General
        Activo = 1,
        Inactivo = 2,
        Programado=3,
        ValidadoTesorero = 4,
        ValidadoContralor = 5,
        Pagado=6,
        Finalizado=7,
        Rechazado=8
    }
    public enum EstadoControlInventario : int
    {       
        Creado = 10
    }
    public enum EstadoSolCreaProd : int
    {
        Creada = 20,
        Enviada=21,
        Aprobada=22,
        Rechazada=23
    }
    public enum Perfilles : int
    {        
        Administrador = 1,
        Solicitante = 2,
        Aprobador = 3,
        Bodeguero = 4,
        JefeBodega = 5,
        CreadorProd=6
    }
}