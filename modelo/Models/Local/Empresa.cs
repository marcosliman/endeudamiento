using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        [Display(Name = "Código Empresa")]
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string IdTributario { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string AliasEmpresa { get; set; }
        public string BaseSoftland { get; set; }
        public int PorPagar { get; set; }
        public int Vencido { get; set; }
        public int Total { get; set; }
        public bool Activo { get; set; }
        public bool TieneBodega { get; set; }
        public string CtaActivo { get; set; }
        public string CtaVentas { get; set; }
        public string CtaGastos { get; set; }
        public string CtaCosto { get; set; }
        public string CtaDevolucion { get; set; }
        public string CodSubGrNuevo { get; set; }
        
    }
}