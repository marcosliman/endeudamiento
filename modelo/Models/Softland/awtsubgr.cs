using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("awtsubgr", Schema = "softland")]
    public class awtsubgr
    {
        [Key]
        public string CodGru { get; set; }
        public string CodSGru { get; set; }
        public string DesSGru { get; set; }
    }
}