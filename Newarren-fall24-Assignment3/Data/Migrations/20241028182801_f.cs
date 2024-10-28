using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Newarren_fall24_Assignment3.Data.Migrations
{
    /// <inheritdoc />
    public partial class f : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sentiments",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sentiments",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sentiments",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Sentiments",
                table: "Actors");
        }
    }
}
