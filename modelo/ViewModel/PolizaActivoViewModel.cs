using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class PolizaActivoViewModel
    {
        public int IdPoliza { get; set; }
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string NumeroPoliza { get; set; }
        public int IdTipoSeguro { get; set; }
        public string NombreTipoSeguro { get; set; }
        public int IdEmpresaAseguradora { get; set; }
        public string NombreEmpresaAseguradora { get; set; }
        public double? MontoAsegurado { get; set; }
        public double? PrimaMensual { get; set; }
        public int? NumeroPagos { get; set; }
        public int IdTipoMoneda { get; set; }
        public string NombreTipoMoneda { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string FechaVencimientoStr { get; set; }
        public string Beneficiario { get; set; }
        public string RutBeneficiario { get; set; }
        public DateTime FechaEnvioBanco { get; set; }
        public string FechaEnvioBancoStr { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

        public int? IdPolizaActivo { get; set; }
        public int? NumeroInterno { get; set; }
        public string CodSoftland { get; set; }
        public int? IdFamilia { get; set; }
        public string Familia { get; set; }
        public string Descripcion { get; set; }
        public string Capacidad { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int? Anio { get; set; }
        public string Grupo { get; set; }
        public string SubGrupo { get; set; }
        public int? IdCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double? Valor { get; set; }
        public int? IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string NumeroFactura { get; set; }
        public string Motor { get; set; }
        public string Chasis { get; set; }
        public string Serie { get; set; }
        public string Patente { get; set; }
        public string Glosa { get; set; }
        public string NombreEstadoActivo { get; set; }
        public DateTime? FechaRegistroActivo { get; set; }
        public string FechaRegistroActivoStr { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string FechaBajaStr { get; set; }
        public string NumeroLeasing { get; set; }
        
    }

}
