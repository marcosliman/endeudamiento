using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_tgrupo", Schema = "softland")]
    public class iw_tgrupo
    {
        [Key]
        public string CodGrupo { get; set; }
        public string DesGrupo { get; set; }
    }
}
