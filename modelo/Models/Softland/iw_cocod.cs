using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_cocod", Schema = "softland")]
    public class iw_cocod
    {
        [Key]
        [Column(Order = 0)]
        public string TipoDoc { get; set; }
        [Key]
        [Column(Order = 1)]
        public string Concepto { get; set; }
        public string DesCodDr { get; set; }
    }
}
