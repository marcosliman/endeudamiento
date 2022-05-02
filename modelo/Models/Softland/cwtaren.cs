using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("cwtaren", Schema = "softland")]
    public class cwtaren
    {
        [Key]        
        public string CodArn { get; set; }       
        public string DesArn { get; set; }        
    }
}