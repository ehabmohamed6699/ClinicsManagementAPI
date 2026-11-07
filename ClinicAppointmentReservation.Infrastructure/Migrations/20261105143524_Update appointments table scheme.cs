using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppointmentReservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updateappointmentstablescheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInMinutes",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Appointments",
                newName: "Fee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fee",
                table: "Appointments",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "DurationInMinutes",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
