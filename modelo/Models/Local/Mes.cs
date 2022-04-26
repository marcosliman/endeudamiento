using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Mes")]
    public class Mes
    {
        [Key]
        [Display(Name = "Código Mes")]
        public int IdMes { get; set; }
        public string NombreMes { get; set; }
    }
}