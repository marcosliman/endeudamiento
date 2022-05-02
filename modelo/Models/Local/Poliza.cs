using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Poliza")]
    public class Poliza
    {
        [Key]
        [Display(Name = "Código Poliza")]
        public int IdPoliza { get; set; }
        public int IdEmpresa { get; set; }
        [ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
        public string NumeroPoliza { get; set; }
        public int IdTipoSeguro { get; set; }
        [ForeignKey("IdTipoSeguro")]
        public virtual TipoSeguro TipoSeguro { get; set; }
        public int IdEmpresaAseguradora { get; set; }
        [ForeignKey("IdEmpresaAseguradora")]
        public virtual EmpresaAseguradora EmpresaAseguradora { get; set; }
        public double MontoAsegurado { get; set; }
        public double PrimaMensual { get; set; }
        public int NumeroPagos { get; set; }
        public int IdTipoMoneda { get; set; }
        [ForeignKey("IdTipoMoneda")]
        public virtual TipoMoneda TipoMoneda { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaEnvioBanco { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}