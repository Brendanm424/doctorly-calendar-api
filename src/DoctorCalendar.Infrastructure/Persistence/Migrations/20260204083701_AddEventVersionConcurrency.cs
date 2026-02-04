using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorCalendar.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventVersionConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CalendarEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "CalendarEvents",
                type: "BLOB",
                rowVersion: true,
                nullable: true);
        }
    }
}
