using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaplexAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsAdditionalToSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdditional",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdditional",
                table: "Sales");
        }
    }
}
