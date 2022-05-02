using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;//valores por defecto
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("cwcpbte", Schema = "softland")]
    public class cwcpbte
    {
        [Key]
        [Column(Order = 0)]
        public string CpbAno { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CpbNum { get; set; }
        [DefaultValue("000")]
        public string AreaCod { get; set; }
        public DateTime? CpbFec { get; set; }
        public string CpbMes { get; set; }
        [DefaultValue("P")]
        public string CpbEst { get; set; }
        [DefaultValue("T")]
        public string CpbTip { get; set; }
        [DefaultValue("00000000")]
        public string CpbNui { get; set; }
        [DefaultValue(" ")]
        public string CpbGlo { get; set; }
        [DefaultValue("N")]
        public string CpbImp { get; set; }
        [DefaultValue("N")]
        public string CpbCon { get; set; }
        public string Sistema { get; set; }
        public string Proceso { get; set; }
        public string Usuario { get; set; }
        [DefaultValue("S")]
        public string CpbNormaIFRS { get; set; }
        [DefaultValue("S")]
        public string CpbNormaTrib { get; set; }
        [DefaultValue("0000")]
        public string CpbAnoRev { get; set; }
        [DefaultValue("00000000")]
        public string CpbNumRev { get; set; }
        public string SistemaMod { get; set; }
        public string ProcesoMod { get; set; }
        [DefaultValue("Getdate.Now")]
        public DateTime? FechaUlMod { get; set; }
        [DefaultValue("U")]
        public string TipoLog { get; set; }
    }
}