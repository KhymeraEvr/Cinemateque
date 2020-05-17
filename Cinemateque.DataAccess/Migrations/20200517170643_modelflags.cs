using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinemateque.DataAccess.Migrations
{
    public partial class modelflags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompaniesFlags",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenresFlags",
                table: "Movies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompaniesFlags",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "GenresFlags",
                table: "Movies");
        }
    }
}
