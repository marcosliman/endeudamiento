
namespace modelo.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Configuration;
    public class SoftLandContext : DbContext
    {
        public SoftLandContext(string dbName)
            : base(GetConnectionString(dbName))
        {
            Database.SetInitializer<SoftLandContext>(null);
        }
        public static string GetConnectionString(string dbName)
        {
            // Server=localhost;Database={0};Uid=username;Pwd=password
            var connString = ConfigurationManager.ConnectionStrings["SoftLandContext"].ConnectionString;

            return String.Format(connString, dbName);
        }        
        public System.Data.Entity.DbSet<Softland.iw_tprod> iw_tprod { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_tgrupo> iw_tgrupo { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_tsubgr> iw_tsubgr { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_costop> iw_costop { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_gmovi> iw_gmovi { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_nivst> iw_nivst { get; set; }
        public System.Data.Entity.DbSet<Softland.OW_vsnpTraeOCEncabezado> OW_vsnpTraeOCEncabezado { get; set; }
        public System.Data.Entity.DbSet<Softland.OW_vsnpTraeOCDetalle> OW_vsnpTraeOCDetalle { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_tbode> iw_tbode { get; set; }
        public System.Data.Entity.DbSet<Softland.cwtauxi> cwtauxi { get; set; }
        public System.Data.Entity.DbSet<Softland.cwtccos> cwtccos { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_tumed> iw_tumed { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_timprod> iw_timprod { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_gsaen> iw_gsaen { get; set; }
        public System.Data.Entity.DbSet<Softland.iw_cocod> iw_cocod { get; set; }
        public System.Data.Entity.DbSet<Softland.cwpctas> cwpctas { get; set; }
    }
}
