using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNest.Migrations
{
    /// <inheritdoc />
    public partial class BRANCHNAMEWASADDED : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Branch",
                table: "Books",
                newName: "BranchName");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "BranchName",
                table: "Books",
                newName: "Branch");
        }
    }
}
