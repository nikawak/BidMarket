using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class nlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CategoryLot",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LotsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryLot", x => new { x.CategoriesId, x.LotsId });
                    table.ForeignKey(
                        name: "FK_CategoryLot_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryLot_Lots_LotsId",
                        column: x => x.LotsId,
                        principalTable: "Lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLot_LotsId",
                table: "CategoryLot",
                column: "LotsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryLot");

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
    }
}
