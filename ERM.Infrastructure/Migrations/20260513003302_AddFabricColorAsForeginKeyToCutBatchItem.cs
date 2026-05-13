using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFabricColorAsForeginKeyToCutBatchItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "CutBatchItems");

            migrationBuilder.AddColumn<Guid>(
                name: "FabricColorId",
                table: "CutBatchItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CutBatchItems_FabricColorId",
                table: "CutBatchItems",
                column: "FabricColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CutBatchItems_FabricColors_FabricColorId",
                table: "CutBatchItems",
                column: "FabricColorId",
                principalTable: "FabricColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CutBatchItems_FabricColors_FabricColorId",
                table: "CutBatchItems");

            migrationBuilder.DropIndex(
                name: "IX_CutBatchItems_FabricColorId",
                table: "CutBatchItems");

            migrationBuilder.DropColumn(
                name: "FabricColorId",
                table: "CutBatchItems");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "CutBatchItems",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
