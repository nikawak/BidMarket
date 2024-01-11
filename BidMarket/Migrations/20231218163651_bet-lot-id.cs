using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class betlotid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Lots_LotId1",
                table: "Bets");

            migrationBuilder.RenameColumn(
                name: "LotId1",
                table: "Bets",
                newName: "LotId");

            migrationBuilder.RenameIndex(
                name: "IX_Bets_LotId1",
                table: "Bets",
                newName: "IX_Bets_LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Lots_LotId",
                table: "Bets",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Lots_LotId",
                table: "Bets");

            migrationBuilder.RenameColumn(
                name: "LotId",
                table: "Bets",
                newName: "LotId1");

            migrationBuilder.RenameIndex(
                name: "IX_Bets_LotId",
                table: "Bets",
                newName: "IX_Bets_LotId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Lots_LotId1",
                table: "Bets",
                column: "LotId1",
                principalTable: "Lots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
