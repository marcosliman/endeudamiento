using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ReporteActivoFinanciadoViewModel
    {
        public int IdActivo { get; set; }
        public string NumeroActivo { get; set; }
        public string Descripcion { get; set; }
        public string Familia { get; set; }
        public string RazonSocial { get; set; }
        public string NombreBanco { get; set; }
        public string NumeroContrato { get; set; }
        public int Numerocuotas { get; set; }
        public DateTime FechaInicio { get; set; }
        public string FechaInicioStr { get; set; }
        public Double MontoContrato { get; set; }
        public double TasaAnual { get; set; }
        public string Moneda { get; set; }
        public string NumeroPoliza { get; set; }
        public string EstadoActivo { get; set; }
    }

}
