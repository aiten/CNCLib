using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                                             name: "DtrIsReset",
                                             table: "Machine",
                                             nullable: false,
                                             defaultValue: false);

            migrationBuilder.AddColumn<string>(
                                               name: "SerialServer",
                                               table: "Machine",
                                               nullable: true);

            migrationBuilder.AddColumn<int>(
                                            name: "SerialServerPort",
                                            table: "Machine",
                                            nullable: false,
                                            defaultValue: 5000);

            migrationBuilder.Sql("update Machine set DtrIsReset = CASE WHEN [NeedDtr]=0 THEN 1 ELSE 0 END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        name: "DtrIsReset",
                                        table: "Machine");

            migrationBuilder.DropColumn(
                                        name: "SerialServer",
                                        table: "Machine");

            migrationBuilder.DropColumn(
                                        name: "SerialServerPort",
                                        table: "Machine");
        }
    }
}