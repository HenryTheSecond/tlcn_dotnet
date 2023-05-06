using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_index_review_resource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewResource_ReviewId",
                table: "ReviewResource");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResource_ReviewId_Type",
                table: "ReviewResource",
                columns: new[] { "ReviewId", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewResource_ReviewId_Type",
                table: "ReviewResource");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResource_ReviewId",
                table: "ReviewResource",
                column: "ReviewId");
        }
    }
}
