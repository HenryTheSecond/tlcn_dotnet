using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_city_code_supplier_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "detailLocation",
                table: "Supplier",
                newName: "DetailLocation");

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "Supplier");

            migrationBuilder.RenameColumn(
                name: "DetailLocation",
                table: "Supplier",
                newName: "detailLocation");
        }
    }
}
