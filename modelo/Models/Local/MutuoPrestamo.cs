using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("MutuoPrestamo")]
    public class MutuoPrestamo
    {
        [Key]
        [Display(Name = "Código MutuoPrestamo")]
        public int IdMutuoPrestamo { get; set; }
        public int IdMutuo { get; set; }
        public double MontoPrestamo { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}