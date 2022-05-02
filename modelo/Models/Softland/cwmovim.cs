using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;//valores por defecto
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("cwmovim", Schema = "softland")]
    public class cwmovim
    {
        [Key]
        [Column(Order = 0)]
        public string CpbAno { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CpbNum { get; set; }
        [Key]
        [Column(Order = 2)]
        public double MovNum { get; set; }
        [DefaultValue("000")]
        public string AreaCod { get; set; }
        public string PctCod { get; set; }
        public DateTime? CpbFec { get; set; }
        public string CpbMes { get; set; }
        [DefaultValue("000")]
        public string CvCod { get; set; }
        [DefaultValue("0000")]
        public string VendCod { get; set; }
        [DefaultValue("000")]
        public string UbicCod { get; set; }
        [DefaultValue("0000000000")]
        public string CajCod { get; set; }
        [DefaultValue("000")]
        public string IfCod { get; set; }
        [DefaultValue(0)]
        public double? MovIfCant { get; set; }
        [DefaultValue("00000000")]
        public string DgaCod { get; set; }
        [DefaultValue(0)]
        public double? MovDgCant { get; set; }
        [DefaultValue("00000000")]
        public string CcCod { get; set; }
        [DefaultValue("00")]
        public string TipDocCb { get; set; }
        [DefaultValue(0)]
        public double? NumDocCb { get; set; }
        [DefaultValue("0000000000")]
        public string CodAux { get; set; }
        [DefaultValue("00")]
        public string TtdCod { get; set; }
        [DefaultValue(0)]
        public double? NumDoc { get; set; }
        public DateTime MovFe { get; set; }
        public DateTime MovFv { get; set; }
        [DefaultValue("00")]
        public string MovTipDocRef { get; set; }
        [DefaultValue(0)]
        public double? MovNumDocRef { get; set; }
        [DefaultValue(0)]
        public double? MovDebe { get; set; }
        [DefaultValue(0)]
        public double? MovHaber { get; set; }
        [DefaultValue(" ")]
        public string MovGlosa { get; set; }
        [DefaultValue("01")]
        public string MonCod { get; set; }
        [DefaultValue(0)]
        public double? MovEquiv { get; set; }
        [DefaultValue(0)]
        public double? MovDebeMa { get; set; }
        [DefaultValue(0)]
        public double? MovHaberMa { get; set; }
        public string MovNumCar { get; set; }
        public string MovTC { get; set; }
        public string MovNC { get; set; }
        public string MovIPr { get; set; }
        [DefaultValue("S")]
        public string MovAEquiv { get; set; }
        public DateTime? FecPag { get; set; }
        public string CODCPAG { get; set; }
        [DefaultValue(0)]
        public double? CbaNumMov { get; set; }
        [DefaultValue(0)]
        public int? CbaAnoC { get; set; }
        [DefaultValue("S")]
        public string GrabaDLib { get; set; }
        public string CpbOri { get; set; }
        public string CodBanco { get; set; }
        public string CodCtaCte { get; set; }
        [DefaultValue(0)]
        public double? MtoTotal { get; set; }
        [DefaultValue(0)]
        public int? Cuota { get; set; }
        [DefaultValue(0)]
        public int? CuotaRef { get; set; }
        [DefaultValue("N")]
        public string Marca { get; set; }
        public DateTime? fecEmisionch { get; set; }
        public string paguesea { get; set; }
        [DefaultValue("N")]
        public string Impreso { get; set; }
        public string dlicoint_aperturas { get; set; }
        public string nro_operacion { get; set; }
        public int? FormadePag { get; set; }
        [DefaultValue("S")]
        public string CpbNormaIFRS { get; set; }
        [DefaultValue("S")]
        public string CpbNormaTrib { get; set; }
        [NotMapped]        
        public string KeyAgrupada
        {
            get
            {
                return CpbAno + "_" + CpbNum + "_" + MovNum.ToString();
            }
        }

    }
}