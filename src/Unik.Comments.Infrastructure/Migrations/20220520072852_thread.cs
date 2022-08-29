using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unik.Comments.Infrastructure.Migrations
{
    public partial class thread : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThreadId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Comments");
        }
    }
}
