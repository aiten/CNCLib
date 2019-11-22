using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserPassword",
                table: "User",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "User",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_User_UserName",
                table: "User",
                newName: "IX_User_Name");

            migrationBuilder.CreateTable(
                name: "UserFile",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 1024, nullable: false),
                    Content = table.Column<byte[]>(nullable: true)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // not possible to drop columns in SqLite
        }
    }
}
