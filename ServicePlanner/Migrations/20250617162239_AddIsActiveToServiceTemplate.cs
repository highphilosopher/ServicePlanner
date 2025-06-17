using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePlanner.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToServiceTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ServiceTemplates",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ServiceTemplates");
        }
    }
}
