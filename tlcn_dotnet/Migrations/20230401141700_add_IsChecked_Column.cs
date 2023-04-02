using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_IsChecked_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Inventory",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Inventory");
        }
    }
}
