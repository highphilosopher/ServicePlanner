using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonalToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeasonal",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeasonal",
                table: "Services");
        }
    }
}
