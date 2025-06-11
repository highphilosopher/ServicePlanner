using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePlanner.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SongName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    SongSelectId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Seasonal = table.Column<bool>(type: "INTEGER", nullable: false),
                    Speed = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Artist = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Disabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEvents_ServiceTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ServiceTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_ServiceTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ServiceTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEventInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SongTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEventInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEventInstances_ServiceEvents_ServiceEventId",
                        column: x => x.ServiceEventId,
                        principalTable: "ServiceEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceEventInstances_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEventInstances_ServiceEventId",
                table: "ServiceEventInstances",
                column: "ServiceEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEventInstances_ServiceId",
                table: "ServiceEventInstances",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEvents_TemplateId",
                table: "ServiceEvents",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_TemplateId",
                table: "Services",
                column: "TemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceEventInstances");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "ServiceEvents");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "ServiceTemplates");
        }
    }
}
