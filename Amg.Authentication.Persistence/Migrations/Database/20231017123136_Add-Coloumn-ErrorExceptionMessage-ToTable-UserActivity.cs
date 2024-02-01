using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amg.Authentication.Persistence.Migrations.Database
{
    public partial class AddColoumnErrorExceptionMessageToTableUserActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorExceptionMessage",
                table: "UserActivities",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorExceptionMessage",
                table: "UserActivities");
        }
    }
}
