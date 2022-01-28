using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("awfichaac", Schema = "softland")]
    public class awfichaac
    {
        [Key]
        public string CodAct { get; set; }
        [DefaultValue("V")]
        public string Estado { get; set; }
        public string DescAct { get; set; }
        public string CodGru { get; set; }
        public string CodSGru { get; set; }
        public string CodIng { get; set; }
        public DateTime? FecIng { get; set; }
        public string CodUbB { get; set; }
        public string CodiCC { get; set; }
        public string CodArn { get; set; }
        public DateTime? FecUlGarVig { get; set; }
        [DefaultValue("N")]
        public string Revalorizable { get; set; }
        public DateTime? FecIniRev { get; set; }
        [DefaultValue("S")]
        public string Depreciable { get; set; }
        public DateTime FecIniDepr { get; set; }
        [DefaultValue("M")]
        public string UMedVUtil { get; set; }
        public string NUMedVUtil { get; set; }
        [DefaultValue("L")]
        public string TipDeprec { get; set; }
        [DefaultValue(0)]
        public int VUtTrib { get; set; }
        [DefaultValue(0)]
        public int VUtFin { get; set; }
        public string CtaCom { get; set; }
        public string CtaRev { get; set; }
        public string CtaDep { get; set; }
        public string CtaRead { get; set; }
        [DefaultValue("N")]
        public string CreAdq { get; set; }
        [DefaultValue(0)]
        public double VCredAdq { get; set; }
        [DefaultValue("C")]
        public string InfPartida { get; set; }
        [DefaultValue(0)]
        public double ValResid { get; set; }
        public DateTime? FecCom { get; set; }
        [DefaultValue(0)]
        public double ValCom { get; set; }
        public string NumFac { get; set; }
        public string CodAux { get; set; }
        public DateTime? FSitCon { get; set; }
        [DefaultValue(0)]
        public double VLibro { get; set; }
        [DefaultValue(0)]
        public double RevAcum { get; set; }
        [DefaultValue(0)]
        public double DepAcum { get; set; }
        [DefaultValue(0)]
        public int VUtRemTrib { get; set; }
        [DefaultValue(0)]
        public int VUtRemFin { get; set; }
        public DateTime? FecBaja { get; set; }
        public string CodBaja { get; set; }
        public string GloBaja { get; set; }
        public string Foto { get; set; }
        public DateTime? FSitConFin { get; set; }
        [DefaultValue(0)]
        public double VLibroFin { get; set; }
        [DefaultValue(0)]
        public double RevAcumFin { get; set; }
        [DefaultValue(0)]
        public double DepAcumFin { get; set; }
        public DateTime? FecIniRevFin { get; set; }
        public DateTime? FecIniDeprFin { get; set; }
        public string CodBarra { get; set; }
        [DefaultValue(0)]
        public int Readecuacion { get; set; }
        public string CodFichaReadec { get; set; }
        [DefaultValue(0)]
        public double PorcentajeActivo { get; set; }
        public string ctaRevAcumulada { get; set; }
        public string ctaDepRevEje { get; set; }
        public string ctaDepRevAcu { get; set; }
        [DefaultValue(0)]
        public double PorcDepAcumSI { get; set; }
        [DefaultValue(0)]
        public int NumMesDepSI { get; set; }
        [DefaultValue(0)]
        public int? NumOC { get; set; }
        [DefaultValue("N")]
        public string Leasing { get; set; }
        [DefaultValue("N")]
        public string EsConjunto { get; set; }
        [DefaultValue(0)]
        public int CantConjunto { get; set; }
        [DefaultValue("N")]
        public string EsArrendable { get; set; }
        [DefaultValue("S")]
        public string EsTributaria { get; set; }
        [DefaultValue("S")]
        public string EsIFRS { get; set; }

    }
}
