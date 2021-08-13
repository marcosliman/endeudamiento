using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("PermisoPerfil")]
    public class PermisoPerfil
    {
        [Key]
        public int IdPermisoPerfil { get; set; }
        public int IdMenu { get; set; }
        public int IdPerfil { get; set; }
        public bool Crear { get; set; }
        public bool Editar { get; set; }
        public bool Eliminar { get; set; }
        public bool Acceder { get; set; }
        public int IdEstadoVigencia { get; set; }        

    }
}