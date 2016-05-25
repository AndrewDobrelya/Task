namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixGoalModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects");
            DropIndex("dbo.Goals", new[] { "Subject_Id" });
            AlterColumn("dbo.Goals", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Goals", "Subject_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Goals", "Subject_Id");
            AddForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects");
            DropIndex("dbo.Goals", new[] { "Subject_Id" });
            AlterColumn("dbo.Goals", "Subject_Id", c => c.Int());
            AlterColumn("dbo.Goals", "Name", c => c.String());
            CreateIndex("dbo.Goals", "Subject_Id");
            AddForeignKey("dbo.Goals", "Subject_Id", "dbo.Subjects", "Id");
        }
    }
}
