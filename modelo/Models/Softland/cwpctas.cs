using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("cwpctas", Schema = "softland")]
    public class cwpctas
    {
        [Key]
        public string PCCODI { get; set; }
        public string PCDESC { get; set; }
        public string PCCDOC { get; set; }

        public string PCAUXI { get; set; }
    }
}