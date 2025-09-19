using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddsAdminUserstoBotConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminUserIds",
                table: "Configs",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUserIds",
                table: "Configs");
        }
    }
}
