using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Estado")]
    public class Estado
    {
        [Key]
        [Display(Name = "Código Estado")]
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public int IdTipoEstado { get; set; }

    }
}