using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_User_UserId",
                table: "Configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Configuration",
                table: "Configuration");

            migrationBuilder.AddColumn<int>(
                name: "UserFileId",
                table: "UserFile",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Configuration",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConfigurationId",
                table: "Configuration",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile",
                column: "UserFileId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserFile_UserId_FileName",
                table: "UserFile",
                columns: new[] { "UserId", "FileName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configuration",
                table: "Configuration",
                column: "ConfigurationId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Configuration_Group_Name_UserId",
                table: "Configuration",
                columns: new[] { "Group", "Name", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_User_UserId",
                table: "Configuration",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_User_UserId",
                table: "Configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserFile_UserId_FileName",
                table: "UserFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Configuration",
                table: "Configuration");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Configuration_Group_Name_UserId",
                table: "Configuration");

            migrationBuilder.DropColumn(
                name: "UserFileId",
                table: "UserFile");

            migrationBuilder.DropColumn(
                name: "ConfigurationId",
                table: "Configuration");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Configuration",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile",
                columns: new[] { "UserId", "FileName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Configuration",
                table: "Configuration",
                columns: new[] { "Group", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_User_UserId",
                table: "Configuration",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
