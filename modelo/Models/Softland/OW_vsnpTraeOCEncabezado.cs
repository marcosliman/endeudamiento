using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("OW_vsnpTraeOCEncabezado", Schema = "softland")]
    public class OW_vsnpTraeOCEncabezado
    {
        [Key]
        [Column(Order = 0)]
        public int NumOC { get; set; }
        [Key]
        [Column(Order = 1)]
        public int NumInterOC { get; set; }
        public DateTime FechaOC { get; set; }
        public string CodEstado { get; set; }
        public string CodAux { get; set; }
        public DateTime FecFinalOC { get; set; }
        public string CodConP { get; set; }
        public string CodArn { get; set; }
        public string CodiCC { get; set; }
        public double SubTotalOC { get; set; }
        public double TotalDescto { get; set; }
        public double NetoAfecto { get; set; }
        public double NetoExento { get; set; }
        public double ValorTotOC { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string CveDes { get; set; }
        public string DesArn { get; set; }
        public string DescCC { get; set; }
        public int? NumReq { get; set; }
        public string NomAux { get; set; }
        public string RutAux { get; set; }
        public string DirAux { get; set; }
        public string FonAux1 { get; set; }
        public string DescEstado { get; set; }
        public string ObservOC { get; set; }
        public string DesEtapaOC { get; set; }
        public string Apro { get; set; }
        public string Etapa { get; set; }
        public double? Total { get; set; }
        public string Estado { get; set; }

    }
}