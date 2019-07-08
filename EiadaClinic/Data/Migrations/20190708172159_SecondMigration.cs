using Microsoft.EntityFrameworkCore.Migrations;

namespace EiadaClinic.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Assistants_AssistantId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_AssistantId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "Appointments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "AssistantId",
                table: "Doctors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AssistantId",
                table: "Doctors",
                column: "AssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Assistants_AssistantId",
                table: "Doctors",
                column: "AssistantId",
                principalTable: "Assistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
