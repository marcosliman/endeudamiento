using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("ContratoActivo")]
    public class ContratoActivo
    {
        [Key]
        [Display(Name = "Código ContratoActivo")]
        public int IdContratoActivo { get; set; }
        public int IdContrato { get; set; }
        public int IdActivo { get; set; }
    }
}