using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DtrIsReset",
                table: "Machine",
                nullable: false,
                defaultValue: true);

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
            // not possible to drop columns in SqLite
        }
    }
}
