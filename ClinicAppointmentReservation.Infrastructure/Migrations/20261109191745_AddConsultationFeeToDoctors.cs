using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicAppointmentReservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationFeeToDoctors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsultationFee",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Doctors");
        }
    }
}
