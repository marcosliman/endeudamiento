using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Moneda")]
    public class Moneda
    {
        [Key]
        [Display(Name = "Codigo Moneda")]
        public int IdMoneda { get; set; }
        public string NombreMoneda { get; set; }
        public bool Activo { get; set; }

    }
}