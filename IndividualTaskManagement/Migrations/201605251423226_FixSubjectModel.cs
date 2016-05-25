namespace IndividualTaskManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSubjectModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subjects", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subjects", "Name", c => c.String());
        }
    }
}
