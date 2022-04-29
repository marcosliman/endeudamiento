using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("ModeloProducto")]
    public class ModeloProducto
    {
        [Key]
        [Display(Name = "Descripción ModeloProducto")]
        public int IdModeloProducto { get; set; }
        public string DescModeloProducto { get; set; }
        public bool  Activo { get; set; }

    }
}