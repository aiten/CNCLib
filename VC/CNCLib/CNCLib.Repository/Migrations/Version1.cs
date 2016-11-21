namespace CNCLib.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Version1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MachineCommand", "JoystickMessage", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MachineCommand", "JoystickMessage");
        }
    }
}
