using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinemateque.DataAccess.Migrations
{
    public partial class movieModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "CrewMembers");

            migrationBuilder.CreateTable(
                name: "CrewRatingEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrewMemberId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewRatingEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrewRatingEntry_CrewMembers_CrewMemberId",
                        column: x => x.CrewMemberId,
                        principalTable: "CrewMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    ActorsCsvs = table.Column<string>(nullable: true),
                    ActorsPopularity = table.Column<string>(nullable: true),
                    CrewCsvs = table.Column<string>(nullable: true),
                    CrewPopularity = table.Column<string>(nullable: true),
                    Genres = table.Column<string>(nullable: true),
                    Budget = table.Column<double>(nullable: false),
                    Companies = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrewRatingEntry_CrewMemberId",
                table: "CrewRatingEntry",
                column: "CrewMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrewRatingEntry");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "CrewMembers",
                nullable: true);
        }
    }
}
