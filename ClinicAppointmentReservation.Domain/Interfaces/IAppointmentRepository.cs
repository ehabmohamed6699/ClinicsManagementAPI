using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Records;
namespace ClinicAppointmentReservation.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Result<Appointment>> BookAppointment(FormAppointment appointment, string userId);
        Task<Result<AppointmentDTO>> GetById(int id, int patientId, int doctorId);
    }
}
