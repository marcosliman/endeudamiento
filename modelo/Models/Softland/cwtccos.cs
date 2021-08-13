using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("cwtccos", Schema = "softland")]
    public class cwtccos
    {
        [Key]
        public string CodiCC { get; set; }
        public string DescCC { get; set; }
        public int NivelCC { get; set; }
        public string Activo { get; set; }
    }
}
