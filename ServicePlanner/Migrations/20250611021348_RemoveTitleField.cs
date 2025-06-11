using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePlanner.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTitleField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Songs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Songs",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }
    }
}
