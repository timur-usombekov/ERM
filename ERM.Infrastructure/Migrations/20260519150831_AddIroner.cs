using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIroner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "IroningPrice",
                table: "ClothingModels",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Ironers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ironers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ironers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IroningAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IronerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubstituteSeamstressId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CutBatchItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignedDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
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
                name: "IX_Ironers_EmployeeId",
                table: "Ironers",
                column: "EmployeeId",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IroningAssignments");

            migrationBuilder.DropTable(
                name: "Ironers");

            migrationBuilder.DropColumn(
                name: "IroningPrice",
                table: "ClothingModels");
        }
    }
}
