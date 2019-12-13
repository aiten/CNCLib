using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V5a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Configuration set UserId = 1 where userid is null");
            migrationBuilder.Sql("update Machine set UserId = 1 where userid is null");
            migrationBuilder.Sql("update Item set UserId = 1 where UserId is null");

            migrationBuilder.Sql("create table Configuration_backup as select * from Configuration");
            migrationBuilder.Sql("create table ItemProperty_backup as select * from ItemProperty");
            migrationBuilder.Sql("create table MachineCommand_backup as select * from MachineCommand");
            migrationBuilder.Sql("create table MachineInitCommand_backup as select * from MachineInitCommand");
            migrationBuilder.Sql("create table UserFile_backup as select * from UserFile");
            migrationBuilder.Sql("create table Item_backup as select * from Item");
            migrationBuilder.Sql("create table Machine_backup as select * from Machine");
            migrationBuilder.Sql("create table User_backup as select * from User");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "ItemProperty");

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
            // not possible to drop columns in SqLite
        }
    }
}
