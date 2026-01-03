using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mariolympics.Migrations
{
    /// <inheritdoc />
    public partial class CreatedScoreProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Player",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Player");
        }
    }
}
