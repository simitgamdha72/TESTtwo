using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookManagement.Migrations
{
    /// <inheritdoc />
    public partial class five : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "UserIssuedBooks_bookid_by_fkey",
                table: "UserIssuedBooks");

            migrationBuilder.DropForeignKey(
                name: "UserIssuedBooks_userid_by_fkey",
                table: "UserIssuedBooks");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserIssuedBooks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_UserIssuedBooks_BookId",
                table: "UserIssuedBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIssuedBooks_UserId",
                table: "UserIssuedBooks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "UserIssuedBooks_bookid_by_fkey",
                table: "UserIssuedBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "UserIssuedBooks_userid_by_fkey",
                table: "UserIssuedBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "UserIssuedBooks_bookid_by_fkey",
                table: "UserIssuedBooks");

            migrationBuilder.DropForeignKey(
                name: "UserIssuedBooks_userid_by_fkey",
                table: "UserIssuedBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserIssuedBooks_BookId",
                table: "UserIssuedBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserIssuedBooks_UserId",
                table: "UserIssuedBooks");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserIssuedBooks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "UserIssuedBooks_bookid_by_fkey",
                table: "UserIssuedBooks",
                column: "Id",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "UserIssuedBooks_userid_by_fkey",
                table: "UserIssuedBooks",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
