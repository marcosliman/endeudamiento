using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Inmobiliaria
{
    [Table("SII_ValoresUF")]
    public class SII_ValoresUF
    {
        [Key]
        [Display(Name = "Código Valor")]
        public int IdValorUF { get; set; }
        public double Valor { get; set; }
        public DateTime Fecha { get; set; }
    }
}