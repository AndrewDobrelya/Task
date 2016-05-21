namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThirdMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Goals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        Completeness = c.Int(nullable: false),
                        Author_Id = c.String(maxLength: 128),
                        Subject_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .ForeignKey("dbo.Subjects", t => t.Subject_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Subject_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects");
            DropForeignKey("dbo.Goals", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Goals", new[] { "Subject_Id" });
            DropIndex("dbo.Goals", new[] { "Author_Id" });
            DropTable("dbo.Goals");
        }
    }
}
