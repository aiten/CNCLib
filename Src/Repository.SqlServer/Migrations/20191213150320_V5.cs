using Microsoft.EntityFrameworkCore.Migrations;

namespace CNCLib.Repository.SqlServer.Migrations
{
    public partial class V5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Configuration set UserId = (select top 1 UserId from [User]) where userid is null");
            migrationBuilder.Sql("update Machine set UserId = (select top 1 UserId from [User]) where userid is null");
            migrationBuilder.Sql("update Item set UserId = (select top 1 UserId from [User]) where UserId is null");

            migrationBuilder.Sql("update [user] set [Name]='global', Password='Z2xvYmFs' where [name] = 'Herbert'");


            migrationBuilder.DropForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Machine_User_UserId",
                table: "Machine");

            migrationBuilder.DropIndex(
                name: "IX_Machine_UserId",
                table: "Machine");

            migrationBuilder.DropIndex(
                name: "IX_Item_Name",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_UserId",
                table: "Item");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Configuration_Group_Name_UserId",
                table: "Configuration");

            migrationBuilder.DropIndex(
                name: "IX_Configuration_UserId",
                table: "Configuration");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Machine",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Item",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Machine_UserId_Name",
                table: "Machine",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Item_UserId_Name",
                table: "Item",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Configuration_UserId_Group_Name",
                table: "Configuration",
                columns: new[] { "UserId", "Group", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Machine_User_UserId",
                table: "Machine",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Machine_User_UserId",
                table: "Machine");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Machine_UserId_Name",
                table: "Machine");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Item_UserId_Name",
                table: "Item");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Configuration_UserId_Group_Name",
                table: "Configuration");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Machine",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Item",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Configuration_Group_Name_UserId",
                table: "Configuration",
                columns: new[] { "Group", "Name", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Machine_UserId",
                table: "Machine",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Name",
                table: "Item",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_UserId",
                table: "Item",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_UserId",
                table: "Configuration",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Machine_User_UserId",
                table: "Machine",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
