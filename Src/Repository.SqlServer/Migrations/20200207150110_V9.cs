using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedDtr",
                table: "Machine");

            migrationBuilder.DropColumn(
                name: "SerialServerPort",
                table: "Machine");

            migrationBuilder.DropColumn(
                name: "SerialServerProtocol",
                table: "Machine");

            migrationBuilder.AlterColumn<string>(
                name: "SerialServer",
                table: "Machine",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SerialServer",
                table: "Machine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NeedDtr",
                table: "Machine",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SerialServerPort",
                table: "Machine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SerialServerProtocol",
                table: "Machine",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
