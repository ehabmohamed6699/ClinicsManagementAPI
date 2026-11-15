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

        public AppointmentDTO(){}
        public AppointmentDTO(Appointment a)
        {
            Id = a.Id;
            AppointmentDate = a.AppointmentDate;
            Fee = a.Fee;
            Status = a.Status;
            PatientNotes = a.PatientNotes;
            CreatedAt = a.CreatedAt;
            Patient = new AppointmentPatientDTO(a.Patient);

            Doctor = new AppointmentDoctorDTO(a.Doctor);

            Clinic = new AppointmentClinicDTO(a.Clinic);
        }
    }

    public class AppointmentPatientDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }

        public AppointmentPatientDTO(){}
        public AppointmentPatientDTO(Patient p)
        {
            Id = p.Id;
            Email = p.User.Email;
            Name = p.User.Name;
        }
    }

    public class AppointmentDoctorDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public AppointmentDoctorDTO(){}
        public AppointmentDoctorDTO(Doctor d)
        {
            Id = d.Id;
            Email = d.User.Email;
            Name = d.User.Name;
        }
    }

    public class AppointmentClinicDTO
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address {get; set;}
        public string? Name { get; set; }
        public AppointmentClinicDTO(){}
        public AppointmentClinicDTO(Clinic c)
        {
            Id = c.Id;
            Name = c.Name;
            Address = c.Address;
            PhoneNumber = c.PhoneNumber;
        }
    }
}
