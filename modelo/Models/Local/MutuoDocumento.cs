using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("MutuoDocumento")]
    public class MutuoDocumento
    {
        [Key]
        [Display(Name = "Código MutuoDocumento")]
        public int IdMutuoDocumento { get; set; }
        public int IdMutuo { get; set; }
        public int IdTipoDocumento { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}