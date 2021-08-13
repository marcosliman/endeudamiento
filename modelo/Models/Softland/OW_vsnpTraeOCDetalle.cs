using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("OW_vsnpTraeOCDetalle", Schema = "softland")]
    public class OW_vsnpTraeOCDetalle
    {
        [Key]
        [Column(Order = 0)]
        public string NumId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int NumOC { get; set; }
        [Key]
        [Column(Order = 2)]
        public int NumLinea { get; set; }
        public int NumInterOC { get; set; }
        public string CodProd { get; set; }
        public DateTime FechaEnt { get; set; }
        public double Cantidad { get; set; }
        public double Saldo { get; set; }
        public string CodiCC { get; set; }
        public double PrecioUnit { get; set; }
        public double TotalDescto { get; set; }
        public double ValorTotal { get; set; }        
        public string DetProd { get; set; }
        public string CodUMed { get; set; }       

    }
}