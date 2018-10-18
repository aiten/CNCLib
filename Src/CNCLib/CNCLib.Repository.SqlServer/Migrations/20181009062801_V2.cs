using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_User_UserID",
                table: "Configuration");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_User_UserID",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemProperty_Item_ItemID",
                table: "ItemProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_Machine_User_UserID",
                table: "Machine");

            migrationBuilder.DropForeignKey(
                name: "FK_MachineCommand_Machine_MachineID",
                table: "MachineCommand");

            migrationBuilder.DropForeignKey(
                name: "FK_MachineInitCommand_Machine_MachineID",
                table: "MachineInitCommand");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "User",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "MachineID",
                table: "MachineInitCommand",
                newName: "MachineId");

            migrationBuilder.RenameColumn(
                name: "MachineInitCommandID",
                table: "MachineInitCommand",
                newName: "MachineInitCommandId");

            migrationBuilder.RenameIndex(
                name: "IX_MachineInitCommand_MachineID",
                table: "MachineInitCommand",
                newName: "IX_MachineInitCommand_MachineId");

            migrationBuilder.RenameColumn(
                name: "MachineID",
                table: "MachineCommand",
                newName: "MachineId");

            migrationBuilder.RenameColumn(
                name: "MachineCommandID",
                table: "MachineCommand",
                newName: "MachineCommandId");

            migrationBuilder.RenameIndex(
                name: "IX_MachineCommand_MachineID",
                table: "MachineCommand",
                newName: "IX_MachineCommand_MachineId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Machine",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "MachineID",
                table: "Machine",
                newName: "MachineId");

            migrationBuilder.RenameIndex(
                name: "IX_Machine_UserID",
                table: "Machine",
                newName: "IX_Machine_UserId");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                table: "ItemProperty",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Item",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ItemID",
                table: "Item",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Item_UserID",
                table: "Item",
                newName: "IX_Item_UserId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Configuration",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Configuration_UserID",
                table: "Configuration",
                newName: "IX_Configuration_UserId");

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LogDate = table.Column<DateTime>(nullable: false),
                    Application = table.Column<string>(maxLength: 50, nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Message = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 250, nullable: true),
                    ServerName = table.Column<string>(maxLength: 64, nullable: true),
                    MachineName = table.Column<string>(maxLength: 64, nullable: true),
                    Port = table.Column<string>(maxLength: 256, nullable: true),
                    Url = table.Column<string>(maxLength: 500, nullable: true),
                    ServerAddress = table.Column<string>(maxLength: 100, nullable: true),
                    RemoteAddress = table.Column<string>(maxLength: 100, nullable: true),
                    Logger = table.Column<string>(maxLength: 250, nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_User_UserId",
                table: "Configuration",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemProperty_Item_ItemId",
                table: "ItemProperty",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Machine_User_UserId",
                table: "Machine",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineCommand_Machine_MachineId",
                table: "MachineCommand",
                column: "MachineId",
                principalTable: "Machine",
                principalColumn: "MachineId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineInitCommand_Machine_MachineId",
                table: "MachineInitCommand",
                column: "MachineId",
                principalTable: "Machine",
                principalColumn: "MachineId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_User_UserId",
                table: "Configuration");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemProperty_Item_ItemId",
                table: "ItemProperty");

            migrationBuilder.DropForeignKey(
                name: "FK_Machine_User_UserId",
                table: "Machine");

            migrationBuilder.DropForeignKey(
                name: "FK_MachineCommand_Machine_MachineId",
                table: "MachineCommand");

            migrationBuilder.DropForeignKey(
                name: "FK_MachineInitCommand_Machine_MachineId",
                table: "MachineInitCommand");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "MachineId",
                table: "MachineInitCommand",
                newName: "MachineID");

            migrationBuilder.RenameColumn(
                name: "MachineInitCommandId",
                table: "MachineInitCommand",
                newName: "MachineInitCommandID");

            migrationBuilder.RenameIndex(
                name: "IX_MachineInitCommand_MachineId",
                table: "MachineInitCommand",
                newName: "IX_MachineInitCommand_MachineID");

            migrationBuilder.RenameColumn(
                name: "MachineId",
                table: "MachineCommand",
                newName: "MachineID");

            migrationBuilder.RenameColumn(
                name: "MachineCommandId",
                table: "MachineCommand",
                newName: "MachineCommandID");

            migrationBuilder.RenameIndex(
                name: "IX_MachineCommand_MachineId",
                table: "MachineCommand",
                newName: "IX_MachineCommand_MachineID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Machine",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "MachineId",
                table: "Machine",
                newName: "MachineID");

            migrationBuilder.RenameIndex(
                name: "IX_Machine_UserId",
                table: "Machine",
                newName: "IX_Machine_UserID");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ItemProperty",
                newName: "ItemID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Item",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Item",
                newName: "ItemID");

            migrationBuilder.RenameIndex(
                name: "IX_Item_UserId",
                table: "Item",
                newName: "IX_Item_UserID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Configuration",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Configuration_UserId",
                table: "Configuration",
                newName: "IX_Configuration_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_User_UserID",
                table: "Configuration",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_User_UserID",
                table: "Item",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemProperty_Item_ItemID",
                table: "ItemProperty",
                column: "ItemID",
                principalTable: "Item",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Machine_User_UserID",
                table: "Machine",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineCommand_Machine_MachineID",
                table: "MachineCommand",
                column: "MachineID",
                principalTable: "Machine",
                principalColumn: "MachineID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineInitCommand_Machine_MachineID",
                table: "MachineInitCommand",
                column: "MachineID",
                principalTable: "Machine",
                principalColumn: "MachineID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
