
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
        public System.Data.Entity.DbSet<Local.Proveedor> Proveedor { get; set; }

        public System.Data.Entity.DbSet<Local.Licitacion> Licitacion { get; set; }
        public System.Data.Entity.DbSet<Local.TipoFinanciamiento> TipoFinanciamiento { get; set; }
        public System.Data.Entity.DbSet<Local.LicitacionActivo> LicitacionActivo { get; set; }
        public System.Data.Entity.DbSet<Local.LicitacionOferta> LicitacionOferta { get; set; }
        public System.Data.Entity.DbSet<Local.Banco> Banco { get; set; }
        public System.Data.Entity.DbSet<Local.LicitacionOfertaDocumento> LicitacionOfertaDocumento { get; set; }

        public System.Data.Entity.DbSet<Local.Contrato> Contrato { get; set; }
        public System.Data.Entity.DbSet<Local.ContratoActivo> ContratoActivo { get; set; }
        public System.Data.Entity.DbSet<Local.TipoImpuesto> TipoImpuesto { get; set; }
    }
}
