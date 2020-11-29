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
                        Id = c.Int(nullable: false),
                        ProtectFromEditing = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Versions", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        LastEdit = c.DateTime(nullable: false),
                        article_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.article_Id)
                .Index(t => t.article_Id);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        ContentPath = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("dbo.DomainArticles", "Article_Id", "dbo.Articles");
            DropForeignKey("dbo.DomainArticles", "Domain_Id", "dbo.Domains");
            DropForeignKey("dbo.Articles", "Id", "dbo.Versions");
            DropForeignKey("dbo.Comments", "article_Id", "dbo.Articles");
            DropIndex("dbo.DomainArticles", new[] { "Article_Id" });
            DropIndex("dbo.DomainArticles", new[] { "Domain_Id" });
            DropIndex("dbo.Comments", new[] { "article_Id" });
            DropIndex("dbo.Articles", new[] { "Id" });
            DropTable("dbo.DomainArticles");
            DropTable("dbo.Domains");
            DropTable("dbo.Versions");
            DropTable("dbo.Comments");
            DropTable("dbo.Articles");
        }
    }
}
