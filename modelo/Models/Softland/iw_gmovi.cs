using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_gmovi", Schema = "softland")]
    public class iw_gmovi
    {
        [Key]
        [Column(Order = 0)]
        public string Tipo { get; set; }
        [Key]
        [Column(Order = 1)]
        public int NroInt { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Linea { get; set; }
        public string CodProd { get; set; }
        public string CodBode { get; set; }
        public double CantIngresada { get; set; }
        public double cantDespachada { get; set; }
        public int Actualizado { get; set; }
        public DateTime Fecha { get; set; }
        public double PreUniMB { get; set; }
        public double TotLinea { get; set; }
        public string DetProd { get; set; }
    }
}
