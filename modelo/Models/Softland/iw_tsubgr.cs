using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_tsubgr", Schema = "softland")]
    public class iw_tsubgr
    {
        [Key]
        public string CodSubGr { get; set; }
        public string DesSubGr { get; set; }        

    }
}
