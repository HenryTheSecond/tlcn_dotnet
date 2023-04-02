using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_Cart_column_CartNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CartId",
                table: "CartNotification",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartNotification_CartId",
                table: "CartNotification",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartNotification_Cart_CartId",
                table: "CartNotification",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartNotification_Cart_CartId",
                table: "CartNotification");

            migrationBuilder.DropIndex(
                name: "IX_CartNotification_CartId",
                table: "CartNotification");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "CartNotification");
        }
    }
}
