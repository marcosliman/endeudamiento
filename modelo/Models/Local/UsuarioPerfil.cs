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
        public int IdPerfil { get; set; }        
        public bool Activo { get; set; }        
    }
}