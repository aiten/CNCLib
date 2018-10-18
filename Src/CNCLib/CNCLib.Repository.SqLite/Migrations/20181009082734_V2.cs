using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqLite.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogDate = table.Column<DateTime>(nullable: false),
                    Application = table.Column<string>(maxLength: 50, nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Message = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 250, nullable: true),
                    ServerName = table.Column<string>(maxLength: 64, nullable: true),
                    MachineName = table.Column<string>(maxLength: 64, nullable: true),
                    Port = table.Column<string>(maxLength: 256, nullable: true),
                    Url = table.Column<string>(maxLength: 500, nullable: true),
                    ServerAddress = table.Column<string>(maxLength: 100, nullable: true),
                    RemoteAddress = table.Column<string>(maxLength: 100, nullable: true),
                    Logger = table.Column<string>(maxLength: 250, nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");
        }
    }
}
