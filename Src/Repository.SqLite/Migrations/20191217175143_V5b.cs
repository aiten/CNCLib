using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V5b : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Group = table.Column<string>(maxLength: 256, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Type = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationId);
                    table.ForeignKey(
                        name: "FK_Configuration_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    ClassName = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Item_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Machine",
                columns: table => new
                {
                    MachineId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    SerialServer = table.Column<string>(nullable: true),
                    SerialServerPort = table.Column<int>(nullable: false),
                    ComPort = table.Column<string>(maxLength: 32, nullable: false),
                    Axis = table.Column<int>(nullable: false),
                    BaudRate = table.Column<int>(nullable: false),
                    DtrIsReset = table.Column<bool>(nullable: false),
                    NeedDtr = table.Column<bool>(nullable: false),
                    SizeX = table.Column<decimal>(nullable: false),
                    SizeY = table.Column<decimal>(nullable: false),
                    SizeZ = table.Column<decimal>(nullable: false),
                    SizeA = table.Column<decimal>(nullable: false),
                    SizeB = table.Column<decimal>(nullable: false),
                    SizeC = table.Column<decimal>(nullable: false),
                    BufferSize = table.Column<int>(nullable: false),
                    CommandToUpper = table.Column<bool>(nullable: false),
                    ProbeSizeX = table.Column<decimal>(nullable: false),
                    ProbeSizeY = table.Column<decimal>(nullable: false),
                    ProbeSizeZ = table.Column<decimal>(nullable: false),
                    ProbeDistUp = table.Column<decimal>(nullable: false),
                    ProbeDist = table.Column<decimal>(nullable: false),
                    ProbeFeed = table.Column<decimal>(nullable: false),
                    SDSupport = table.Column<bool>(nullable: false),
                    Spindle = table.Column<bool>(nullable: false),
                    Coolant = table.Column<bool>(nullable: false),
                    Laser = table.Column<bool>(nullable: false),
                    Rotate = table.Column<bool>(nullable: false),
                    CommandSyntax = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machine", x => x.MachineId);
                    table.ForeignKey(
                        name: "FK_Machine_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFile",
                columns: table => new
                {
                    UserFileId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 1024, nullable: false),
                    Content = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFile", x => x.UserFileId);
                    table.ForeignKey(
                        name: "FK_UserFile_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemProperty",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemProperty", x => new { x.ItemId, x.Name });
                    table.ForeignKey(
                        name: "FK_ItemProperty_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineCommand",
                columns: table => new
                {
                    MachineCommandId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommandName = table.Column<string>(maxLength: 64, nullable: false),
                    CommandString = table.Column<string>(maxLength: 64, nullable: false),
                    MachineId = table.Column<int>(nullable: false),
                    PosX = table.Column<int>(nullable: true),
                    PosY = table.Column<int>(nullable: true),
                    JoystickMessage = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineCommand", x => x.MachineCommandId);
                    table.ForeignKey(
                        name: "FK_MachineCommand_Machine_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machine",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineInitCommand",
                columns: table => new
                {
                    MachineInitCommandId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeqNo = table.Column<int>(nullable: false),
                    CommandString = table.Column<string>(maxLength: 64, nullable: false),
                    MachineId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineInitCommand", x => x.MachineInitCommandId);
                    table.ForeignKey(
                        name: "FK_MachineInitCommand_Machine_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machine",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_UserId_Group_Name",
                table: "Configuration",
                columns: new[] { "UserId", "Group", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_UserId_Name",
                table: "Item",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machine_UserId_Name",
                table: "Machine",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineCommand_MachineId",
                table: "MachineCommand",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineInitCommand_MachineId",
                table: "MachineInitCommand",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                table: "User",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFile_UserId_FileName",
                table: "UserFile",
                columns: new[] { "UserId", "FileName" },
                unique: true);

            migrationBuilder.Sql("insert into User (UserId,Name,Password) select UserId,Name,Password from User_backup");

            migrationBuilder.Sql("insert into Configuration ([ConfigurationId],[UserId],[Group],[Name],[Type],[Value]) select [ConfigurationId],[UserId],[Group],[Name],[Type],[Value] from Configuration_backup");
            migrationBuilder.Sql("insert into UserFile (UserFileId,UserId,FileName,Content) select UserFileId,UserId,FileName,Content from UserFile_backup");

            migrationBuilder.Sql("insert into Item (ItemId,UserId,Name,ClassName) select ItemId,UserId,Name,ClassName from Item_backup");
            migrationBuilder.Sql("insert into ItemProperty ([ItemId],[Name],[Value]) select [ItemId],[Name],[Value] from ItemProperty_backup");

            migrationBuilder.Sql("insert into Machine ([MachineId],[UserId],[Name],[SerialServer],[SerialServerPort],[ComPort],[Axis],[BaudRate],[DtrIsReset],[NeedDtr],[SizeX],[SizeY],[SizeZ],[SizeA],[SizeB],[SizeC],[BufferSize],[CommandToUpper],[ProbeSizeX],[ProbeSizeY],[ProbeSizeZ],[ProbeDistUp],[ProbeDist],[ProbeFeed],[SDSupport],[Spindle],[Coolant],[Laser],[Rotate],[CommandSyntax]) select [MachineId],[UserId],[Name],[SerialServer],[SerialServerPort],[ComPort],[Axis],[BaudRate],[DtrIsReset],[NeedDtr],[SizeX],[SizeY],[SizeZ],[SizeA],[SizeB],[SizeC],[BufferSize],[CommandToUpper],[ProbeSizeX],[ProbeSizeY],[ProbeSizeZ],[ProbeDistUp],[ProbeDist],[ProbeFeed],[SDSupport],[Spindle],[Coolant],[Laser],[Rotate],[CommandSyntax] from Machine_backup");
            migrationBuilder.Sql("insert into MachineCommand ([MachineCommandId],[CommandName],[CommandString],[MachineId],[PosX],[PosY],[JoystickMessage]) select [MachineCommandId],[CommandName],[CommandString],[MachineId],[PosX],[PosY],[JoystickMessage] from MachineCommand_backup");
            migrationBuilder.Sql("insert into MachineInitCommand ([MachineInitCommandId],[SeqNo],[CommandString],[MachineId]) select [MachineInitCommandId],[SeqNo],[CommandString],[MachineId] from MachineInitCommand_backup");


            migrationBuilder.Sql("drop table Configuration_backup");
            migrationBuilder.Sql("drop table ItemProperty_backup");
            migrationBuilder.Sql("drop table MachineCommand_backup");
            migrationBuilder.Sql("drop table MachineInitCommand_backup");
            migrationBuilder.Sql("drop table UserFile_backup");
            migrationBuilder.Sql("drop table Item_backup");
            migrationBuilder.Sql("drop table Machine_backup");
            migrationBuilder.Sql("drop table User_backup");

            migrationBuilder.Sql("update user set Name='global', Password='Z2xvYmFs' where name = 'Herbert'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // not possible
        }
    }
}
