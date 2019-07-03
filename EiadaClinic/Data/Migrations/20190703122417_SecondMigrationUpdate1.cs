using Microsoft.EntityFrameworkCore.Migrations;

namespace EiadaClinic.Data.Migrations
{
    public partial class SecondMigrationUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assistants_AspNetUsers_UserId",
                table: "Assistants");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Assistants",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Assistants_UserId",
                table: "Assistants",
                newName: "IX_Assistants_DoctorId");

            migrationBuilder.AddColumn<string>(
                name: "AssistantId",
                table: "Doctors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AssistantId",
                table: "Doctors",
                column: "AssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assistants_Doctors_DoctorId",
                table: "Assistants",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Assistants_AssistantId",
                table: "Doctors",
                column: "AssistantId",
                principalTable: "Assistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assistants_Doctors_DoctorId",
                table: "Assistants");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Assistants_AssistantId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_AssistantId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Assistants",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Assistants_DoctorId",
                table: "Assistants",
                newName: "IX_Assistants_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assistants_AspNetUsers_UserId",
                table: "Assistants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
