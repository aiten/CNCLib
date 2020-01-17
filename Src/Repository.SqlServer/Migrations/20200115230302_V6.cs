using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialServerProtocol",
                table: "Machine",
                maxLength: 10,
                nullable: true);

            migrationBuilder.Sql("update Machine set SerialServerProtocol='http'");
            migrationBuilder.Sql("update ItemProperty set value = N'Hpgl' where name = 'LoadType' and value = N'HPGL'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialServerProtocol",
                table: "Machine");
        }
    }
}
