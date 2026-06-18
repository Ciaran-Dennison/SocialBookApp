using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialBookAppInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorGenresString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Genres",
                table: "Authors",
                newName: "GenresString");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GenresString",
                table: "Authors",
                newName: "Genres");
        }
    }
}
