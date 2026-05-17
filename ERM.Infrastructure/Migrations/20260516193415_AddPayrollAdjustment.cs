using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayrollAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SeamstressId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollAdjustments_Seamstresses_SeamstressId",
                        column: x => x.SeamstressId,
                        principalTable: "Seamstresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollAdjustments_Date",
                table: "PayrollAdjustments",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollAdjustments_SeamstressId",
                table: "PayrollAdjustments",
                column: "SeamstressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayrollAdjustments");
        }
    }
}
