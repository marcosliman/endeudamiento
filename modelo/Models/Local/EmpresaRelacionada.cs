using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("EmpresaRelacionada")]
    public class EmpresaRelacionada
    {
        [Key]
        [Column(Order = 0)]
        public int IdEmpresa { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdEmpresaRelacionada { get; set; }   
  
    }
}