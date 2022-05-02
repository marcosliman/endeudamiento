
namespace modelo.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ErpContext : DbContext
    {
        public ErpContext()
            : base("name=ErpContext")
        {
            Database.SetInitializer<ErpContext>(null);
        }
              
        public System.Data.Entity.DbSet<Local.Menu> Menu { get; set; }
        public System.Data.Entity.DbSet<Local.Perfil> Perfil { get; set; }
        public System.Data.Entity.DbSet<Local.PermisoPerfil> PermisoPerfil { get; set; }           
        public System.Data.Entity.DbSet<Local.Usuario> Usuario { get; set; }
        public System.Data.Entity.DbSet<Local.UsuarioPerfil> UsuarioPerfil { get; set; }
        public System.Data.Entity.DbSet<Local.Empresa> Empresa { get; set; }
        public System.Data.Entity.DbSet<Local.TipoEstado> TipoEstado { get; set; }
        public System.Data.Entity.DbSet<Local.Estado> Estado { get; set; }
        public System.Data.Entity.DbSet<Local.Activo> Activo { get; set; }
        public System.Data.Entity.DbSet<Local.Familia> Familia { get; set; }
        public System.Data.Entity.DbSet<Local.Proveedor> Proveedor { get; set; }

        public System.Data.Entity.DbSet<Local.MarcaProducto> MarcaProducto { get; set; }

        public System.Data.Entity.DbSet<Local.ModeloProducto> ModeloProducto { get; set; }

        public System.Data.Entity.DbSet<Local.Licitacion> Licitacion { get; set; }
        public System.Data.Entity.DbSet<Local.TipoFinanciamiento> TipoFinanciamiento { get; set; }
        public System.Data.Entity.DbSet<Local.LicitacionActivo> LicitacionActivo { get; set; }
        public System.Data.Entity.DbSet<Local.LicitacionOferta> LicitacionOferta { get; set; }
        public System.Data.Entity.DbSet<Local.Banco> Banco { get; set; }
        public System.Data.Entity.DbSet<Local.LicitacionOfertaDocumento> LicitacionOfertaDocumento { get; set; }

        public System.Data.Entity.DbSet<Local.Contrato> Contrato { get; set; }
        public System.Data.Entity.DbSet<Local.TipoContrato> TipoContrato { get; set; }
        public System.Data.Entity.DbSet<Local.ContratoActivo> ContratoActivo { get; set; }
        public System.Data.Entity.DbSet<Local.TipoImpuesto> TipoImpuesto { get; set; }
        public System.Data.Entity.DbSet<Local.TipoDocumento> TipoDocumento { get; set; }
        public System.Data.Entity.DbSet<Local.CategoriaDocumento> CategoriaDocumento { get; set; }
        public System.Data.Entity.DbSet<Local.ContratoActivoDocumento> ContratoActivoDocumento { get; set; }
        public System.Data.Entity.DbSet<Local.TipoLog> TipoLog { get; set; }
        public System.Data.Entity.DbSet<Local.ContratoLog> ContratoLog { get; set; }

        public System.Data.Entity.DbSet<Local.Poliza> Poliza { get; set; }
        public System.Data.Entity.DbSet<Local.PolizaActivo> PolizaActivo { get; set; }
        public System.Data.Entity.DbSet<Local.PolizaDocumento> PolizaDocumento { get; set; }
        public System.Data.Entity.DbSet<Local.TipoSeguro> TipoSeguro { get; set; }
        public System.Data.Entity.DbSet<Local.TipoPerdida> TipoPerdida { get; set; }
        public System.Data.Entity.DbSet<Local.EmpresaAseguradora> EmpresaAseguradora { get; set; }
        public System.Data.Entity.DbSet<Local.TipoMoneda> TipoMoneda { get; set; }
        public System.Data.Entity.DbSet<Local.Siniestro> Siniestro { get; set; }
        public System.Data.Entity.DbSet<Local.SiniestroDocumento> SiniestroDocumento { get; set; }

        public System.Data.Entity.DbSet<Local.Contrato_Amortizacion> Contrato_Amortizacion { get; set; }
        public System.Data.Entity.DbSet<Local.Contrato_DetAmortizacion> Contrato_DetAmortizacion { get; set; }
        public System.Data.Entity.DbSet<Local.EmpresaRelacionada> EmpresaRelacionada { get; set; }

        /*mutuo*/
        public System.Data.Entity.DbSet<Local.Mutuo> Mutuo { get; set; }
        public System.Data.Entity.DbSet<Local.MutuoDocumento> MutuoDocumento { get; set; }
        public System.Data.Entity.DbSet<Local.MutuoPrestamo> MutuoPrestamo { get; set; }
        public System.Data.Entity.DbSet<Local.MutuoAbono> MutuoAbono { get; set; }

        public System.Data.Entity.DbSet<Local.Mes> Mes { get; set; }
        public System.Data.Entity.DbSet<Local.ComprobanteDetAmortizacion> ComprobanteDetAmortizacion { get; set; }
    }
}
