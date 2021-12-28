
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoDocumento")]
    public class TipoDocumento
    {
        [Key]
        [Display(Name = "Código TipoDocumento")]
        public int IdTipoDocumento { get; set; }
        public int IdCategoriaDocumento { get; set; }
        public string NombreTipoDocumento { get; set; }
        public bool  Activo { get; set; }

    }
}