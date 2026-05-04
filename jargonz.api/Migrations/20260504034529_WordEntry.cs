using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jargonz.api.Migrations
{
    /// <inheritdoc />
    public partial class WordEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", nullable: false),
                    Word = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    BookId = table.Column<string>(type: "character varying(26)", nullable: false),
                    Definition = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Phonetic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PartOfSpeech = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Etymology = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ContextSentence = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExampleSentence = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    UserNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: true),
                    EaseFactor = table.Column<double>(type: "double precision", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false),
                    Repetitions = table.Column<int>(type: "integer", nullable: false),
                    NextReviewDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TimesReviewed = table.Column<int>(type: "integer", nullable: false),
                    TimesCorrect = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Words_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Words_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_BookId",
                table: "Words",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_NextReviewDate",
                table: "Words",
                column: "NextReviewDate");

            migrationBuilder.CreateIndex(
                name: "IX_Words_UserId",
                table: "Words",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Word",
                table: "Words",
                column: "Word");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
