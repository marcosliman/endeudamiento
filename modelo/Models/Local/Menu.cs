using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        public int IdMenu { get; set; }
        public Nullable<int> IdMenuPadre { get; set; }
        public string Nombre { get; set; }
        public string NombreMenu { get; set; }
        public string Icono { get; set; }
        public string UrlMenu { get; set; }
        public int NivelMenu { get; set; }
        public int OrdenMenu { get; set; }
        public bool TieneCrear { get; set; }
        public bool TieneEliminar { get; set; }
        public bool TieneEditar { get; set; }
        public bool TieneAcceder { get; set; }
        public Nullable<int> IdEstadoVigencia { get; set; }        
    }
}