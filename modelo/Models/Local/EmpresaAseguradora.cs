using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("EmpresaAseguradora")]
    public class EmpresaAseguradora
    {
        [Key]
        [Display(Name = "Código EmpresaAseguradora")]
        public int IdEmpresaAseguradora { get; set; }
        public string NombreEmpresaAseguradora { get; set; }
        public bool  Activo { get; set; }

    }
}