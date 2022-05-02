using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class PolizaViewModel
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
        //public string Beneficiario { get; set; }
        //public string RutBeneficiario { get; set; }
        public DateTime FechaEnvioBanco { get; set; }
        public string FechaEnvioBancoStr { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string ExistePoliza { get; set; }
        public string TituloBoton { get; set; }
        public bool PuedeEliminar { get; set; }
        public string Familia { get; set; }
    }

}
