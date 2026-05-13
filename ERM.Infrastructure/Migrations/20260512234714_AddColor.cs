using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FabricColors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FabricColors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FabricColors_Name",
                table: "FabricColors",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FabricColors");
        }
    }
}
