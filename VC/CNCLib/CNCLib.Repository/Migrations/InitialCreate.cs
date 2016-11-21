namespace CNCLib.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        Group = c.String(nullable: false, maxLength: 256),
                        Name = c.String(nullable: false, maxLength: 256),
                        Type = c.String(nullable: false, maxLength: 256),
                        Value = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => new { t.Group, t.Name });
            
            CreateTable(
                "dbo.ItemProperty",
                c => new
                    {
                        ItemID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Value = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => new { t.ItemID, t.Name })
                .ForeignKey("dbo.Item", t => t.ItemID, cascadeDelete: true)
                .Index(t => t.ItemID);
            
            CreateTable(
                "dbo.Item",
                c => new
                    {
                        ItemID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        ClassName = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.ItemID)
                .Index(t => t.Name, unique: true, name: "IDX_UniqueName");
            
            CreateTable(
                "dbo.MachineCommand",
                c => new
                    {
                        MachineCommandID = c.Int(nullable: false, identity: true),
                        CommandName = c.String(nullable: false, maxLength: 64),
                        CommandString = c.String(nullable: false, maxLength: 64),
                        MachineID = c.Int(nullable: false),
                        PosX = c.Int(),
                        PosY = c.Int(),
                    })
                .PrimaryKey(t => t.MachineCommandID)
                .ForeignKey("dbo.Machine", t => t.MachineID, cascadeDelete: true)
                .Index(t => t.MachineID);
            
            CreateTable(
                "dbo.Machine",
                c => new
                    {
                        MachineID = c.Int(nullable: false, identity: true),
                        ComPort = c.String(nullable: false, maxLength: 32),
                        Axis = c.Int(nullable: false),
                        BaudRate = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        SizeX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SizeY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SizeZ = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SizeA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SizeB = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SizeC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BufferSize = c.Int(nullable: false),
                        CommandToUpper = c.Boolean(nullable: false),
                        ProbeSizeX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProbeSizeY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProbeSizeZ = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProbeDistUp = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProbeDist = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProbeFeed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SDSupport = c.Boolean(nullable: false),
                        Spindle = c.Boolean(nullable: false),
                        Coolant = c.Boolean(nullable: false),
                        Laser = c.Boolean(nullable: false),
                        Rotate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MachineID);
            
            CreateTable(
                "dbo.MachineInitCommand",
                c => new
                    {
                        MachineInitCommandID = c.Int(nullable: false, identity: true),
                        SeqNo = c.Int(nullable: false),
                        CommandString = c.String(nullable: false, maxLength: 64),
                        MachineID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MachineInitCommandID)
                .ForeignKey("dbo.Machine", t => t.MachineID, cascadeDelete: true)
                .Index(t => t.MachineID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MachineInitCommand", "MachineID", "dbo.Machine");
            DropForeignKey("dbo.MachineCommand", "MachineID", "dbo.Machine");
            DropForeignKey("dbo.ItemProperty", "ItemID", "dbo.Item");
            DropIndex("dbo.MachineInitCommand", new[] { "MachineID" });
            DropIndex("dbo.MachineCommand", new[] { "MachineID" });
            DropIndex("dbo.Item", "IDX_UniqueName");
            DropIndex("dbo.ItemProperty", new[] { "ItemID" });
            DropTable("dbo.MachineInitCommand");
            DropTable("dbo.Machine");
            DropTable("dbo.MachineCommand");
            DropTable("dbo.Item");
            DropTable("dbo.ItemProperty");
            DropTable("dbo.Configuration");
        }
    }
}
