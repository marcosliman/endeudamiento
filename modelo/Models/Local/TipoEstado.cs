using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoEstado")]
    public class TipoEstado
    {
        [Key]
        [Display(Name = "Código Tipo Estado")]
        public int IdTipoEstado { get; set; }
        public string NombreTipoEstado { get; set; }

    }
}