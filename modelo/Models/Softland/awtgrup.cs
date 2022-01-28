using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("awtgrup", Schema = "softland")]
    public class awtgrup
    {
        [Key]
        public string CodGru { get; set; }
        public string DesGru { get; set; }
    }
}