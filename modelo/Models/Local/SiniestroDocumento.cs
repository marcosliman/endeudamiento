using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("SiniestroDocumento")]
    public class SiniestroDocumento
    {
        [Key]
        [Display(Name = "Código SiniestroDocumento")]
        public int IdSiniestroDocumento { get; set; }
        public int IdSiniestro { get; set; }
        public int IdTipoDocumento { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}