using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class LicitacionViewModel
    {
        public int IdLicitacion { get; set; }
        public int Correlativo { get; set; }
        public string Autogenerado { get; set; }
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public int IdTipoContrato { get; set; }
        public int IdTipoFinanciamiento { get; set; }
        public string NombreTipoFinanciamiento { get; set; }
        public double? Monto { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string ExisteLicitacion { get; set; }

        public string NumeroContrato { get; set; }
    }

}
