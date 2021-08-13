using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_tumed", Schema = "softland")]
    public class iw_tumed
    {
        [Key]
        public string CodUMed { get; set; }
        public string DesUMed { get; set; }
        public string DesSubUMed { get; set; }
    }
}
