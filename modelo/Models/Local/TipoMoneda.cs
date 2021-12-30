using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoMoneda")]
    public class TipoMoneda
    {
        [Key]
        [Display(Name = "Código TipoMoneda")]
        public int IdTipoMoneda { get; set; }
        public string NombreTipoMoneda { get; set; }
        public bool  Activo { get; set; }

    }
}