using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("iw_tprod", Schema = "softland")]
    public class iw_tprod
    {
        [Key]
        public string CodProd { get; set; }
        public string DesProd { get; set; }
        public string CodBarra { get; set; }
        public string CodUMed { get; set; }
        public int Inventariable { get; set; }
        public string CodGrupo { get; set; }
        public string CodSubGr { get; set; }
        public int Inactivo { get; set; }
        public int esParaCompra { get; set; }
        public string CtaActivo { get; set; }
        public string CtaVentas { get; set; }
        public string CtaGastos { get; set; }
        public string CtaCosto { get; set; }
        public string CtaDevolucion { get; set; }
        public double PrecioVta { get; set; }
        public double NivMin { get; set; }
        public double NivRep { get; set; }
        public double NivMax { get; set; }
        public int esParaVenta { get; set; }
        public int Impuesto { get; set; }
        public string Usuario { get; set; }
        public string Proceso { get; set; }
        public DateTime FechaUlMod { get; set; }
    }
}
