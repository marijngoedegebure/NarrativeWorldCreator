namespace Narratives.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Objectaddition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NarrativeObjects", "Placed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NarrativeObjects", "Placed");
        }
    }
}
