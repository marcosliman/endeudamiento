using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Proveedor")]
    public class Proveedor
    {
        [Key]
        [Display(Name = "Código Proveedor")]
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public bool  Activo { get; set; }

    }
}