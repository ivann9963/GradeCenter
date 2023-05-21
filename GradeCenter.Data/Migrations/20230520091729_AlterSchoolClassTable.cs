using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeCenter.Data.Migrations
{
    public partial class AlterSchoolClassTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SchoolId",
                table: "SchoolClasses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_SchoolId",
                table: "SchoolClasses",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Schools_SchoolId",
                table: "SchoolClasses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Schools_SchoolId",
                table: "SchoolClasses");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClasses_SchoolId",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "SchoolClasses");
        }
    }
}
