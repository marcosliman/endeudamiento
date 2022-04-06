using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("TipoFinanciamiento")]
    public class TipoFinanciamiento
    {
        [Key]
        [Display(Name = "Código TipoFinanciamiento")]
        public int IdTipoFinanciamiento { get; set; }
        public int IdTipoContrato { get; set; }
        [ForeignKey("IdTipoContrato")]
        public virtual TipoContrato TipoContrato { get; set; }
        public string NombreTipoFinanciamiento { get; set; }
        public bool  Activo { get; set; }

    }
}