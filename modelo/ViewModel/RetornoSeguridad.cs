using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace modelo.ViewModel
{
    public class RetornoSeguridad
    {
        public string Controlador { get; set; }
        public string Vista { get; set; }
        public bool AccesoValido { get; set; }
        public int Estado { get; set; }
        public string Mensaje { get; set; }
    }

}
