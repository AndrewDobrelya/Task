namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeventhMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgoals", "AtTerm", c => c.Boolean(nullable: false));
            DropColumn("dbo.Subgoals", "AtTetm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subgoals", "AtTetm", c => c.Boolean(nullable: false));
            DropColumn("dbo.Subgoals", "AtTerm");
        }
    }
}
