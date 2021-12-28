using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("ContratoActivoDocumento")]
    public class ContratoActivoDocumento
    {
        [Key]
        [Display(Name = "Código ContratoActivoDocumento")]
        public int IdContratoActivoDocumento { get; set; }
        public int IdContratoActivo { get; set; }
        public int IdTipoDocumento { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}