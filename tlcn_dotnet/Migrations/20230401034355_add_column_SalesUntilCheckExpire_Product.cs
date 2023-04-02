using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_column_SalesUntilCheckExpire_Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SalesUntilCheckExpire",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesUntilCheckExpire",
                table: "Product");
        }
    }
}
