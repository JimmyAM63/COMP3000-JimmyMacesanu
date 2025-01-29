using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaplexAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDiscountedToSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "Sales");
        }
    }
}
