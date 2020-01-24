﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialServerPassword",
                table: "Machine",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialServerUser",
                table: "Machine",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // not possible
        }
    }
}
