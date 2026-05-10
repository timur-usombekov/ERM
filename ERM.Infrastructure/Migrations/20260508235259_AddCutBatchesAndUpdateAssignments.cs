using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCutBatchesAndUpdateAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkAssignments_ClothingModels_ClothingModelId",
                table: "WorkAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedQuantity",
                table: "WorkAssignments");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "WorkAssignments");

            migrationBuilder.RenameColumn(
                name: "CompletedQuantity",
                table: "WorkAssignments",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "ClothingModelId",
                table: "WorkAssignments",
                newName: "CutBatchItemId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkAssignments_ClothingModelId",
                table: "WorkAssignments",
                newName: "IX_WorkAssignments_CutBatchItemId");

            migrationBuilder.CreateTable(
                name: "CutBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CutBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CutBatchItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CutBatchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClothingModelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CutBatchItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CutBatchItems_ClothingModels_ClothingModelId",
                        column: x => x.ClothingModelId,
                        principalTable: "ClothingModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CutBatchItems_CutBatches_CutBatchId",
                        column: x => x.CutBatchId,
                        principalTable: "CutBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkAssignments_AssignedDate",
                table: "WorkAssignments",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CutBatchItems_ClothingModelId",
                table: "CutBatchItems",
                column: "ClothingModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CutBatchItems_CutBatchId",
                table: "CutBatchItems",
                column: "CutBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkAssignments_CutBatchItems_CutBatchItemId",
                table: "WorkAssignments",
                column: "CutBatchItemId",
                principalTable: "CutBatchItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkAssignments_CutBatchItems_CutBatchItemId",
                table: "WorkAssignments");

            migrationBuilder.DropTable(
                name: "CutBatchItems");

            migrationBuilder.DropTable(
                name: "CutBatches");

            migrationBuilder.DropIndex(
                name: "IX_WorkAssignments_AssignedDate",
                table: "WorkAssignments");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "WorkAssignments",
                newName: "CompletedQuantity");

            migrationBuilder.RenameColumn(
                name: "CutBatchItemId",
                table: "WorkAssignments",
                newName: "ClothingModelId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkAssignments_CutBatchItemId",
                table: "WorkAssignments",
                newName: "IX_WorkAssignments_ClothingModelId");

            migrationBuilder.AddColumn<int>(
                name: "AssignedQuantity",
                table: "WorkAssignments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "WorkAssignments",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkAssignments_ClothingModels_ClothingModelId",
                table: "WorkAssignments",
                column: "ClothingModelId",
                principalTable: "ClothingModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
