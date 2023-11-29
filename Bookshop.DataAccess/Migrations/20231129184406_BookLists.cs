using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookshop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BookLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookListProduct",
                columns: table => new
                {
                    BookListsId = table.Column<int>(type: "int", nullable: false),
                    BooksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookListProduct", x => new { x.BookListsId, x.BooksId });
                    table.ForeignKey(
                        name: "FK_BookListProduct_BookLists_BookListsId",
                        column: x => x.BookListsId,
                        principalTable: "BookLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookListProduct_Products_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookListProduct_BooksId",
                table: "BookListProduct",
                column: "BooksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookListProduct");

            migrationBuilder.DropTable(
                name: "BookLists");
        }
    }
}
