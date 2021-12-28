using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class LicitacionOfertaViewModel
    {
        public int IdLicitacionOferta { get; set; }
        public int IdLicitacion { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdTipoFinanciamiento { get; set; }
        public int IdBanco { get; set; }
        public string NombreBanco { get; set; }
        public double? TasaMensual { get; set; }
        public double? TasaAnual { get; set; }
        public int? Plazo { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public List<LicitacionOfertaDocumento> Archivos { get; set; }
        public string TituloBoton { get; set; }
    }

}
