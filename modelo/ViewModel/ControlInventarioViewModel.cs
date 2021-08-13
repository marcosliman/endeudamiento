using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ControlInventarioViewModel
    {
        public int IdControlInventario { get; set; }
        public string CodBode { get; set; }
        public string DesBode { get; set; }
        public double CantStock { get; set; }
        public double PrecioTotal { get; set; }
        public double CantContabilizada { get; set; }
        public double PrecioContabilizada { get; set; }
        public double CantNoContabilizada { get; set; }
        public double PrecioNoContabilizada { get; set; }
        public double CantConDiferencia { get; set; }
        public double PrecioConDiferencia { get; set; }
    }

}
