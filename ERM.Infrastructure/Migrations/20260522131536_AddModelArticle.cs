using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "ClothingModels",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "ClothingModels");
        }
    }
}
