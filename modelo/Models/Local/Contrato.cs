using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Contrato")]
    public class Contrato
    {
        [Key]
        [Display(Name = "Código Contrato")]
        public int IdContrato { get; set; }
        public int IdTipoContrato { get; set; }
        public int? IdLicitacionOferta { get; set; }
        [ForeignKey("IdLicitacionOferta")]
        public virtual LicitacionOferta LicitacionOferta { get; set; }
        public int IdEmpresa { get; set; }
        [ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
        public string NumeroContrato { get; set; }
        public string MotivoEleccion { get; set; }
        public int IdBanco { get; set; }
        [ForeignKey("IdBanco")]
        public virtual Banco Banco { get; set; }
        public int IdTipoFinanciamiento { get; set; }
        [ForeignKey("IdTipoFinanciamiento")]
        public virtual TipoFinanciamiento TipoFinanciamiento { get; set; }
        public int IdTipoImpuesto { get; set; }
        [ForeignKey("IdTipoImpuesto")]
        public virtual TipoImpuesto TipoImpuesto { get; set; }
        public string TipoGarantia { get; set; }
        public double TasaMensual { get; set; }
        public double TasaAnual { get; set; }
        public int Plazo { get; set; }
        public double Monto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public int IdEstado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdTipoMoneda { get; set; }
        [ForeignKey("IdTipoMoneda")]
        public virtual TipoMoneda TipoMoneda { get; set; }
        public string Descripcion { get; set; }
        public int? IdFamilia { get; set; }
        [ForeignKey("IdFamilia")]
        public virtual Familia Familia { get; set; }
    }
}