
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoPerdida")]
    public class TipoPerdida
    {
        [Key]
        [Display(Name = "Código TipoPerdida")]
        public int IdTipoPerdida { get; set; }
        public string NombreTipoPerdida { get; set; }
        public bool  Activo { get; set; }

    }
}