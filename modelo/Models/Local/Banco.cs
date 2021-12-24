using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Banco")]
    public class Banco
    {
        [Key]
        [Display(Name = "Código Banco")]
        public int IdBanco { get; set; }
        public string NombreBanco { get; set; }
        public bool  Activo { get; set; }

    }
}