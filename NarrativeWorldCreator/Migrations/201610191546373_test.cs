namespace NarrativeWorldCreator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NarrativeObjectTypes", "name", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NarrativeObjectTypes", "name");
        }
    }
}
