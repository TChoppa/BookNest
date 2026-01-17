using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookNest.Migrations
{
    /// <inheritdoc />
    public partial class TableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Admin",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "Fk_RoleId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RoleMasters",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMasters", x => x.RoleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_Fk_RoleId",
                table: "users",
                column: "Fk_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_RoleMasters_Fk_RoleId",
                table: "users",
                column: "Fk_RoleId",
                principalTable: "RoleMasters",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_RoleMasters_Fk_RoleId",
                table: "users");

            migrationBuilder.DropTable(
                name: "RoleMasters");

            migrationBuilder.DropIndex(
                name: "IX_users_Fk_RoleId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Fk_RoleId",
                table: "users");

            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
