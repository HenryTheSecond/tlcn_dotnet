using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class remove_productId_cartId_unique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartDetail_ProductId_CartId",
                table: "CartDetail");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_ProductId",
                table: "CartDetail",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartDetail_ProductId",
                table: "CartDetail");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_ProductId_CartId",
                table: "CartDetail",
                columns: new[] { "ProductId", "CartId" },
                unique: true,
                filter: "[ProductId] IS NOT NULL AND [CartId] IS NOT NULL");
        }
    }
}
