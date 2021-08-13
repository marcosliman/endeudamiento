using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_tbode", Schema = "softland")]
    public class iw_tbode
    {
        [Key]
        public string CodBode { get; set; }
        public string DesBode { get; set; }

    }
}
