using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("soempre", Schema = "softland")]
    public class soempre
    {
        [Key]        
        public string RutE { get; set; }       
        public string NomB { get; set; }
        public string Giro { get; set; }
        public string Dire { get; set; }
        public string Regi { get; set; }
        public string Ciud { get; set; }
        public string Comu { get; set; }
        public string Fono { get; set; }
    }
}