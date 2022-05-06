using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ReporteContratoViewModel
    {
        public int IdContrato { get; set; }
        public string Empresa { get; set; }
        public string Acreedor { get; set; }
        public string NumeroContrato { get; set; }
        public double? TasaMensual { get; set; }
        public double? TasaAnual { get; set; }
        public string Moneda { get; set; }
        public double? Total { get; set; }
        public double? Total1 { get; set; }
        public double? Total2 { get; set; }
        public double? Total3 { get; set; }
        public double? TotalGeneral { get; set; }
    }

}
