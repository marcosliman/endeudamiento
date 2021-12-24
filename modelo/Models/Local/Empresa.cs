using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        [Display(Name = "Código Empresa")]
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public bool Activo { get; set; }
      
    }
}