using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("PolizaActivo")]
    public class PolizaActivo
    {
        [Key]
        [Display(Name = "Código PolizaActivo")]
        public int IdPolizaActivo { get; set; }
        public int IdPoliza { get; set; }
        [ForeignKey("IdPoliza")]
        public virtual Poliza Poliza { get; set; }
        public int IdActivo { get; set; }
        [ForeignKey("IdActivo")]
        public virtual Activo Activo { get; set; }
        public string Beneficiario { get; set; }
        public string RutBeneficiario { get; set; }
        public int? PaginaInicial { get; set; }
        public int? PaginaTermino { get; set; }
        public string CodAux { get; set; }
        public double? ValorPrima { get; set; }
        public string NumeroEndoso { get; set; }
        public string FechaEndoso { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}