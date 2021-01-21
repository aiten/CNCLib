using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "UserFile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadTime",
                table: "UserFile",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("update UserFile set IsSystem=1 where FileName <> '$$$'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "UserFile");

            migrationBuilder.DropColumn(
                name: "UploadTime",
                table: "UserFile");
        }
    }
}
