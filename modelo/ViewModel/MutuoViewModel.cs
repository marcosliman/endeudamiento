using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class MutuoViewModel
    {
        public int IdMutuo { get; set; }
        public int IdEmpresaFinancia { get; set; }
        public string EmpresaFinancia { get; set; }
        public int IdEmpresaReceptora { get; set; }
        public string EmpresaReceptora { get; set; }
        public double TasaMensual { get; set; }
        public double TasaDiaria { get; set; }
        public double MontoPrestamo { get; set; }
        public double CapitalActual { get; set; }
        public double InteresTotal { get; set; }
        public double? Porcentaje { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public string FechaPrestamoStr { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public string ExisteMutuo { get; set; }
        public string TituloBoton { get; set; }
        public bool PuedeProcesar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeEditar { get; set; }
        public int IdTipoMoneda { get; set; }
    }

}
