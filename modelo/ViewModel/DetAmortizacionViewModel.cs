﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class DetAmortizacionViewModel
    {       
        public int IdContratoDetAmortizacion { get; set; }
        public int IdContratoAmortizacion { get; set; }       
        public int? Anio { get; set; }
        public int Mes { get; set; }
        public int CortoPlazo { get; set; }
        public int LargoPlazo { get; set; }
        public DateTime FechaPago { get; set; }
        public int Periodo { get; set; }
        public int Cuota { get; set; }
        public double IvaDiferido { get; set; }
        public double Intereses { get; set; }
        public double Amortizacion { get; set; }
        public double Saldo_Insoluto { get; set; }
    }

}
