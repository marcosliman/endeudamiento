using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ConsolidadoLeasingViewModel
    {
        public int IdContrato { get; set; }
        public int IdTipoContrato { get; set; }
        public int? IdLicitacionOferta { get; set; }
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string NumeroContrato { get; set; }
        public string MotivoEleccion { get; set; }
        public int IdBanco { get; set; }
        public string NombreBanco { get; set; }
        public int IdTipoFinanciamiento { get; set; }
        public string NombreTipoFinanciamiento { get; set; }
        public int IdTipoImpuesto { get; set; }
        public string TipoGarantia { get; set; }
        public double? TasaMensual { get; set; }
        public double? TasaAnual { get; set; }
        public int? Plazo { get; set; }
        public double? Monto { get; set; }
        public DateTime FechaInicio { get; set; }
        public string FechaInicioStr { get; set; }
        public DateTime FechaTermino { get; set; }
        public string FechaTerminoStr { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string ExisteContrato { get; set; }
        public string TituloBoton { get; set; }
        public string EsLicitacion { get; set; }
        public bool PuedeEliminar { get; set; }
        public string Descripcion { get; set; }
        public string DescActivo { get; set; }
    }

}
