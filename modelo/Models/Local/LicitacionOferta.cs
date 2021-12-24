using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("LicitacionOferta")]
    public class LicitacionOferta
    {
        [Key]
        [Display(Name = "Código LicitacionOferta")]
        public int IdLicitacionOferta { get; set; }
        public int IdLicitacion { get; set; }
        public int IdBanco { get; set; }
        [ForeignKey("IdBanco")]
        public virtual Banco Banco { get; set; }
        public double TasaMensual { get; set; }
        public double TasaAnual { get; set; }
        public int Plazo { get; set; }
        public int IdEstado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}