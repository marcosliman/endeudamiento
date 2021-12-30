
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Familia")]
    public class Familia
    {
        [Key]
        [Display(Name = "Código Familia")]
        public int IdFamilia { get; set; }
        public string NombreFamilia { get; set; }
        public bool  Activo { get; set; }

    }
}