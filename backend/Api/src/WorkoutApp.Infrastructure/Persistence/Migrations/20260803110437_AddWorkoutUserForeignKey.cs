using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutUserForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "fk_workouts_users_user_id",
                table: "workouts",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_workouts_users_user_id",
                table: "workouts");
        }
    }
}
