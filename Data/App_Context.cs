using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DawtNetProject.Data
{
    public class App_Context : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public App_Context() : base("DBConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<App_Context,
                                    DawtNetProject.Migrations.Configuration>("DBConnectionString"));
        }

        public System.Data.Entity.DbSet<DawtNetProject.Models.Article> Articles { get; set; }

        public System.Data.Entity.DbSet<DawtNetProject.Models.Domain> Domains { get; set; }

        public System.Data.Entity.DbSet<DawtNetProject.Models.Version> Versions { get; set; }

        public System.Data.Entity.DbSet<DawtNetProject.Models.Comment> Comments { get; set; }
    }
}
