namespace Narratives.Migrations
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
                        Name = c.String(unicode: false),
                        Narrative_NarrativeId = c.Int(),
                    })
                .PrimaryKey(t => t.NarrativeActionId)
                .ForeignKey("dbo.Narratives", t => t.Narrative_NarrativeId)
                .Index(t => t.Narrative_NarrativeId);
            
            CreateTable(
                "dbo.NarrativeArguments",
                c => new
                    {
                        NarrativeArgumentId = c.Int(nullable: false, identity: true),
                        Type_NarrativeObjectTypeId = c.Int(),
                        NarrativeAction_NarrativeActionId = c.Int(),
                        PredicateType_PredicateTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.NarrativeArgumentId)
                .ForeignKey("dbo.NarrativeObjectTypes", t => t.Type_NarrativeObjectTypeId)
                .ForeignKey("dbo.NarrativeActions", t => t.NarrativeAction_NarrativeActionId)
                .ForeignKey("dbo.PredicateTypes", t => t.PredicateType_PredicateTypeId)
                .Index(t => t.Type_NarrativeObjectTypeId)
                .Index(t => t.NarrativeAction_NarrativeActionId)
                .Index(t => t.PredicateType_PredicateTypeId);
            
            CreateTable(
                "dbo.NarrativeObjectTypes",
                c => new
                    {
                        NarrativeObjectTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Narrative_NarrativeId = c.Int(),
                    })
                .PrimaryKey(t => t.NarrativeObjectTypeId)
                .ForeignKey("dbo.Narratives", t => t.Narrative_NarrativeId)
                .Index(t => t.Narrative_NarrativeId);
            
            CreateTable(
                "dbo.NarrativeEvents",
                c => new
                    {
                        NarrativeEventId = c.Int(nullable: false, identity: true),
                        NarrativeAction_NarrativeActionId = c.Int(),
                        Narrative_NarrativeId = c.Int(),
                    })
                .PrimaryKey(t => t.NarrativeEventId)
                .ForeignKey("dbo.NarrativeActions", t => t.NarrativeAction_NarrativeActionId)
                .ForeignKey("dbo.Narratives", t => t.Narrative_NarrativeId)
                .Index(t => t.NarrativeAction_NarrativeActionId)
                .Index(t => t.Narrative_NarrativeId);
            
            CreateTable(
                "dbo.NarrativeObjects",
                c => new
                    {
                        NarrativeObjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Type_NarrativeObjectTypeId = c.Int(),
                        NarrativeEvent_NarrativeEventId = c.Int(),
                        NarrativePredicate_NarrativePredicateId = c.Int(),
                        Narrative_NarrativeId = c.Int(),
                    })
                .PrimaryKey(t => t.NarrativeObjectId)
                .ForeignKey("dbo.NarrativeObjectTypes", t => t.Type_NarrativeObjectTypeId)
                .ForeignKey("dbo.NarrativeEvents", t => t.NarrativeEvent_NarrativeEventId)
                .ForeignKey("dbo.NarrativePredicates", t => t.NarrativePredicate_NarrativePredicateId)
                .ForeignKey("dbo.Narratives", t => t.Narrative_NarrativeId)
                .Index(t => t.Type_NarrativeObjectTypeId)
                .Index(t => t.NarrativeEvent_NarrativeEventId)
                .Index(t => t.NarrativePredicate_NarrativePredicateId)
                .Index(t => t.Narrative_NarrativeId);
            
            CreateTable(
                "dbo.NarrativePredicates",
                c => new
                    {
                        NarrativePredicateId = c.Int(nullable: false, identity: true),
                        PredicateType_PredicateTypeId = c.Int(),
                        Narrative_NarrativeId = c.Int(),
                    })
                .PrimaryKey(t => t.NarrativePredicateId)
                .ForeignKey("dbo.PredicateTypes", t => t.PredicateType_PredicateTypeId)
                .ForeignKey("dbo.Narratives", t => t.Narrative_NarrativeId)
                .Index(t => t.PredicateType_PredicateTypeId)
                .Index(t => t.Narrative_NarrativeId);
            
            CreateTable(
                "dbo.PredicateTypes",
                c => new
                    {
                        PredicateTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Narrative_NarrativeId = c.Int(),
                    })
                .PrimaryKey(t => t.PredicateTypeId)
                .ForeignKey("dbo.Narratives", t => t.Narrative_NarrativeId)
                .Index(t => t.Narrative_NarrativeId);
            
            CreateTable(
                "dbo.Narratives",
                c => new
                    {
                        NarrativeId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.NarrativeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PredicateTypes", "Narrative_NarrativeId", "dbo.Narratives");
            DropForeignKey("dbo.NarrativePredicates", "Narrative_NarrativeId", "dbo.Narratives");
            DropForeignKey("dbo.NarrativeObjectTypes", "Narrative_NarrativeId", "dbo.Narratives");
            DropForeignKey("dbo.NarrativeObjects", "Narrative_NarrativeId", "dbo.Narratives");
            DropForeignKey("dbo.NarrativeEvents", "Narrative_NarrativeId", "dbo.Narratives");
            DropForeignKey("dbo.NarrativeActions", "Narrative_NarrativeId", "dbo.Narratives");
            DropForeignKey("dbo.NarrativePredicates", "PredicateType_PredicateTypeId", "dbo.PredicateTypes");
            DropForeignKey("dbo.NarrativeArguments", "PredicateType_PredicateTypeId", "dbo.PredicateTypes");
            DropForeignKey("dbo.NarrativeObjects", "NarrativePredicate_NarrativePredicateId", "dbo.NarrativePredicates");
            DropForeignKey("dbo.NarrativeObjects", "NarrativeEvent_NarrativeEventId", "dbo.NarrativeEvents");
            DropForeignKey("dbo.NarrativeObjects", "Type_NarrativeObjectTypeId", "dbo.NarrativeObjectTypes");
            DropForeignKey("dbo.NarrativeEvents", "NarrativeAction_NarrativeActionId", "dbo.NarrativeActions");
            DropForeignKey("dbo.NarrativeArguments", "NarrativeAction_NarrativeActionId", "dbo.NarrativeActions");
            DropForeignKey("dbo.NarrativeArguments", "Type_NarrativeObjectTypeId", "dbo.NarrativeObjectTypes");
            DropIndex("dbo.PredicateTypes", new[] { "Narrative_NarrativeId" });
            DropIndex("dbo.NarrativePredicates", new[] { "Narrative_NarrativeId" });
            DropIndex("dbo.NarrativePredicates", new[] { "PredicateType_PredicateTypeId" });
            DropIndex("dbo.NarrativeObjects", new[] { "Narrative_NarrativeId" });
            DropIndex("dbo.NarrativeObjects", new[] { "NarrativePredicate_NarrativePredicateId" });
            DropIndex("dbo.NarrativeObjects", new[] { "NarrativeEvent_NarrativeEventId" });
            DropIndex("dbo.NarrativeObjects", new[] { "Type_NarrativeObjectTypeId" });
            DropIndex("dbo.NarrativeEvents", new[] { "Narrative_NarrativeId" });
            DropIndex("dbo.NarrativeEvents", new[] { "NarrativeAction_NarrativeActionId" });
            DropIndex("dbo.NarrativeObjectTypes", new[] { "Narrative_NarrativeId" });
            DropIndex("dbo.NarrativeArguments", new[] { "PredicateType_PredicateTypeId" });
            DropIndex("dbo.NarrativeArguments", new[] { "NarrativeAction_NarrativeActionId" });
            DropIndex("dbo.NarrativeArguments", new[] { "Type_NarrativeObjectTypeId" });
            DropIndex("dbo.NarrativeActions", new[] { "Narrative_NarrativeId" });
            DropTable("dbo.Narratives");
            DropTable("dbo.PredicateTypes");
            DropTable("dbo.NarrativePredicates");
            DropTable("dbo.NarrativeObjects");
            DropTable("dbo.NarrativeEvents");
            DropTable("dbo.NarrativeObjectTypes");
            DropTable("dbo.NarrativeArguments");
            DropTable("dbo.NarrativeActions");
        }
    }
}
