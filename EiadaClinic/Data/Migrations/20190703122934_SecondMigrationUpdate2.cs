using Microsoft.EntityFrameworkCore.Migrations;

namespace EiadaClinic.Data.Migrations
{
    public partial class SecondMigrationUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Assistants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assistants_UserId",
                table: "Assistants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assistants_AspNetUsers_UserId",
                table: "Assistants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assistants_AspNetUsers_UserId",
                table: "Assistants");

            migrationBuilder.DropIndex(
                name: "IX_Assistants_UserId",
                table: "Assistants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Assistants");
        }
    }
}
