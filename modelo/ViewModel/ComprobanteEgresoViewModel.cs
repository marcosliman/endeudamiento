using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Softland;
namespace modelo.ViewModel
{
    public class ComprobanteEgresoViewModel
    {
        public int IdComprobanteEgreso { get; set; }
        public int IdEgreso { get; set; }
        public string NroComprobanteEgreso { get; set; }
        public cwcpbte cwcpbte { get; set; }
        public DateTime? CpbFec { get; set; }
        public string Proceso { get; set; }
        public string CpbAno { get; set; }
        public soempre soempre { get; set; }
        public string AreaNegocio { get; set; }
        public wisusuarios wisusuarios { get; set; }
        public string AliasEmpresa { get; set; }
        public string CpbNum { get; set; }
        public double? MontoTotal { get; set; }
        public string CpbTip { get; set; }
        public List<cwmovimViewModels> detalleComprobante { get; set; }
    }

}
