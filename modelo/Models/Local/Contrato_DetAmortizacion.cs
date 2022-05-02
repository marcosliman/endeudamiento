using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Contrato_DetAmortizacion")]
    public class Contrato_DetAmortizacion
    {
        [Key]
        public int IdContratoDetAmortizacion { get; set; }
        public int IdContratoAmortizacion { get; set; }
        [ForeignKey("IdContratoAmortizacion")]
        public virtual Contrato_Amortizacion Contrato_Amortizacion { get; set; }
        public int? Anio { get; set; }
        public int IdMes { get; set; }
        [ForeignKey("IdMes")]
        public virtual Mes Mes { get; set; }
        public int CortoPlazo { get; set; }
        public int LargoPlazo { get; set; }
        public DateTime FechaPago { get; set; }
        public int Periodo { get; set; }
        public int Cuota { get; set; }
        public double IvaDiferido { get; set; }
        public double Intereses { get; set; }
        public double Amortizacion { get; set; }
        public double Saldo_Insoluto { get; set; }
        public double Obligacion { get; set; }
        public int EsMes { get; set; }
    }
}