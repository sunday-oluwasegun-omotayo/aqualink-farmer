using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquaLink.Prices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PricesInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FarmerAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FarmerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MessageSent = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    AlertDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmerAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceIndexes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Market = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Commodity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PriceNairaPerKg = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: false),
                    SubmittedByAgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceIndexes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FarmerAlerts_FarmerId_AlertDate",
                table: "FarmerAlerts",
                columns: new[] { "FarmerId", "AlertDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndexes_Market_Commodity_PriceDate",
                table: "PriceIndexes",
                columns: new[] { "Market", "Commodity", "PriceDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmerAlerts");

            migrationBuilder.DropTable(
                name: "PriceIndexes");
        }
    }
}
