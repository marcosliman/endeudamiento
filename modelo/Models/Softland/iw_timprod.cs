using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_timprod", Schema = "softland")]
    public class iw_timprod
    {
        [Key]
        [Column(Order = 0)]
        public string CodProd { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CodImpto { get; set; }
        public double Usuario { get; set; }
        public double Sistema { get; set; }        

    }
}
