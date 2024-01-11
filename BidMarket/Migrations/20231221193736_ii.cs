using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class ii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "Messages",
                newName: "RecieverId1");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RecieverId",
                table: "Messages",
                newName: "IX_Messages_RecieverId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId1",
                table: "Messages",
                column: "RecieverId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId1",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "RecieverId1",
                table: "Messages",
                newName: "RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RecieverId1",
                table: "Messages",
                newName: "IX_Messages_RecieverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_RecieverId",
                table: "Messages",
                column: "RecieverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
