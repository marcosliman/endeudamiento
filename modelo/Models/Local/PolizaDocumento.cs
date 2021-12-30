using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("PolizaDocumento")]
    public class PolizaDocumento
    {
        [Key]
        [Display(Name = "Código PolizaDocumento")]
        public int IdPolizaDocumento { get; set; }
        public int IdPoliza { get; set; }
        public int IdTipoDocumento { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}