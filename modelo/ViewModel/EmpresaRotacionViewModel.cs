using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace modelo.ViewModel
{
    public class EmpresaRotacionViewModel
    {       
        public string CodGrupo { get; set; }
        public string CodSubGr { get; set; }
        public string Nombre { get; set; }      
        public int TotalProductos { get; set; }
        public double TotalPrecio { get; set; }
        public List<EmpresaRotacionViewModel> grupos { get; set; }

    }

}
