using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class RotacionProductoViewModel
    {
        public string DT_RowId { get; set; }
        public string DesBode { get; set; }
        public string CodBode { get; set; }
        public string CodProd { get; set; }

        public string DesProd { get; set; }
        public string DesGrupo { get; set; }
        public string CodSubGr { get; set; }
        public string DesSubGr { get; set; }
        public string Empresa { get; set; }
        public string GrupoEmpresa { get; set; }
        public string CodUMed { get; set; }
        public double Stock { get; set; }
        public double costo { get; set; }
        public double Total { get; set; }
        public int TotalProd { get; set; }
        public double TotalPrecio { get; set; }
        public List<EmpresaRotacionViewModel> empresas;
        public DateTime? UltimaEntrada { get; set; }
        public DateTime? UltimaSalida { get; set; }
        public string UltimoProveedor { get; set; }
        public int? UltimaOC { get; set; }
        public int? UltimoNumReq { get; set; }
        public string UltimoSolicitante { get; set; }
    }

}
