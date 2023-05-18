using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_gift_cart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GiftCartId",
                table: "CartDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetail_GiftCartId",
                table: "CartDetail",
                column: "GiftCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetail_GiftCart_GiftCartId",
                table: "CartDetail",
                column: "GiftCartId",
                principalTable: "GiftCart",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetail_GiftCart_GiftCartId",
                table: "CartDetail");

            migrationBuilder.DropIndex(
                name: "IX_CartDetail_GiftCartId",
                table: "CartDetail");

            migrationBuilder.DropColumn(
                name: "GiftCartId",
                table: "CartDetail");
        }
    }
}
