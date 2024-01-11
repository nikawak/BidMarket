using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidMarket.Migrations
{
    /// <inheritdoc />
    public partial class confirmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Lots",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Lots");
        }
    }
}
