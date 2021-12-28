using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ContratoLogViewModel
    {
        public int IdContratoLog { get; set; }
        public int IdContrato { get; set; }
        public int IdTipoLog { get; set; }
        public string NombreTipoLog { get; set; }
        public string NombreLog { get; set; }
        public int IdUsuarioResgistro { get; set; }
        public string NombreUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string FechaRegistroStr { get; set; }
    }

}
