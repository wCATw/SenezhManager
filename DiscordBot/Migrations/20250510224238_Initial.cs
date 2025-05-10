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
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
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
                    Id = table.Column<byte>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ranks", x => new { x.GuildId, x.Id });
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
                    Id = table.Column<byte>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statuses", x => new { x.GuildId, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CallSign = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StatusId = table.Column<byte>(type: "INTEGER", nullable: true),
                    RankId = table.Column<byte>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => new { x.GuildId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_members_ranks_GuildId_RankId",
                        columns: x => new { x.GuildId, x.RankId },
                        principalTable: "ranks",
                        principalColumns: new[] { "GuildId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_members_statuses_GuildId_StatusId",
                        columns: x => new { x.GuildId, x.StatusId },
                        principalTable: "statuses",
                        principalColumns: new[] { "GuildId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ranks",
                columns: new[] { "GuildId", "Id", "Name", "Order" },
                values: new object[,]
                {
                    { 478923127120723969ul, (byte)0, "Новобранец", 0 },
                    { 478923127120723969ul, (byte)1, "Рядовой", 1 },
                    { 478923127120723969ul, (byte)2, "Капрал", 2 },
                    { 478923127120723969ul, (byte)3, "Мл Сержант", 3 },
                    { 478923127120723969ul, (byte)4, "Сержант", 4 },
                    { 478923127120723969ul, (byte)5, "Ст. Сержант", 5 },
                    { 478923127120723969ul, (byte)6, "Старшина", 6 },
                    { 478923127120723969ul, (byte)7, "Прапорщик", 7 },
                    { 478923127120723969ul, (byte)8, "Ст. Прапорщик", 8 },
                    { 478923127120723969ul, (byte)9, "Мл. Лейтенант", 9 },
                    { 478923127120723969ul, (byte)10, "Лейтенант", 10 },
                    { 478923127120723969ul, (byte)11, "Ст. Лейтенант", 11 },
                    { 478923127120723969ul, (byte)12, "Капитан", 12 },
                    { 478923127120723969ul, (byte)13, "Майор", 13 },
                    { 478923127120723969ul, (byte)14, "Подполковник", 14 },
                    { 478923127120723969ul, (byte)15, "Полковник", 15 }
                });

            migrationBuilder.InsertData(
                table: "statuses",
                columns: new[] { "GuildId", "Id", "Name" },
                values: new object[,]
                {
                    { 478923127120723969ul, (byte)0, "Неизвестно" },
                    { 478923127120723969ul, (byte)1, "Активен" },
                    { 478923127120723969ul, (byte)2, "В отпуске" },
                    { 478923127120723969ul, (byte)3, "В резерве" },
                    { 478923127120723969ul, (byte)4, "Выбыл" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_members_GuildId_RankId",
                table: "members",
                columns: new[] { "GuildId", "RankId" });

            migrationBuilder.CreateIndex(
                name: "IX_members_GuildId_StatusId",
                table: "members",
                columns: new[] { "GuildId", "StatusId" });
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
