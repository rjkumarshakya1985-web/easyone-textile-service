using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Textile.Core.Infrastructure.Migrations
{
    public partial class AddStickerPrintFieldSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StickerPrintFieldSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StickerPrintSettingId = table.Column<int>(type: "int", nullable: false),
                    FieldKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    X = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Y = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    FontSize = table.Column<int>(type: "int", nullable: false),
                    FontWeight = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TextAlign = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StickerPrintFieldSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StickerPrintFieldSettings_StickerPrintSettings_StickerPrintSettingId",
                        column: x => x.StickerPrintSettingId,
                        principalTable: "StickerPrintSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StickerPrintFieldSettings_StickerPrintSettingId_FieldKey",
                table: "StickerPrintFieldSettings",
                columns: new[] { "StickerPrintSettingId", "FieldKey" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StickerPrintFieldSettings");
        }
    }
}
