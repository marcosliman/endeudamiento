using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_nivst", Schema = "softland")]
    public class iw_nivst
    {
        [Key]
        [Column(Order = 0)]
        public string CodProd { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CodBode { get; set; }
        public double NivMin { get; set; }
        public double NivRep { get; set; }
        public double NivMax { get; set; }
        public string UbicProd { get; set; }
        public double NivelStock { get; set; }

    }
}
