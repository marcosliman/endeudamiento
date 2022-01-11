using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
namespace modelo.Models.Local
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Display(Name = "Código Usuario")]
        public int IdUsuario { get; set; }
        public string RutUsuario { get; set; }   
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        [DataType(DataType.Password)]
        public string Clave { get; set; }        
        public bool Activo { get; set; }
        public bool Interno { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string RutRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string NombreRegistro { get; set; }
        public bool CambiarClave { get; set; }
    }
}