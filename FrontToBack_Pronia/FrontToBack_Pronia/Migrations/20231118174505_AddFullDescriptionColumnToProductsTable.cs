using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrontToBack_Pronia.Migrations
{
    /// <inheritdoc />
    public partial class AddFullDescriptionColumnToProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullDescription",
                table: "Products");
        }
    }
}
