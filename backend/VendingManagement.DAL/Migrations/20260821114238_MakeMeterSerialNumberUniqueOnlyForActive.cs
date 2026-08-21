using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendingManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeMeterSerialNumberUniqueOnlyForActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meters_MeterSerialNumber",
                table: "Meters");

            migrationBuilder.CreateIndex(
                name: "IX_Meters_MeterSerialNumber",
                table: "Meters",
                column: "MeterSerialNumber",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meters_MeterSerialNumber",
                table: "Meters");

            migrationBuilder.CreateIndex(
                name: "IX_Meters_MeterSerialNumber",
                table: "Meters",
                column: "MeterSerialNumber",
                unique: true);
        }
    }
}
