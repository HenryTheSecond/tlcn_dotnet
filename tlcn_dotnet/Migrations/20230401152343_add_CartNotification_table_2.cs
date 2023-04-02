using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tlcn_dotnet.Migrations
{
    public partial class add_CartNotification_table_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartNotification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartNotification_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartNotification_AccountId",
                table: "CartNotification",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartNotification");
        }
    }
}
