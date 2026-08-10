using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Textile.Core.Infrastructure.Migrations
{
    public partial class AddStickerPrintSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StickerPrintSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowSupplierCode = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowCompanyShortName = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowWholeSaleRate = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowProductName = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowPrintDate = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowRetailRate = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowBarcode = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowBarcodeText = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CompanyShortName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "SSBD"),
                    ApplyWholeSaleRateFormula = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    WholeSaleRatePrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "5"),
                    WholeSaleRatePostfix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WholeSaleRateAddAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 500m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StickerPrintSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "StickerPrintSettings",
                columns: new[]
                {
                    "ShowSupplierCode",
                    "ShowCompanyShortName",
                    "ShowWholeSaleRate",
                    "ShowProductName",
                    "ShowPrintDate",
                    "ShowRetailRate",
                    "ShowBarcode",
                    "ShowBarcodeText",
                    "CompanyShortName",
                    "ApplyWholeSaleRateFormula",
                    "WholeSaleRatePrefix",
                    "WholeSaleRatePostfix",
                    "WholeSaleRateAddAmount"
                },
                values: new object[]
                {
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    "SSBD",
                    true,
                    "5",
                    null,
                    500m
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StickerPrintSettings");
        }
    }
}
