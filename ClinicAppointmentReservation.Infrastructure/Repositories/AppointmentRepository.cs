using ClinicAppointmentReservation.Domain.Enums;
using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class AppointmentRepository(AppDbContext context, ResultBuilder builder) : IAppointmentRepository
    {
        private readonly int SlotMinutes = 30;
        private readonly int MaxBookingDays = 7;
        public async Task<Result<Appointment>> BookAppointment(FormAppointment appointment, string userId)
        {
            var now = DateTime.Now;
            if(appointment.AppointmentDate > now.AddDays(MaxBookingDays) || appointment.AppointmentDate <= now.AddMinutes(SlotMinutes))
                return builder.BadRequestResult<Appointment>($"Appointment date must be at least {SlotMinutes} minutes from now and at most {MaxBookingDays} days from now");
            var patinet = await context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patinet == null) 
                return builder.NotFoundResult<Appointment>($"User {userId} has no profile.");
            var doctor = await context.Doctors.FindAsync(appointment.DoctorId);
            if(doctor == null) 
                return builder.NotFoundResult<Appointment>($"Doctor {appointment.DoctorId} was not found.");
            if(!doctor!.IsApproved) 
                return builder.ConflictResult<Appointment>($"Doctor {appointment.DoctorId} is not approved.");
            var clinic = await context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == appointment.ClinicId);
            if(clinic == null) 
                return builder.NotFoundResult<Appointment>($"Clinic {appointment.ClinicId} was not found.");
            var doctorInClinic = clinic!.Doctors.Any(d => d.Id == appointment.DoctorId);
            if(!doctorInClinic) 
                return builder.ConflictResult<Appointment>("Doctor not in clinic.");
            var scheduled = await context.Appointments.AnyAsync(a => 
                !(a.AppointmentDate.AddMinutes(SlotMinutes) <= appointment.AppointmentDate || a.AppointmentDate >= appointment.AppointmentDate.AddMinutes(SlotMinutes))
                && (appointment.DoctorId == a.DoctorId || patinet.Id == a.PatientId)
                && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed)
            );
            if(scheduled)
                return builder.ConflictResult<Appointment>($"You cannot schedule an appointment at {appointment.AppointmentDate}.");
            var createdAppointment = new Appointment
            {
                AppointmentDate = appointment.AppointmentDate,
                PatientNotes = appointment.PatientNotes,
                Fee = doctor.ConsultationFee,
                PatientId = patinet!.Id,
                DoctorId = appointment.DoctorId,
                ClinicId = appointment.ClinicId
            };
            context.Appointments.Add(createdAppointment);
            return builder.CreatedResult<Appointment>(createdAppointment);
        }
    }
}
