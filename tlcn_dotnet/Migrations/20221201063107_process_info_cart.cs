using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class process_info_cart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProcessAccountId",
                table: "Cart",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessDescription",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProcessAccountId",
                table: "Cart",
                column: "ProcessAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Account_ProcessAccountId",
                table: "Cart",
                column: "ProcessAccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Account_ProcessAccountId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_ProcessAccountId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "ProcessAccountId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "ProcessDescription",
                table: "Cart");
        }
    }
}
