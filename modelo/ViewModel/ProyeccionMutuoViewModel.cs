using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class ProyeccionMutuoViewModel
    {
        public int IdMutuo { get; set; }
        public int item { get; set; }
        public DateTime FechaInicio { get; set; }
        public string FechaInicioStr { get; set; }
        public DateTime FechaTermino { get; set; }
        public string FechaTerminoStr { get; set; }
        public int CantidadDias { get; set; }
        public double Monto { get; set; }
        public double Interes { get; set; }
        public double MontoTotal { get; set; }
        public double InteresTotal { get; set; }
        public double MontoAmortizacion { get; set; }
        public double MontoPrestamo { get; set; }
        public double InteresNuevo { get; set; }
        public DateTime? FechaNuevo { get; set; }
        public string FechaNuevoStr { get; set; }
        public int CantidadDiasNuevo { get; set; }
        public string ExisteMutuo { get; set; }
    }

}
