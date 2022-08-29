using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unik.Comments.Infrastructure.Migrations
{
    public partial class cascade_delete_content : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_CommentsContent_ContentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ContentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ContentId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "CommentsContent",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CommentsContent_CommentId",
                table: "CommentsContent",
                column: "CommentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsContent_Comments_CommentId",
                table: "CommentsContent",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "InternalId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsContent_Comments_CommentId",
                table: "CommentsContent");

            migrationBuilder.DropIndex(
                name: "IX_CommentsContent_CommentId",
                table: "CommentsContent");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "CommentsContent");

            migrationBuilder.AddColumn<int>(
                name: "ContentId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ContentId",
                table: "Comments",
                column: "ContentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_CommentsContent_ContentId",
                table: "Comments",
                column: "ContentId",
                principalTable: "CommentsContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
