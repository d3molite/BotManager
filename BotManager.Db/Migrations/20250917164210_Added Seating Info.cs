using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeatingInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "SeatingMapChannelId",
                table: "LanPlannerConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "SeatName",
                table: "LanMembers",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SeatingOrder",
                table: "LanMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatingMapChannelId",
                table: "LanPlannerConfigs");

            migrationBuilder.DropColumn(
                name: "SeatName",
                table: "LanMembers");

            migrationBuilder.DropColumn(
                name: "SeatingOrder",
                table: "LanMembers");
        }
    }
}
