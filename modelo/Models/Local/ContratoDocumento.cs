using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("ContratoDocumento")]
    public class ContratoDocumento
    {
        [Key]
        [Display(Name = "Código ContratoDocumento")]
        public int IdContratoDocumento { get; set; }
        public int IdContrato { get; set; }
        public int IdTipoDocumento { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}