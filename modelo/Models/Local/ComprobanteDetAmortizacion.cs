using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("ComprobanteDetAmortizacion")]
    public class ComprobanteDetAmortizacion
    {
        [Key]
        public int IdComprobanteDetAmortizacion { get; set; }
        public int IdContratoDetAmortizacion { get; set; }
        [ForeignKey("IdContratoDetAmortizacion")]
        public virtual Contrato_DetAmortizacion Contrato_DetAmortizacion { get; set; }       
        public string CpbNum { get; set; }
        public string CpbAno { get; set; }
        public double? Monto { get; set; }
        public string CpbGlo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string CpbMes { get; set; }
        public DateTime? CpbFec { get; set; }
        public string BaseSoftland { get; set; }

        [DefaultValue(false)]
        public bool? EsCreado { get; set; }
        [DefaultValue(false)]
        public bool? AsociadoManual { get; set; }
        public string CpbTip { get; set; }
    }
}