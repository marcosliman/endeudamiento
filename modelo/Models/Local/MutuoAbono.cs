using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("MutuoAbono")]
    public class MutuoAbono
    {
        [Key]
        [Display(Name = "Código MutuoAbono")]
        public int IdMutuoAbono { get; set; }
        public int IdMutuo { get; set; }
        public double MontoAbono { get; set; }
        public DateTime FechaAbono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}