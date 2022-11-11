using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_product_id_column_in_review_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "Review",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProductId",
                table: "Review",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_ProductId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Review");
        }
    }
}
