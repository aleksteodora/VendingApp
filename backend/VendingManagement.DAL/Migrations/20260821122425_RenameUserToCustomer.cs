using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendingManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meters_Users_UserId",
                table: "Meters");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ApiKey",
                table: "Customers",
                newName: "IX_Customers_ApiKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Meters_Customers_UserId",
                table: "Meters",
                column: "UserId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meters_Customers_UserId",
                table: "Meters");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_ApiKey",
                table: "Users",
                newName: "IX_Users_ApiKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Meters_Users_UserId",
                table: "Meters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}