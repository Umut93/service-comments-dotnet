using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unik.Comments.Infrastructure.Migrations
{
    public partial class parent_misspell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParrentInternalId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "ParrentInternalId",
                table: "Comments",
                newName: "ParentInternalId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParrentInternalId",
                table: "Comments",
                newName: "IX_Comments_ParentInternalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentInternalId",
                table: "Comments",
                column: "ParentInternalId",
                principalTable: "Comments",
                principalColumn: "InternalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentInternalId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "ParentInternalId",
                table: "Comments",
                newName: "ParrentInternalId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentInternalId",
                table: "Comments",
                newName: "IX_Comments_ParrentInternalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParrentInternalId",
                table: "Comments",
                column: "ParrentInternalId",
                principalTable: "Comments",
                principalColumn: "InternalId");
        }
    }
}
