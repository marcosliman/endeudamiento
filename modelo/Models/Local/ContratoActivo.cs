﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("ContratoActivo")]
    public class ContratoActivo
    {
        [Key]
        [Display(Name = "Código ContratoActivo")]
        public int IdContratoActivo { get; set; }
        public int IdContrato { get; set; }
        [ForeignKey("IdContrato")]
        public virtual Contrato Contrato { get; set; }
        public int IdActivo { get; set; }
        [ForeignKey("IdActivo")]
        public virtual Activo Activo { get; set; }
        public bool? EsEditable { get; set; }
    }
}