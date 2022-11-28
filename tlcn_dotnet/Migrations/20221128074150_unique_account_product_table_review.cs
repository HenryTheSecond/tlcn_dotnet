using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class unique_account_product_table_review : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_ProductId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProductId_AccountId",
                table: "Review",
                columns: new[] { "ProductId", "AccountId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_ProductId_AccountId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProductId",
                table: "Review",
                column: "ProductId");
        }
    }
}
