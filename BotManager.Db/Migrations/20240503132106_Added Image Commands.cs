using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ItemNumber",
                table: "OrderItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ImageConfigId",
                table: "GuildConfigs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoiceChannelConfigId",
                table: "GuildConfigs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoiceChannelConfig",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CommandChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    VoiceCategoryId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceChannelConfig", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_ImageConfigId",
                table: "GuildConfigs",
                column: "ImageConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_VoiceChannelConfigId",
                table: "GuildConfigs",
                column: "VoiceChannelConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildConfigs_ImageConfigs_ImageConfigId",
                table: "GuildConfigs",
                column: "ImageConfigId",
                principalTable: "ImageConfigs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildConfigs_VoiceChannelConfig_VoiceChannelConfigId",
                table: "GuildConfigs",
                column: "VoiceChannelConfigId",
                principalTable: "VoiceChannelConfig",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildConfigs_ImageConfigs_ImageConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildConfigs_VoiceChannelConfig_VoiceChannelConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "ImageConfigs");

            migrationBuilder.DropTable(
                name: "VoiceChannelConfig");

            migrationBuilder.DropIndex(
                name: "IX_GuildConfigs_ImageConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropIndex(
                name: "IX_GuildConfigs_VoiceChannelConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "ImageConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "VoiceChannelConfigId",
                table: "GuildConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "ItemNumber",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
