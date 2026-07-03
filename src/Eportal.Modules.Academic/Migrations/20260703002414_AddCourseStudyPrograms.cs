using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eportal.Modules.Academic.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseStudyPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseStudyPrograms",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    StudyProgramId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudyPrograms", x => new { x.CourseId, x.StudyProgramId });
                    table.ForeignKey(
                        name: "FK_CourseStudyPrograms_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudyPrograms_StudyPrograms_StudyProgramId",
                        column: x => x.StudyProgramId,
                        principalTable: "StudyPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudyPrograms_StudyProgramId",
                table: "CourseStudyPrograms",
                column: "StudyProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseStudyPrograms");
        }
    }
}
