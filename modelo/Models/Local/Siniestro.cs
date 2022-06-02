using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Siniestro")]
    public class Siniestro
    {
        [Key]
        [Display(Name = "Código Siniestro")]
        public int IdSiniestro { get; set; }
        public int IdPolizaActivo { get; set; }
        [ForeignKey("IdPolizaActivo")]
        public virtual PolizaActivo PolizaActivo { get; set; }
        public string NumeroLiquidacion { get; set; }
        public double? MontoLiquidacion { get; set; }
        public DateTime FechaDeclaracion { get; set; }
        public string NumeroSiniestro { get; set; }
        public DateTime FechaSiniestro { get; set; }
        public string DetalleSiniestro { get; set; }
        public double? MontoReclamado { get; set; }
        public int IdPerdidaReclamada { get; set; }
        [ForeignKey("IdPerdidaReclamada")]
        public virtual TipoPerdida TipoPerdidaReclamada { get; set; }
        public int IdPerdidaDeterminada { get; set; }
        [ForeignKey("IdPerdidaDeterminada")]
        public virtual TipoPerdida TipoPerdidaDeterminada { get; set; }
        public int IdEstado { get; set; }
        public string Liquidador { get; set; }
        public string Infraseguro { get; set; }
        public double? Deducible { get; set; }
        public double? Indemnizacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}