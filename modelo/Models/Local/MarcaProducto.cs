using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("MarcaProducto")]
    public class MarcaProducto
    {
        [Key]
        [Display(Name = "Descripción MarcaProducto")]
        public int IdMarcaProducto { get; set; }
        public string DescMarcaProducto { get; set; }
        public bool  Activo { get; set; }

    }
}