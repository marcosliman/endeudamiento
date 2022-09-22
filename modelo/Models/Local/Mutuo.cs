using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Mutuo")]
    public class Mutuo
    {
        [Key]
        [Display(Name = "Código Mutuo")]
        public int IdMutuo { get; set; }
        public int IdEmpresaFinancia { get; set; }
        [ForeignKey("IdEmpresaFinancia")]
        public virtual Empresa EmpresaFinancia { get; set; }
        public int IdEmpresaReceptora { get; set; }
        [ForeignKey("IdEmpresaReceptora")]
        public virtual Empresa EmpresaReceptora { get; set; }
        public double TasaMensual { get; set; }
        public double TasaDiaria { get; set; }
        public double MontoPrestamo { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public double CapitalActual { get; set; }
        public double InteresTotal { get; set; }
        public int IdTipoMoneda { get; set; }
        public double? ValorCambio { get; set; }
    }
}