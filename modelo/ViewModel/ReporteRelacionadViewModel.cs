using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ReporteRelacionadViewModel
    {
        public int? IdMutuo { get; set; }
        public string EmiteDeuda { get; set; }
        public string RecibeDeuda { get; set; }
        public double? MontoPrestamo { get; set; }
        public DateTime? FechaPrestamo { get; set; }
        public double? InteresTotal { get; set; }
        public double? MontoAbono { get; set; }
        public double? Saldo { get; set; }
       
    }

}
