using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_gsaen", Schema = "softland")]
    public class iw_gsaen
    {
        [Key]
        [Column(Order = 0)]
        public string Tipo { get; set; }
        [Key]
        [Column(Order = 1)]
        public int NroInt { get; set; }
        public string CodBode { get; set; }
        public decimal Folio { get; set; }
        public string Concepto { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public string Glosa { get; set; }
        public int Orden { get; set; }
        public string CodAux { get; set; }
        public string CodiCC { get; set; }
        public string CodBod { get; set; }
        public decimal AuxGuiaNum { get; set; }
        public decimal? AuxDocNum { get; set; }
        public string Usuario { get; set; }
        public double NetoAfecto { get; set; }
        public double NetoExento { get; set; }
        public double IVA { get; set; }
        public double Total { get; set; }
        public string CentroDeCosto { get; set; }
        public double SubTotal { get; set; }
    }
}
