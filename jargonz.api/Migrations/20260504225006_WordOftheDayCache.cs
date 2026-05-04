    using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jargonz.api.Migrations
{
    /// <inheritdoc />
    public partial class WordOftheDayCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordOfTheDayCache",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", nullable: false),
                    UserId = table.Column<string>(type: "character varying(26)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    WordEntryId = table.Column<string>(type: "character varying(26)", nullable: false),
                    CachedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordOfTheDayCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordOfTheDayCache_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordOfTheDayCache_Words_WordEntryId",
                        column: x => x.WordEntryId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordOfTheDayCache_UserId_Date",
                table: "WordOfTheDayCache",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WordOfTheDayCache_WordEntryId",
                table: "WordOfTheDayCache",
                column: "WordEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordOfTheDayCache");
        }
    }
}
