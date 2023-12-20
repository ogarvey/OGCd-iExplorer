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
                    ReleaseYear = table.Column<string>(type: "TEXT", nullable: false),
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
                name: "CdiFiles",
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
                    table.PrimaryKey("PK_CdiFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CdiFiles_CdiTitles_CdiTitleId",
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

            migrationBuilder.CreateTable(
                name: "CdiSectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SectorIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    SectorType = table.Column<int>(type: "INTEGER", nullable: false),
                    FileNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Channel = table.Column<int>(type: "INTEGER", nullable: false),
                    CdiFileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CdiSectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CdiSectors_CdiFiles_CdiFileId",
                        column: x => x.CdiFileId,
                        principalTable: "CdiFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CdiFiles_CdiTitleId",
                table: "CdiFiles",
                column: "CdiTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_CdiSectors_CdiFileId",
                table: "CdiSectors",
                column: "CdiFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CdiTitleDeveloper_DevelopersId",
                table: "CdiTitleDeveloper",
                column: "DevelopersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CdiSectors");

            migrationBuilder.DropTable(
                name: "CdiTitleDeveloper");

            migrationBuilder.DropTable(
                name: "CdiFiles");

            migrationBuilder.DropTable(
                name: "Developers");

            migrationBuilder.DropTable(
                name: "CdiTitles");
        }
    }
}
