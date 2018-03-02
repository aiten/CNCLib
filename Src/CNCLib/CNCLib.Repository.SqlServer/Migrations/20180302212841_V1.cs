using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NeedDtr",
                table: "Machine",
                newName: "DtrIsReset");

            migrationBuilder.Sql("update Machine set DtrIsReset = CASE WHEN [DtrIsReset]=0 THEN 1 ELSE 0 END");

            migrationBuilder.AddColumn<string>(
                name: "SerialServer",
                table: "Machine",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SerialServerPort",
                table: "Machine",
                nullable: false,
                defaultValue: 5000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialServer",
                table: "Machine");

            migrationBuilder.DropColumn(
                name: "SerialServerPort",
                table: "Machine");

            migrationBuilder.Sql("update Machine set DtrIsReset = CASE WHEN [DtrIsReset]=0 THEN 1 ELSE 0 END");

            migrationBuilder.RenameColumn(
                name: "DtrIsReset",
                table: "Machine",
                newName: "NeedDtr");
        }
    }
}
