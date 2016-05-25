namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSubgoal : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Subgoals", new[] { "Student_Id" });
            AlterColumn("dbo.Subgoals", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Subgoals", "Student_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Subgoals", "Student_Id");
            AddForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Subgoals", new[] { "Student_Id" });
            AlterColumn("dbo.Subgoals", "Student_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Subgoals", "Name", c => c.String());
            CreateIndex("dbo.Subgoals", "Student_Id");
            AddForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
