using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ReporteContratoViewModel
    {
        public int IdContrato { get; set; }
        public string Empresa { get; set; }
        public string Acreedor { get; set; }
        public string NumeroContrato { get; set; }
        public double? TasaMensual { get; set; }
        public double? TasaAnual { get; set; }
        public string Moneda { get; set; }
        public double? Total { get; set; }
        public double? Total1 { get; set; }
        public double? Total2 { get; set; }
        public double? Total3 { get; set; }
        public double? TotalGeneral { get; set; }
        public double? TotalCP { get; set; }
        public double? TotalLP { get; set; }
        public int? IdBanco { get; set; }
        public string NombreBanco { get; set; }
        public int? IdEmpresa { get; set; }
        public double? SaldoInsoluto { get; set; }
        public double? TotalFinal { get; set; }
        public int? IdTipoFinanciamiento { get; set; }
        public string NombreTipoFinanciamiento { get; set; }
        public double? DeudaCapital { get; set; }
        public double? DeudaCuota { get; set; }
        public int? anio { get; set; }
        public int? IdTipoContrato { get; set; }
        public string NombreTipoContrato { get; set; }
        public int? IdFamilia { get; set; }
        public double? MontoContrato { get; set; }
        public int? IdTipoMoneda { get; set; }
        public int? IdEstado { get; set; }
        public double? IVAMes { get; set; }
        public double? CapitalPagado { get; set; }
        public double? DeudaInteres { get; set; }
        public double? DeudaInteresMes { get; set; }
        public double? DeudaCuotaMes { get; set; }
        public double? DeudaCapitalMes { get; set; }
        public string CpbAnoTraspaso { get; set; }
        public string CpbNumTraspaso { get; set; }
        public string BaseSoftland { get; set; }
        public double? ObligacionCP { get; set; }
        public double? InteresesCP { get; set; }
        public double? IVACP { get; set; }
        public double? ObligacionLP { get; set; }
        public double? InteresesLP { get; set; }
        public double? IVALP { get; set; }
        public string RutBanco { get; set; }
        public string PCCODI { get; set; }
        public double? BalanceObligacionCP { get; set; }
        public double? BalanceInteresCP { get; set; }
        public double? BalanceIVACP { get; set; }
        public double? BalanceObligacionLP { get; set; }
        public double? BalanceInteresLP { get; set; }
        public double? BalanceIVALP { get; set; }
        public double? MontoOriginal { get; set; }
    }

}
