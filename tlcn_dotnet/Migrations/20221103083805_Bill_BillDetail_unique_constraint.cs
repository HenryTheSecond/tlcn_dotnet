using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class Bill_BillDetail_unique_constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BillDetail_BillId_ProductId",
                table: "BillDetail");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetail_BillId_ProductId",
                table: "BillDetail",
                columns: new[] { "BillId", "ProductId" },
                unique: true,
                filter: "[BillId] IS NOT NULL AND [ProductId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BillDetail_BillId_ProductId",
                table: "BillDetail");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetail_BillId_ProductId",
                table: "BillDetail",
                columns: new[] { "BillId", "ProductId" });
        }
    }
}
