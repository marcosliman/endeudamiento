using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("LicitacionActivo")]
    public class LicitacionActivo
    {
        [Key]
        [Display(Name = "Código LicitacionActivo")]
        public int IdLicitacionActivo { get; set; }
        public int IdLicitacion { get; set; }
        public int IdActivo { get; set; }
    }
}