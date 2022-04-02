using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Contrato_Amortizacion")]
    public class Contrato_Amortizacion
    {
        [Key]
        [Display(Name = "Código Contrato Amortización")]
        public int IdContratoAmortizacion { get; set; }
        public int IdContrato { get; set; }
        [ForeignKey("IdContrato")]
        public virtual Contrato Contrato { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

        public double? TasaAnual { get; set; }
        public double? TasaMensual { get; set; }

    }
}