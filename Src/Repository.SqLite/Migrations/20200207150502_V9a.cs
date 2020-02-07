using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V9a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create table Configuration_backup as select * from Configuration");
            migrationBuilder.Sql("create table ItemProperty_backup as select * from ItemProperty");
            migrationBuilder.Sql("create table MachineCommand_backup as select * from MachineCommand");
            migrationBuilder.Sql("create table MachineInitCommand_backup as select * from MachineInitCommand");
            migrationBuilder.Sql("create table UserFile_backup as select * from UserFile");
            migrationBuilder.Sql("create table Item_backup as select * from Item");
            migrationBuilder.Sql("create table Machine_backup as select * from Machine");
            migrationBuilder.Sql("create table User_backup as select * from User");
            migrationBuilder.Sql("create table Log_backup as select * from Log");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "ItemProperty");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "MachineCommand");

            migrationBuilder.DropTable(
                name: "MachineInitCommand");

            migrationBuilder.DropTable(
                name: "UserFile");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Machine");

            migrationBuilder.DropTable(
                name: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
