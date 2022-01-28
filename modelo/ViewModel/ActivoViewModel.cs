using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ActivoViewModel
    {
        public int IdActivo { get; set; }
        public int? IdLicitacionActivo { get; set; }
        public int? IdContratoActivo { get; set; }
        public int? IdPolizaActivo { get; set; }
        public int? IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string NumeroInterno { get; set; }
        public string CodSoftland { get; set; }
        public int? IdFamilia { get; set; }
        public string Familia { get; set; }
        public string Descripcion { get; set; }
        public string Capacidad { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int? Anio { get; set; }
        public string Grupo { get; set; }
        public string SubGrupo { get; set; }
        public string IdCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double? Valor { get; set; }
        public string IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string NumeroFactura { get; set; }
        public string Motor { get; set; }
        public string Chasis { get; set; }
        public string Serie { get; set; }
        public string Patente { get; set; }
        public string Glosa { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string FechaBajaStr { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string FechaRegistroStr { get; set; }
        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }
        public string NumeroLeasing { get; set; }
        public string TituloBoton { get; set; }
        public bool Editable { get; set; }
    }

}
