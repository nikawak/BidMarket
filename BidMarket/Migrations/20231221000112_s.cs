using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class s : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Lots");

            migrationBuilder.AddColumn<Guid>(
                name: "LotId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_LotId",
                table: "Categories",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Lots_LotId",
                table: "Categories",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Lots_LotId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_LotId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "Categories");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Lots",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
