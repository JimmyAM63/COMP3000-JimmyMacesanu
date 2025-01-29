using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaplexAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDataTypeDateTimeTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MonthYear",
                table: "Targets",
                type: "date",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MonthYear",
                table: "Targets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
