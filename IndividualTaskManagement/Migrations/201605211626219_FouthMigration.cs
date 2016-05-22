namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FouthMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subgoals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        EndDate = c.DateTime(nullable: false),
                        Overdue = c.Boolean(nullable: false),
                        AtTetm = c.Boolean(nullable: false),
                        Goal_Id = c.Int(),
                        Student_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Goals", t => t.Goal_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Student_Id)
                .Index(t => t.Goal_Id)
                .Index(t => t.Student_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subgoals", "Goal_Id", "dbo.Goals");
            DropIndex("dbo.Subgoals", new[] { "Student_Id" });
            DropIndex("dbo.Subgoals", new[] { "Goal_Id" });
            DropTable("dbo.Subgoals");
        }
    }
}
