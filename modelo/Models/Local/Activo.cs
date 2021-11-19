using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Activo")]
    public class Activo
    {
        [Key]
        [Display(Name = "Código Activo")]
        public int IdActivo { get; set; }
        public int? NumeroInterno { get; set; }
        public string CodSoftland { get; set; }
        public string Descripcion { get; set; }
        public string Capacidad { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Motor { get; set; }
        public string Serie { get; set; }
        public int? Anio { get; set; }
        public double? Valor { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuarioRegistro { get; set; }
    }
}