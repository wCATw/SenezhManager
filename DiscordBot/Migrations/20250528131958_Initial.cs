using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBot.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_templates",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_templates", x => x.InternalId);
                    table.UniqueConstraint("AK_event_templates_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.InternalId);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.InternalId);
                });

            migrationBuilder.CreateTable(
                name: "event_repeatability",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: true),
                    Time = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_repeatability", x => x.InternalId);
                    table.ForeignKey(
                        name: "FK_event_repeatability_event_templates_Id",
                        column: x => x.Id,
                        principalTable: "event_templates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_repeatability_GuildId_Id",
                table: "event_repeatability",
                columns: new[] { "GuildId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_repeatability_Id",
                table: "event_repeatability",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_templates_GuildId_Id",
                table: "event_templates",
                columns: new[] { "GuildId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_GuildId_Id",
                table: "events",
                columns: new[] { "GuildId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_settings_GuildId",
                table: "settings",
                column: "GuildId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_repeatability");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "event_templates");
        }
    }
}
