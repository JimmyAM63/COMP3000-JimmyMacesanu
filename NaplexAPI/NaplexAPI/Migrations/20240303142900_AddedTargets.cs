using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaplexAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdditionalAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntertainmentAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntertainmentTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HBBAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HBBTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HBBUpAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HBBUpTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RevAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RevTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TalkMobileAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TalkMobileTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnlimitedAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnlimitedTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpgradesAct",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpgradesTar",
                table: "Targets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "AdditionalTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "EntertainmentAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "EntertainmentTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "HBBAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "HBBTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "HBBUpAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "HBBUpTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "InsuranceAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "InsuranceTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "RevAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "RevTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "TalkMobileAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "TalkMobileTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "UnlimitedAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "UnlimitedTar",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "UpgradesAct",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "UpgradesTar",
                table: "Targets");
        }
    }
}
