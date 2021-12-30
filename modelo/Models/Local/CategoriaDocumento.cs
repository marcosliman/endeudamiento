using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("CategoriaDocumento")]
    public class CategoriaDocumento
    {
        [Key]
        [Display(Name = "Código Categoria Documento")]
        public int IdCategoriaDocumento { get; set; }
        public string NombreCategoriaDocumento { get; set; }       
        public bool Activo { get; set; }        
    }
}