using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class FamiliaViewModel
    {
        public int IdFamilia { get; set; }
        public string NombreFamilia { get; set; }
        public bool? Activo { get; set; }
    }

}
