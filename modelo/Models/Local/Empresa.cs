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
        public string IdTributario { get; set; }
        public string AliasEmpresa { get; set; }
        public string BaseSoftland { get; set; }
        public bool Activo { get; set; }
        public string CtaLeasingDirecto { get; set; }
        public string CtaLeasingIndirecto { get; set; }
    }
}