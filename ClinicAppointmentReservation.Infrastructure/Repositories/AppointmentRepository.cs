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
            var doctor = await context.Doctors.FindAsync(appointment.DoctorId);
            if(doctor == null) throw new DoctorNotFoundException();
            if(!doctor.IsApproved) throw new DoctorHasNoApprovedProfileException();
            context.Appointments.Add(new Appointment
            {
                AppointmentDate = appointment.AppointmentDate,
                PatientNotes = appointment.PatientNotes,
                Fee = doctor.ConsultationFee,
                PatientId = patinet.Id,
                DoctorId = appointment.DoctorId,
                ClinicId = appointment.ClinicId
            });
        }
    }
}
