using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "User", columns: table => new
            {
                UserID       = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                UserName     = table.Column<string>(maxLength: 128, nullable: false),
                UserPassword = table.Column<string>(maxLength: 255, nullable: true)
            }, constraints: table => { table.PrimaryKey("PK_User", x => x.UserID); });

            migrationBuilder.CreateTable(name: "Configuration", columns: table => new
            {
                Group  = table.Column<string>(maxLength: 256, nullable: false),
                Name   = table.Column<string>(maxLength: 256, nullable: false),
                Type   = table.Column<string>(maxLength: 256, nullable: false),
                UserID = table.Column<int>(nullable: true),
                Value  = table.Column<string>(maxLength: 4000, nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Configuration", x => new { x.Group, x.Name });
                table.ForeignKey(name: "FK_Configuration_User_UserID", column: x => x.UserID, principalTable: "User", principalColumn: "UserID", onDelete: ReferentialAction.Restrict);
            });

            migrationBuilder.CreateTable(name: "Item", columns: table => new
            {
                ItemID    = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                ClassName = table.Column<string>(maxLength: 255, nullable: false),
                Name      = table.Column<string>(maxLength: 64,  nullable: false),
                UserID    = table.Column<int>(nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Item", x => x.ItemID);
                table.ForeignKey(name: "FK_Item_User_UserID", column: x => x.UserID, principalTable: "User", principalColumn: "UserID", onDelete: ReferentialAction.Restrict);
            });

            migrationBuilder.CreateTable(name: "Machine", columns: table => new
            {
                MachineID      = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                Axis           = table.Column<int>(nullable: false),
                BaudRate       = table.Column<int>(nullable: false),
                BufferSize     = table.Column<int>(nullable: false),
                ComPort        = table.Column<string>(maxLength: 32, nullable: false),
                CommandSyntax  = table.Column<int>(nullable: false),
                CommandToUpper = table.Column<bool>(nullable: false),
                Coolant        = table.Column<bool>(nullable: false),
                Laser          = table.Column<bool>(nullable: false),
                Name           = table.Column<string>(maxLength: 64, nullable: false),
                NeedDtr        = table.Column<bool>(nullable: false),
                ProbeDist      = table.Column<decimal>(nullable: false),
                ProbeDistUp    = table.Column<decimal>(nullable: false),
                ProbeFeed      = table.Column<decimal>(nullable: false),
                ProbeSizeX     = table.Column<decimal>(nullable: false),
                ProbeSizeY     = table.Column<decimal>(nullable: false),
                ProbeSizeZ     = table.Column<decimal>(nullable: false),
                Rotate         = table.Column<bool>(nullable: false),
                SDSupport      = table.Column<bool>(nullable: false),
                SizeA          = table.Column<decimal>(nullable: false),
                SizeB          = table.Column<decimal>(nullable: false),
                SizeC          = table.Column<decimal>(nullable: false),
                SizeX          = table.Column<decimal>(nullable: false),
                SizeY          = table.Column<decimal>(nullable: false),
                SizeZ          = table.Column<decimal>(nullable: false),
                Spindle        = table.Column<bool>(nullable: false),
                UserID         = table.Column<int>(nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_Machine", x => x.MachineID);
                table.ForeignKey(name: "FK_Machine_User_UserID", column: x => x.UserID, principalTable: "User", principalColumn: "UserID", onDelete: ReferentialAction.Restrict);
            });

            migrationBuilder.CreateTable(name: "ItemProperty", columns: table => new
            {
                ItemID = table.Column<int>(nullable: false),
                Name   = table.Column<string>(maxLength: 255, nullable: false),
                Value  = table.Column<string>(nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_ItemProperty", x => new { x.ItemID, x.Name });
                table.ForeignKey(name: "FK_ItemProperty_Item_ItemID", column: x => x.ItemID, principalTable: "Item", principalColumn: "ItemID", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable(name: "MachineCommand", columns: table => new
            {
                MachineCommandID = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                CommandName      = table.Column<string>(maxLength: 64, nullable: false),
                CommandString    = table.Column<string>(maxLength: 64, nullable: false),
                JoystickMessage  = table.Column<string>(maxLength: 64, nullable: true),
                MachineID        = table.Column<int>(nullable: false),
                PosX             = table.Column<int>(nullable: true),
                PosY             = table.Column<int>(nullable: true)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_MachineCommand", x => x.MachineCommandID);
                table.ForeignKey(name: "FK_MachineCommand_Machine_MachineID", column: x => x.MachineID, principalTable: "Machine", principalColumn: "MachineID", onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable(name: "MachineInitCommand", columns: table => new
            {
                MachineInitCommandID = table.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                CommandString        = table.Column<string>(maxLength: 64, nullable: false),
                MachineID            = table.Column<int>(nullable: false),
                SeqNo                = table.Column<int>(nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_MachineInitCommand", x => x.MachineInitCommandID);
                table.ForeignKey(name: "FK_MachineInitCommand_Machine_MachineID", column: x => x.MachineID, principalTable: "Machine", principalColumn: "MachineID",
                                 onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateIndex(name: "IX_Configuration_UserID", table: "Configuration", column: "UserID");

            migrationBuilder.CreateIndex(name: "IX_Item_Name", table: "Item", column: "Name", unique: true);

            migrationBuilder.CreateIndex(name: "IX_Item_UserID", table: "Item", column: "UserID");

            migrationBuilder.CreateIndex(name: "IX_Machine_UserID", table: "Machine", column: "UserID");

            migrationBuilder.CreateIndex(name: "IX_MachineCommand_MachineID", table: "MachineCommand", column: "MachineID");

            migrationBuilder.CreateIndex(name: "IX_MachineInitCommand_MachineID", table: "MachineInitCommand", column: "MachineID");

            migrationBuilder.CreateIndex(name: "IX_User_UserName", table: "User", column: "UserName", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Configuration");

            migrationBuilder.DropTable(name: "ItemProperty");

            migrationBuilder.DropTable(name: "MachineCommand");

            migrationBuilder.DropTable(name: "MachineInitCommand");

            migrationBuilder.DropTable(name: "Item");

            migrationBuilder.DropTable(name: "Machine");

            migrationBuilder.DropTable(name: "User");
        }
    }
}