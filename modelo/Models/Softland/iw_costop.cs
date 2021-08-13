using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_costop", Schema = "softland")]
    public class iw_costop
    {
        [Key]
        [Column(Order = 0)]
        public string CodProd { get; set; }
        [Key]
        [Column(Order = 1)]
        public DateTime Fecha { get; set; }        
        public double CostoUnitario { get; set; }
        public double Stock { get; set; }
    }
}
