using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using modelo.Models.Local;
namespace modelo.ViewModel
{
    public class MarcaProductoViewModel
    {
        public int IdMarcaProducto { get; set; }
        public string DescMarcaProducto { get; set; }
        public bool? Activo { get; set; }
    }

}
