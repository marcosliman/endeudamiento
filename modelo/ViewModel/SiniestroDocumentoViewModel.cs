using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class SiniestroDocumentoViewModel
    {
        public int IdSiniestroDocumento { get; set; }
        public int IdSiniestro { get; set; }

        public int IdTipoDocumento { get; set; }
        public string NombreTipoDocumento { get; set; }
        public string UrlDocumento { get; set; }
        public string NombreOriginal { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

}
