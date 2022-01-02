using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class TipoSeguroViewModel
    {
        public int IdTipoSeguro { get; set; }
        public string NombreTipoSeguro { get; set; }
        public bool? Activo { get; set; }
    }

}
