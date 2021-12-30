using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class TipoDoctoViewModel
    {
        public int IdTipoDocumento { get; set; }
        public int? IdCategoriaDocumento { get; set; }
        public string NombreTipoDocumento { get; set; }
        public string NombreCategoriaDocumento { get; set; }
        public bool? Activo { get; set; }
    }

}
