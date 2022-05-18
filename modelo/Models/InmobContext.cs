
namespace modelo.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    public class InmobContext : DbContext
    {
        public InmobContext()
            : base("name=InmobContext")
        {
            Database.SetInitializer<InmobContext>(null);
        }              
        public System.Data.Entity.DbSet<Inmobiliaria.SII_ValoresUF> SII_ValoresUF { get; set; }       
    }
}
