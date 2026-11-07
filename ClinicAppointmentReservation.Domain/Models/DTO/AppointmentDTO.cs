using ClinicAppointmentReservation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Models.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }


        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public decimal Fee { get; set; }
        public string? PatientNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public AppointmentPatientDTO? Patient { get; set; } = null;

        public AppointmentDoctorDTO? Doctor { get; set; } = null;

        public AppointmentClinicDTO? Clinic { get; set; } = null;
    }

    public class AppointmentPatientDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

    public class AppointmentDoctorDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

    public class AppointmentClinicDTO
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
    }
}
