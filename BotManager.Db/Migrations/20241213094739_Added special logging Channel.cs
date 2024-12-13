using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedspecialloggingChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "CriticalMessageChannelId",
                table: "LoggingConfigs",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CriticalMessageChannelId",
                table: "LoggingConfigs");
        }
    }
}
