using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Licitacion")]
    public class Licitacion
    {
        [Key]
        [Display(Name = "Código Licitacion")]
        public int IdLicitacion { get; set; }
        public int Correlativo { get; set; }
        public string Autogenerado { get; set; }
        public int IdEmpresa { get; set; }
        [ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
        public int IdTipoFinanciamiento { get; set; }
        [ForeignKey("IdTipoFinanciamiento")]
        public virtual TipoFinanciamiento TipoFinanciamiento { get; set; }
        public double Monto { get; set; }
        public int IdEstado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}