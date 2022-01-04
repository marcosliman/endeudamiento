using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoContrato")]
    public class TipoContrato
    {
        [Key]
        [Display(Name = "Código Tipo Contrato")]
        public int IdTipoContrato { get; set; }
        public string NombreTipoContrato { get; set; }
        public bool Activo { get; set; }

    }
}