using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIssuedQuantityToCutBatchItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssuedQuantity",
                table: "CutBatchItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedQuantity",
                table: "CutBatchItems");
        }
    }
}
