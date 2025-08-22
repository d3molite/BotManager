using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedLanPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanPlannerConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PlannerChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanPlannerConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanPlannerConfigs_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanPlans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Nickname = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    SeatingGroup = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    SeatA = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    SeatB = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    LanPlanId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanMembers_LanPlans_LanPlanId",
                        column: x => x.LanPlanId,
                        principalTable: "LanPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanMembers_LanPlanId",
                table: "LanMembers",
                column: "LanPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LanPlannerConfigs_GuildConfigId",
                table: "LanPlannerConfigs",
                column: "GuildConfigId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanMembers");

            migrationBuilder.DropTable(
                name: "LanPlannerConfigs");

            migrationBuilder.DropTable(
                name: "LanPlans");
        }
    }
}
