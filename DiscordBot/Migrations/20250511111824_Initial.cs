using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    StartTimeSpan = table.Column<TimeSpan>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => new { x.GuildId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "ranks",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ranks", x => new { x.GuildId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrationChannelId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "statuses",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statuses", x => new { x.GuildId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CallSign = table.Column<string>(type: "TEXT", nullable: true),
                    StatusName = table.Column<string>(type: "TEXT", nullable: true),
                    RankName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => new { x.GuildId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_members_ranks_GuildId_RankName",
                        columns: x => new { x.GuildId, x.RankName },
                        principalTable: "ranks",
                        principalColumns: new[] { "GuildId", "Name" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_members_statuses_GuildId_StatusName",
                        columns: x => new { x.GuildId, x.StatusName },
                        principalTable: "statuses",
                        principalColumns: new[] { "GuildId", "Name" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ranks",
                columns: new[] { "GuildId", "Name", "Order" },
                values: new object[,]
                {
                    { 478923127120723969ul, "Капитан", 12 },
                    { 478923127120723969ul, "Капрал", 2 },
                    { 478923127120723969ul, "Лейтенант", 10 },
                    { 478923127120723969ul, "Майор", 13 },
                    { 478923127120723969ul, "Мл Сержант", 3 },
                    { 478923127120723969ul, "Мл. Лейтенант", 9 },
                    { 478923127120723969ul, "Новобранец", 0 },
                    { 478923127120723969ul, "Подполковник", 14 },
                    { 478923127120723969ul, "Полковник", 15 },
                    { 478923127120723969ul, "Прапорщик", 7 },
                    { 478923127120723969ul, "Рядовой", 1 },
                    { 478923127120723969ul, "Сержант", 4 },
                    { 478923127120723969ul, "Ст. Лейтенант", 11 },
                    { 478923127120723969ul, "Ст. Прапорщик", 8 },
                    { 478923127120723969ul, "Ст. Сержант", 5 },
                    { 478923127120723969ul, "Старшина", 6 }
                });

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "GuildId", "RegistrationChannelId" },
                values: new object[] { 478923127120723969ul, 1201393133292896286ul });

            migrationBuilder.InsertData(
                table: "statuses",
                columns: new[] { "GuildId", "Name" },
                values: new object[,]
                {
                    { 478923127120723969ul, "Активен" },
                    { 478923127120723969ul, "В отпуске" },
                    { 478923127120723969ul, "В резерве" },
                    { 478923127120723969ul, "Выбыл" },
                    { 478923127120723969ul, "Неизвестно" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_GuildId_Id",
                table: "events",
                columns: new[] { "GuildId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_GuildId_MemberId",
                table: "members",
                columns: new[] { "GuildId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_members_GuildId_RankName",
                table: "members",
                columns: new[] { "GuildId", "RankName" });

            migrationBuilder.CreateIndex(
                name: "IX_members_GuildId_StatusName",
                table: "members",
                columns: new[] { "GuildId", "StatusName" });

            migrationBuilder.CreateIndex(
                name: "IX_ranks_GuildId_Name",
                table: "ranks",
                columns: new[] { "GuildId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ranks_GuildId_Order",
                table: "ranks",
                columns: new[] { "GuildId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_settings_GuildId",
                table: "settings",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_statuses_GuildId_Name",
                table: "statuses",
                columns: new[] { "GuildId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "ranks");

            migrationBuilder.DropTable(
                name: "statuses");
        }
    }
}
