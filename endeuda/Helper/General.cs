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

    public enum TipoEstado : int
    {
        Siniestro = 7
    }
    public enum Estado : int
    {
        //General
        Vigente = 1,
        NoVigente = 2,

        ActCreado =3,
        ActDisponible = 4,
        ActLicitacion = 5,
        ActEnContrato = 6,
        ActBaja=7,
        ActEnPoliza=8,

        LicCreada=15,
        LicFinalizada=16,
        LicContrato=17,

        OfeGenerada=25,
        OfeEnContrato=26,
        OfeNoUtilizada=27,

        ConCreado=30,
        ConActivo=31,
        ConFinalizado=32,

        PolCreado = 40,
        PolActivo = 41,
        PolFinalizado = 42,


    }

    public enum TipoContrato : int
    {
        Leasing = 1,
        Contrato = 2
    }

    public enum CategoriaDocumento : int
    {
        ActivoContrato = 1,
        Poliza = 2,
        Siniestro = 3
    }

    public enum TipoFinanciamiento : int
    {
        Leasing = 1,
        EstructuradoConGarantia = 2,
        EstructuradoSinGarantia = 3,
        CapitalDeTrabajoBullet = 4
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