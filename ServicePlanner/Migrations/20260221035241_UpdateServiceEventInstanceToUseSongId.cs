using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePlanner.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceEventInstanceToUseSongId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SongTitle",
                table: "ServiceEventInstances");

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "ServiceEventInstances",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEventInstances_SongId",
                table: "ServiceEventInstances",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceEventInstances_Songs_SongId",
                table: "ServiceEventInstances",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceEventInstances_Songs_SongId",
                table: "ServiceEventInstances");

            migrationBuilder.DropIndex(
                name: "IX_ServiceEventInstances_SongId",
                table: "ServiceEventInstances");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "ServiceEventInstances");

            migrationBuilder.AddColumn<string>(
                name: "SongTitle",
                table: "ServiceEventInstances",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }
    }
}
