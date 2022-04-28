using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class EmpresaAseguradoraViewModel
    {
        public int IdEmpresaAseguradora { get; set; }
        public string NombreEmpresaAseguradora { get; set; }
        public bool? Activo { get; set; }
    }

}
