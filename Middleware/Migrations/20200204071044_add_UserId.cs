using Microsoft.EntityFrameworkCore.Migrations;

namespace Middleware.Migrations
{
    public partial class add_UserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Users_Userid",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "Userid",
                table: "Post",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_Userid",
                table: "Post",
                newName: "IX_Post_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Post",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Users_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Users_UserId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Post",
                newName: "Userid");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserId",
                table: "Post",
                newName: "IX_Post_Userid");

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Post",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Users_Userid",
                table: "Post",
                column: "Userid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
