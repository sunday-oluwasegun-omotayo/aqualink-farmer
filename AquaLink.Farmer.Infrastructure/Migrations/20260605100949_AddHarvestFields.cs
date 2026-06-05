using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquaLink.Farmer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHarvestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "HarvestedAt",
                table: "FarmCycles",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HarvestedWeightKg",
                table: "FarmCycles",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePricePerKg",
                table: "FarmCycles",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HarvestedAt",
                table: "FarmCycles");

            migrationBuilder.DropColumn(
                name: "HarvestedWeightKg",
                table: "FarmCycles");

            migrationBuilder.DropColumn(
                name: "SalePricePerKg",
                table: "FarmCycles");
        }
    }
}
