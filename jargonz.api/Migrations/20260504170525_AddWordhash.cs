using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jargonz.api.Migrations
{
    /// <inheritdoc />
    public partial class AddWordhash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WordHash",
                table: "Words",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Words_WordHash",
                table: "Words",
                column: "WordHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_WordHash",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "WordHash",
                table: "Words");
        }
    }
}
