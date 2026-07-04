using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eportal.Modules.Exams.Migrations
{
    /// <inheritdoc />
    public partial class RestructureExamPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamRegistrations_ExamPeriods_ExamPeriodId",
                table: "ExamRegistrations");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "ExamPeriods");

            migrationBuilder.RenameColumn(
                name: "ExamPeriodId",
                table: "ExamRegistrations",
                newName: "ExamId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamRegistrations_ExamPeriodId_StudentId",
                table: "ExamRegistrations",
                newName: "IX_ExamRegistrations_ExamId_StudentId");

            migrationBuilder.RenameColumn(
                name: "ExamDate",
                table: "ExamPeriods",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ExamPeriods",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExamPeriodId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ExamDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_ExamPeriods_ExamPeriodId",
                        column: x => x.ExamPeriodId,
                        principalTable: "ExamPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamPeriodId",
                table: "Exams",
                column: "ExamPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRegistrations_Exams_ExamId",
                table: "ExamRegistrations",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamRegistrations_Exams_ExamId",
                table: "ExamRegistrations");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ExamPeriods");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "ExamRegistrations",
                newName: "ExamPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamRegistrations_ExamId_StudentId",
                table: "ExamRegistrations",
                newName: "IX_ExamRegistrations_ExamPeriodId_StudentId");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "ExamPeriods",
                newName: "ExamDate");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "ExamPeriods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRegistrations_ExamPeriods_ExamPeriodId",
                table: "ExamRegistrations",
                column: "ExamPeriodId",
                principalTable: "ExamPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
