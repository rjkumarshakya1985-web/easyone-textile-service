using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Textile.Core.Infrastructure.Migrations
{
    public partial class AddDeveloperMenuSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeveloper",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AdminMenuSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMenuSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminMenuSettings_MenuKey",
                table: "AdminMenuSettings",
                column: "MenuKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AdminMenuSettings");

            migrationBuilder.DropColumn(
                name: "IsDeveloper",
                table: "Users");
        }
    }
}
