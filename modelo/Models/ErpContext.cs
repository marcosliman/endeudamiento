
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
        public System.Data.Entity.DbSet<Local.Estado> Estado { get; set; }
    }
}
