namespace DawtNetProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProtectFromEditing = c.Boolean(nullable: false),
                        CurrentVersion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Versions", t => t.CurrentVersion_Id)
                .Index(t => t.CurrentVersion_Id);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VersionNumber = c.Int(nullable: false),
                        Title = c.String(nullable: false),
                        ContentPath = c.String(nullable: false),
                        Article_Id = c.Int(),
                        Article_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.Article_Id)
                .ForeignKey("dbo.Articles", t => t.Article_Id1)
                .Index(t => t.Article_Id)
                .Index(t => t.Article_Id1);
            
            CreateTable(
                "dbo.Domains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        LastEdit = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DomainArticles",
                c => new
                    {
                        Domain_Id = c.Int(nullable: false),
                        Article_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Domain_Id, t.Article_Id })
                .ForeignKey("dbo.Domains", t => t.Domain_Id, cascadeDelete: true)
                .ForeignKey("dbo.Articles", t => t.Article_Id, cascadeDelete: true)
                .Index(t => t.Domain_Id)
                .Index(t => t.Article_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Versions", "Article_Id1", "dbo.Articles");
            DropForeignKey("dbo.DomainArticles", "Article_Id", "dbo.Articles");
            DropForeignKey("dbo.DomainArticles", "Domain_Id", "dbo.Domains");
            DropForeignKey("dbo.Articles", "CurrentVersion_Id", "dbo.Versions");
            DropForeignKey("dbo.Versions", "Article_Id", "dbo.Articles");
            DropIndex("dbo.DomainArticles", new[] { "Article_Id" });
            DropIndex("dbo.DomainArticles", new[] { "Domain_Id" });
            DropIndex("dbo.Versions", new[] { "Article_Id1" });
            DropIndex("dbo.Versions", new[] { "Article_Id" });
            DropIndex("dbo.Articles", new[] { "CurrentVersion_Id" });
            DropTable("dbo.DomainArticles");
            DropTable("dbo.Domains");
            DropTable("dbo.Versions");
            DropTable("dbo.Articles");
        }
    }
}
