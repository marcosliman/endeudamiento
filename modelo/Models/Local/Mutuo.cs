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
        public int IdEmpresaReceptora { get; set; }
        public double TasaMensual { get; set; }
        public double TasaDiaria { get; set; }
        public double MontoPrestamo { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}