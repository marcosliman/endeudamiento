using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class cwmovimViewModels
    {        
        public string CpbAno { get; set; }        
        public string CpbNum { get; set; }      
        public double MovNum { get; set; }
        public string AreaCod { get; set; }
        public string PctCod { get; set; }
        public DateTime CpbFec { get; set; }
        public string CpbMes { get; set; }
        public string CvCod { get; set; }
        public string VendCod { get; set; }
        public string UbicCod { get; set; }
        public string CajCod { get; set; }
        public string IfCod { get; set; }
        public double? MovIfCant { get; set; }
        public string DgaCod { get; set; }
        public double? MovDgCant { get; set; }
        public string CcCod { get; set; }
        public string TipDocCb { get; set; }
        public double? NumDocCb { get; set; }
        public string CodAux { get; set; }
        public string TtdCod { get; set; }
        public double? NumDoc { get; set; }
        public DateTime MovFe { get; set; }
        public DateTime MovFv { get; set; }
        public string MovTipDocRef { get; set; }
        public double? MovNumDocRef { get; set; }
        public double? MovDebe { get; set; }
        public double? MovHaber { get; set; }
        public string MovGlosa { get; set; }
        public string MonCod { get; set; }
        public double? MovEquiv { get; set; }
        public double? MovDebeMa { get; set; }
        public double? MovHaberMa { get; set; }
        public string MovNumCar { get; set; }
        public string MovTC { get; set; }
        public string MovNC { get; set; }
        public string MovIPr { get; set; }
        public string MovAEquiv { get; set; }
        public DateTime? FecPag { get; set; }
        public string CODCPAG { get; set; }
        public double? CbaNumMov { get; set; }
        public int? CbaAnoC { get; set; }
        public string GrabaDLib { get; set; }
        public string CpbOri { get; set; }
        public string CodBanco { get; set; }
        public string CodCtaCte { get; set; }
        public double? MtoTotal { get; set; }
        public int? Cuota { get; set; }
        public int? CuotaRef { get; set; }
        public string Marca { get; set; }
        public DateTime? fecEmisionch { get; set; }
        public string paguesea { get; set; }
        public string Impreso { get; set; }
        public string dlicoint_aperturas { get; set; }
        public string nro_operacion { get; set; }
        public int? FormadePag { get; set; }
        public string CpbNormaIFRS { get; set; }
        public string CpbNormaTrib { get; set; }     
        public DateTime FechaActual { get; set; }
        public int? dias { get; set; }
        public string DesDoc { get; set; }
        public string RutAux { get; set; }
        public string NomAux { get; set; }
        public string PCDESC { get; set; }
    }

}
