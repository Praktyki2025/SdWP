using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SdWP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToValuationItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValuationItems_Valuations_ValuationId",
                table: "ValuationItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ValuationItems_Valuations_ValuationId",
                table: "ValuationItems",
                column: "ValuationId",
                principalTable: "Valuations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValuationItems_Valuations_ValuationId",
                table: "ValuationItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ValuationItems_Valuations_ValuationId",
                table: "ValuationItems",
                column: "ValuationId",
                principalTable: "Valuations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
