using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoImpuesto")]
    public class TipoImpuesto
    {
        [Key]
        [Display(Name = "Código TipoImpuesto")]
        public int IdTipoImpuesto { get; set; }
        public string NombreTipoImpuesto { get; set; }
        public bool  Activo { get; set; }

    }
}