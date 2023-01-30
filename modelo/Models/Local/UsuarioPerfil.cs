using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("UsuarioPerfil")]
    public class UsuarioPerfil
    {
        [Key]
        public int IdUsuarioPerfil { get; set; }
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
        public int IdPerfil { get; set; }
        [ForeignKey("IdPerfil")]
        public virtual Perfil Perfil { get; set; }
        public bool Activo { get; set; }
    }
}