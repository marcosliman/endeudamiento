using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace modelo.ViewModel
{
    public class GraficosViewModel
    {
        public double? TotalProducto { get; set; }
        public List<RetornoGrafico> Bodegas { get; set; }
        public List<RetornoGrafico> SubGrupos { get; set; }
        public List<RetornoGrafico> DataSubGrupo { get; set; }
    }

}
