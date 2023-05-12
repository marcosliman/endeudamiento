using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("GrupoTarifario")]
    public class GrupoTarifario
    {
        [Key]
        [Display(Name = "Código Grupo Tarifario")]
        public int IdGrupoTarifario { get; set; }
        public string GrupoFamilia { get; set; }
        public string DescripcionGrupoTarifario { get; set; } 
        public double UF { get; set; }
        public bool Activo { get; set; }        
    }
}