using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNest.Migrations
{
    /// <inheritdoc />
    public partial class AddingColumnbranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branch",
                table: "Books");
        }
    }
}
