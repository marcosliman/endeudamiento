using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoSeguro")]
    public class TipoSeguro
    {
        [Key]
        [Display(Name = "Código TipoSeguro")]
        public int IdTipoSeguro { get; set; }
        public string NombreTipoSeguro { get; set; }
        public bool  Activo { get; set; }

    }
}