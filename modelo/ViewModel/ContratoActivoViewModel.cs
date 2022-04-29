using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class ContratoActivoViewModel
    {
        public int IdContratoActivo { get; set; }
        public int IdContrato { get; set; }
        public int IdActivo { get; set; }
        public string NumeroInterno { get; set; }
        public string CodSoftland { get; set; }
        public string Familia { get; set; }
        public string Descripcion { get; set; }
        public List<ContratoDocumentoViewModel> Archivos { get; set; }
        public int IdEstado { get; set; }
        public int IdTipoContrato { get; set; }
    }

}
