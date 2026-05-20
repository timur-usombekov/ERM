using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIronerAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IroningAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IroningAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CutBatchItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IronerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubstituteSeamstressId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AssignedDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    WeekNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IroningAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IroningAssignments_CutBatchItems_CutBatchItemId",
                        column: x => x.CutBatchItemId,
                        principalTable: "CutBatchItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IroningAssignments_Ironers_IronerId",
                        column: x => x.IronerId,
                        principalTable: "Ironers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IroningAssignments_Seamstresses_SubstituteSeamstressId",
                        column: x => x.SubstituteSeamstressId,
                        principalTable: "Seamstresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IroningAssignments_AssignedDate",
                table: "IroningAssignments",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_IroningAssignments_CutBatchItemId",
                table: "IroningAssignments",
                column: "CutBatchItemId");

            migrationBuilder.CreateIndex(
                name: "IX_IroningAssignments_IronerId",
                table: "IroningAssignments",
                column: "IronerId");

            migrationBuilder.CreateIndex(
                name: "IX_IroningAssignments_SubstituteSeamstressId",
                table: "IroningAssignments",
                column: "SubstituteSeamstressId");

            migrationBuilder.CreateIndex(
                name: "IX_IroningAssignments_WeekNumber_Year",
                table: "IroningAssignments",
                columns: new[] { "WeekNumber", "Year" });
        }
    }
}
