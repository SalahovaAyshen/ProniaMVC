using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrontToBack_Pronia.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableProductSizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductSizeId",
                table: "Sizes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductProductSize",
                columns: table => new
                {
                    ProductSizesId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProductSize", x => new { x.ProductSizesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_ProductProductSize_ProductSizes_ProductSizesId",
                        column: x => x.ProductSizesId,
                        principalTable: "ProductSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductProductSize_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_ProductSizeId",
                table: "Sizes",
                column: "ProductSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductSize_ProductsId",
                table: "ProductProductSize",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sizes_ProductSizes_ProductSizeId",
                table: "Sizes",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sizes_ProductSizes_ProductSizeId",
                table: "Sizes");

            migrationBuilder.DropTable(
                name: "ProductProductSize");

            migrationBuilder.DropTable(
                name: "ProductSizes");

            migrationBuilder.DropIndex(
                name: "IX_Sizes_ProductSizeId",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "ProductSizeId",
                table: "Sizes");
        }
    }
}
