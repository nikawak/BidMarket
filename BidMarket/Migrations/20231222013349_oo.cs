using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class oo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryLot_Categories_CategoriesId",
                table: "CategoryLot");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryLot_Lots_LotsId",
                table: "CategoryLot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryLot",
                table: "CategoryLot");

            migrationBuilder.RenameColumn(
                name: "LotsId",
                table: "CategoryLot",
                newName: "LotId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryLot",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryLot_LotsId",
                table: "CategoryLot",
                newName: "IX_CategoryLot_LotId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CategoryLot",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryLot",
                table: "CategoryLot",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLot_CategoryId",
                table: "CategoryLot",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryLot_Categories_CategoryId",
                table: "CategoryLot",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryLot_Lots_LotId",
                table: "CategoryLot",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryLot_Categories_CategoryId",
                table: "CategoryLot");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryLot_Lots_LotId",
                table: "CategoryLot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryLot",
                table: "CategoryLot");

            migrationBuilder.DropIndex(
                name: "IX_CategoryLot_CategoryId",
                table: "CategoryLot");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CategoryLot");

            migrationBuilder.RenameColumn(
                name: "LotId",
                table: "CategoryLot",
                newName: "LotsId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "CategoryLot",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryLot_LotId",
                table: "CategoryLot",
                newName: "IX_CategoryLot_LotsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryLot",
                table: "CategoryLot",
                columns: new[] { "CategoriesId", "LotsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryLot_Categories_CategoriesId",
                table: "CategoryLot",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryLot_Lots_LotsId",
                table: "CategoryLot",
                column: "LotsId",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
