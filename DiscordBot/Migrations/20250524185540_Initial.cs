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
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_templates", x => new { x.GuildId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => new { x.GuildId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleChannelId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    ScheduleMessageId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "event_repeatability",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: true),
                    Time = table.Column<TimeSpan>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_repeatability", x => new { x.GuildId, x.Id });
                    table.ForeignKey(
                        name: "FK_event_repeatability_event_templates_GuildId_Id",
                        columns: x => new { x.GuildId, x.Id },
                        principalTable: "event_templates",
                        principalColumns: new[] { "GuildId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_repeatability_GuildId_Id",
                table: "event_repeatability",
                columns: new[] { "GuildId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_templates_GuildId_Id",
                table: "event_templates",
                columns: new[] { "GuildId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_GuildId_ChannelId_MessageId",
                table: "events",
                columns: new[] { "GuildId", "ChannelId", "MessageId" },
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
