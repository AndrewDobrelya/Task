namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SixMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        text = c.String(),
                        previosComment_id = c.Int(),
                        subgoal_Id = c.Int(),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Comments", t => t.previosComment_id)
                .ForeignKey("dbo.Subgoals", t => t.subgoal_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.previosComment_id)
                .Index(t => t.subgoal_Id)
                .Index(t => t.user_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "subgoal_Id", "dbo.Subgoals");
            DropForeignKey("dbo.Comments", "previosComment_id", "dbo.Comments");
            DropIndex("dbo.Comments", new[] { "user_Id" });
            DropIndex("dbo.Comments", new[] { "subgoal_Id" });
            DropIndex("dbo.Comments", new[] { "previosComment_id" });
            DropTable("dbo.Comments");
        }
    }
}
