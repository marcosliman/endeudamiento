using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("PolizaActivo")]
    public class PolizaActivo
    {
        [Key]
        [Display(Name = "Código PolizaActivo")]
        public int IdPolizaActivo { get; set; }
        public int IdPoliza { get; set; }
        public int IdActivo { get; set; }
    }
}