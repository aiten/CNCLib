using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V4a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "UserFile");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Group = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Value = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => new { x.Group, x.Name });
                    table.ForeignKey(
                        name: "FK_Configuration_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserFile",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFile", x => new { x.UserId, x.FileName });
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
    }
}
