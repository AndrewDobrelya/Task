namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FifthMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subgoals", "EndDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subgoals", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
