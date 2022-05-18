using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Local
{
    [Table("Activo")]
    public class Activo
    {
        [Key]
        [Display(Name = "Código Activo")]
        public int IdActivo { get; set; }
        public int? IdEmpresa { get; set; }
        [ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
        public string NumeroInterno { get; set; }
        public string CodSoftland { get; set; }
        public int? IdFamilia { get; set; }
        [ForeignKey("IdFamilia")]
        public virtual Familia Familia { get; set; }
        public string Descripcion { get; set; }
        public string Capacidad { get; set; }
        public int? IdMarcaProducto { get; set; }
        public string Marca { get; set; }
        public int? IdModeloProducto { get; set; }
        public string Modelo { get; set; }
        public int? Anio { get; set; }
        public string Grupo { get; set; }
        public string SubGrupo { get; set; }
        public string IdCuenta { get; set; }
        public double? Valor { get; set; }
        public string IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        //[ForeignKey("IdProveedor")]
        //public virtual Proveedor Proveedor { get; set; }
        public string NumeroFactura { get; set; }
        public string Motor { get; set; }
        public string Chasis { get; set; }
        public string Serie { get; set; }
        public string Patente { get; set; }
        public string Glosa { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int IdEstado { get; set; }
        [ForeignKey("IdEstado")]
        public virtual Estado Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? IdUsuarioRegistro { get; set; }
        public bool SincronizadoSoftland { get; set; }
        public string NumeroContrato { get; set; }
        public string CampoGD { get; set; }
        public string MesAnio { get; set; }
        public string CodiCC_Mqs { get; set; }
        public string CodiCC_MqsSur { get; set; }
        public string DescCC_Mqs { get; set; }
        public string DescCC_MqsSur { get; set; }
        public double? ValorFactura { get; set; }
    }
}