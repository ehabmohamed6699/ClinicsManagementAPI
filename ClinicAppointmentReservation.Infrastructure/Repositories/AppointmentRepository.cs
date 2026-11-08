using ClinicAppointmentReservation.Domain.Exceptions;
using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class AppointmentRepository(AppDbContext context) : IAppointmentRepository
    {
        public async Task BookAppointment(FormAppointment appointment, string userId)
        {
            var patinet = await context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patinet == null) throw new PatientNotFoundException();
            context.Appointments.Add(new Appointment
            {
                AppointmentDate = appointment.AppointmentDate,
                PatientNotes = appointment.PatientNotes,
                PatientId = patinet.Id,
                DoctorId = appointment.DoctorId,
                ClinicId = appointment.ClinicId
            });
        }
    }
}
