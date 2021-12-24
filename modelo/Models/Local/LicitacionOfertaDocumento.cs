using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("LicitacionOfertaDocumento")]
    public class LicitacionOfertaDocumento
    {
        [Key]
        [Display(Name = "Código LicitacionOfertaDocumento")]
        public int IdLicitacionOfertaDocumento { get; set; }
        public int IdLicitacionOferta { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}