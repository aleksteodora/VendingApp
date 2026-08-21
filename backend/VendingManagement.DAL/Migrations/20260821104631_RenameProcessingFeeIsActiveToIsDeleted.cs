using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendingManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameProcessingFeeIsActiveToIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ProcessingFees",
                newName: "IsDeleted");

            migrationBuilder.Sql("UPDATE \"ProcessingFees\" SET \"IsDeleted\" = NOT \"IsDeleted\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"ProcessingFees\" SET \"IsDeleted\" = NOT \"IsDeleted\";");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ProcessingFees",
                newName: "IsActive");
        }
    }
}