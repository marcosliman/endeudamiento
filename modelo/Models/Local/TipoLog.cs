using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoLog")]
    public class TipoLog
    {
        [Key]
        [Display(Name = "Código TipoLog")]
        public int IdTipoLog { get; set; }
        public string NombreTipoLog { get; set; }


    }
}