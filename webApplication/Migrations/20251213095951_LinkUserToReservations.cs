using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webApplication.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserToReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicantName",
                table: "SportRegistrations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Reservations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ApplicantName",
                table: "CourseRegistrations",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SportRegistrations_UserId",
                table: "SportRegistrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRegistrations_UserId",
                table: "CourseRegistrations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseRegistrations_AspNetUsers_UserId",
                table: "CourseRegistrations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SportRegistrations_AspNetUsers_UserId",
                table: "SportRegistrations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseRegistrations_AspNetUsers_UserId",
                table: "CourseRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_SportRegistrations_AspNetUsers_UserId",
                table: "SportRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_SportRegistrations_UserId",
                table: "SportRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_CourseRegistrations_UserId",
                table: "CourseRegistrations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SportRegistrations",
                newName: "ApplicantName");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservations",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CourseRegistrations",
                newName: "ApplicantName");
        }
    }
}
