using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialBookAppInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePublisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookPublisher");

            migrationBuilder.DropTable(
                name: "BookPublisher1");

            migrationBuilder.DropTable(
                name: "Publishers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookPublisher",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    PublishersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookPublisher", x => new { x.BooksId, x.PublishersId });
                    table.ForeignKey(
                        name: "FK_BookPublisher_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookPublisher_Publishers_PublishersId",
                        column: x => x.PublishersId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookPublisher1",
                columns: table => new
                {
                    BestSellersId = table.Column<int>(type: "int", nullable: false),
                    PublisherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookPublisher1", x => new { x.BestSellersId, x.PublisherId });
                    table.ForeignKey(
                        name: "FK_BookPublisher1_Books_BestSellersId",
                        column: x => x.BestSellersId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookPublisher1_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookPublisher_PublishersId",
                table: "BookPublisher",
                column: "PublishersId");

            migrationBuilder.CreateIndex(
                name: "IX_BookPublisher1_PublisherId",
                table: "BookPublisher1",
                column: "PublisherId");
        }
    }
}
