using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Perfil")]
    public class Perfil
    {
        [Key]
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public bool Activo { get; set; }
        public bool Editable { get; set; }

    }
}