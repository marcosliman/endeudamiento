using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bodega.Helper
{
    public class RequestSolicitud
    {
        public string IdSolicitudProducto { get; set; }
        public string CodProd { get; set; }
        public string DesProd { get; set; }
        public string CantidadRequerida { get; set; }
        public string CantidadRetiro { get; set; }
        public string Cantidad { get; set; }
        public string CostoUn { get; set; }
        public string UMedida { get; set; }
        //aux
        public string Status { get; set; }
        public string Stock { get; set; }
        public string total { get; set; }
        public int IdPP { get; set; }

    }
}