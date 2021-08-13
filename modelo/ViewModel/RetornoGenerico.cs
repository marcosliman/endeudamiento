using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace modelo.ViewModel
{
    public class RetornoGenerico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Seleccionado { get; set; }
        public bool Editable { get; set; }
        public string TipoRegistro { get; set; }
        public int IdTipo { get; set; }
        public int? ValorInt { get; set; }
        public decimal? ValorDecimal { get; set; }
        public float? ValorFloat { get; set; }
        public double? ValorDouble { get; set; }
        public string ValorString { get; set; }
    }

    public class RetornoGenericoMaterial
    {
        public int Id { get; set; }
        public string DesProd { get; set; }
        public int IdEstado { get; set; }
        public int CantidadRequerida { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class RetornoGenericoS
    {
        // IndexSolCrear
        public int IdSolicitudCreacion { get; set; }
        public string NombreProducto { get; set; }
        public string Grupo { get; set; }
        public string UMedida { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string EsParaCompra { get; set; }
        public string UsuarioSolicitante { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string SubGrupo { get; set; }
    }

}
