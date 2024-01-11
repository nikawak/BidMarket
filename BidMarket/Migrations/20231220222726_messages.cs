using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_RecieverId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_SenderId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Reviews",
                newName: "SenderId1");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "Reviews",
                newName: "RecieverId1");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_SenderId",
                table: "Reviews",
                newName: "IX_Reviews_SenderId1");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RecieverId",
                table: "Reviews",
                newName: "IX_Reviews_RecieverId1");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NonceMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_RecieverId1",
                table: "Reviews",
                column: "RecieverId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_SenderId1",
                table: "Reviews",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_RecieverId1",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_SenderId1",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.RenameColumn(
                name: "SenderId1",
                table: "Reviews",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "RecieverId1",
                table: "Reviews",
                newName: "RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_SenderId1",
                table: "Reviews",
                newName: "IX_Reviews_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RecieverId1",
                table: "Reviews",
                newName: "IX_Reviews_RecieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_RecieverId",
                table: "Reviews",
                column: "RecieverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_SenderId",
                table: "Reviews",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
