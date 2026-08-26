using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendingManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAdminIsSuperAdminToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Admins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE \"Admins\" SET \"Role\" = CASE WHEN \"IsSuperAdmin\" = true THEN 1 ELSE 0 END;");

            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "Admins");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "Admins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                "UPDATE \"Admins\" SET \"IsSuperAdmin\" = CASE WHEN \"Role\" = 1 THEN true ELSE false END;");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Admins");
        }
    }
}