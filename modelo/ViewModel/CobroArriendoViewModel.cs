using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class CobroArriendoViewModel
    {
        public string NroEquipo { get; set; }
        public string Sucursal { get; set; }
        public string DiasArriendo { get; set; }
        public string DiasDisponible { get; set; }
        public string DiasTaller { get; set; }
        public string Depreciacion { get; set; }
        public string DesArn { get; set; }
        public string DescripcionFamilia { get; set; }
        public string GrupoTarifario { get; set; }
        public string DescCC_MqsSur { get; set; }
        public string DescCC_Mqs { get; set; }
        public int? Anio { get; set; }
        public string EstadoActivo { get; set; }
        public double? TarifaUF { get; set; }
        public double? TarifaCLP { get; set; }
        public string Propiedad { get; set; }
        public double? TarifaCLP_Aplicar { get; set; }
        public double? Cobro_DiasArriendo { get; set; }
        public double? Cobro_DiasDisponible { get; set; }
        public double? Cobro_DiasTaller { get; set; }
        public string CodArn { get; set; }
        public double? MontoVenta { get; set; }
        public double? PorcentajeVenta { get; set; }
    }

}
