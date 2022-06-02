using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class SiniestroViewModel
    {
        public int IdSiniestro { get; set; }
        public int IdPolizaActivo { get; set; }
        public string NumeroLiquidacion { get; set; }
        public double? MontoLiquidacion { get; set; }
        public DateTime? FechaDeclaracion { get; set; }
        public string NumeroSiniestro { get; set; }
        public DateTime? FechaSiniestro { get; set; }
        public string FechaSiniestroStr { get; set; }
        public string DetalleSiniestro { get; set; }
        public int IdPerdidaReclamada { get; set; }
        public int IdPerdidaDeterminada { get; set; }
        public double? MontoReclamado { get; set; }
        public int IdEstado { get; set; }
        public string Liquidador { get; set; }
        public string Infraseguro { get; set; }
        public double? Deducible { get; set; }
        public double? Indemnizacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string ExisteSiniestro { get; set; }
        public string TituloBoton { get; set; }
        public bool PuedeEliminar { get; set; }
    }

}
