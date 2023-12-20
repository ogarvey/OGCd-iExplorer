using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OGCdiExplorer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CdiTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    BasePath = table.Column<string>(type: "TEXT", nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", nullable: false),
                    DevelopmentCompany = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdiTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Developers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CdiFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<string>(type: "TEXT", nullable: false),
                    FileHash = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CdiTitleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdiFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CdiFile_CdiTitles_CdiTitleId",
                        column: x => x.CdiTitleId,
                        principalTable: "CdiTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CdiTitleDeveloper",
                columns: table => new
                {
                    CdiTitlesId = table.Column<int>(type: "INTEGER", nullable: false),
                    DevelopersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdiTitleDeveloper", x => new { x.CdiTitlesId, x.DevelopersId });
                    table.ForeignKey(
                        name: "FK_CdiTitleDeveloper_CdiTitles_CdiTitlesId",
                        column: x => x.CdiTitlesId,
                        principalTable: "CdiTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CdiTitleDeveloper_Developers_DevelopersId",
                        column: x => x.DevelopersId,
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CdiFile_CdiTitleId",
                table: "CdiFile",
                column: "CdiTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_CdiTitleDeveloper_DevelopersId",
                table: "CdiTitleDeveloper",
                column: "DevelopersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CdiFile");

            migrationBuilder.DropTable(
                name: "CdiTitleDeveloper");

            migrationBuilder.DropTable(
                name: "CdiTitles");

            migrationBuilder.DropTable(
                name: "Developers");
        }
    }
}
