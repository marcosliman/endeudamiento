using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class ModeloProductoViewModel
    {
        public int IdModeloProducto { get; set; }
        public string DescModeloProducto { get; set; }
        public bool? Activo { get; set; }
    }

}
