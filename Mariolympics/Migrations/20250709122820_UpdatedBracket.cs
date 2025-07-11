using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mariolympics.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBracket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BronzeMedalMatchId",
                table: "Bracket",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBronzeMedalMatch",
                table: "Bracket",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Bracket_BronzeMedalMatchId",
                table: "Bracket",
                column: "BronzeMedalMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bracket_Match_BronzeMedalMatchId",
                table: "Bracket",
                column: "BronzeMedalMatchId",
                principalTable: "Match",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bracket_Match_BronzeMedalMatchId",
                table: "Bracket");

            migrationBuilder.DropIndex(
                name: "IX_Bracket_BronzeMedalMatchId",
                table: "Bracket");

            migrationBuilder.DropColumn(
                name: "BronzeMedalMatchId",
                table: "Bracket");

            migrationBuilder.DropColumn(
                name: "HasBronzeMedalMatch",
                table: "Bracket");
        }
    }
}
