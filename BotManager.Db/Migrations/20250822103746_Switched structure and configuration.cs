using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class Switchedstructureandconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LanMembers_LanPlans_LanPlanId",
                table: "LanMembers");

            migrationBuilder.AlterColumn<string>(
                name: "LanPlanId",
                table: "LanMembers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 64);

            migrationBuilder.AddForeignKey(
                name: "FK_LanMembers_LanPlans_LanPlanId",
                table: "LanMembers",
                column: "LanPlanId",
                principalTable: "LanPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LanMembers_LanPlans_LanPlanId",
                table: "LanMembers");

            migrationBuilder.AlterColumn<string>(
                name: "LanPlanId",
                table: "LanMembers",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LanMembers_LanPlans_LanPlanId",
                table: "LanMembers",
                column: "LanPlanId",
                principalTable: "LanPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
