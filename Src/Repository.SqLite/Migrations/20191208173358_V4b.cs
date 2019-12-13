using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V4b : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Group = table.Column<string>(maxLength: 256, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Type = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(maxLength: 4000, nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationId);
                    table.UniqueConstraint("AK_Configuration_Group_Name_UserId", x => new { x.Group, x.Name, x.UserId });
                    table.ForeignKey(
                        name: "FK_Configuration_User_UserId",
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
                    FileName = table.Column<string>(maxLength: 1024, nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Content = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFile", x => x.UserFileId);
                    table.UniqueConstraint("AK_UserFile_UserId_FileName", x => new { x.UserId, x.FileName });
                    table.ForeignKey(
                        name: "FK_UserFile_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_UserId",
                table: "Configuration",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "UserFile");
        }
    }
}
