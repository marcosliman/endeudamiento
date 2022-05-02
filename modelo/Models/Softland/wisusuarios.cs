using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("wisusuarios", Schema = "softland")]
    public class wisusuarios
    {
        [Key]        
        public string Usuario { get; set; }       
        public string Nombre { get; set; }
        public string Rut { get; set; }        
    }
}