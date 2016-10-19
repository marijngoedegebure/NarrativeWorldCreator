namespace NarrativeWorldCreator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NarrativeActions",
                c => new
                    {
                        NarrativeActionId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeActionId);
            
            CreateTable(
                "dbo.NarrativeArguments",
                c => new
                    {
                        NarrativeArgumentId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeArgumentId);
            
            CreateTable(
                "dbo.NarrativeEvents",
                c => new
                    {
                        NarrativeEventId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeEventId);
            
            CreateTable(
                "dbo.NarrativeObjects",
                c => new
                    {
                        NarrativeObjectId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeObjectId);
            
            CreateTable(
                "dbo.NarrativeObjectTypes",
                c => new
                    {
                        NarrativeObjectTypeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeObjectTypeId);
            
            CreateTable(
                "dbo.NarrativePredicates",
                c => new
                    {
                        NarrativePredicateId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativePredicateId);
            
            CreateTable(
                "dbo.Narratives",
                c => new
                    {
                        NarrativeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeId);
            
            CreateTable(
                "dbo.PredicateTypes",
                c => new
                    {
                        PredicateTypeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.PredicateTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PredicateTypes");
            DropTable("dbo.Narratives");
            DropTable("dbo.NarrativePredicates");
            DropTable("dbo.NarrativeObjectTypes");
            DropTable("dbo.NarrativeObjects");
            DropTable("dbo.NarrativeEvents");
            DropTable("dbo.NarrativeArguments");
            DropTable("dbo.NarrativeActions");
        }
    }
}
