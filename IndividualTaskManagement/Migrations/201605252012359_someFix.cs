namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects");
            DropIndex("dbo.Subgoals", new[] { "Student_Id" });
            DropIndex("dbo.Goals", new[] { "Subject_Id" });
            AlterColumn("dbo.Subgoals", "Name", c => c.String());
            AlterColumn("dbo.Subgoals", "Student_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Goals", "Name", c => c.String());
            AlterColumn("dbo.Goals", "Subject_Id", c => c.Int());
            AlterColumn("dbo.Subjects", "Name", c => c.String());
            CreateIndex("dbo.Subgoals", "Student_Id");
            CreateIndex("dbo.Goals", "Subject_Id");
            AddForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects");
            DropForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Goals", new[] { "Subject_Id" });
            DropIndex("dbo.Subgoals", new[] { "Student_Id" });
            AlterColumn("dbo.Subjects", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Goals", "Subject_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Goals", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Subgoals", "Student_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Subgoals", "Name", c => c.String(nullable: false));
            CreateIndex("dbo.Goals", "Subject_Id");
            CreateIndex("dbo.Subgoals", "Student_Id");
            AddForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Subgoals", "Student_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
