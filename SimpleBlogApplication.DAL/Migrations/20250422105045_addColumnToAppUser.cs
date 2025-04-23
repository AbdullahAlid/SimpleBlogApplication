using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBlogApplication.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addColumnToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValidityStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidityStatus",
                table: "AspNetUsers");
        }
    }
}
