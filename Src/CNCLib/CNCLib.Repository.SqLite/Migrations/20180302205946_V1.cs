using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"PRAGMA foreign_keys = 0;
 
              CREATE TABLE Machine_temp AS SELECT *
                                            FROM Machine;
              
              DROP TABLE Machine;
              
              CREATE TABLE Machine (
                  Id         INTEGER NOT NULL CONSTRAINT PK_Machine PRIMARY KEY AUTOINCREMENT,
                  Address    TEXT,
                  City       TEXT,
                  Email      TEXT,
                  Name       TEXT,
                  Phone      TEXT,
                  PostalCode TEXT,
                  Subregion  TEXT
              );
              
              INSERT INTO Machine 
              (
                  Id,
                  Address,
                  City,
                  Email,
                  Name,
                  Phone,
                  PostalCode,
                  Subregion
              )
              SELECT Id,
                     Address,
                     City,
                     Email,
                     Name,
                     Phone,
                     PostalCode,
                     State
              FROM Machine_temp;
              
              DROP TABLE Machine_temp;
              
              PRAGMA foreign_keys = 1;");


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
        }
    }
}
