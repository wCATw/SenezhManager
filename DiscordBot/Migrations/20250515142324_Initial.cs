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
                name: "events",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MentionMessageId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Heading = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    IsRepeatable = table.Column<bool>(type: "INTEGER", nullable: true),
                    RepeatDayOfWeek = table.Column<int>(type: "INTEGER", nullable: true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExpireEventMessageDelayMinutes = table.Column<byte>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.GuildId);
                });

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
                name: "events");

            migrationBuilder.DropTable(
                name: "settings");
        }
    }
}
