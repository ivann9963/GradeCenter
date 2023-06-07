using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeCenter.Data.Migrations
{
    public partial class AddStatisticsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AverageRate = table.Column<double>(type: "float", nullable: false),
                    ComparedToLastWeek = table.Column<double>(type: "float", nullable: false),
                    ComparedToLastMonth = table.Column<double>(type: "float", nullable: false),
                    ComparedToLastYear = table.Column<double>(type: "float", nullable: false),
                    SchoolClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SchoolId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisciplineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatisticType = table.Column<int>(type: "int", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Statistics_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Statistics_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Statistics_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_Statistics_DisciplineId",
                table: "Statistics",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_SchoolClassId",
                table: "Statistics",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_SchoolId",
                table: "Statistics",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_TeacherId",
                table: "Statistics",
                column: "TeacherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistics");
        }
    }
}
